using Sxta.Core;
using Sxta.Math;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using Matrix4d = Sxta.Math.Matrix4d;
using Vector3d = Sxta.Math.Vector3d;

namespace Sxta.Render.Scenegraph
{
    /// <summary>
    /// 
    /// A scene graph node. A scene graph is a tree of generic scene nodes, where
    /// each node can be seen as an object with a state (fields) and a behavior
    /// (methods). The state is made of a reference frame (relatively to the parent
    /// node), some meshes, modules and uniforms (that can reference textures), and
    /// any other user defined values. The behavior is made of methods, completely
    /// defined by the user by combining basic tasks (draw a mesh, set a projection
    /// matrix, etc) with control structures (sequences, loops, etc).
    /// </summary>
    public class SceneNode : ISwappable<SceneNode>
    {

        /*
         * An iterator to iterate over a set of flags.
         */
        // typedef SetIterator<string> FlagIterator;

        /*
         * An iterator to iterate over a map of Value.
         */
        // typedef MapIterator<string, ptr<Value> > ValueIterator;

        /*
         * An iterator to iterate over a map of Module.
         */
        // typedef MapIterator<string, ptr<Module> > ModuleIterator;

        /*
         * An iterator to iterate over a map of Mesh.
         */
        // typedef MapIterator<string, ptr<MeshBuffers> > MeshIterator;

        /*
         * An iterator to iterate over a map of SceneNode fields.
         */
        // typedef MapIterator<string, ptr<Object> > FieldIterator;

        /*
         * An iterator to iterate over a map of SceneNode Method.
         */
        //typedef MapIterator<string, ptr<Method> > MethodIterator;

        /*
         * True if this scene node is visible, false otherwise.
         */
        public bool isVisible;

