using System.Windows;
// ReSharper disable StyleCop.SA1600
// ReSharper disable StyleCop.SA1300

namespace MainDemoWPF
{
    using System;
    using System.IO;
    using System.Windows.Input;

    using Microsoft.Win32;

    using VisioForge.CrossPlatform.Controls;
    using VisioForge.CrossPlatform.Controls.MediaPlayer;
    using VisioForge.CrossPlatform.Controls.Types;
    using VisioForge.CrossPlatform.Controls.Types.VideoProcessing;
    using VisioForge.CrossPlatform.Controls.WinForms.Dialogs;

    using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

    public partial class MainWindow : Window
    {
        private readonly MediaPlayer player;

        private readonly MediaInfoReader infoReader;

        private readonly OpenFileDialog openFileDialog = new OpenFileDialog();

        public MainWindow()
        {
            InitializeComponent();

            player = new MediaPlayer(VideoView);
            player.OnPositionChange += Player_OnPositionChange;
            player.OnStop += Player_OnStop;
            player.OnMediaLoaded += Player_OnMediaLoaded;
            player.OnError += Player_OnError;

            infoReader = new MediaInfoReader();
        }

        private void Player_OnError(object sender, VisioForge.CrossPlatform.Controls.Types.ErrorEventArgs e)
        {
            Dispatcher?.BeginInvoke((Action)(() =>
                                                    {
                                                        mmLog.Text += e.Message + Environment.NewLine;
                                                    }));
        }

        private void Player_OnMediaLoaded(object sender, EventArgs e)
        {
            Dispatcher?.BeginInvoke((Action)(() =>
                                                    {
                                                        // fill audio and video streams
                                                        cbVideoStream.Items.Clear();
                                                        for (int i = 0; i < player.Video_StreamCount; i++)
                                                        {
                                                            cbVideoStream.Items.Add(i.ToString());
                                                        }

                                                        if (cbVideoStream.Items.Count > 0)
                                                        {
                                                            cbVideoStream.SelectedIndex = 0;
                                                        }

                                                        cbAudioStream.Items.Clear();
                                                        for (int i = 0; i < player.Audio_StreamCount; i++)
                                                        {
                                                            cbAudioStream.Items.Add(i.ToString());
                                                        }

                                                        if (cbAudioStream.Items.Count > 0)
                                                        {
                                                            cbAudioStream.SelectedIndex = 0;
                                                        }

                                                        // file info
                                                        mmInfo.Text = string.Empty;

                                                        foreach (var track in player.Video_Streams)
                                                        {
                                                            mmInfo.Text += "Video stream: " + Environment.NewLine;
                                                            mmInfo.Text += track.ToString();

                                                            mmInfo.Text += Environment.NewLine;
                                                        }

                                                        foreach (var track in player.Audio_Streams)
                                                        {
                                                            mmInfo.Text += "Audio stream: " + Environment.NewLine;
                                                            mmInfo.Text += track.ToString();

                                                            mmInfo.Text += Environment.NewLine;
                                                        }

                                                        foreach (var track in player.Subtitle_Streams)
                                                        {
                                                            mmInfo.Text += "Subtitle stream: " + Environment.NewLine;
                                                            mmInfo.Text += track.ToString();

                                                            mmInfo.Text += Environment.NewLine;
                                                        }

                                                        //if (player.Is360Video)
                                                        //{
                                                        //    videoViewFake.Show();
                                                        //}
                                                        //else
                                                        //{
                                                        //    videoViewFake.Hide();
                                                        //}
                                                    }));
        }

        private void Player_OnStop(object sender, EventArgs e)
        {
            Dispatcher?.BeginInvoke((Action)(async () =>
                                                    {
                                                        await player.StopAsync();
                                                        tbTimeline.Value = 0;
                                                        lbTime.Content = "00:00:00/" + TimeSpan.FromSeconds(tbTimeline.Maximum).ToString(@"hh\:mm\:ss");
                                                    }));
        }

