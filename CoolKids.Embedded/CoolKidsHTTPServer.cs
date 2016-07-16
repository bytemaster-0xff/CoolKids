using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CoolKids.Uwp.Embedded
{
    public class CoolKidsHTTPServer
    {
        //int port = 8000;

        int port = 4005;
        private const uint BUFFER_SIZE = 8192;

        public void StartServer()
        {
            StreamSocketListener listener = new StreamSocketListener();
            listener.BindServiceNameAsync(port.ToString());
            Debug.WriteLine("Bound to port: " + port.ToString());
            listener.ConnectionReceived += async (s, e) =>
            {
                Debug.WriteLine("Got connection");

                var request = new StringBuilder();
                using (var input = e.Socket.InputStream)
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

                using (IOutputStream output = e.Socket.OutputStream)
                {
                    using (Stream response = output.AsStreamForWrite())
                    {
                        string[] lines = File.ReadAllLines("Assets\\index.html");
                        
                        string responseContent = "";

                        foreach (var line in lines)
                        {
                            responseContent += line;
                            responseContent += Environment.NewLine;
                        }



                        await WriteResponseAsync(output, 200, responseContent);
                    }
                }
            };
        }



        private async Task WriteResponseAsync(IOutputStream os, 
                                              int responseCode, 
                                              String resposneContent)
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
