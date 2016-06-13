using System;
using Gwen.Control;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Layout", Order = 403)]
	public class CrossSplitter : GUnit
    {
        private int m_CurZoom;
        private readonly Control.CrossSplitter m_Splitter;

        public CrossSplitter(ControlBase parent)
            : base(parent)
        {
            m_CurZoom = 0;

            m_Splitter = new Control.CrossSplitter(this);
            m_Splitter.Dock = Dock.Fill;

            {
                VerticalSplitter splitter = new VerticalSplitter(m_Splitter);
                Control.Button button1 = new Control.Button(splitter);
                button1.Text = "Vertical left";
                Control.Button button2 = new Control.Button(splitter);
                button2.Text = "Vertical right";
                splitter.SetPanel(0, button1);
                splitter.SetPanel(1, button2);
                m_Splitter.SetPanel(0, splitter);
            }

            {
                HorizontalSplitter splitter = new HorizontalSplitter(m_Splitter);
                Control.Button button1 = new Control.Button(splitter);
                button1.Text = "Horizontal up";
                Control.Button button2 = new Control.Button(splitter);
                button2.Text = "Horizontal down";
                splitter.SetPanel(0, button1);
                splitter.SetPanel(1, button2);
                m_Splitter.SetPanel(1, splitter);
            }

            {
                HorizontalSplitter splitter = new HorizontalSplitter(m_Splitter);
                Control.Button button1 = new Control.Button(splitter);
                button1.Text = "Horizontal up";
                Control.Button button2 = new Control.Button(splitter);
                button2.Text = "Horizontal down";
                splitter.SetPanel(0, button1);
                splitter.SetPanel(1, button2);
                m_Splitter.SetPanel(2, splitter);
            }

            {
                VerticalSplitter splitter = new VerticalSplitter(m_Splitter);
                Control.Button button1 = new Control.Button(splitter);
				button1.Text = "Vertical left";
                Control.Button button2 = new Control.Button(splitter);
                button2.Text = "Vertical right";
                splitter.SetPanel(0, button1);
                splitter.SetPanel(1, button2);
                m_Splitter.SetPanel(3, splitter);
            }

            //Status bar to hold unit testing buttons
            Control.StatusBar pStatus = new Control.StatusBar(this);
            pStatus.Dock = Dock.Bottom;

            {
                Control.Button pButton = new Control.Button(pStatus);
                pButton.Text = "Zoom";
                pButton.Clicked += ZoomTest;
                pStatus.AddControl(pButton, false);
            }

            {
                Control.Button pButton = new Control.Button(pStatus);
                pButton.Text = "UnZoom";
                pButton.Clicked += UnZoomTest;
                pStatus.AddControl(pButton, false);
            }

            {
                Control.Button pButton = new Control.Button(pStatus);
                pButton.Text = "CenterPanels";
                pButton.Clicked += CenterPanels;
                pStatus.AddControl(pButton, true);
            }

            {
                Control.Button pButton = new Control.Button(pStatus);
                pButton.Text = "Splitters";
                pButton.Clicked += ToggleSplitters;
                pStatus.AddControl(pButton, true);
            }
        }

		void ZoomTest(ControlBase control, EventArgs args)
        {
            m_Splitter.Zoom(m_CurZoom);
            m_CurZoom++;
            if (m_CurZoom == 4)
                m_CurZoom = 0;
        }

		void UnZoomTest(ControlBase control, EventArgs args)
        {
            m_Splitter.UnZoom();
        }

		void CenterPanels(ControlBase control, EventArgs args)
        {
            m_Splitter.CenterPanels();
            m_Splitter.UnZoom();
        }

		void ToggleSplitters(ControlBase control, EventArgs args)
        {
            m_Splitter.SplittersVisible = !m_Splitter.SplittersVisible;
        }

#if false
		protected override void Layout(Skin.Base skin)
        {
            
        }
#endif
    }
}
