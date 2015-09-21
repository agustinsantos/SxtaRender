using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// Creates stylesheets on the fly as needed. The factory keeps a cache of built sheets for optimisation.
    /// </summary>
    static class StyleSheetFactory
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// Initialise the style factory
        public static bool Initialise()
        {
            //selectors["nth-child"] = new StyleSheetNodeSelectorNthChild();
            //selectors["nth-last-child"] = new StyleSheetNodeSelectorNthLastChild();
            //selectors["nth-of-type"] = new StyleSheetNodeSelectorNthOfType();
            //selectors["nth-last-of-type"] = new StyleSheetNodeSelectorNthLastOfType();
            //selectors["first-child"] = new StyleSheetNodeSelectorFirstChild();
            //selectors["last-child"] = new StyleSheetNodeSelectorLastChild();
            //selectors["first-of-type"] = new StyleSheetNodeSelectorFirstOfType();
            //selectors["last-of-type"] = new StyleSheetNodeSelectorLastOfType();
            //selectors["only-child"] = new StyleSheetNodeSelectorOnlyChild();
            //selectors["only-of-type"] = new StyleSheetNodeSelectorOnlyOfType();
            //selectors["empty"] = new StyleSheetNodeSelectorEmpty();

            return true;
        }

        /// Shutdown style manager
        public static void Shutdown()
        {
            ClearStyleSheetCache();
        }

        /// Gets the named sheet, retrieving it from the cache if its already been loaded
        /// @param sheet name of sheet to load
        public static StyleSheet GetStyleSheet(string sheet_name)
        {
            // Look up the sheet definition in the cache
            if (stylesheets.ContainsKey(sheet_name))
            {
                return stylesheets[sheet_name];
            }

            // Don't currently have the sheet, attempt to load it
            StyleSheet sheet = LoadStyleSheet(sheet_name);
            if (sheet == null)
                return null;

            // Add it to the cache, and add a reference count so the cache will keep hold of it.
            stylesheets[sheet_name] = sheet;

            return sheet;
        }


        /// Builds and returns a stylesheet based on the list of input sheets
        /// Generated sheets will be cached for later use
        /// @param sheets List of sheets to combine into one	
        public static StyleSheet GetStyleSheet(List<string> sheets)
        {
            // Generate a unique key for these sheets
            String combined_key = "";
            for (int i = 0; i < sheets.Count; i++)
            {
                combined_key += Path.GetFileName(sheets[i]);
            }

            // Look up the sheet definition in the cache.
            if (stylesheet_cache.ContainsKey(combined_key))
            {
                return stylesheet_cache[combined_key];
            }

            // Load and combine the sheets.
            StyleSheet sheet = null;
            for (int i = 0; i < sheets.Count; i++)
            {
                StyleSheet sub_sheet = GetStyleSheet(sheets[i]);
                if (sub_sheet != null)
                {
                    if (sheet != null)
                    {
                        StyleSheet new_sheet = sheet.CombineStyleSheet(sub_sheet);

                        sheet = new_sheet;
                    }
                    else
                        sheet = sub_sheet;
                }
                else
                    log.ErrorFormat("Failed to load style sheet {0}.", sheets[i]);
            }

            if (sheet == null)
                return null;

            // Add to cache, and a reference to the sheet to hold it in the cache.
            stylesheet_cache[combined_key] = sheet;
            return sheet;
        }

        /// Clear the style sheet cache.
        public static void ClearStyleSheetCache()
        { throw new NotImplementedException(); }

        /// Returns one of the available node selectors.
        /// @param name[in] The name of the desired selector.
        /// @return The selector registered with the given name, or NULL if none exists.
        public static StyleSheetNodeSelector GetSelector(string name)
        { throw new NotImplementedException(); }

        // Loads an individual style sheet
        public static StyleSheet LoadStyleSheet(string sheet)
        {

            // Open stream, construct new sheet and pass the stream into the sheet
            // TODO: Make this support ASYNC
            StyleSheet new_style_sheet = new StyleSheet();
            if (!new_style_sheet.LoadStyleSheet(sheet))
            {
                new_style_sheet = null;
            }
            return new_style_sheet;
        }

        // Individual loaded stylesheets
        //typedef std::map<String, StyleSheet*> StyleSheets;
        private static Dictionary<string, StyleSheet> stylesheets = new Dictionary<string, StyleSheet>();

        // Cache of combined style sheets
        private static Dictionary<string, StyleSheet> stylesheet_cache = new Dictionary<string, StyleSheet>();

        // Custom complex selectors available for style sheets.
        //typedef std::map< String, StyleSheetNodeSelector* > SelectorMap;
        private static Dictionary<string, StyleSheetNodeSelector> selectors = new Dictionary<string, StyleSheetNodeSelector>();
    }
}
