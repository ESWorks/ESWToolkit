using System;
using System.IO;
using System.Net;

namespace ESWToolbox.WebHost
{
    public static class DataResponse
    {
        public static bool SendMimeData(string mimetype, HttpListenerContext context, Stream input, DateTime modifyDate)
        {
            try
            {
                //Adding permanent http response headers
                
                context.Response.ContentType = mimetype ?? "application/octet-stream";
                context.Response.ContentLength64 = input.Length;
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                context.Response.AddHeader("Last-Modified", modifyDate.ToString("r"));

                byte[] buffer = new byte[1024 * 16];
                int nbytes;
                while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    context.Response.OutputStream.Write(buffer, 0, nbytes);
                input.Close();

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Flush();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
        }
        public static bool SendMimeData(string mimetype, HttpListenerContext context, Stream input)
        {
            return SendMimeData(mimetype, context, input, DateTime.Now);
        }
        public static bool SendMimeData(string mimetype, HttpListenerContext context, string filename)
        {
            return File.Exists(filename) && SendMimeData(mimetype, context, File.OpenRead(filename), File.GetLastWriteTime(filename));
        }
    }
}
