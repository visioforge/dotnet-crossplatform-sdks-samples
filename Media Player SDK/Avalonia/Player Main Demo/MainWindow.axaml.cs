using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using VisioForge.CrossPlatform.Controls;
using VisioForge.CrossPlatform.Controls.Avalonia;
using VisioForge.CrossPlatform.Controls.MediaPlayer;
using VisioForge.CrossPlatform.Controls.Types;
using VisioForge.CrossPlatform.Controls.Types.VideoProcessing;

namespace Player_Main_Demo
{
    public class MainWindow : Window
    {
        private readonly VideoView videoView;

        private readonly TextBox edFilenameOrURL;

        private readonly Grid gridVideoView;

        private readonly Slider slSeeking;

        private readonly Label lbTimestamp;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            videoView = this.FindControl<VideoView>("videoView");
            edFilenameOrURL = this.FindControl<TextBox>("edFilenameOrURL");
            gridVideoView = this.FindControl<Grid>("gridVideoView");
            slSeeking = this.FindControl<Slider>("slSeeking");
            lbTimestamp = this.FindControl<Label>("lbTimestamp");
            
            _mediaPlayer = new MediaPlayer(videoView);
            _mediaPlayer.OnError += MediaPlayer_OnError;
            _mediaPlayer.OnMediaLengthChanged += MediaPlayer_OnMediaLengthChanged;
            _mediaPlayer.OnPositionChange += MediaPlayer_OnPositionChange;
        }

        private void MediaPlayer_OnPositionChange(object? sender, PositionChangedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                slSeeking.Value = e.Position.TotalSeconds;
                lbTimestamp.Content = $"{e.Position:hh\\:mm\\:ss} / {TimeSpan.FromSeconds(slSeeking.Maximum):hh\\:mm\\:ss}";
            });
        }

        private void MediaPlayer_OnMediaLengthChanged(object? sender, MediaLengthChangedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                slSeeking.Maximum = e.Length.TotalSeconds;
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private readonly MediaPlayer _mediaPlayer;

        private void MediaPlayer_OnError(object? sender, VisioForge.CrossPlatform.Controls.Types.ErrorEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync((Action)(() =>
            {
                Debug.WriteLine(e.Message);
            }));
        }

        private async void btPlay_Click(object sender, RoutedEventArgs e)
        {
            videoView.VerticalAlignment = VerticalAlignment.Stretch;
            videoView.HorizontalAlignment = HorizontalAlignment.Stretch;
            videoView.Width = gridVideoView.Width;
            videoView.Height = gridVideoView.Height;
            await _mediaPlayer.PlayAsync(new Uri(edFilenameOrURL.Text));
        }

        private async void btStop_Click(object sender, RoutedEventArgs e)
        {
            await _mediaPlayer.StopAsync();
        }

        private async void btPause_Click(object sender, RoutedEventArgs e)
        {
            await _mediaPlayer.PauseAsync();
        }
        
        private async void btResume_Click(object sender, RoutedEventArgs e)
        {
            await _mediaPlayer.ResumeAsync();
        }
        
        private async void btSelectFile_Click(object? sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            var res = await dlg.ShowAsync(this);

            if (res.Length > 0)
            {
                edFilenameOrURL.Text = res[0];
            }
        }

        private void SlSeeking_OnTapped(object? sender, RoutedEventArgs e)
        {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _mediaPlayer.Position = TimeSpan.FromSeconds(slSeeking.Value);
                });
        }
    }
}
