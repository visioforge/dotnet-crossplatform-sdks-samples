using Windows.UI.Xaml.Controls;

using VisioForge.CrossPlatform.Controls;
using VisioForge.CrossPlatform.Controls.Types;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MiniDemoUWP
{
    using System;

    using VisioForge.CrossPlatform.Controls.MediaPlayer;

    /// <summary>
    /// The main page of the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly VisioForge.CrossPlatform.Core.Controls.UWP.VideoView videoView = new VisioForge.CrossPlatform.Core.Controls.UWP.VideoView();

        public MediaPlayer Player { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="MainPage"/> class
        public MainPage()
        {
            InitializeComponent();

            videoView.HorizontalAlignment = HorizontalAlignment.Stretch;
            videoView.VerticalAlignment = VerticalAlignment.Stretch;
            VideoViewHost.Children.Add(videoView);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            edFilenameOrURL.Text = "http://help.visioforge.com/video.mp4";

            Player = new MediaPlayer(videoView);
            Player.OnPositionChange += Player_OnPositionChange;
            Player.OnStop += Player_OnStop;
            Player.OnMediaLoaded += Player_OnMediaLoaded;
            Player.OnError += Player_OnError;
        }

        private void AddLog(string text)
        {
            edLog.Text += text + Environment.NewLine;
        }

        private async void Player_OnError(object sender, ErrorEventArgs e)
        {
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    AddLog(e.Message);
                });
        }

        private async void Player_OnMediaLoaded(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    // file info
                    AddLog("Video streams:");
                    foreach (var track in Player.Video_Streams)
                    {
                        AddLog(track.ToString());
                    }

                    AddLog("Audio streams:");
                    foreach (var track in Player.Audio_Streams)
                    {
                        AddLog(track.ToString());
                    }

                    AddLog("Subtitle streams:");
                    foreach (var track in Player.Subtitle_Streams)
                    {
                        AddLog(track.ToString());
                    }
                });
        }

        private async void Player_OnStop(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                async () =>
                {
                    await Player.StopAsync();
                    tbTimeline.Value = 0;
                    lbTime.Text = "00:00:00/" + TimeSpan.FromSeconds(tbTimeline.Maximum).ToString(@"hh\:mm\:ss");
                });
        }

        private async void Player_OnPositionChange(object sender, PositionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    tbTimeline.Tag = 1;
                    tbTimeline.Maximum = (int)Player.Duration.TotalSeconds;

                    int value = (int)Player.Position.TotalSeconds;
                    if ((value > 0) && (value < tbTimeline.Maximum))
                    {
                        tbTimeline.Value = value;
                    }

                    tbTimeline.Tag = 0;

                    lbTime.Text = Player.Position.ToString(@"hh\:mm\:ss") + "/" + Player.Duration.ToString(@"hh\:mm\:ss");
                });
        }
        
        private async void btStart_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            edLog.Text = string.Empty;
            Player.Debug_Mode = cbDebug.IsChecked == true;

            await Player.PlayAsync(new Uri(edFilenameOrURL.Text));
        }

        private async void btResume_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Player.ResumeAsync();
        }

        private async void btPause_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Player.PauseAsync();
        }

        private async void btStop_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Player.StopAsync();
        }

        private async void btSelectFile_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".avi");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                edFilenameOrURL.Text = file.Path;
            }
            else
            {
                edFilenameOrURL.Text = string.Empty;
            }
        }
        private void tbTimeline_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (Convert.ToInt32(tbTimeline.Tag) == 0)
            {
                Player.Position = TimeSpan.FromSeconds(tbTimeline.Value);
            }
        }
    }
}
