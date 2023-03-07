using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Ephemera.NBagOfTricks.Slog;


namespace Embuddy
{
    /// <summary>
    /// Provides the functionality of sending and receiving reply to a remote server through a TCP Socket
    /// </summary>
    public class RawSocket : Connection
    {
        #region Fields
        private Socket? _server = null;
        byte[] _reply = new byte[0];
        readonly Logger _logger = LogManager.CreateLogger("RawSocket");

        #endregion

        #region Properties
        public int Timeout { get; set; }

        #endregion

        #region Lifecycle

        /// <summary>
        /// Resource clean up.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            Close();
            _server?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Public functions
        public override void Open()
        {
            try
            {
                _open();
                //_telnetClient = new TcpClient(new IPEndPoint(IP, Port));
                //_telnetStream = _telnetClient.GetStream();
            }
            catch
            {
                OnResponse(new ResponseEventArgs(ResponseCat.ERR, $"Failed while connecting to : {IP}:{Port}"));
            }
        }

        public override void Close()
        {
            _close();
        }

        /// <summary>
        /// Send a command to telnet server.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override void Command(string command)
        {
            // Send command

            var response = Send(command, true);

            //byte[] data = Encoding.ASCII.GetBytes(command);
            //_telnetStream.Write(data, 0, data.Length);
            ////Console.Write("Sent : {0}", command);

            //// WaitShort(); TODO need a thread

            //// Receive response.
            //byte[] responseData = new byte[256];
            //int numberOfBytesRead = _telnetStream.Read(responseData, 0, responseData.Length);
            //string response = Encoding.ASCII.GetString(responseData, 0, numberOfBytesRead);

            OnResponse(new ResponseEventArgs(ResponseCat.RES, response));
        }

        /// <summary>
        /// Send array of object to the server and get bytes[]
        /// </summary>
        /// <param name="messages">array of messages to send</param>
        /// <param name="delimeter">delimeter between each element in the message array (e.g. space,comma,...)</param>
        /// <param name="closeConnection">if true close connection after sending message.</param>
        /// <returns>reply from the server in bytes[]</returns>
        public byte[] Send(object[] messages, string delimeter, bool closeConnection)
        {
            const int TRUNCATE_ON = 200;

            if (_server == null || !_server.Connected)
            {
                Open();
            }

            // prepare data to send to server
            var al = new List<byte>();
            var bDelimeter = ToByteArray(delimeter);
            for (int x = 0; x < messages.Length; x++)
            {
                // add data
                byte[] ba = ToByteArray(messages[x]);
                foreach (byte b in ba)
                {
                    al.Add(b);
                }

                // add delimeter except for the last element
                if (x < (messages.Length - 1))
                {
                    foreach (var b in bDelimeter)
                    {
                        al.Add(b);
                    }
                }
            }

            // increase buffer if needed
            if(al.ToArray().Length > _server.SendBufferSize)
            {
                _server.SendBufferSize = al.ToArray().Length + 10;
                Debug.WriteLine("\tincreased buffer to "+_server.SendBufferSize);
            }

            // send data to server
            _server.Send(al.ToArray(),al.ToArray().Length,SocketFlags.None);

            // receive server's reply
            byte[] rawData = new byte[1024 * 5000];
            int dataLenght = _server.Receive(rawData);
            Debug.WriteLine("\tSocketClient::Send.dataRECEIVEDLength = " + dataLenght);

            // close the connection if closeIt is true
            if (closeConnection) { Close(); }

            // copy received data 
            var bar = rawData.Take(dataLenght);
            byte[] _reply = bar.ToArray();

            Debug.WriteLine("\tSocketClient::Send.RECEIVED=<" + ObjsToString(new object[]{_reply}, TRUNCATE_ON) + ">");
            return _reply;
        }

        /// <summary>
        /// Send data to server and receive the reply
        /// </summary>
        /// <param name="message">message to send</param>
        /// <param name="closeIt">true: close the conection after receiving the replay</param>
        /// <returns>server's reply</returns>
        public string Send(string message, bool closeIt)
        {
            Send(new Object[] { message }, " ", closeIt);
            return ReceivedReplyToString;
            /*
            byte[] ba = Send(new Object[] { message }, " ", closeIt);
            return Encoding.ASCII.GetString(ba);
            */
        }

