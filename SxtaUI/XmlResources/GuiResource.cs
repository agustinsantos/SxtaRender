using Gwen.Control;
using log4net;
using Sxta.Math;
using Sxta.Render.OpenGLExt;
using Sxta.Render.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sxta.UI.XmlResources
{
    public static class RegisterResourceReader
    {
        public static void RegisterResources()
        {
            ResourceFactory.getInstance().addType("guimanager", GuiManagerResource.Create);
            ResourceFactory.getInstance().addType("panel", PanelResource.Create);
            ResourceFactory.getInstance().addType("window", WindowControlResource.Create);
            ResourceFactory.getInstance().addType("button", ButtonResource.Create);
            ResourceFactory.getInstance().addType("imagepanel", ImagePanelResource.Create);
            ResourceFactory.getInstance().addType("statusbar", StatusBarResource.Create);
            ResourceFactory.getInstance().addType("listbox", ListBoxResource.Create);
            ResourceFactory.getInstance().addType("groupbox", GroupBoxResource.Create);
        }
    }
    public class GuiElementResource
    {
        public static GuiElementResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new GuiElementResource(manager, name, desc, e, context);
        }
        public GuiElementResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
            : base()
        { }
    }
    /// <summary>
    ///  A template Resource class to ease the implementation of concrete Resource
    /// subclasses. This template class takes care of the two phase commit for the
    /// update of resources, provided that the actual %resource class implements a
    /// swap(ptr<C>) method that can swap two instances of resources of class C.
    ///
    /// @ingroup resource
    ///
    /// @tparam o the update order of resources of class C (see #getUpdateOrder).
    /// @tparam C the concrete %resource class.
    /// </summary>
    public class GuiResource<C> : Resource
    {
        protected readonly int updateorder;
        protected C valueC;

        public override object get()
        {
            return valueC;
        }
        public override void clearValue(bool dispose)
        {
            if (dispose && valueC != null && valueC is IDisposable)
                ((IDisposable)valueC).Dispose();
            valueC = default(C);
        }

        /*
         * Creates a new %resource of class C.
         *
         * @param manager the manager that will keep track of this %resource.
         * @param name the name of this %resource.
         * @param desc the descriptor of this %resource.
         */
        public GuiResource(int order, ResourceManager manager, string name, ResourceDescriptor desc)
            : base(manager, name, desc)
        {
            this.updateorder = order;
        }

        /*
         * Returns the template parameter o.
         */
        public override int getUpdateOrder()
        {
            return updateorder;
        }

        /*
         * If the descriptor of this method has not changed, does nothing and
         * returns true. Otherwise creates a new instance of this %resource using the
         * new descriptor #newDesc and then swaps it with this instance, saving the
         * current valueC in #oldValue. If the %resource creation fails, does nothing
         * and returns false.
         */
        public override bool prepareUpdate()
        {
            if (base.prepareUpdate())
            { // if the descriptor has changed
                oldValue = default(C);
                try
                {
                    // creates a new resource using this new descriptor
                    //TODO oldValue = (C)ResourceFactory.getInstance().create( manager, base.name, newDesc).get();
                }
                catch (Exception e)
                {
                    log.Error("Exception with resource " + e);
                }
                if (oldValue != null)
                { // if the creation is a success
                    return true;
                }
                return false; // if the creation fails, do nothing
            }
            return true; // nothing to do if the descriptor has not changed
        }

        /*
         * If commit is true swaps the #desc and #newDesc fields, and sets the
         * #oldValue to null. If commit is false reverts the changes made in
         * #prepareUpdate by swapping again this instance with #oldValue.
         */
        public override void doUpdate(bool commit)
        {
            if (commit)
            { // if we must commit changes
                if (newDesc != null)
                { // and if there was some changes
                    desc = newDesc; // we set the descriptor to its new value
                    if (log.IsInfoEnabled)
                    {
                        log.Info("Resource '" + base.getName() + "' updated");
                    }
                    // nothing to do for the actual resource,
                    // this has already been done in prepareUpdate with the swap
                }
            }
            else
            { // if we must abort changes
                if (oldValue != null)
                { // and if there was some changes
                }
            }
            // in all cases we set oldValue and newDesc to null to release memory
            oldValue = default(C);
            newDesc = null;
        }

        /*
         * Returns true if this %resource has changed.
         */
        public override bool changed()
        {
            return oldValue != null;
        }


        /*
         * The old valueC of this %resource.
         */
        protected C oldValue;

        ///*
        // * Calls ResourceManager#releaseResource to release this %resource.
        // */
        //protected virtual void doRelease()
        //{
        //    if (base.manager != null)
        //    {
        //        base.manager.releaseResource(this);
        //    }
        //}
        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    if (valueC is IDisposable)
        //        ((IDisposable)valueC).Dispose();
        //}

        public const string COMMONATTR = "name,position,size,dock,";
        public static void ProcessXMl(Base valueC, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            string val = e.GetAttribute("name");
            if (!string.IsNullOrWhiteSpace(val))
            {
                valueC.Name = val;
            }

            val = e.GetAttribute("size");
            if (!string.IsNullOrWhiteSpace(val))
            {
                int width = 800, height = 600;
                Resource.get2IntParameter(val, out width, out height);
                valueC.SetSize(width, height);
            }

            val = e.GetAttribute("position");
            if (!string.IsNullOrWhiteSpace(val))
            {
                Gwen.Pos pos;
                if (Enum.TryParse<Gwen.Pos>(val, true, out pos))
                {
                    valueC.Position(pos);
                }
                else
                {
                    int x = 0, y = 0;
                    Resource.get2IntParameter(val, out x, out y);
                    valueC.SetPosition(x, y);
                }
            }
            val = e.GetAttribute("dock");
            if (!string.IsNullOrWhiteSpace(val))
            {
                Gwen.Pos pos;
                if (Enum.TryParse<Gwen.Pos>(val, true, out pos))
                {
                    valueC.Dock = pos;
                }
            }
            // TODO ... Process other properties
        }

        public static Base.GwenEventHandler GetEventHandlerByName(string methodName)
        {
            int i = methodName.LastIndexOf('.');
            string lhs = i < 0 ? methodName : methodName.Substring(0, i),
                   rhs = i < 0 ? "" : methodName.Substring(i + 1);

            Type target = Type.GetType(lhs);
            MethodInfo method = target.GetMethod(rhs, BindingFlags.Public | BindingFlags.Static);
            return (Base.GwenEventHandler)Delegate.CreateDelegate(typeof(Base.GwenEventHandler), method);
        }

        private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }

    public class GuiManagerResource : GuiResource<GuiManager>
    {
        public static GuiManagerResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new GuiManagerResource(manager, name, desc, e, context);
        }
        public GuiManagerResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, COMMONATTR + "skin,");
            string skin = getParameter(desc, e, "skin");
            this.valueC = new GuiManager(skin);
            foreach (XmlNode n in e.ChildNodes)
            {
                XmlElement f = n as XmlElement;
                if (f != null)
                {
                    Resource res;
                    if (f.Name == "guielement")
                    {
                        string filename = f.GetAttribute("filename");
                        res = manager.loadResource(filename, this.valueC);
                    }
                    else
                    {
                        res = ResourceFactory.getInstance().create(manager, "", desc, f, this.valueC);
                    }
                    this.valueC.AddGuiElement(res.get() as Base);
                    //if (res is PanelResource && !((Window)res.get()).IsDisabled)
                    //    this.valueC.Canvas = res.get() as Window;

                }
            }

        }

        private Base LoadGuiElement(XmlElement e = null, object context = null)
        {
            string filename = e.GetAttribute("filename");
            if (!string.IsNullOrWhiteSpace(filename))
            {
            }
            throw new NotImplementedException();
        }
    }

    public class PanelResource : GuiResource<Window>
    {
        public static PanelResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new PanelResource(manager, name, desc, e, context);
        }
        public PanelResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, COMMONATTR + "");
            this.valueC = new Window((GuiManager)context);

            ProcessXMl(this.valueC, desc, e, context);
            foreach (XmlNode node in e.ChildNodes)
            {
                XmlElement f = node as XmlElement;
                if (f != null)
                {
                    ResourceFactory.getInstance().create(manager, "", desc, f, this.valueC);
                }
            }
        }
    }
    public class WindowControlResource : GuiResource<WindowControl>
    {
        public static WindowControlResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new WindowControlResource(manager, name, desc, e, context);
        }
        public WindowControlResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, COMMONATTR + "title,");

            Base parent;
            if (context != null && context is GuiManager)
                parent = ((GuiManager)context).Canvas;
            else
                parent = context as Base;
            this.valueC = new WindowControl(parent);
            string title = getParameter(desc, e, "title");
            this.valueC.Caption = title;

            ProcessXMl(this.valueC, desc, e, context);
            foreach (XmlNode node in e.ChildNodes)
            {
                XmlElement f = node as XmlElement;
                if (f != null)
                {
                    ResourceFactory.getInstance().create(manager, "", desc, f, this.valueC);
                }
            }
        }
    }
    public class ButtonResource : GuiResource<Button>
    {
        public static ButtonResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new ButtonResource(manager, name, desc, e, context);
        }
        public ButtonResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            e = e == null ? desc.descriptor : e;
            checkParameters(desc, e, COMMONATTR + "text,pressed,");
            string text = getParameter(desc, e, "text");
            string pressed = getParameter(desc, e, "pressed");

            this.valueC = new Button((Base)context);
            this.valueC.Text = text;
            if (!string.IsNullOrWhiteSpace(pressed))
            {
                this.valueC.Pressed += GetEventHandlerByName(pressed);
            }

            ProcessXMl(this.valueC, desc, e, context);
        }
    }

    public class ImagePanelResource : GuiResource<ImagePanel>
    {
        public static ImagePanelResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new ImagePanelResource(manager, name, desc, e, context);
        }
        public ImagePanelResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            checkParameters(desc, e, COMMONATTR + "texture,");
            string texture = getParameter(desc, e, "texture");

            this.valueC = new ImagePanel((Base)context);
            this.valueC.ImageName = texture;

            ProcessXMl(this.valueC, desc, e, context);
        }
    }
    public class StatusBarResource : GuiResource<StatusBar>
    {
        public static StatusBarResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new StatusBarResource(manager, name, desc, e, context);
        }
        public StatusBarResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            checkParameters(desc, e, COMMONATTR + "");

            this.valueC = new StatusBar(((GuiManager)context).Canvas);

            ProcessXMl(this.valueC, desc, e, context);
        }
    }

    public class ListBoxResource : GuiResource<ListBox>
    {
        public static ListBoxResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new ListBoxResource(manager, name, desc, e, context);
        }
        public ListBoxResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            checkParameters(desc, e, COMMONATTR + "");
            Base parent;
            if (context != null && context is GuiManager)
                parent = ((GuiManager)context).Canvas;
            else
                parent = context as Base;
            this.valueC = new ListBox(parent);

            ProcessXMl(this.valueC, desc, e, context);
        }
    }
    public class LabelResource : GuiResource<Label>
    {
        public static LabelResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new LabelResource(manager, name, desc, e, context);
        }
        public LabelResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            checkParameters(desc, e, COMMONATTR + "text, textcolor, font");
            this.valueC = new Label((Base)context);

            ProcessXMl(this.valueC, desc, e, context);
            ProcessLabelAttr(this.valueC, desc, e, context);
        }

        public static void ProcessLabelAttr(Label label, ResourceDescriptor desc, XmlElement e, object context = null)
        {
            string text = getParameter(desc, e, "text");
            if (!string.IsNullOrWhiteSpace(text))
                label.Text = text;
            string textcolor = getParameter(desc, e, "textcolor");
            if (!string.IsNullOrWhiteSpace(textcolor))
            {
                label.TextColor = System.Drawing.Color.FromName(textcolor);
            }
        }
    }
    public class GroupBoxResource : GuiResource<GroupBox>
    {
        public static GroupBoxResource Create(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null)
        {
            return new GroupBoxResource(manager, name, desc, e, context);
        }
        public GroupBoxResource(ResourceManager manager, string name, ResourceDescriptor desc, XmlElement e = null, object context = null) :
            base(100, manager, name, desc)
        {
            checkParameters(desc, e, COMMONATTR + "text, textcolor, font");
            this.valueC = new GroupBox((Base)context);

            ProcessXMl(this.valueC, desc, e, context);
            LabelResource.ProcessLabelAttr(this.valueC, desc, e, context);
        }
    }
}
