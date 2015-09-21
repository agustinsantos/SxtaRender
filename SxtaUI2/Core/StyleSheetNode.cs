using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    public enum StyleSheetNodeType
    {
        TAG = 0,
        CLASS,
        ID,
        PSEUDO_CLASS,
        STRUCTURAL_PSEUDO_CLASS,
        NUM_NODE_TYPES,	// only counts the listed node types
        ROOT			// special node type we don't keep in a list
    }

    class StyleSheetNode
    {
        /// Constructs a generic style-sheet node.
        public StyleSheetNode(string name, StyleSheetNodeType type, StyleSheetNode parent = null)
        { }

        /// Merges an entire tree hierarchy into our hierarchy.
        public bool MergeHierarchy(StyleSheetNode node, int specificity_offset = 0)
        {
            this.StyleSheet.Rules.AddRange(node.StyleSheet.Rules);
#if TODO
            // Merge the other node's properties into ours.
            MergeProperties(node->properties, specificity_offset);

            selector = node->selector;
            a = node->a;
            b = node->b;

            for (int i = 0; i < NUM_NODE_TYPES; i++)
            {
                for (NodeMap::iterator iterator = node->children[i].begin(); iterator != node->children[i].end(); iterator++)
                {
                    StyleSheetNode* local_node = GetChildNode((*iterator).second->name, (NodeType)i);
                    local_node->MergeHierarchy((*iterator).second, specificity_offset);
                }
            }
#endif
            return true;
        }

        public ExCSS.StyleSheet StyleSheet
        {
            get { return parsedSheet; }
            set { parsedSheet = value; }
        }

        private ExCSS.StyleSheet parsedSheet = new ExCSS.StyleSheet();
    }
}
