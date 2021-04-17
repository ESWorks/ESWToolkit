using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace ESWToolbox.Utility
{
    public class Copy
    {
        public delegate void IntDelegate(int Int);
        public delegate void VoidDelegate();
        private event IntDelegate FileCopyProgress;
        private event VoidDelegate FileCopyCompleted;

        public Copy()
        {
            
        }
        public WebClient CopyFileWithProgressAsync(string source, string destination, IntDelegate progress = null, VoidDelegate completed = null)
        {
            var webClient = new WebClient();
            FileCopyProgress = progress;
            FileCopyCompleted = completed;
            webClient.DownloadProgressChanged += DownloadProgress;
            webClient.DownloadFileCompleted += DownloadComplete;
            webClient.DownloadFileAsync(new Uri(source), destination);
            return webClient;
        }

        public void StopAsyncWebClient(WebClient wc)
        {
            wc?.CancelAsync();
        }
        public bool CopyFile(string source, string destination)
        {
            try
            {
                var webClient = new WebClient();
                webClient.DownloadFile(new Uri(source), destination);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            FileCopyCompleted?.Invoke();
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            FileCopyProgress?.Invoke(e.ProgressPercentage);
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
    }
}
