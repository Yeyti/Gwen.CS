using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Gwen.Renderer.OpenTK
{
	public class OpenTKGL40 : OpenTKBase
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Vertex
		{
			public float x, y;
			public float u, v;
			public float r, g, b, a;
		}

		private const int MaxVerts = 4096;
		private int m_VertNum;
		private readonly Vertex[] m_Vertices;
		private readonly int m_VertexSize;
		private int m_TotalVertNum;

		private bool m_WasBlendEnabled, m_WasDepthTestEnabled;
		private int m_PrevBlendSrc, m_PrevBlendDst, m_PrevAlphaFunc;
		private float m_PrevAlphaRef;
		private bool m_RestoreRenderState;

		private int m_Vbo, m_Vao;

		private GLShader40 m_GuiShader;

		public OpenTKGL40(bool restoreRenderState = true)
			: base()
		{
			m_Vertices = new Vertex[MaxVerts];
			m_VertexSize = Marshal.SizeOf(m_Vertices[0]);
			m_RestoreRenderState = restoreRenderState;

			CreateBuffers();

			m_GuiShader = new GLShader40();
			m_GuiShader.Load("gui");
		}

		private void CreateBuffers()
		{
			GL.GenVertexArrays(1, out m_Vao);
			GL.BindVertexArray(m_Vao);

			GL.GenBuffers(1, out m_Vbo);
			GL.BindBuffer(BufferTarget.ArrayBuffer, m_Vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(m_VertexSize * MaxVerts), IntPtr.Zero, BufferUsageHint.StreamDraw); // Allocate

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
			GL.BindVertexArray(0);
		}

		public override void Begin()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.UseProgram(m_GuiShader.Program);

			GL.BindVertexArray(m_Vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, m_Vbo);

			if (m_RestoreRenderState)
			{
				// Get previous parameter values before changing them.
				GL.GetInteger(GetPName.BlendSrc, out m_PrevBlendSrc);
				GL.GetInteger(GetPName.BlendDst, out m_PrevBlendDst);
				GL.GetInteger(GetPName.AlphaTestFunc, out m_PrevAlphaFunc);
				GL.GetFloat(GetPName.AlphaTestRef, out m_PrevAlphaRef);

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
			GL.BindVertexArray(0);
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
		}

		public override int VertexCount { get { return m_TotalVertNum; } }

		protected override void Flush()
		{
			if (m_VertNum == 0) return;

			if (GLVersion >= 43)
				GL.InvalidateBufferData(m_Vbo);
			//else
				// This will slow down rendering. It seems to work without this but there could be synchronization problems with some drivers.
				// If that's the case then enable this.
				//GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(m_VertexSize * MaxVerts), IntPtr.Zero, BufferUsageHint.StreamDraw);

			GL.BufferSubData<Vertex>(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(m_VertNum * m_VertexSize), m_Vertices);

			GL.Uniform1(m_GuiShader.Uniforms["uUseTexture"], m_TextureEnabled ? 1.0f : 0.0f);

			GL.DrawArrays(PrimitiveType.Triangles, 0, m_VertNum);

			m_DrawCallCount++;
			m_TotalVertNum += m_VertNum;
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

		public override void Resize(int width, int height)
		{
			GL.Viewport(0, 0, width, height);
			GL.UseProgram(m_GuiShader.Program);
			GL.Uniform2(m_GuiShader.Uniforms["uScreenSize"], (float)width, (float)height);
		}
	}
}
