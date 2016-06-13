using System;
using Gwen.Control;

namespace Gwen.Control.Internal
{
	public class InnerContentControl : ContentControl
	{
		public InnerContentControl(ControlBase parent)
			: base(parent)
		{
			MouseInputEnabled = false;
			KeyboardInputEnabled = false;
		}

		protected override void OnChildAdded(ControlBase child)
		{
			if (m_InnerPanel == null)
				m_InnerPanel = Children[0];

			base.OnChildAdded(child);
		}

		protected override Size Measure(Size availableSize)
		{
			if (m_InnerPanel != null)
			{
				return m_InnerPanel.DoMeasure(availableSize - Padding) + Padding;
			}

			return Size.Zero;
		}

		protected override Size Arrange(Size finalSize)
		{
			if (m_InnerPanel != null)
				m_InnerPanel.DoArrange(new Rectangle(Padding.Left, Padding.Top, finalSize - Padding));

			return finalSize;
		}

		public override ControlBase FindChildByName(string name, bool recursive = false)
		{
			if (m_InnerPanel != null && m_InnerPanel.Name == name)
				return m_InnerPanel;

			return base.FindChildByName(name, recursive);
		}
	}
}
