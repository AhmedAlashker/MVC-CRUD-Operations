using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Demo.PL.Helpers
{
    public static class DocumentSettings
    {
        // Upload     
        public static string UploadFile(IFormFile file, string FolderName)
        {

            // 1. Get Located Folder Path 
            string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            // 2. Get file Name and Make it uniqe
            //string FileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string FileName = $"{file.FileName}";

            // 3. Get File Path[Folder Path + FileName]
            string FilePath = Path.Combine(FolderPath, FileName);

            // 4. Save Fiile As Streams
            using var Fs = new FileStream(FilePath, FileMode.Create);
            file.CopyTo(Fs);

            // 5. Return File name
            return FileName;

        }

        // Delete
        public static void DeleteFile(string FileName, string FolderName)
        {
            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName, FileName);
            if(File.Exists(FilePath))
                File.Delete(FilePath);


        }

    }
}
