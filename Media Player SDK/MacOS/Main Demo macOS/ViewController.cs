using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using VisioForge.CrossPlatform.Core.Controls.Mac;
using VisioForge.CrossPlatform.Core.Shared;
using Xamarin.Essentials;

namespace PlayerMainDemoMacOS
{
    public partial class ViewController : NSViewController
    {
        VideoView _videoView;

        VisioForge.CrossPlatform.Controls.MediaPlayer.MediaPlayer _mediaPlayer;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();            

            // Do any additional setup after loading the view.
            _videoView = new VideoView();
            _videoView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
            _videoView.Frame = new CoreGraphics.CGRect(0, 0, pnScreen.Frame.Width, pnScreen.Frame.Height);
            pnScreen.AddSubview(_videoView);

            edURL.StringValue = "https://help.visioforge.com/video.mp4";

            _mediaPlayer = new VisioForge.CrossPlatform.Controls.MediaPlayer.MediaPlayer(_videoView);
            _mediaPlayer.OnError += _mediaPlayer_OnError;
            _mediaPlayer.OnMediaLoaded += _mediaPlayer_OnMediaLoaded;
            _mediaPlayer.OnPositionChange += _mediaPlayer_OnPositionChange;

            slSeeking.FloatValue = 0;
            slSeeking.Continuous = true;
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        private void _mediaPlayer_OnPositionChange(object sender, VisioForge.CrossPlatform.Controls.PositionChangedEventArgs e)
        {
            InvokeOnMainThread(() => {
                slSeeking.FloatValue  = (float)e.Position.TotalSeconds;
                lbTimestamp.StringValue = e.Position.ToString(@"hh\:mm\:ss") + " / " + _mediaPlayer.Duration.ToString(@"hh\:mm\:ss");
            });
        }

        private void _mediaPlayer_OnMediaLoaded(object sender, EventArgs e)
        {
            InvokeOnMainThread(() => { slSeeking.MaxValue = (float)_mediaPlayer.Duration.TotalSeconds; });
        }

        private void _mediaPlayer_OnError(object sender, VisioForge.CrossPlatform.Controls.Types.ErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }

        partial void btPause_CLick(Foundation.NSObject sender)
        {
            _mediaPlayer.PauseAsync();
        }

        partial void btResume_Click(Foundation.NSObject sender)
        {
            _mediaPlayer.ResumeAsync();
        }

        partial void btStart_CLick(Foundation.NSObject sender)
        {
            _mediaPlayer.PlayAsync(new Uri(edURL.StringValue));
        }

        partial void btStop_Click(Foundation.NSObject sender)
        {
            _mediaPlayer.StopAsync();
        }

        partial void slSlider_Changed(Foundation.NSObject sender)
        {
            _mediaPlayer.Position = TimeSpan.FromSeconds(slSeeking.FloatValue);
        }

        partial void btSelectFile_Click(Foundation.NSObject sender)
        {
            try
            {
                var dlg = NSOpenPanel.OpenPanel;
                dlg.CanChooseFiles = true;
                dlg.CanChooseDirectories = false;
                
                if (dlg.RunModal() == 1)
                {
                    // Nab the first file
                    var url = dlg.Urls[0];

                    if (url != null)
                    {
                        var path = url.Path;
                        edURL.StringValue = path;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }
    }
}
