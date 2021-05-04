// ReSharper disable StyleCop.SA1600
// ReSharper disable InconsistentNaming
// ReSharper disable StyleCop.SA1300

namespace Main_Demo_WinForms
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using VisioForge.CrossPlatform.Controls;
    using VisioForge.CrossPlatform.Controls.MediaPlayer;
    using VisioForge.CrossPlatform.Controls.Types;
    using VisioForge.CrossPlatform.Controls.Types.VideoProcessing;
    using VisioForge.CrossPlatform.Controls.WinForms.Dialogs;

    public partial class Form1 : Form
    {
        private readonly MediaPlayer player;

        private readonly MediaInfoReader infoReader;

        public Form1()
        {
            InitializeComponent();

            player = new MediaPlayer(videoView1);
            player.OnPositionChange += Player_OnPositionChange;
            player.OnStop += Player_OnStop;
            player.OnMediaLoaded += Player_OnMediaLoaded;
            player.OnError += Player_OnError;

            infoReader = new MediaInfoReader();
        }

        private void Player_OnError(object sender, VisioForge.CrossPlatform.Controls.Types.ErrorEventArgs e)
        {
            BeginInvoke((Action)(() =>
                                        {
                                            mmLog.Text += e.Message + Environment.NewLine;
                                        }));
        }

        private void Player_OnMediaLoaded(object sender, EventArgs e)
        {
            BeginInvoke((Action)(() =>
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

                                            if (player.Is360Video)
                                            {
                                                videoViewFake.Show();
                                            }
                                            else
                                            {
                                                videoViewFake.Hide();
                                            }
                                        }));
        }

        private void Player_OnStop(object sender, EventArgs e)
        {
            BeginInvoke((Action)(async () =>
                                        {
                                            await player.StopAsync();
                                            tbTimeline.Value = 0;
                                            lbTime.Text = "00:00:00/" + TimeSpan.FromSeconds(tbTimeline.Maximum).ToString(@"hh\:mm\:ss");
                                        }));
        }

        private void Player_OnPositionChange(object sender, PositionChangedEventArgs e)
        {
            BeginInvoke((Action)(() =>
                                        {
                                            tbTimeline.Maximum = (int)(player.Duration.TotalMilliseconds / 1000);

                                            int value = (int)(player.Position.TotalMilliseconds / 1000);
                                            if ((value > 0) && (value < tbTimeline.Maximum))
                                            {
                                                tbTimeline.Value = value;
                                            }

                                            lbTime.Text = player.Position.ToString(@"hh\:mm\:ss") + "/" + player.Duration.ToString(@"hh\:mm\:ss");
                                        }));
        }

        private async void BtStart_Click(object sender, EventArgs e)
        {
            ApplyCrop();

            if (cbAudioOutputDevice.SelectedIndex != -1)
            {
                player.Audio_OutputDevice = cbAudioOutputDevice.Text;
            }
            
            player.Mute = !cbPlayAudio.Checked;
            
            ApplyVideoAdjustments();
            ApplyVideoDeinterlace();

            await player.PlayAsync(new Uri(edFilenameOrURL.Text));
        }

        private async void BtResume_Click(object sender, EventArgs e)
        {
            await player.ResumeAsync();
        }

        private async void BtPause_Click(object sender, EventArgs e)
        {
            await player.PauseAsync();
        }

        private async void BtStop_Click(object sender, EventArgs e)
        {
           await player.StopAsync();
        }

        private void TbTimeline_Scroll(object sender, EventArgs e)
        {
            if (Convert.ToInt32(tbTimeline.Tag) == 0)
            {
                player.Position = new TimeSpan(tbTimeline.Value * 1000 * TimeSpan.TicksPerMillisecond);
            }
        }

        private void BtNextFrame_Click(object sender, EventArgs e)
        {
            player.NextFrame();
        }

        private void TbSpeed_Scroll(object sender, EventArgs e)
        {
            player.Speed = tbSpeed.Value / 10.0f;
        }

        private void BtPreviousFrame_Click(object sender, EventArgs e)
        {
            player.PreviousFrame();
        }

        private void CbAudioStream_SelectedIndexChanged(object sender, EventArgs e)
        {
            player.Audio_Stream = cbAudioStream.SelectedIndex;
        }

        private void CbVideoStream_SelectedIndexChanged(object sender, EventArgs e)
        {
            player.Video_Stream = cbVideoStream.SelectedIndex;
        }

        private void Form1_Load(object sender, EventArgs e)
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

        private async void BtSaveScreenshot_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "frame.jpg",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Filter = "JPEG Image|*.jpg|All files|*.*"
            };

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                await player.TakeSnapshotAsync(saveFileDialog.FileName, SnapshotFormat.JPEG);
            }
        }

        private void TbVolume1_Scroll(object sender, EventArgs e)
        {
            player.Volume = tbVolume1.Value;
        }

        private void BtSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                edFilenameOrURL.Text = openFileDialog1.FileName;
            }
        }

        private async void BtReadInfo_Click(object sender, EventArgs e)
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

        private void BtImageLogoAdd_Click(object sender, EventArgs e)
        {
            var dlg = new ImageLogoSettingsDialog();

            var name = dlg.GenerateNewEffectName(player);
            var effect = new ImageOverlay(name);

            player.Video_Overlays_Add(effect);
            lbImageLogos.Items.Add(effect.Name);

            dlg.Fill(effect);
            dlg.ShowDialog(this);
            dlg.Dispose();
        }

        private void BtImageLogoEdit_Click(object sender, EventArgs e)
        {
            if (lbImageLogos.SelectedItem != null)
            {
                var dlg = new ImageLogoSettingsDialog();
                var effect = player.Video_Overlays_Get((string)lbImageLogos.SelectedItem);
                dlg.Attach(effect);

                dlg.ShowDialog(this);
                dlg.Dispose();
            }
        }

        private void BtImageLogoRemove_Click(object sender, EventArgs e)
        {
            if (lbImageLogos.SelectedItem != null)
            {
                player.Video_Overlays_Remove((string)lbImageLogos.SelectedItem);
                lbImageLogos.Items.Remove(lbImageLogos.SelectedItem);
            }
        }

        private void tbBrightness_Scroll(object sender, EventArgs e)
        {
            player.Video_Adjust_Brightness = tbBrightness.Value / 100.0f;
        }

        private void tbSaturation_Scroll(object sender, EventArgs e)
        {
            player.Video_Adjust_Saturation = tbSaturation.Value / 100.0f;
        }

        private void tbHue_Scroll(object sender, EventArgs e)
        {
            player.Video_Adjust_Hue = tbHue.Value;
        }

        private void tbContrast_Scroll(object sender, EventArgs e)
        {
            player.Video_Adjust_Contrast = tbContrast.Value / 100.0f;
        }

        private void tbGamma_Scroll(object sender, EventArgs e)
        {
            player.Video_Adjust_Gamma = tbGamma.Value / 100.0f;
        }

        private void cbAdjustments_CheckedChanged(object sender, EventArgs e)
        {
            player.Video_Adjust_Enabled = cbVideoAdjust.Checked;
        }

        private void ApplyVideoAdjustments()
        {
            player.Video_Adjust_Enabled = cbVideoAdjust.Checked;

            player.Video_Adjust_Brightness = tbBrightness.Value / 100.0f;
            player.Video_Adjust_Saturation = tbSaturation.Value / 100.0f;
            player.Video_Adjust_Hue = tbHue.Value;
            player.Video_Adjust_Contrast = tbContrast.Value / 100.0f;
            player.Video_Adjust_Gamma = tbGamma.Value / 100.0f;
        }

        private void cbVideoDeinterlace_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyVideoDeinterlace();
        }

        private void ApplyVideoDeinterlace()
        {
            player.Video_Deinterlace = (DeinterlaceMode)cbVideoDeinterlace.SelectedIndex;
        }

        private void btTextLogoAdd_Click(object sender, EventArgs e)
        {
            var dlg = new TextLogoSettingsDialog();

            var name = $"textlogo{lbTextLogos.Items.Count + 1}";
            var effect = new TextOverlay(name, "TEST", 20, 20);

            player.Video_Overlays_Add(effect);
            lbTextLogos.Items.Add(effect.Name);
            dlg.Fill(effect);

            dlg.ShowDialog(this);
            dlg.Dispose();
        }

        private void btTextLogoEdit_Click(object sender, EventArgs e)
        {
            if (lbTextLogos.SelectedItem != null)
            {
                var dlg = new TextLogoSettingsDialog();
                var effect = player.Video_Overlays_Get((string)lbTextLogos.SelectedItem);
                dlg.Attach(effect);

                dlg.ShowDialog(this);
                dlg.Dispose();
            }
        }

        private void btTextLogoRemove_Click(object sender, EventArgs e)
        {
            if (lbTextLogos.SelectedItem != null)
            {
                player.Video_Overlays_Remove((string)lbTextLogos.SelectedItem);
                lbTextLogos.Items.Remove(lbTextLogos.SelectedItem);
            }
        }

        private void cbGreyscale_CheckedChanged(object sender, EventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new GrayscaleVideoEffect(cbGreyscale.Checked));
        }

        private void tbVideoRotate_Scroll(object sender, EventArgs e)
        {
            player.Video_Rotate = (uint)tbVideoRotate.Value;
        }

        private void cbFlipX_CheckedChanged(object sender, EventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new HorizontalFlipVideoEffect(cbFlipX.Checked));
        }

        private void cbFlipY_CheckedChanged(object sender, EventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new VerticalFlipVideoEffect(cbFlipY.Checked));
        }

        private void cbInvert_CheckedChanged(object sender, EventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new InvertVideoEffect(cbInvert.Checked));
        }

        private void tbSepia_Scroll(object sender, EventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new SepiaVideoEffect(tbSepia.Value > 0, (byte)tbSepia.Value));
        }

        private void tbSharpen_Scroll(object sender, EventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new SharpenVideoEffect(tbSharpen.Value > 0, tbSharpen.Value / 100.0f));
        }

        private void tbGaussianBlur_Scroll(object sender, EventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new GaussianBlurVideoEffect(tbGaussianBlur.Value > 0, tbGaussianBlur.Value / 100.0f));
        }

        private void tbMotionBlur_Scroll(object sender, EventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new MotionBlurVideoEffect(tbMotionBlur.Value > 0, (byte)tbMotionBlur.Value));
        }

        private void cbVideoRendererAnaglyphMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            player.Video_Renderer_3D_Anaglyph_Mode = (Anaglyph3DMode)cbVideoRendererAnaglyphMode.SelectedIndex;
        }

        private void btEffZoomOut_Click(object sender, EventArgs e)
        {
            player.Video_Zoom -= 0.05f; 
        }

        private void btEffZoomIn_Click(object sender, EventArgs e)
        {
            player.Video_Zoom += 0.05f;
        }

        private void btEffZoomUp_Click(object sender, EventArgs e)
        {
            player.Video_Zoom_ShiftY += 2;
        }

        private void btEffZoomDown_Click(object sender, EventArgs e)
        {
            player.Video_Zoom_ShiftY -= 2;
        }

        private void btEffZoomLeft_Click(object sender, EventArgs e)
        {
            player.Video_Zoom_ShiftX -= 2;
        }

        private void btEffZoomRight_Click(object sender, EventArgs e)
        {
            player.Video_Zoom_ShiftX += 2;
        }

        private void btEffZoomReset_Click(object sender, EventArgs e)
        {
            player.Video_Zoom = 1.0f;
            player.Video_Zoom_ShiftX = 0;
            player.Video_Zoom_ShiftY = 0;
        }

        private void cbOldMovie_CheckedChanged(object sender, EventArgs e)
        {
            player.Video_Effects_AddOrUpdate(new OldMovieVideoEffect(cbOldMovie.Checked));
        }

        #region 360 degree

        private bool videoViewMouseDown;

        private Point videoViewMousePos;

        private void videoViewFake_MouseDown(object sender, MouseEventArgs e)
        {
            videoViewMouseDown = true;

            videoViewMousePos = e.Location;
        }

        private void videoViewFake_MouseMove(object sender, MouseEventArgs e)
        {
            if (videoViewMouseDown)
            {
                var deltaX = e.X - videoViewMousePos.X;
                var deltaY = e.Y - videoViewMousePos.Y;

                videoViewMousePos = e.Location;

                var value = player.Video_Viewpoint_Get();
                player.Video_Viewpoint_Set(new D360Viewpoint(value.Yaw + deltaX, value.Pitch + deltaY, value.Roll, 80));
            }
        }

        private void videoViewFake_MouseUp(object sender, MouseEventArgs e)
        {
            videoViewMouseDown = false;

            videoViewMousePos = new Point();
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

        private void btCropUpdate_Click(object sender, EventArgs e)
        {
            ApplyCrop();
        }
    }
}
