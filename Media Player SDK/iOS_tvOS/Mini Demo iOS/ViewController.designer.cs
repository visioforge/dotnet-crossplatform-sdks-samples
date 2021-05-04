// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MiniDemoiOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton btOpen { get; set; }

		[Outlet]
		UIKit.UIButton btPause { get; set; }

		[Outlet]
		UIKit.UIButton btPlay { get; set; }

		[Outlet]
		UIKit.UIButton btStop { get; set; }

		[Outlet]
		UIKit.UITextField edURL { get; set; }

		[Outlet]
		UIKit.UILabel lbPosition { get; set; }

		[Outlet]
		UIKit.UISlider slPosition { get; set; }

		[Outlet]
		UIKit.UIView videoView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btOpen != null) {
				btOpen.Dispose ();
				btOpen = null;
			}

			if (btPause != null) {
				btPause.Dispose ();
				btPause = null;
			}

			if (btPlay != null) {
				btPlay.Dispose ();
				btPlay = null;
			}

			if (btStop != null) {
				btStop.Dispose ();
				btStop = null;
			}

			if (edURL != null) {
				edURL.Dispose ();
				edURL = null;
			}

			if (lbPosition != null) {
				lbPosition.Dispose ();
				lbPosition = null;
			}

			if (slPosition != null) {
				slPosition.Dispose ();
				slPosition = null;
			}

			if (videoView != null) {
				videoView.Dispose ();
				videoView = null;
			}
		}
	}
}
