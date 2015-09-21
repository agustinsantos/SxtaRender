using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxtaUI2.Core
{
    /// <summary>
    /// Base class for all objects that hold a scriptable object.
    /// </summary>
    public interface ScriptInterface
    {
        object GetScriptObject();
    }
}
