using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// This class provides interfaces for Time, Translation and Logging.
    /// Time is the only required implementation.
    /// The default implemention of Translation doesn't translate anything
    /// The default implementation of logging logs Windows Debug Console,
    /// or Standard Error, depending on what platform you're using.
    /// </summary>
    public abstract class SystemInterface
    {
        /// Get the number of seconds elapsed since the start of the application.
        /// @return Elapsed time, in seconds.
        public abstract float GetElapsedTime();

        /// Translate the input string into the translated string.
        /// @param[out] translated Translated string ready for display.
        /// @param[in] input String as received from XML.
        /// @return Number of translations that occured.
        public virtual int TranslateString(out string translated, string input)
        {
            translated = input;
            return 0;
        }

        /// Joins the path of an RML or RCSS file with the path of a resource specified within the file.
        /// @param[out] translated_path The joined path.
        /// @param[in] document_path The path of the source document (including the file name).
        /// @param[in] path The path of the resource specified in the document.
        public virtual void JoinPath(out string translated_path, string document_path, string path)
        {
            // If the path is absolute, strip the leading / and return it.
            if (path.StartsWith("/"))
            {
                translated_path = path.Substring(1);
                return;
            }

            // If the path is a Windows-style absolute path, return it directly.
            int drive_pos = path.IndexOf(":");
            int slash_pos = Math.Min(path.IndexOf("/"), path.IndexOf("\\"));
            if (drive_pos != -1 && drive_pos < slash_pos)
            {
                translated_path = path;
                return;
            }

            // Strip off the referencing document name.
            translated_path = document_path;
            translated_path = translated_path.Replace("\\", "/");
             int file_start = translated_path.LastIndexOf("/");
            if (file_start != -1)
                translated_path = translated_path.Substring(0, file_start);
            else
                translated_path = "";

            // Append the paths and send through URL to removing any '..'.
            //Uri url = new Uri(translated_path.Replace(":", "|") + path.Replace("\\", "/"));
            translated_path = Path.GetFullPath(Path.Combine(translated_path, path));
         }

        /// Activate keyboard (for touchscreen devices)
        public virtual void ActivateKeyboard()
        {
        }

        /// Deactivate keyboard (for touchscreen devices)
        public virtual void DeactivateKeyboard()
        {
        }
    }

    class DefaultSystemInterface : SystemInterface
    {
        public override float GetElapsedTime()
        {
            throw new NotImplementedException();
        }
    }

}
