using ExCSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// StyleSheet maintains a single stylesheet definition. A stylesheet can be combined with another stylesheet to create
    /// a new, merged stylesheet.
    /// </summary>
    public class StyleSheet
    {
        public StyleSheet()
        {
            root = new StyleSheetNode("", StyleSheetNodeType.ROOT);
            specificity_offset = 0;
        }

        public bool LoadStyleSheet(string filePath)
        {
            string buff;
            using (StreamReader reader = new StreamReader(filePath))
            {
                buff = reader.ReadToEnd();
            }
            var parser = new Parser();
            root.StyleSheet = parser.Parse(buff);

            //var imageUrl = stylesheet.StyleRules
            //            .SelectMany(r => r.Declarations)
            //            .FirstOrDefault(d => d.Name.Equals("font-style", StringComparison.InvariantCultureIgnoreCase))
            //            .Term
            //            .ToString(); // Finds the url('/images/logo.png') image url
            return root.StyleSheet != null;
        }


        /// Combines this style sheet with another one, producing a new sheet
        public StyleSheet CombineStyleSheet(StyleSheet other_sheet)
        {
           StyleSheet new_sheet = new StyleSheet();
            if (!new_sheet.root.MergeHierarchy(root) ||
                !new_sheet.root.MergeHierarchy(other_sheet.root, specificity_offset))
            {
                 return null;
            }

            new_sheet.specificity_offset = specificity_offset + other_sheet.specificity_offset;
            return new_sheet;
        }


        // Root level node, attributes from special nodes like "body" get added to this node
        private StyleSheetNode root;

        // The maximum specificity offset used in this style sheet to distinguish between properties in
        // similarly-specific rules, but declared on different lines. When style sheets are merged, the
        // more-specific style sheet (ie, coming further 'down' the include path) adds the offset of
        // the less-specific style sheet onto its offset, thereby ensuring its properties take
        // precedence in the event of a conflict.
        private int specificity_offset;

        // typedef std::set< StyleSheetNode* > NodeList;
        // typedef std::map< String, NodeList > NodeIndex;

        // Map of only nodes with actual style information.
        private Dictionary<string, HashSet<StyleSheetNode>> styled_node_index = new Dictionary<string, HashSet<StyleSheetNode>>();
        // Map of every node, even empty, un-styled, nodes.
        private Dictionary<string, HashSet<StyleSheetNode>> complete_node_index = new Dictionary<string, HashSet<StyleSheetNode>>();

        // typedef std::map< String, ElementDefinition* > ElementDefinitionCache;
        // Index of element addresses to element definitions.
        private Dictionary<string, ElementDefinition> address_cache;
        // Index of node sets to element definitions.
        private Dictionary<string, ElementDefinition> node_cache;
    }
}
