using Sxta.Render.OSG;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.OSGViewer
{
    public enum ThreadingModel
    {
        SingleThreaded,
        CullDrawThreadPerContext,
        ThreadPerContext = CullDrawThreadPerContext,
        DrawThreadPerContext,
        CullThreadPerCameraDrawThreadPerContext,
        ThreadPerCamera = CullThreadPerCameraDrawThreadPerContext,
        AutomaticSelection
    }

    public enum BarrierPosition
    {
        BeforeSwapBuffers,
        AfterSwapBuffers
    }

    public enum FrameScheme
    {
        ON_DEMAND,
        CONTINUOUS
    }
    /// <summary>
    /// ViewerBase is the view base class that is inherited by both Viewer and CompositeViewer.
    /// </summary>
    public abstract class ViewerBase : IDisposable
    {
        public ViewerBase()
        {
            ViewerBaseInit();
        }

        /** Set/Get the Stats object used for collect various frame related timing and scene graph stats.*/
        public abstract Stats ViewerStats { get; set; }

        /** read the viewer configuration from a configuration file.*/
        public abstract bool ReadConfiguration(string filename);

        /** Get whether at least of one of this viewers windows are realized.*/
        public abstract bool IsRealized();

        /** set up windows and associated threads.*/
        public abstract void Realize();

        /** Set the threading model the rendering traversals will use.*/
        public ThreadingModel ThreadingModel
        {
            get { return _threadingModel; }
            set
            {
                if (_threadingModel == value) return;

                if (_threadsRunning) StopThreading();
                _threadingModel = value;
                if (IsRealized() && _threadingModel != ThreadingModel.SingleThreaded)
                    StartThreading();
            }
        }

        /** Let the viewer suggest the best threading model for the viewers camera/window setup and the hardware available.*/
        public virtual ThreadingModel SuggestBestThreadingModel()
        { throw new NotImplementedException(); }

        /** Set up the threading and processor affinity as per the viewers threading model.*/
        public virtual void setUpThreading() { throw new NotImplementedException(); }

        /** Return true if viewer threads are running. */
        public bool AreThreadsRunning() { return _threadsRunning; }

        /** Stop any threads begin run by viewer.*/
        public virtual void StopThreading() { throw new NotImplementedException(); }

        /** Start any threads required by the viewer.*/
        public virtual void StartThreading() { throw new NotImplementedException(); }


        /** Set the position of the end barrier.
         * AfterSwapBuffers may result in slightly higher framerates, but may
         * lead to inconsistent swapping between different windows.
         * BeforeSwapBuffers may lead to slightly lower framerate, but improve consistency in timing of swap buffers,
         * especially important if you are likely to consistently break frame.*/
        public void setEndBarrierPosition(BarrierPosition bp) { throw new NotImplementedException(); }


        /** Get the end barrier position.*/
        public BarrierPosition getEndBarrierPosition() { return _endBarrierPosition; }


        /** Set the end barrier operation. \c op may be one of GL_FLUSH, GL_FINISH,
         * or NO_OPERATION. NO_OPERATION is the default. Per BarrierOperation::operator()(),
         * a glFlush() command, glFinish() command, or no additional OpenGL command will be
         * issued before entering the end barrier. */
        public void setEndBarrierOperation(BarrierOperation.PreBlockOp op) { throw new NotImplementedException(); }


        /** Get the end barrier operation. */
        public BarrierOperation.PreBlockOp EndBarrierOperation
        {
            get { return _endBarrierOperation; }
            set { throw new NotImplementedException(); }
        }


        /** Set the done flag to signal the viewer's work is done and should exit the frame loop.*/
        public void SetDone(bool done) { _done = done; }

        /** Return true if  viewer's work is done and should exit the frame loop.*/
        public bool Done() { return _done; }

        /** get/Set the key event that the viewer checks on each frame to see if the viewer's done flag should be set to
        * signal end of viewers main loop.
        * Default value is Escape (osgGA::GUIEVentAdapter::KEY_Escape).
        * Setting to 0 switches off the feature.*/
        public int KeyEventSetsDone
        {
            set { _keyEventSetsDone = value; }
            get { return _keyEventSetsDone; }
        }

        /** if the flag is true, the viewer set its done flag when a QUIT_APPLICATION is received, false disables this feature */
        public bool QuitEventSetsDone
        {
            set { _quitEventSetsDone = value; }
            get { return _quitEventSetsDone; }
        }

        /** Hint to tell the renderingTraversals() method whether to call relaseContext() on the last
          * context that was made current by the thread calling renderingTraverals().  Note, when
          * running multi-threaded viewer no threads will be made current or release current.
          * Setting this hint to false can enable the frame loop to be lazy about calling makeCurrent
          * and releaseContext on each new frame, helping performance.  However, if you frame loop
          * is managing multiple graphics context all from the main frame thread then this hint must
          * be left on, otherwise the wrong context could be left active, introducing errors in rendering.*/
        public bool ReleaseContextAtEndOfFrameHint
        {
            set { _releaseContextAtEndOfFrameHint = value; }
            get { return _releaseContextAtEndOfFrameHint; }
        }

#if TODO
        /** Get/Set the UpdateVisitor. */
        public UpdateVisitor setUpdateVisitor
        {
            set { _updateVisitor = updateVisitor; }
            get { return _updateVisitor.get(); }

        }
#endif

        /** Get/Set the Update OperationQueue. */
        public OperationQueue UpdateOperations
        {
            set { _updateOperations = value; }
            get { return _updateOperations; }
        }

        /** Add an update operation.*/
        public void AddUpdateOperation(Operation operation)
        { throw new NotImplementedException(); }

        /** Remove an update operation.*/
        public void RemoveUpdateOperation(Operation operation)
        { throw new NotImplementedException(); }

        /** Set the graphics operation to call on realization of the viewers graphics windows.*/
        public Operation RealizeOperation
        {
            set { _realizeOperation = value; }
            get { return _realizeOperation; }
        }


#if TODO
        /** Set the incremental compile operation.
          * Used to manage the OpenGL object compilation and merging of subgraphs in a way that avoids overloading
          * the rendering of frame with too many new objects in one frame. */
        public void setIncrementalCompileOperation(osgUtil::IncrementalCompileOperation* ico);

        /** Get the incremental compile operation. */
        public osgUtil::IncrementalCompileOperation* getIncrementalCompileOperation() { return _incrementalCompileOperation.get(); }
#endif
        public FrameScheme RunFrameScheme
        {
            get { return _runFrameScheme; }
            set { _runFrameScheme = value; }
        }

        public double RunMaxFrameRate
        {
            get { return _runMaxFrameRate; }
            set { _runMaxFrameRate = value; }
        }

        /** Execute a main frame loop.
          * Equivalent to while (!viewer.done()) viewer.frame();
          * Also calls realize() if the viewer is not already realized,
          * and installs trackball manipulator if one is not already assigned.
          */
        public virtual int Run()
        {
#if TODO
            if (!isRealized())
            {
                realize();
            }

            string run_frame_count_str = getenv("OSG_RUN_FRAME_COUNT");
            int runTillFrameNumber = run_frame_count_str == 0 ? UNINITIALIZED_FRAME_NUMBER : int.Parse(run_frame_count_str);

            while (!done() && (run_frame_count_str == 0 || getViewerFrameStamp().getFrameNumber() < runTillFrameNumber))
            {
                double minFrameTime = _runMaxFrameRate > 0.0 ? 1.0 / _runMaxFrameRate : 0.0;
                Timer_t startFrameTick = Timer.instance().tick();
                if (_runFrameScheme == ON_DEMAND)
                {
                    if (checkNeedToDoFrame())
                    {
                        frame();
                    }
                    else
                    {
                        // we don't need to render a frame but we don't want to spin the run loop so make sure the minimum
                        // loop time is 1/100th of second, if not otherwise set, so enabling the frame microSleep below to
                        // avoid consume excessive CPU resources.
                        if (minFrameTime == 0.0) minFrameTime = 0.01;
                    }
                }
                else
                {
                    frame();
                }

                // work out if we need to force a sleep to hold back the frame rate
                Timer_t endFrameTick = Timer.instance().tick();
                double frameTime = Timer.instance().delta_s(startFrameTick, endFrameTick);
                if (frameTime < minFrameTime)
                    OpenThreads.Thread.microSleep(1000000.0 * (minFrameTime - frameTime));
            }

            return 0;
#endif
            throw new NotImplementedException();
        }

        /** check to see if the new frame is required, called by run(..) when FrameScheme is set to ON_DEMAND.*/
        public abstract bool CheckNeedToDoFrame();

        /** check to see if events have been received, return true if events are now available.*/
        public abstract bool CheckEvents();

        /** Render a complete new frame.
          * Calls advance(), eventTraversal(), updateTraversal(), renderingTraversals(). */
        public void Frame(double simulationTime = USE_REFERENCE_TIME)
        {
#if TODO
            if (_done) return;

            // OSG_NOTICE<<std::endl<<"CompositeViewer::frame()"<<std::endl<<std::endl;

            if (_firstFrame)
            {
                viewerInit();

                if (!isRealized())
                {
                    realize();
                }

                _firstFrame = false;
            }
            advance(simulationTime);

            eventTraversal();
            updateTraversal();
            renderingTraversals();
#endif
            throw new NotImplementedException();
        }

        public abstract void Advance(double simulationTime = USE_REFERENCE_TIME);

        public abstract void EventTraversal();

        public abstract void UpdateTraversal();

        public void RenderingTraversals()
        {
            throw new NotImplementedException();
        }

#if TODO
        //typedef std::vector<osg::Camera*> Cameras;
        public abstract void GetCameras(List<Camera> cameras, bool onlyActive = true);

        //typedef std::vector<osg::GraphicsContext*> Contexts;
        public virtual void GetContexts(List<GraphicsContext> contexts, bool onlyValid = true);

        //typedef std::vector<osgViewer::GraphicsWindow*> Windows;
        public virtual void GetWindows(List<GraphicsWindow> windows, bool onlyValid = true);

        //typedef std::vector<OpenThreads::Thread*> Threads;
        public abstract void GetAllThreads(List<Thread> threads, bool onlyActive = true);

        //typedef std::vector<osg::OperationThread*> OperationThreads;
        public abstract void GetOperationThreads(List<OperationThread> threads, bool onlyActive = true);

        //typedef std::vector<osgViewer::Scene*> Scenes;
        public abstract void GetScenes(List<Scene> scenes, bool onlyValid = true);

        //typedef std::vector<osgViewer::View*> Views;
        public abstract void GetViews(List<View> views, bool onlyValid = true);

        /** Check to see if any windows are still open. If not, set viewer done to true. */
        public void CheckWindowStatus();
        /** Check to see if windows are still open using the list of contexts given as a parameter.
         *  If no windows are open, stop rendering threads and set viewer done to true.
         *  This function is more effective than checkWindowStatus() as it does not query
         *  the context list and should be used whenever context list is already available in your code.*/
       public  void CheckWindowStatus(List<Context> contexts);

        public abstract double ElapsedTime();

        public abstract FrameStamp GetViewerFrameStamp();

        /** Get the keyboard and mouse usage of this viewer.*/
        public abstract void getUsage(ApplicationUsage usage); 

        protected   void makeCurrent(GraphicsContext gc)
        {
            if (_currentContext==gc) return;

            releaseContext();

            if (gc && gc.valid() && gc.makeCurrent()) _currentContext = gc;
        }

        protected  void releaseContext()
        {
            if (_currentContext.valid() && _currentContext->valid())
            {
                _currentContext.releaseContext();
            }
            _currentContext = 0;
        }
#endif
        protected abstract void viewerInit();

        protected bool _firstFrame;
        protected bool _done;
        protected int _keyEventSetsDone;
        protected bool _quitEventSetsDone;
        protected bool _releaseContextAtEndOfFrameHint;

        protected ThreadingModel _threadingModel;
        protected bool _threadsRunning;

        protected bool _requestRedraw;
        protected bool _requestContinousUpdate;

        protected FrameScheme _runFrameScheme;
        protected double _runMaxFrameRate;


        protected BarrierPosition _endBarrierPosition;
        protected BarrierOperation.PreBlockOp _endBarrierOperation;

        protected BarrierOperation _startRenderingBarrier;
        protected BarrierOperation _endRenderingDispatchBarrier;
#if TODO
        protected EndOfDynamicDrawBlock _endDynamicDrawBlock;


        protected osgGA.EventVisitor _eventVisitor;
#endif
        protected OperationQueue _updateOperations;
        protected OSGUtil.UpdateVisitor _updateVisitor;

        protected Operation _realizeOperation;
        protected OSGUtil.IncrementalCompileOperation _incrementalCompileOperation;

#if TODO
        protected observer_ptr<GraphicsContext> _currentContext;
#endif

        public const double USE_REFERENCE_TIME = double.MaxValue;


        protected void ViewerBaseInit()
        {
            _firstFrame = true;
            _done = false;
#if TODO
            _keyEventSetsDone = osgGA.GUIEventAdapter.KEY_Escape;
#endif 
            throw new NotImplementedException();
            _quitEventSetsDone = true;
            _releaseContextAtEndOfFrameHint = true;
            _threadingModel = ThreadingModel.AutomaticSelection;
            _threadsRunning = false;
            _endBarrierPosition = BarrierPosition.AfterSwapBuffers;
            _endBarrierOperation = BarrierOperation.PreBlockOp.NO_OPERATION;
            _requestRedraw = true;
            _requestContinousUpdate = false;

            _runFrameScheme = FrameScheme.CONTINUOUS;
            _runMaxFrameRate = 0.0f;

            string str = ConfigurationManager.AppSettings["OSG_RUN_FRAME_SCHEME"];
            if (!String.IsNullOrWhiteSpace(str))
            {
                if      (str=="ON_DEMAND") _runFrameScheme = FrameScheme.ON_DEMAND;
                else if (str=="CONTINUOUS") _runFrameScheme = FrameScheme.CONTINUOUS;
            }

            str = ConfigurationManager.AppSettings["OSG_RUN_MAX_FRAME_RATE"];
            if (!String.IsNullOrWhiteSpace(str))
            {
                _runMaxFrameRate = Double.Parse(str);
            }  
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
