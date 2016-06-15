using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Android.Graphics;
using OpenTK.Graphics.ES20;
using Android.App;

namespace Gwen.Renderer.Android.OpenTK
{
	public class OpenTK : RendererBase
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Vertex
		{
			public float x, y;
			public float u, v;
			public float r, g, b, a;
		}

		protected Color m_Color;

		private readonly Dictionary<Tuple<String, Font>, TextRenderer> m_StringCache;
		protected int m_DrawCallCount;
		protected bool m_ClipEnabled;
		protected bool m_TextureEnabled;
		static protected int m_LastTextureID;

		private const int MaxVerts = 4096;
		private int m_VertNum;
		private readonly Vertex[] m_Vertices;
		private readonly int m_VertexSize;
		private int m_TotalVertNum;

		private bool m_WasBlendEnabled, m_WasDepthTestEnabled;
		private int m_PrevBlendSrc, m_PrevBlendDst, m_PrevAlphaFunc;
		private float m_PrevAlphaRef;
		private bool m_RestoreRenderState;

		private int m_Vbo;

		GLShader m_GuiShader;

		public OpenTK(bool restoreRenderState = true)
            : base()
        {
			m_StringCache = new Dictionary<Tuple<String, Font>, TextRenderer>();

			m_Vertices = new Vertex[MaxVerts];
			m_VertexSize = Marshal.SizeOf(m_Vertices[0]);
			m_RestoreRenderState = restoreRenderState;

			CreateBuffers();

			m_GuiShader = new GLShader();
			m_GuiShader.Load();
		}

		public override void Dispose()
		{
			FlushTextCache();
			base.Dispose();
		}

		private void CreateBuffers()
		{
			GL.GenBuffers(1, out m_Vbo);
			GL.BindBuffer(BufferTarget.ArrayBuffer, m_Vbo);
			GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(m_VertexSize * MaxVerts), m_Vertices, BufferUsage.StreamDraw); // Allocate

