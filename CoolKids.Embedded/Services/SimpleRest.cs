using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CoolKids.Uwp.Embedded.Services
{
    public abstract class SimpleRest
    {
        private const uint BUFFER_SIZE = 8192;
        private readonly StreamSocketListener _streamSocketListener;

        protected SimpleRest()
        {
            _streamSocketListener = new StreamSocketListener();
            _streamSocketListener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
        }

        public int Port { get; set; }


        public void StartServer()
        {
#pragma warning disable CS4014
            _streamSocketListener.BindServiceNameAsync(Port.ToString());
#pragma warning restore CS4014
        }

        public void Dispose()
        {
            _streamSocketListener.Dispose();
        }

        private async void ProcessRequestAsync(StreamSocket socket)
        {
            var request = new StringBuilder();
            using (var input = socket.InputStream)
            {
                var data = new byte[BUFFER_SIZE];
                var buffer = data.AsBuffer();
                var dataRead = BUFFER_SIZE;
                while (dataRead == BUFFER_SIZE)
                {
                    await input.ReadAsync(buffer, BUFFER_SIZE, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            using (var output = socket.OutputStream)
            {
                if (request.ToString().ToLower().Contains("favicon"))
                    await WriteResponseAsync(output, 404, "NOT FOUND");
                else
                {
                    var requestMethod = request.ToString().Split('\n')[0];
                    var requestParts = requestMethod.Split(' ');

                    var resposneContent = Process(requestParts[1]);

                    if (requestParts[0] == "GET")
                        await WriteResponseAsync(output, 200, resposneContent);
                    else
                        throw new InvalidDataException("HTTP method not supported: " + requestParts[0]);
                }
            }
        }

        public abstract String Process(String queryString);

        private async Task WriteResponseAsync(IOutputStream os, int responseCode, String resposneContent)
        {
            using (var resp = os.AsStreamForWrite())
            {
                var bodyArray = Encoding.UTF8.GetBytes(resposneContent);
                using (var stream = new MemoryStream(bodyArray))
                {
                    var header = String.Format("HTTP/1.1 {0} OK\r\n" +
                                      "Content-Length: {1}\r\n" +
                                      "Connection: close\r\n\r\n",
                                      responseCode, stream.Length);
                    var headerArray = Encoding.UTF8.GetBytes(header);
                    await resp.WriteAsync(headerArray, 0, headerArray.Length);
                    await stream.CopyToAsync(resp);
                    await resp.FlushAsync();
                }
            }
        }
    }
}
