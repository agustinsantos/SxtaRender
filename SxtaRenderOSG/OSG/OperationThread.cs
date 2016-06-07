using OpenTK.Graphics.OpenGL;
using Sxta.Render.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxta.OSG
{
    /// <summary>
    /// Base class for implementing graphics operations.
    /// </summary>
    public abstract class Operation
    {
        public Operation(string name, bool keep)
        {
            _name = name;
            _keep = keep;
        }

        /** Get/Set the human readable name of the operation.*/
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /** Get/Set whether the operation should be kept once its been applied.*/
        public bool Keep
        {
            get { return _keep; }
            set { _keep = value; }
        }

        /** if this operation is a barrier then release it.*/
        public virtual void Release() { }

        /** Do the actual task of this operation.*/
        public abstract void Do(Object p);

        protected Operation()
        {
            _keep = false;
        }

        protected Operation(Operation op)
        {
            _name = op._name;
            _keep = op._keep;
        }

        protected string _name;
        protected bool _keep;
    }

    public class OperationQueue
    {
        public OperationQueue()
        {
            throw new NotImplementedException();
        }

        /** Get the next operation from the operation queue.
          * Return null ref_ptr<> if no operations are left in queue. */
        public Operation GetNextOperation(bool blockIfEmpty = false)
        {
            throw new NotImplementedException();
        }

        /** Return true if the operation queue is empty. */
        public bool Empty()
        {
            throw new NotImplementedException();
        }

        /** Return the num of pending operations that are sitting in the OperationQueue.*/
        public int GetNumOperationsInQueue()
        {
            throw new NotImplementedException();
        }

        /** Add operation to end of OperationQueue, this will be
          * executed by the operation thread once this operation gets to the head of the queue.*/
        public void Add(Operation operation)
        {
            throw new NotImplementedException();
        }

        /** Remove operation from OperationQueue.*/
        public void Remove(Operation operation)
        {
            throw new NotImplementedException();
        }

        /** Remove named operation from OperationQueue.*/
        public void Remove(string name)
        {
            throw new NotImplementedException();
        }

        /** Remove all operations from OperationQueue.*/
        public void RemoveAllOperations()
        {
            throw new NotImplementedException();
        }

        /** Run the operations. */
        public void RunOperations(Object callingObject = null)
        {
            throw new NotImplementedException();
        }

        /** Call release on all operations. */
        public void ReleaseAllOperations()
        {
            throw new NotImplementedException();
        }

        /** Release operations block that is used to block threads that are waiting on an empty operations queue.*/
        public void ReleaseOperationsBlock()
        {
            throw new NotImplementedException();
        }

        //typedef std::set<OperationThread*> OperationThreads;

        /** Get the set of OperationThreads that are sharing this OperationQueue. */
        public ISet<OperationThread> getOperationThreads() { return _operationThreads; }

        //protected     virtual ~OperationQueue();

        //friend class OperationThread;

        protected void AddOperationThread(OperationThread thread)
        {
            throw new NotImplementedException();
        }
        protected void RemoveOperationThread(OperationThread thread)
        {
            throw new NotImplementedException();
        }

        //typedef std::list< osg::ref_ptr<Operation> > Operations;

        protected object _operationsMutex; //OpenThreads::Mutex
#if TODO
        protected osg::ref_ptr<osg::RefBlock> _operationsBlock;
        protected List<Operation> _operations;
        protected Operations.iterator _currentOperationIterator;
#endif
        protected ISet<OperationThread> _operationThreads = new System.Collections.Generic.HashSet<OperationThread>();

    }

    /** OperationThread is a helper class for running Operation within a single thread.*/
    public class OperationThread //: Thread
    {
        public OperationThread()
        {
#if TODO
            _parent = null;
            _done = false;
            setOperationQueue(new OperationQueue());
#endif
        }

#if TODO
        public void SetParent(Object parent) { _parent = parent; }

        public Object GetParent() { return _parent.get(); }


        /** Set the OperationQueue. */
        public void SetOperationQueue(OperationQueue opq);

        /** Get the OperationQueue. */
        public OperationQueue GetOperationQueue() { return _operationQueue.get(); }
#endif

        /** Add operation to end of OperationQueue, this will be
          * executed by the graphics thread once this operation gets to the head of the queue.*/
        public void Add(Operation operation)
        {
            throw new NotImplementedException();
        }

        /** Remove operation from OperationQueue.*/
        public void Remove(Operation operation)
        {
            throw new NotImplementedException();
        }

        /** Remove named operation from OperationQueue.*/
        public void Remove(string name)
        {
            throw new NotImplementedException();
        }

        /** Remove all operations from OperationQueue.*/
        public void RemoveAllOperations()
        {
            throw new NotImplementedException();
        }


        /** Get the operation currently being run.*/
        public Operation GetCurrentOperation() { return _currentOperation; }

        /** Run does the opertion thread run loop.*/
        public virtual void Run()
        {
            throw new NotImplementedException();
        }

        public void SetDone(bool done)
        {
            throw new NotImplementedException();
        }

        public bool GetDone() { return _done; }

        /** Cancel this graphics thread.*/
        public virtual int Cancel()
        {
            throw new NotImplementedException();
        }

#if TODO
        protected observer_ptr<Object> _parent;
#endif
        protected bool _done;

        protected object _threadMutex; //OpenThreads::Mutex 
        protected OperationQueue _operationQueue;
        protected Operation _currentOperation;
    }

    public class BlockOperation : Operation //, public Block
    {
        public BlockOperation()
            : base("Block", false)
        {
            block.Reset();
        }

        public override void Release()
        {
            block.Release();
        }

        public override void Do(Object p)
        {
            GL.Flush();
            block.Release();
        }

        Block block = new Block();
    }
}
