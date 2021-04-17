using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ESWToolbox.Utility
{
    public class DirectoryHelper
    {
        public static long GetDirectorySize(string p, SearchOption option = SearchOption.AllDirectories)
        {
            return GetDirectoryFileInfo(p).Select(info => info.Length).Sum();
        }
        public static FileInfo[] GetDirectoryFileInfo(string directory, string search = "*.*", SearchOption option = SearchOption.AllDirectories)
        {
            List<FileInfo> files = new List<FileInfo>();
            RecurseDirectory(directory, search, option, files);
            return files.ToArray();
        }
        public static List<DirectoryInfo> GetDirectoryNames(string source)
        {
            List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>();
            RecurseDirectoryNames(source, directoryInfos);
            return directoryInfos;
        }

        private static void RecurseDirectoryNames(string current, List<DirectoryInfo> directoryList)
        {
            foreach (string directory in Directory.GetDirectories(current))
            {
                try
                {
                    directoryList.Add(new DirectoryInfo(directory));
                    RecurseDirectoryNames(directory, directoryList);
                }
                catch
                {
                    Console.WriteLine("Can't Access Directory.");
                }

            }
        }

        public static bool CopySourceBPB(string sourcefile, string copydirectory)
        {
            if (!File.Exists(sourcefile))
            {
                return false;
            }
            try
            {
                string copy = Path.Combine(copydirectory, new FileInfo(sourcefile).Name);

                FileStream fs1 = new FileStream(sourcefile, FileMode.Open, FileAccess.Read);
                FileStream fs2 = new FileStream(copy, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                fs1.Seek(0, SeekOrigin.Begin);
                fs2.Seek(0, SeekOrigin.Begin);
                do
                {
                    fs2.WriteByte((byte)fs1.ReadByte());
                }
                while (fs1.CanRead && fs2.CanWrite);

                // Close the files.
                fs1.Close();
                fs2.Close();

                return new FileInfo(sourcefile).Length == new FileInfo(copy).Length;
            }
            catch
            {
                return false;
            }

        }
        public delegate void ReportByte(long total, long copy);
        public static bool CopySource(string sourcefile, string copydirectory, ReportByte report, int buffersize = 4096)
        {
            if (!File.Exists(sourcefile))
            {
                return false;
            }
            try
            {
                string copy = Path.Combine(copydirectory, new FileInfo(sourcefile).Name);

                long total = new FileInfo(sourcefile).Length;
                
                FileStream fs1 = new FileStream(sourcefile, FileMode.Open, FileAccess.Read, FileShare.Read, buffersize);

                fs1.Seek(0, SeekOrigin.Begin);

                CopySection(fs1, copy, (int)total, buffersize, report);

                // Close the files.
                fs1.Close();


                return new FileInfo(sourcefile).Length == new FileInfo(copy).Length;
            }
            catch
            {
                return false;
            }

        }
        private static void CopySection(Stream input, string targetFile, int length, int buffersize, ReportByte report)
        {
            byte[] buffer = new byte[buffersize];
            int total = length;

            using (Stream output = File.OpenWrite(targetFile))
            {
                int bytesRead = 1;
                int totalBytes = 0;
                // This will finish silently if we couldn't read "length" bytes.
                // An alternative would be to throw an exception
                while (length > 0 && bytesRead > 0)
                {
                    int fil = Math.Min(length, buffer.Length);
                    bytesRead = input.Read(buffer, 0, fil);
                    output.Write(buffer, 0, bytesRead);
                    length -= bytesRead;

                    totalBytes += fil;
                    report(total, totalBytes);
                }
            }
        }
        public static void RecurseDirectory(string current, string filter, SearchOption option, List<FileInfo> fileInfos)
        {
            foreach (string file in Directory.GetFiles(current, filter))
            {
                try
                {
                    fileInfos.Add(new FileInfo(file));
                }
                catch
                {
                    Console.WriteLine("Can't Access File.");
                    // protected, ignore
                }
                
            }
            if (option == SearchOption.AllDirectories)
            {
                foreach (string subDir in Directory.GetDirectories(current))
                {
                    try
                    {
                        RecurseDirectory(subDir, filter, option, fileInfos);
                    }
                    catch
                    {
                        Console.WriteLine("Can't Access Directory.");
                        // protected, ignore
                    }
                }
            }
        }
    }
}