        /// <summary>
        /// Send parameters to remote server, get a string reply from the server and close the connection.
        /// </summary>
        /// <param name="parameters">objects to send</param>
        /// <returns></returns>
        public byte[] Send(Object[] parameters)
        {
            Send(parameters, " ", true);
            //return Encoding.ASCII.GetString(rs);
            return ReceivedReplyToBytes;
        }

        /// <summary>
        /// Send data to server, receive the reply and close connection.
        /// </summary>
        /// <param name="message">message to send</param>
        /// <returns>server's reply</returns>
        public string Send(string message)
        {
            return Send(message, true);
        }


        #endregion

        #region Private functions


        #endregion




////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Get the reply [in bytes] afer the <see cref="Send(object[],string,bool)"/> method it's executed.
        /// </summary>
        public byte[] ReceivedReplyToBytes
        {
            get { return _reply; }
        }

        public string ReceivedReplyToString
        {
            get { return Encoding.ASCII.GetString(_reply).Replace("\0", "").Trim(); }
        }


        /// <summary>
        /// Open the connection 
        /// </summary>
        void _open()
        {
            // create object
            _server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Set timeouts
            _server.SendTimeout = Timeout;
            _server.ReceiveTimeout = Timeout;
            // start connection
            _server.Connect(IP, Port);
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        void _close()
        {
            if (_server != null && _server.Connected)
            {
                //_server.Shutdown(SocketShutdown.Both);
                _server.Close();
            }
        }

        /// <summary>
        /// Convert object (string / byte[]) type to byte[]
        /// </summary>
        /// <param name="obj">object should be a string or a byte[]</param>
        /// <returns></returns>
        private byte[] ToByteArray(Object obj)
        {
            byte[] ba = null;
            if (obj is string)
            {
                ba = Encoding.ASCII.GetBytes(obj.ToString());
            }
            else if(obj is object[])
            {
                List<byte> barr = new List<byte>();
                foreach (var b in (object[])obj)
                {
                    barr.AddRange(ToByteArray(b));
                }
                ba = barr.ToArray();
            }
            else if (obj.GetType() == typeof(byte[]))
            {
                return (byte[])obj;
            }
            else
            {
                ba = Encoding.ASCII.GetBytes(obj.ToString()); //check behavior with other types
            }
            return ba;
        }


        /// <summary>
        /// Convert array of object to string
        /// </summary>
        /// <param name="objs">array of objects</param>
        /// <param name="max">maximun number of char, -1 to print all</param>
        /// <returns>string</returns>
        public static string ObjsToString(object[] objs, int max)
        {
            const string SEPARATOR = " ";
            //
            var sb = new StringBuilder();
            foreach (var o in objs)
            {
                if (o is byte[])
                {
                    if (max > 0 && (((byte[])o).Length > max))
                    {
                        sb.Append(Encoding.ASCII.GetString((byte[])o, 0, max));
                        sb.Append("...~truncated");
                    }
                    else 
                    {
                        sb.Append(Encoding.ASCII.GetString((byte[])o));
                    }
                }
                else
                {
                    sb.Append(o);
                }
                if (!o.Equals(objs[objs.Length - 1]))
                    sb.Append(SEPARATOR);
            }
            return sb.ToString();
        }
    }


    /// <summary>
    /// Manage the TexTool commands and send the command to a remote server.
    /// </summary>
    internal class API_example
    {
        private readonly RawSocket _sock;
        private const string SUCCESSFULL_REPLY = "OK";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">host name</param>
        /// <param name="port">port number</param>
        public API_example(IPAddress ip, int port, int timeout)
        {
            _sock = new()
            {
                IP = ip,
                Port = port,
                Timeout = timeout
            };

            _sock.Open();
        }
        
