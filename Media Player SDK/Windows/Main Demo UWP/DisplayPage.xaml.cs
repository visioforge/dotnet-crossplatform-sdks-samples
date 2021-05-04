// ReSharper disable StyleCop.SA1600
// ReSharper disable StyleCop.SA1300
// ReSharper disable StyleCop.SA1600
// ReSharper disable StyleCop.SA1300

namespace MainDemoUWP
{
    using VisioForge.CrossPlatform.Controls.Types.VideoProcessing;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisplayPage : Page
    {
        private MainPage mainPage;

        public DisplayPage()
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

        private void tbVideoRotate_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Rotate = (uint)tbVideoRotate.Value;
        }

        private void cbFlipX_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Effects_AddOrUpdate(new HorizontalFlipVideoEffect(cbFlipX.IsChecked == true));
        }

        private void cbFlipY_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Effects_AddOrUpdate(new VerticalFlipVideoEffect(cbFlipY.IsChecked == true));
        }

        private void cbVideoRendererAnaglyphMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Video_Renderer_3D_Anaglyph_Mode = (Anaglyph3DMode)cbVideoRendererAnaglyphMode.SelectedIndex;
        }

        private void btZoomOut_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Zoom -= 0.05f;
        }

        private void btZoomIn_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Zoom += 0.05f;
        }

        private void btZoomUp_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Zoom_ShiftY += 2;
        }

        private void btZoomDown_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Zoom_ShiftY -= 2;
        }

        private void btZoomLeft_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Zoom_ShiftX -= 2;
        }

        private void btZoomRight_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Zoom_ShiftX += 2;
        }

        private void btZoomReset_Click(object sender, RoutedEventArgs e)
        {
            mainPage.Player.Video_Zoom = 1.0f;
            mainPage.Player.Video_Zoom_ShiftX = 0;
            mainPage.Player.Video_Zoom_ShiftY = 0;
        }
    }
}
