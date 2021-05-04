using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MainDemoUWP
{
    using VisioForge.CrossPlatform.Controls.Types.VideoProcessing;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CropPage : Page
    {
        private MainPage mainPage;

        public TextBox CropLeft => edCropLeft;

        public TextBox CropTop => edCropTop;

        public TextBox CropRight => edCropRight;

        public TextBox CropBottom => edCropBottom;

        public CropPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is MainPage page)
            {
                mainPage = page;
                page.CropPage = this;
            }

            base.OnNavigatedTo(e);
        }

        public void ApplyCrop()
        {
            if (edCropLeft.Text == "0" && edCropTop.Text == "0" && edCropRight.Text == "0" && edCropBottom.Text == "0")
            {
                mainPage.Player.Video_Crop = null;
            }
            else
            {
                mainPage.Player.Video_Crop = new VideoCropSettings(
                    Convert.ToInt32(edCropLeft.Text),
                    Convert.ToInt32(edCropTop.Text),
                    Convert.ToInt32(edCropRight.Text),
                    Convert.ToInt32(edCropBottom.Text));
            }
        }

        private void btCropUpdate_Click(object sender, RoutedEventArgs e)
        {
            ApplyCrop();
        }
    }
}
