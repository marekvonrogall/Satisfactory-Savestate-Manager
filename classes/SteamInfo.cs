using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;

namespace SatisfactorySavestateManager.classes
{
    public class SteamInfo
    {
        public string DefaultSavePath { get { return Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FactoryGame", "Saved", "SaveGames"); } }

        private string _profilePicUrl = null;
        private string _displayName = null;
        private string _steam64ID;
        public string Steam64ID { get { return _steam64ID; } }
        public string ProfilePicUrl { get { return _profilePicUrl; }}
        public string DisplayName { get { return _displayName; }}

        public bool GetSteamProfileInfo()
        {
            string profileUrl = $"https://steamcommunity.com/profiles/{Steam64ID}";
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(profileUrl);

                // Extract profile picture URL
                var profilePicMeta = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                if (profilePicMeta != null)
                    _profilePicUrl = profilePicMeta.GetAttributeValue("content", "");

                // Extract display name
                var displayNameNode = doc.DocumentNode.SelectSingleNode("//span[@class='actual_persona_name']");
                if (displayNameNode != null)
                    _displayName = displayNameNode.InnerText.Trim();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void GetSteam64ID()
        {
            string[] folders = Directory.GetDirectories(DefaultSavePath);

            foreach (string folder in folders)
            {
                string folderName = new DirectoryInfo(folder).Name;
                if (folderName.All(char.IsDigit))
                {
                    _steam64ID = folderName;
                }
            }
        }
    }
}
