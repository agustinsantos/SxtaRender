using Sxta.Math;
using Sxta.Render.OpenGLExt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sxta.Render.Core;

namespace Sxta.Render.Resources.XmlResources
{
    public class FontResource : ResourceTemplate<Font>
    {
        public static FontResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new FontResource(manager, name, desc, e);
        }

        public FontResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,tex,nCols,nRows,minChar,maxChar,invalidChar,charWidths,");
            this.valueC = new Font();

            string tex = getParameter(desc, e, "tex");
            Texture2D fontTex = (Texture2D)manager.loadResource(tex).get();

            Vector2f fontSize = new Vector2f(19.0f / 32.0f, 24.0f);
            int nCols, nRows;
            int minChar, maxChar, invalidChar;

            getIntParameter(desc, e, "nCols", out nCols);
            getIntParameter(desc, e, "nRows", out nRows);
            getIntParameter(desc, e, "minChar", out minChar);
            getIntParameter(desc, e, "maxChar", out maxChar);
            getIntParameter(desc, e, "invalidChar", out invalidChar);

            // some checks
            Debug.Assert(nCols >= 0);
            Debug.Assert(nRows >= 0);
            Debug.Assert(minChar >= 0);
            Debug.Assert(minChar <= 255);
            Debug.Assert(maxChar > minChar); // at least 1 character
            Debug.Assert(maxChar <= 255);
            Debug.Assert(invalidChar >= minChar);
            Debug.Assert(invalidChar <= maxChar);

            List<int> charWidths = new List<int>();
            int count = 1 + maxChar - minChar;

            charWidths.Resize(count);

            bool fixedWidth;

            // parse character widths
            // if only one width is specified, the font is fixed-width and all character have the same width
            //                           else, the font is variable-width and character can overlap
            {
                int n = 0;
                string charWidthS = e.GetAttribute("charWidths");
                int width;

                foreach (string widthStr in charWidthS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(widthStr, out width))
                    {
                        if (n >= count) 
                            Debug.Assert(false); // too much widths
                        charWidths[n] = width;
                    }
                    else
                    {
                        Debug.Assert(false); // not an integer number, check your XML
                    }
                    n++;
                }
                Debug.Assert(n > 0);

                fixedWidth = (n == 1);

                // no more items, fill with last one (allow to specify only one width for all characters)
                for (int i = n; i < count; ++i)
                {
                    charWidths[i] = charWidths[i - 1];
                }
            }

            this.valueC.init(fontTex, nCols, nRows, minChar, maxChar, invalidChar, fixedWidth, charWidths);
        }
    }
}
