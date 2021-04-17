using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ESWToolbox.WebHost
{
    public class HTTPServer : IDisposable
    {
        
        public bool DataCommand;

        private static readonly IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
                #region extension to MIME type list
                {".asf", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".avi", "video/x-msvideo"},
                {".bin", "application/octet-stream"},
                {".cco", "application/x-cocoa"},
                {".crt", "application/x-x509-ca-cert"},
                {".css", "text/css"},
                {".deb", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dll", "application/octet-stream"},
                {".dmg", "application/octet-stream"},
                {".ear", "application/java-archive"},
                {".eot", "application/octet-stream"},
                {".exe", "application/octet-stream"},
                {".flv", "video/x-flv"},
                {".gif", "image/gif"},
                {".hqx", "application/mac-binhex40"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".ico", "image/x-icon"},
                {".img", "application/octet-stream"},
                {".iso", "application/octet-stream"},
                {".jar", "application/java-archive"},
                {".jardiff", "application/x-java-archive-diff"},
                {".jng", "image/x-jng"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/x-javascript"},
                {".mml", "text/mathml"},
                {".mng", "video/x-mng"},
                {".mov", "video/quicktime"},
                {".mp3", "audio/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpg", "video/mpeg"},
                {".msi", "application/octet-stream"},
                {".msm", "application/octet-stream"},
                {".msp", "application/octet-stream"},
                {".pdb", "application/x-pilot"},
                {".pdf", "application/pdf"},
                {".pem", "application/x-x509-ca-cert"},
                {".pl", "application/x-perl"},
                {".pm", "application/x-perl"},
                {".png", "image/png"},
                {".prc", "application/x-pilot"},
                {".ra", "audio/x-realaudio"},
                {".rar", "application/x-rar-compressed"},
                {".rpm", "application/x-redhat-package-manager"},
                {".rss", "text/xml"},
                {".run", "application/x-makeself"},
                {".sea", "application/x-sea"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".swf", "application/x-shockwave-flash"},
                {".tcl", "application/x-tcl"},
                {".tk", "application/x-tcl"},
                {".txt", "text/plain"},
                {".war", "application/java-archive"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wmv", "video/x-ms-wmv"},
                {".xml", "text/xml"},
                {".xpi", "application/x-xpinstall"},
                {".zip", "application/zip"},
                #endregion
            };

        private Thread _serverThread;
        private HttpListener _listener;
        public Dictionary<string,Action<HttpListenerContext>> EventDictionary = new Dictionary<string, Action<HttpListenerContext>>();
        private readonly Dictionary<long,Thread> _processThreads = new Dictionary<long,Thread>();
        public int Port { get; private set; }

        public static string MimeTypeFinder(string extension) => _mimeTypeMappings[extension];

        private readonly bool _instMode;

        public HTTPServer()
        {
            _instMode = true;
        }

        /// <summary>
        /// Only available on default constructor.
        /// </summary>
        public bool Start(int port, List<string> prefix = null)
        {
            if (!_instMode) return false;
            try
            {
                if (Port > 0)
                {
                    Initialize(port, prefix);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            

        }
        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public HTTPServer(int port)
        {
            Initialize(port);
        }

        /// <summary>
        /// Construct server with suitable port.
        /// </summary>
        public HTTPServer(bool publicService)
        {
            //get an empty port
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            if(!publicService) Initialize(port);
            else Initialize(port, new List<string>() { $"http://+:{port}/", $"http://*:{port}/" });
        }

       
        public HTTPServer(int port, List<string> prefix)
        {
            Initialize(port, prefix);
        }
        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            int len = _processThreads.Keys.Count;
            long[] ids = new long[len];
            _processThreads.Keys.CopyTo(ids, 0);

            foreach (long id in ids)
            {
                _processThreads[id].Join(5);
                _processThreads[id].Abort();
                _processThreads.Remove(id);
            }
            _serverThread.Abort();
            _listener.Stop();
        }

        private void Listen(List<string> prefixList = null)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://127.0.0.1:" + Port.ToString() + "/");
            if (prefixList != null)
            {
                foreach (string prefix in prefixList)
                {
                    _listener.Prefixes.Add(prefix);
                }
            }
            _listener.Start();

            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    long id = DateTime.Now.Ticks;
                    Thread thread = new Thread(() => Process(context, id));
                    _processThreads.Add(id, thread);
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void Process(HttpListenerContext context, long id)
        {
            string keyname = context.Request.Url.AbsolutePath;
            Console.WriteLine(keyname);
            Console.WriteLine(context.Request.Url);
            if (EventDictionary.ContainsKey(keyname))
            {
                EventDictionary[keyname]?.Invoke(context);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Flush();
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Close();
            }
            

            _processThreads.Remove(id);
            Thread.CurrentThread.Join(5);
            Thread.CurrentThread.Abort();
        }

        private void Initialize(int port, List<string> prefixList = null)
        {
            Port = port;
            _serverThread = new Thread(()=>Listen(prefixList)) {IsBackground = true};
            _serverThread.Start();
        }


        public void Dispose()
        {
            ((IDisposable) _listener)?.Dispose();
        }
    }
}