        /*
         * Creates an empty SceneNode.
         */
        public SceneNode()
        {
            owner = null;
            localToParent = Matrix4d.Identity;
            localToWorld = Matrix4d.Identity;
            worldToLocalUpToDate = false;
            localBounds = new Box3d(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            localToScreen = Matrix4d.Identity;
        }

        /*
         * Deletes this SceneNode.
         */
        ~SceneNode()
        {
            //MethodIterator i = getMethods();
            foreach (var entry in getMethods())
            {
                entry.Value.owner = null;
            }
        }

        /*
         * Returns the SceneManager that manages the scene graph to which this node
         * belongs.
         */
        public SceneManager getOwner()
        {
            return owner;
        }

        /*
         * Returns the transformation from this node to its parent node.
         */
        public Matrix4d getLocalToParent()
        {
            return localToParent;
        }

        /*
         * Sets the transformation from this node to its parent node.
         *
         * @param t the new localToParent transformation.
         */
        public void setLocalToParent(Matrix4d t)
        {
            localToParent = t;
        }

        /*
         * Returns the transformation from this node to the root node.
         */
        public Matrix4d getLocalToWorld()
        {
            return localToWorld;
        }

        /*
         * Returns the transformation from the root node to this node.
         */
        public Matrix4d getWorldToLocal()
        {
            if (!worldToLocalUpToDate)
            {
                worldToLocal = localToWorld;
                worldToLocal.Invert();
                worldToLocalUpToDate = true;
            }
            return worldToLocal;
        }

        /*
         * Returns the transformation from this node to the camera node.
         */
        public Matrix4d getLocalToCamera()
        {
            return localToCamera;
        }

        /*
         * Returns the tranformation from this node to the screen. This is the
         * transformation from this node to the camera node, followed by the
         * transformation from the camera space to the screen space (defined by the
         * cameraToScreen mat4 uniform of the camera node).
         */
        public Matrix4d getLocalToScreen()
        {
            return localToScreen;
        }

        /*
         * Returns the bounding box of this node in local coordinates.
         */
        public Box3d getLocalBounds()
        {
            return localBounds;
        }

        /*
         * Sets the bounding box of this node in local coordinates.
         */
        public void setLocalBounds(Box3d bounds)
        {
            localBounds = bounds;
        }

        /*
         * Returns the bounding box of this node in world coordinates.
         */
        public Box3d getWorldBounds()
        {
            return worldBounds;
        }

        /*
         * Returns the origin of the local reference frame in world coordinates.
         */
        public Vector3d getWorldPos()
        {
            return worldPos;
        }

        /*
         * Returns the flags of this node.
         */
        public ISet<string> getFlags()
        {
            return flags;
        }

        /*
         * Returns true is this node has the given flag.
         *
         * @param flag a flag.
         */
        public bool hasFlag(string flag)
        {
            return flags.Contains(flag);
        }

        /*
         * Adds the given flag to the flags of this node.
         *
         * @param flag the flag to be added to this node.
         */
        public void addFlag(string flag)
        {
            flags.Add(flag);
            if (owner != null)
            {
                owner.clearNodeMap();
            }
        }

        /*
         * Removes the given flag from the flags of this node.
         *
         * @param flag the flag to be removed from this node.
         */
        public void removeFlag(string flag)
        {
            flags.Remove(flag);
            if (owner != null)
            {
                owner.clearNodeMap();
            }
        }

        /*
         * Returns the values of this node.
         */
        public IDictionary<string, Value> getValues()
        {
            return values;
        }

        /*
         * Returns the valueC of this node whose local name is given.
         *
         * @param name the local name of a valueC.
         */
        public Value getValue(string name)
        {
            return values[name];
        }

        /*
         * Adds a valueC to this node under the given local name.
         *
         * @param valueC a valueC.
         */
        public void addValue(Value value)
        {
            values.Add(value.getName(), value);
        }

        /*
         * Removes the valueC whose local name is given from this node.
         *
         * @param name the local name of the valueC.
         */
        public void removeValue(string name)
        {
            values.Remove(name);
        }

        /*
         * Returns the modules of this node.
         */
        public IDictionary<string, Module> getModules()
        {
            return modules;
        }

        /*
         * Returns the module of this node whose local name is given.
         *
         * @param name the local name of a module.
         */
        public Module getModule(string name)
        {
            return modules[name];
        }

        /*
         * Adds a module to this node under the given local name.
         *
         * @param name a local name.
         * @param s a Module.
         */
        public void addModule(string name, Module s)
        {
            modules[name] = s;
        }

        /*
         * Removes the module whose local name is given from this node.
         *
         * @param name the local name of the module.
         */
        public void removeModule(string name)
        {
            modules.Remove(name);
        }

        /*
         * Returns the meshes of this node.
         */
        public IDictionary<string, MeshBuffers> getMeshes()
        {
            return meshes;
        }

        /*
         * Returns the mesh of this node whose local name is given.
         *
         * @param name the local name of a mesh.
         */
        public MeshBuffers getMesh(string name)
        {
            return meshes[name];
        }

        /*
         * Adds a mesh to this node under the given local name.
         *
         * @param name a local name.
         * @param m a MeshBuffers.
         */
        public void addMesh(string name, MeshBuffers m)
        {
            meshes[name] = m;
            localBounds = localBounds.enlarge(m.bounds);
        }

        /*
         * Removes the mesh whose local name is given from this node.
         *
         * @param name the local name of the mesh.
         */
        public void removeMesh(string name)
        {
            meshes.Remove(name);
        }

        /*
         * Returns the fields of this node.
         */
        public IDictionary<string, object> getFields()
        {
            return fields;
        }

        /*
         * Returns the field of this node whose name is given.
         *
         * @param name the name of a field.
         */
        public Object getField(string name)
        {
            return fields[name];
        }

        /*
         * Adds a field to this node under the given name.
         *
         * @param name the field name.
         * @param f the field valueC.
         */
        public void addField(string name, Object f)
        {
            removeField(name);
            fields[name] = f;
        }

        /*
         * Removes the field whose name is given from this node.
         *
         * @param name the name of the field.
         */
        public void removeField(string name)
        {
            fields.Remove(name);
        }

        /*
         * Returns the methods of this node.
         */
        public IDictionary<string, Method> getMethods()
        {
            return methods;
        }

        /*
         * Returns the method of this node whose name is given.
         *
         * @param name the name of a method.
         */
        public Method getMethod(string name)
        {
            return methods[name];
        }

        /*
         * Adds a method to this node under the given name.
         *
         * @param name the method name.
         * @param m the method.
         */
        public void addMethod(string name, Method m)
        {
            removeMethod(name);
            methods[name] = m;
            m.owner = this;
        }

        /*
         * Removes the method whose name is given from this node.
         *
         * @param name the name of the method.
         */
        public void removeMethod(string name)
        {
            Method m;
            if (methods.TryGetValue(name, out m))
            {
                methods.Remove(name);
                m.owner = null;
            }
        }

        /*
         * Returns the number of child node of this node.
         */
        public int getChildrenCount()
        {
            return children.Count;
        }

        /*
         * Returns the child node of this node whose index is given.
         *
         * @param index a child node index between 0 and #getChildrenCount - 1.
         */
        public SceneNode getChild(int index)
        {
            return children[index];
        }

        /*
         * Adds a child node to this node.
         *
         * @param child a child node.
         */
        public void addChild(SceneNode child)
        {
            if (child.owner == null)
            {
                children.Add(child);
                child.setOwner(owner);
                if (owner != null)
                {
                    owner.clearNodeMap();
                }
            }
        }

        /*
         * Removes a child node from this node.
         *
         * @param index a child node index between 0 and #getChildrenCount - 1.
         */
        public void removeChild(int index)
        {
            children.RemoveAt(index);
        }


        /*
         * Swaps this scene node with the given one.
         */
        protected void swap(SceneNode n)
        {
            Std.Swap(ref localToParent, ref  n.localToParent);
            Std.Swap(ref flags, ref n.flags);
            Std.Swap(ref values, ref n.values);
            Std.Swap(ref modules, ref  n.modules);
            Std.Swap(ref meshes, ref n.meshes);
            Std.Swap(ref methods, ref n.methods);
            Std.Swap(ref children, ref n.children);
            foreach (Method i in methods.Values)
            {
                i.owner = this;
            }
            foreach (Method i in n.methods.Values)
            {
                i.owner = n;
            }
            if (owner != null)
            {
                owner.clearNodeMap();
            }
            setOwner(owner);
            n.setOwner(null);
        }


        /*
         * The SceneManager that manages the scene graph to which this node belongs.
         */
        internal SceneManager owner;

        /*
         * The transformation from this node to its parent node.
         */
        private Matrix4d localToParent;

        /*
         * The transformation from this node to the root node.
         */
        private Matrix4d localToWorld;

        /*
         * The transformation from the root node to this node.
         */
        private Matrix4d worldToLocal;

        /*
         * The transformation from this node to the camera node.
         */
        private Matrix4d localToCamera;

        /*
         * The transformation from this node to the screen.
         */
        private Matrix4d localToScreen;

        /*
         * The bounding box of this node in local coordinates.
         */
        private Box3d localBounds;

        /*
         * The bounding box of this node in world coordinates.
         */
        private Box3d worldBounds;

        /*
         * The origin of the local reference frame of this node in world coordinates.
         */
        private Vector3d worldPos;

        /*
         * True if the #worldToLocal transform is up to date.
         */
        private bool worldToLocalUpToDate;

        /*
         * The flags of this node.
         */
        private ISet<string> flags = new HashSet<string>();

        /*
         * The values of this node.
         */
        private IDictionary<string, Value> values = new Dictionary<string, Value>();

        /*
         * The modules of this node.
         */
        private IDictionary<string, Module> modules = new Dictionary<string, Module>();

        /*
         * The meshes of this node.
         */
        private IDictionary<string, MeshBuffers> meshes = new Dictionary<string, MeshBuffers>();

        /*
         * The fields of this node.
         */
        private IDictionary<string, Object> fields = new Dictionary<string, Object>();

        /*
         * The methods of this node.
         */
        private IDictionary<string, Method> methods = new Dictionary<string, Method>();

        /*
         * The child nodes of this node.
         */
        private IList<SceneNode> children = new List<SceneNode>();

        /*
         * Sets the SceneManager that manages the scene graph to which this node
         * belongs.
         *
         * @param owner a SceneManager.
         */
        internal void setOwner(SceneManager owner)
        {
            this.owner = owner;
            foreach (SceneNode i in children)
            {
                i.setOwner(owner);
            }
        }

        /*
         * Updates the #localToWorld transform. This method also updates #worldBounds
         * and #worldPos.
         *
         * @param parent the parent node of this node.
         */
        internal void updateLocalToWorld(SceneNode parent)
        {
            if (parent != null)
            {
                localToWorld = localToParent * parent.localToWorld ;//AQUI MULT MATRIX
            }

            foreach (SceneNode i in children)
            {
                i.updateLocalToWorld(this);
            }
            Matrix4d localToWorld0 = localToWorld;

            worldBounds = localToWorld0 * localBounds;
            worldPos = localToWorld0 * Vector3d.Zero;
            foreach (SceneNode i in children)
            {
                worldBounds = worldBounds.enlarge(i.worldBounds);
            }
            worldToLocalUpToDate = false;
        }

        /*
         * Updates the #localToCamera and the #localToScreen transforms.
         *
         * @param worldToCamera the world to camera transform.
         * @param cameraToScreen the camera to screen transform.
         */
        internal void updateLocalToCamera(Matrix4d worldToCamera, Matrix4d cameraToScreen)
        {
            localToCamera = localToWorld * worldToCamera;//AQUI MULT MATRIX
            localToScreen = localToCamera * cameraToScreen;//AQUI MULT MATRIX

            foreach (SceneNode i in children)
            {
                i.updateLocalToCamera(worldToCamera, cameraToScreen);
            }
        }

        void ISwappable<SceneNode>.swap(SceneNode obj)
        {
            throw new NotImplementedException();
        }
    }
}
