/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers.SaveFiles
{
    /// <summary>
    /// Extension for saving images to a file system folder.
    /// </summary>
    public class SaveImages : TransformerBase, Extensions.ISpecial
    {
        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Publish geocache images"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get { return Transformers.ExecutionOrder.Publish + 1; }
        }

        /// <summary>
        /// The delegate that is used to generate full path where to save each image.
        /// If the returned path includes a placeholder <c>{0}</c> it is filled with the title of the image.
        /// </summary>
        private Func<Gpx.GpxWaypoint, Gpx.GeocacheImage, string> _generatePath;

        /// <summary>
        /// Holds a list of accepted extensions. Reset before each cycle.
        /// </summary>
        private HashSet<string> _validExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Holds a list of file names that were copied to the target location.
        /// </summary>
        private HashSet<string> _usedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Holds the default extension used when the image has to be converted.
        /// </summary>
        private string _defaultExtension;

        /// <summary>
        /// Holds the WebClient that is used to download the images.
        /// </summary>
        private System.Net.WebClient _webClient;

        /// <summary>
        /// The path to the local storage folder.
        /// </summary>
        private string _localStorage;

        /// <summary>
        /// Gets or the list of formats that are supported for publishing. The first format is used as target for any conversion
        /// if the loaded image is not one of these. The default is to support only JPG images.
        /// </summary>
        public List<ImageFormat> AcceptedFormats { get; private set; }

        /// <summary>
        /// Gets or sets the encoder that is used to convert the image if it's extension is not one of <see cref="AcceptedFormats"/>.
        /// Default is to use JPEG.
        /// </summary>
        public ImageCodecInfo DefaultEncoder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that every image has to be encoded using <see cref="DefaultEncoder"/> even
        /// if the source format is one of <see cref="AcceptedFormats"/>. This should be used only when it is known
        /// that the target device has problems with correctly processing images.
        /// The conversion also removes all metadata from the image.
        /// </summary>
        public bool EncodeEverything { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if existing images that are no longer needed are removed.
        /// If this is set to <c>true</c>, <see cref="ImageRootPath"/> property must be set.
        /// </summary>
        public bool RemoveObsoleteImages { get; set; }

        /// <summary>
        /// Gets or sets a value if the log images will be published.
        /// </summary>
        public bool PublishLogImages { get; set; }

        /// <summary>
        /// Gets or sets the folder that will be cleaned when <see cref="RemoveObsoleteImages"/> is <c>true</c>.
        /// The path itself will be removed as well if it is empty. To avoid this, end the path with path separator character.
        /// </summary>
        public string ImageRootPath { get; set; }

        /// <summary>
        /// Gets or sets the maximum size for published images. Anything larger than this will be resized to fit the rectangle.
        /// </summary>
        public System.Drawing.Size? MaximumSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveImages"/> class.
        /// </summary>
        /// <param name="generatePath">The delegate that is used to generate full path where to save each image. 
        /// If the returned path includes a placeholder <c>{0}</c> it is filled with the title of the image.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="generatePath"/> is <c>null</c></exception>
        public SaveImages(Func<Gpx.GpxWaypoint, Gpx.GeocacheImage, string> generatePath)
        {
            if (generatePath == null)
                throw new ArgumentNullException("generatePath");

            this._generatePath = generatePath;

            this.AcceptedFormats = new List<System.Drawing.Imaging.ImageFormat>() { System.Drawing.Imaging.ImageFormat.Jpeg };
            this.DefaultEncoder = ImageCodecInfo.GetImageEncoders().First(o => o.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);
            this._localStorage = Extensions.ExtensionLoader.GetLocalStoragePath(typeof(SaveImages));
        }

        /// <summary>
        /// Creates the full path where to store the given image.
        /// </summary>
        /// <param name="waypoint">The waypoint that contains the image.</param>
        /// <param name="image">The image data.</param>
        /// <returns>The unique path where to store the image.</returns>
        private string CreateFullPath(Gpx.GpxWaypoint waypoint, Gpx.GeocacheImage image)
        {
            var path = this._generatePath(waypoint, image);
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("The path generator returned an empty path.");

            var t = image.Title;
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                t = t.Replace(c, '_');

            path = path.Replace("{0}", t);

            var ext = System.IO.Path.GetExtension(path);
            if (!this._validExtensions.Contains(ext))
            {
                path = path.Substring(0, path.Length - ext.Length) + this._defaultExtension;
                ext = this._defaultExtension;
            }

            var uniquePath = path;
            var i = 0;
            while (this._usedPaths.Contains(uniquePath))
            {
                i++;
                uniquePath = path.Substring(0, path.Length - ext.Length) + " [" + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]" + this._defaultExtension;
            }

            this._usedPaths.Add(uniquePath);
            return uniquePath;
        }

        /// <summary>
        /// Downloads the given image when needed and saves it in the local cache. Returns the path to 
        /// </summary>
        /// <param name="waypoint"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        private string DownloadImage(Gpx.GpxWaypoint waypoint, Gpx.GeocacheImage image)
        {
            var code = waypoint.Name;

            var lastChar = code.Length > 2 ? code[code.Length - 1].ToString() : "0";
            var lastChar2 = code.Length > 3 ? code[code.Length - 2].ToString() : "0";

            var fname = image.Address.LocalPath;
            foreach (var x in System.IO.Path.GetInvalidFileNameChars())
                fname = fname.Replace(x, '_');

            var path = System.IO.Path.Combine(this._localStorage, lastChar, lastChar2, code);
            System.IO.Directory.CreateDirectory(path);
            var filePath = System.IO.Path.Combine(path, fname);

            if (!System.IO.File.Exists(filePath))
            {
                this.ReportStatus("Downloading image '{0}' for cache {1}.", image.Title, waypoint.Geocache.Name);
                this._webClient.DownloadFile(image.Address, filePath);
            }

            return filePath;
        }

        /// <summary>
        /// Determines if the given <paramref name="image"/> requires conversion for the current publish settings.
        /// </summary>
        private bool ConvertNeeded(System.Drawing.Image image)
        {
            if (!this.AcceptedFormats.Contains(image.RawFormat))
                return true;

            if (this.MaximumSize.HasValue && (image.Width > this.MaximumSize.Value.Width || image.Height > this.MaximumSize.Value.Height))
                return true;

            return false;
        }

        /// <summary>
        /// Determines if the converted image is up-to-date (it could be out-of-date if previously it was resized to smaller
        /// version than is allowed now).
        /// </summary>
        private bool ReconvertNeeded(System.Drawing.Image original, System.Drawing.Image converted)
        {
            if (converted.Width < original.Width || converted.Height < original.Height)
            {
                // the converted image has been resized but it is smaller than the current maximum (-1 to compensate for any rounding issues)
                if (!this.MaximumSize.HasValue || (converted.Width < this.MaximumSize.Value.Width - 1 && converted.Height < this.MaximumSize.Value.Height - 1))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Converts the given <paramref name="image"/> and saves it to <paramref name="targetPath"/>.
        /// </summary>
        private void ConvertImage(System.Drawing.Image image, string targetPath)
        {
            var maxSize = this.MaximumSize ?? new System.Drawing.Size(image.Width, image.Height);

            var x = (double)maxSize.Width / (double)image.Width;
            var y = (double)maxSize.Height / (double)image.Height;

            var ratio = x > y ? y : x;
            int xn, yn;
            if (ratio < 1)
            {
                xn = (int)(image.Width * ratio);
                yn = (int)(image.Height * ratio);
                if (xn == 0) xn = 1;
                if (yn == 0) yn = 1;
            }
            else
            {
                xn = image.Width;
                yn = image.Height;
            }

            using (var imageCopy = new System.Drawing.Bitmap(xn, yn, image.PixelFormat))
            using (var graphics = System.Drawing.Graphics.FromImage(imageCopy))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.DrawImage(image,
                    new System.Drawing.Rectangle(0, 0, imageCopy.Width, imageCopy.Height),
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    System.Drawing.GraphicsUnit.Pixel);
                graphics.Flush();

                EncoderParameters encParams;
                if (this.DefaultEncoder.FormatID == ImageFormat.Jpeg.Guid)
                {
                    encParams = new EncoderParameters(1);

                    // for thumbnail sized images use higher quality.
                    long quality = (imageCopy.Width > 200 && imageCopy.Height > 200) ? 95 : 90;
                    encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                }
                else
                {
                    encParams = new EncoderParameters(0);
                }

                imageCopy.Save(targetPath, this.DefaultEncoder, encParams);
            }
        }

        /// <summary>
        /// Converts (if needed) the image at the given path and returns the file path that should be used 
        /// (can be both the original or a different file if conversion was needed).
        /// </summary>
        private string ConvertImage(string filePath)
        {
            var convertedFilePath = filePath.Substring(0, filePath.Length - System.IO.Path.GetExtension(filePath).Length) + ".converted" + this._defaultExtension;

            using (var bitmap = System.Drawing.Bitmap.FromFile(filePath))
            {
                if (!this.EncodeEverything && !this.ConvertNeeded(bitmap))
                    return filePath;

                if (System.IO.File.Exists(convertedFilePath))
                {
                    using (var convertedBitmap = System.Drawing.Bitmap.FromFile(convertedFilePath))
                    {
                        if (!this.ConvertNeeded(convertedBitmap) && !this.ReconvertNeeded(bitmap, convertedBitmap))
                            return convertedFilePath;
                    }
                }

                this.ConvertImage(bitmap, convertedFilePath);
                return convertedFilePath;
            }
        }

        /// <summary>
        /// Processes the specified GPX documents. If the method is not overriden in the derived class,
        /// calls <see cref="Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            if (this.AcceptedFormats.Count == 0)
                throw new InvalidOperationException("AcceptedFormats collection cannot be empty.");

            if (this.DefaultEncoder == null)
                throw new InvalidOperationException("DefaultEncoder must be specified.");

            this._usedPaths.Clear();

            this._validExtensions.Clear();
            foreach (var enc in ImageCodecInfo.GetImageEncoders())
            {
                if (!this.AcceptedFormats.Any(o => o.Guid == enc.FormatID))
                    continue;

                this._validExtensions.UnionWith(enc.FilenameExtension.Replace("*", string.Empty).Split(';'));
            }

            this._defaultExtension = DefaultEncoder.FilenameExtension.Split(';')[0].Substring(1);

            try
            {
                this._webClient = new System.Net.WebClient();

                var images = new LinkedList<Tuple<Gpx.GpxWaypoint, Gpx.GeocacheImage>>();

                foreach (var doc in documents)
                    foreach (var wpt in doc.Waypoints)
                    {
                        foreach (var img in wpt.Geocache.Images)
                        {
                            if (this.PublishLogImages || !img.Address.LocalPath.Contains("/log/"))
                                images.AddLast(Tuple.Create(wpt, img));
                        }
                        if (this.PublishLogImages)
                        {
                            foreach (var log in wpt.Geocache.Logs)
                                foreach (var img in log.Images)
                                    images.AddLast(Tuple.Create(wpt, img));
                        }
                    }

                int i = 0;
                foreach (var img in images)
                {
                    this.Process(img.Item1, img.Item2);

                    this.TerminateExecutionIfNeeded();

                    i++;
                    this.ReportProgress(i, images.Count);

                    //TODO: remove when transformerprogress will show progress
                    this.ReportStatus("Progress: {0}%", i * 100 / images.Count);
                }
            }
            finally
            {
                if (this._webClient != null)
                    this._webClient.Dispose();
                this._webClient = null;
            }

            if (this.RemoveObsoleteImages)
            {
                if (string.IsNullOrEmpty(this.ImageRootPath))
                    throw new InvalidOperationException("ImageRootPath must be set when RemoveObsoleteImages is true.");

                this.ReportStatus("Removing obsolete images.");

                try
                {
                    var directories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var f in System.IO.Directory.EnumerateFiles(this.ImageRootPath, "*.*", System.IO.SearchOption.AllDirectories))
                    {
                        if (this._usedPaths.Contains(f))
                            continue;

                        System.IO.File.Delete(f);
                        directories.Add(System.IO.Path.GetDirectoryName(f));
                    }

                    // remove empty directories
                    foreach (var d in directories)
                    {
                        var dir = d;
                        while (dir.StartsWith(this.ImageRootPath, StringComparison.OrdinalIgnoreCase))
                        {
                            if (System.IO.Directory.EnumerateFileSystemEntries(dir).Any())
                                break;

                            System.IO.Directory.Delete(dir);

                            dir = System.IO.Path.GetDirectoryName(dir);
                        }
                    }
                }
                catch (System.IO.IOException ex)
                {
                    this.ReportStatus(StatusSeverity.Warning, "Unable to clean old images: " + ex.Message);
                }
            }

            this.ReportStatus("Images published.");
        }

        /// <summary>
        /// Processes the specified image.
        /// </summary>
        /// <param name="waypoint">The waypoint that has to be processed.</param>
        /// <param name="image">The image data</param>
        protected void Process(Gpx.GpxWaypoint waypoint, Gpx.GeocacheImage image)
        {
            try
            {
                var targetPath = this.CreateFullPath(waypoint, image);
                if (System.IO.File.Exists(targetPath))
                    return;

                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(targetPath));

                var imagePath = this.DownloadImage(waypoint, image);
                var convertedPath = this.ConvertImage(imagePath);
                System.IO.File.Copy(convertedPath, targetPath);
            }
            catch (Exception ex)
            {
                this.ReportStatus(StatusSeverity.Warning, "Unable to publish an image '{1}' for cache {0}: {2}", waypoint.Name, image.Title, ex.Message);
            }
        }
    }
}
