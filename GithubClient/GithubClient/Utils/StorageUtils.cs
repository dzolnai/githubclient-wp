using GithubClient.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GithubClient.Utils
{
    public class StorageUtils
    {
        private static IsolatedStorageFile isoFile;
        private static DataContractSerializer fileSerializer = new DataContractSerializer(typeof(DownloadedFile));

        private static IsolatedStorageFile IsoFile
        {
            get
            {
                if (isoFile == null)
                {
                    isoFile = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                }
                return isoFile;
            }
        }

        /**
         * Use this method to save a file to the isolated storage.
         */
        public static void SaveFile(DownloadedFile file)
        {
            SaveFileToDirectory(file, "files");
        }

        /**
         * Use this method to save a repo to the isolated storage.
         */
        public static void SaveRepo(DownloadedFile repo)
        {
            SaveFileToDirectory(repo, "repos");
        }

        /**
         * Saves the file to the directory provided in the parameters.
         */
        private static void SaveFileToDirectory(DownloadedFile file, string directory)
        {
            string fileName = GetFileName(file);
            string path = String.Format("{0}/{1}.dat", directory, fileName);
            if (!IsoFile.DirectoryExists(directory))
            {
                IsoFile.CreateDirectory(directory);
            }
            try
            {
                using (var targetFile = IsoFile.CreateFile(path))
                {
                    fileSerializer.WriteObject(targetFile, file);
                }
                Debugger.Log(0, "Serialization", "File written to: " + path + "\n");
            }
            catch (Exception e)
            {
                Debugger.Log(0, "Serialization", e.Message + "\n");
                IsoFile.DeleteFile(fileName);
            }
        }


        public static List<DownloadedFile> GetAllRepos()
        {
            List<DownloadedFile> result = new List<DownloadedFile>();
            if (!IsoFile.DirectoryExists("repos"))
            {
                return result;
            }
            string[] repoFiles = IsoFile.GetFileNames("repos\\*.*");
            foreach (var repoFile in repoFiles)
            {
                using (var sourceStream = IsoFile.OpenFile("repos\\" + repoFile, FileMode.Open))
                {
                    result.Add((DownloadedFile)fileSerializer.ReadObject(sourceStream));
                }
            }
            foreach (var repo in result)
            {
                Debugger.Log(0, "", repo.Name + "\n");
            }
            return result;
        }

        /**
         * Converts the filename to a storage-compatible one.
         */
        private static string GetFileName(DownloadedFile file)
        {
            string name = file.Url;
            name = name.Replace("/", "_");
            name = name.Replace("\\", "_");
            name = name.Replace("=", "_");
            name = name.Replace(":", "_");
            name = name.Replace(" ", "_");
            name = name.Replace("?", "_");
            name = name.Replace("&", "_");
            return name;
        }

    }
}
