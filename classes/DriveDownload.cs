using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using System.IO;
using System.Windows.Forms;

namespace SatisfactorySavestateManager.classes
{
    public class DriveDownload
    {
        public async Task DownloadFileFromDrive(string saveName, string folderId, DriveService service)
        {
            var request = service.Files.List();
            request.Q = $"'{folderId}' in parents and name='{saveName}'";
            var result = await request.ExecuteAsync();
            if (result.Files.Count > 0)
            {
                var fileId = result.Files[0].Id;
                var requestDownload = service.Files.Get(fileId);
                using (var stream = new MemoryStream())
                {
                    await requestDownload.DownloadAsync(stream);
                    using (var fileStream = new FileStream(Properties.Settings.Default.SaveLocation + "\\" + saveName, FileMode.Create, FileAccess.Write))
                    {
                        stream.Position = 0;
                        stream.CopyTo(fileStream);
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: File not found!");
            }
        }
    }
}
