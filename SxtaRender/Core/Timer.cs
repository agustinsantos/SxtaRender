using System;
using System.Diagnostics;

namespace Sxta.Core
{
    /// <summary>
    /// A timer to measure time and time intervals.
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// Creates a new timer.
        /// </summary>
        public Timer()
        {
            numCycles = 0;
            running = false;
            minDuration = 1e9;
            maxDuration = 0.0;
            lastDuration = 0.0;
            totalDuration = 0.0;
        }


        /// <summary>
        /// Starts this timer and returns the current time in micro seconds.
        /// </summary>
        /// <returns></returns>
        public virtual double start()
        {
            running = true;
            numCycles++;
            t = getCurrentTime();
            return t;
        }


        /// <summary>
        /// Returns the delay since the last call to #start() in micro seconds.
        /// </summary>
        /// <returns></returns>
        public virtual double end()
        {
            lastDuration = getCurrentTime() - t;
            totalDuration += lastDuration;
            minDuration = System.Math.Min(lastDuration, minDuration);
            maxDuration = System.Math.Max(lastDuration, maxDuration);
            running = false;
            return lastDuration;
        }


        /// <summary>
        /// Returns the delay recorded at the last end() call.
        /// </summary>
        /// <returns></returns>
        public virtual double getTime()
        {
            return lastDuration;
        }


        /// <summary>
        /// Returns the average delay at every call to #end() in micro seconds.
        /// This won't be accurate if the timer is not stopped.
        /// </summary>
        /// <returns></returns>
        public virtual double getAvgTime()
        {
            if (numCycles == 0)
            {
                return 0.0;
            }
            if (running)
            {
                end();
            }
            return totalDuration / numCycles;
        }


        /// <summary>
        /// Returns the number of calls to start since last reset().
        /// </summary>
        /// <returns></returns>
        public int getNumCycles()
        {
            return numCycles;
        }

        /// <summary>
        /// Returns the lowest duration between a start() and an end() call in micro seconds.
        /// </summary>
        /// <returns></returns>
        public double getMinDuration()
        {
            return minDuration;
        }

        /// <summary>
        /// Returns the highest duration between a start() and an end() call in micro seconds.
        /// </summary>
        /// <returns></returns>
        public double getMaxDuration()
        {
            return maxDuration;
        }

        /// <summary>
        /// Resets the statistics (total, average, min, and max durations).
        /// </summary>
        public virtual void reset()
        {
            start();
            numCycles = 0;
            running = false;
            minDuration = double.MaxValue;
            maxDuration = 0.0;
            lastDuration = 0.0;
            totalDuration = 0.0;
        }


        /// <summary>
        /// Get a string based on the current date and time of the day.
        /// Buffer must be of sufficient length.
        /// Format YYYY.MM.DD.HH.MM.SS
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufSize"></param>
        public static void getDateTimeString(ref String buffer, int bufSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a string based on the current date.
        /// Buffer must be of sufficient length.
        /// Format YYYY.MM.DD
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufSize"></param>
        public static void getDateString(ref String buffer, int bufSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a string based on the current time of the day.
         /// Buffer must be of sufficient length.
        /// Format HH.MM.SS
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufSize"></param>
        public static void getTimeOfTheDayString(ref String buffer, int bufSize)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Time of last call to  start or  reset.
        /// </summary>
        protected double t;

        /// <summary>
        /// The accumulated elapsed time.
        /// </summary>
        protected double totalDuration;

        /// <summary>
        /// Number of calls to start since last reset() call.
        /// </summary>
        protected int numCycles;

        /// <summary>
        /// Last recorded duration recorded at #end() call.
        /// </summary>
        protected double lastDuration;

        /// <summary>
        ///  The lowest duration between a #start() and an #end() call in micro seconds.
        /// </summary>
        protected double minDuration;

        /// <summary>
        /// The highest duration between a #start() and an #end() call in micro seconds.
        /// </summary>
        protected double maxDuration;

        /// <summary>
        /// True if the timer has a start value.
        /// </summary>
        protected bool running;

        /// <summary>
        /// Returns the current time in microseconds.
        ///  The origin of time may depend on the platform.
        /// </summary>
        /// <returns></returns>
        protected double getCurrentTime()
        {
            return sw.ElapsedTicks / (double)Stopwatch.Frequency * 1000000;
        }

        private Stopwatch sw = new Stopwatch();
    }
}
