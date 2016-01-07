using OpenTK.Graphics;
using Sxta.Math;
using Sxta.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaRender.Fonts
{
    /// <summary>
    /// Class to hide TextNodeList and related classes from 
    /// user whilst allowing a textNodeList to be passed around.
    /// </summary>
    public class ProcessedText
    {
        internal TextNodeList TextNodeList;
        internal SizeF MaxSize;
        internal FontAlignment Alignment;
        internal SdfUniformStruct uniform;
        internal Mesh<SdfVertexStruct, uint> mesh;
        internal TextRenderer renderer;
        public void Draw()
        {
            if (mesh != null)
                renderer.Draw(uniform, mesh);
        }
    }

    public class TextRenderer : IDisposable
    {
        private readonly FontRenderOptions Options;
        private readonly SdfShader shader = new SdfShader();
        private FrameBuffer fb;
        private Texture tex;
        private TextureFont fontData;
        private Mesh<SdfVertexStruct, uint> mesh;
        private Vector2f printOffset;
        private Vector4i viewport;

        public Vector2f PrintOffset
        {
            get
            {
                return printOffset;
            }

            set
            {
                printOffset = value;
            }
        }
        public float LineSpacing
        {
            get { return (float)Math.Ceiling(fontData.LineSpacingPixel * Options.LineSpacing); }
        }

        public TextRenderer(FrameBuffer fb, TextureFont fontData, FontRenderOptions options)
        {
            this.Options = options;
            this.fb = fb;
            this.fontData = fontData;
            if (this.Options.UseSDF && fontData.HasSDFTexture)
                tex = this.SdfTexture;
            else
                tex = this.NormalTexture;
        }

        public void Init()
        {
            shader.Init();
            shader.Matrix = Matrix4f.Identity;
            shader.ExMatrix = Matrix4f.Identity;
            shader.Zoom = 160;
            shader.Skewed = false;
            shader.Fadedist = 0.1f;
            shader.Minfadezoom = 10;
            shader.Maxfadezoom = 250;
            shader.Fadezoom = 250;
            shader.Texture = tex;
            shader.TexSize = new Vector2f(tex.Width, tex.Height);
            shader.Color = new Vector4f(0.0f, 0.2f, 0.2f, 1.0f);
            shader.Buffer = 0.47f; // range [0.1, 0.7]
            shader.Gamma = 0.4f; // range [0, 4]

            mesh = new Mesh<SdfVertexStruct, uint>(SdfVertexStruct.SizeInBytes, sizeof(uint), MeshMode.TRIANGLES, MeshUsage.GPU_STATIC, 3);
            //mesh.addAttributes(SdfVertexStruct.Attributes);
            mesh.addAttributeType(0, 2, AttributeType.A32F, false);
            mesh.addAttributeType(1, 2, AttributeType.A32F, false);
            mesh.addAttributeType(2, 4, AttributeType.A32F, false);
            mesh.addAttributeType(3, 4, AttributeType.A32F, false);
        }

        Matrix4f vmMatrix;
        public void SetViewport(Vector4i viewport)
        {
            this.viewport = viewport;
            float left = viewport.X;
            float right = viewport.X + viewport.Z;
            float top = viewport.Y + viewport.W;
            float bottom = viewport.Y;
            float near = -1f;
            float far = 1f;
            Matrix4f.CreateOrthographicOffCenter(left, right, bottom, top, near, far, out vmMatrix);
            shader.Matrix = vmMatrix;
        }

        public void Clear()
        {
            if (mesh != null)
                mesh.clear();
        }
        public Texture NormalTexture
        {
            get
            {
                return CreateTexture(fontData.NormalBitmap);
            }
        }
        public Texture SdfTexture
        {
            get
            {
                return CreateTexture(fontData.SdfBitmap);
            }
        }
        public Texture ShadowTexture
        {
            get
            {
                return CreateTexture(fontData.ShadowBitmap);
            }
        }

        public void Draw()
        {
            if (mesh != null)
                fb.draw(shader.Program, mesh);
        }
        public void Draw(SdfUniformStruct uniform, Mesh<SdfVertexStruct, uint> _mesh)
        {
            if (uniform != null)
                uniform.Set(shader);
            if (_mesh != null)
                fb.draw(shader.Program, _mesh);
        }


        public ProcessedText ProcessText(string text, SizeF maxSize, FontAlignment alignment)
        {
            var nodeList = new TextNodeList(text);
            nodeList.MeasureNodes(fontData, Options);

            var processedText = new ProcessedText();
            processedText.TextNodeList = nodeList;
            processedText.MaxSize = maxSize;
            processedText.Alignment = alignment;
            //PreRenderText(processedText);
            return processedText;
        }

        public SizeF PreRenderText(int x, int y, ProcessedText processedText)
        {
            float maxMeasuredWidth = 0f;

            float xPos = x;
            float yPos = y;

            float xOffset = xPos;
            float yOffset = yPos;

            float maxWidth = processedText.MaxSize.Width;
            var alignment = processedText.Alignment;

            var nodeList = processedText.TextNodeList;
            for (var node = nodeList.First; node != null; node = node.Next)
                node.Value.LengthTweak = 0f;  //reset tweaks

            if (alignment == FontAlignment.ALIGN_RIGHT)
                xOffset -= (float)Math.Ceiling(TextNodeLineLength(nodeList.First, maxWidth) - maxWidth);
            else if (alignment == FontAlignment.ALIGN_CENTER)
                xOffset -= (float)Math.Ceiling(0.5f * TextNodeLineLength(nodeList.First, maxWidth));
            else if (alignment == FontAlignment.ALIGN_LEFT)
                JustifyLine(nodeList.First, maxWidth);

            bool atLeastOneNodeCosumedOnLine = false;
            float length = 0f;
            for (var node = nodeList.First; node != null; node = node.Next)
            {
                bool newLine = false;

                if (node.Value.Type == TextNodeType.LineBreak)
                {
                    newLine = true;
                }
                else
                {
                    if (Options.WordWrap && SkipTrailingSpace(node, length, maxWidth) && atLeastOneNodeCosumedOnLine)
                    {
                        newLine = true;
                    }
                    else if (length + node.Value.ModifiedLength <= maxWidth || !atLeastOneNodeCosumedOnLine)
                    {
                        atLeastOneNodeCosumedOnLine = true;

                        RenderWord(xOffset + length, yOffset, node);
                        length += node.Value.ModifiedLength;

                        maxMeasuredWidth = Math.Max(length, maxMeasuredWidth);

                    }
                    else if (Options.WordWrap)
                    {
                        newLine = true;
                        if (node.Previous != null)
                            node = node.Previous;
                    }
                    else
                        continue; // continue so we still read line breaks even if reached max width
                }

                if (newLine)
                {
                    if (yOffset - LineSpacing - yPos >= processedText.MaxSize.Height)
                        break;

                    yOffset -= LineSpacing;
                    xOffset = xPos;
                    length = 0f;
                    atLeastOneNodeCosumedOnLine = false;

                    if (node.Next != null)
                    {
                        if (alignment == FontAlignment.ALIGN_RIGHT)
                            xOffset -= (float)Math.Ceiling(TextNodeLineLength(node.Next, maxWidth) - maxWidth);
                        else if (alignment == FontAlignment.ALIGN_CENTER)
                            xOffset -= (float)Math.Ceiling(0.5f * TextNodeLineLength(node.Next, maxWidth));
                        else if (alignment == FontAlignment.ALIGN_LEFT)
                            JustifyLine(node.Next, maxWidth);
                    }
                }
            }
            return new SizeF(maxMeasuredWidth, yOffset - LineSpacing + yPos);
        }

        private Vector2f LockToPixel(Vector2f input)
        {
            if (Options.LockToPixel)
            {
                float r = Options.LockToPixelRatio;
                return new Vector2f((1 - r) * input.X + r * ((int)Math.Round(input.X)), (1 - r) * input.Y + r * ((int)Math.Round(input.Y)));
            }
            return input;
        }

        /// <summary>
        /// Computes the length of the next line, and whether the line is valid for
        /// justification.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="maxLength"></param>
        /// <param name="justifable"></param>
        /// <returns></returns>
        private float TextNodeLineLength(LinkedListNode<TextNode> node, float maxLength)
        {
            if (node == null)
                return 0;

            bool atLeastOneNodeCosumedOnLine = false;
            float length = 0;
            for (; node != null; node = node.Next)
            {

                if (node.Value.Type == TextNodeType.LineBreak)
                    break;

                if (SkipTrailingSpace(node, length, maxLength) && atLeastOneNodeCosumedOnLine)
                    break;

                if (length + node.Value.Length <= maxLength || !atLeastOneNodeCosumedOnLine)
                {
                    atLeastOneNodeCosumedOnLine = true;
                    length += node.Value.Length;
                }
                else
                {
                    break;
                }
            }
            return length;
        }

        /// <summary>
        /// Computes the length of the next line, and whether the line is valid for
        /// justification.
        /// </summary>
        private void JustifyLine(LinkedListNode<TextNode> linkedNode, float targetLength)
        {
            TextNode node = linkedNode.Value;
            bool justifiable = false;

            if (node == null)
                return;

            var headNode = linkedNode; //keep track of the head node


            //start by finding the length of the block of text that we know will actually fit:

            int charGaps = 0;
            int spaceGaps = 0;

            bool atLeastOneNodeCosumedOnLine = false;
            float length = 0;
            var expandEndNode = linkedNode; //the node at the end of the smaller list (before adding additional word)
            for (; linkedNode != null; linkedNode = linkedNode.Next)
            {
                node = linkedNode.Value;
                if (node.Type == TextNodeType.LineBreak)
                    break;

                if (SkipTrailingSpace(linkedNode, length, targetLength) && atLeastOneNodeCosumedOnLine)
                {
                    justifiable = true;
                    break;
                }

                if (length + node.Length < targetLength || !atLeastOneNodeCosumedOnLine)
                {

                    expandEndNode = linkedNode;

                    if (node.Type == TextNodeType.Space)
                        spaceGaps++;

                    if (node.Type == TextNodeType.Word)
                    {
                        charGaps += (node.Text.Length - 1);

                        //word was part of a crumbled word, so there's an extra char cap between the two words
                        if (CrumbledWord(linkedNode))
                            charGaps++;
                    }

                    atLeastOneNodeCosumedOnLine = true;
                    length += node.Length;
                }
                else
                {
                    justifiable = true;
                    break;
                }
            }


            //now we check how much additional length is added by adding an additional word to the line
            float extraLength = 0f;
            int extraSpaceGaps = 0;
            int extraCharGaps = 0;
            bool contractPossible = false;
            LinkedListNode<TextNode> contractEndNode = null;
            for (linkedNode = expandEndNode.Next; linkedNode != null; linkedNode = linkedNode.Next)
            {
                node = linkedNode.Value;
                if (node.Type == TextNodeType.LineBreak)
                    break;

                if (node.Type == TextNodeType.Space)
                {
                    extraLength += node.Length;
                    extraSpaceGaps++;
                }
                else if (node.Type == TextNodeType.Word)
                {
                    contractEndNode = linkedNode;
                    contractPossible = true;
                    extraLength += node.Length;
                    extraCharGaps += (node.Text.Length - 1);
                    break;
                }
            }

            if (justifiable)
            {
                //last part of this condition is to ensure that the full contraction is possible (it is all or nothing with contractions, since it looks really bad if we don't manage the full)
                bool contract = contractPossible && (extraLength + length - targetLength) * Options.JustifyContractionPenalty < (targetLength - length) &&
                    ((targetLength - (length + extraLength + 1)) / targetLength > -Options.JustifyCapContract);

                if ((!contract && length < targetLength) || (contract && length + extraLength > targetLength))  //calculate padding pixels per word and char
                {

                    if (contract)
                    {
                        length += extraLength + 1;
                        charGaps += extraCharGaps;
                        spaceGaps += extraSpaceGaps;
                    }

                    int totalPixels = (int)(targetLength - length); //the total number of pixels that need to be added to line to justify it
                    int spacePixels = 0; //number of pixels to spread out amongst spaces
                    int charPixels = 0; //number of pixels to spread out amongst char gaps

                    if (contract)
                    {

                        if (totalPixels / targetLength < -Options.JustifyCapContract)
                            totalPixels = (int)(-Options.JustifyCapContract * targetLength);
                    }
                    else
                    {
                        if (totalPixels / targetLength > Options.JustifyCapExpand)
                            totalPixels = (int)(Options.JustifyCapExpand * targetLength);
                    }

                    //work out how to spread pixles between character gaps and word spaces
                    if (charGaps == 0)
                    {
                        spacePixels = totalPixels;
                    }
                    else if (spaceGaps == 0)
                    {
                        charPixels = totalPixels;
                    }
                    else
                    {
                        if (contract)
                            charPixels = (int)(totalPixels * Options.JustifyCharacterWeightForContract * charGaps / spaceGaps);
                        else
                            charPixels = (int)(totalPixels * Options.JustifyCharacterWeightForExpand * charGaps / spaceGaps);


                        if ((!contract && charPixels > totalPixels) ||
                            (contract && charPixels < totalPixels))
                            charPixels = totalPixels;

                        spacePixels = totalPixels - charPixels;
                    }

                    int pixelsPerChar = 0;  //minimum number of pixels to add per char
                    int leftOverCharPixels = 0; //number of pixels remaining to only add for some chars

                    if (charGaps != 0)
                    {
                        pixelsPerChar = charPixels / charGaps;
                        leftOverCharPixels = charPixels - pixelsPerChar * charGaps;
                    }

                    int pixelsPerSpace = 0; //minimum number of pixels to add per space
                    int leftOverSpacePixels = 0; //number of pixels remaining to only add for some spaces

                    if (spaceGaps != 0)
                    {
                        pixelsPerSpace = spacePixels / spaceGaps;
                        leftOverSpacePixels = spacePixels - pixelsPerSpace * spaceGaps;
                    }

                    //now actually iterate over all nodes and set tweaked length
                    for (linkedNode = headNode; linkedNode != null; linkedNode = linkedNode.Next)
                    {
                        node = linkedNode.Value;
                        if (node.Type == TextNodeType.Space)
                        {
                            node.LengthTweak = pixelsPerSpace;
                            if (leftOverSpacePixels > 0)
                            {
                                node.LengthTweak += 1;
                                leftOverSpacePixels--;
                            }
                            else if (leftOverSpacePixels < 0)
                            {
                                node.LengthTweak -= 1;
                                leftOverSpacePixels++;
                            }
                        }
                        else if (node.Type == TextNodeType.Word)
                        {
                            int cGaps = (node.Text.Length - 1);
                            if (CrumbledWord(linkedNode))
                                cGaps++;

                            node.LengthTweak = cGaps * pixelsPerChar;

                            if (leftOverCharPixels >= cGaps)
                            {
                                node.LengthTweak += cGaps;
                                leftOverCharPixels -= cGaps;
                            }
                            else if (leftOverCharPixels <= -cGaps)
                            {
                                node.LengthTweak -= cGaps;
                                leftOverCharPixels += cGaps;
                            }
                            else
                            {
                                node.LengthTweak += leftOverCharPixels;
                                leftOverCharPixels = 0;
                            }
                        }

                        if ((!contract && linkedNode == expandEndNode) || (contract && linkedNode == contractEndNode))
                            break;

                    }
                }
            }
        }

        /// <summary>
        /// Checks whether to skip trailing space on line because the next word does not
        /// fit.
        /// 
        /// We only check one space - the assumption is that if there is more than one,
        /// it is a deliberate attempt to insert spaces.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="lengthSoFar"></param>
        /// <param name="boundWidth"></param>
        /// <returns></returns>
        private bool SkipTrailingSpace(LinkedListNode<TextNode> node, float lengthSoFar, float boundWidth)
        {

            if (node.Value.Type == TextNodeType.Space && node.Next != null && node.Next.Value.Type == TextNodeType.Word && node.Value.ModifiedLength + node.Next.Value.ModifiedLength + lengthSoFar > boundWidth)
            {
                return true;
            }
            return false;
        }

        public void RenderWord(float x, float y, string word)
        {
            var nodeList = new TextNodeList(word);
            RenderWord(x, y, nodeList.First);
        }

        public void RenderGlyph(char c)
        {
            TextureGlyph glyph = fontData.CharSetMapping[c];
            float x = printOffset.X;
            float y = printOffset.Y;
            uint index = (uint)mesh.getVertexCount();
            mesh.addVertex(SdfVertexStruct.NewVertex(x + 0, y + glyph.offset_y + glyph.Height, 0, 0, (short)glyph.X, (short)glyph.Y));
            mesh.addVertex(SdfVertexStruct.NewVertex(x + glyph.Width, y + glyph.offset_y + glyph.Height, 0, 0, (short)(glyph.X + glyph.Width), (short)glyph.Y));
            mesh.addVertex(SdfVertexStruct.NewVertex(x + 0, y + glyph.offset_y + 0, 0, 0, (short)glyph.X, (short)(glyph.Y + glyph.Height)));
            mesh.addVertex(SdfVertexStruct.NewVertex(x + glyph.Width, y + glyph.offset_y + 0, 0, 0, (short)(glyph.X + glyph.Width), (short)(glyph.Y + glyph.Height)));
            mesh.addIndices(new uint[] { index + 0, index + 1, index + 2, index + 1, index + 2, index + 3 });
        }

        private void RenderDropShadow(float x, float y, char c, TextureGlyph nonShadowGlyph)
        {
            //note can cast drop shadow offset to int, but then you can't move the shadow smoothly...
            if (fontData.HasShadowTexture && Options.DropShadowActive)
            {
#if TODO
                //make sure fontdata font's options are synced with the actual options
                if (fontData.dropShadow.Options != Options)
                    fontData.dropShadow.Options = Options;

                fontData.dropShadow.RenderGlyph(
                    x + (fontData.meanGlyphWidth * Options.DropShadowOffset.X + nonShadowGlyph.rect.Width * 0.5f),
                    y + (fontData.meanGlyphWidth * Options.DropShadowOffset.Y + nonShadowGlyph.rect.Height * 0.5f + nonShadowGlyph.yOffset), c, true);
#endif
            }
        }

        public void RenderGlyph(float x, float y, char c, bool isDropShadow, float scale = 0.3f)
        {
            TextureGlyph glyph = fontData.CharSetMapping[c];

            //note: it's not immediately obvious, but this combined with the paramteters to 
            //RenderGlyph for the shadow mean that we render the shadow centrally (despite it being a different size)
            //under the glyph
            if (isDropShadow)
            {
                x -= (int)(glyph.Width * 0.5f);
                y -= (int)(glyph.Height * 0.5f + glyph.offset_y);
            }

            RenderDropShadow(x, y, c, glyph);

            float tx1 = glyph.X;
            float ty1 = glyph.Y;
            float tx2 = glyph.X + glyph.Width;
            float ty2 = glyph.Y + glyph.Height;

            var tv0 = new Vector2f(tx1, ty1);
            var tv1 = new Vector2f(tx2, ty1);
            var tv2 = new Vector2f(tx1, ty2);
            var tv3 = new Vector2f(tx2, ty2);
            float ybase = y - glyph.offset_y;

            var v0 = PrintOffset + new Vector2f(x, ybase) * scale; // top-left
            var v1 = PrintOffset + new Vector2f(x + glyph.Width, ybase) * scale; // top-right
            var v2 = PrintOffset + new Vector2f(x, ybase - glyph.Height) * scale;// bottom-left
            var v3 = PrintOffset + new Vector2f(x + glyph.Width, ybase - glyph.Height) * scale;// bottom-right

            Color4 color = this.Options.Colour;
            if (isDropShadow)
                color = Color.FromArgb((int)(Options.DropShadowOpacity * 255f), Color.White);

            uint index = (uint)mesh.getVertexCount();
            mesh.addVertex(SdfVertexStruct.NewVertex(v0, Vector2f.Zero, tv0));
            mesh.addVertex(SdfVertexStruct.NewVertex(v1, Vector2f.Zero, tv1));
            mesh.addVertex(SdfVertexStruct.NewVertex(v2, Vector2f.Zero, tv2));
            mesh.addVertex(SdfVertexStruct.NewVertex(v3, Vector2f.Zero, tv3));
            mesh.addIndices(new uint[] { index + 0, index + 1, index + 2, index + 1, index + 2, index + 3 });
        }

        private void RenderWord(float x, float y, LinkedListNode<TextNode> node)
        {

            if (node.Value.Type != TextNodeType.Word)
                return;

            string text = node.Value.Text;
            int charGaps = text.Length - 1;
            bool isCrumbleWord = CrumbledWord(node);
            if (isCrumbleWord)
                charGaps++;

            int pixelsPerGap = 0;
            int leftOverPixels = 0;

            if (charGaps != 0)
            {
                pixelsPerGap = (int)node.Value.LengthTweak / charGaps;
                leftOverPixels = (int)node.Value.LengthTweak - pixelsPerGap * charGaps;
            }

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (fontData.CharSetMapping.ContainsKey(c))
                {
                    var glyph = fontData.CharSetMapping[c];

                    RenderGlyph(x, y, c, false);

                    if (this.fontData.IsMonospacingActive(this.Options))
                        x += this.fontData.GetMonoSpaceWidth(this.Options);
                    else
                        x += (int)Math.Ceiling(glyph.Width + fontData.MeanGlyphWidth * Options.CharacterSpacing + fontData.GetKerningPairCorrection(i, text));

                    x += pixelsPerGap;
                    if (leftOverPixels > 0)
                    {
                        x += 1.0f;
                        leftOverPixels--;
                    }
                    else if (leftOverPixels < 0)
                    {
                        x -= 1.0f;
                        leftOverPixels++;
                    }
                }
            }
        }
        private bool CrumbledWord(LinkedListNode<TextNode> node)
        {
            return (node.Value.Type == TextNodeType.Word && node.Next != null && node.Next.Value.Type == TextNodeType.Word);
        }

        private Texture CreateTexture(Bitmap img)
        {
            TextureInternalFormat pif;
            TextureFormat pf;
            Sxta.Render.PixelType pt;
            int size;
            EnumConversion.ConvertPixelFormat(img.PixelFormat, out pif, out pf, out pt, out size);
            //img.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData Data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            GPUBuffer buff = new GPUBuffer();
            buff.setData(Data.Width * Data.Height * size, Data.Scan0, BufferUsage.STATIC_DRAW);
            img.UnlockBits(Data);
            Texture.Parameters params_ = new Texture.Parameters();
            params_.min(TextureFilter.LINEAR);
            params_.mag(TextureFilter.LINEAR);
            //params_.min(TextureFilter.NEAREST);
            //params_.mag(TextureFilter.NEAREST);
            params_.wrapS(TextureWrap.CLAMP_TO_EDGE);
            params_.wrapT(TextureWrap.CLAMP_TO_EDGE);
            Sxta.Render.Buffer.Parameters s = new Sxta.Render.Buffer.Parameters();
            Texture texture = new Texture2D(img.Width, img.Height, pif, pf, pt, params_, s, buff);
            buff.Dispose();
            return texture;
        }
        public void Dispose()
        {
            if (tex != null)
                tex.Dispose();
            if (mesh != null)
                mesh.Dispose();

            if (shader != null)
            {
                shader.Dispose();
            }
        }
    }
}
