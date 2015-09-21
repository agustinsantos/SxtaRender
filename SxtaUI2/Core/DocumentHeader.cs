using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// The document header struct contains the
    /// header details gathered from an XML document parse.
    /// </summary>
    public class DocumentHeader
    {

        /// <summary>
        /// Path and filename this document was loaded from
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The title of the document
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A list of template resources that can used while parsing the document
        /// </summary>
        public List<string> TemplateResources
        {
            get { return templateResources; }
            set { templateResources = value; }
        }
        /// <summary>
        /// Inline RCSS definitions
        /// </summary>
        public List<string> RcssInline
        {
            get { return rcssInline; }
            set { rcssInline = value; }
        }

        /// <summary>
        /// External RCSS definitions that should be loaded
        /// </summary>
        public List<string> RcssExternal
        {
            get { return rcssExternal; }
            set { rcssExternal = value; }
        }

        /// <summary>
        /// Inline script source
        /// </summary>
        public List<string> ScriptsInline
        {
            get { return scriptsInline; }
            set { scriptsInline = value; }
        }

        /// <summary>
        /// External scripts that should be loaded
        /// </summary>
        public List<string> ScriptsExternal
        {
            get { return scriptsExternal; }
            set { scriptsExternal = value; }
        }



        /// Merges the specified header with this one
        /// @param header Header to merge
        public void MergeHeader(DocumentHeader header)
        {
            // Copy the title across if ours is empty
            if (string.IsNullOrWhiteSpace(Title))
                Title = header.Title;
            // Copy the url across if ours is empty
            if (string.IsNullOrWhiteSpace(Source))
                Source = header.Source;

            // Combine internal data	
            rcssInline.AddRange(header.rcssInline);
            scriptsInline.AddRange(header.scriptsInline);

            // Combine external data, keeping relative paths
            MergePaths(templateResources, header.templateResources, header.Source);
            MergePaths(rcssExternal, header.rcssExternal, header.Source);
            MergePaths(scriptsExternal, header.scriptsExternal, header.Source);
        }

        /// Merges paths from one string list to another, preserving the base_path
        public void MergePaths(List<string> target, List<string> source, string base_path)
        {
            for (int i = 0; i < source.Count; i++)
            {
                String joined_path;
                CoreEngine.GetSystemInterface().JoinPath(out joined_path, base_path, source[i]);

                target.Add(joined_path);
            }
        }


        private List<string> rcssInline = new List<string>();
        private List<string> rcssExternal = new List<string>();
        private List<string> templateResources = new List<string>();
        private List<string> scriptsInline = new List<string>();
        private List<string> scriptsExternal = new List<string>();
    }
}
