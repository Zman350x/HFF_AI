using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace HFFinterface
{
    class PythonInterface
    {
        public static string str;

        private static IPHostEntry ipHost;
        private static IPAddress ipAddr;
        private static IPEndPoint localEndPoint;
        private static Task<Socket> clientSocket;

        public static bool init()
        {
            ipHost = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = new IPAddress( new byte[] { 0x00, 0x00, 0x00, 0x00 });
            localEndPoint = new IPEndPoint(ipAddr, 11111);
            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            str = "test message";
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                listener.Blocking = false;
                run_python("BepInEx\\plugins\\python\\main.py", ipAddr.MapToIPv4().ToString());
                clientSocket = listener.AcceptAsync();
            }
            catch (Exception e)
            {
                Shell.Print(e.ToString());
                return false;
            }
            return true;
        }

        private static void run_python(string prgm, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Users\\Temp\\AppData\\Local\\Programs\\Python\\Python310\\pythonw.exe";
            start.Arguments = string.Format("{0} {1}", prgm, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            Process.Start(start);
        }

        public static void listen()
        {
            string data = null;
            byte[] bytes;
            Socket mySocket;

            try
            {
                if (clientSocket.IsCompleted)
                {
                    mySocket = clientSocket.Result;
                }
                else
                {
                    Shell.Print("No socket connected.");
                    return;
                }
            }
            catch (Exception e)
            {
                Shell.Print(e.ToString());
                return;
            }

            if (mySocket != null)
            {
                bool avail = mySocket.Available > 0;
                if (!avail) { /*Shell.Print("No packets available.");*/  return; }

                while (avail)
                {
                    bytes = new byte[1024];
                    int bytesRec = mySocket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<E>") > -1)
                    {
                        data = data.Substring(0, data.IndexOf("<E>"));
                        //HFFInput.hffControl = 
                        byte[] message = Encoding.ASCII.GetBytes(str);
                        mySocket.Send(message);
                        break;
                    }
                    avail = mySocket.Available > 0;
                }
                Shell.Print(data);
            }
            else
            {
                Shell.Print("Socket is null");
            }
        }
    }
}
