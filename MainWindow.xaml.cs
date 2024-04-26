using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.VisualBasic.ApplicationServices;
using SatisfactorySavestateManager.classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SatisfactorySavestateManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Config config = new Config();
        public SteamInfo steamInfo = new SteamInfo();
        public MainWindow()
        {
            InitializeComponent();
            steamInfo.GetSteam64ID();
            steamInfo.GetSteamProfileInfo();
            labelUserName.Content = steamInfo.DisplayName;
            SetImageFromUrl(steamInfo.ProfilePicUrl);
            if(config.IsPrivateKeySet() && config.IsSaveStatePathValid())
            {
                canvasUploadDownload.Visibility = Visibility.Visible;
                canvasSetup.Visibility = Visibility.Hidden;
            }
            else
            {
                canvasSetup.Visibility = Visibility.Visible;
                canvasUploadDownload.Visibility = Visibility.Hidden;
            }

            //Properties.Settings.Default.SaveLocation = null;
            //Properties.Settings.Default.PrivateKeyJSONPath = null;
            //Properties.Settings.Default.Save();
        }

        //ITEM COLORS:
        //current step: #FFF5D183
        //failed: #FFF58383
        //succeeded: #FFCAF583
        //disabled: #FF737070

        public SolidColorBrush colorGreen = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFCAF583"));
        public SolidColorBrush colorRed = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF58383"));
        public SolidColorBrush colorYellow = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5D183"));
        public SolidColorBrush colorGrey = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF737070"));
        public SolidColorBrush colorWhite = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        private void buttonImportDriveCredentials_Click(object sender, RoutedEventArgs e)
        {
            config.SelectJsonFile();
            if(config.AreCredentialsValid())
            {
                step1Ellipse.Stroke = colorGreen;
                step1Bar1.Fill = colorGreen;
                step1Bar1.Stroke = colorGreen;
                step1Bar2.Fill = colorGreen;
                step1Bar2.Stroke = colorGreen;
                step1Label.Foreground = colorGrey;
                step1EllipseLabel.Foreground = colorGrey;
                buttonImportDriveCredentials.IsEnabled = false;
                buttonImportDriveCredentials.Opacity = 40;

                step2Ellipse.Stroke = colorYellow;
                step2Bar1.Fill = colorWhite;
                step2Bar1.Stroke = colorWhite;
                step2Bar2.Fill = colorWhite;
                step2Bar2.Stroke = colorWhite;
                step2Label.Foreground = colorWhite;
                step2EllipseLabel.Foreground = colorWhite;
                buttonSelectSaveStatePath.IsEnabled = true;
                buttonSelectSaveStatePath.Opacity = 100;
                buttonSetDefaultSaveStatePath.IsEnabled = true;
                buttonSetDefaultSaveStatePath.Opacity = 100;
            }
            else
            {
                step1Ellipse.Stroke = colorRed;
            }
        }

        private void SetImageFromUrl(string url)
        {
            ImageBrush imageBrush = (ImageBrush)imageProfilePicture.Fill;
            imageBrush.ImageSource = new BitmapImage(new Uri(url, UriKind.Absolute));
        }

        private void buttonSelectSaveStatePath_Click(object sender, RoutedEventArgs e)
        {
            config.SelectSaveStatePath();
            if(config.IsSaveStatePathValid())
            {
                AllSetUp();
            }
            else
            {
                step2Ellipse.Stroke = colorRed;
                Properties.Settings.Default.SaveLocation = null;
                Properties.Settings.Default.Save();
            }
        }

        private void buttonSetDefaultSaveStatePath_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SaveLocation = Path.Combine(steamInfo.DefaultSavePath, steamInfo.Steam64ID);
            Properties.Settings.Default.Save();
            if (config.IsSaveStatePathValid())
            {
                AllSetUp();
            }
            else
            {
                step2Ellipse.Stroke = colorRed;
                Properties.Settings.Default.SaveLocation = null;
                Properties.Settings.Default.Save();
            }
        }

        private void AllSetUp()
        {
            if (Properties.Settings.Default.SaveLocation != null)
            {
                step2Ellipse.Stroke = colorGreen;
                step2Bar1.Fill = colorGreen;
                step2Bar1.Stroke = colorGreen;
                step2Bar2.Fill = colorGreen;
                step2Bar2.Stroke = colorGreen;
                step2Label.Foreground = colorGrey;
                step2EllipseLabel.Foreground = colorGrey;
                buttonSelectSaveStatePath.IsEnabled = false;
                buttonSelectSaveStatePath.Opacity = 40;
                buttonSetDefaultSaveStatePath.IsEnabled = false;
                buttonSetDefaultSaveStatePath.Opacity = 40;

                step3Ellipse.Stroke = colorGreen;
                step3Label.Foreground = colorWhite;
                step3EllipseLabel.Foreground = colorWhite;

                buttonGetStarted.Opacity = 100;
                buttonGetStarted.IsEnabled = true;
            }
        }

        private void buttonGetStarted_Click(object sender, RoutedEventArgs e)
        {
            canvasSetup.Visibility = Visibility.Hidden;
            canvasUploadDownload.Visibility = Visibility.Visible;
        }
    }
}
