using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ephemera.NBagOfTricks.Slog;


namespace Embuddy
{
    /// <summary>
    /// 
    /// </summary>
    public class Telnet : Connection
    {
        #region Fields
        NetworkStream? _telnetStream = null;

        TcpClient? _telnetClient = null;

        readonly Logger _logger = LogManager.CreateLogger("Telnet");

        #endregion

        #region Properties

        #endregion

        #region Lifecycle
        /// <summary>
        /// Resource clean up.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            Close();

            base.Dispose(disposing);
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Connect to telnet server.
        /// </summary>
        public override void Open()
        {
            try
            {
                _telnetClient = new TcpClient(new IPEndPoint(IP, Port));
                _telnetStream = _telnetClient.GetStream();
            }
            catch
            {
                OnResponse(new ResponseEventArgs(ResponseCat.ERR, $"Failed while connecting to : {IP}:{Port}"));
            }
        }

        public override void Close()
        {
            _telnetClient?.Close();
            _telnetStream?.Close();
            _telnetClient?.Dispose();
            _telnetStream?.Dispose();
            _telnetClient = null;
            _telnetStream = null;
        }


        /// <summary>
        /// Send a command to telnet server.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override void Command(string command)
        {
            // Send command
            byte[] data = Encoding.ASCII.GetBytes(command);
            _telnetStream.Write(data, 0, data.Length);
            //Console.Write("Sent : {0}", command);

            // WaitShort(); TODO need a thread

            // Receive response.
            byte[] responseData = new byte[256];
            int numberOfBytesRead = _telnetStream.Read(responseData, 0, responseData.Length);
            string response = Encoding.ASCII.GetString(responseData, 0, numberOfBytesRead);

            OnResponse(new ResponseEventArgs(ResponseCat.RES, response));
        }

        #endregion

        #region Private functions


        #endregion
    }
}
