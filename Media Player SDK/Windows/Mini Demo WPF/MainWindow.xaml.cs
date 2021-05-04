using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mini_Demo_WPF
{
    using System.Diagnostics;

    using VisioForge.CrossPlatform.Controls.MediaPlayer;
    using VisioForge.CrossPlatform.Controls.Types;
    using VisioForge.CrossPlatform.Controls.WPF;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MediaPlayer player;

        public MainWindow()
        {
            InitializeComponent();

            player = new MediaPlayer(VV);
            player.OnError += Player_OnError;
        }

        private void Player_OnError(object sender, ErrorEventArgs e)
        {
            Dispatcher?.BeginInvoke((Action)(() =>
                                                    {
                                                        Debug.WriteLine(e.Message);
                                                    }));
        }

        private async void btPlay_Click(object sender, RoutedEventArgs e)
        {
            await player.PlayAsync(new Uri(edFilenameOrURL.Text));
        }

        private async void btStop_Click(object sender, RoutedEventArgs e)
        {
            await player.StopAsync();
        }
    }
}
