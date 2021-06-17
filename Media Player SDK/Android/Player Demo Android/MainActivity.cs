using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Globalization;
using System.Threading;
using VisioForge.CrossPlatform.Controls;
using VisioForge.CrossPlatform.Controls.Types.VideoProcessing;
using Xamarin.Essentials;

using Resource = PlayerDemoAndroid.Resource;

namespace PlayerDemoAndroid
{
    using LibVLCSharp.Shared;

    using VisioForge.CrossPlatform.Controls.MediaPlayer;

    [Activity(Label = "PlayerDemoAndroid", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private const string VIDEO_URL = "http://help.visioforge.com/video.mp4"; //"/storage/emulated/0/DCIM/!video.avi"; //"http://help.visioforge.com/video.mp4";

        private LibVLCSharp.Platforms.Android.VideoView videoView;

        private MediaPlayerControl mediaPlayer;

        private Button btOpenFile;

        private Button btStart;

        private Button btPause;

        private Button btStop;

        private Button btVideoEffects;

        private EditText edURL;

        private SeekBar sbTimeline;

        private TextView lbPosition;

        private GridLayout pnScreen;

        private bool isSeeking = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            videoView = FindViewById<LibVLCSharp.Platforms.Android.VideoView>(Resource.Id.videoView1);

            btOpenFile = FindViewById<Button>(Resource.Id.btOpenFile);
            btOpenFile.Click += btOpenFile_Click;

            btStart = FindViewById<Button>(Resource.Id.btStart);
            btStart.Click += btStart_Click;

            btPause = FindViewById<Button>(Resource.Id.btPause);
            btPause.Click += btPause_Click;

            btStop = FindViewById<Button>(Resource.Id.btStop);
            btStop.Click += btStop_Click;

            btVideoEffects = FindViewById<Button>(Resource.Id.btVideoEffects);
            btVideoEffects.Click += btVideoEffects_Click;

            sbTimeline = FindViewById<SeekBar>(Resource.Id.sbTimeline);
            sbTimeline.ProgressChanged += sbTimeline_ProgressChanged;
            sbTimeline.StartTrackingTouch += delegate (object sender, SeekBar.StartTrackingTouchEventArgs args)
            {
                isSeeking = true;
            };
            sbTimeline.StopTrackingTouch += delegate (object sender, SeekBar.StopTrackingTouchEventArgs args)
            {
                isSeeking = false;
            };

            lbPosition = FindViewById<TextView>(Resource.Id.lbPosition);

            pnScreen = FindViewById<GridLayout>(Resource.Id.pnScreen);

            edURL = FindViewById<EditText>(Resource.Id.edURL);
            edURL.Text = VIDEO_URL;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void sbTimeline_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (isSeeking)
            {
                mediaPlayer.Position = TimeSpan.FromMilliseconds(e.Progress);
            }
        }

        private async void btOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                Thread.Sleep(200);

                var file = await FilePicker.PickAsync();
                if (file == null)
                    return; // user canceled file picking

                RunOnUiThread(() =>
                {
                    edURL.Text = file.FullPath;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        private void btVideoEffects_Click(object sender, EventArgs e)
        {
            mediaPlayer.Video_Effects_Add(new GrayscaleVideoEffect(true));
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            await mediaPlayer?.StopAsync();
            mediaPlayer?.Dispose();
            mediaPlayer = null;

            // clear screen workaround
            pnScreen.RemoveView(videoView);
            pnScreen.AddView(videoView);
        }

        private async void btPause_Click(object sender, EventArgs e)
        {
            if (mediaPlayer == null)
            {
                return;
            }

            if (btPause.Text == "Pause")
            {
                await mediaPlayer.PauseAsync();
                btPause.Text = "Resume";
            }
            else
            {
                await mediaPlayer.ResumeAsync();
                btPause.Text = "Pause";
            }
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            isSeeking = false;

            mediaPlayer = new MediaPlayerControl(videoView)
            {
                //EnableHardwareDecoding = true
            };

            mediaPlayer.OnPositionChange += MediaPlayer_OnPositionChange;
            mediaPlayer.OnMediaLoaded += MediaPlayer_OnMediaLoaded;

            //var name = $"textlogo1";
            //var effect = new TextOverlay(name, "TEST", 20, 20);

            //mediaPlayer.Video_Overlays_Add(effect);

            await mediaPlayer.PlayAsync(new Uri(edURL.Text));
        }

        protected override void OnResume()
        {
            base.OnResume();

            Core.Initialize();
        }

        private void MediaPlayer_OnMediaLoaded(object sender, EventArgs e)
        {
            sbTimeline.Max = (int)mediaPlayer.Duration.TotalMilliseconds;

            videoView.TriggerLayoutChangeListener();
        }

        private void MediaPlayer_OnPositionChange(object sender, PositionChangedEventArgs e)
        {
            if (e == null || isSeeking)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("Timestamp: " + e.Position.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));

            var progress = (int)e.Position.TotalMilliseconds;
            if (progress > sbTimeline.Max)
            {
                sbTimeline.Progress = sbTimeline.Max;
            }
            else
            {
                sbTimeline.Progress = progress;
            }

            try
            {
                RunOnUiThread(() =>
                {
                    if (mediaPlayer == null)
                    {
                        return;
                    }

                    // This is where the received data is passed
                    lbPosition.Text = $"{e.Position.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)}/{mediaPlayer.Duration.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)}";
                });
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            mediaPlayer.StopAsync();
        }
    }
}
