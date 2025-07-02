namespace Pizzashop.Web.Utils;

public static class FileUploadHandler
{
    public static async Task<string> UploadImage(IFormFile? imageFile)
    {
        try
        {
            if (imageFile == null)
            {
                return string.Empty;
            }

            string rootpath = Directory.GetCurrentDirectory();

            string path = Path.Combine(rootpath, "wwwroot/Uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Check the allowed extensions
            string ext = Path.GetExtension(imageFile.FileName);
            string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            if (!imageFile.ContentType.StartsWith("image/"))
            {
                throw new ArgumentException($"Only images are allowed.");
            }

            // generate a unique filename
            string uniquename = $"{Guid.NewGuid()}{fileName}{ext}";
            string fileNameWithPath = Path.Combine(path, uniquename);
            using FileStream stream = new(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return uniquename;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public static void DeleteFile(string? fileNameWithExtension)
    {
        try
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }

            string rootpath = Directory.GetCurrentDirectory();

            string path = Path.Combine(rootpath, "wwwroot/Uploads", fileNameWithExtension);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Invalid file path");
            }
            File.Delete(path);
        }
        catch (Exception)
        {
            return;
        }
    }
}
