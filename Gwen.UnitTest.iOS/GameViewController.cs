using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundation;
using GLKit;
using UIKit;
using CoreGraphics;
using ObjCRuntime;
using OpenGLES;
using OpenTK;
using OpenTK.Graphics.ES20;
using Gwen.Control;
using Gwen.Input;

namespace Gwen.UnitTest.iOS
{
	[Register("GameViewController")]
	[Adopts("UIKeyInput")]
	public class GameViewController : GLKViewController, IGLKViewDelegate
	{
		private Gwen.Renderer.iOS.OpenTK.OpenTK m_Renderer;
		private Gwen.Skin.SkinBase m_Skin;
		private Gwen.Control.Canvas m_Canvas;
		private Gwen.UnitTest.UnitTest m_UnitTest;

		const int FpsFrames = 50;
		private readonly List<long> m_Ftime;
		private readonly Stopwatch m_Stopwatch;
		private long m_LastTime;

		EAGLContext context { get; set; }

		[Export("initWithCoder:")]
		public GameViewController(NSCoder coder) : base(coder)
		{
			m_Ftime = new List<long>(FpsFrames);
			m_Stopwatch = new Stopwatch();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			context = new EAGLContext(EAGLRenderingAPI.OpenGLES2);

			if (context == null)
			{
				Debug.WriteLine("Failed to create ES context");
			}

			var view = (GLKView)View;
			view.Context = context;
			view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;
			view.UserInteractionEnabled = true;

			UILongPressGestureRecognizer gr = new UILongPressGestureRecognizer(() => { m_Canvas.Input_MouseButton(1, true); m_Canvas.Input_MouseButton(1, false); });
			gr.CancelsTouchesInView = false;
			view.AddGestureRecognizer(gr);

			SetupGL();
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
	
			m_Renderer.Resize((int)View.Bounds.Size.Width, (int)View.Bounds.Size.Height);
			m_Canvas.SetSize((int)View.Bounds.Size.Width, (int)View.Bounds.Size.Height);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			TearDownGL();

			if (EAGLContext.CurrentContext == context)
				EAGLContext.SetCurrentContext(null);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();

			if (IsViewLoaded && View.Window == null)
			{
				View = null;

				TearDownGL();

				if (EAGLContext.CurrentContext == context)
				{
					EAGLContext.SetCurrentContext(null);
				}
			}

			// Dispose of any resources that can be recreated.
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

		void SetupGL()
		{
			EAGLContext.SetCurrentContext(context);

			var view = (GLKView)View;

			Platform.Platform.Init(new Platform.iOS());

			m_Renderer = new Gwen.Renderer.iOS.OpenTK.OpenTK(false);
			m_Skin = new Gwen.Skin.TexturedBase(m_Renderer, "DefaultSkin.png");
			m_Skin.DefaultFont = new Font(m_Renderer, "Arial", 13);
			m_Canvas = new Canvas(m_Skin);
			m_Canvas.ShouldDrawBackground = true;
			m_Canvas.BackgroundColor = new Color(255, 150, 170, 170);

			m_Canvas.Scale = 1.5f;

			m_UnitTest = new Gwen.UnitTest.UnitTest(m_Canvas);

			m_Stopwatch.Restart();
			m_LastTime = 0;
		}

		void TearDownGL()
		{
			EAGLContext.SetCurrentContext(context);

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
		}

		public override void TouchesBegan(NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				CGPoint p = touch.LocationInView(touch.View);
				m_Canvas.Input_MouseMoved((int)p.X, (int)p.Y, 0, 0);
				m_Canvas.Input_MouseButton(0, true);
			}
		}

		public override void TouchesEnded(NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				m_Canvas.Input_MouseButton(0, false);

				// Cause mouse leave event
				m_Canvas.Input_MouseMoved(int.MaxValue, int.MaxValue, 0, 0);
			}

			ProcessKeyboard();
		}

		public override void TouchesMoved(NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				CGPoint p = touch.LocationInView(touch.View);

				m_Canvas.Input_MouseMoved((int)p.X, (int)p.Y, 0, 0);
			}
		}

		public override bool CanBecomeFirstResponder
		{
			get
			{
				return InputHandler.KeyboardFocus != null && InputHandler.KeyboardFocus.KeyboardNeeded && InputHandler.KeyboardFocus.IsVisible;
			}
		}

		[Export("hasText")]
		public bool HasText { get { return true; } }

		[Export("insertText:")]
		public void InsertText(String text)
		{
			char ch = text.Length > 0 ? text[0] : (char)0;
			Key key = Key.Invalid;

			switch (ch)
			{
				case '\n':
					key = Key.Return;
					break;
				case '\t':
					key = Key.Tab;
					break;
			}

			m_Canvas.Input_Key(key, true);
			if (ch != 0)
				m_Canvas.Input_Character(ch);
			m_Canvas.Input_Key(key, false);

			ProcessKeyboard();
		}

		[Export("deleteBackward")]
		public void DeleteBackward()
		{
			m_Canvas.Input_Key(Key.Backspace, true);
			m_Canvas.Input_Key(Key.Backspace, false);
		}

		private void ProcessKeyboard()
		{
			if (InputHandler.KeyboardFocus != null && InputHandler.KeyboardFocus.KeyboardNeeded && InputHandler.KeyboardFocus.IsVisible)
			{
				if (!IsFirstResponder)
					BecomeFirstResponder();
			}
			else
			{
				if (IsFirstResponder)
					ResignFirstResponder();
			}
		}

		public override void Update()
		{
		}
			
		void IGLKViewDelegate.DrawInRect(GLKView view, CoreGraphics.CGRect rect)
		{
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
		}
	}
}