        /// <summary>
        /// Run lua script on QNX and pull out the report
        /// </summary>
        /// <param name="localfile">path to copy the file</param>
        /// <param name="srvrfile">local lua script to run</param>
        /// <returns>test result (report) PATH</returns>
        public string RunLuaTestCase(string localfile, string srvrfile)
        {
            PriorTransferFilesPathCheck(ref localfile, ref srvrfile);

            if (string.IsNullOrEmpty(localfile))
                throw new Exception("RunLuaTestCase needs a valid localfile instead of <"+localfile+">");

            if (string.IsNullOrEmpty(srvrfile))
                srvrfile = "~/" + Path.GetFileName(localfile); //home directory

            // Report name
            var rptName = GetRemoteReportFilename(srvrfile);

            // Execute commands to put, load, run the script and get the report
            PutFileCmd(srvrfile, localfile);
            LoadfileCmd(srvrfile);
            ReportCmd(rptName);
            RunCmd();
            string tmp = GetLocalReportPath(srvrfile);

            // TODO do the waiting in a more elegant way (e.g. non-blocking)
            System.Threading.Thread.Sleep(5000); // wait for the report to be generated
            
            GetFileCmd(rptName, tmp); //get the report

            return tmp;
        }

        public string GetRemoteReportFilename(string srvrfile)
        {
            return Path.GetDirectoryName(srvrfile) + "\\" + Path.GetFileName(srvrfile).Replace(".", "_") + ".rpt";
        }

        public string GetLocalReportPath(string srvrfile)
        {
            return @"C:\tmp\" + Path.GetFileName(GetRemoteReportFilename(srvrfile));
        }

        /// <summary>
        /// Open socket coneection
        /// </summary>
        public void OpenCmd()
        {
            _sock.Open();
        }

        /// <summary>
        /// Close socket connection
        /// </summary>
        public void CloseCmd()
        {
            _sock.Close();
        }

        /// <summary>
        /// Set the report file name. Usually this is executed after loadfile and before run
        /// </summary>
        /// <param name="reportFileName"></param>
        /// <returns></returns>
        public string ReportCmd(string reportFileName)
        {
            return SendCommand(new object[] { "report", reportFileName });
        }

        /// <summary>
        /// Sends data to the remote server using the PutfileCmd
        /// </summary>
        /// <param name="srvrfile">remote path to send the file</param>
        /// <param name="localfile">local path of the file to be send</param>
        /// <returns></returns>
        public string PutFileCmd(string srvrfile, string localfile)
        {
            PriorTransferFilesPathCheck(ref localfile, ref srvrfile);

            var filecontents = FileToByteArray(localfile);
            var filelen = filecontents.Length;
            Debug.WriteLine(string.Format("putfile len: {0}", filelen));
            return SendCommand(new object[] { "putfile", srvrfile, filelen, filecontents});
        }

        /// <summary>
        /// Get file from server
        /// </summary>
        /// <param name="srvrfile">file path in the server</param>
        /// <param name="localfile">file path to be added in the local machine</param>
        /// <returns>status</returns>
        public string GetFileCmd(string srvrfile, string localfile)
        {
            PriorTransferFilesPathCheck(ref srvrfile, ref localfile);

            SendCommand(new object[] { "getfile", srvrfile });
            
            byte[] b = _sock.ReceivedReplyToBytes;
            var bs = Encoding.ASCII.GetString(b);

            //Debug.WriteLine(string.Format("getfile ({1}): <{0}>", bs, bs.Length));

            // get file size
            var start = SUCCESSFULL_REPLY.Length + 1;
            var end = bs.IndexOf(" ", start);
            int lenght = Int32.Parse(bs.Substring(start, end - start));

            // write file in local
            using (var fs = new FileStream(localfile, FileMode.Create, FileAccess.Write))
            {
                fs.Write(b, end + 1, lenght);
                fs.Flush();
                fs.Close();
            }
            return _sock.ReceivedReplyToString;
        }

        public string LoadfileCmd(string srvrfile)
        {
            _sock.Send(new object[] {"loadfile", srvrfile});
            return _sock.ReceivedReplyToString;
        }

