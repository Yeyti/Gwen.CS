using System;
using Android.App;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Android.Content.PM;

namespace Gwen.UnitTest.Android
{
	[Activity(Label = "Gwen.net Unit Test",
		MainLauncher = true,
		Icon = "@drawable/icon",
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden
#if __ANDROID_11__
		,HardwareAccelerated=false
#endif
		)]
	public class MainActivity : Activity
	{
		UnitTestGameView m_View;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Enable to disable title bar
			//RequestWindowFeature(WindowFeatures.NoTitle);
			
			// Create our OpenGL view, and display it
			m_View = new UnitTestGameView(this);
			SetContentView(m_View);
		}

		protected override void OnPause()
		{
			base.OnPause();
			m_View.Pause();
		}

		protected override void OnResume()
		{
			base.OnResume();
			m_View.Resume();
		}

		public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
		{
			m_View.ProcessKeyDown(keyCode, e);
			return base.OnKeyDown(keyCode, e);
		}

		public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent e)
		{
			m_View.ProcessKeyUp(keyCode, e);
			return base.OnKeyUp(keyCode, e);
		}
	}
}

