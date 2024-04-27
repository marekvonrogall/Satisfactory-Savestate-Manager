using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SatisfactorySavestateManager.classes;
using Google.Apis.Drive.v3;
using System.Diagnostics;

namespace SatisfactorySavestateManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Config config = new Config();
        public SteamInfo steamInfo = new SteamInfo();
        public LocalSaves localSaves = new LocalSaves();
        public RemoteSaves remoteSaves = new RemoteSaves();
        public DriveUpload driveUpload = new DriveUpload();
        public DriveDownload driveDownload = new DriveDownload();
        private DriveService service;

        public MainWindow()
        {
            InitializeComponent();
            steamInfo.GetSteam64ID();
            steamInfo.GetSteamProfileInfo();
            labelUserName.Content = steamInfo.DisplayName;
            SetImageFromUrl(steamInfo.ProfilePicUrl);
            if(config.IsPrivateKeySet() && config.IsSaveStatePathValid())
            {
                MainApp();
            }
            else
            {
                canvasSetup.Visibility = Visibility.Visible;
                canvasUploadDownload.Visibility = Visibility.Hidden;
            }
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
            MainApp();
        }

        private void MainApp()
        {
            canvasSetup.Visibility = Visibility.Hidden;
            canvasUploadDownload.Visibility = Visibility.Visible;
            
            localSaves.SearchForSaves(Properties.Settings.Default.SaveLocation);
            CreateBoxes(localSaves.SaveSessionNames.Distinct(), localSessionContainer, true);

            service = remoteSaves.AuthenticateServiceAccount(Properties.Settings.Default.PrivateKeyJSONPath);
            remoteSaves.GetFolderNames(service, Properties.Settings.Default.FolderID);
            CreateBoxes(remoteSaves.SessionFolders.Distinct(), remoteSessionContainer, true);
        }

        private void CreateBoxes(IEnumerable<string> saveSessionNames, StackPanel stackPanel, bool isSessionName)
        {
            foreach (string name in saveSessionNames)
            {
                Border sessionBorder = new Border();
                sessionBorder.Background = new SolidColorBrush(Color.FromRgb(166, 178, 197)); // #FFA6B2C5
                sessionBorder.Padding = new Thickness(5);
                sessionBorder.Margin = new Thickness(5);
                sessionBorder.CornerRadius = new CornerRadius(5);
                sessionBorder.BorderThickness = new Thickness(3);
                sessionBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                Label sessionLabel = new Label();
                sessionLabel.Content = name;
                sessionLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                sessionBorder.Child = sessionLabel;
                sessionBorder.MouseEnter += (sender, e) => {
                    sessionBorder.Background = new SolidColorBrush(Color.FromRgb(89, 111, 146)); // #FF596F92
                };
                sessionBorder.MouseLeave += (sender, e) => {
                    sessionBorder.Background = new SolidColorBrush(Color.FromRgb(166, 178, 197)); // #FFA6B2C5
                };
                if(isSessionName)
                {
                    sessionBorder.MouseLeftButtonDown += (sender, e) => {
                        SessionNameClicked(name, stackPanel);
                    };
                }
                else
                {
                    sessionBorder.MouseLeftButtonDown += (sender, e) => {
                        SaveStateClicked(name, stackPanel);
                    };
                }
                stackPanel.Children.Add(sessionBorder);
            }
        }

        private void SessionNameClicked(string sessionName, StackPanel stackPanel)
        {
            if(stackPanel.Name == "localSessionContainer")
            {
                buttonBackLocal.Content = "< Back";
                localSessionContainer.Children.Clear();
                CreateBoxes(localSaves.GetMatchingSessionNames(sessionName), localSessionContainer, false);
            }
            if (stackPanel.Name == "remoteSessionContainer")
            {
                buttonBackRemote.Content = "< Back";
                remoteSaves.GetSaveStateNames(service, sessionName);
                if(remoteSaves.SaveNames.Count > 0)
                {
                    remoteSessionContainer.Children.Clear();
                    CreateBoxes(remoteSaves.SaveNames, remoteSessionContainer, false);
                }

            }
        }

        private async void SaveStateClicked(string saveName, StackPanel stackPanel)
        {
            if (stackPanel.Name == "localSessionContainer")
            {
                string sessionName = localSaves.GetSessionName(saveName);
                string savePath = localSaves.GetSessionPath(saveName);

                try
                {
                    labelUploadLocal.Content = "Please wait until the upload finished.";
                    await driveUpload.UploadFileToDrive(sessionName, savePath, service);
                    BackRemote();
                    labelUploadLocal.Content = "Upload a local save state:";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error uploading file: " + ex.Message);
                }
            }
            if (stackPanel.Name == "remoteSessionContainer")
            {
                try
                {
                    labelDownloadRemote.Content = "Please wait until the download finished.";
                    await driveDownload.DownloadFileFromDrive(saveName, remoteSaves.SessionNameFolderID, service);
                    labelDownloadRemote.Content = "Download a remote save state:";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error downloading file: " + ex.Message);
                }
            }
        }

        private void ButtonBackRemote_Click(object sender, RoutedEventArgs e)
        {
            BackRemote();
        }

        private void BackRemote()
        {
            buttonBackRemote.Content = "Refresh";
            remoteSaves.GetFolderNames(service, Properties.Settings.Default.FolderID);
            remoteSessionContainer.Children.Clear();
            CreateBoxes(remoteSaves.SessionFolders.Distinct(), remoteSessionContainer, true);
        }

        private void ButtonBackLocal_Click(object sender, RoutedEventArgs e)
        {
            buttonBackLocal.Content = "Refresh";
            localSaves.SearchForSaves(Properties.Settings.Default.SaveLocation);
            localSessionContainer.Children.Clear();
            CreateBoxes(localSaves.SaveSessionNames.Distinct(), localSessionContainer, true);
        }

        private void ButtonResetSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SaveLocation = null;
            Properties.Settings.Default.PrivateKeyJSONPath = null;
            Properties.Settings.Default.FolderID = null;
            Properties.Settings.Default.Save();

            Process currentProcess = Process.GetCurrentProcess();
            Process.Start(currentProcess.MainModule.FileName);
            currentProcess.CloseMainWindow();
            currentProcess.Close();
        }
    }
}
