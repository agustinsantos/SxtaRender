using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxta.OSG
{
    public class RenderInfo
    {
        public RenderInfo() { }

        public RenderInfo(RenderInfo rhs)
        {
            this._state = rhs._state;
            this._view = rhs._view;
            this._cameraStack = rhs._cameraStack;
            this._renderBinStack = rhs._renderBinStack;
            this._userData = rhs._userData;
        }

        RenderInfo(State state, View view)
        {
            this._state = state;
            this._view = view;
        }

#if TODO
        public RenderInfo operator = ( RenderInfo rhs)
    {
        _state = rhs._state;
        _view = rhs._view;
        _cameraStack = rhs._cameraStack;
        _renderBinStack = rhs._renderBinStack;
        _userData = rhs._userData;
        return *this;
    }
#endif

        public uint getContextID() { return _state != null ? _state.getContextID() : 0; }

        public void setState(State state) { _state = state; }
        public State getState() { return _state; }

        public void setView(View view) { _view = view; }
        public View getView() { return _view; }

        public void pushCamera(Camera camera) { _cameraStack.Enqueue(camera); }
        public void popCamera() { if ( _cameraStack.Count != 0) _cameraStack.Dequeue(); }

        //typedef std::vector<Camera*> CameraStack;
        public Queue<Camera> getCameraStack() { return _cameraStack; }

        public Camera getCurrentCamera() { return _cameraStack.Count == 0 ? null : _cameraStack.Dequeue(); }

        public void pushRenderBin(RenderBin bin) { _renderBinStack.Enqueue(bin); }
        public void popRenderBin() { _renderBinStack.Dequeue(); }

        // typedef std::vector<osgUtil::RenderBin*> RenderBinStack;
        public Queue<RenderBin>  getRenderBinStack() { return _renderBinStack; }

        public void setUserData(object userData) { _userData = userData; }
        public object getUserData() { return _userData; }




        protected State _state;
        protected View _view = null;
        protected Queue<Camera> _cameraStack;
        protected Queue<RenderBin> _renderBinStack;
        protected object _userData;

    }
}