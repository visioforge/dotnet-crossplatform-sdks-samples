using System.Windows;

namespace MainDemoUWP
{
    using LibVLCSharp.Shared;

    public partial class App : Application
    {
        public App()
        {
            Core.Initialize();
        }
    }
}