using System;
using VisioForge.CrossPlatform.Controls.MediaPlayer;
using VisioForge.CrossPlatform.Core.Shared;

namespace MainDemoNetCore
{
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            MediaPlayer mediaPlayer = new MediaPlayer(null);
            await mediaPlayer.PlayAsync(new Uri("http://help.visioforge.com/video.mp4"));

            Console.WriteLine("Please press any key to stop.");
            Console.ReadKey();
        }
    }
}