// ReSharper disable StyleCop.SA1600
// ReSharper disable StyleCop.SA1300

namespace MainDemoUWP
{
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AudioOutputPage : Page
    {
        public ComboBox AudioOutputDevice => cbAudioOutputDevice;

        public CheckBox PlayAudio => cbPlayAudio;

        private MainPage mainPage;

        public AudioOutputPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is MainPage page)
            {
                mainPage = page;
                page.AudioOutputPage = this;
            }

            base.OnNavigatedTo(e);
        }

        private void tbVolume1_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mainPage == null)
            {
                return;
            }

            mainPage.Player.Volume = (int)tbVolume1.Value;
        }
    }
}
