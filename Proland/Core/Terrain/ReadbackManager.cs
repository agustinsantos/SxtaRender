using Sxta.Render;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proland
{
    public class ReadbackManager// : Object
    {
        const uint BUFFER_ALIGNMENT = 32;

        /// <summary>
        /// A callback function called when a readback is done; see ReadbackManager.
        /// </summary>
        public interface Callback //TODO Inheritance from Object ("ReadbackManager::Callback")
        {
            /// <summary>
            /// Called when a readback is finished.
            /// </summary>
            void dataRead(Object data);
        }

        /// <summary>
        /// Creates a new readback manager.
        /// </summary>
        /// <param name="maxReadbackPerFrame"> maximum number of readbacks that can be
        /// started per frame.</param>
        /// <param name="readbackDelay">number of frames between the start of a readback
        /// and its end.</param>
        /// <param name="bufferSize">bufferSize maximum number of bytes per readback.</param>
        public ReadbackManager(int maxReadbackPerFrame, int readbackDelay, int bufferSize)
        {
            this.maxReadbackPerFrame = maxReadbackPerFrame;
            this.readbackDelay = readbackDelay;
            this.bufferSize = bufferSize;

            readCount = new int[readbackDelay];
            GPUBuffer[][] toRead = new GPUBuffer[readbackDelay][];
            Callback[][] toReadCallbacks = new Callback[readbackDelay][];

            for (int i = 0; i < readbackDelay; ++i)
            {
                readCount[i] = 0;
                toRead[i] = new GPUBuffer[maxReadbackPerFrame];
                toReadCallbacks[i] = new Callback[maxReadbackPerFrame];
                for (int j = 0; j < maxReadbackPerFrame; ++j)
                {
                    toRead[i][j] = new GPUBuffer();
                    toRead[i][j].setData(bufferSize, (IntPtr)null, BufferUsage.STREAM_READ);
                }
            }
        }

        /// <summary>
        /// Returns true if a new readback can be started for the current frame.
        /// </summary>
        /// <returns></returns>
        public bool canReadback()
        {
            return readCount[0] < maxReadbackPerFrame;
        }

        /// <summary>
        /// Starts a new readback and returns immediately. Returns true if the
        ///readback has effectively started(see #canReadback()).
        /// </summary>
        /// <param name="fb">the framebuffer from which the data must be read. Data will
        /// be read from the last buffer specified with FrameBuffer#setReadBuffer
        /// for this framebuffer.</param>
        /// <param name="x">x coordinate of the lower left corner of the region to be read.</param>
        /// <param name="y">x coordinate of the lower left corner of the region to be read.</param>
        /// <param name="w">width of the region to be read.</param>
        /// <param name="h">height the region to be read.</param>
        /// <param name="f">the components to be read.</param>
        /// <param name="t">the type to be used to store the read data.</param>
        /// <param name="cb">the function to be called when the readback is finished.</param>
        /// <returns></returns>
        public bool readback(FrameBuffer fb, int x, int y, int w, int h, TextureFormat f, PixelType t, Callback cb)
        {
            if (readCount[0] < maxReadbackPerFrame)
            {
                int index = readCount[0];
                fb.readPixels(x, y, w, h, f, t, new Sxta.Render.Buffer.Parameters(), toRead[0][index]);
                toReadCallbacks[0][index] = cb;
                ++readCount[0];
                return true;
            }
            else
            {
                Debug.Assert(false); // should not happen, call canReadback before
                return false;
            }
        }

        /// <summary>
        /// Informs this manager that a new frame has started.
        /// </summary>
        public void newFrame()
        {
            int lastIndex = readbackDelay - 1;
            for (int i = 0; i < readCount[lastIndex]; ++i)
            {
                BufferAccess a = BufferAccess.READ_ONLY;
                Object data = toRead[lastIndex][i].map(a);
                toReadCallbacks[lastIndex][i].dataRead(data);
                toReadCallbacks[lastIndex][i] = null;
                toRead[lastIndex][i].unmap();
            }

            // rotate buffer to the left and clear readCount
            GPUBuffer[] bufs = toRead[lastIndex];
            Callback[] calls = toReadCallbacks[lastIndex];

            for (int i = readbackDelay - 1; i > 0; --i)
            {
                readCount[i] = readCount[i - 1];
                toRead[i] = toRead[i - 1];
                toReadCallbacks[i] = toReadCallbacks[i - 1];
            }
            readCount[0] = 0;
            toRead[0] = bufs;
            toReadCallbacks[0] = calls;
        }


        private int maxReadbackPerFrame;
        private int readbackDelay;
        private int[] readCount;
        private GPUBuffer[][] toRead;
        private Callback[][] toReadCallbacks;
        private int bufferSize;
    };
}

