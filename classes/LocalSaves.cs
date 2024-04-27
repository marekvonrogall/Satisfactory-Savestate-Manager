using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SatisfactorySavestateManager.classes
{
    public class LocalSaves
    {
        private List<string> _savePaths = new List<string>();
        private List<string> _saveNames = new List<string>();
        private List<string> _saveSessionNames = new List<string>();

        public List<string> SavePaths { get { return _savePaths; } }
        public List<string> SaveNames { get { return _saveNames; } }
        public List<string> SaveSessionNames { get { return _saveSessionNames; } }

        public List<string> GetMatchingSessionNames(string sessionName)
        {
            List<string> matchingSessionNames = new List<string>();

            for (int i = 0; i < SaveSessionNames.Count; i++)
            {
                if (SaveSessionNames[i] == sessionName)
                {
                    matchingSessionNames.Add(SaveNames[i]);
                }
            }

            return matchingSessionNames;
        }

        public void SearchForSaves(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath, "*.sav");
            string pattern = @"sessionName=([^?]+)";

            var sortedFiles = files.Select(file => new FileInfo(file))
                                   .OrderByDescending(fileInfo => fileInfo.LastWriteTime)
                                   .Select(fileInfo => fileInfo.FullName);

            foreach (string file in sortedFiles)
            {
                string fileName = Path.GetFileName(file);
                string sessionName = "";

                string[] lines = File.ReadLines(file).Take(10).ToArray();

                foreach (string line in lines)
                {
                    Match match = Regex.Match(line, pattern);
                    if (match.Success)
                    {
                        sessionName = match.Groups[1].Value;
                        break;
                    }
                }

                _savePaths.Add(file);
                _saveNames.Add(fileName);
                _saveSessionNames.Add(sessionName);
            }
        }

        public string GetSessionName(string saveName)
        {
            int index = SaveNames.IndexOf(saveName);
            if (index != -1 && index < SaveSessionNames.Count)
            {
                return SaveSessionNames[index];
            }
            return null;
        }

        public string GetSessionPath(string saveName)
        {
            int index = SaveNames.IndexOf(saveName);
            if (index != -1 && index < SavePaths.Count)
            {
                return SavePaths[index];
            }
            return null;
        }
    }
}