        private void Player_OnPositionChange(object sender, PositionChangedEventArgs e)
        {
            Dispatcher?.BeginInvoke((Action)(() =>
                                                    {
                                                        tbTimeline.Tag = 1;
                                                        tbTimeline.Maximum = (int)(player.Duration.TotalMilliseconds / 1000);

                                                        int value = (int)(player.Position.TotalMilliseconds / 1000);
                                                        if ((value > 0) && (value < tbTimeline.Maximum))
                                                        {
                                                            tbTimeline.Value = value;
                                                        }

                                                        tbTimeline.Tag = 0;
                                                        lbTime.Content = player.Position.ToString(@"hh\:mm\:ss") + "/" + player.Duration.ToString(@"hh\:mm\:ss");
                                                    }));
        }

        private async void btStart_Click(object sender, RoutedEventArgs e)
        {
            ApplyCrop();

            if (cbAudioOutputDevice.SelectedIndex != -1)
            {
                player.Audio_OutputDevice = cbAudioOutputDevice.Text;
            }

            player.Mute = !cbPlayAudio.IsChecked == true;

            ApplyVideoAdjustments();
            ApplyVideoDeinterlace();

            await player.PlayAsync(new Uri(edFilenameOrURL.Text));
        }

        private async void btResume_Click(object sender, RoutedEventArgs e)
        {
            await player.ResumeAsync();
        }

        private async void btPause_Click(object sender, RoutedEventArgs e)
        {
            await player.PauseAsync();
        }

        private async void btStop_Click(object sender, RoutedEventArgs e)
        {
            await player.StopAsync();
        }

        private void tbTimeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null)
            {
                return;
            }

