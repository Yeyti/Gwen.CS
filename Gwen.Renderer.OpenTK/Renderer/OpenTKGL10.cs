using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Gwen.Renderer.OpenTK
{
	public class OpenTKGL10 : OpenTKBase
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Vertex
		{
			public short x, y;
			public float u, v;
			public byte r, g, b, a;
		}

		private const int MaxVerts = 1024;
		private int m_VertNum;
		private readonly Vertex[] m_Vertices;
		private readonly int m_VertexSize;

		private bool m_WasBlendEnabled, m_WasTexture2DEnabled, m_WasDepthTestEnabled;
		private int m_PrevBlendSrc, m_PrevBlendDst, m_PrevAlphaFunc;
		private float m_PrevAlphaRef;
		private bool m_RestoreRenderState;

		public OpenTKGL10(bool restoreRenderState = true)
			: base()
		{
			m_Vertices = new Vertex[MaxVerts];
			m_VertexSize = Marshal.SizeOf(m_Vertices[0]);
			m_RestoreRenderState = restoreRenderState;
		}

		public override void Begin()
		{
			if (m_RestoreRenderState)
			{
				// Get previous parameter values before changing them.
				GL.GetInteger(GetPName.BlendSrc, out m_PrevBlendSrc);
				GL.GetInteger(GetPName.BlendDst, out m_PrevBlendDst);
				GL.GetInteger(GetPName.AlphaTestFunc, out m_PrevAlphaFunc);
				GL.GetFloat(GetPName.AlphaTestRef, out m_PrevAlphaRef);

				m_WasBlendEnabled = GL.IsEnabled(EnableCap.Blend);
				m_WasTexture2DEnabled = GL.IsEnabled(EnableCap.Texture2D);
				m_WasDepthTestEnabled = GL.IsEnabled(EnableCap.DepthTest);
			}

			// Set default values and enable/disable caps.
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.AlphaFunc(AlphaFunction.Greater, 1.0f);
			GL.Enable(EnableCap.Blend);
			GL.Disable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Texture2D);

			m_VertNum = 0;
			m_DrawCallCount = 0;
			m_ClipEnabled = false;
			m_TextureEnabled = false;
			m_LastTextureID = -1;

			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
		}

		public override void End()
		{
			Flush();

			if (m_RestoreRenderState)
			{
				GL.BindTexture(TextureTarget.Texture2D, 0);
				m_LastTextureID = 0;

				// Restore the previous parameter values.
				GL.BlendFunc((BlendingFactorSrc)m_PrevBlendSrc, (BlendingFactorDest)m_PrevBlendDst);
				GL.AlphaFunc((AlphaFunction)m_PrevAlphaFunc, m_PrevAlphaRef);

				if (!m_WasBlendEnabled)
					GL.Disable(EnableCap.Blend);

				if (m_WasTexture2DEnabled && !m_TextureEnabled)
					GL.Enable(EnableCap.Texture2D);

				if (m_WasDepthTestEnabled)
					GL.Enable(EnableCap.DepthTest);
			}

			GL.DisableClientState(ArrayCap.VertexArray);
			GL.DisableClientState(ArrayCap.ColorArray);
			GL.DisableClientState(ArrayCap.TextureCoordArray);
		}

		public override int VertexCount { get { return m_VertNum; } }

		protected override unsafe void Flush()
		{
			if (m_VertNum == 0) return;

			fixed (short* ptr1 = &m_Vertices[0].x)
			fixed (byte* ptr2 = &m_Vertices[0].r)
			fixed (float* ptr3 = &m_Vertices[0].u)
			{
				GL.VertexPointer(2, VertexPointerType.Short, m_VertexSize, (IntPtr)ptr1);
				GL.ColorPointer(4, ColorPointerType.UnsignedByte, m_VertexSize, (IntPtr)ptr2);
				GL.TexCoordPointer(2, TexCoordPointerType.Float, m_VertexSize, (IntPtr)ptr3);

				GL.DrawArrays(PrimitiveType.Quads, 0, m_VertNum);
			}

			m_DrawCallCount++;
			m_VertNum = 0;
		}

		protected override void DrawRect(Rectangle rect, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1)
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

			int vertexIndex = m_VertNum;
			m_Vertices[vertexIndex].x = (short)rect.X;
			m_Vertices[vertexIndex].y = (short)rect.Y;
			m_Vertices[vertexIndex].u = u1;
			m_Vertices[vertexIndex].v = v1;
			m_Vertices[vertexIndex].r = m_Color.R;
			m_Vertices[vertexIndex].g = m_Color.G;
			m_Vertices[vertexIndex].b = m_Color.B;
			m_Vertices[vertexIndex].a = m_Color.A;

			vertexIndex++;
			m_Vertices[vertexIndex].x = (short)(rect.X + rect.Width);
			m_Vertices[vertexIndex].y = (short)rect.Y;
			m_Vertices[vertexIndex].u = u2;
			m_Vertices[vertexIndex].v = v1;
			m_Vertices[vertexIndex].r = m_Color.R;
			m_Vertices[vertexIndex].g = m_Color.G;
			m_Vertices[vertexIndex].b = m_Color.B;
			m_Vertices[vertexIndex].a = m_Color.A;

			vertexIndex++;
			m_Vertices[vertexIndex].x = (short)(rect.X + rect.Width);
			m_Vertices[vertexIndex].y = (short)(rect.Y + rect.Height);
			m_Vertices[vertexIndex].u = u2;
			m_Vertices[vertexIndex].v = v2;
			m_Vertices[vertexIndex].r = m_Color.R;
			m_Vertices[vertexIndex].g = m_Color.G;
			m_Vertices[vertexIndex].b = m_Color.B;
			m_Vertices[vertexIndex].a = m_Color.A;

			vertexIndex++;
			m_Vertices[vertexIndex].x = (short)rect.X;
			m_Vertices[vertexIndex].y = (short)(rect.Y + rect.Height);
			m_Vertices[vertexIndex].u = u1;
			m_Vertices[vertexIndex].v = v2;
			m_Vertices[vertexIndex].r = m_Color.R;
			m_Vertices[vertexIndex].g = m_Color.G;
			m_Vertices[vertexIndex].b = m_Color.B;
			m_Vertices[vertexIndex].a = m_Color.A;

			m_VertNum += 4;
		}

		public override void Resize(int width, int height)
		{
			GL.Viewport(0, 0, width, height);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, width, height, 0, -1, 1);
		}
	}
}
