/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoTransformer.Transformers.LoadFromLiveApi
{
    public class Query
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        public Query()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Creates the request that can be used with Live API. 
        /// </summary>
        public GeocachingService.SearchForGeocachesRequest CreateRequest()
        {
            var req = new GeocachingService.SearchForGeocachesRequest();

            string myUserNameCached = null;
            Func<string> myUserName = () => 
            {
                if (myUserNameCached == null)
                    using (var client = GeocachingService.LiveClient.CreateClientProxy())
                        myUserNameCached = client.GetYourUserProfileCached().Profile.User.UserName;
                return myUserNameCached;
            };

            req.BookmarksExclude = new GeocachingService.BookmarksExcludeFilter() { ExcludeIgnoreList = true };

            if (this.Difficulty != null && (this.Difficulty.Item1 != 1 || this.Difficulty.Item2 != 5))
                req.Difficulty = new GeocachingService.DifficultyFilter() { MinDifficulty = this.Difficulty.Item1, MaxDifficulty = this.Difficulty.Item2 };
            
            if (this.Terrain != null && (this.Terrain.Item1 != 1 || this.Terrain.Item2 != 5))
                req.Terrain = new GeocachingService.TerrainFilter() { MinTerrain = this.Terrain.Item1, MaxTerrain = this.Terrain.Item2 };

            if (this.MinimumFavoritePoints != 0)
                req.FavoritePoints = new GeocachingService.FavoritePointsFilter() { MinFavoritePoints = this.MinimumFavoritePoints };

            if (!this.DownloadDisabled)
               req.GeocacheExclusions = new GeocachingService.GeocacheExclusionsFilter() { Available = true };

            if (this.HiddenByMe == HiddenByMeFilter.OnlyMine)
                req.HiddenByUsers = new GeocachingService.HiddenByUsersFilter() { UserNames = new string[] { myUserName() } };
            
            if (this.FoundByMe == FoundByMeFilter.Skip)
                req.NotFoundByUsers = new GeocachingService.NotFoundByUsersFilter() { UserNames = new string[] { myUserName() } };
            
            if (this.HiddenByMe == HiddenByMeFilter.Skip)
                req.NotHiddenByUsers = new GeocachingService.NotHiddenByUsersFilter() { UserNames = new string[] { myUserName() } };
            
            if (this.CenterCoordinates.HasValue)
                req.PointRadius = new GeocachingService.PointRadiusFilter() { Point = (GeocachingService.LatLngPoint)this.CenterCoordinates, DistanceInMeters = this.MaximumRadius * 1000 };
            
            return req;
        }

        /// <summary>
        /// Gets or sets the title of the query.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the query. The ID defines equality of two different <see cref="Query"/> objects even if other properties differ.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the center coordinates.
        /// </summary>
        public Coordinates.Wgs84Point? CenterCoordinates { get; set; }

        /// <summary>
        /// Gets or sets the place name of the center coordinates. This value is not used in the query but only for visualization.
        /// </summary>
        public string CenterName { get; set; }

        /// <summary>
        /// Gets or sets the maximum radius in kilometers.
        /// </summary>
        public int MaximumRadius { get; set; }

        /// <summary>
        /// Gets or sets the maximum caches to be returned from the search.
        /// </summary>
        public int MaximumCaches { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to download disabled caches.
        /// </summary>
        public bool DownloadDisabled { get; set; }

        /// <summary>
        /// Gets or sets the difficulty range for caches.
        /// </summary>
        public Tuple<double, double> Difficulty { get; set; }

        /// <summary>
        /// Gets or sets the terrain range for caches.
        /// </summary>
        public Tuple<double, double> Terrain { get; set; }

        /// <summary>
        /// Gets or sets the minimum favorite points.
        /// </summary>
        public int MinimumFavoritePoints { get; set; }

        /// <summary>
        /// Gets or sets the filter settings for caches that have been found by the user.
        /// </summary>
        public FoundByMeFilter FoundByMe { get; set; }

        /// <summary>
        /// Gets or sets the filter settings for caches that have been hidden by the user.
        /// </summary>
        public HiddenByMeFilter HiddenByMe { get; set; }

        /// <summary>
        /// Gets or sets the number of caches last time downloaded by this query.
        /// </summary>
        public int? LastDownloadCacheCount { get; set; }

        /// <summary>
        /// Gets the description of this query - the filter settings in human readable form.
        /// </summary>
        public string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(this.MaximumCaches);
                sb.Append(this.DownloadDisabled ? " caches" : " active caches");
                if (this.CenterCoordinates.HasValue)
                {
                    sb.Append(" in ");
                    sb.Append(this.MaximumRadius);
                    sb.Append("km radius");
                }

                if (!string.IsNullOrWhiteSpace(this.CenterName))
                {
                    sb.Append(" near ");
                    sb.Append(this.CenterName);
                }

                if (this.Difficulty != null && (this.Difficulty.Item1 != 1 || this.Difficulty.Item2 != 5))
                {
                    sb.Append(", difficuly ");
                    sb.Append(this.Difficulty.Item1);
                    sb.Append("-");
                    sb.Append(this.Difficulty.Item2);
                }

                if (this.Terrain != null && (this.Terrain.Item1 != 1 || this.Terrain.Item2 != 5))
                {
                    sb.Append(", terrain ");
                    sb.Append(this.Terrain.Item1);
                    sb.Append("-");
                    sb.Append(this.Terrain.Item2);
                }

                if (this.MinimumFavoritePoints != 0)
                {
                    sb.Append(", at least ");
                    sb.Append(this.MinimumFavoritePoints);
                    sb.Append(" fav points");
                }

                if (this.FoundByMe == FoundByMeFilter.Include)
                    sb.Append(", include my finds");

                if (this.HiddenByMe == HiddenByMeFilter.Include)
                    sb.Append(", include my hides");

                if (this.HiddenByMe == HiddenByMeFilter.OnlyMine)
                    sb.Append(", only my hides");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Serializes the query in the specified writer.
        /// </summary>
        /// <param name="writer">The binary writer into which the query will be serialized.</param>
        internal void Serialize(System.IO.BinaryWriter writer)
        {
            writer.Write((byte)0); // serialize the version of the data - this will be used if the data model is changed between versions
            writer.Write(this.Id.ToByteArray());
            writer.Write(this.Title);
            writer.Write(this.CenterCoordinates.HasValue);
            if (this.CenterCoordinates.HasValue)
            {
                writer.Write(this.CenterCoordinates.Value.Latitude);
                writer.Write(this.CenterCoordinates.Value.Longitude);
            }
            writer.Write(this.MaximumRadius);
            writer.Write(this.MaximumCaches);
            writer.Write(this.DownloadDisabled);
            writer.Write(this.Difficulty.Item1);
            writer.Write(this.Difficulty.Item2);
            writer.Write(this.Terrain.Item1);
            writer.Write(this.Terrain.Item2);
            writer.Write(this.MinimumFavoritePoints);
            writer.Write((int)this.FoundByMe);
            writer.Write((int)this.HiddenByMe);
            writer.Write(this.CenterName ?? string.Empty);
        }

        /// <summary>
        /// Deserializes the query from the specified reader. 
        /// Note that the method does not check if the reader has any more data to be read.
        /// </summary>
        /// <param name="reader">The reader that contains the query.</param>
        /// <returns>A deserialized <see cref="Query"/> object.</returns>
        internal static Query Deserialize(System.IO.BinaryReader reader)
        {
            var q = new Query();
            var version = reader.ReadByte();
            q.Id = new Guid(reader.ReadBytes(16));
            q.Title = reader.ReadString();
            if (reader.ReadBoolean())
                q.CenterCoordinates = new Coordinates.Wgs84Point(reader.ReadDecimal(), reader.ReadDecimal());
            q.MaximumRadius = reader.ReadInt32();
            q.MaximumCaches = reader.ReadInt32();
            q.DownloadDisabled = reader.ReadBoolean();
            q.Difficulty = Tuple.Create(reader.ReadDouble(), reader.ReadDouble());
            q.Terrain = Tuple.Create(reader.ReadDouble(), reader.ReadDouble());
            q.MinimumFavoritePoints = reader.ReadInt32();
            q.FoundByMe = (FoundByMeFilter)reader.ReadInt32();
            q.HiddenByMe = (HiddenByMeFilter)reader.ReadInt32();
            q.CenterName = reader.ReadString();
            return q;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Description;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var x = obj as Query;
            if (x == null)
                return false;
            return x.Id == this.Id;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }

    /// <summary>
    /// Defines whether to include geocaches that are found by the user in the query results.
    /// </summary>
    public enum FoundByMeFilter
    {
        /// <summary>
        /// The results do not include any geocaches that are found by the user.
        /// </summary>
        Skip,
        /// <summary>
        /// The results include geocaches that are found by the user.
        /// </summary>
        Include
    }

    /// <summary>
    /// Defines whether to include geocaches that are hidden by the user in the query results.
    /// </summary>
    public enum HiddenByMeFilter
    {
        /// <summary>
        /// The results do not include any geocaches hidden by the user.
        /// </summary>
        Skip,
        /// <summary>
        /// The results include geocaches hidden by the user.
        /// </summary>
        Include,
        /// <summary>
        /// The results include only geocaches hidden by the user and ignores everything else.
        /// </summary>
        OnlyMine
    }
}
