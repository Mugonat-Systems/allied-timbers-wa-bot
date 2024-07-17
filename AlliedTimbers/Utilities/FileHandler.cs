using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AlliedTimbers.Utilities;

public static class FileHandler
{
   public static string Upload(HttpPostedFileBase postedFile, string savePath = null,
  string fileType = null,
  bool compressIfImage = true)
{
    if (postedFile == null)
    {
        return "";
    }

    if (postedFile.ContentLength == 0) return null;

    //var fileExtension = postedFile.FileName.Split('.').Last().ToLower();
    var fileExtension = postedFile.FileName.Split('.').Last().ToLower();
    var guid = Guid.NewGuid().ToString();
    var newFileName = $"{guid}.{fileExtension}";
    var type = fileType ?? postedFile.ContentType.Split('/').FirstOrDefault();

    var path = $"/assets/uploads/{savePath ?? DateTime.Now.Year.ToString()}/{type}/";

    if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))
        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));

    var finalPath = HttpContext.Current.Server.MapPath(path) + newFileName;

    postedFile.SaveAs(finalPath);

    if (!postedFile.ContentType.Contains("image") || !compressIfImage) return $"{path}{newFileName}";

    var fileBytes = File.ReadAllBytes(finalPath);
    byte[] newFileBytes;
    var oldExtension = fileExtension;

    switch (fileExtension)
    {
        case "png":
            newFileBytes = ImageHandler.CompressPng(fileBytes);
            break;
        case "jpg":
        case "jpeg":
            newFileBytes = ImageHandler.CompressJpeg(fileBytes);
            break;
        default:
            newFileBytes = ImageHandler.CompressJpeg(ImageHandler.ToJpeg(fileBytes));
            break;
    }

    if (!new[] { "jpg", "jpeg", "png" }.Contains(fileExtension))
    {
        File.Delete(finalPath);
        fileExtension = "jpg";
        newFileName = newFileName.Replace($".{oldExtension}", $".{fileExtension}");
    }
    
    var fileName = finalPath.Replace($".{oldExtension}", $".{fileExtension}");

    File.WriteAllBytes(fileName, newFileBytes);
    
    var index = fileName.LastIndexOf('.');

    var resizedPath = finalPath.Replace($".", "resized.");

    var image = FileHandler.ResizeImage(256, 200, finalPath);
    image.Save($"{resizedPath}");
    
    return $"{path}{newFileName}";
}
   
   public static string FindImage(string source = "no source")
   {
       const string regexImgSrc = @"<img.*?src=""(?<url>.*?)"".*?>";
       var matchesImgSrc =
           Regex.Matches(source, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);

       var listOfImgData = (from Match m in matchesImgSrc select m.Groups[1].Value).ToList();

       return listOfImgData.FirstOrDefault() ?? "/Content/Img/bg_generic.png";
   }
   public static decimal GetFileSize(string fileName)
   {
       if (string.IsNullOrEmpty(fileName)) return default;

       var info = new FileInfo(HttpContext.Current.Server.MapPath(fileName));
       return info.Length;
   }
   public static string FormattedFileSize(long s) => FormattedFileSize(double.Parse(s.ToString()));
   public static string FormattedFileSize(double s)
   {
       string[] sizes = { "Bytes", "KB", "MB", "GB", "TB" };
       var order = 0;
       while (s >= 1024 && order < sizes.Length - 1)
       {
           order++;
           s /= 1024;
       }

       return $"{Math.Round(s, 2)} {sizes[order]}";
   }
   public static string SaveFile(string path, string fileName, string content)
   {
       if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))
           Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));

       File.WriteAllText(HttpContext.Current.Server.MapPath(path + fileName), content);

       return path + fileName;
   }
   public static string GetText(string path) => File.ReadAllText(HttpContext.Current.Server.MapPath(path));
   public static void Delete(string imagePath)
   {
       var serverPath = HttpContext.Current.Server.MapPath(imagePath);

       if (File.Exists(serverPath))
           File.Delete(serverPath);
   }

   public static string FileType(string fileName)
   {
       return fileName.Split('.').Last().ToLower();
   }
   
   /// <summary>
   /// Resize the image to the specified width and height.
   /// </summary>
   /// <param name="image">The image to resize.</param>
   /// <param name="width">The width to resize to.</param>
   /// <param name="height">The height to resize to.</param>
   /// <returns>The resized image.</returns>
   public static Bitmap ResizeImage(string filename, int width, int height)
   {

       var image = Image.FromFile(filename);

       var destRect = new Rectangle(0, 0, width, height);
       var destImage = new Bitmap(width, height);

       destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

       using (var graphics = Graphics.FromImage(destImage))
       {
           graphics.CompositingMode = CompositingMode.SourceCopy;
           graphics.CompositingQuality = CompositingQuality.HighQuality;
           graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
           graphics.SmoothingMode = SmoothingMode.HighQuality;
           graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

           using (var wrapMode = new ImageAttributes())
           {
               wrapMode.SetWrapMode(WrapMode.TileFlipXY);
               graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
           }
       }

       return destImage;
   }
   public static Image ResizeImage(int newWidth, int newHeight, string stPhotoPath)
   {
       Image imgPhoto = Image.FromFile(stPhotoPath);

       int sourceWidth = imgPhoto.Width;
       int sourceHeight = imgPhoto.Height;

       //Consider vertical pics
       if (sourceWidth < sourceHeight)
       {
           int buff = newWidth;

           newWidth = newHeight;
           newHeight = buff;
       }

       int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
       float nPercent = 0, nPercentW = 0, nPercentH = 0;

       nPercentW = ((float)newWidth / (float)sourceWidth);
       nPercentH = ((float)newHeight / (float)sourceHeight);
       if (nPercentH < nPercentW)
       {
           nPercent = nPercentH;
           destX = System.Convert.ToInt16((newWidth -
                                           (sourceWidth * nPercent)) / 2);
       }
       else
       {
           nPercent = nPercentW;
           destY = System.Convert.ToInt16((newHeight -
                                           (sourceHeight * nPercent)) / 2);
       }

       int destWidth = (int)(sourceWidth * nPercent);
       int destHeight = (int)(sourceHeight * nPercent);


       Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
           PixelFormat.Format24bppRgb);

       bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
           imgPhoto.VerticalResolution);

       Graphics grPhoto = Graphics.FromImage(bmPhoto);
       grPhoto.Clear(Color.Black);
       grPhoto.InterpolationMode =
           InterpolationMode.HighQualityBicubic;

       grPhoto.DrawImage(imgPhoto,
           new Rectangle(destX, destY, destWidth, destHeight),
           new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
           GraphicsUnit.Pixel);

       grPhoto.Dispose();
       return bmPhoto;
   }
   
   
   
   
}