using VisioForge.CrossPlatform.Controls;
using System;
using System.IO;

namespace ConsoleDemoWindows
{
    using System.Threading.Tasks;

    using LibVLCSharp.Shared;

    using VisioForge.CrossPlatform.Controls.MediaPlayer;

    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter file name as app parameter.");
                return;
            }

            string filename = args[0];
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File {filename} don't exists!");
                return;
            }

            Core.Initialize();
            
            var mp = new MediaPlayerControl(null);
            await mp.PlayAsync(new Uri(filename));

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey();
        }
    }
}