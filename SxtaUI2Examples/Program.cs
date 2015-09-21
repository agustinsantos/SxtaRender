using SxtaUI2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SxtaUI2Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadRmlFile();
        }

        private static void ReadRmlFile()
        {
            CoreEngine.Initialise();
            Console.WriteLine("Version: " + CoreEngine.GetVersion());
            ElementDocument root = new ElementDocument("root");
            XMLParser parser = new XMLParser(root);
            parser.Parse("Samples/Assets/demo.rml");
        }
    }
}
