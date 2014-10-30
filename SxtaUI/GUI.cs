using System;
using Control = Gwen.Control;
using Gwen.Control.Layout;
using Gwen.Control;
using Gwen;
using System.Drawing;
using OpenTK;
using Sxta.Render.Resources;
using System.Collections.Generic;

namespace Sxta.UI
{
    public class GuiManager : IDisposable
    {
        private Gwen.Input.OpenTK input;
        private Gwen.Renderer.OpenTK renderer;
        private Dictionary<string, Base> children = new Dictionary<string, Base>();

        public Canvas Canvas { get; set; }
        public Gwen.Skin.Base Skin { get; private set; }

        public GuiManager(string skinName = null)
        {
            renderer = new Gwen.Renderer.OpenTK();
            if (string.IsNullOrWhiteSpace(skinName))
                skinName = "Skin2.png";
            Skin = new Gwen.Skin.TexturedBase(renderer, skinName);
            Canvas = new Canvas(Skin);
        }

        public void InitGuiSystem(GameWindow parent)
        {
            input = new Gwen.Input.OpenTK(parent);
            input.Initialize(Canvas);
        }

        public void update(double dt)
        { }

        public void draw()
        {
            Canvas.RenderCanvas();
        }

        public void SetSize(int width, int height)
        {
            Canvas.SetSize(width, height);
        }

        public void AddGuiElement(Base child)
        {
            children.Add(child.Name, child);
        }
        public void RemoveGuiElement(string name)
        {
            children.Remove(name);
        }
        public Base this[string name]
        {
            get
            {
                if (children.ContainsKey(name))
                    return children[name];
                else
                {
                    Base found = null;
                    foreach (Base child in children.Values)
                        found = LookForChild(child, name);
                    if (found != null) return found;
                }
                return null;
            }
        }
        private Base LookForChild(Base obj, string name)
        {
            Base found = null;
            foreach (Base grandchild in obj.Children)
            {
                if (grandchild.Name == name)
                    return grandchild;
                else if (grandchild.Children.Count > 0)
                {
                    found = LookForChild(grandchild, name);
                    if (found != null) return found;
                }
            }
            return null;
        }
        public void Dispose()
        {
            if (Canvas != null)
            {
                Canvas.Dispose();
            }
            Skin.Dispose();
            renderer.Dispose();
        }
    }

    public class GuiElement : Canvas 
    {
        public GuiElement(GuiManager parent)
            : base(parent.Skin)
        {
        }
    }

    public class Window : GuiElement
    {
        public Window(GuiManager parent)
            : base(parent)
        {
        }
    }

#if PARA_BORRAR
    public class GUI : Canvas
    {
        private Control.Base m_LastControl;
        private readonly Control.StatusBar m_StatusBar;
        private readonly Control.ListBox m_TextOutput;
        private WindowControl m_LogWindow;
        private Control.Button buttonIA,
            buttonDF,
            buttonTE,
            buttonCP,
            buttonTACREF,
            buttonLB,
            buttonACMI,
            buttonCO,
            buttonTHEATER,
            buttonSTP,
            buttonEXIT;

        public double Fps; // set this in your rendering loop
        public String Note; // additional text to display in status bar
        private readonly GameWindow parentContext;