        /// <summary>
        /// Load and Run a LUA script in an arbitrary shell command on the target.
        /// </summary>
        /// <param name="srvrfile">run [fileToLoad]</param>
        /// <returns></returns>
        public string RunCmd(string srvrfile)
        {
            // load file before run it
            if (!string.IsNullOrEmpty(srvrfile))
            {
                LoadfileCmd(srvrfile); //load file to memory
                ReportCmd(srvrfile + ".rpt"); // set the report name
            }

            // just run the already loaded file (if any)
            var t = _sock.Timeout; // backup current timeout
            _sock.Timeout = 30000; // set timeout to run scripts
            SendCommand(new object[] { "run" }); // run script
            _sock.Timeout = t; // restore the normal timeout.

            return _sock.ReceivedReplyToString;
        }

        /// <summary>
        /// Run (already loaded lua script) in an arbitrary shell command on the target.
        /// </summary>
        /// <param name="cmd">run</param>
        /// <returns></returns>
        public string RunCmd()
        {
            return RunCmd(null);
        }

        /// <summary>
        /// Run an arbitrary command on the target. 
        /// </summary>
        /// <param name="cmdline">Command to execute in the remote machine</param>
        /// <remarks>rcmd [cmdline]. Run an arbitrary command on the target. Returns stdout as: OK 123 b1b2b3...bN where 123 is the returned text length and b1 etc are the text bytes.</remarks>
        /// <returns>stdout from the remote machine</returns>
        public string rcCmd(object[] parameters)
        {
            _sock.Send(new object[] { "rcmd", parameters });
            return _sock.ReceivedReplyToString;
        }

        public string PingCmd(string p)
        {
            return _sock.Send(p);
            //return _sock.ReceivedReplyToBytes;
        }


        /// <summary>
        /// Function to get byte array from a file
        /// </summary>
        /// <param name="fileName">File name to get byte array</param>
        /// <returns>Byte Array</returns>
        public static byte[] FileToByteArray(string fileName)
        {
            byte[] buffer = null;

            // Open file for reading
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            // attach filestream to binary reader
            var binaryReader = new BinaryReader(fileStream);

            // get total byte length of the file
            var totalBytes = new FileInfo(fileName).Length;

            // read entire file into buffer
            buffer = binaryReader.ReadBytes((Int32)totalBytes);

            // close file reader
            fileStream.Close();
            fileStream.Dispose();
            binaryReader.Close();

            return buffer;
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="path">convert file to string</param>
        /// <returns></returns>
        public static string GetFile(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Put file / Create file from string
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="content">file content</param>
        public static void PutFile(string path, string content)
        {
            using (TextWriter tw = new StreamWriter(path))
            {
                tw.Write(content);
            }
        }

        /// <summary>
        /// Compare files
        /// </summary>
        /// <param name="file1">file 1</param>
        /// <param name="file2">file 2</param>
        /// <returns>true if equals</returns>
        public static bool FileEquals(string file1, string file2)
        {
            return GetFile(file1).Equals(GetFile(file2));
        }

        /// <summary>
        /// Check the destination and source file paths.
        /// </summary>
        /// <remarks>If the source just have a path (without filename) it will copy the file with the same filename of the source.</remarks>
        /// <param name="source">file name+path to be transfered</param>
        /// <param name="destination">location where the "source" will be copied to.</param>
        public void PriorTransferFilesPathCheck(ref string source, ref string destination)
        {
            //clean it
            source = source.Trim();
            destination = destination.Trim();

            // check if destination has a file name
            var destFilename = Path.GetFileName(destination);
            if (destFilename == "")
            {
                // get source filename
                destination += Path.GetFileName(source);
            }
        }

        /// <summary>
        /// Check the reply from the server. If it was successfull should start with OK, otherwise will throw an error.
        /// </summary>
        /// <param name="serverReply"></param>
        /// <returns></returns>
        public string SendCommand(object[] command)
        {
            Debug.WriteLine(command[0] + ": args=" + command.Length);

            _sock.Send(command);
            var serverReply = _sock.ReceivedReplyToString; //Optimization opportunity: just read the first SUCCESSFULL_REPLY.length bytes instead of the whole thing...

            if (serverReply.StartsWith(SUCCESSFULL_REPLY))
            {
                return serverReply;
            }

            throw new Exception("Invalid server reply or timeout. Server Replied=<"+serverReply+">");
        }
    }
}
