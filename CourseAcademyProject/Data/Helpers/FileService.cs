using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;

namespace CourseAcademyProject.Data.Helpers
{
    public static class FileService
    {
        public static string СreateFilePathFromFileName(string fileName,string foldername)
        {
            if (fileName.Contains("\\"))
            {
                fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
            }
            return $"/{foldername}/" + Guid.NewGuid().ToString() + fileName;
        }
        public static async Task SaveFile(string filePath, IFormFile file, IWebHostEnvironment appEnvironment)
        {
            using (var fileStream = new FileStream(appEnvironment.WebRootPath + filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }
      
    }
}
