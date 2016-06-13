using System;
using Gwen.Control;
using Gwen.Xml;

namespace Gwen.XmlDesigner
{
	public class ExceptionEventArgs : EventArgs
	{
		public Exception Exception { get; set; }
	}

	public class Viewer : Component
	{
		[XmlEvent]
		public event ControlBase.GwenEventHandler<ExceptionEventArgs> Exception;

		[XmlProperty]
		public bool DebugOutlines { get { return m_DebugOutlines; } set { m_DebugOutlines = value; if (m_CurrentElement != null) m_CurrentElement.DrawDebugOutlines = value; } }

		public Viewer(ControlBase parent)
			: base(parent, new XmlStringSource(Xml))
		{
			m_ExceptionEventArgs = new ExceptionEventArgs();
		}

		public void SetXml(string xml)
		{
			RemoveCurrent();

			try
			{
				XmlStringSource xmlSource = new XmlStringSource(xml);
				using (Parser parser = new Parser(xmlSource.GetStream()))
				{
					m_CurrentElement = parser.Parse(this.View);
				}

				m_CurrentElement.Show();

				m_CurrentElement.DrawDebugOutlines = m_DebugOutlines;

				if (m_CurrentElement is Window || m_CurrentElement is ToolWindow)
				{
					if (m_CurrentElement is Window)
						((Window)m_CurrentElement).IsDraggingEnabled = false;
					else
						((ToolWindow)m_CurrentElement).IsDraggingEnabled = false;

					m_CurrentElement.MaximumSize = this.View.ActualSize;
					m_CurrentElement.Position = View.LocalPosToCanvas(Point.Zero);
				}

				if (Exception != null)
				{
					m_ExceptionEventArgs.Exception = null;
					Exception(View, m_ExceptionEventArgs);
				}
			}
			catch (Exception ex)
			{
				if (Exception != null)
				{
					m_ExceptionEventArgs.Exception = ex;
					Exception(View, m_ExceptionEventArgs);
				}

				RemoveCurrent();
			}
		}

		private void RemoveCurrent()
		{
			if (m_CurrentElement != null)
			{
				if (m_CurrentElement is Window || m_CurrentElement is ToolWindow)
				{
					m_CurrentElement.Parent.RemoveChild(m_CurrentElement, true);
				}
			}

			View.DeleteAllChildren();

			m_CurrentElement = null;
		}

		private ControlBase m_CurrentElement;

		private bool m_DebugOutlines;

		private ExceptionEventArgs m_ExceptionEventArgs;

		private static readonly string Xml = @"<?xml version='1.0' encoding='UTF-8'?>
			<DockLayout />
			";
	}
}
