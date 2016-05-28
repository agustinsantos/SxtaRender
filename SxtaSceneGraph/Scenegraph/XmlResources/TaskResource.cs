using Sxta.Math;
using Sxta.Render.OpenGLExt;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.Render.Scenegraph.XmlResources
{

    public class CallMethodTaskResource : ResourceTemplate<CallMethodTask>
    {
        public static CallMethodTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new CallMethodTaskResource(manager, name, desc, e);
        }
        public CallMethodTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,");
            this.valueC = new CallMethodTask();
            string n = getParameter(desc, e, "name");
            this.valueC.init(new AbstractTask.QualifiedName(n));
        }
    }

    public class DrawMeshTaskResource : ResourceTemplate<DrawMeshTask>
    {
        public static DrawMeshTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new DrawMeshTaskResource(manager, name, desc, e);
        }
        public DrawMeshTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            this.valueC = new DrawMeshTask();
            checkParameters(desc, e, "name,count,");
            string n = getParameter(desc, e, "name");
            int count = 1;
            string s = e.GetAttribute("count");
            if (!string.IsNullOrWhiteSpace(s))
            {
                getIntParameter(desc, e, "count", out count);
            }
            this.valueC.init(new AbstractTask.QualifiedName(n), count);
        }
    }

    public class LoopTaskResource : ResourceTemplate<LoopTask>
    {
        public static LoopTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new LoopTaskResource(manager, name, desc, e);
        }
        public LoopTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            this.valueC = new LoopTask();
            checkParameters(desc, e, "var,flag,culling,parallel,");

            string var = getParameter(desc, e, "var");
            string flag = getParameter(desc, e, "flag");
            bool cull = false;
            bool parallel = false;
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("culling")) && (e.GetAttribute("culling") == "true"))
            {
                cull = true;
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("parallel")) && (e.GetAttribute("parallel") == "true"))
            {
                parallel = true;
            }
            List<TaskFactory> subtasks = new List<TaskFactory>();
            foreach (XmlNode n in e.ChildNodes)
            {
                XmlElement f = n as XmlElement;
                if (f != null)
                {
                    TaskFactory tf;
                    tf = ResourceFactory.getInstance().create(manager, "", desc, f).get() as TaskFactory;
                    subtasks.Add(tf);
                }
            }
            if (subtasks.Count == 1)
            {
                this.valueC.init(var, flag, cull, parallel, subtasks[0]);
            }
            else
            {
                this.valueC.init(var, flag, cull, parallel, new SequenceTask(subtasks));
            }
        }
    }

    public class SequenceTaskResource : ResourceTemplate<SequenceTask>
    {
        public static SequenceTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new SequenceTaskResource(manager, name, desc, e);
        }
        public SequenceTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            this.valueC = new SequenceTask();
            checkParameters(desc, e, "name,");
            checkParameters(desc, e, "name,");
            List<TaskFactory> subtasks = new List<TaskFactory>();
            foreach (XmlNode n in e.ChildNodes)
            {
                XmlElement f = n as XmlElement;
                if (f != null)
                {
                    TaskFactory tf;
                    tf = ResourceFactory.getInstance().create(manager, "", desc, f).get() as TaskFactory;
                    subtasks.Add(tf);
                }
            }
            this.valueC.init(subtasks);
        }
    }

    public class SetProgramTaskResource : ResourceTemplate<SetProgramTask>
    {
        public static SetProgramTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new SetProgramTaskResource(manager, name, desc, e);
        }
        public SetProgramTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            this.valueC = new SetProgramTask();

            checkParameters(desc, e, "setUniforms,");
            List<AbstractTask.QualifiedName> modules = new List<AbstractTask.QualifiedName>();
            bool setUniforms = false;
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("setUniforms")))
            {
                setUniforms = e.GetAttribute("setUniforms") == "true";
            }
            foreach (XmlNode n in e.ChildNodes)
            {
                XmlElement f = n as XmlElement;
                if (f != null)
                {
                    if (f.Name != "module")
                    {
                        throw new Exception("Invalid subelement '" + f.Name + "'");
                    }
                    checkParameters(desc, f, "name,");
                    string moduleName = f.GetAttribute("name");
                    if (string.IsNullOrWhiteSpace(moduleName))
                    {
                        throw new Exception("Missing 'name' attribute");
                    }
                    modules.Add(new AbstractTask.QualifiedName(moduleName));
                }
            }
            this.valueC.init(modules, setUniforms);
        }
    }

    public class SetStateTaskResource : ResourceTemplate<SetStateTask>
    {
        public static SetStateTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new SetStateTaskResource(manager, name, desc, e);
        }

        public static BufferId getBufferFromName(string v)
        {
            switch (v)
            {
                case "NONE":
                    return (BufferId)(0);
                case "COLOR0":
                    return BufferId.COLOR0;
                case "COLOR1":
                    return BufferId.COLOR1;
                case "COLOR2":
                    return BufferId.COLOR2;
                case "COLOR3":
                    return BufferId.COLOR3;
                case "COLOR4":
                    return BufferId.COLOR4;
                case "COLOR5":
                    return BufferId.COLOR5;
                case "COLOR6":
                    return BufferId.COLOR6;
                case "COLOR7":
                    return BufferId.COLOR7;
                case "DEPTH":
                    return BufferId.DEPTH;
                default:
                    throw new ArgumentException("Invalid BufferId Name");
            }
        }


        BlendEquation getBlendEquation(ResourceDescriptor desc, XmlElement e, string name)
        {
            if (e.GetAttribute(name) == "ADD")
            {
                return BlendEquation.ADD;
            }
            if (e.GetAttribute(name) == "SUBTRACT")
            {
                return BlendEquation.SUBTRACT;
            }
            if (e.GetAttribute(name) == "REVERSE_SUBTRACT")
            {
                return BlendEquation.REVERSE_SUBTRACT;
            }
            if (e.GetAttribute(name) == "MIN")
            {
                return BlendEquation.MIN;
            }
            if (e.GetAttribute(name) == "MAX")
            {
                return BlendEquation.MAX;
            }
            throw new Exception("Invalid blend equation: " + e.GetAttribute(name));
        }

        BlendArgument getBlendArgument(ResourceDescriptor desc, XmlElement e, string name)
        {
            if (e.GetAttribute(name) == "ZERO")
            {
                return BlendArgument.ZERO;
            }
            if (e.GetAttribute(name) == "ONE")
            {
                return BlendArgument.ONE;
            }
            if (e.GetAttribute(name) == "SRC_COLOR")
            {
                return BlendArgument.SRC_COLOR;
            }
            if (e.GetAttribute(name) == "ONE_MINUS_SRC_COLOR")
            {
                return BlendArgument.ONE_MINUS_SRC_COLOR;
            }
            if (e.GetAttribute(name) == "DST_COLOR")
            {
                return BlendArgument.DST_COLOR;
            }
            if (e.GetAttribute(name) == "ONE_MINUS_DST_COLOR")
            {
                return BlendArgument.ONE_MINUS_DST_COLOR;
            }
            if (e.GetAttribute(name) == "SRC_ALPHA")
            {
                return BlendArgument.SRC_ALPHA;
            }
            if (e.GetAttribute(name) == "ONE_MINUS_SRC_ALPHA")
            {
                return BlendArgument.ONE_MINUS_SRC_ALPHA;
            }
            if (e.GetAttribute(name) == "DST_ALPHA")
            {
                return BlendArgument.DST_ALPHA;
            }
            if (e.GetAttribute(name) == "ONE_MINUS_DST_ALPHA")
            {
                return BlendArgument.ONE_MINUS_DST_ALPHA;
            }
            if (e.GetAttribute(name) == "CONSTANT_COLOR")
            {
                return BlendArgument.CONSTANT_COLOR;
            }
            if (e.GetAttribute(name) == "ONE_MINUS_CONSTANT_COLOR")
            {
                return BlendArgument.ONE_MINUS_CONSTANT_COLOR;
            }
            if (e.GetAttribute(name) == "CONSTANT_ALPHA")
            {
                return BlendArgument.CONSTANT_ALPHA;
            }
            if (e.GetAttribute(name) == "ONE_MINUS_CONSTANT_ALPHA")
            {
                return BlendArgument.ONE_MINUS_CONSTANT_ALPHA;
            }
            throw new Exception("Invalid blend argument: " + e.GetAttribute(name));
        }

        Function getFunction(ResourceDescriptor desc, XmlElement e, string name)
        {
            if (e.GetAttribute(name) == "NEVER")
            {
                return Function.NEVER;
            }
            if (e.GetAttribute(name) == "ALWAYS")
            {
                return Function.ALWAYS;
            }
            if (e.GetAttribute(name) == "LESS")
            {
                return Function.LESS;
            }
            if (e.GetAttribute(name) == "LEQUAL")
            {
                return Function.LEQUAL;
            }
            if (e.GetAttribute(name) == "EQUAL")
            {
                return Function.EQUAL;
            }
            if (e.GetAttribute(name) == "GREATER")
            {
                return Function.GREATER;
            }
            if (e.GetAttribute(name) == "GEQUAL")
            {
                return Function.GEQUAL;
            }
            if (e.GetAttribute(name) == "NOTEQUAL")
            {
                return Function.NOTEQUAL;
            }
            throw new Exception("Invalid function:" + e.GetAttribute(name));
        }

        StencilOperation getStencilOperation(ResourceDescriptor desc, XmlElement e, string name)
        {
            if (e.GetAttribute(name) == "KEEP")
            {
                return StencilOperation.KEEP;
            }
            if (e.GetAttribute(name) == "RESET")
            {
                return StencilOperation.RESET;
            }
            if (e.GetAttribute(name) == "REPLACE")
            {
                return StencilOperation.REPLACE;
            }
            if (e.GetAttribute(name) == "INCR")
            {
                return StencilOperation.INCR;
            }
            if (e.GetAttribute(name) == "DECR")
            {
                return StencilOperation.DECR;
            }
            if (e.GetAttribute(name) == "INVERT")
            {
                return StencilOperation.INVERT;
            }
            if (e.GetAttribute(name) == "INCR_WRAP")
            {
                return StencilOperation.INCR_WRAP;
            }
            if (e.GetAttribute(name) == "DECR_WRAP")
            {
                return StencilOperation.DECR_WRAP;
            }
            throw new Exception("Invalid Stencil Operation:" + e.GetAttribute(name));
        }

        LogicOperation getLogicOperation(ResourceDescriptor desc, XmlElement e, string name)
        {
            if (e.GetAttribute(name) == "CLEAR")
            {
                return LogicOperation.CLEAR;
            }
            if (e.GetAttribute(name) == "AND")
            {
                return LogicOperation.AND;
            }
            if (e.GetAttribute(name) == "AND_REVERSE")
            {
                return LogicOperation.AND_REVERSE;
            }
            if (e.GetAttribute(name) == "COPY")
            {
                return LogicOperation.COPY;
            }
            if (e.GetAttribute(name) == "AND_INVERTED")
            {
                return LogicOperation.AND_INVERTED;
            }
            if (e.GetAttribute(name) == "NOOP")
            {
                return LogicOperation.NOOP;
            }
            if (e.GetAttribute(name) == "XOR")
            {
                return LogicOperation.XOR;
            }
            if (e.GetAttribute(name) == "OR")
            {
                return LogicOperation.OR;
            }
            if (e.GetAttribute(name) == "NOR")
            {
                return LogicOperation.NOR;
            }
            if (e.GetAttribute(name) == "EQUIV")
            {
                return LogicOperation.EQUIV;
            }
            if (e.GetAttribute(name) == "NOT")
            {
                return LogicOperation.NOT;
            }
            if (e.GetAttribute(name) == "OR_REVERSE")
            {
                return LogicOperation.OR_REVERSE;
            }
            if (e.GetAttribute(name) == "COPY_INVERTED")
            {
                return LogicOperation.COPY_INVERTED;
            }
            if (e.GetAttribute(name) == "OR_INVERTED")
            {
                return LogicOperation.OR_INVERTED;
            }
            if (e.GetAttribute(name) == "NAND")
            {
                return LogicOperation.NAND;
            }
            if (e.GetAttribute(name) == "SET")
            {
                return LogicOperation.SET;
            }
            throw new Exception("Invalid Logic Operation:" + e.GetAttribute(name));
        }

        QueryType getQueryType(ResourceDescriptor desc, XmlElement e, string name)
        {
            if (e.GetAttribute(name) == "PRIMITIVES_GENERATED")
            {
                return QueryType.PRIMITIVES_GENERATED;
            }
            if (e.GetAttribute(name) == "TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN")
            {
                return QueryType.TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN;
            }
            if (e.GetAttribute(name) == "SAMPLES_PASSED")
            {
                return QueryType.SAMPLES_PASSED;
            }
            if (e.GetAttribute(name) == "ANY_SAMPLES_PASSED")
            {
                return QueryType.ANY_SAMPLES_PASSED;
            }
            if (e.GetAttribute(name) == "TIME_STAMP")
            {
                return QueryType.TIME_STAMP;
            }
            throw new Exception("Invalid Query Type: " + e.GetAttribute(name));
        }

        QueryMode getQueryMode(ResourceDescriptor desc, XmlElement e, string name)
        {
            if (e.GetAttribute(name) == "WAIT")
            {
                return QueryMode.WAIT;
            }
            if (e.GetAttribute(name) == "NO_WAIT")
            {
                return QueryMode.NO_WAIT;
            }
            if (e.GetAttribute(name) == "REGION_WAIT")
            {
                return QueryMode.REGION_WAIT;
            }
            if (e.GetAttribute(name) == "REGION_NO_WAIT")
            {
                return QueryMode.REGION_NO_WAIT;
            }

            throw new Exception("Invalid Query Mode:" + e.GetAttribute(name));
        }

        public SetStateTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "readBuffer,drawBuffer,clearColor,clearStencil,clearDepth,");

            BufferId rb = (BufferId)(-1);
            BufferId db = (BufferId)(-1);
            bool clearColor = !string.IsNullOrWhiteSpace(e.GetAttribute("clearColor")) && e.GetAttribute("clearColor") == "true";
            bool clearStencil = !string.IsNullOrWhiteSpace(e.GetAttribute("clearStencil")) && e.GetAttribute("clearStencil") == "true";
            bool clearDepth = !string.IsNullOrWhiteSpace(e.GetAttribute("clearDepth")) && e.GetAttribute("clearDepth") == "true";
            this.valueC = new SetStateTask();
            try
            {
                if (!string.IsNullOrWhiteSpace(e.GetAttribute("readBuffer")))
                {
                    rb = getBufferFromName(e.GetAttribute("readBuffer"));
                }
                if (!string.IsNullOrWhiteSpace(e.GetAttribute("drawBuffer")))
                {
                    db = (BufferId)(0);
                    string names = getParameter(desc, e, "drawBuffer");
                    foreach (string n in names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        db = (BufferId)(db | getBufferFromName(n));
                    }
                }
            }
            catch
            {
                throw new Exception("Invalid buffer name");
            }

            this.valueC.addRunnable(new SetStateTask.SetBuffers(rb, db));

            foreach (XmlNode n in e.ChildNodes)
            {
                XmlElement f = n as XmlElement;
                if (f == null)
                {
                    continue;
                }
                if (f.Name == "viewport")
                {
                    checkParameters(desc, f, "x,y,width,height,");
                    Vector4i vp = Vector4i.Zero;
                    getIntParameter(desc, f, "x", out vp.X);
                    getIntParameter(desc, f, "y", out vp.Y);
                    getIntParameter(desc, f, "width", out vp.Z);
                    getIntParameter(desc, f, "height", out vp.W);
                    this.valueC.addRunnable(new SetStateTask.SetViewport(vp));
                }
                else if (f.Name == "depthRange")
                {
                    checkParameters(desc, f, "near,far,");
                    float near;
                    float far;
                    getFloatParameter(desc, f, "near", out near);
                    getFloatParameter(desc, f, "far", out far);
                    this.valueC.addRunnable(new SetStateTask.SetDepthRange(near, far));
                }
                else if (f.Name == "clipDistances")
                {
                    checkParameters(desc, f, "value,");
                    int val;
                    getIntParameter(desc, f, "value", out val);
                    this.valueC.addRunnable(new SetStateTask.SetClipDistances(val));
                }
                else if (f.Name == "blend")
                {
                    if (f.Attributes[0] == null)
                    {
                        this.valueC.addRunnable(new SetStateTask.SetBlend((BufferId)(-1), false));
                    }
                    else
                    {
                        checkParameters(desc, f, "buffer,enable,r,g,b,a,eq,alphaeq,src,dst,alphasrc,alphadst,");
                        BufferId b = (BufferId)(-1);
                        bool enable = !string.IsNullOrWhiteSpace(f.GetAttribute("enable")) && f.GetAttribute("enable") == "true";
                        BlendEquation rgbEq = (BlendEquation)(-1);
                        BlendEquation alphaEq = (BlendEquation)(-1);
                        BlendArgument srgbf = (BlendArgument)(-1);
                        BlendArgument drgbf = (BlendArgument)(-1);
                        BlendArgument salphaf = (BlendArgument)(-1);
                        BlendArgument dalphaf = (BlendArgument)(-1);

                        if (!string.IsNullOrWhiteSpace(f.GetAttribute("buffer")))
                        {
                            b = BufferId.DEFAULT;
                            string names = getParameter(desc, f, "buffer");
                            foreach (string n1 in names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                b = (BufferId)(b | getBufferFromName(n1));
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(f.GetAttribute("eq")))
                        {
                            rgbEq = getBlendEquation(desc, f, "eq");
                            srgbf = getBlendArgument(desc, f, "src");
                            drgbf = getBlendArgument(desc, f, "dst");
                            if (f.GetAttribute("alphaeq") != null)
                            {
                                alphaEq = getBlendEquation(desc, f, "alphaeq");
                                salphaf = getBlendArgument(desc, f, "alphasrc");
                                dalphaf = getBlendArgument(desc, f, "alphadst");
                            }
                        }
                        this.valueC.addRunnable(new SetStateTask.SetBlend(b, enable, rgbEq, srgbf, drgbf, alphaEq, salphaf, dalphaf));

                        if (!string.IsNullOrWhiteSpace(f.GetAttribute("r")))
                        {
                            Vector4f color = new Vector4f();
                            getFloatParameter(desc, f, "r", out color.X);
                            getFloatParameter(desc, f, "g", out color.Y);
                            getFloatParameter(desc, f, "b", out color.Z);
                            getFloatParameter(desc, f, "a", out color.W);

                            this.valueC.addRunnable(new SetStateTask.SetBlendColor(color));
                        }
                    }
                }
                else if (f.Name == "clear")
                {
                    Vector4f c = new Vector4f(0.0f, 0.0f, 0.0f, 0.0f);
                    int s = 0;
                    float d = 1.0f;
                    checkParameters(desc, f, "r,g,b,a,stencil,depth,");
                    if (f.GetAttribute("r") != null)
                    {
                        getFloatParameter(desc, f, "r", out c.X);
                        getFloatParameter(desc, f, "g", out c.Y);
                        getFloatParameter(desc, f, "b", out c.Z);
                        getFloatParameter(desc, f, "a", out c.W);
                        this.valueC.addRunnable(new SetStateTask.SetClearColor(c));
                    }

                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("stencil")))
                    {
                        getIntParameter(desc, f, "stencil", out s);
                        this.valueC.addRunnable(new SetStateTask.SetClearStencil(s));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("depth")))
                    {
                        getFloatParameter(desc, f, "depth", out d);
                        this.valueC.addRunnable(new SetStateTask.SetClearDepth(d));
                    }
                }
                else if (f.Name == "point")
                {
                    checkParameters(desc, f, "size,threshold,lowerleftorigin,");
                    float s;

                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("size")))
                    {
                        getFloatParameter(desc, f, "size", out s);
                        this.valueC.addRunnable(new SetStateTask.SetPointSize(s));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("threshold")))
                    {
                        getFloatParameter(desc, f, "threshold", out s);
                        this.valueC.addRunnable(new SetStateTask.SetPointFadeThresholdSize(s));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("lowerleftorigin")))
                    {
                        bool llo = f.GetAttribute("clearColor") == "true";
                        this.valueC.addRunnable(new SetStateTask.SetPointLowerLeftOrigin(llo));
                    }

                }
                else if (f.Name == "line")
                {
                    checkParameters(desc, f, "width,smooth,");
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("smooth")))
                    {
                        bool smooth = f.GetAttribute("smooth") == "true";
                        this.valueC.addRunnable(new SetStateTask.SetLineSmooth(smooth));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("width")))
                    {
                        float width;
                        getFloatParameter(desc, f, "width", out width);
                        this.valueC.addRunnable(new SetStateTask.SetLineWidth(width));
                    }
                }
                else if (f.Name == "polygon")
                {
                    string[] v = new string[2];
                    bool frontCW;
                    bool smooth;
                    bool pointOffset;
                    bool lineOffset;
                    bool polygonOffset;
                    float factor;
                    float units;
                    PolygonMode[] cull = new PolygonMode[2];
                    checkParameters(desc, f, "frontCW,front,back,smooth,offsetFactor,offsetUnits,pointOffset,lineOffset,polygonOffset,");

                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("front")))
                    {
                        v[0] = getParameter(desc, f, "front");
                        v[1] = getParameter(desc, f, "back");
                        for (int i = 0; i < 2; ++i)
                        {
                            if (v[i] == "CULL")
                            {
                                cull[i] = PolygonMode.CULL;
                            }
                            else if (v[i] == "LINE")
                            {
                                cull[i] = PolygonMode.LINE;
                            }
                            else if (v[i] == "FILL")
                            {
                                cull[i] = PolygonMode.FILL;
                            }
                            else
                            {
                                throw new Exception("Invalid cull value");
                            }
                        }
                        this.valueC.addRunnable(new SetStateTask.SetPolygonMode(cull[0], cull[1]));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("frontCW")))
                    {
                        frontCW = f.GetAttribute("frontCW") == "true";
                        this.valueC.addRunnable(new SetStateTask.SetFrontFaceCW(frontCW));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("smooth")))
                    {
                        smooth = f.GetAttribute("smooth") == "true";
                        this.valueC.addRunnable(new SetStateTask.SetPolygonSmooth(smooth));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("offsetFactor")))
                    {
                        getFloatParameter(desc, f, "factor", out factor);
                        getFloatParameter(desc, f, "units", out units);

                        this.valueC.addRunnable(new SetStateTask.SetPolygonOffset(factor, units));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("pointOffset")))
                    {
                        pointOffset = f.GetAttribute("pointOffset") == "true";
                        lineOffset = f.GetAttribute("lineOffset") == "true";
                        polygonOffset = f.GetAttribute("polygonOffset") == "true";
                        this.valueC.addRunnable(new SetStateTask.SetPolygonOffsets(pointOffset, lineOffset, polygonOffset));
                    }

                }
                else if (f.Name == "depth")
                {
                    checkParameters(desc, f, "enable,value,");
                    Function depth = (Function)(-1);
                    bool enable;
                    enable = f.GetAttribute("enable") != null && f.GetAttribute("enable") == "true";
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("value")))
                    {
                        depth = getFunction(desc, f, "value");
                    }

                    this.valueC.addRunnable(new SetStateTask.SetDepthTest(enable, depth));
                }
                else if (f.Name == "stencil")
                {
                    checkParameters(desc, f, "enable,ffunction,bfunction,fref,bref,fmask,bmask,ffail,bfail,fdpfail,bdpfail,fdppass,bdppass,");
                    bool enableStencil = f.GetAttribute("enable") != null && f.GetAttribute("enable") == "true";

                    Function ff = (Function)(-1);
                    int fref = -1;
                    int fmask = 0;
                    StencilOperation ffail = (StencilOperation)(-1);
                    StencilOperation fdpfail = (StencilOperation)(-1);
                    StencilOperation fdppass = (StencilOperation)(-1);

                    Function bf = (Function)(-1);
                    int bref = -1;
                    int bmask = 0;
                    StencilOperation bfail = (StencilOperation)(-1);
                    StencilOperation bdpfail = (StencilOperation)(-1);
                    StencilOperation bdppass = (StencilOperation)(-1);
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("ffunction")))
                    {
                        ff = getFunction(desc, f, "ffunction");
                        getIntParameter(desc, f, "fref", out fref);
                        getIntParameter(desc, f, "fmask", out fmask);
                        ffail = getStencilOperation(desc, f, "ffail");
                        ffail = getStencilOperation(desc, f, "fdpfail");
                        ffail = getStencilOperation(desc, f, "fdppass");
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("bfunction")))
                    {
                        bf = getFunction(desc, f, "bfunction");
                        getIntParameter(desc, f, "bref", out bref);
                        getIntParameter(desc, f, "bmask", out bmask);
                        bfail = getStencilOperation(desc, f, "bfail");
                        bfail = getStencilOperation(desc, f, "bdpfail");
                        bfail = getStencilOperation(desc, f, "bdppass");
                    }

                    this.valueC.addRunnable(new SetStateTask.SetStencilTest(enableStencil, ff, fref, (uint)(fmask), ffail, fdpfail, fdppass, bf, bref, (uint)(bmask), bfail, bdpfail, bdppass));
                }
                else if (f.Name == "write")
                {
                    checkParameters(desc, f, "buffer,r,g,b,a,d,fs,bs,");
                    BufferId id = (BufferId)(-1);
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("buffer")))
                    {
                        id = (BufferId)(0);
                        string names = getParameter(desc, f, "buffer");
                        foreach (string n1 in names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            id = (BufferId)(id | getBufferFromName(n1));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("r")))
                    {
                        bool r = f.GetAttribute("r") == "true";
                        bool g = f.GetAttribute("g") == "true";
                        bool b = f.GetAttribute("b") == "true";
                        bool a = f.GetAttribute("a") == "true";
                        this.valueC.addRunnable(new SetStateTask.SetColorMask(id, r, g, b, a));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("d")))
                    {
                        bool d = f.GetAttribute("d") == "true";
                        this.valueC.addRunnable(new SetStateTask.SetDepthMask(d));
                    }
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("fs")))
                    {
                        int fs;
                        int bs;
                        getIntParameter(desc, f, "fs", out fs);
                        getIntParameter(desc, f, "bs", out bs);
                        this.valueC.addRunnable(new SetStateTask.SetStencilMask((uint)(fs), (uint)(bs)));
                    }
                }
                else if (f.Name == "logic")
                {
                    checkParameters(desc, f, "enable,value,");
                    LogicOperation l = (LogicOperation)(-1);
                    bool enable = f.GetAttribute("enable") != null && f.GetAttribute("enable") == "true";
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("value")))
                    {
                        l = getLogicOperation(desc, f, "value");
                    }
                    this.valueC.addRunnable(new SetStateTask.SetLogicOp(enable, l));
                }
                else if (f.Name == "scissor")
                {
                    checkParameters(desc, f, "enable,x,y,width,height,");
                    bool enable = f.GetAttribute("enable") != null && f.GetAttribute("enable") == "true";
                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("x")))
                    {
                        Vector4i vp = new Vector4i();
                        getIntParameter(desc, f, "x", out vp.X);
                        getIntParameter(desc, f, "y", out vp.Y);
                        getIntParameter(desc, f, "width", out vp.Z);
                        getIntParameter(desc, f, "height", out vp.W);
                        this.valueC.addRunnable(new SetStateTask.SetScissorTestValue(enable, vp));
                    }
                    else
                    {
                        this.valueC.addRunnable(new SetStateTask.SetScissorTest(enable));
                    }
                }
                else if (f.Name == "occlusion")
                {
                    checkParameters(desc, f, "query,mode,");
                    Query q;
                    QueryMode m = (QueryMode)(-1);

                    m = getQueryMode(desc, f, "mode");
                    q = new Query(getQueryType(desc, f, "query"));
                    this.valueC.addRunnable(new SetStateTask.SetOcclusionTest(q, m));

                }
                else if (f.Name == "multisampling")
                {
                    checkParameters(desc, f, "enable,alphaToCoverage,alphaToOne,coverage,mask,shading,min,");
                    bool enable = f.GetAttribute("enable") != null && f.GetAttribute("enable") == "true";
                    this.valueC.addRunnable(new SetStateTask.SetMultisample(enable));

                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("alphaToCoverage")))
                    {
                        bool ac = f.GetAttribute("alphaToCoverage") == "true";
                        bool ao = f.GetAttribute("alphaToOne") == "true";
                        this.valueC.addRunnable(new SetStateTask.SetSampleAlpha(ac, ao));
                    }

                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("coverage")))
                    {
                        float c;
                        getFloatParameter(desc, f, "coverage", out c);
                        this.valueC.addRunnable(new SetStateTask.SetSampleCoverage(c));
                    }

                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("mask")))
                    {
                        int m;
                        getIntParameter(desc, f, "mask", out m);
                        this.valueC.addRunnable(new SetStateTask.SetSampleMask((uint)(m)));
                    }

                    if (!string.IsNullOrWhiteSpace(f.GetAttribute("shading")))
                    {
                        float m;
                        enable = f.GetAttribute("shading") == "true";
                        getFloatParameter(desc, f, "min", out m);
                        this.valueC.addRunnable(new SetStateTask.SetSampleShading(enable, m));
                    }
                }
                else
                {
                    throw new Exception("Invalid sub element");
                }
            }

            this.valueC.addRunnable(new SetStateTask.SetClearState(clearColor, clearStencil, clearDepth));
        }
    }

    public class SetTargetTaskResource : ResourceTemplate<SetTargetTask>
    {
        public static SetTargetTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new SetTargetTaskResource(manager, name, desc, e);
        }
        public SetTargetTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            List<SetTargetTask.Target> targets = new List<SetTargetTask.Target>();
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,autoResize,");
            this.valueC = new SetTargetTask();
            bool autoResize = false;
            if (e.GetAttribute("autoResize") != null)
            {
                autoResize = e.GetAttribute("autoResize") == "true";
            }
            foreach (XmlNode n in e.ChildNodes)
            {
                XmlElement f = n as XmlElement;
                if (f != null)
                {
                    SetTargetTask.Target t = new SetTargetTask.Target();
                    if (f.Name == "buffer")
                    {
                        throw new Exception("Invalid subelement");
                    }
                    checkParameters(desc, f, "name,texture,level,layer,");
                    string namestr = getParameter(desc, f, "name");
                    try
                    {
                        t.buffer = SetStateTaskResource.getBufferFromName(namestr);
                    }
                    catch
                    {
                        throw new Exception("Invalid buffer name");
                    }
                    t.texture = new AbstractTask.QualifiedName(getParameter(desc, f, "texture"));
                    t.level = 0;
                    if (f.GetAttribute("level") != null)
                    {
                        getIntParameter(desc, f, "level", out t.level);
                    }
                    t.layer = 0;
                    if (f.GetAttribute("layer") != null)
                    {
                        getIntParameter(desc, f, "layer", out t.layer);
                    }
                    targets.Add(t);
                }
            }
            this.valueC.init(targets, autoResize);
        }
    }

    public class SetTransformsTaskResource : ResourceTemplate<SetTransformsTask>
    {
        public static SetTransformsTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new SetTransformsTaskResource(manager, name, desc, e);
        }
        public SetTransformsTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "screen,time,localToWorld,localToScreen,cameraToWorld,cameraToScreen,screenToCamera,module,worldToScreen,worldPos,worldDir,");
            this.valueC = new SetTransformsTask();
            string s = e.GetAttribute("screen");
            string screen = s == null ? "" : e.GetAttribute("screen");

            s = e.GetAttribute("module");
            AbstractTask.QualifiedName module = new AbstractTask.QualifiedName(s == null ? "" : s);

            string time = e.GetAttribute("time");
            string localToWorld = e.GetAttribute("localToWorld");
            string localToScreen = e.GetAttribute("localToScreen");
            string cameraToWorld = e.GetAttribute("cameraToWorld");
            string cameraToScreen = e.GetAttribute("cameraToScreen");
            string screenToCamera = e.GetAttribute("screenToCamera");
            string worldToScreen = e.GetAttribute("worldToScreen");
            string worldPos = e.GetAttribute("worldPos");
            string worldDir = e.GetAttribute("worldDir");
            this.valueC.init(screen, module, time, localToWorld, localToScreen, cameraToWorld, cameraToScreen, screenToCamera, worldToScreen, worldPos, worldDir);
        }
    }

    public class ShowInfoTaskResource : ResourceTemplate<ShowInfoTask>
    {
        public static ShowInfoTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new ShowInfoTaskResource(manager, name, desc, e);
        }
        public ShowInfoTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {
            Font f;
            Program p;
            int c;
            Vector3i pos;
            float size;
            this.valueC = new ShowInfoTask();
            initInfoTask(manager, name, desc, e, out f, out p, out c, out size, out pos);
            this.valueC.init(f, p, c, size, pos);
        }

        public static void initInfoTask(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e,
        out Font f, out Program p, out int c, out float size, out Vector3i pos)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, "name,x,y,maxLines,font,fontSize,fontColor,fontProgram,");
            int x = 4;
            int y = -4;
            int maxLines = 8;
            Vector4f color = new Vector4f(1.0f, 0.0f, 0.0f, 0.0f);

            string fontName = "defaultFont";
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("font")))
            {
                fontName = getParameter(desc, e, "font");
            }
            f = manager.loadResource(fontName).get() as Font;

            size = f.getTileHeight();
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("fontSize")))
            {
                getFloatParameter(desc, e, "fontSize", out size);
            }

            if (!string.IsNullOrWhiteSpace(e.GetAttribute("x")))
            {
                getIntParameter(desc, e, "x", out x);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("y")))
            {
                getIntParameter(desc, e, "y", out y);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("maxLines")))
            {
                getIntParameter(desc, e, "maxLines", out maxLines);
            }
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("fontColor")))
            {
                string[] colors = e.GetAttribute("fontColor").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                color.X = float.Parse(colors[0], CultureInfo.InvariantCulture) / 255;
                color.Y = float.Parse(colors[1], CultureInfo.InvariantCulture) / 255;
                color.Z = float.Parse(colors[2], CultureInfo.InvariantCulture) / 255;
                color.W = float.Parse(colors[3], CultureInfo.InvariantCulture) / 255;
            }
            string fontProgram = "text;";
            if (!string.IsNullOrWhiteSpace(e.GetAttribute("fontProgram")))
            {
                fontProgram = e.GetAttribute("fontProgram");
            }
            p = manager.loadResource(fontProgram).get() as Program;
            c = ((int)(color.X * 255) & 0xFF) << 24;
            c |= ((int)(color.Y * 255) & 0xFF) << 16;
            c |= ((int)(color.Z * 255) & 0xFF) << 8;
            c |= (int)(color.W * 255) & 0xFF;
            pos = new Vector3i(x, y, maxLines);
        }
    }

    public class ShowLogTaskResource : ResourceTemplate<ShowLogTask>
    {
        public static ShowLogTaskResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new ShowLogTaskResource(manager, name, desc, e);
        }
        public ShowLogTaskResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(40, manager, name, desc)
        {

            Font f;
            Program p;
            int c;
            float fontHeight;
            Vector3i pos;
            this.valueC = new ShowLogTask();
            ShowInfoTaskResource.initInfoTask(manager, name, desc, e, out f, out p, out c, out fontHeight, out pos);
            this.valueC.init(f, p, fontHeight, pos);
        }
    }

}
