using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO.Packaging;

namespace SatisfactorySavestateManager.classes
{
    public class Config
    {
        public Credentials credentials = new Credentials();
        private string _driveCredentialsPath;
        private string _saveStatePath;
        public string SaveStatePath { get { return _saveStatePath; } }
        public string DriveCredentialsPath { get { return _driveCredentialsPath; } }

        public bool IsPrivateKeySet()
        {
            if (Properties.Settings.Default.PrivateKeyJSONPath == null || !File.Exists(Properties.Settings.Default.PrivateKeyJSONPath))
            {
                return false;
            }
            else return true;
        }

        public void SelectJsonFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                _driveCredentialsPath = openFileDialog.FileName;
                Properties.Settings.Default.PrivateKeyJSONPath = DriveCredentialsPath;
                Properties.Settings.Default.Save();
            }
            else
            {
                _driveCredentialsPath = null;
                Properties.Settings.Default.PrivateKeyJSONPath = DriveCredentialsPath;
                Properties.Settings.Default.Save();
            }
        }

        public bool AreCredentialsValid()
        {
            return credentials.AreCredentialsValid(_driveCredentialsPath);
        }

        public void SelectSaveStatePath()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            folderBrowserDialog.RootFolder = Environment.SpecialFolder.LocalApplicationData;
            folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;
                _saveStatePath = selectedFolder;
                Properties.Settings.Default.SaveLocation = SaveStatePath;
                Properties.Settings.Default.Save();
            }
            else
            {
                _saveStatePath = null;
                Properties.Settings.Default.SaveLocation = SaveStatePath;
                Properties.Settings.Default.Save();
            }
        }

        public bool IsSaveStatePathValid()
        {
            if (SaveStatePath == null) { return false; }
            else return true;
        }
    }
}
