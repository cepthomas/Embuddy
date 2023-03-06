using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Embuddy
{
    /// <summary>
    /// 
    /// </summary>
    public class Telnet : Connection
    {
        #region Fields
        NetworkStream? _telnetCliStream = null;

        TcpClient? _telnetCliClient = null;
        #endregion

        #region Properties

        #endregion

        #region Lifecycle
        /// <summary>
        /// Connect to cli telnet server.
        /// </summary>
        public void Init()
        {
            try
            {
                _telnetCliClient = new TcpClient(new IPEndPoint(IP, Port));
                _telnetCliStream = _telnetCliClient.GetStream();
            }
            catch
            {
                OnResponse(new ResponseEventArgs(ResponseCat.ERR, $"Failed while connecting to : {IP}:{Port}"));
            }
        }

        /// <summary>
        /// Resource clean up.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            _telnetCliClient?.Dispose();
            _telnetCliStream?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Public functions
        public void Command(string msg)
        {

        }

        #endregion

        #region Private functions
        /// <summary>
        /// Send a command to cli telnet server.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string SendMessage_telnet(string command)
        {
            // Send command
            byte[] data = Encoding.ASCII.GetBytes(command);
            _telnetCliStream.Write(data, 0, data.Length);
            //Console.Write("Sent : {0}", command);

            WaitShort();

            // Receive response
            byte[] responseData = new byte[256];
            int numberOfBytesRead = _telnetCliStream.Read(responseData, 0, responseData.Length);
            string response = Encoding.ASCII.GetString(responseData, 0, numberOfBytesRead);

            response = ParseData(response);

            if (response == "SEND_COMMAND_AGAIN")
            {
                //if (DEBUG) Console.WriteLine("[ReadMessage] : Error Retreiving data. Send command again.");
            }
            //if (DEBUG) Console.WriteLine("Received : {0}", response);

            return response;
        }

        string ParseData(string response)
        {
            return response;
        }

        void WaitShort()
        {
            for (int i = 0; i < 1536; i++)
            {
                int x = i;
            }
        }

        #endregion
    }
}
