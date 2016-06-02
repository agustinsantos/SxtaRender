using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Render.Resources
{
    /// <summary>
    /// A ResourceLoader that loads ResourceDescriptor from XML files. This loader
    /// can load resources from individual XML files, each file containing a single
    /// ResourceDescriptor, and from so called XML archive files, each archive file
    /// containing one or more ResourceDescriptor. This loader can search for these
    /// files in several directories and archives files configured with #addPath and
    /// #addArchive.
    /// </summary>
    public class XMLResourceLoader : ResourceLoader
    {

        /**
         * Creates a new XMLResourceLoader.
         */
        public XMLResourceLoader()
            : base()
        {
        }


        /**
         * Deletes this XMLResourceLoader.
         */
#if TODO 
        ~XMLResourceLoader()
        {

            for (map<string, pair<TiXmlDocument*, time_t> >::iterator i = cache.begin(); i != cache.end(); ++i) {
                delete i.second.first;
            }
            cache.clear();
        }
#endif

        /**
         * Adds a search path where ResourceDescriptor can be looked for.
         */
        public void addPath(string path)
        {
            paths.Add(path);
        }


        /**
         * Adds an XML archive file where ResourceDescriptor can be looked for.
         */
        public void addArchive(string archive)
        {
            archives.Add(archive);
        }

        /**
         * Returns the path of the resource of the given name.
         *
         * @param name the name of a resource.
         * @return the path of this resource.
         * @throw exception if the resource is not found.
         */
        public override string findResource(string name)
        {
            XmlElement desc = new XmlDocument().CreateElement(name);
            return findFile(desc, paths, name);
        }

        /**
         * Loads the ResourceDescriptor of the given name.
         *
         * @param name the name of the ResourceDescriptor to be loaded.
         * @return the ResourceDescriptor of the given name, or null if the %resource
         *      is not found.
         */
        public override ResourceDescriptor loadResource(string name)
        {
            DateTime stamp = DateTime.MinValue;
            XmlElement desc = null;
            if (name.StartsWith("renderbuffer"))
            {
                // resource names of the form "renderbuffer-X-Y" describe texture
                // resources that are not described by any file, either for the XML part
                // or for the binary part. The XML part is generated from the resource
                // name, and the binary part is null
                desc = buildTextureDescriptor(name);
            }
            else if (isTextureFile(name))
            {
                // 2D texture resources can be loaded directly from an image file; the
                // texture parameters (internal format, filters, etc) then get default values

                desc = new XmlDocument().CreateElement("texture2D");
                desc.SetAttribute("name", name);
                desc.SetAttribute("source", name);
                desc.SetAttribute("internalformat", "RGBA8");
                desc.SetAttribute("min", "LINEAR_MIPMAP_LINEAR");
                desc.SetAttribute("mag", "LINEAR");
                desc.SetAttribute("wraps", "REPEAT");
                desc.SetAttribute("wrapt", "REPEAT");
            }
            else if (name.Contains(';'))
            {
                // resource names of the form "module1;module;module3;..." describe
                // program resources that may not be described by any file, either for the
                // XML part or for the binary part. The XML part is generated from the
                // resource name, and the binary part is null (unless a compiled program
                // exists for this program)
                desc = findDescriptor(name, ref stamp, false);
                if (desc == null)
                {
                    desc = buildProgramDescriptor(name);
                }

            }
            else if (name.Length >= 5 && name.EndsWith(".mesh"))
            {
                // mesh resources do not have any file to describe the XML part, which is
                // trivial and hence generated on the fly here:
                XmlDocument doc = new XmlDocument();
                desc = doc.CreateElement("mesh");
                desc.SetAttribute("source", name);
            }

            else
            {
                // for all other resource types, the XML part is described in a file,
                // which must be loaded
                desc = findDescriptor(name, ref stamp);
            }
            if (desc != null)
            {
                // when we have the XML part we can load the binary part, if any
                List<Tuple<string, DateTime>> dataStamps = new List<Tuple<string, DateTime>>();
                int size = 0;
                object data = loadData(desc, out size, dataStamps);
                return new XMLResourceDescriptor(desc, data, size, stamp, dataStamps);
            }
            return null;
        }

        /**
         * Reloads the ResourceDescriptor of the given name.
         *
         * @param name the name of the ResourceDescriptor to be loaded.
         * @param currentValue the current value of this ResourceDescriptor.
         * @return the new value of this ResourceDescriptor, or null if this value
         *      has not changed on disk.
         */
        public override ResourceDescriptor reloadResource(string name, ResourceDescriptor currentValue)
        {
            throw new NotImplementedException();
        }

        /**
         * Returns true if the given file name extension corresponds to an image file.
         *
         * @param a file name.
         * @return true if the file name extension corresponds to an image file.
         */
        private static bool isTextureFile(string name)
        {
            if (name.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
                name.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase) ||
                name.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase) ||
                name.EndsWith(".tif", StringComparison.InvariantCultureIgnoreCase) ||
                name.EndsWith(".bmp", StringComparison.InvariantCultureIgnoreCase) ||
                name.EndsWith(".tga", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /**
         * Looks for a file in a set of directories.
         *
         * @param desc the XML part of a ResourceDescriptor.
         * @param paths a set of directory names.
         * @param file a relative file name.
         * @return the absolute file name of the file.
         * @throw exception if the file is not found in any directory.
         */
        protected virtual string findFile(XmlElement desc, List<string> paths, string file)
        {
            foreach (string dir in paths)
            {
                string path = dir + '/' + file;
                if (File.Exists(path))
                    return path;

            }
            if (log.IsErrorEnabled)
            {
                log.Error("Cannot find '" + file + "' file");
            }
            throw new Exception("Cannot find '" + file + "' file");
        }

        /**
         * Loads the content of a file.
         *
         * @param file the name of a file.
         * @param[out] size returns the size of the file's content in bytes.
         * @return the file's content.
         */
        protected virtual byte[] loadFile(string file, out int size)
        {
            byte[] data = File.ReadAllBytes(file);
            size = data.Length;
            return data;
        }
        protected virtual string loadStringFile(string file)
        {
            return File.ReadAllText(file);
        }

        /**
         * Computes the last modification time of the given file.
         *
         * @param name a fie name.
         * @param[out] t returns the last modification time of the given file.
         */
        protected virtual void getTimeStamp(string name, out DateTime t)
        {
            t = DateTime.MinValue;
            if (File.Exists(name))
                t = File.GetLastWriteTime(name);
        }


        /**
         * The directories where individual ResourceDescriptor files can be looked for.
         */
        private List<string> paths = new List<string>();

        /**
         * The archives where other ResourceDescriptor files can be looked for.
         */
        private List<string> archives = new List<string>();

        /**
         * A cache of the archive files. Maps archive file names to archive content
         * and last modification time on disk.
         */
        private Dictionary<string, Tuple<XmlDocument, DateTime>> cache = new Dictionary<string, Tuple<XmlDocument, DateTime>>();

        /**
         * Returns the XML part of the ResourceDescriptor of the given name. This
         * method looks for this descriptor in the archive files and then, if not
         * found, in the directories specified with #addPath.
         *
         * @param name the name of a ResourceDescriptor.
         * @param[in,out] t the last modification time of this %resource descriptor,
         *      or 0 if it has not been loaded yet. This modification time is
         *      updated by this method if it has changed.
         * @param log true to log an error message if the descriptor is not found.
         * @return the XML part of the ResourceDescriptor of the given name, of null
         *      if the last modification time is still equal to t or if the %resource
         *      is not found.
         */
        public XmlElement findDescriptor(string name, ref DateTime t, bool mustlog = true)
        {
            // we first look in the archive files
            for (int i = 0; i < archives.Count; ++i)
            {
                DateTime u = t;
                XmlDocument archive = loadArchive(archives[i], out u);
                if (archive != null)
                {
                    XmlElement desc = findDescriptor(archive, name);
                    if (desc != null)
                    {
                        if (u == t)
                        {
                            // if the last modification time is equal to the last known
                            // modification time, return null
                            return null;
                        }
                        else
                        {
                            t = u;
                            return desc;
                        }
                    }
                }
            }
            // then in the directories specified with #addPath
            for (int i = 0; i < paths.Count; ++i)
            {
                string filename = paths[i] + "/" + name + ".xml";
                DateTime u;
                getTimeStamp(filename, out u);
                if (u != DateTime.MinValue)
                {
                    if (u == t)
                    {
                        // if the last modification time is equal to the last known
                        // modification time, return null
                        return null;
                    }
                    else
                    {
                        t = u;
                    }
                    XmlDocument doc = new XmlDocument();
                    try
                    {
                        doc.Load(filename);
                        if (log.IsInfoEnabled)
                        {
                            log.Info("Loaded file '" + filename + "'");
                        }
                        //TODO delete[] data;
                        return doc.DocumentElement;
                    }
                    catch (Exception ex)
                    {
                        if (log.IsErrorEnabled)
                        {
                            log.Error("Syntax error in '" + filename + "'; Exception =" + ex);
                        }
                    }
                }
            }
            if (mustlog && log.IsErrorEnabled)
            {
                log.Error("Cannot find resource '" + name + "'");
            }
            // resource not found, return null
            return null;
        }

        /**
         * Returns the XML part of the ResourceDescriptor of the given name. This
         * method looks for this descriptor in the given archive file.
         *
         * @param archive the archive file where the descriptor must be looked for.
         * @param name the name of a ResourceDescriptor.
         * @return the XML part of the ResourceDescriptor, or null if the archive
         *      file does not contain this %resource descriptor.
         */
        private static XmlElement findDescriptor(XmlDocument archive, string name)
        {
            // we first load the archive ...
            XmlElement root = archive.DocumentElement;
            if (root != null && root.Name == "archive")
            {
                // ... then we look for our descriptor in the archive content
                foreach (XmlNode child in root.ChildNodes)
                {
                    XmlElement desc = child as XmlElement;
                    if (desc != null)
                    {
                        string n = desc.GetAttribute("name");
                        if (n != null && n == name)
                        {
                            return desc;
                        }
                    }
                }
            }
            return null;
        }

        /**
         * Builds the XML part of texture %resource descriptors for the special textures
         * 'renderbuffer-X-Y'. The XML part is generated from the %resource name.
         */
        private static XmlElement buildTextureDescriptor(string name)
        {
            int index1 = name.IndexOf('-', 0);
            int index2 = name.IndexOf('-', index1 + 1);
            string size = name.Substring(index1 + 1, index2 - index1 - 1);
            int index3 = name.IndexOf('-', index2 + 1);
            string internalformat = name.Substring(index2 + 1, (index3 == -1 ? name.Length : index3) - index2 - 1);

            XmlElement p = new XmlDocument().CreateElement("texture2D");
            p.SetAttribute("name", name);
            p.SetAttribute("internalformat", internalformat);
            p.SetAttribute("width", size);
            p.SetAttribute("height", size);
            p.SetAttribute("format", "RED");
            p.SetAttribute("type", "FLOAT");
            p.SetAttribute("min", "NEAREST");
            p.SetAttribute("mag", "NEAREST");
            return p;
        }


        /**
         * Builds the XML part of program %resource descriptors. The XML part is
         * generated from the %resource name of the form "shader1;shader2;shader3;...".
         */
        private static XmlElement buildProgramDescriptor(string name)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement p = doc.CreateElement("program");
            p.SetAttribute("name", name);
            doc.AppendChild(p);
            foreach (string module in name.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                XmlElement s = doc.CreateElement("module");
                s.SetAttribute("name", module);
                p.AppendChild(s);
            }
            return p;
        }


        /**
         * Loads the archive file of the given name.
         *
         * @param name the name of the archive file to be loaded.
         * @param[out] t returns the last modification time of this file on disk.
         * @return the archive file of the given name, or null if this file is not
         *      found.
         */
        private XmlDocument loadArchive(string name, out DateTime t)
        {
            t = DateTime.MinValue;
            // we first look in the cache
            Tuple<XmlDocument, DateTime> rst;
            if (cache.TryGetValue(name, out rst))
            {
                // if the last modification time of the file is equal to the last
                // modification time of the file in cache ...
                getTimeStamp(name, out t);
                if (rst.Item2 == t)
                {
                    // ... then we just return the cached file content
                    return rst.Item1;
                }
            }
            // then we try to load the archive file
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(name);
                if (log.IsInfoEnabled)
                {
                    log.Info("Loaded file '" + name + "'");
                }
                if (rst != null)
                {
                    // if the cache already contains a value for this name, delete it
                    //delete i.second.first;
                }
                else
                {
                    getTimeStamp(name, out t);
                }
                // put the new value and its last modification time in cache
                cache[name] = new Tuple<XmlDocument, DateTime>(doc, t);
                return doc;
            }
            catch (Exception ex)
            {

                if (log.IsErrorEnabled)
                {
                    log.Error("File not found or syntax error in '" + name + "'");
                    log.Error(ex);
                }
                return null;
            }
        }

        /**
         * Loads the ASCII or binary part of a ResourceDescriptor.
         *
         * @param desc the XML part of a ResourceDescriptor.
         * @param[out] size returns the size of ASCII or binary part.
         * @param[in,out] stamps the last modification time of the file(s) that
         *      contain this ASCII or binary part, or an empty vector if this ASCII
         *      or binary part has not been loaded yet. These modification times are
         *      updated by this method if they have changed. Each element of this
         *      vector contains a file name and its last modification time.
         * @return the ASCII or binary part of the given ResourceDescriptor, or null
         *      if this %resource has no binary part, if this part is not found, or
         *      if the last modification times are still equal to the given
         *      modification times.
         */
        private object loadData(XmlElement desc, out int size, List<Tuple<string, DateTime>> stamps)
        {
            size = -1;

            // if the resource has an ASCII or binary part ...
            if (desc.Name == "texture1D" ||
                desc.Name == "texture1DArray" ||
                desc.Name == "texture2D" ||
                desc.Name == "texture2DArray" ||
                desc.Name == "texture3D" ||
                desc.Name == "textureCube" ||
                desc.Name == "textureCubeArray" ||
                desc.Name == "textureRectangle" ||
                desc.Name == "module" ||
                desc.Name == "mesh" )
            {
                // we first get the name of the file containing this ASCII or binary part
                string file = desc.GetAttribute("source");
                if (string.IsNullOrWhiteSpace(file))
                {
                    if (desc.Name != "module" &&
                        desc.Name != "mesh" &&
                        !string.IsNullOrWhiteSpace(desc.GetAttribute("width")))
                    {
                        // a texture resource can have no binary part, provided its
                        // dimensions are specified in the XML part. In this case we
                        // return a null binary part.
                        return null;
                    }
                    if (!(desc.Name == "module" &&
                        (!string.IsNullOrWhiteSpace(desc.GetAttribute("vertex")) ||
                        !string.IsNullOrWhiteSpace(desc.GetAttribute("tessControl")) ||
                        !string.IsNullOrWhiteSpace(desc.GetAttribute("tessEvaluation")) ||
                        !string.IsNullOrWhiteSpace(desc.GetAttribute("geometry")) ||
                        !string.IsNullOrWhiteSpace(desc.GetAttribute("fragment")))))
                    {
                        // a module may not have a 'source' attribute if it has one
                        // of the vertex, tessControl, ... or fragment attribute
                        if (log.IsErrorEnabled)
                        {
                            log.Error("Missing 'source' attribute");
                        }
                        // otherwise it is an error if the binary part file name is not specified
                        throw new Exception("Missing 'source' attribute");
                    }
                }

                // then we test if the modification times have changed or not
                // if not then do not load the ASCII or binary part again
                bool doLoad = false;
                if (stamps.Count == 0)
                { // if the binary part has not been loaded yet
                    doLoad = true; // we load it
                }
                else
                {
                    // otherwise we load it only if the last modification times have changed
                    for (int i = 0; i < stamps.Count; ++i)
                    {
                        DateTime newt;
                        getTimeStamp(stamps[i].Item1, out newt);
                        if (newt != stamps[i].Item2)
                        {
                            doLoad = true;
                            break;
                        }
                    }
                }
                if (!doLoad)
                {
                    return null;
                }

                // special case for modules made of separate files
                if (desc.Name == "module" && string.IsNullOrWhiteSpace(file))
                {
                    stamps.Clear();
                    string vertexData = null;
                    if (!string.IsNullOrWhiteSpace(desc.GetAttribute("vertex")))
                    {
                        string shaderpath = findFile(desc, paths, desc.GetAttribute("vertex"));
                        string shader = loadStringFile(shaderpath);
                        vertexData = loadShaderData(desc, paths, shaderpath, shader, stamps);
                    }
                    string tessControlData = null;
                    if (!string.IsNullOrWhiteSpace(desc.GetAttribute("tessControl")))
                    {
                        string shaderpath = findFile(desc, paths, desc.GetAttribute("tessControl"));
                        string shader = loadStringFile(shaderpath);
                        tessControlData = loadShaderData(desc, paths, shaderpath, shader, stamps);
                    }
                    string tessEvalData = null;
                    if (!string.IsNullOrWhiteSpace(desc.GetAttribute("tessEvaluation")))
                    {
                        string shaderpath = findFile(desc, paths, desc.GetAttribute("tessEvaluation"));
                        string shader = loadStringFile(shaderpath);
                        tessEvalData = loadShaderData(desc, paths, shaderpath, shader, stamps);
                    }
                    string geometryData = null;
                    if (!string.IsNullOrWhiteSpace(desc.GetAttribute("geometry")))
                    {
                        string shaderpath = findFile(desc, paths, desc.GetAttribute("geometry"));
                        string shader = loadStringFile(shaderpath);
                        geometryData = loadShaderData(desc, paths, shaderpath, shader, stamps);
                    }
                    string fragmentData = null;
                    if (!string.IsNullOrWhiteSpace(desc.GetAttribute("fragment")))
                    {
                        string shaderpath = findFile(desc, paths, desc.GetAttribute("fragment"));
                        string shader = loadStringFile(shaderpath);
                        fragmentData = loadShaderData(desc, paths, shaderpath, shader, stamps);
                    }
                    return new string[] { vertexData, tessControlData, tessEvalData, geometryData, fragmentData };
                }

                // then we load the raw ASCII or binary part
                string path = stamps.Count == 0 ? findFile(desc, paths, file) : stamps[0].Item1;


                stamps.Clear();

                // and then we process it depending on the type of the resource
                if (desc.Name == "module")
                {
                    // for a shader resource the ASCII part can reference other files
                    // via #include directives; we need to load them and to substitute
                    // their content
                    String data = loadStringFile(path);
                    size = data.Length;
                    return new string[] { loadShaderData(desc, paths, path, data, stamps) };

                }
                else if (desc.Name == "mesh")
                {
                    // for a mesh or compiled program resource, no processing is needed
                    DateTime t;
                    getTimeStamp(path, out t);
                    stamps.Add(new Tuple<string, DateTime>(path, t));
                    byte[] data = loadFile(path, out size);
                    return data;
                }
                else
                {
                    // for a texture we need to decompress the file (PNG, JPG, etc)
                    byte[] data = loadFile(path, out size);
                    return loadTextureData(desc, path, data, ref size, stamps);
                }
            }
            return null;
        }


        /// <summary>
        /// Loads the ASCII part of a shader %resource, i.e. the shader source code.
        /// </summary>
        /// <param name="desc">the XML part of a shader ResourceDescriptor.</param>
        /// <param name="paths">the directories where the shader source files must be looked for.</param>
        /// <param name="path">a file containing (a part of the) shader source code.</param>
        /// <param name="data">the content of the 'path' file.</param>
        /// <param name="stamps">the last modification time of the file(s) that contain
        ///      the shader source code, or an empty vector if these files are not known
        ///      yet.These modification times are updated by this method if they have
        ///      changed.Each element of this vector contains a file name and its last
        ///      modification time.</param>
        /// <returns></returns>
        private string loadShaderData(XmlElement desc, List<string> paths, string path, string data, List<Tuple<string, DateTime>> stamps)
        {
            DateTime t;
            getTimeStamp(path, out t);
            stamps.Add(new Tuple<string, DateTime>(path, t));
            if (!data.Contains("#include"))
            {
                // if there is no #include directive in 'data' then we can directly return
                return data;
            }
            // otherwise we must load the referenced files and substitute their content;
            // the result will be placed in the 'result' string
            int size = data.Length;
            StringBuilder result = new StringBuilder(size);
            bool comment = false;
            bool lineComment = false;
            int i = 0;

            // we parse the file to find the #include that are not inside comments
            while (i < size)
            {
                if (!comment)
                { // if we are not inside a comment
                    if (i + 1 < size)
                    {
                        if (data[i] == '/' && data[i + 1] == '*')
                        {
                            // if we find a '/*' then we now are in a comment
                            result.Append("/*");
                            comment = true;
                            lineComment = false;
                            i += 2;
                            continue;
                        }
                        else if (data[i] == '/' && data[i + 1] == '/')
                        {
                            // likewise if we find a '//'
                            result.Append("//");
                            comment = true;
                            lineComment = true;
                            i += 2;
                            continue;
                        }
                    }
                    if (i + 8 <= size && data.Substring(i, 8) == "#include")
                    {
                        // if we find a #include
                        int s = data.IndexOf('\"', i);
                        if (s >= 0)
                        { // not found
                            int e = data.IndexOf('\"', s + 1);
                            if (e >= 0)
                            {
                                // we first extract the referenced file name
                                string inc = data.Substring(s + 1, e - s - 1);
                                string incFile;
                                try
                                {
                                    // then we find the absolute name of this file
                                    incFile = findFile(desc, paths, inc);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                int incSize;
                                // we can then load the content of the referenced file
                                string incData = Encoding.Default.GetString(loadFile(incFile, out incSize));
                                // and then analyze its content with a recursive call
                                // to process the #include directives that this file may
                                // in turn contain
                                string incShader = loadShaderData(desc, paths, incFile, incData, stamps);
                                // finally we append this processed content to the
                                // result data, instead of the #include directive itself
                                result.Append(incShader);

                                // i = (e - (char*) data) + 1;
                                i = e + 1;
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (lineComment)
                    { // if we are in a line comment
                        if (data[i] == '\n')
                        { // and find a newline we exit the comment
                            result.Append(data[i++]);
                            comment = false;
                            continue;
                        }
                    }
                    else if (i + 1 < size && data[i] == '*' && data[i + 1] == '/')
                    {
                        // likewise, if we find a '*/' in a block comment, we exit it
                        result.Append("*/");
                        comment = false;
                        i += 2;
                        continue;
                    }
                }
                result.Append(data[i++]);
            }
            //delete[] data;
            //size = result.size();
            //data = new unsigned char[size + 1];
            //memcpy(data, result.c_str(), size + 1);
            return result.ToString();
        }


        /**
         * Loads the binary part of a texture %resource.
         *
         * @param desc the XML part of the texture %resource descriptor.
         * @param path the absolute name of the file containing the texture image.
         * @param data the encoded image data (in PNG, PJG, etc format).
         * @param[in,out] size the size of the encoded image data and, after the
         *      method's execution, the size of the returned data.
         * @param[in,out] stamps the last modification time of the file that contain the
         *      texture image, or an empty vector if this file is not loaded yet. This
         *      modification time is updated by this method if it has changed. Each
         *      element of this vector contains a file name and its last modification
         *      time.
         */
        private GPUBuffer loadTextureData(XmlElement desc, string path,
                 byte[] data, ref int size, List<Tuple<string, DateTime>> stamps)
        {
            if (path.EndsWith("png", StringComparison.InvariantCultureIgnoreCase) ||
                path.EndsWith("bmp", StringComparison.InvariantCultureIgnoreCase) ||
                path.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    Bitmap img = new Bitmap(new MemoryStream(data));
                    TextureInternalFormat pif;
                    TextureFormat pf;
                    Sxta.Render.PixelType pt;
                    int txsize;
                    EnumConversion.ConvertPixelFormat(img.PixelFormat, out pif, out pf, out pt, out txsize);

                    desc.SetAttribute("width", img.Width.ToString());

                    if (string.IsNullOrWhiteSpace(desc.GetAttribute("height")))
                    {
                        desc.SetAttribute("height", img.Height.ToString());
                    }
                    if (string.IsNullOrWhiteSpace(desc.GetAttribute("format")))
                    {
                        desc.SetAttribute("format", pf.ToString());
                    }
                    if (string.IsNullOrWhiteSpace(desc.GetAttribute("type")))
                    {
                        desc.SetAttribute("type", pt.ToString());
                    }
                    if (string.IsNullOrWhiteSpace(desc.GetAttribute("internalformat")))
                    {
                        desc.SetAttribute("internalformat", pif.ToString());
                    }

                    // all formats except 'raw' store the image from top to bottom
                    // while OpenGL requires a bottom to top layout; so we revert the
                    // order of lines here to get a good orientation in OpenGL

                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    BitmapData Data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                    GPUBuffer buff = new GPUBuffer();
                    buff.setData(Data.Width * Data.Height * txsize, Data.Scan0, BufferUsage.STATIC_DRAW);
                    img.UnlockBits(Data);
                    DateTime t = File.GetLastWriteTime(path);
                    stamps.Add(new Tuple<string, DateTime>(path, t));
                    return buff;
                }
                catch (Exception ex)
                {
                    log.Error("Cannot load texture file '" + path + "', exception:" + ex.Message);
                    throw new Exception("Cannot load texture file '" + path + "'");
                }
            }
            else if (path.EndsWith("raw", StringComparison.InvariantCultureIgnoreCase))
            {
                bool raw = BitConverter.ToUInt32(data, size - 5 * sizeof(uint)) == 0xCAFEBABE;
                if (!raw)
                {
                    log.Error("Cannot load texture file '" + path + "', Invalid Raw Format");
                    throw new Exception("Cannot load texture file '" + path + "', Invalid Raw Format");
                }
                int w = BitConverter.ToInt32(data, size - 4 * sizeof(int)); // texture width
                int h = BitConverter.ToInt32(data, size - 3 * sizeof(int)); // texture height (ignored for 1D textures)
                int d = BitConverter.ToInt32(data, size - 2 * sizeof(int)); // texture depth (0 for 2D textures)
                int channels = BitConverter.ToInt32(data, size - 1 * sizeof(int)); // number of channels per pixel (1..4)
                if (d > 0)
                    desc.SetAttribute("depth", d.ToString());
                desc.SetAttribute("width", w.ToString());
                if (string.IsNullOrWhiteSpace(desc.GetAttribute("height")))
                {
                    desc.SetAttribute("height", h.ToString());
                }
                if (channels == 1)
                {
                    if (string.IsNullOrWhiteSpace(desc.GetAttribute("format")))
                    {
                        desc.SetAttribute("format", "RED");
                    }
                }
                else if (channels == 2)
                {
                    if (string.IsNullOrWhiteSpace(desc.GetAttribute("format")))
                    {
                        desc.SetAttribute("format", "RG");
                    }
                }
                else if (channels == 3)
                {
                    if (string.IsNullOrWhiteSpace(desc.GetAttribute("format")))
                    {
                        desc.SetAttribute("format", "RGB");
                    }
                }
                else if (channels == 4)
                {
                    if (string.IsNullOrWhiteSpace(desc.GetAttribute("format")))
                    {
                        desc.SetAttribute("format", "RGBA");
                    }
                }
                else
                {
                    log.Error("Cannot load texture file '" + path + "', Invalid Raw Format, Channels =" + channels);
                    throw new Exception("Cannot load texture file '" + path + "', Invalid Raw Format, Channels =" + channels);
                }
                if (string.IsNullOrWhiteSpace(desc.GetAttribute("type")))
                {
                    desc.SetAttribute("type", "FLOAT");
                }
                DateTime t = File.GetLastWriteTime(path);
                stamps.Add(new Tuple<string, DateTime>(path, t));
                GPUBuffer buff = new GPUBuffer();
                Debug.Assert(size - 5 * sizeof(uint) == w * channels * sizeof(float) * h);
                buff.setData(w * channels * sizeof(float) * h, data, BufferUsage.STATIC_DRAW);
                return buff;
            }
            else if (path.EndsWith("hdr", StringComparison.InvariantCultureIgnoreCase))
            {

                log.Error("HDR image format is not implemented'" + path);
                throw new NotImplementedException("HDR image format is not implemented'" + path);
            }
            log.Error("Unknown image format '" + path);
            throw new NotImplementedException("Unknown image format '" + path);

        }
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }

    /**
     * A ResourceDescriptor that also stores a set of last modification times.
     */
    public class XMLResourceDescriptor : ResourceDescriptor
    {

        //typedef vector< pair<string, time_t> > Stamps;

        /**
         * Creates a new XMLResourceDescriptor.
         *
         * @param descriptor the XML part of this descriptor.
         * @param data the ASCII or binary part of this descriptor.
         * @param size the size in bytes of the ASCII or binary part.
         * @param stamp the last modification time of the file that contain the XML
         *      part.
         * @param stamps the last modification time(s) of the file(s) that contain
         *      the ASCII or binary part.
         */
        public XMLResourceDescriptor(XmlElement descriptor, object data, int size,
                                     DateTime stamp, List<Tuple<string, DateTime>> dataStamps) :
            base(descriptor, data, size)
        {
            this.stamp = stamp;
            this.dataStamps = dataStamps;
        }

        /**
         * Deletes this XMLResourceDescriptor.
         */
        ~XMLResourceDescriptor()
        {
        }

        /**
         * Returns true if this descriptor is equal to the given one.
         *
         * @param desc the XML part of another XMLResourceDescriptor.
         * @param stamp the stamp of another XMLResourceDescriptor.
         * @param dataStamps the dataStamps of another XMLResourceDescriptor.
         * @return true if the XML parts of both XMLResourceDescriptor are equal and
         *      if the modification times of the binary parts are equal.
         */
        public bool equal(XmlElement desc, DateTime stamp, List<Tuple<string, DateTime>> dataStamps)
        {
            if (dataStamps.Count != this.dataStamps.Count)
            {
                return false;
            }
            for (int i = 0; i < dataStamps.Count; ++i)
            {
                if (dataStamps[i].Item2 != this.dataStamps[i].Item2)
                {
                    return false;
                }
            }
            if (stamp == this.stamp)
            {
                return true;
            }
            return equal(desc, this.descriptor);
        }

        /**
         * Returns true if the given XML elements are equal. This means that they
         * have the same attibutes and the same sub elements.
         */
        public static bool equal(XmlNode n1, XmlNode n2)
        {
            return XmlNode.Equals(n1, n2);
        }


        /**
         * The last modification time of the file that contains the XML part of this
         * %resource descriptor.
         */
        private DateTime stamp;

        /**
         * The last modification time(s) of the file(s) that contain the ASCII or
         * binary part of this %resource descriptor. Each element of this vector
         * contains a file name and its last modification time.
         */
        private List<Tuple<string, DateTime>> dataStamps;



        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    }
}
