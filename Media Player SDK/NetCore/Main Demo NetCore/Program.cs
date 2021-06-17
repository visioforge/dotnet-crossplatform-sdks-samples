using System;
using VisioForge.CrossPlatform.Controls.MediaPlayer;

namespace MainDemoNetCore
{
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            MediaPlayerControl mediaPlayer = new MediaPlayerControl(null);
            await mediaPlayer.PlayAsync(new Uri("http://help.visioforge.com/video.mp4"));

            Console.WriteLine("Please press any key to stop.");
            Console.ReadKey();
        }
    }
}