using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SxtaRender.Fonts
{
    internal enum TextNodeType { Word, LineBreak, Space }

    internal class TextNode
    {
        public TextNodeType Type;
        public string Text;
        public float Length; //pixel length (without tweaks)
        public float LengthTweak; //length tweak for justification

        public float ModifiedLength
        {
            get { return Length + LengthTweak; }
        }

        public TextNode(TextNodeType Type, string Text)
        {
            this.Type = Type;
            this.Text = Text;
        }
    }

    /// <summary>
    /// A doubly linked list of text nodes
    /// </summary>
    internal class TextNodeList : LinkedList<TextNode>
    {
        private static readonly Regex re = new Regex(@"(?<word>[^ \t\r\n]+)|(?<space>[ \t])|(?<newl>(\r\n|\r))", RegexOptions.Compiled);

        public TextNodeList()
        { }

        /// <summary>
        /// Builds a doubly linked list of text nodes from the given input string
        /// </summary>
        /// <param name="text"></param>
        public TextNodeList(string text)
        {
            MatchCollection mc = re.Matches(text);
            int mIdx = 0;
            foreach (Match m in mc)
            {
                if (m.Groups["word"].Success)
                    this.AddLast(new TextNode(TextNodeType.Word, m.Groups["word"].Value));
                else if (m.Groups["space"].Success)
                    this.AddLast(new TextNode(TextNodeType.Space, null));
                else if (m.Groups["newl"].Success)
                    this.AddLast(new TextNode(TextNodeType.LineBreak, null));
                mIdx++;
            }
        }

        public void MeasureNodes(TextureFont fontData, FontRenderOptions options)
        {
            for (var node = this.First; node != null; node = node.Next)
            {
                if (node.Value.Length == 0f)
                    node.Value.Length = MeasureTextNodeLength(node, fontData, options);
            }
        }


        private float MeasureTextNodeLength(LinkedListNode<TextNode> node, TextureFont fontData, FontRenderOptions options)
        {

            bool monospaced = fontData.IsMonospacingActive(options);
            float monospaceWidth = fontData.GetMonoSpaceWidth(options);

            if (node.Value.Type == TextNodeType.Space)
            {
                if (monospaced)
                    return monospaceWidth;

                return (float)Math.Ceiling(fontData.meanGlyphWidth * options.WordSpacing);
            }

            float length = 0f;
            if (node.Value.Type == TextNodeType.Word)
            {

                for (int i = 0; i < node.Value.Text.Length; i++)
                {
                    char c = node.Value.Text[i];
                    if (fontData.CharSetMapping.ContainsKey(c))
                    {
                        if (monospaced)
                            length += monospaceWidth;
                        else
                            length += (float)Math.Ceiling(fontData.CharSetMapping[c].Rect.Width + fontData.meanGlyphWidth * options.CharacterSpacing + GetKerningPairCorrection(fontData, i, node.Value.Text, node));
                    }
                }
            }
            return length;
        }

        /// <summary>
        /// Returns the kerning length correction for the character at the given index in the given string.
        /// Also, if the text is part of a textNode list, the nextNode is given so that the following 
        /// node can be checked in case of two adjacent word nodes.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <param name="textNode"></param>
        /// <returns></returns>
        private int GetKerningPairCorrection(TextureFont fontData, int index, string text, LinkedListNode<TextNode> textNode)
        {
            if (fontData.KerningPairs == null)
                return 0;

            var chars = new char[2];

            if (index + 1 == text.Length)
            {
                if (textNode != null && textNode.Next != null && textNode.Next.Value.Type == TextNodeType.Word)
                    chars[1] = textNode.Next.Value.Text[0];
                else
                    return 0;
            }
            else
            {
                chars[1] = text[index + 1];
            }

            chars[0] = text[index];

            String str = new String(chars);

            if (fontData.KerningPairs.ContainsKey(str))
                return fontData.KerningPairs[str];

            return 0;
        }

        

        /// <summary>
        /// Splits a word into sub-words of size less than or equal to baseCaseSize 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="baseCaseSize"></param>
        public void Crumble(LinkedListNode<TextNode> node, int baseCaseSize)
        {

            //base case
            if (node.Value.Text.Length <= baseCaseSize)
                return;

            var left = SplitNode(node);
            var right = left.Next;

            Crumble(left, baseCaseSize);
            Crumble(right, baseCaseSize);

        }

        /// <summary>
        /// Splits a word node in two, adding both new nodes to the list in sequence.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>The first new node</returns>
        public LinkedListNode<TextNode> SplitNode(LinkedListNode<TextNode> node)
        {
            if (node.Value.Type != TextNodeType.Word)
                throw new Exception("Cannot slit text node of type: " + node.Value.Type);

            int midPoint = node.Value.Text.Length / 2;

            string newFirstHalf = node.Value.Text.Substring(0, midPoint);
            string newSecondHalf = node.Value.Text.Substring(midPoint, node.Value.Text.Length - midPoint);


            TextNode newFirst = new TextNode(TextNodeType.Word, newFirstHalf);
            TextNode newSecond = new TextNode(TextNodeType.Word, newSecondHalf);

            LinkedListNode<TextNode> newNode = this.AddAfter(node, newFirst);
            this.AddAfter(newNode, newSecond);
            this.Remove(node);
            return newNode;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (TextNode node in this)
            {
                if (node.Type == TextNodeType.Space)
                    builder.Append(" ");
                if (node.Type == TextNodeType.LineBreak)
                    builder.Append(System.Environment.NewLine);
                if (node.Type == TextNodeType.Word)
                    builder.Append(node.Text);
            }
            return builder.ToString();
        }
    }
}
