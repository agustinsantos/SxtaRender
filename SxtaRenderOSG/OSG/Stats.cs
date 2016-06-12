using log4net;
using Sxta.Core;
using Sxta.Render.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sxta.OSG
{
    public class Stats
    {
        public Stats(string name)
        {
            _name = name;
            Allocate(25);
        }

        public Stats(string name, int numberOfFrames)
        {
            _name = name;
            Allocate(numberOfFrames);
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void Allocate(int numberOfFrames)
        {
            lock (_mutex)
            {
                _baseFrameNumber = 0;
                _latestFrameNumber = 0;
                _attributeMapList.Clear();
                _attributeMapList.Resize(numberOfFrames);
            }
        }


        public int getEarliestFrameNumber()
        {
            return _latestFrameNumber < _attributeMapList.Count ? 0 : _latestFrameNumber - _attributeMapList.Count + 1;
        }

        public int getLatestFrameNumber() { return _latestFrameNumber; }

        public bool setAttribute(int frameNumber, string attributeName, double value)
        {
            if (frameNumber < getEarliestFrameNumber()) return false;

            lock (_mutex)
            {

                if (frameNumber > _latestFrameNumber)
                {
                    // need to advance

                    // first clear the entries up to and including the new frameNumber
                    for (int i = _latestFrameNumber + 1; i <= frameNumber; ++i)
                    {
                        int index = (i - _baseFrameNumber) % _attributeMapList.Count;
                        _attributeMapList[index].Clear();
                    }

                    if ((frameNumber - _baseFrameNumber) >= _attributeMapList.Count)
                    {
                        _baseFrameNumber = (frameNumber / _attributeMapList.Count) * _attributeMapList.Count;
                    }

                    _latestFrameNumber = frameNumber;

                }

                int ind = getIndex(frameNumber);
                if (ind < 0)
                {
                    log.WarnFormat("Failed to assing valid index for Stats::setAttribute({0}, {1}, {2})", frameNumber, attributeName, value);
                    return false;
                }

                IDictionary<string, double> attributeMap = _attributeMapList[ind];
                attributeMap[attributeName] = value;

                return true;
            }
        }

        public bool getAttribute(int frameNumber, string attributeName, out double value)
        {
            lock (_mutex)
            {
                return getAttributeNoMutex(frameNumber, attributeName, out value);
            }
        }

        public bool getAveragedAttribute(string attributeName, out double value, bool averageInInverseSpace = false)
        {
            return getAveragedAttribute(getEarliestFrameNumber(), getLatestFrameNumber(), attributeName, out value, averageInInverseSpace);
        }


        public bool getAveragedAttribute(int startFrameNumber, int endFrameNumber, string attributeName, out double value, bool averageInInverseSpace = false)
        {
            value = 0.0;
            if (endFrameNumber < startFrameNumber)
            {
                Std.Swap(ref endFrameNumber, ref startFrameNumber);
            }

            lock (_mutex)
            {

                double total = 0.0;
                double numValidSamples = 0.0;
                for (int i = startFrameNumber; i <= endFrameNumber; ++i)
                {
                    double v = 0.0;
                    if (getAttributeNoMutex(i, attributeName, out v))
                    {
                        if (averageInInverseSpace) total += 1.0 / v;
                        else total += v;
                        numValidSamples += 1.0;
                    }
                }
                if (numValidSamples > 0.0)
                {
                    if (averageInInverseSpace) value = numValidSamples / total;
                    else value = total / numValidSamples;
                    return true;
                }
                else return false;
            }
        }

        public IDictionary<string, double> getAttributeMap(int frameNumber)
        {
            lock (_mutex)
            {
                return getAttributeMapNoMutex(frameNumber);
            }
        }

        //typedef std::map<std::string, bool> CollectMap;

        public void collectStats(string str, bool flag) { _collectMap[str] = flag; }

        public bool collectStats(string str)
        {
            lock (_mutex)
            {
                bool value;
                if (_collectMap.TryGetValue(str, out value))
                    return value;
                else
                    return false;
            }
        }

#if TODO
        public void report(std::ostream& out, const char* indent=0)  ;
        public void report(std::ostream& out,   int frameNumber, const char* indent=0)  ;
#endif

        //protected virtual ~Stats() {}

        protected bool getAttributeNoMutex(int frameNumber, string attributeName, out double value)
        {
            value = 0.0;
            int index = getIndex(frameNumber);
            if (index < 0) return false;

            IDictionary<string, double> attributeMap = _attributeMapList[index];

            return attributeMap.TryGetValue(attributeName, out value);
        }

        protected IDictionary<string, double> getAttributeMapNoMutex(int frameNumber)
        {
            int index = getIndex(frameNumber);
            if (index < 0) return _invalidAttributeMap;

            return _attributeMapList[index];
        }


        protected int getIndex(int frameNumber)
        {
            // reject frame that are in the future
            if (frameNumber > _latestFrameNumber) return -1;

            // reject frames that are too early
            if (frameNumber < getEarliestFrameNumber()) return -1;

            if (frameNumber >= _baseFrameNumber) return frameNumber - _baseFrameNumber;
            else return _attributeMapList.Count - (_baseFrameNumber - frameNumber);
        }

        protected string _name;

        protected object _mutex; //OpenThreads::Mutex

        protected int _baseFrameNumber;
        protected int _latestFrameNumber;

        //typedef std::map<std::string, double> AttributeMap;
        //typedef std::vector<AttributeMap> AttributeMapList;

        protected List<Dictionary<string, double>> _attributeMapList = new List<Dictionary<string, double>>(); // AttributeMapList
        protected IDictionary<string, double> _invalidAttributeMap = new Dictionary<string, double>(); //AttributeMap

        protected Dictionary<string, bool> _collectMap; // CollectMap

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    }
}