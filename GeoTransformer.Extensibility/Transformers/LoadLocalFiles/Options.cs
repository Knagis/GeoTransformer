/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GeoTransformer.Transformers.LoadLocalFiles
{
    internal partial class Options : UI.UserControlBase
    {
        public Options(byte[] configuration)
        {
            InitializeComponent();

            this.imageList.Images.Add("gpx", Resources.gpx);
            this.imageList.Images.Add("zip", Resources.zip);

            if (configuration == null || configuration.Length == 0)
                this.SelectedPath = Application.StartupPath;
            else
            {
                using (var reader = new System.IO.BinaryReader(new System.IO.MemoryStream(configuration)))
                {
                    var path = reader.ReadString();
                    this.UnselectedFromConfiguration = new List<string>();
                    var i = reader.ReadInt32();
                    for (var j = 0; j < i; j++)
                        this.UnselectedFromConfiguration.Add(reader.ReadString());

                    if (!string.IsNullOrEmpty(path))
                    {
                        var applicationPath = new Uri(Application.StartupPath);
                        var relativeUri = new Uri(path, UriKind.RelativeOrAbsolute);
                        var absoluteUri = new Uri(applicationPath, relativeUri);

                        this.SelectedPath = absoluteUri.LocalPath;
                    }
                    else
                    {
                        this.SelectedPath = Application.StartupPath;
                    }
                }
            }
        }

        public byte[] SerializeConfiguration()
        {
            using (var ms = new System.IO.MemoryStream())
            using (var writer = new System.IO.BinaryWriter(ms))
            {
                var absolutePath = new Uri(this.SelectedPath);
                var applicationPath = new Uri(Application.StartupPath);

                var relativePath = applicationPath.MakeRelativeUri(absolutePath).ToString();
                var lenDif = relativePath.Length - relativePath.TrimStart('.', '/').Length;
                // do not store the relative path if it is more than 3 levels up from the current.
                writer.Write(lenDif < 10 ? relativePath : absolutePath.LocalPath);

                var unselected = new HashSet<string>();
                GetUnselectedPaths(unselected, this.treeLoadedGpx.Nodes, null);
                writer.Write(unselected.Count);
                foreach (var x in unselected)
                    writer.Write(x);

                writer.Flush();
                return ms.ToArray();
            }
        }

        private IList<string> UnselectedFromConfiguration;
        public string SelectedPath
        {
            get
            {
                return (string)this.linkFolder.Tag;
            }
            set
            {
                if (!System.IO.Path.IsPathRooted(value))
                    value = System.IO.Path.GetFullPath(value);

                if (this.SelectedPath == value)
                    return;

                this.linkFolder.Text = value;
                this.linkFolder.Tag = value;

                if (System.IO.Directory.Exists(value))
                    fileSystemWatcher.Path = value;
                
                this.PrepareTreeView();
            }
        }

        private class NodeTag
        {
            public bool ZipContainer;
            public bool WithinZip;
            public System.IO.FileInfo File;
            public string ZipEntry;

            public string FullName
            {
                get
                {
                    if (this.WithinZip)
                        return this.File.FullName + "::" + this.ZipEntry;
                    else
                        return this.File.FullName;
                }
            }
        }

        private void treeLoadedGpx_AfterCheck(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode child in e.Node.Nodes)
                if (child.Checked != e.Node.Checked)
                    child.Checked = e.Node.Checked;

            if (e.Node.Parent != null && e.Node.Parent.Checked != e.Node.Checked && e.Node.Parent.Nodes.Cast<TreeNode>().All(o => o.Checked == e.Node.Checked))
                e.Node.Parent.Checked = e.Node.Checked;
        }

        private void fileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (!e.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) && !e.Name.EndsWith(".gpx", StringComparison.OrdinalIgnoreCase))
                return;

            this.PrepareTreeView();
        }

        private void fileSystemWatcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            if (!e.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) && !e.Name.EndsWith(".gpx", StringComparison.OrdinalIgnoreCase) &&
                !e.OldName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) && !e.OldName.EndsWith(".gpx", StringComparison.OrdinalIgnoreCase))
                return;

            this.PrepareTreeView(e);
        }

        private static void GetCollapsedPaths(HashSet<string> results, TreeNodeCollection nodes, System.IO.RenamedEventArgs renamedFile)
        {
            foreach (TreeNode n in nodes)
            {
                if (n.Tag is int)
                    return;

                var t = (NodeTag)n.Tag;
                if (renamedFile != null && string.Equals(t.File.FullName, renamedFile.OldFullPath, StringComparison.OrdinalIgnoreCase))
                    t.File = new System.IO.FileInfo(renamedFile.FullPath);
                if (!n.IsExpanded && n.Nodes.Count > 0)
                    results.Add(t.FullName);

                GetCollapsedPaths(results, n.Nodes, renamedFile);
            }
        }
        private static void GetUnselectedPaths(HashSet<string> results, TreeNodeCollection nodes, System.IO.RenamedEventArgs renamedFile)
        {
            foreach (TreeNode n in nodes)
            {
                if (n.Tag is int)
                    return;

                var t = (NodeTag)n.Tag;
                if (renamedFile != null && string.Equals(t.File.FullName, renamedFile.OldFullPath, StringComparison.OrdinalIgnoreCase))
                    t.File = new System.IO.FileInfo(renamedFile.FullPath);
                if (!n.Checked)
                    results.Add(t.FullName);

                GetUnselectedPaths(results, n.Nodes, renamedFile);
            }
        }
        public void PrepareTreeView()
        {
            PrepareTreeView(null);
        }
        public void PrepareTreeView(System.IO.RenamedEventArgs renamedFile)
        {
            var treeView = this.treeLoadedGpx;
            treeView.SuspendLayout();

            var collapsedNodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var unselectedNodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (this.UnselectedFromConfiguration != null)
            {
                unselectedNodes.UnionWith(this.UnselectedFromConfiguration);
                this.UnselectedFromConfiguration = null;
            }
            GetUnselectedPaths(unselectedNodes, treeView.Nodes, renamedFile);
            GetCollapsedPaths(collapsedNodes, treeView.Nodes, renamedFile);
            treeView.Nodes.Clear();

            treeView.CheckBoxes = true;
            treeView.Enabled = true;

            CreateTreeNodes(treeView, collapsedNodes, unselectedNodes, this.SelectedPath);

            if (treeView.Nodes.Count == 0)
            {
                treeView.CheckBoxes = false;
                treeView.Enabled = false;
                var node = treeView.Nodes.Add("No supported files found in this folder.");
                node.Tag = 0;
                node.ImageKey = node.SelectedImageKey = node.StateImageKey = "nonexistant";
            }

            AutoSizeTreeView(treeView);

            treeView.ResumeLayout(true);
        }

        private static void AutoSizeTreeView(TreeView treeView)
        {
            var rows = 0;

            var nodes = new Stack<TreeNode>();

            foreach (TreeNode node in treeView.Nodes)
            {
                rows++;
                if (node.IsExpanded)
                    nodes.Push(node);
            }

            while (nodes.Count > 0)
                foreach (TreeNode node in nodes.Pop().Nodes)
                {
                    rows++;
                    if (node.IsExpanded)
                        nodes.Push(node);
                }

            var delta = treeView.Size.Height - treeView.ClientSize.Height;
            treeView.Height = treeView.ItemHeight * rows + delta;
        }

        private static void CreateTreeNodes(TreeView treeView, HashSet<string> collapsedNodes, HashSet<string> unselectedNodes, string path)
        {
            var di = new System.IO.DirectoryInfo(path);
            if (!di.Exists)
                return;

            NodeTag tag, parentTag = null;
            foreach (var fi in di.GetFiles("*.gpx", System.IO.SearchOption.TopDirectoryOnly))
            {
                var node = new TreeNode(fi.Name);
                node.Tag = tag = new NodeTag() { File = fi };
                node.ImageKey = node.SelectedImageKey = node.StateImageKey = "gpx";
                node.Checked = !unselectedNodes.Contains(tag.FullName);
                treeView.Nodes.Add(node);
            }
            foreach (var fi in di.GetFiles("*.zip", System.IO.SearchOption.TopDirectoryOnly))
            {
                TreeNode parent = null;
                using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(fi.FullName))
                {
                    foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry zipe in zip)
                    {
                        if (zipe.CanDecompress && zipe.IsFile && zipe.Name.EndsWith(".gpx", StringComparison.OrdinalIgnoreCase))
                        {
                            if (parent == null)
                            {
                                parent = new TreeNode(fi.Name);

                                parent.Tag = tag = parentTag = new NodeTag() { ZipContainer = true, File = fi };
                                parent.ImageKey = parent.SelectedImageKey = parent.StateImageKey = "zip";
                                parent.Checked = !unselectedNodes.Contains(tag.FullName);

                                treeView.Nodes.Add(parent);
                            }

                            var node = new TreeNode(zipe.Name);
                            node.Tag = tag = new NodeTag() { File = fi, WithinZip = true, ZipEntry = zipe.Name };
                            node.ImageKey = node.SelectedImageKey = node.StateImageKey = "gpx";
                            node.Checked = !unselectedNodes.Contains(tag.FullName);
                            parent.Nodes.Add(node);
                        }
                    }
                }
                if (parent != null && !collapsedNodes.Contains(parentTag.FullName))
                    parent.Expand();
            }
        }

        public IEnumerable<Gpx.GpxDocument> Process()
        {
            return this.Process(this.treeLoadedGpx.Nodes);
        }

        private IEnumerable<Gpx.GpxDocument> Process(TreeNodeCollection nodes)
        {
            foreach (TreeNode n in nodes)
            {
                if (n.Tag is int)
                    yield break;

                yield return this.Process(n);

                foreach (var xml in this.Process(n.Nodes))
                    yield return xml;
            }
        }

        private Gpx.GpxDocument Process(TreeNode node)
        {
            var t = (NodeTag)node.Tag;
            if (t.ZipContainer || !node.Checked) return null;

            if (t.WithinZip)
                return Gpx.Loader.Zip(t.File.FullName, t.ZipEntry);
            else
                return Gpx.Loader.Gpx(t.File.FullName);
        }

        private void linkFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", this.SelectedPath);
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog.SelectedPath = this.SelectedPath;
            var res = this.folderBrowserDialog.ShowDialog();
            if (res == DialogResult.OK)
                this.SelectedPath = this.folderBrowserDialog.SelectedPath;
        }

        private void treeLoadedGpx_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            AutoSizeTreeView(this.treeLoadedGpx);
        }

        private void treeLoadedGpx_AfterExpand(object sender, TreeViewEventArgs e)
        {
            AutoSizeTreeView(this.treeLoadedGpx);
        }

    }
}
