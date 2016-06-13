using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Gwen.Renderer.OpenTK
{
	public abstract class OpenTKBase : RendererBase
	{
		protected Color m_Color;

		private readonly Dictionary<Tuple<String, Font>, TextRenderer> m_StringCache;
		private readonly Graphics m_Graphics; // only used for text measurement
		protected int m_DrawCallCount;
		protected bool m_ClipEnabled;
		protected bool m_TextureEnabled;
		static protected int m_LastTextureID;

		private StringFormat m_StringFormat;

		private int m_GLVersion;

		public OpenTKBase()
            : base()
        {
			m_GLVersion = GL.GetInteger(GetPName.MajorVersion) * 10 + GL.GetInteger(GetPName.MinorVersion);

			m_StringCache = new Dictionary<Tuple<String, Font>, TextRenderer>();
			m_Graphics = Graphics.FromImage(new Bitmap(1024, 1024, PixelFormat.Format32bppArgb));
			m_StringFormat = new StringFormat(StringFormat.GenericTypographic);
			m_StringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
		}

		public override void Dispose()
		{
			FlushTextCache();
			base.Dispose();
		}

		protected override void OnScaleChanged(float oldScale)
		{
			FlushTextCache();
		}

		protected abstract void Flush();

		/// <summary>
		/// Returns number of cached strings in the text cache.
		/// </summary>
		public int TextCacheSize { get { return m_StringCache.Count; } }

		public int DrawCallCount { get { return m_DrawCallCount; } }

		public abstract int VertexCount { get; }

		public int GLVersion { get { return m_GLVersion; } }

		/// <summary>
		/// Clears the text rendering cache. Make sure to call this if cached strings size becomes too big (check TextCacheSize).
		/// </summary>
		public void FlushTextCache()
		{
			// todo: some auto-expiring cache? based on number of elements or age
			foreach (var textRenderer in m_StringCache.Values)
			{
				textRenderer.Dispose();
			}
			m_StringCache.Clear();
		}

		public override void DrawFilledRect(Rectangle rect)
		{
			if (m_TextureEnabled)
			{
				Flush();
				GL.Disable(EnableCap.Texture2D);
				m_TextureEnabled = false;
			}

			rect = Translate(rect);

			DrawRect(rect);
		}

		public override Color DrawColor
		{
			get { return m_Color; }
			set
			{
				m_Color = value;
			}
		}

		public override void StartClip()
		{
			m_ClipEnabled = true;
		}

		public override void EndClip()
		{
			m_ClipEnabled = false;
		}

		public override void DrawTexturedRect(Texture t, Rectangle rect, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1)
		{
			// Missing image, not loaded properly?
			if (null == t.RendererData)
			{
				DrawMissingImage(rect);
				return;
			}

			int tex = (int)t.RendererData;
			rect = Translate(rect);

			bool differentTexture = (tex != m_LastTextureID);
			if (!m_TextureEnabled || differentTexture)
			{
				Flush();
			}

			if (!m_TextureEnabled)
			{
				GL.Enable(EnableCap.Texture2D);
				m_TextureEnabled = true;
			}

			if (differentTexture)
			{
				GL.BindTexture(TextureTarget.Texture2D, tex);
				m_LastTextureID = tex;
			}

			DrawRect(rect, u1, v1, u2, v2);
		}

		protected abstract void DrawRect(Rectangle rect, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1);

		public override bool LoadFont(Font font)
		{
			font.RealSize = (float)Math.Ceiling(font.Size * Scale);
			System.Drawing.Font sysFont = font.RendererData as System.Drawing.Font;

			if (sysFont != null)
				sysFont.Dispose();

			System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
			if (font.Bold) fontStyle |= System.Drawing.FontStyle.Bold;
			if (font.Italic) fontStyle |= System.Drawing.FontStyle.Italic;
			if (font.Underline) fontStyle |= System.Drawing.FontStyle.Underline;
			if (font.Strikeout) fontStyle |= System.Drawing.FontStyle.Strikeout;

			// apaprently this can't fail @_@
			// "If you attempt to use a font that is not supported, or the font is not installed on the machine that is running the application, the Microsoft Sans Serif font will be substituted."
			sysFont = new System.Drawing.Font(font.FaceName, font.RealSize, fontStyle);
			font.RendererData = sysFont;

			return true;
		}

		public override void FreeFont(Font font)
		{
			if (font.RendererData == null)
				return;

			System.Drawing.Font sysFont = font.RendererData as System.Drawing.Font;
			if (sysFont == null)
				throw new InvalidOperationException("Freeing empty font");

			sysFont.Dispose();
			font.RendererData = null;
		}

		public override FontMetrics GetFontMetrics(Font font)
		{
			System.Drawing.Font sysFont = font.RendererData as System.Drawing.Font;

			if (sysFont == null || Math.Abs(font.RealSize - font.Size * Scale) > 2)
			{
				FreeFont(font);
				LoadFont(font);
				sysFont = font.RendererData as System.Drawing.Font;
			}

			// from: http://csharphelper.com/blog/2014/08/get-font-metrics-in-c
			float emHeight = sysFont.FontFamily.GetEmHeight(sysFont.Style);
			float emHeightPixels = ConvertToPixels(sysFont.Size, sysFont.Unit);
			float designToPixels = emHeightPixels / emHeight;

			float ascentPixels = designToPixels * sysFont.FontFamily.GetCellAscent(sysFont.Style);
			float descentPixels = designToPixels * sysFont.FontFamily.GetCellDescent(sysFont.Style);
			float cellHeightPixels = ascentPixels + descentPixels;
			float internalLeadingPixels = cellHeightPixels - emHeightPixels;
			float lineSpacingPixels = designToPixels * sysFont.FontFamily.GetLineSpacing(sysFont.Style);
			float externalLeadingPixels = lineSpacingPixels - cellHeightPixels;

			FontMetrics fm = new FontMetrics
			(
				emHeightPixels,
				ascentPixels,
				descentPixels,
				cellHeightPixels,
				internalLeadingPixels,
				lineSpacingPixels,
				externalLeadingPixels
			);

			return fm;
		}

		private float ConvertToPixels(float value, GraphicsUnit unit)
		{
			switch (unit)
			{
				case GraphicsUnit.Document: value *= m_Graphics.DpiX / 300; break;
				case GraphicsUnit.Inch: value *= m_Graphics.DpiX; break;
				case GraphicsUnit.Millimeter: value *= m_Graphics.DpiX / 25.4F; break;
				case GraphicsUnit.Pixel: break;
				case GraphicsUnit.Point: value *= m_Graphics.DpiX / 72; break;
				default: throw new Exception("Unknown unit " + unit.ToString());
			}

			return value;
		}

		public override Size MeasureText(Font font, string text)
		{
			System.Drawing.Font sysFont = font.RendererData as System.Drawing.Font;

			if (sysFont == null || Math.Abs(font.RealSize - font.Size * Scale) > 2)
			{
				FreeFont(font);
				LoadFont(font);
				sysFont = font.RendererData as System.Drawing.Font;
			}

			var key = new Tuple<String, Font>(text, font);

			if (m_StringCache.ContainsKey(key))
			{
				var tex = m_StringCache[key].Texture;
				return new Size(tex.Width, tex.Height);
			}

			SizeF TabSize = m_Graphics.MeasureString("....", sysFont); //Spaces are not being picked up, let's just use .'s.
			m_StringFormat.SetTabStops(0f, new float[] { TabSize.Width });

			SizeF size = m_Graphics.MeasureString(text, sysFont, System.Drawing.Point.Empty, m_StringFormat);

			return new Size(Util.Ceil(size.Width), Util.Ceil(size.Height));
		}

		public override void RenderText(Font font, Point position, string text)
		{
			Flush();

			System.Drawing.Font sysFont = font.RendererData as System.Drawing.Font;

			if (sysFont == null || Math.Abs(font.RealSize - font.Size * Scale) > 2)
			{
				FreeFont(font);
				LoadFont(font);
				sysFont = font.RendererData as System.Drawing.Font;
			}

			var key = new Tuple<String, Font>(text, font);

			if (!m_StringCache.ContainsKey(key))
			{
				// not cached - create text renderer
				Size size = MeasureText(font, text);
				TextRenderer tr = new TextRenderer(size.Width, size.Height, this);
				tr.DrawString(text, sysFont, Brushes.White, Point.Zero, m_StringFormat); // renders string on the texture

				DrawTexturedRect(tr.Texture, new Rectangle(position.X, position.Y, tr.Texture.Width, tr.Texture.Height));

				m_StringCache[key] = tr;
			}
			else
			{
				TextRenderer tr = m_StringCache[key];
				DrawTexturedRect(tr.Texture, new Rectangle(position.X, position.Y, tr.Texture.Width, tr.Texture.Height));
			}
		}

		internal static void LoadTextureInternal(Texture t, Bitmap bmp)
		{
			// todo: convert to proper format
			PixelFormat lock_format = PixelFormat.Undefined;
			switch (bmp.PixelFormat)
			{
				case PixelFormat.Format32bppArgb:
					lock_format = PixelFormat.Format32bppArgb;
					break;
				case PixelFormat.Format24bppRgb:
					lock_format = PixelFormat.Format32bppArgb;
					break;
				default:
					t.Failed = true;
					return;
			}

			int glTex;

			// Create the opengl texture
			GL.GenTextures(1, out glTex);

			GL.BindTexture(TextureTarget.Texture2D, glTex);
			m_LastTextureID = glTex;

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			// Sort out our GWEN texture
			t.RendererData = glTex;
			t.Width = bmp.Width;
			t.Height = bmp.Height;

			BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, lock_format);

			switch (lock_format)
			{
				case PixelFormat.Format32bppArgb:
					GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, t.Width, t.Height, 0, global::OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
					break;
				default:
					// invalid
					break;
			}

			bmp.UnlockBits(data);
		}

		public override void LoadTexture(Texture t)
		{
			Bitmap bmp;
			try
			{
				bmp = new Bitmap(t.Name);
			}
			catch (Exception)
			{
				t.Failed = true;
				return;
			}

			LoadTextureInternal(t, bmp);
			bmp.Dispose();
		}

		public override void LoadTextureStream(Texture t, System.IO.Stream data)
		{
			Bitmap bmp;
			try
			{
				bmp = new Bitmap(data);
			}
			catch (Exception)
			{
				t.Failed = true;
				return;
			}

			LoadTextureInternal(t, bmp);
			bmp.Dispose();
		}

		public override void LoadTextureRaw(Texture t, byte[] pixelData)
		{
			Bitmap bmp;
			try
			{
				unsafe
				{
					fixed (byte* ptr = &pixelData[0])
						bmp = new Bitmap(t.Width, t.Height, 4 * t.Width, PixelFormat.Format32bppArgb, (IntPtr)ptr);
				}
			}
			catch (Exception)
			{
				t.Failed = true;
				return;
			}

			int glTex;

			// Create the opengl texture
			GL.GenTextures(1, out glTex);

			GL.BindTexture(TextureTarget.Texture2D, glTex);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			// Sort out our GWEN texture
			t.RendererData = glTex;

			var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
				PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, t.Width, t.Height, 0, global::OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);

			bmp.UnlockBits(data);
			bmp.Dispose();

			//[halfofastaple] Must rebind previous texture, to ensure creating a texture doesn't mess with the render flow.
			// Setting m_LastTextureID isn't working, for some reason (even if you always rebind the texture,
			// even if the previous one was the same), we are probably making draw calls where we shouldn't be?
			// Eventually the bug needs to be fixed (color picker in a window causes graphical errors), but for now,
			// this is fine.
			GL.BindTexture(TextureTarget.Texture2D, m_LastTextureID);
		}

		public override void FreeTexture(Texture t)
		{
			if (t.RendererData == null)
				return;
			int tex = (int)t.RendererData;
			if (tex == 0)
				return;
			GL.DeleteTextures(1, ref tex);
			t.RendererData = null;
		}

		public override unsafe Color PixelColor(Texture texture, uint x, uint y, Color defaultColor)
		{
			if (texture.RendererData == null)
				return defaultColor;

			int tex = (int)texture.RendererData;
			if (tex == 0)
				return defaultColor;

			Color pixel;

			GL.BindTexture(TextureTarget.Texture2D, tex);
			m_LastTextureID = tex;

			long offset = 4 * (x + y * texture.Width);
			byte[] data = new byte[4 * texture.Width * texture.Height];
			fixed (byte* ptr = &data[0])
			{
				GL.GetTexImage(TextureTarget.Texture2D, 0, global::OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)ptr);
				pixel = new Color(data[offset + 3], data[offset + 0], data[offset + 1], data[offset + 2]);
			}

			return pixel;
		}

		public abstract void Resize(int width, int height);
	}
}
