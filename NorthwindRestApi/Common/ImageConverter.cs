using System.Collections;

namespace NorthwindRestApi.Common
{
    public static class ImageConverter
    {
        public static string ConvertToBase64(byte[]? image)
        {
            if (image == null)
                return null;
            using (var ms = new MemoryStream())
            {
                // JPEG header offset
                int offset = 78;
                // Skip the first 78 bytes
                ms.Write(image, offset, image.Length - offset);
                // Get the byte array without the header
                var byteArray = ms.ToArray();
                // Convert to base64 string
                var base64string = Convert.ToBase64String(byteArray);
                return base64string;
            }
        }

        public static byte[] AddNorthwindPictureHeader(byte[] imageBytes)
        {
            var header = new byte[78]; // täytä tarvittaessa oikealla Northwind-headerillä
            var result = new byte[header.Length + imageBytes.Length];

            Buffer.BlockCopy(header, 0, result, 0, header.Length);
            Buffer.BlockCopy(imageBytes, 0, result, header.Length, imageBytes.Length);

            return result;
        }
    }
}
