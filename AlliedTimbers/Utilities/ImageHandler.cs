using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using nQuant;

namespace AlliedTimbers.Utilities;

public static class ImageHandler
{
    public static byte[] ToJpeg(byte[] data)
    {
        try
        {
            using (var inStream = new MemoryStream(data))
            using (var outStream = new MemoryStream())
            {
                var imageStream = Image.FromStream(inStream);
                imageStream.Save(outStream, ImageFormat.Jpeg);
                return outStream.ToArray();
            }
        }
        catch
        {
            return data;
        }
    }

    public static byte[] CompressJpeg(byte[] data) => Compress(data, ImageFormat.Jpeg);

    public static byte[] CompressPng(byte[] data)
    {
        using var inStream = new MemoryStream(data);
        using var outStream = new MemoryStream();
        using (var quantized = new WuQuantizer().QuantizeImage(new Bitmap(inStream)))
        {
            quantized.Save(outStream, ImageFormat.Png);
        }

        return outStream.ToArray();
    }

    private static byte[] Compress(byte[] data, ImageFormat format)
    {
        var encoder = GetEncoder(format);
        try
        {
            using (var inStream = new MemoryStream(data))
            using (var outStream = new MemoryStream())
            {
                var image = Image.FromStream(inStream);

                // if we aren't able to retrieve our encoder
                // we should just save the current image and
                // return to prevent any exceptions from happening
                if (encoder == null)
                {
                    image.Save(outStream, format);
                }
                else
                {
                    var encoderParameters = new EncoderParameters(1)
                    {
                        Param =
                        {
                            [0] = new EncoderParameter(Encoder.Quality, 50L)
                        }
                    };
                    image.Save(outStream, encoder, encoderParameters);
                }

                return outStream.ToArray();
            }
        }
        catch
        {
            return data;
        }
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        var codecs = ImageCodecInfo.GetImageDecoders();
        return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
    }
}