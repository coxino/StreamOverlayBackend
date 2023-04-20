using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coverlay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpGet("DatabaseFiles")]
        public List<string> DatabaseFiles([FromQuery]string userName)
        {
            var files = new List<string>();
            DirectoryInfo source = new DirectoryInfo(Settings.ProjectSettings.DatabaseFolder);
            
            files.AddRange(GetFilesRecursive(source));
            
            return files;
        }

        [HttpGet("DeleteFolder")]
        public string DeleteFolder([FromQuery] string userName, [FromQuery] string folder)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.ProjectSettings.DatabaseFolder + userName + "/" + folder);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            return "Deleted";
        }

        private List<string> GetFilesRecursive(DirectoryInfo source)
        {
            var files = new List<string>();
            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                files.Add(fi.FullName);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                files.AddRange(GetFilesRecursive(diSourceSubDir));
            }

            return files;
        }
    }
}
