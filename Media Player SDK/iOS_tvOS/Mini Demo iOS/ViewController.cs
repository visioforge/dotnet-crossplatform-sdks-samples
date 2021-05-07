using System;
using UIKit;
using VisioForge.CrossPlatform.Core.Controls.iOS;
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;

namespace MiniDemoiOS
{
    public partial class ViewController : UIViewController
    {
        VideoView _videoView;

        bool _isPaused;

        VisioForge.CrossPlatform.Controls.MediaPlayer.MediaPlayer _mediaPlayer;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            _videoView = new VideoView();
            _videoView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            _videoView.Frame = new CoreGraphics.CGRect(0, 0, videoView.Frame.Width, videoView.Frame.Height);
            videoView.Add(_videoView);

            edURL.Text = "https://help.visioforge.com/video.mp4";

            _mediaPlayer = new VisioForge.CrossPlatform.Controls.MediaPlayer.MediaPlayer(_videoView);
            _mediaPlayer.OnError += _mediaPlayer_OnError;
            _mediaPlayer.OnPause += _mediaPlayer_OnPause;
            _mediaPlayer.OnPlaying += _mediaPlayer_OnPlaying;
            _mediaPlayer.OnMediaLoaded += _mediaPlayer_OnMediaLoaded;
            _mediaPlayer.OnPositionChange += _mediaPlayer_OnPositionChange;
            
            btPlay.TouchUpInside += btPlay_TouchUpInside;
            btStop.TouchUpInside += btStop_TouchUpInside;
            btPause.TouchUpInside += btPause_TouchUpInside;
            btOpen.TouchUpInside += btOpen_TouchUpInside;
            slPosition.ValueChanged += slPosition_ValueChanged;
        }

        private void slPosition_ValueChanged(object sender, EventArgs e)
        {
            _mediaPlayer.Position = TimeSpan.FromSeconds(slPosition.Value);
        }

        private void _mediaPlayer_OnPositionChange(object sender, VisioForge.CrossPlatform.Controls.PositionChangedEventArgs e)
        {
            InvokeOnMainThread(() => {
                slPosition.Value = (float)e.Position.TotalSeconds;
                lbPosition.Text = e.Position.ToString(@"hh\:mm\:ss");
            });            
        }

        private void _mediaPlayer_OnMediaLoaded(object sender, EventArgs e)
        {
            InvokeOnMainThread(() => { slPosition.MaxValue = (float)_mediaPlayer.Duration.TotalSeconds; });            
        }

        private void _mediaPlayer_OnPlaying(object sender, EventArgs e)
        {
            _isPaused = false;
        }

        private void _mediaPlayer_OnPause(object sender, EventArgs e)
        {
            _isPaused = true;
        }

        private void _mediaPlayer_OnError(object sender, VisioForge.CrossPlatform.Controls.Types.ErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }

        private async void btOpen_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return; 

                edURL.Text = fileData.FilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        private async void btPause_TouchUpInside(object sender, EventArgs e)
        {
            await _mediaPlayer.PauseAsync();
        }

        private async void btStop_TouchUpInside(object sender, EventArgs e)
        {
            await _mediaPlayer.StopAsync();

            _isPaused = false;
        }

        private async void btPlay_TouchUpInside(object sender, EventArgs e)
        {       
            if (_isPaused)
            {
                await _mediaPlayer.ResumeAsync();
            }
            else
            {
                await _mediaPlayer.PlayAsync(new Uri(edURL.Text));
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}