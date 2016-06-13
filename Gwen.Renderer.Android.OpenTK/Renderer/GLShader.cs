using System;
using OpenTK.Graphics.ES20;
using System.Diagnostics;
using System.Collections.Generic;

namespace Gwen.Renderer.Android.OpenTK
{
	public class GLShader : IDisposable
	{
		private int m_Program;
		private int m_VertexShader;
		private int m_FragmentShader;

		private UniformDictionary m_Uniforms;
		public UniformDictionary Uniforms { get { return m_Uniforms; } }

		public GLShader()
		{
			m_Program = 0;
			m_VertexShader = 0;
			m_FragmentShader = 0;
		}

		public void Start()
		{
			GL.UseProgram(m_Program);
		}

		public void Stop()
		{
			GL.UseProgram(0);
		}

		public void Load()
		{
			string vSource = m_vShaderSource;
			string fSource = m_fShaderSource;

			int vShader = GL.CreateShader(ShaderType.VertexShader);
			int fShader = GL.CreateShader(ShaderType.FragmentShader);

			GL.ShaderSource(vShader, vSource);
			GL.ShaderSource(fShader, fSource);
			// Compile shaders
			GL.CompileShader(vShader);
			GL.CompileShader(fShader);
			Debug.WriteLine(GL.GetShaderInfoLog(vShader));
			Debug.WriteLine(GL.GetShaderInfoLog(fShader));

			int program = GL.CreateProgram();
			// Link and attach shaders to program
			GL.AttachShader(program, vShader);
			GL.AttachShader(program, fShader);

			GL.BindAttribLocation(program, 0, "in_screen_coords");
			GL.BindAttribLocation(program, 1, "in_uv");
			GL.BindAttribLocation(program, 2, "in_color");

			GL.LinkProgram(program);
			Debug.WriteLine(GL.GetProgramInfoLog(program));

			m_Program = program;
			m_VertexShader = vShader;
			m_FragmentShader = fShader;

			m_Uniforms = new UniformDictionary(m_Program);
		}

		public class UniformDictionary
		{
			private Dictionary<string, int> m_Data;
			private int m_Program;

			public UniformDictionary(int program)
			{
				m_Data = new Dictionary<string, int>();
				m_Program = program;
			}

			public int this[string key]
			{
				get
				{
					if (!m_Data.ContainsKey(key))
					{
						int uniformLocation = GL.GetUniformLocation(m_Program, key);
						m_Data.Add(key, uniformLocation);
					}

					int loc = -1;
					m_Data.TryGetValue(key, out loc);

					return loc;
				}
			}
		}

		public void Dispose()
		{
			GL.DeleteProgram(m_Program);
		}

		private const string m_vShaderSource = @"
#version 120

attribute vec2 in_screen_coords;
attribute vec2 in_uv;
attribute vec4 in_color;

varying vec2 frag_uv;
varying vec4 frag_color;

uniform vec2 uScreenSize;

void main(void)
{
	frag_uv = in_uv;
	frag_color = in_color;

	vec2 ndc_position = 2.0 * (in_screen_coords / uScreenSize) - 1.0;
	ndc_position.y *= -1.0;

	gl_Position = vec4(ndc_position, 0.0, 1);
}";

		private const string m_fShaderSource = @"
#version 120

varying vec2 frag_uv;
varying vec4 frag_color;

uniform sampler2D tex;

uniform float uUseTexture;

void main(void)
{
	vec4 texColor = texture2D(tex, frag_uv);
	if (uUseTexture > 0.0)
		gl_FragColor = texColor * frag_color;
	else
		gl_FragColor = frag_color;
}";

	}
}
