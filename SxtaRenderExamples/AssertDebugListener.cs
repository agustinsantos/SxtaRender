using System;
using System.Diagnostics;

namespace Examples
{
    public class AssertDebugListener : TextWriterTraceListener
    {
        public AssertDebugListener(string filename) : base(filename)
        { }

        public override void Write(string message)
        {
            if (message == "Fail: ")
                throw new Exception("Debug.Assert failed at " + Environment.StackTrace);
            base.Write(message);
        }

        public override void WriteLine(string message)
        {
            if (message.Equals("Fail: "))
                throw new Exception("Debug.Assert failed at " + Environment.StackTrace);
            base.WriteLine(message);
        }
    }
}
