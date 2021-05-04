// ReSharper disable StyleCop.SA1600
// ReSharper disable StyleCop.SA1300

namespace MainDemoUWP
{
    using VisioForge.CrossPlatform.Controls.Types.VideoProcessing;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Navigation;

    //using VisioForge.CrossPlatform.Controls.UWP.Dialogs;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EffectsPage : Page
    {
        private MainPage mainPage;

        public EffectsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is MainPage page)
            {
                mainPage = page;
            }

            base.OnNavigatedTo(e);
        }

        private void btImageLogoAdd_Click(object sender, RoutedEventArgs e)
        {
            //var dlg = new ImageLogoSettingsDialog();

            //var name = dlg.GenerateNewEffectName(mainPage.Player);
            //var effect = new ImageOverlay(name);

            //mainPage.Player.Video_Overlays_Add(effect);
            //lbImageLogos.Items?.Add(effect.Name);

            //dlg.Fill(effect);
            //dlg.ShowDialog(this);
            //dlg.Dispose();
        }

        private void btImageLogoEdit_Click(object sender, RoutedEventArgs e)
        {
            //if (lbImageLogos.SelectedItem != null)
            //{
            //    var dlg = new ImageLogoSettingsDialog();
            //    var effect = mainPage.Player.Video_Overlays_Get((string)lbImageLogos.SelectedItem);
            //    dlg.Attach(effect);

            //    dlg.ShowDialog(this);
            //    dlg.Dispose();
            //}
        }

        private void btImageLogoRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbImageLogos.SelectedItem != null)
            {
                mainPage.Player.Video_Overlays_Remove((string)lbImageLogos.SelectedItem);
                lbImageLogos.Items?.Remove(lbImageLogos.SelectedItem);
            }
        }

        private void cbVideoDeinterlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            ApplyVideoDeinterlace();
        }

        private void ApplyVideoDeinterlace()
        {
            mainPage.Player.Video_Deinterlace = (DeinterlaceMode)cbVideoDeinterlace.SelectedIndex;
        }

        private void btTextLogoAdd_Click(object sender, RoutedEventArgs e)
        {
            //var dlg = new TextLogoSettingsDialog();

            //var name = $"textlogo{lbTextLogos.Items.Count + 1}";
            //var effect = new TextOverlay(name, "TEST", 20, 20);

            //mainPage.Player.Video_Overlays_Add(effect);
            //lbTextLogos.Items.Add(effect.Name);
            //dlg.Fill(effect);

            //dlg.ShowDialog(this);
            //dlg.Dispose();
        }

        private void btTextLogoEdit_Click(object sender, RoutedEventArgs e)
        {
            //if (lbTextLogos.SelectedItem != null)
            //{
            //    var dlg = new TextLogoSettingsDialog();
            //    var effect = mainPage.Player.Video_Overlays_Get((string)lbTextLogos.SelectedItem);
            //    dlg.Attach(effect);

            //    dlg.ShowDialog(this);
            //    dlg.Dispose();
            //}
        }

        private void btTextLogoRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbTextLogos.SelectedItem != null)
            {
                mainPage.Player.Video_Overlays_Remove((string)lbTextLogos.SelectedItem);
                lbTextLogos.Items?.Remove(lbTextLogos.SelectedItem);
            }
        }

        private void cbGreyscale_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Effects_AddOrUpdate(new GrayscaleVideoEffect(cbGreyscale.IsChecked == true));
        }

        private void cbInvert_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Effects_AddOrUpdate(new InvertVideoEffect(cbInvert.IsChecked == true));
        }

        private void tbSepia_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Effects_AddOrUpdate(new SepiaVideoEffect(tbSepia.Value > 0, (byte)tbSepia.Value));
        }

        private void tbSharpen_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Effects_AddOrUpdate(new SharpenVideoEffect(tbSharpen.Value > 0, (float)tbSharpen.Value / 100.0f));
        }

        private void tbGaussianBlur_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Effects_AddOrUpdate(new GaussianBlurVideoEffect(tbGaussianBlur.Value > 0, (float)tbGaussianBlur.Value / 100.0f));
        }

        private void tbMotionBlur_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Effects_AddOrUpdate(new MotionBlurVideoEffect(tbMotionBlur.Value > 0, (byte)tbMotionBlur.Value));
        }

        private void cbOldMovie_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Effects_AddOrUpdate(new OldMovieVideoEffect(cbOldMovie.IsChecked == true));
        }
    }
}
