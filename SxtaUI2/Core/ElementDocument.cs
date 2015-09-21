using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// Represents a document in the dom tree.
    /// </summary>
    public class ElementDocument : Element
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ElementDocument(string tag)
            : base(tag)
        {
#if TODO
            style_sheet = null;
            context = null;

            modal = false;
            layout_dirty = true;
            lock_layout = 0;

            ForceLocalStackingContext();

            SetProperty(POSITION, "absolute");
#endif
        }

        /// Process given document header
        public void ProcessHeader(DocumentHeader document_header)
        {
            // Store the source address that we came from
            source_url = document_header.Source;

            // Construct a new header and copy the template details across
            DocumentHeader header = new DocumentHeader();
            header.MergePaths(header.TemplateResources, document_header.TemplateResources, document_header.Source);
            // Merge in any templates, note a merge may cause more templates to merge
            for (int i = 0; i < header.TemplateResources.Count; i++)
            {
                Template merge_template = TemplateCache.LoadTemplate(header.TemplateResources[i]);

                if (merge_template != null)
                    header.MergeHeader(merge_template.GetHeader());
                else
                    log.WarnFormat("Template {0} not found", header.TemplateResources[i]);
            }

            // Merge the document's header last, as it is the most overriding.
            header.MergeHeader(document_header);

            // Set the title to the document title.
            title = document_header.Title;

            // If a style-sheet (or sheets) has been specified for this element, then we load them and set the combined sheet
            // on the element; all of its children will inherit it by default.
            StyleSheet style_sheet = null;
            if (header.RcssExternal.Count > 0)
                style_sheet = StyleSheetFactory.GetStyleSheet(header.RcssExternal);

            // Combine any inline sheets.
            if (header.RcssInline.Count > 0)
            {
                for (int i = 0; i < header.RcssInline.Count; i++)
                {
                    StyleSheet new_sheet = new StyleSheet();
                    //StreamMemory* stream = new StreamMemory((const byte*) header.rcss_inline[i].CString(), header.rcss_inline[i].Length());
                    //stream->SetSourceURL(document_header->source);

                    if (new_sheet.LoadStyleSheet(null))
                    {
                        if (style_sheet != null)
                        {
                            StyleSheet combined_sheet = style_sheet.CombineStyleSheet(new_sheet);
                            style_sheet = combined_sheet;
                        }
                        else
                            style_sheet = new_sheet;
                    }
                }
            }
#if TODO
	// If a style sheet is available, set it on the document and release it.
	if (style_sheet)
	{
		SetStyleSheet(style_sheet);
		style_sheet->RemoveReference();
	}

	// Load external scripts.
	for (size_t i = 0; i < header.scripts_external.size(); i++)
	{
		StreamFile* stream = new StreamFile();
		if (stream->Open(header.scripts_external[i]))
			LoadScript(stream, header.scripts_external[i]);

		stream->RemoveReference();
	}

	// Load internal scripts.
	for (size_t i = 0; i < header.scripts_inline.size(); i++)
	{
		StreamMemory* stream = new StreamMemory((const byte*) header.scripts_inline[i].CString(), header.scripts_inline[i].Length());
		LoadScript(stream, "");
		stream->RemoveReference();
	}

	// Hide this document.
	SetProperty(VISIBILITY, "hidden");
#endif
        }

        public override ElementDocument GetOwnerDocument()
        {
            return this;
        }

        // Title of the document
        internal string title;

        // The original path this document came from
        internal string source_url;

        // The document's style sheet.
        internal StyleSheet style_sheet;

        internal Context context;

        // Is the current display modal
        internal bool modal;

        // Is the layout dirty?
        internal bool layout_dirty;
        internal bool lock_layout;

    }
}