			// Vertex positions
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, m_VertexSize, 0);

			// Tex coords
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, m_VertexSize, 2 * sizeof(float));

			// Colors
			GL.EnableVertexAttribArray(2);
			GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, m_VertexSize, 2 * (sizeof(float) + sizeof(float)));

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public override void Begin()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			m_GuiShader.Start();

			GL.BindBuffer(BufferTarget.ArrayBuffer, m_Vbo);

			if (m_RestoreRenderState)
			{
				// Get previous parameter values before changing them.
				GL.GetInteger(GetPName.BlendSrcRgb, out m_PrevBlendSrc);
				GL.GetInteger(GetPName.BlendDstRgb, out m_PrevBlendDst);
				GL.GetInteger(GetPName.BlendEquationAlpha, out m_PrevAlphaFunc);
				GL.GetFloat(GetPName.BlendSrcAlpha, out m_PrevAlphaRef);

				m_WasBlendEnabled = GL.IsEnabled(EnableCap.Blend);
				m_WasDepthTestEnabled = GL.IsEnabled(EnableCap.DepthTest);
			}

			// Set default values and enable/disable caps.
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Blend);
			GL.Disable(EnableCap.DepthTest);

			m_VertNum = 0;
			m_TotalVertNum = 0;
			m_DrawCallCount = 0;
			m_ClipEnabled = false;
			m_TextureEnabled = false;
			m_LastTextureID = -1;
		}

		public override void End()
		{
			Flush();

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			if (m_RestoreRenderState)
			{
				GL.BindTexture(TextureTarget.Texture2D, 0);
				m_LastTextureID = 0;

				// Restore the previous parameter values.
				GL.BlendFunc((BlendingFactorSrc)m_PrevBlendSrc, (BlendingFactorDest)m_PrevBlendDst);

				if (!m_WasBlendEnabled)
					GL.Disable(EnableCap.Blend);

				if (m_WasDepthTestEnabled)
					GL.Enable(EnableCap.DepthTest);
			}

			m_GuiShader.Stop();
		}

		protected void Flush()
		{
			if (m_VertNum == 0) return;

			GL.BufferSubData<Vertex>(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(m_VertNum * m_VertexSize), m_Vertices);

			GL.Uniform1(m_GuiShader.Uniforms["uUseTexture"], m_TextureEnabled ? 1.0f : 0.0f);

			GL.DrawArrays(BeginMode.Triangles, 0, m_VertNum);

			m_DrawCallCount++;
			m_TotalVertNum += m_VertNum;
			m_VertNum = 0;
		}

		/// <summary>
		/// Returns number of cached strings in the text cache.
		/// </summary>
		public int TextCacheSize { get { return m_StringCache.Count; } }

		public int DrawCallCount { get { return m_DrawCallCount; } }

		public int VertexCount { get { return m_TotalVertNum; } }

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

		protected void DrawRect(Rectangle rect, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1)
		{
			if (m_VertNum + 4 >= MaxVerts)
			{
				Flush();
			}

			if (m_ClipEnabled)
			{
				// cpu scissors test

				if (rect.Y < ClipRegion.Y)
				{
					int oldHeight = rect.Height;
					int delta = ClipRegion.Y - rect.Y;
					rect.Y = ClipRegion.Y;
					rect.Height -= delta;

					if (rect.Height <= 0)
					{
						return;
					}

					float dv = (float)delta / (float)oldHeight;

					v1 += dv * (v2 - v1);
				}

				if ((rect.Y + rect.Height) > (ClipRegion.Y + ClipRegion.Height))
				{
					int oldHeight = rect.Height;
					int delta = (rect.Y + rect.Height) - (ClipRegion.Y + ClipRegion.Height);

					rect.Height -= delta;

					if (rect.Height <= 0)
					{
						return;
					}

					float dv = (float)delta / (float)oldHeight;

					v2 -= dv * (v2 - v1);
				}

				if (rect.X < ClipRegion.X)
				{
					int oldWidth = rect.Width;
					int delta = ClipRegion.X - rect.X;
					rect.X = ClipRegion.X;
					rect.Width -= delta;

					if (rect.Width <= 0)
					{
						return;
					}

					float du = (float)delta / (float)oldWidth;

					u1 += du * (u2 - u1);
				}

				if ((rect.X + rect.Width) > (ClipRegion.X + ClipRegion.Width))
				{
					int oldWidth = rect.Width;
					int delta = (rect.X + rect.Width) - (ClipRegion.X + ClipRegion.Width);

					rect.Width -= delta;

					if (rect.Width <= 0)
					{
						return;
					}

					float du = (float)delta / (float)oldWidth;

					u2 -= du * (u2 - u1);
				}
			}

			float cR = m_Color.R / 255f;
			float cG = m_Color.G / 255f;
			float cB = m_Color.B / 255f;
			float cA = m_Color.A / 255f;

			int vertexIndex = m_VertNum;
			m_Vertices[vertexIndex].x = (short)rect.X;
			m_Vertices[vertexIndex].y = (short)rect.Y;
			m_Vertices[vertexIndex].u = u1;
			m_Vertices[vertexIndex].v = v1;
			m_Vertices[vertexIndex].r = cR;
			m_Vertices[vertexIndex].g = cG;
			m_Vertices[vertexIndex].b = cB;
			m_Vertices[vertexIndex].a = cA;

			vertexIndex++;
			m_Vertices[vertexIndex].x = (short)(rect.X + rect.Width);
			m_Vertices[vertexIndex].y = (short)rect.Y;
			m_Vertices[vertexIndex].u = u2;
			m_Vertices[vertexIndex].v = v1;
			m_Vertices[vertexIndex].r = cR;
			m_Vertices[vertexIndex].g = cG;
			m_Vertices[vertexIndex].b = cB;
			m_Vertices[vertexIndex].a = cA;

			vertexIndex++;
			m_Vertices[vertexIndex].x = (short)(rect.X + rect.Width);
			m_Vertices[vertexIndex].y = (short)(rect.Y + rect.Height);
			m_Vertices[vertexIndex].u = u2;
			m_Vertices[vertexIndex].v = v2;
			m_Vertices[vertexIndex].r = cR;
			m_Vertices[vertexIndex].g = cG;
			m_Vertices[vertexIndex].b = cB;
			m_Vertices[vertexIndex].a = cA;

			vertexIndex++;
			m_Vertices[vertexIndex].x = (short)rect.X;
			m_Vertices[vertexIndex].y = (short)rect.Y;
			m_Vertices[vertexIndex].u = u1;
			m_Vertices[vertexIndex].v = v1;
			m_Vertices[vertexIndex].r = cR;
			m_Vertices[vertexIndex].g = cG;
			m_Vertices[vertexIndex].b = cB;
			m_Vertices[vertexIndex].a = cA;

			vertexIndex++;
			m_Vertices[vertexIndex].x = (short)(rect.X + rect.Width);
			m_Vertices[vertexIndex].y = (short)(rect.Y + rect.Height);
			m_Vertices[vertexIndex].u = u2;
			m_Vertices[vertexIndex].v = v2;
			m_Vertices[vertexIndex].r = cR;
			m_Vertices[vertexIndex].g = cG;
			m_Vertices[vertexIndex].b = cB;
			m_Vertices[vertexIndex].a = cA;

			vertexIndex++;
			m_Vertices[vertexIndex].x = (short)rect.X;
			m_Vertices[vertexIndex].y = (short)(rect.Y + rect.Height);
			m_Vertices[vertexIndex].u = u1;
			m_Vertices[vertexIndex].v = v2;
			m_Vertices[vertexIndex].r = cR;
			m_Vertices[vertexIndex].g = cG;
			m_Vertices[vertexIndex].b = cB;
			m_Vertices[vertexIndex].a = cA;

			m_VertNum += 6;
		}

		public override bool LoadFont(Font font)
		{
			font.RealSize = font.Size * Scale;
			Paint paint = font.RendererData as Paint;

			if (paint != null)
				paint.Dispose();

			TypefaceStyle fontStyle = TypefaceStyle.Normal;
			if (font.Bold && font.Italic) fontStyle = TypefaceStyle.BoldItalic;
			if (font.Italic) fontStyle = TypefaceStyle.Italic;
			if (font.Bold) fontStyle = TypefaceStyle.Bold;

			PaintFlags paintFlags = PaintFlags.LinearText;
			if (font.Underline) paintFlags |= PaintFlags.UnderlineText;
			if (font.Strikeout) paintFlags |= PaintFlags.StrikeThruText;

			Typeface typeface = Typeface.Create(font.FaceName, fontStyle);
			
			paint = new Paint(paintFlags);
			paint.SetTypeface(typeface);
			paint.SetStyle(Paint.Style.Fill);
			paint.TextSize = font.RealSize;
			paint.TextAlign = Paint.Align.Left;
			paint.Color = global::Android.Graphics.Color.White;
			paint.AntiAlias = true;

			font.RendererData = paint;

			return true;
		}

		public override void FreeFont(Font font)
		{
			if (font.RendererData == null)
				return;

			Paint paint = font.RendererData as Paint;
			if (paint == null)
				throw new InvalidOperationException("Freeing empty font");

			paint.Dispose();
			font.RendererData = null;
		}

		public override FontMetrics GetFontMetrics(Font font)
		{
			Paint paint = font.RendererData as Paint;

			if (paint == null || Math.Abs(font.RealSize - font.Size * Scale) > 2)
			{
				FreeFont(font);
				LoadFont(font);
				paint = font.RendererData as Paint;
			}

			Paint.FontMetrics metrics = paint.GetFontMetrics();

			FontMetrics fm = new FontMetrics
			(
				-metrics.Top - metrics.Leading,
				-metrics.Ascent,
				metrics.Descent,
				-metrics.Top + metrics.Bottom,
				metrics.Leading,
				paint.FontSpacing,
				0
			);

			return fm;
		}

		public override Size MeasureText(Font font, string text)
		{
			Paint paint = font.RendererData as Paint;

			if (paint == null || Math.Abs(font.RealSize - font.Size * Scale) > 2)
			{
				FreeFont(font);
				LoadFont(font);
				paint = font.RendererData as Paint;
			}

			var key = new Tuple<String, Font>(text, font);

			if (m_StringCache.ContainsKey(key))
			{
				var tex = m_StringCache[key].Texture;
				return new Size(tex.Width, tex.Height);
			}

			text = text.Replace("\t", "    ");

			float width = paint.MeasureText(text);

			return new Size(Util.Ceil(width), Util.Ceil(paint.FontSpacing));
		}

		public override void RenderText(Font font, Point position, string text)
		{
			Flush();

			Paint paint = font.RendererData as Paint;

			if (paint == null || Math.Abs(font.RealSize - font.Size * Scale) > 2)
			{
				FreeFont(font);
				LoadFont(font);
				paint = font.RendererData as Paint;
			}

			var key = new Tuple<String, Font>(text, font);

			if (!m_StringCache.ContainsKey(key))
			{
				// not cached - create text renderer
				Size size = MeasureText(font, text);
				TextRenderer tr = new TextRenderer(size.Width, size.Height, this);
				tr.DrawString(text, paint, Point.Zero); // renders string on the texture

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

			IntPtr data = bmp.LockPixels();
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, t.Width, t.Height, 0, global::OpenTK.Graphics.ES20.PixelFormat.Rgba, PixelType.UnsignedByte, data);
            
			bmp.UnlockPixels();
		    bmp.Dispose();
		}

		public override void LoadTexture(Texture t)
		{
			Bitmap bmp;
			try
			{
				bmp = BitmapFactory.DecodeStream(Application.Context.Assets.Open(t.Name));
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
				bmp = BitmapFactory.DecodeStream(data);
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
			int glTex;

			// Create the opengl texture
			GL.GenTextures(1, out glTex);

			GL.BindTexture(TextureTarget.Texture2D, glTex);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			// Sort out our GWEN texture
			t.RendererData = glTex;

			unsafe
			{
				fixed (byte* ptr = &pixelData[0])
					GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, t.Width, t.Height, 0, global::OpenTK.Graphics.ES20.PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)ptr);
			}

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
			//GL.DeleteTextures(1, ref tex);
            GL.DeleteTexture(tex);
            t.RendererData = null;
		}

		public override unsafe Color PixelColor(Texture texture, uint x, uint y, Color defaultColor)
		{
			int tex = (int)texture.RendererData;
			if (tex == 0)
				return defaultColor;

			int fbo;
			GL.GenFramebuffers(1, out fbo);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.Texture2D, tex, 0);

			byte[] pixel = new byte[4];

			fixed (byte* ptr = &pixel[0])
				GL.ReadPixels((int)x, (int)y, 1, 1, global::OpenTK.Graphics.ES20.PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)ptr);

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.DeleteFramebuffers(1, ref fbo);

			return new Color(pixel[3], pixel[0], pixel[1], pixel[2]);
		}

		public void Resize(int width, int height)
		{
			GL.Viewport(0, 0, width, height);
			m_GuiShader.Start();
			GL.Uniform2(m_GuiShader.Uniforms["uScreenSize"], (float)width, (float)height);
			m_GuiShader.Stop();
		}
	}
}
