using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.Render.OSG
{
    /** GraphicsThread is a helper class for running OpenGL GraphicsOperation within a single thread assigned to a specific GraphicsContext.*/
    public class GraphicsThread : OperationThread
    {
        public GraphicsThread()
        {
            throw new NotImplementedException();
        }

        /** Run does the graphics thread run loop.*/
        public virtual void run()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class GraphicsOperation : Operation
    {
        public GraphicsOperation(string name, bool keep) :
            base(name, keep) { }

        /** Override the standard Operation operator and dynamic cast object to a GraphicsContext,
          * on success call operation()(GraphicsContext*).*/
        public override void Do(Object obj)
        {
            throw new NotImplementedException();
        }

        public abstract void Do(GraphicsContext context);
    }


    /** SwapBufferOperation calls swap buffers on the GraphicsContext.*/
    public class SwapBuffersOperation : GraphicsOperation
    {
        public SwapBuffersOperation() :
            base("SwapBuffers", true) { }

        public override void Do(GraphicsContext context)
        {
            throw new NotImplementedException();
        }
    }

    /** BarrierOperation allows one to synchronize multiple GraphicsThreads with each other.*/
    public class BarrierOperation : Operation //, public OpenThreads::Barrier
    {
        public enum PreBlockOp
        {
            NO_OPERATION,
            GL_FLUSH,
            GL_FINISH
        };

        public BarrierOperation(int numThreads, PreBlockOp op = PreBlockOp.NO_OPERATION, bool keep = true)
            : base("Barrier", keep)
        {
#if TODO
            OpenThreads.Barrier(numThreads);
#endif
            _preBlockOp = op;
        }

        public virtual void Release()
        {
            throw new NotImplementedException();
        }

        public override void Do(Object obj)
        {
            throw new NotImplementedException();
        }

        public PreBlockOp _preBlockOp;
    }

    /** ReleaseContext_Block_MakeCurrentOperation releases the context for another thread to acquire,
      * then blocks waiting for context to be released, once the block is release the context is re-acquired.*/
    public class ReleaseContext_Block_MakeCurrentOperation : GraphicsOperation //, public RefBlock
    {
        public ReleaseContext_Block_MakeCurrentOperation() :
            base("ReleaseContext_Block_MakeCurrent", false) { }

        public virtual void Release()
        {
            throw new NotImplementedException();
        }

        public override void Do(GraphicsContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class BlockAndFlushOperation : GraphicsOperation //, public OpenThreads.Block
    {
        public BlockAndFlushOperation()
            : base("TODO", false)
        {
            throw new NotImplementedException();
        }

        public override void Release()
        {
            throw new NotImplementedException();
        }

        public override void Do(GraphicsContext context)
        {
            throw new NotImplementedException();
        }
    }


    public class FlushDeletedGLObjectsOperation : GraphicsOperation
    {
        public FlushDeletedGLObjectsOperation(double availableTime, bool keep = false) :
            base("TODO", false)
        {
            throw new NotImplementedException();
        }

        public override void Do(GraphicsContext context)
        {
            throw new NotImplementedException();
        }

        public double _availableTime;
    }

    public class RunOperations : GraphicsOperation
    {
        public RunOperations() :
            base("RunOperation", true) { }

        public override void Do(GraphicsContext context)
        {
            throw new NotImplementedException();
        }
    }

#if TODO
    public class EndOfDynamicDrawBlock : State.DynamicObjectRenderingCompletedCallback //OpenThreads::BlockCount,  
    {
        public EndOfDynamicDrawBlock(int p);

        public void completed(State state);

    }
#endif
}
