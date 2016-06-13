using System;
using Gwen.Control;
using Gwen.Xml;

namespace Gwen.XmlDesigner
{
	public class XmlChangedEventArgs : EventArgs
	{
		public string Xml { get; set; }
	}

	public class TextEditor : Component
	{
		[XmlEvent]
		public event ControlBase.GwenEventHandler<XmlChangedEventArgs> XmlChanged;

		[XmlProperty]
		public bool IsEditingDisabled { get { return m_TextBox.IsHidden; } set { m_TextBox.IsHidden = value; } }

		public TextEditor(ControlBase parent)
			: base(parent, new XmlStringSource(Xml))
		{
			m_XmlChangedEventArgs = new XmlChangedEventArgs();

			m_Timer = new Timer();
			m_Timer.Elapsed += TimerElapsed;
			m_Timer.Interval = 1500;
			m_Timer.IsOneTime = true;
		}

		protected override void OnCreated()
		{
			m_Container = GetControl<Container>("TextEditorContainer");
			m_TextBox = GetControl<MultilineTextBox>("TextEditor");
		}

		public void SetXml(string title, string xml)
		{
			m_Container.Title = title;

			if (xml != null)
			{
				m_TextBox.IsHidden = false;
				m_TextBox.Text = xml;
			}
			else
			{
				m_TextBox.IsHidden = true;
			}

			m_Timer.Stop();
		}

		public void AddElement(string element)
		{
			m_TextBox.InsertText(String.Format("<{0}></{0}>", element));

			m_TextBox.Focus();
		}

		public void AddAttribute(string attribute)
		{
			m_TextBox.InsertText(String.Format(" {0}=\"\"", attribute));

			m_TextBox.Focus();
		}

		private void TimerElapsed(object sender, EventArgs args)
		{
			SendXmlChangedEvent();
		}

		private void SendXmlChangedEvent()
		{
			if (XmlChanged != null)
			{
				m_XmlChangedEventArgs.Xml = m_TextBox.Text;
				XmlChanged(m_TextBox, m_XmlChangedEventArgs);
			}

			m_TextBox.Focus();
		}

		public void OnTextChanged(ControlBase sender, EventArgs args)
		{
			m_Timer.Start();
		}

		private Container m_Container;
		private MultilineTextBox m_TextBox;

		private Timer m_Timer;

		private XmlChangedEventArgs m_XmlChangedEventArgs;

		private static readonly string Xml = @"<?xml version='1.0' encoding='UTF-8'?>
			<DockLayout>
				<Container Name='TextEditorContainer' Dock='Fill' Title='Xml'>
					<MultilineTextBox Name='TextEditor' Font='Courier New;12' TextChanged='OnTextChanged' />
				</Container>
			</DockLayout>
			";
	}
}
