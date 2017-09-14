using System;
using System.Collections.Generic;

namespace Gwen
{
	public class FontCache : IDisposable
	{
		public static Font GetFont(Renderer.RendererBase renderer, string faceName, int size = 10, FontStyle style = 0)
		{
			if (m_Instance == null)
				m_Instance = new FontCache();

			return m_Instance.InternalGetFont(renderer, faceName, size, style);
		}

		public static void FreeCache()
		{
			if (m_Instance != null)
				m_Instance.Dispose();
		}

        private Font InternalGetFont(Renderer.RendererBase renderer, string faceName, int size, FontStyle style)
		{
			string id = String.Format("{0};{1};{2}", faceName, size, (int)style);
			Font font;
			if (!m_FontCache.TryGetValue(id, out font))
			{
				font = new Font(renderer, faceName, size);

				if ((style & FontStyle.Bold) != 0)
					font.Bold = true;
				if ((style & FontStyle.Italic) != 0)
					font.Italic = true;
				if ((style & FontStyle.Underline) != 0)
					font.Underline = true;
				if ((style & FontStyle.Strikeout) != 0)
					font.Strikeout = true;

				m_FontCache[id] = font;
			}

			return font;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			m_Instance = null;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var font in m_FontCache)
				{
					font.Value.Dispose();
				}

				m_FontCache.Clear();
			}
		}

		private static FontCache m_Instance = null;

		private Dictionary<string, Font> m_FontCache = new Dictionary<string, Font>();
	}
}
