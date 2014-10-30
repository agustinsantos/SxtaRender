using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxta.Render.Core
{
    
    /// <summary>
    /// Block is a block that can be used to halt a thread that is waiting another thread to release it.
    /// </summary>
	public class Block
    {
        public Block()
        {
            throw new NotImplementedException();
        }

        ~Block()
        {
            Release();
        }

        public bool Enter() //Original Block()
        {
            lock (this)
            {
                if (!_released)
                {
                    return Monitor.Wait(this) == false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool Enter(int timeout) // Original Block(int timeout)
        {
            lock (this)
            {
                if (!_released)
                {
                    return Monitor.Wait(this, timeout) == false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void Release()
        {
            lock (this)
            {
                if (!_released)
                {
                    _released = true;
                    Monitor.PulseAll(this);
                }
            }
        }

        public void Reset()
        {
            lock (this)
            {
                _released = false;
            }
        }

        public void Set(bool doRelease)
        {
            if (doRelease != _released)
            {
                if (doRelease) Release();
                else Reset();
            }
        }

        protected bool _released = false;
    }
}
