using System;
using Gwen.Control;
using OpenTK.Platform.Android;
using Android.Views;

namespace Gwen.Renderer.Android.OpenTK.Input
{
	public class OpenTK
	{
		private Canvas m_Canvas = null;

		private int m_MouseX = 0;
		private int m_MouseY = 0;

		private int m_DeadKey = 0;

		public OpenTK(AndroidGameView window)
		{
		}

		public void Initialize(Canvas c)
		{
			m_Canvas = c;
		}

		/// <summary>
		/// Translates control key's Android key code to GWEN's code.
		/// </summary>
		/// <param name="key">Android key code.</param>
		/// <returns>GWEN key code.</returns>
		private Key TranslateKeyCode(Keycode keyCode)
		{
			switch (keyCode)
			{
				case Keycode.Del: return Key.Backspace;
				case Keycode.Enter: return Key.Return;
				case Keycode.Escape: return Key.Escape;
				case Keycode.Tab: return Key.Tab;
				case Keycode.Space: return Key.Space;
				case Keycode.DpadUp: return Key.Up;
				case Keycode.DpadDown: return Key.Down;
				case Keycode.DpadLeft: return Key.Left;
				case Keycode.DpadRight: return Key.Right;
				case Keycode.MoveHome: return Key.Home;
				case Keycode.MoveEnd: return Key.End;
			}

			return Key.Invalid;
		}

		public bool ProcessMotionEvent(MotionEvent me)
		{
			MotionEventActions actions = me.Action;

			int x = (int)me.GetX();
			int y = (int)me.GetY();

			int dx = x - m_MouseX;
			int dy = y - m_MouseY;

			m_MouseX = x;
			m_MouseY = y;

			if (actions == MotionEventActions.Down)
			{
				// Cause mouse hover event
				m_Canvas.Input_MouseMoved(x, y, dx, dy);

				return m_Canvas.Input_MouseButton(0, true);
			}
			else if (actions == MotionEventActions.Up)
			{
				bool result = m_Canvas.Input_MouseButton(0, false);

				// Cause mouse leave event
				m_Canvas.Input_MouseMoved(int.MaxValue, int.MaxValue, dx, dy);

				return result;
			}
			else if (actions == MotionEventActions.Move)
			{
				return m_Canvas.Input_MouseMoved(x, y, dx, dy);
			}

			return false;
		}

		public bool ProcessLongPress()
		{
			return m_Canvas.Input_MouseButton(1, true) || m_Canvas.Input_MouseButton(1, false);
		}

		public bool ProcessKeyDown(Keycode keyCode, KeyEvent e)
		{
			Key iKey = TranslateKeyCode(keyCode);

			return m_Canvas.Input_Key(iKey, true);
		}

		public bool ProcessKeyUp(Keycode keyCode, KeyEvent e)
		{
			Key iKey = TranslateKeyCode(keyCode);

			if (iKey == Key.Invalid)
			{
				int ch = e.UnicodeChar;
				if (ch != 0)
				{
					if ((ch & KeyCharacterMap.CombiningAccent) != 0)
					{
						m_DeadKey = ch & KeyCharacterMap.CombiningAccentMask;
					}
					else
					{
						if (m_DeadKey != 0)
						{
							ch = KeyCharacterMap.GetDeadChar(m_DeadKey, ch);
							m_DeadKey = 0;
						}

						m_Canvas.Input_Character((char)ch);
					}
				}
			}

			return m_Canvas.Input_Key(iKey, false);
		}
	}
}
