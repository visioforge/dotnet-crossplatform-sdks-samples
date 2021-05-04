// ReSharper disable StyleCop.SA1600
// ReSharper disable StyleCop.SA1300

namespace MainDemoUWP
{
    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InfoPage : Page
    {
        public ListBox Info => mmInfo;

        private MainPage mainPage;

        public InfoPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is MainPage page)
            {
                mainPage = page;
                page.InfoPage = this;
            }

            base.OnNavigatedTo(e);
        }

        private async void btReadInfo_Click(object sender, RoutedEventArgs e)
        {
            var res = await mainPage.InfoReader.ReadFileInfo(new Uri(mainPage.FilenameOrURL.Text));

            mmInfo.Items?.Clear();

            if (res == null)
            {
                return;
            }

            foreach (var track in res.VideoStreams)
            {
                mmInfo.Items?.Add("Video stream: ");
                mmInfo.Items?.Add(track.ToString());
            }

            foreach (var track in mainPage.Player.Audio_Streams)
            {
                mmInfo.Items?.Add("Audio stream: ");
                mmInfo.Items?.Add(track.ToString());
            }

            foreach (var track in mainPage.Player.Subtitle_Streams)
            {
                mmInfo.Items?.Add("Subtitle stream: ");
                mmInfo.Items?.Add(track.ToString());
            }
        }
    }
}
