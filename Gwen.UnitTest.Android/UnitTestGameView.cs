using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.Views.InputMethods;
using Android.App;
using Gwen.Control;
using Gwen.Input;

namespace Gwen.UnitTest.Android
{
	class UnitTestGameView : AndroidGameView
	{
		private Gwen.Renderer.Android.OpenTK.Input.OpenTK m_Input;
		private Gwen.Renderer.Android.OpenTK.OpenTK m_Renderer;
		private Gwen.Skin.SkinBase m_Skin;
		private Gwen.Control.Canvas m_Canvas;
		private Gwen.UnitTest.UnitTest m_UnitTest;

		const int FpsFrames = 50;
		private readonly List<long> m_Ftime;
		private readonly Stopwatch m_Stopwatch;
		private long m_LastTime;

		public UnitTestGameView(Context context) : base(context)
		{
			m_Ftime = new List<long>(FpsFrames);
			m_Stopwatch = new Stopwatch();

			Touch += OnTouch;
			LongClick += OnLongClick;
		}

		// This gets called when the drawing surface is ready
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Platform.Platform.Init(new Platform.Android());

			m_Renderer = new Gwen.Renderer.Android.OpenTK.OpenTK(false);
			m_Skin = new Gwen.Skin.TexturedBase(m_Renderer, "DefaultSkin.png");
			m_Skin.DefaultFont = new Font(m_Renderer, "Arial", 11);
			m_Canvas = new Canvas(m_Skin);
			m_Input = new Gwen.Renderer.Android.OpenTK.Input.OpenTK(this);
			m_Input.Initialize(m_Canvas);

			m_Canvas.SetSize(Width, Height);
			m_Canvas.ShouldDrawBackground = true;
			m_Canvas.BackgroundColor = new Color(255, 150, 170, 170);

			m_UnitTest = new Gwen.UnitTest.UnitTest(m_Canvas);

			m_Stopwatch.Restart();
			m_LastTime = 0;

			// Run the render loop
			Run();
		}

		protected override void Dispose(bool disposing)
		{
			if (m_Canvas != null)
			{
				m_Canvas.Dispose();
				m_Canvas = null;
			}
			if (m_Skin != null)
			{
				m_Skin.Dispose();
				m_Skin = null;
			}
			if (m_Renderer != null)
			{
				m_Renderer.Dispose();
				m_Renderer = null;
			}

			base.Dispose();
		}

		// This method is called everytime the context needs
		// to be recreated. Use it to set any egl-specific settings
		// prior to context creation
		//
		// In this particular case, we demonstrate how to set
		// the graphics mode and fallback in case the device doesn't
		// support the defaults
		protected override void CreateFrameBuffer()
		{
			ContextRenderingApi = GLVersion.ES2;

			// the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
			try
			{
				Log.Verbose("GLCube", "Loading with default settings");

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer();
				return;
			}
			catch (Exception ex)
			{
				Log.Verbose("GLCube", "{0}", ex);
			}

			throw new Exception("Can't load egl, aborting");
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (m_Renderer != null)
			{
				m_Renderer.Resize(Width, Height);
				m_Canvas.SetSize(Width, Height);
			}
		}

		private void OnTouch(object sender, TouchEventArgs e)
		{
			m_Input.ProcessMotionEvent(e.Event);

			CheckKeyboardNeed();

			e.Handled = false;
		}

		private void OnLongClick(object sender, LongClickEventArgs e)
		{
			m_Input.ProcessLongPress();
		}

		public bool ProcessKeyDown(Keycode keyCode, KeyEvent e)
		{
			return m_Input.ProcessKeyDown(keyCode, e);
		}

		public bool ProcessKeyUp(Keycode keyCode, KeyEvent e)
		{
			bool result = m_Input.ProcessKeyUp(keyCode, e);

			CheckKeyboardNeed();

			return result;
		}

		private void CheckKeyboardNeed()
		{
			InputMethodManager inputMethodManager = Application.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;

			if (InputHandler.KeyboardFocus != null && InputHandler.KeyboardFocus.KeyboardNeeded && InputHandler.KeyboardFocus.IsVisible && !inputMethodManager.IsAcceptingText)
			{
				inputMethodManager.ShowSoftInput(this, ShowFlags.Forced);
				inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
			}
			else if (inputMethodManager.IsActive)
			{
				inputMethodManager.HideSoftInputFromWindow(WindowToken, HideSoftInputFlags.None);
			}
		}

		// This gets called on each frame render
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			if (m_Ftime.Count == FpsFrames)
				m_Ftime.RemoveAt(0);

			m_Ftime.Add(m_Stopwatch.ElapsedMilliseconds - m_LastTime);
			m_LastTime = m_Stopwatch.ElapsedMilliseconds;

			if (m_Stopwatch.ElapsedMilliseconds > 1000)
			{
				m_UnitTest.Note = String.Format("String Cache size: {0} Draw Calls: {1} Vertex Count: {2}", m_Renderer.TextCacheSize, m_Renderer.DrawCallCount, m_Renderer.VertexCount);
				m_UnitTest.Fps = 1000f * m_Ftime.Count / m_Ftime.Sum();

				m_Stopwatch.Restart();

				if (m_Renderer.TextCacheSize > 1000) // each cached string is an allocated texture, flush the cache once in a while in your real project
					m_Renderer.FlushTextCache();
			}

			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

			m_Canvas.RenderCanvas();

			SwapBuffers();
		}
	}
}
