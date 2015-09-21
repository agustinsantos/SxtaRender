using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    internal static class PluginRegistry
    {
        public static void RegisterPlugin(Plugin plugin)
        {
            throw new NotImplementedException();
        }

        /// Calls OnInitialise() on all plugins.
        public static void NotifyInitialise()
        {
            throw new NotImplementedException();
        }

        /// Calls OnShutdown() on all plugins.
        public static void NotifyShutdown()
        {
            throw new NotImplementedException();
        }


        /// Calls OnContextCreate() on all plugins.
        public static void NotifyContextCreate(Context context)
        {
            throw new NotImplementedException();
        }

        /// Calls OnContextDestroy() on all plugins.
        public static void NotifyContextDestroy(Context context)
        {
            throw new NotImplementedException();
        }


        /// Calls OnDocumentOpen() on all plugins.
        public static void NotifyDocumentOpen(Context context, string document_path)
        {
           // throw new NotImplementedException();
        }

        /// Calls OnDocumentLoad() on all plugins.
        public static void NotifyDocumentLoad(ElementDocument document)
        {
            throw new NotImplementedException();
        }

        /// Calls OnDocumentUnload() on all plugins.
        public static void NotifyDocumentUnload(ElementDocument document)
        {
            throw new NotImplementedException();
        }


        /// Calls OnElementCreate() on all plugins.
        public static void NotifyElementCreate(Element element)
        {
            throw new NotImplementedException();
        }

        /// Calls OnElementDestroy() on all plugins.
        public static void NotifyElementDestroy(Element element)
        {
            throw new NotImplementedException();
        }
    }
}
