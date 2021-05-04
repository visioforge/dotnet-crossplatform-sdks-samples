// ReSharper disable StyleCop.SA1600
// ReSharper disable StyleCop.SA1300

namespace MainDemoUWP
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdjustmentsPage : Page
    {
        private MainPage mainPage;

        public AdjustmentsPage()
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

        private void tbBrightness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Adjust_Brightness = (float)tbBrightness.Value / 100.0f;
        }

        private void tbSaturation_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Adjust_Saturation = (float)tbSaturation.Value / 100.0f;
        }

        private void tbHue_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Adjust_Hue = (float)tbHue.Value;
        }

        private void tbContrast_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Adjust_Contrast = (float)tbContrast.Value / 100.0f;
        }

        private void tbGamma_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Adjust_Gamma = (float)tbGamma.Value / 100.0f;
        }

        private void cbVideoAdjust_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Adjust_Enabled = cbVideoAdjust.IsChecked == true;
        }

        public void ApplyVideoAdjustments()
        {
            mainPage.Player.Video_Adjust_Enabled = cbVideoAdjust.IsChecked == true;

            mainPage.Player.Video_Adjust_Brightness = (float)tbBrightness.Value / 100.0f;
            mainPage.Player.Video_Adjust_Saturation = (float)tbSaturation.Value / 100.0f;
            mainPage.Player.Video_Adjust_Hue = (float)tbHue.Value;
            mainPage.Player.Video_Adjust_Contrast = (float)tbContrast.Value / 100.0f;
            mainPage.Player.Video_Adjust_Gamma = (float)tbGamma.Value / 100.0f;
        }
    }
}
