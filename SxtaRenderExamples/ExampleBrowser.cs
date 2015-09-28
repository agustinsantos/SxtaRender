#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;
using Sxta3D.Examples.Properties;
using System.Threading;
using System.IO;

namespace Examples
{
    public partial class ExampleBrowser : Form
    {
        #region Fields

        //PrivateFontCollection font_collection = new PrivateFontCollection();

        bool show_warning = true;

        static readonly string SourcePath = FindSourcePath();

        #endregion

        #region Constructors

        public ExampleBrowser()
        {
            Font = SystemFonts.DialogFont;

            InitializeComponent();
            Icon = Resources.App;

            // Windows 6 (Vista) and higher come with Consolas, a high-quality monospace font. Use that or fallback to
            // the generic monospace font on other systems.
            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT &&
                System.Environment.OSVersion.Version.Major >= 6)
            {
                textBoxOutput.Font = richTextBoxSource.Font = new Font("Consolas", 10.0f, FontStyle.Regular);
            }
            else
            {
                textBoxOutput.Font = richTextBoxSource.Font =
                    new Font(FontFamily.GenericMonospace, 10.0f, FontStyle.Regular);
            }
        }

        #endregion

        #region Protected Members

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Add those by hand, because using the designer results in an empty
            // image list when cross-compiling on Mono.
            imageListSampleCategories.Images.Add("CoreUsage", Resources.OpenGLWrapper);

            Trace.Listeners.Add(new TextBoxTraceListener(textBoxOutput));
            treeViewSamples.TreeViewNodeSorter = new SamplesTreeViewSorter();

            LoadSamplesFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnShown(EventArgs e)
        {
            if (show_warning)
            {
                show_warning = false;
            }
        }

        #endregion

        #region Private Members

        #region Events

        #region TreeView

        private void treeViewSamples_AfterSelect(object sender, TreeViewEventArgs e)
        {
            const string no_docs = "Documentation has not been entered.";
            const string no_source = "Source code has not been entered.";
            string docs = null;

           
            richTextBoxSource.Text = no_source;
            richTextBoxDescription.Text = no_docs;
            
            if (e.Node.Tag != null)
            {
                ExampleInfo einfo = (ExampleInfo)e.Node.Tag;
                string sample = einfo.Attribute.Documentation;
                string category = einfo.Attribute.Category.ToString();
                string subcategory = ""; // einfo.Attribute.Subcategory;

                if (!String.IsNullOrEmpty(((ExampleInfo)e.Node.Tag).Attribute.Documentation))
                {
                    string path_rtf = Path.Combine(Path.Combine(Path.Combine(SourcePath, category), subcategory), sample);
                    string sample_rtf = Path.ChangeExtension(path_rtf, "rtf");

                    if (File.Exists(sample_rtf))
                    {
                        docs = File.ReadAllText(sample_rtf);
                    }


                    if (String.IsNullOrEmpty(docs))
                        richTextBoxDescription.Text = String.Format("File {0} not found.", sample_rtf);
                    else
                        richTextBoxDescription.Rtf = docs;
                }

                if (!String.IsNullOrEmpty(((ExampleInfo)e.Node.Tag).Attribute.Source))
                {
                    string source = null;
                    string path_cs = Path.Combine(Path.Combine(Path.Combine(SourcePath, category), subcategory), einfo.Attribute.Source);
                    string sample_cs = Path.ChangeExtension(path_cs, "cs");
                    if (File.Exists(sample_cs))
                    {
                        source = File.ReadAllText(sample_cs);
                    }
                    if (String.IsNullOrEmpty(source))
                        richTextBoxSource.Text = String.Format("File {0} not found.", sample_cs);
                    else
                        richTextBoxSource.Text = source;
                }
            }
        }

