using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// core API.
    /// </summary>
    public static class CoreEngine
    {
        /// Initialises Rocket.
        public static bool Initialise()
        {
            Factory.Initialise();
            initialised = true;

            return true;
        }
        /// Shutdown Rocket.
        public static void Shutdown()
        {
            Factory.Shutdown();
            initialised = false;
        }

        /// Returns the version of this Rocket library.
        /// @return The version number.
        public static String GetVersion()
        {
            Assembly assembly = typeof(CoreEngine).Assembly;
            Version ver = assembly.GetName().Version;
            return ver.ToString();
        }

        /// Sets the interface through which all system requests are made. This must be called before Initialise().
        /// @param[in] system_interface The application-specified logging interface.
        public static void SetSystemInterface(SystemInterface sysinterface)
        {
            if (system_interface == sysinterface)
                return;


            system_interface = sysinterface;
        }

        /// Returns Rocket's system interface.
        /// @return Rocket's system interface.
        public static SystemInterface GetSystemInterface()
        {
            return system_interface;
        }


        private static bool initialised = false;
        private static SystemInterface system_interface = new DefaultSystemInterface();
    }
}
