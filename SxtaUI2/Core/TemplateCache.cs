using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// Manages requests for loading templates, caching as it goes.
    /// </summary>
    static class TemplateCache
    {
        /// Initialisation and Shutdown
        public static bool Initialise()
        {
            return true;
        }

        public static void Shutdown()
        { }

        /// Load the named template from the given path, if its already loaded get the cached copy
        public static Template LoadTemplate(string name)
        {
            // Check if the template is already loaded
            if (templates.ContainsKey(name))
                return templates[name];

            // Nope, we better load it
            Template new_template = new Template();
            if (!new_template.Load(name))
            {
                //Log::Message(Log::LT_ERROR, "Failed to load template %s.", name.CString());
                new_template = null;
            }
            else if (string.IsNullOrWhiteSpace(new_template.GetName()))
            {
                //Log::Message(Log::LT_ERROR, "Failed to load template %s, template is missing its name.", name.CString());
                new_template = null;
            }
            else
            {
                templates[name] = new_template;
                template_ids[new_template.GetName()] = new_template;
            }


            return new_template;
        }

        /// Get the template by id
        public static Template GetTemplate(string id)
        {
            Template rst;
            // Check if the template is already loaded
            if (template_ids.TryGetValue(id, out rst))
                return rst;
            else
                return null;
        }

        /// Clear the template cache.
        public static void Clear()
        {
            templates.Clear();
            template_ids.Clear();
        }


        //typedef std::map<String, Template*> Templates;
        static Dictionary<string, Template> templates = new Dictionary<string, Template>();
        static Dictionary<string, Template> template_ids = new Dictionary<string, Template>();
    }
}