        private void treeViewSamples_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                ActivateNode(e.Node);
            }
        }

        private void treeViewSamples_KeyDown(object sender, KeyEventArgs e)
        {
            // The enter key activates a node (either expands/collapses or executes its sample).
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    ActivateNode(treeViewSamples.SelectedNode);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void treeViewSamples_MouseDown(object sender, MouseEventArgs e)
        {
            // Make sure that right-clicking a new node will select that node before displaying
            // the context menu. Without this, right-clicking a node does not select it, which
            // is completely disorienting.
            // As a bonus, make any mouse button select the underlying node,
            TreeNode node = treeViewSamples.HitTest(e.Location).Node;
            if (node != null)
                treeViewSamples.SelectedNode = node;

            // Middle click selects and activates a node (either expands/collapses or executes its sample).
            // Right button displays the context menu.
            // All other mouse buttons simply select the underlying node.
            switch (e.Button)
            {
                case MouseButtons.Middle:
                    ActivateNode(node);
                    break;

                case MouseButtons.Right:
                    treeViewSamples.ContextMenuStrip.Show(sender as Control, e.Location);
                    break;
            }
        }

        private void treeViewSamples_AfterExpand(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode child in e.Node.Nodes)
                child.EnsureVisible();
        }

        private void contextMenuStripSamples_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "&Run Sample": RunSample(this, (ExampleInfo)treeViewSamples.SelectedNode.Tag); break;
                case "View Description": tabControlSample.SelectedTab = tabDescription; break;
                case "View Source Code": tabControlSample.SelectedTab = tabSource; break;
            }
        }

        #endregion

        #region Description

        private void richTextBoxDescription_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                richTextBoxDescription.ContextMenuStrip.Show(sender as Control, e.X, e.Y);
            }
        }

        private void contextMenuStripDescription_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "&Copy")
            {
                Clipboard.SetText(richTextBoxDescription.SelectedRtf, TextDataFormat.Rtf);
            }
        }

        #endregion

        #region Source Code

        private void richTextBoxSource_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                richTextBoxSource.ContextMenuStrip.Show(sender as Control, e.X, e.Y);
            }
        }

        private void contextMenuStripSource_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "&Copy")
            {
                Clipboard.SetText(richTextBoxSource.SelectedText, TextDataFormat.Text);
            }
        }

        #endregion

        #endregion

        #region Actions

        void LoadSamplesFromAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                object[] attributes = type.GetCustomAttributes(false);
                ExampleAttribute example = null;
                foreach (object attr in attributes)
                {
                    if (attr is ExampleAttribute)
                    {
                        example = (ExampleAttribute)attr;

                        if (example.Visible)
                        {
                            // Add this example to the sample TreeView.
                            // First check whether the ExampleCategory exists in the tree (and add it if it doesn't).
                            // Then add the example as a child node on this category.

                            if (!treeViewSamples.Nodes.ContainsKey(example.Category.ToString()))
                            {
                                int category_index = GetImageIndexForSample(imageListSampleCategories, example.Category.ToString(), String.Empty);
                                treeViewSamples.Nodes.Add(example.Category.ToString(), String.Format("{0} samples", example.Category),
                                    category_index, category_index);
                            }

                            if (!treeViewSamples.Nodes[example.Category.ToString()].Nodes.ContainsKey(example.Subcategory.ToString()))
                            {
                                int category_index = GetImageIndexForSample(imageListSampleCategories, example.Category.ToString(), String.Empty);
                                treeViewSamples.Nodes[example.Category.ToString()].Nodes.Add(example.Subcategory.ToString(), 
                                    example.Subcategory, category_index, category_index);
                            }

                            int image_index = GetImageIndexForSample(imageListSampleCategories, example.Category.ToString(), example.Subcategory);
                            TreeNode node = new TreeNode(example.Title, image_index, image_index);
                            node.Name = example.Title;
                            node.Tag = new ExampleInfo(type, example);
                            treeViewSamples.Nodes[example.Category.ToString()].Nodes[example.Subcategory.ToString()].Nodes.Add(node);
                        }
                    }
                }
            }

            treeViewSamples.Sort();
        }

        void ActivateNode(TreeNode node)
        {
            if (node == null)
                return;

            if (node.Tag == null)
            {
                if (node.IsExpanded)
                    node.Collapse();
                else
                    node.Expand();
            }
            else
            {
                tabControlSample.SelectedTab = tabPageOutput;
                textBoxOutput.Clear();
                RunSample(node.TreeView.TopLevelControl, (ExampleInfo)node.Tag);
            }
        }

        static int GetImageIndexForSample(ImageList list, string category, string subcategory)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            foreach (string extension in new string[] { "", ".png", ".jpg" })
            {
                string name = subcategory.ToString() + extension;
                if (list.Images.ContainsKey(name))
                    return list.Images.IndexOfKey(name);

                name = category.ToString() + extension;
                if (list.Images.ContainsKey(name))
                    return list.Images.IndexOfKey(name);
            }

            return -1;
        }

        static void RunSample(Control parent, ExampleInfo e)
        {
            if (e == null)
                return;

            MethodInfo main =
                e.Example.GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) ??
                e.Example.GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(object), typeof(object) }, null);
            if (main != null)
            {
                try
                {
                    if (parent != null)
                    {
                        parent.Visible = false;
                        Application.DoEvents();
                    }
                    Trace.WriteLine(String.Format("Launching sample: \"{0}\"", e.Attribute.Title));
                    Trace.WriteLine(String.Empty);

                    Thread thread = new Thread((ThreadStart)delegate
                    {
                        try
                        {
                            main.Invoke(null, null);
                        }
                        catch (TargetInvocationException expt)
                        {
                            string ex_info;
                            if (expt.InnerException != null)
                                ex_info = expt.InnerException.ToString();
                            else
                                ex_info = expt.ToString();
                            MessageBox.Show(ex_info, "An example encountered an error.", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            Debug.Print(expt.ToString());
                        }
                        catch (NullReferenceException expt)
                        {
                            MessageBox.Show(expt.ToString(), "The Example launcher failed to load the example.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    });
                    thread.IsBackground = true;
                    thread.Start();
                    thread.Join();
                }
                finally
                {
                    if (parent != null)
                    {
                        parent.Visible = true;
                        Application.DoEvents();
                    }
                }
            }
            else
            {
                MessageBox.Show("The selected example does not define a Main method", "Entry point not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        // Tries to detect the path that contains the source for the examples.
        static string FindSourcePath()
        {
            string current_dir = Directory.GetCurrentDirectory();

            // Typically, our working directory is either "[opentk]/Binaries/OpenTK/[config]" or "[opentk]".
            // The desired source path is "[opentk]/Source/Examples/[ExampleCategory]"

            string guess = current_dir;
            if (CheckPath(ref guess))
                return guess; // We were in [opentk] after all

            guess = current_dir;
            for (int i = 0; i < 3; i++)
            {
                DirectoryInfo dir = Directory.GetParent(guess);
                if (!dir.Exists)
                    break;
                guess = dir.FullName;
            }

            if (CheckPath(ref guess))
                return guess; // We were in [opentk]/Binaries/OpenTK/[config] after all

            throw new DirectoryNotFoundException();
        }

        static bool CheckPath(ref string path)
        {
            string guess = path;
            if (Directory.Exists(guess))
            {
                guess = Path.Combine(guess, "SxtaRenderExamples");
                if (Directory.Exists(guess))
                {
                    guess = Path.Combine(guess, "Tutorials");
                    if (Directory.Exists(guess))
                    {
                        // We are have found [MainDir]/Proland.Main/Tutorials
                        path = guess;
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        private void ExampleBrowser_Load(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
