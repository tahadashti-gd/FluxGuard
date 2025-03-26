using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxGuard.Core.FLuxModules
{
    internal class FileExplorerModule
    {
        public static List<DirectoryInfo> GetDirectories(string path)
        {
            List<DirectoryInfo> folders = new List<DirectoryInfo>();

            if (Directory.Exists(path))
            {
                try
                {
                    foreach (string folder in Directory.GetDirectories(path))
                    {
                        folders.Add(new DirectoryInfo(folder));
                    }
                    LogService.LogServiceActivity("Fetched directories");
                }
                catch (Exception ex)
                {
                    LogService.LogError(ex, "GetDirectories");
                }
            }
            else
            {
                LogService.LogWarning("GetDirectories", "Path not found");
            }

            return folders;
        }

        public static List<FileInfo> GetFiles(string path)
        {
            List<FileInfo> files = new List<FileInfo>();

            if (Directory.Exists(path))
            {
                try
                {
                    foreach (var file in Directory.GetFiles(path))
                    {
                        files.Add(new FileInfo(file));
                    }
                    LogService.LogServiceActivity("Fetched files");
                }
                catch (Exception ex)
                {
                    LogService.LogError(ex, "GetFiles");
                }
            }
            else
            {
                LogService.LogWarning("GetFiles", "Path not found");
            }

            return files;
        }

        public static List<string> GetDrives()
        {
            List<string> drives = new List<string>();
            try
            {
                foreach (var drive in DriveInfo.GetDrives())
                {
                    drives.Add(drive.Name);
                }
                LogService.LogServiceActivity("Fetched drives");
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "GetDrives");
            }
            return drives;
        }
    }
}
