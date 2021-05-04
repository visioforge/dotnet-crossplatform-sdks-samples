// ReSharper disable StyleCop.SA1600
// ReSharper disable StyleCop.SA1300
namespace MainDemoUWP
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using VisioForge.CrossPlatform.Controls;
    using VisioForge.CrossPlatform.Controls.Types;

    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    using SkiaSharp;

    using MediaPlayer = VisioForge.CrossPlatform.Controls.MediaPlayer.MediaPlayer;

    /// <summary>
    /// The main page of the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private VisioForge.CrossPlatform.Controls.UWP.VideoView videoView = new VisioForge.CrossPlatform.Controls.UWP.VideoView();

        public MediaPlayer Player { get; private set; }

        public MediaInfoReader InfoReader { get; private set; }

        public InfoPage InfoPage { get; set; }

        public CropPage CropPage { get; set; }

        public AudioOutputPage AudioOutputPage { get; set; }

        public TextBox FilenameOrURL => edFilenameOrURL;

        public MainPage()
        {
            InitializeComponent();

            InfoPageFrame.Navigate(typeof(InfoPage), this);
            AudioOutputPageFrame.Navigate(typeof(AudioOutputPage), this);
            InfoPageFrame.Navigate(typeof(InfoPage), this);
            DisplayPageFrame.Navigate(typeof(DisplayPage), this);

            AdjustmentsPageFrame.Navigate(typeof(AdjustmentsPage), this);
            EffectsPageFrame.Navigate(typeof(EffectsPage), this);
            CropPageFrame.Navigate(typeof(CropPage), this);

            videoView.HorizontalAlignment = HorizontalAlignment.Stretch;
            videoView.VerticalAlignment = VerticalAlignment.Stretch;
            VideoViewHost.Children.Add(videoView);
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Player = new MediaPlayer(videoView);
            Player.OnPositionChange += Player_OnPositionChange;
            Player.OnStop += Player_OnStop;
            Player.OnMediaLoaded += Player_OnMediaLoaded;
            Player.OnError += Player_OnError;

            InfoReader = new MediaInfoReader();

            AudioOutputPage.AudioOutputDevice.Items?.Clear();
            foreach (var device in Player.Audio_OutputDevices)
            {
                AudioOutputPage.AudioOutputDevice.Items?.Add(device.Description);
            }

            if (AudioOutputPage.AudioOutputDevice.Items?.Count > 0)
            {
                AudioOutputPage.AudioOutputDevice.SelectedIndex = 0;
            }
        }

        private async void Player_OnError(object sender, VisioForge.CrossPlatform.Controls.Types.ErrorEventArgs e)
        {
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                    {
                        mmLog.Text += e.Message + Environment.NewLine;
                    });
        }

        private async void Player_OnMediaLoaded(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                    {
                        // fill audio and video streams
                        cbVideoStream.Items?.Clear();
                        for (int i = 0; i < Player.Video_StreamCount; i++)
                        {
                            cbVideoStream.Items?.Add(i.ToString());
                        }

                        if (cbVideoStream.Items?.Count > 0)
                        {
                            cbVideoStream.SelectedIndex = 0;
                        }

                        cbAudioStream.Items?.Clear();
                        for (int i = 0; i < Player.Audio_StreamCount; i++)
                        {
                            cbAudioStream.Items?.Add(i.ToString());
                        }

                        if (cbAudioStream.Items?.Count > 0)
                        {
                            cbAudioStream.SelectedIndex = 0;
                        }

                        // file info
                        InfoPage.Info.Items?.Clear();

                        foreach (var track in Player.Video_Streams)
                        {
                            InfoPage.Info.Items?.Add("Video stream: ");
                            InfoPage.Info.Items?.Add(track.ToString());
                        }

                        foreach (var track in Player.Audio_Streams)
                        {
                            InfoPage.Info.Items?.Add("Audio stream: ");
                            InfoPage.Info.Items?.Add(track.ToString());
                        }

                        foreach (var track in Player.Subtitle_Streams)
                        {
                            InfoPage.Info.Items?.Add("Subtitle stream: ");
                            InfoPage.Info.Items?.Add(track.ToString());
                        }

                        //if (player.Is360Video)
                        //{
                        //    videoViewFake.Show();
                        //}
                        //else
                        //{
                        //    videoViewFake.Hide();
                        //}
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
            Player.Debug_Mode = cbDebug.IsChecked == true;

            CropPage.ApplyCrop();

            if (AudioOutputPage.AudioOutputDevice.SelectedIndex != -1)
            {
                Player.Audio_OutputDevice = AudioOutputPage.AudioOutputDevice.Text;
            }

            Player.Mute = !AudioOutputPage.PlayAudio.IsChecked == true;

            //ApplyVideoAdjustments();
            //ApplyVideoDeinterlace();

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

        private void tbTimeline_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (Convert.ToInt32(tbTimeline.Tag) == 0)
            {
                Player.Position = TimeSpan.FromSeconds(tbTimeline.Value);
            }
        }

        private void btNextFrame_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Player.NextFrame();
        }

        private void tbSpeed_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Player.Speed = (float)tbSpeed.Value / 10.0f;
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

        private void btPrevFrame_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Player.PreviousFrame();
        }

        private void cbAudioStream_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Player.Audio_Stream = cbAudioStream.SelectedIndex;
        }

        private void cbVideoStream_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Player.Video_Stream = cbVideoStream.SelectedIndex;
        }

        private async void btSaveScreenshot_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("JPEG Image", new List<string>() { ".jpg" });
            savePicker.SuggestedFileName = "frame.jpg";

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                var image = await Player.TakeSnapshotAsync();

                var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);
                   
                using (var ms = new MemoryStream())
                {
                    data.SaveTo(ms);

                    await FileIO.WriteBytesAsync(file, ms.GetBuffer());
                }

                data.Dispose();
            }
        }

        //#region 360 degree

        //private bool videoViewMouseDown;

        //private Point videoViewMousePos;

        //private void videoViewFake_MouseDown(object sender, MouseEventArgs e)
        //{
        //    videoViewMouseDown = true;

        //    videoViewMousePos = e.Location;
        //}

        //private void videoViewFake_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (videoViewMouseDown)
        //    {
        //        var deltaX = e.X - videoViewMousePos.X;
        //        var deltaY = e.Y - videoViewMousePos.Y;

        //        videoViewMousePos = e.Location;

        //        var value = player.Video_Viewpoint_Get();
        //        player.Video_Viewpoint_Set(new D360Viewpoint(value.Yaw + deltaX, value.Pitch + deltaY, value.Roll, 80));
        //    }
        //}

        //private void videoViewFake_MouseUp(object sender, MouseEventArgs e)
        //{
        //    videoViewMouseDown = false;

        //    videoViewMousePos = new Point();
        //}

        //#endregion
    }
}
