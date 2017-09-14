using System;

namespace Gwen
{
	/// <summary>
	/// Font metrics.
	/// </summary>
	public struct FontMetrics
	{
		public float EmHeightPixels { get; internal set; }
		public float AscentPixels { get; internal set; }
		public float DescentPixels { get; internal set; }
		public float CellHeightPixels { get; internal set; }
		public float InternalLeadingPixels { get; internal set; }
		public float LineSpacingPixels { get; internal set; }
		public float ExternalLeadingPixels { get; internal set; }

		public float Top { get { return InternalLeadingPixels; } }
		public float Baseline { get { return AscentPixels; } }
		public float Bottom { get { return CellHeightPixels; } }

		public FontMetrics(Font font)
		{
			EmHeightPixels = font.RealSize;
			InternalLeadingPixels = 0.0f;
			ExternalLeadingPixels = 0.0f;
			DescentPixels = 0.0f;
			AscentPixels = EmHeightPixels;
			CellHeightPixels = EmHeightPixels;
			LineSpacingPixels = EmHeightPixels;
		}

		public FontMetrics
		(
			float emHeightPixels,
			float ascentPixels,
			float descentPixels,
			float cellHeightPixels,
			float internalLeadingPixels,
			float lineSpacingPixels,
			float externalLeadingPixels
		)
		{
			EmHeightPixels = emHeightPixels;
			AscentPixels = ascentPixels;
			DescentPixels = descentPixels;
			CellHeightPixels = cellHeightPixels;
			InternalLeadingPixels = internalLeadingPixels;
			LineSpacingPixels = lineSpacingPixels;
			ExternalLeadingPixels = externalLeadingPixels;
		}
	}
}
