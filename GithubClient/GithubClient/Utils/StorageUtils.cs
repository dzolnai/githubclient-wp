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
            string fileName = GetFileName(file.Url);
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

        /**
         * Get the files for all the repos stored on the isolated storage.
         */
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
         * Get a file by its URL on the isolated storage
         */
        public static DownloadedFile GetFileByUrl(string url)
        {
            string fileName = GetFileName(url) + ".dat";
            Debugger.Log(0, "Data", "Getting: " + fileName + "\n");
            try
            {
                if (!IsoFile.DirectoryExists("files") || !IsoFile.FileExists("files\\" + fileName))
                {
                    // it might be a repo
                    if (!IsoFile.DirectoryExists("repos") || !IsoFile.FileExists("repos\\" + fileName))
                    {
                        return null;
                    }
                    else
                    {
                        using (var sourceStream = IsoFile.OpenFile("repos\\" + fileName, FileMode.Open))
                        {
                            return (DownloadedFile)fileSerializer.ReadObject(sourceStream);
                        }
                    }
                }
                using (var sourceStream = IsoFile.OpenFile("files\\" + fileName, FileMode.Open))
                {
                    return (DownloadedFile)fileSerializer.ReadObject(sourceStream);
                }
            }
            catch (PathTooLongException ex)
            {
                return null;
            }
        }

        public static void DeleteFilesFromRoot(DownloadedFile root)
        {
            if (root == null)
            {
                Debugger.Log(0, "Data", "Can't delete file!");
                return;
            }
            if (root.ContainsFiles != null)
            {
                foreach (string url in root.ContainsFiles)
                {
                    DownloadedFile subFile = GetFileByUrl(url);
                    DeleteFilesFromRoot(subFile);
                }
                // delete this file
                string fileName = GetFileName(root.Url) + ".dat";
                if (!IsoFile.DirectoryExists("files") || !IsoFile.FileExists("files\\" + fileName))
                {
                    // it might be a repo
                    if (!IsoFile.DirectoryExists("repos") || !IsoFile.FileExists("repos\\" + fileName))
                    {
                        // Don't do anything
                        return;
                    }
                    else
                    {
                        IsoFile.DeleteFile("repos\\" + fileName);
                        return;
                    }
                }
                // if it exists, delete it.
                IsoFile.DeleteFile("files\\" + fileName);
                return;

            }
        }

        /**
         * Converts the filename to a storage-compatible one.
         */
        private static string GetFileName(string url)
        {
            string name = url;
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
