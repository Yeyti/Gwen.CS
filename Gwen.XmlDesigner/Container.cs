using System;
using Gwen.Control;
using Gwen.Xml;

namespace Gwen.XmlDesigner
{
	public class Container : Component
	{
		public Container(ControlBase parent)
			: base(parent, new XmlStringSource(Xml))
		{
		}

		[XmlProperty]
		public string Title { get { return m_Title.Text; } set { m_Title.Text = value; } }

		protected override void OnCreated()
		{
			m_Border = GetControl<Border>("Border");
			m_Title = GetControl<Label>("Title");
		}

		protected override void OnChildAdded(ControlBase child)
		{
			child.Dock = Dock.Fill;
			child.Margin = Margin.Two;
			child.Parent = m_Border;
		}

		private Border m_Border;
		private Label m_Title;

		private static readonly string Xml = @"<?xml version='1.0' encoding='UTF-8'?>
			<Border Name='Border' BorderType='PanelNormal'>
				<Border Dock='Top' Margin='2' BorderType='PanelDark'>
					<Label Name='Title' Margin='2' Dock='Fill' />
				</Border>
			</Border>
			";
	}
}