        public GUI(int width, int height, Gwen.Skin.Base skin, GameWindow parent)
            : base(skin)
        {
            this.SetSize(width, height);
            this.ShouldDrawBackground = false;
            this.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            this.parentContext = parent;

            Dock = Pos.Fill;

            Control.ImagePanel img = new Control.ImagePanel(this);
            img.ImageName = "Media/140130-F-BR512-102.png"; //main_scrn.png";
            img.Dock = Pos.Fill;

            buttonIA = new Control.Button(this);
            buttonIA.Text = "INSTANT ACTION";
            buttonIA.SetPosition(20, 100);
            buttonIA.SetSize(20 * 10, 25);
            buttonIA.Pressed += onButtonIA;
            //buttonIA.ShouldDrawBackground = false;

            buttonDF = new Control.Button(this);
            buttonDF.Text = "DOGFIGHT";
            //Align.PlaceDownLeft(buttonDF, buttonIA, 10);
            buttonDF.SetPosition(20, 100 + 30);
            buttonDF.SetSize(20 * 10, 25);
            buttonDF.Pressed += onButtonDF;

            buttonTE = new Control.Button(this);
            buttonTE.Text = "TACTICAL ENGAGEMENT";
            buttonTE.SetPosition(20, 100 + 30 * 2);
            buttonTE.SetSize(20 * 10, 25);
            buttonTE.Pressed += onButtonTE;

            buttonCP = new Control.Button(this);
            buttonCP.Text = "CAMPAIGN";
            buttonCP.SetPosition(20, 100 + 30 * 3);
            buttonCP.SetSize(20 * 10, 25);
            buttonCP.Pressed += onButtonCP;

            buttonTACREF = new Control.Button(this);
            buttonTACREF.Text = "TACTICAL REFERENCE";
            buttonTACREF.SetPosition(20, 100 + 30 * 4);
            buttonTACREF.SetSize(20 * 10, 25);
            buttonTACREF.Pressed += onButtonTACREF;

            buttonLB = new Control.Button(this);
            buttonLB.Text = "LOGBOOK";
            buttonLB.SetPosition(20, 100 + 30 * 5);
            buttonLB.SetSize(20 * 10, 25);
            buttonLB.Pressed += onButtonLB;

            buttonACMI = new Control.Button(this);
            buttonACMI.Text = "ACMI";
            buttonACMI.SetPosition(20, 100 + 30 * 6);
            buttonACMI.SetSize(20 * 10, 25);
            buttonACMI.Pressed += onButtonACMI;

            buttonCO = new Control.Button(this);
            buttonCO.Text = "COMMS";
            buttonCO.SetPosition(20, 100 + 30 * 7);
            buttonCO.SetSize(20 * 10, 25);
            buttonCO.Pressed += onButtonCO;

            buttonTHEATER = new Control.Button(this);
            buttonTHEATER.Text = "THEATER";
            buttonTHEATER.SetPosition(20, 100 + 30 * 8);
            buttonTHEATER.SetSize(20 * 10, 25);
            buttonTHEATER.Pressed += onButtonTHEATER;

            buttonSTP = new Control.Button(this);
            buttonSTP.Text = "SETUP";
            buttonSTP.SetPosition(20, 100 + 30 * 9);
            buttonSTP.SetSize(20 * 10, 25);
            buttonSTP.Pressed += onButtonSTP;

            buttonEXIT = new Control.Button(this);
            buttonEXIT.Text = "EXIT";
            buttonEXIT.TextColorOverride = Color.Red;
            buttonEXIT.SetPosition(20, 100 + 30 * 10);
            buttonEXIT.SetSize(20 * 10, 25);
            buttonEXIT.Pressed += onButtonEXIT;

            m_LogWindow = new WindowControl(this);
            m_LogWindow.SetSize(width, height / 4);
            m_LogWindow.Position(Pos.CenterH | Pos.Bottom);
            m_TextOutput = new Control.ListBox(m_LogWindow);
            m_TextOutput.Dock = Pos.Fill;

            m_StatusBar = new Control.StatusBar(this);
            m_StatusBar.Dock = Pos.Bottom;
            m_StatusBar.SendToBack();
            PrintText("Sxta UI Test started!");
        }


        private void OnCategorySelect(Base control)
        {
            if (m_LastControl != null)
            {
                m_LastControl.Hide();
            }
            Base test = control.UserData as Base;
            test.Show();
            m_LastControl = test;
        }

        public void PrintText(String str)
        {
            m_TextOutput.AddRow(str);
            m_TextOutput.ScrollToBottom();
        }

        protected override void Layout(Gwen.Skin.Base skin)
        {
            base.Layout(skin);
        }

        protected override void Render(Gwen.Skin.Base skin)
        {
            m_StatusBar.Text = String.Format("Sxta.UI - {0:F0} fps. {1}", Fps, Note);

            base.Render(skin);
        }

