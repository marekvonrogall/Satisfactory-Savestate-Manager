using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using System.IO;

namespace SatisfactorySavestateManager.classes
{
    public class DriveUpload
    {
        public async Task UploadFileToDrive(string sessionName, string savePath, DriveService service)
        {
            string folderId = Properties.Settings.Default.FolderID;

            bool folderExists = false;
            string folderName = sessionName;
            var request = service.Files.List();
            request.Q = $"'{folderId}' in parents and mimeType='application/vnd.google-apps.folder' and name='{folderName}'";
            var result = await request.ExecuteAsync();
            if (result.Files.Count > 0)
            {
                folderExists = true;
                folderId = result.Files[0].Id;
            }

            if (!folderExists)
            {
                var folderMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new List<string> { folderId }
                };
                var folder = service.Files.Create(folderMetadata).Execute();
                folderId = folder.Id;
            }

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(savePath),
                Parents = new List<string> { folderId }
            };
            FilesResource.CreateMediaUpload requestUpload;
            using (var stream = new FileStream(savePath, FileMode.Open))
            {
                requestUpload = service.Files.Create(fileMetadata, stream, "application/octet-stream");
                requestUpload.Fields = "id";
                await requestUpload.UploadAsync();
            }
        }
    }
}
