// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PlayerMainDemoMacOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton btStart { get; set; }

		[Outlet]
		AppKit.NSButton btStop { get; set; }

		[Outlet]
		AppKit.NSTextField edURL { get; set; }

		[Outlet]
		AppKit.NSTextField lbTimestamp { get; set; }

		[Outlet]
		AppKit.NSBox pnScreen { get; set; }

		[Outlet]
		AppKit.NSSlider slSeeking { get; set; }

		[Action ("btPause_CLick:")]
		partial void btPause_CLick (Foundation.NSObject sender);

		[Action ("btResume_Click:")]
		partial void btResume_Click (Foundation.NSObject sender);

		[Action ("btSelectFile_Click:")]
		partial void btSelectFile_Click (Foundation.NSObject sender);

		[Action ("btStart_CLick:")]
		partial void btStart_CLick (Foundation.NSObject sender);

		[Action ("btStop_Click:")]
		partial void btStop_Click (Foundation.NSObject sender);

		[Action ("slSlider_Changed:")]
		partial void slSlider_Changed (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btStart != null) {
				btStart.Dispose ();
				btStart = null;
			}

			if (btStop != null) {
				btStop.Dispose ();
				btStop = null;
			}

			if (edURL != null) {
				edURL.Dispose ();
				edURL = null;
			}

			if (lbTimestamp != null) {
				lbTimestamp.Dispose ();
				lbTimestamp = null;
			}

			if (pnScreen != null) {
				pnScreen.Dispose ();
				pnScreen = null;
			}

			if (slSeeking != null) {
				slSeeking.Dispose ();
				slSeeking = null;
			}
		}
	}
}