            if (Convert.ToInt32(tbTimeline.Tag) == 0)
            {
                player.Position = new TimeSpan((long)tbTimeline.Value * 1000 * TimeSpan.TicksPerMillisecond);
            }
        }

        private void btNextFrame_Click(object sender, RoutedEventArgs e)
        {
            player.NextFrame();
        }

        private void tbSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null)
            {
                return;
            }

            player.Speed = (long)tbSpeed.Value / 10.0f;
        }

        private void btPrevFrame_Click(object sender, RoutedEventArgs e)
        {
            player.PreviousFrame();
        }

        private void cbAudioStream_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            player.Audio_Stream = cbAudioStream.SelectedIndex;
        }

        private void cbVideoStream_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            player.Video_Stream = cbVideoStream.SelectedIndex;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbAudioOutputDevice.Items.Clear();
            foreach (var device in player.Audio_OutputDevices)
            {
                cbAudioOutputDevice.Items.Add(device.Description);
            }

            if (cbAudioOutputDevice.Items.Count > 0)
            {
                cbAudioOutputDevice.SelectedIndex = 0;
            }

            cbVideoDeinterlace.SelectedIndex = 0;
            cbVideoRendererAnaglyphMode.SelectedIndex = 0;
        }

        private async void btSaveScreenshot_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
                                     {
                                         FileName = "frame.jpg",
                                         InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                                         Filter = "JPEG Image|*.jpg|All files|*.*"
                                     };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await player.TakeSnapshotAsync(saveFileDialog.FileName, SnapshotFormat.JPEG);
            }
        }

        private void tbVolume1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null)
            {
                return;
            }

            player.Volume = (int)tbVolume1.Value;
        }

        private void btSelectFile_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                edFilenameOrURL.Text = openFileDialog.FileName;
            }
        }

        private async void btReadInfo_Click(object sender, RoutedEventArgs e)
        {
            var res = await infoReader.ReadFileInfo(new Uri(edFilenameOrURL.Text));

            mmInfo.Text = string.Empty;

            foreach (var track in res.VideoStreams)
            {
                mmInfo.Text += "Video stream: " + Environment.NewLine;
                mmInfo.Text += track.ToString();

                mmInfo.Text += Environment.NewLine;
            }

            foreach (var track in player.Audio_Streams)
            {
                mmInfo.Text += "Audio stream: " + Environment.NewLine;
                mmInfo.Text += track.ToString();

                mmInfo.Text += Environment.NewLine;
            }

            foreach (var track in player.Subtitle_Streams)
            {
                mmInfo.Text += "Subtitle stream: " + Environment.NewLine;
                mmInfo.Text += track.ToString();

                mmInfo.Text += Environment.NewLine;
            }
        }

        private void BtImageLogoAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ImageLogoSettingsDialog();

            var name = dlg.GenerateNewEffectName(player);
            var effect = new ImageOverlay(name);

            player.Video_Overlays_Add(effect);
            lbImageLogos.Items.Add(effect.Name);

            dlg.Fill(effect);
            dlg.ShowDialog();
            dlg.Dispose();
        }

        private void BtImageLogoEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lbImageLogos.SelectedItem != null)
            {
                var dlg = new ImageLogoSettingsDialog();
                var effect = player.Video_Overlays_Get((string)lbImageLogos.SelectedItem);
                dlg.Attach(effect);

                dlg.ShowDialog();
                dlg.Dispose();
            }
        }

        private void BtImageLogoRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbImageLogos.SelectedItem != null)
            {
                player.Video_Overlays_Remove((string)lbImageLogos.SelectedItem);
                lbImageLogos.Items.Remove(lbImageLogos.SelectedItem);
            }
        }

        private void tbBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null)
            {
                return;
            }

            player.Video_Adjust_Brightness = (int)tbBrightness.Value / 100.0f;
        }

        private void tbSaturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null)
            {
                return;
            }

            player.Video_Adjust_Saturation = (int)tbSaturation.Value / 100.0f;
        }

        private void tbHue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null)
            {
                return;
            }

            player.Video_Adjust_Hue = (int)tbHue.Value;
        }

        private void tbContrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null)
            {
                return;
            }

            player.Video_Adjust_Contrast = (int)tbContrast.Value / 100.0f;
        }

        private void tbGamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null)
            {
                return;
            }

            player.Video_Adjust_Gamma = (int)tbGamma.Value / 100.0f;
        }

        private void cbVideoAdjust_Click(object sender, RoutedEventArgs e)
        {
            player.Video_Adjust_Enabled = cbVideoAdjust.IsChecked == true;
        }

        private void ApplyVideoAdjustments()
        {
            player.Video_Adjust_Enabled = cbVideoAdjust.IsChecked == true;

            player.Video_Adjust_Brightness = (int)tbBrightness.Value / 100.0f;
            player.Video_Adjust_Saturation = (int)tbSaturation.Value / 100.0f;
            player.Video_Adjust_Hue = (float)tbHue.Value;
            player.Video_Adjust_Contrast = (int)tbContrast.Value / 100.0f;
            player.Video_Adjust_Gamma = (int)tbGamma.Value / 100.0f;
        }

        private void cbVideoDeinterlace_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyVideoDeinterlace();
        }

        private void ApplyVideoDeinterlace()
        {
            if (player == null)
            {
                return;
            }

            player.Video_Deinterlace = (DeinterlaceMode)cbVideoDeinterlace.SelectedIndex;
        }

        private void BtTextLogoAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new TextLogoSettingsDialog();

            var name = $"textlogo{lbTextLogos.Items.Count + 1}";
            var effect = new TextOverlay(name, "TEST", 20, 20);

            player.Video_Overlays_Add(effect);
            lbTextLogos.Items.Add(effect.Name);
            dlg.Fill(effect);

            dlg.ShowDialog();
            dlg.Dispose();
        }

        private void BtTextLogoEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lbTextLogos.SelectedItem != null)
            {
                var dlg = new TextLogoSettingsDialog();
                var effect = player.Video_Overlays_Get((string)lbTextLogos.SelectedItem);
                dlg.Attach(effect);

                dlg.ShowDialog();
                dlg.Dispose();
            }
        }

        private void BtTextLogoRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbTextLogos.SelectedItem != null)
            {
                player.Video_Overlays_Remove((string)lbTextLogos.SelectedItem);
                lbTextLogos.Items.Remove(lbTextLogos.SelectedItem);
            }
        }

        private void cbGreyscale_Checked(object sender, RoutedEventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new GrayscaleVideoEffect(cbGreyscale.IsChecked == true));
        }

        private void tbVideoRotate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null)
            {
                return;
            }

            player.Video_Rotate = (uint)tbVideoRotate.Value;
        }

        private void cbFlipX_Checked(object sender, RoutedEventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new HorizontalFlipVideoEffect(cbFlipX.IsChecked == true));
        }

        private void cbFlipY_Checked(object sender, RoutedEventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new VerticalFlipVideoEffect(cbFlipY.IsChecked == true));
        }

        private void cbInvert_Checked(object sender, RoutedEventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new InvertVideoEffect(cbInvert.IsChecked == true));
        }

        private void tbSepia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Video_Effects_AddOrUpdate(new SepiaVideoEffect(tbSepia.Value > 0, (byte)tbSepia.Value));
        }

        private void tbSharpen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Video_Effects_AddOrUpdate(new SharpenVideoEffect(
                tbSharpen.Value > 0, (int)tbSharpen.Value / 100.0f));
        }

        private void tbGaussianBlur_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Video_Effects_AddOrUpdate(new GaussianBlurVideoEffect(
                tbGaussianBlur.Value > 0, (int)tbGaussianBlur.Value / 100.0f));
        }

        private void tbMotionBlur_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Video_Effects_AddOrUpdate(new MotionBlurVideoEffect(
                tbMotionBlur.Value > 0, (byte)tbMotionBlur.Value));
        }

        private void cbVideoRendererAnaglyphMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (player == null)
            {
                return;
            }

            player.Video_Renderer_3D_Anaglyph_Mode = (Anaglyph3DMode)cbVideoRendererAnaglyphMode.SelectedIndex;
        }

        private void btZoomOut_Click(object sender, RoutedEventArgs e)
        {
            player.Video_Zoom -= 0.05f;
        }

        private void btZoomIn_Click(object sender, RoutedEventArgs e)
        {
            player.Video_Zoom += 0.05f;
        }

        private void btZoomShiftUp_Click(object sender, RoutedEventArgs e)
        {
            player.Video_Zoom_ShiftY += 2;
        }

        private void btZoomShiftDown_Click(object sender, RoutedEventArgs e)
        {
            player.Video_Zoom_ShiftY -= 2;
        }

        private void btZoomShiftLeft_Click(object sender, RoutedEventArgs e)
        {
            player.Video_Zoom_ShiftX -= 2;
        }

        private void btZoomShiftRight_Click(object sender, RoutedEventArgs e)
        {
            player.Video_Zoom_ShiftX += 2;
        }

        private void btZoomReset_Click(object sender, RoutedEventArgs e)
        {
            player.Video_Zoom = 1.0f;
            player.Video_Zoom_ShiftX = 0;
            player.Video_Zoom_ShiftY = 0;
        }

        private void cbOldMovie_Checked(object sender, RoutedEventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new OldMovieVideoEffect(cbOldMovie.IsChecked == true));
        }

        #region 360 degree

        private bool videoViewMouseDown;

        private Point videoViewMousePos;

        private void VideoView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            videoViewMouseDown = true;

            videoViewMousePos = e.GetPosition(VideoView);
        }

        private void VideoView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            videoViewMouseDown = false;

            videoViewMousePos = new Point();
        }

        private void VideoView_MouseMove(object sender, MouseEventArgs e)
        {
            if (videoViewMouseDown)
            {
                var pos = e.GetPosition(VideoView);

                var deltaX = (float)(pos.X - videoViewMousePos.X);
                var deltaY = (float)(pos.Y - videoViewMousePos.Y);

                videoViewMousePos = pos;

                var value = player.Video_Viewpoint_Get();
                player.Video_Viewpoint_Set(new D360Viewpoint(
                    value.Yaw + deltaX,
                    value.Pitch + deltaY,
                    value.Roll,
                    80));
            }
        }


        #endregion

        private void ApplyCrop()
        {
            if (edCropLeft.Text == "0" && edCropTop.Text == "0" && edCropRight.Text == "0" && edCropBottom.Text == "0")
            {
                player.Video_Crop = null;
            }
            else
            {
                player.Video_Crop = new VideoCropSettings(
                    Convert.ToInt32(edCropLeft.Text),
                    Convert.ToInt32(edCropTop.Text),
                    Convert.ToInt32(edCropRight.Text),
                    Convert.ToInt32(edCropBottom.Text));
            }
        }

        private void btCropUpdate_Click(object sender, RoutedEventArgs e)
        {
            ApplyCrop();
        }
    }
}