        public static void onButtonIA(Base control)
        {
            //PrintText("Button INSTANT ACTION.");
        }
        public static void onButtonDF(Base control)
        {
            //PrintText("Button DOGFIGHT.");
        }
        public static void onButtonTE(Base control)
        {
            //PrintText("Button TACTICAL ENGAGEMENT.");
        }
        public static void onButtonCP(Base control)
        {
            //PrintText("Button CAMPAIGN.");
        }
        public static void onButtonTACREF(Base control)
        {
            //PrintText("Button TACTICAL REFERENCE.");
        }
        public static void onButtonLB(Base control)
        {
            //PrintText("Button LOGBOOK.");
            // Logbook logbook = new Logbook(this);
        }
        public static void onButtonACMI(Base control)
        {
            //PrintText("Button ACMI.");
        }
        public static void onButtonCO(Base control)
        {
            //PrintText("Button COMMS.");
        }
        public static void onButtonTHEATER(Base control)
        {
            //PrintText("Button THEATER.");
        }
        public static void onButtonSTP(Base control)
        {
            //PrintText("Button SETUP.");
        }
        public static void onButtonEXIT(Base control)
        {
            //PrintText("Button EXIT.");
            //MessageBox window = new MessageBox(GetCanvas(), "Do you want to quit?");
            //window.Position(Pos.Center);
            //parentContext.Exit();
        }
    }
#endif

    public class Logbook : WindowControl
    {
        public Logbook(Base parent)
            : base(parent, "LOGBOOK")
        {
            this.SetSize(610, 450);
            this.SetPosition(250, 60);

            Control.ImagePanel img = new Control.ImagePanel(this);
            img.ImageName = "Media/logbook.png";
            img.Dock = Pos.Fill;
            img.SendToBack();

            Control.GroupBox gbCareer = new Control.GroupBox(this);
            //gbCareer.AutoSizeToContents = true;
            gbCareer.SetSize((610 - 40) / 4, 450 / 3);
            gbCareer.Text = "CAREER";
            gbCareer.SetPosition(5, 135);
            gbCareer.TextColor = Color.WhiteSmoke;
            Control.Label labelCommissioned = new Control.Label(gbCareer);
            labelCommissioned.AutoSizeToContents = true;
            labelCommissioned.Text = "Commissioned";
            labelCommissioned.TextColor = Color.WhiteSmoke;

            Control.Label labelFltHrs = new Control.Label(gbCareer);
            labelFltHrs.AutoSizeToContents = true;
            labelFltHrs.TextColor = Color.WhiteSmoke;
            labelFltHrs.Text = "Flight Hours";
            Align.PlaceDownLeft(labelFltHrs, labelCommissioned, 5);

            Control.Label labelAceFactor = new Control.Label(gbCareer);
            labelAceFactor.AutoSizeToContents = true;
            labelAceFactor.Text = "Ace Factor";
            labelAceFactor.TextColor = Color.WhiteSmoke;
            Align.PlaceDownLeft(labelAceFactor, labelFltHrs, 5);

            Control.Label labelTotalScore = new Control.Label(gbCareer);
            labelTotalScore.AutoSizeToContents = true;
            labelTotalScore.Text = "Total Score";
            labelTotalScore.TextColor = Color.WhiteSmoke;
            Align.PlaceDownLeft(labelTotalScore, labelAceFactor, 5);

            Control.Label labelLastMisPts = new Control.Label(gbCareer);
            labelLastMisPts.AutoSizeToContents = true;
            labelLastMisPts.Text = "Last Mis Points";
            labelLastMisPts.TextColor = Color.WhiteSmoke;
            Align.PlaceDownLeft(labelLastMisPts, labelTotalScore, 5);

            Control.GroupBox gbCampaign = new Control.GroupBox(this);
            //gbCampaign.AutoSizeToContents = true;
            gbCampaign.SetSize((610 - 40) * 2 / 4, 400 / 3);
            gbCampaign.Text = "CAMPAIGN";
            gbCampaign.TextColor = Color.WhiteSmoke;
            gbCampaign.SetPosition((610 - 40) / 4 + 20, 450 / 3);

            Control.GroupBox gbDogfight = new Control.GroupBox(this);
            //gbCampaign.AutoSizeToContents = true;
            gbDogfight.SetSize((610 - 20) / 4, 400 / 3);
            gbDogfight.Text = "DOGFIGHT";
            gbDogfight.TextColor = Color.WhiteSmoke;
            gbDogfight.SetPosition((610 - 40) * 3 / 4 + 20, 450 / 3);

        }
    }
}
