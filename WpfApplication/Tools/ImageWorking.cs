using System.IO;

namespace WpfApplication.Tools
{
    public class ImageWorking
    {
        public static byte[] ConvertImageToByteArray(string filename)
        {
            byte[] imageData;
            string pathToFile = PathToDir.CalculatePath(filename);
            using (FileStream fs = new FileStream(pathToFile, FileMode.Open))
            {
                imageData = new byte[fs.Length];
                fs.Read(imageData, 0, imageData.Length);
                fs.Close();
            }
            return imageData;
        }
    }
}
