using System.IO;

namespace WpfApplication.Tools
{
    public class PathToDir
    {
        public static string CalculatePath(string file)
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string resPath = Path.Combine(exeDir, file);
            resPath = Path.GetFullPath(resPath);
            return resPath;
        }
    }
}
