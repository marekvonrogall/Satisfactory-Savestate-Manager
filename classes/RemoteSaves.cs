using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace SatisfactorySavestateManager.classes
{
    public class RemoteSaves
    {
        private List<string> _sessionFolders = new List<string>();
        private List<string> _saveNames = new List<string>();
        public List<string> SessionFolders { get { return _sessionFolders; } }
        public List<string> SaveNames { get { return _saveNames; } }
        private string _sessionNameFolderID;
        public string SessionNameFolderID { get { return _sessionNameFolderID; } }

        public DriveService AuthenticateServiceAccount(string jsonPath)
        {
            GoogleCredential credential;

            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(new[] { DriveService.Scope.Drive });
            }

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Satisfactory Savestate Manager",
            });
        }

        public void GetFolderNames(DriveService service, string folderId)
        {
            var folders = new List<string>();
            var listRequest = service.Files.List();
            listRequest.Q = $"'{folderId}' in parents and mimeType = 'application/vnd.google-apps.folder'";
            listRequest.Fields = "files(name)";
            var files = listRequest.Execute().Files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    folders.Add(file.Name);
                }
            }

            _sessionFolders = folders;
        }

        public void GetSaveStateNames(DriveService service, string folderName)
        {
            var saveStateNames = new List<string>();
            var folderId = GetFolderIdByName(service, folderName);
            _sessionNameFolderID = folderId;
            if (folderId == null)
            {
                Console.WriteLine($"Folder '{folderName}' not found.");
                _saveNames = saveStateNames;
            }
            var listRequest = service.Files.List();
            listRequest.Q = $"'{folderId}' in parents";
            listRequest.Fields = "files(name, modifiedTime)";
            listRequest.OrderBy = "modifiedTime asc";

            var files = listRequest.Execute().Files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    saveStateNames.Add(file.Name);
                }
            }
            _saveNames = saveStateNames;
        }

        private string GetFolderIdByName(DriveService service, string folderName)
        {
            var listRequest = service.Files.List();
            listRequest.Q = $"name = '{folderName}' and mimeType = 'application/vnd.google-apps.folder'";
            listRequest.Fields = "files(id)";

            var files = listRequest.Execute().Files;
            if (files != null && files.Count > 0)
            {
                return files[0].Id;
            }
            return null;
        }
    }
}
