using System;
using System.IO;
using System.IO.Compression;

namespace ESWToolbox.FileIO
{
    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(ZipArchive archive, string destinationDirectoryName, bool overwrite, EventHandler<ZipFileArgs> on_change = null)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            int fileindex = 0;
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                Console.WriteLine(completeFileName);
                string directory = Path.GetDirectoryName(completeFileName);
                if(!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
                file.ExtractToFile(completeFileName, true);
                fileindex++;
                on_change?.Invoke(null, new ZipFileArgs(fileindex, archive.Entries.Count, file.Name));
            }
        }
        public class ZipFileArgs : EventArgs
        {
            public int FileIndex;
            public int ArchiveTotal;
            public string FileName;

            public ZipFileArgs(int index, int archiveTotal, string fileName)
            {
                FileIndex = index;
                ArchiveTotal = archiveTotal;
                FileName = fileName;
            }
        }
    }
}
