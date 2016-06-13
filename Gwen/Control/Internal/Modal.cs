using System;

namespace Gwen.Control.Internal
{
    /// <summary>
    /// Modal control for windows.
    /// </summary>
    public class Modal : ControlBase
    {
		public Color? BackgroundColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Modal"/> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Modal(ControlBase parent)
            : base(parent)
        {
            KeyboardInputEnabled = true;
            MouseInputEnabled = true;
            ShouldDrawBackground = true;
			BackgroundColor = null;
        }

		protected override Size Measure(Size availableSize)
		{
			base.Measure(availableSize);

			return availableSize;
		}

		protected override Size Arrange(Size finalSize)
		{
			base.Arrange(finalSize);

			return finalSize;
		}

		/// <summary>
		/// Renders the control using specified skin.
		/// </summary>
		/// <param name="skin">Skin to use.</param>
		protected override void Render(Skin.SkinBase skin)
        {
            skin.DrawModalControl(this, BackgroundColor);
        }
    }
}
