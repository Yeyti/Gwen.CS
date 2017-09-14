using System;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Collections.Generic;

namespace Gwen.Renderer.OpenTK
{
	public class GLShader20 : IDisposable
	{
		public int Program { get; set; }
		public int VertexShader { get; set; }
		public int FragmentShader { get; set; }

		private UniformDictionary _uniforms;
		public UniformDictionary Uniforms { get { return _uniforms; } set { return; } }

		public GLShader20()
		{
			this.Program = 0;
			this.VertexShader = 0;
			this.FragmentShader = 0;
		}

		public void Load(string shaderName)
		{
			Load(shaderName, shaderName);
		}

		public void Apply()
		{
			GL.UseProgram(this.Program);
		}

		public void Load(string vertexShaderName, string fragmentShaderName)
		{
			string vSource = vShaderSource;
			string fSource = fShaderSource;

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

			this.Program = program;
			this.VertexShader = vShader;
			this.FragmentShader = fShader;
			this._uniforms = new UniformDictionary(Program);
		}

		public class UniformDictionary
		{
			private Dictionary<string, int> _data;
			private int _program;

			public UniformDictionary(int program)
			{
				_data = new Dictionary<string, int>();
				_program = program;
			}

			public int this[string key]
			{
				get
				{
					if (!this._data.ContainsKey(key))
					{
						int uniformLocation = GL.GetUniformLocation(_program, key);
						this._data.Add(key, uniformLocation);
					}

					int loc = -1;
					this._data.TryGetValue(key, out loc);

					return loc;
				}
			}
		}

		public void Dispose()
		{
			GL.DeleteProgram(this.Program);
		}

		private const string vShaderSource = @"
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

		private const string fShaderSource = @"
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
