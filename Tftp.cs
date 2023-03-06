using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tftp.Net;


namespace Embuddy
{
    /// <summary>
    /// 
    /// </summary>
    public class Tftp : Connection
    {
        #region Fields
        AutoResetEvent _tftpTransferFinishedEvent = new AutoResetEvent(false);
        #endregion

        #region Properties

        #endregion

        #region Lifecycle
        /// <summary>
        /// Resource clean up.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            _tftpTransferFinishedEvent?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Public functions
        public void Command(string msg)
        {
            //put - Transfers the file Source on the local computer to the file Destination on the remote computer.
            // Because the TFTP protocol does not support user authentication, the user must be logged onto the remote computer,
            // and the files must be writable on the remote computer.
            //get - Transfers the file Destination on the remote computer to the file Source on the local computer.
            //<Source> - Specifies the file to transfer.
            //<Destination> - Specifies where to transfer the file.
        }
        #endregion

        #region Private functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localFn">Local file name</param>
        /// <param name="remoteFn">Remote file name</param>
        void Upload(string localFn, string remoteFn)
        {
            var client = new TftpClient(IP, Port);

            using (MemoryStream ms = new())
            using (FileStream file = new(localFn, FileMode.Open, FileAccess.Read))
            {
                file.CopyTo(ms);

                var transfer = client.Upload(remoteFn);

                //Capture the events that may happen during the transfer
                transfer.OnProgress += Tftp_OnProgress;
                transfer.OnFinished += Tftp_OnFinshed;
                transfer.OnError += Tftp_OnError;

                // Start the transfer and read the data that we're uploading into a memory stream.
                transfer.Start(ms);

                // Wait for the transfer to finish
                _tftpTransferFinishedEvent.WaitOne();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localFn">Local file name</param>
        /// <param name="remoteFn">Remote file name</param>
        void Download(string localFn, string remoteFn)
        {
            var client = new TftpClient(IP, Port);

            using (MemoryStream ms = new())
            using (FileStream file = new(localFn, FileMode.CreateNew, FileAccess.Write))
            {
                // Prepare a simple transfer (GET ...)
                var transfer = client.Download(remoteFn);

                //Capture the events that may happen during the transfer
                transfer.OnProgress += Tftp_OnProgress;
                transfer.OnFinished += Tftp_OnFinshed;
                transfer.OnError += Tftp_OnError;

                // Start the transfer and write the data that we're downloading into a memory stream
                transfer.Start(ms);

                //Wait for the transfer to finish
                _tftpTransferFinishedEvent.WaitOne();

                // Write collected stream to local file.
                ms.CopyTo(file);
            }
        }
        #endregion


        void Tftp_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            OnResponse(new ResponseEventArgs(ResponseCat.INF, $"Transfer running. Progress: {progress}"));
        }

        void Tftp_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            OnResponse(new ResponseEventArgs(ResponseCat.ERR, $"Transfer failed: {error}"));
            _tftpTransferFinishedEvent.Set();
        }

        void Tftp_OnFinshed(ITftpTransfer transfer)
        {
            OnResponse(new ResponseEventArgs(ResponseCat.INF, $"Transfer succeeded."));
            _tftpTransferFinishedEvent.Set();
        }
    }
}
