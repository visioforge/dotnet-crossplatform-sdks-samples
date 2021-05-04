﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisioForge.CrossPlatform.Controls.Types;
using VisioForge.CrossPlatform.Controls.XamarinForms.Shared;
using Xamarin.Essentials;
using Xamarin.Forms;
using PositionChangedEventArgs = VisioForge.CrossPlatform.Controls.PositionChangedEventArgs;

namespace MediaPlayer.MiniDemo.XF
{
    public partial class MainPage : ContentPage
    {
        private VisioForge.CrossPlatform.Controls.MediaPlayer.MediaPlayer _mediaPlayer;
        
        private string _filename = "http://help.visioforge.com/video.mp4";

        private bool IsVideoViewInitialized;

        public MainPage()
        {
            InitializeComponent();

            CreatePlayer();
        }

        private void CreatePlayer()
        {
            if (_mediaPlayer != null)
            {
                DestroyPlayer();
            }

            _mediaPlayer = new VisioForge.CrossPlatform.Controls.MediaPlayer.MediaPlayer(null);
            _mediaPlayer.OnMediaLengthChanged += MediaPlayerOnOnMediaLengthChanged;
            _mediaPlayer.OnPositionChange += MediaPlayerOnOnPositionChange;
        }

        private void MediaPlayerOnOnPositionChange(object sender, PositionChangedEventArgs e)
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                slPosition.Value = e.Position.TotalSeconds;
                lbTimestamp.Text = $"{e.Position:hh\\:mm\\:ss} / {TimeSpan.FromSeconds(slPosition.Maximum):hh\\:mm\\:ss}";
            });
        }

        private void MediaPlayerOnOnMediaLengthChanged(object sender, MediaLengthChangedEventArgs e)
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                slPosition.Maximum = e.Length.TotalSeconds;
            });
        }

        private async void DestroyPlayer()
        {
            if (_mediaPlayer == null)
            {
                return;
            }

            await _mediaPlayer.StopAsync();
            _mediaPlayer.OnMediaLengthChanged -= MediaPlayerOnOnMediaLengthChanged;
            _mediaPlayer.OnPositionChange -= MediaPlayerOnOnPositionChange;
            _mediaPlayer.Dispose();
            _mediaPlayer = null;
        }

        private async void btPlay_OnClicked(object sender, EventArgs e)
        {
            if (IsVideoViewInitialized)
            {
                DestroyPlayer();
                CreatePlayer();

                foreach (var view in MainGrid.Children)
                {
                    if (view.GetType() == typeof(VideoView))
                    {
                        videoView = (VideoView)view;
                    }
                }

                _mediaPlayer.UpdateView(videoView);
                await _mediaPlayer.PlayAsync(new Uri(_filename));
            }
        }

        private async void btPaused_OnClicked(object sender, EventArgs e)
        {
            await _mediaPlayer.PauseAsync();
        }

        private async void btResume_OnClicked(object sender, EventArgs e)
        {
            await _mediaPlayer.ResumeAsync();
        }

        private async void btStop_OnClicked(object sender, EventArgs e)
        {
            await _mediaPlayer.StopAsync();
        }

        private void slPosition_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            //if (Math.Abs(e.NewValue - e.OldValue) > 3)
            //{
            //    Dispatcher.BeginInvokeOnMainThread(() =>
            //    {
            //        _mediaPlayer.Position = TimeSpan.FromSeconds(slPosition.Value);
            //    });
            //}
        }

        private async void btSelectFile_OnClicked(object sender, EventArgs e)
        {
            DestroyPlayer();

            var result = await FilePicker.PickAsync();
            if (result != null)
            {
                _filename = result.FullPath;
                edFilename.Text = _filename;
            }
        }

        private void MainPage_OnAppearing(object sender, EventArgs e)
        {
            _mediaPlayer.UpdateView(videoView);
            IsVideoViewInitialized = true;

            edFilename.Text = _filename;
        }

        private void SlPosition_OnDragCompleted(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                _mediaPlayer.Position = TimeSpan.FromSeconds(slPosition.Value);
            });
        }

        private void SlPosition_OnDragStarted(object sender, EventArgs e)
        {
            
        }
    }
}