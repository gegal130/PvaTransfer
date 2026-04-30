using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using FluentFTP;
using FluentFTP.Helpers;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;

namespace PvaTransfer
{
    /// <summary>
    /// Basisklasse (abstrakt) für PVA Verarbeitung.
    /// Implementiert Initialisierung (FTP) und Cleanup().
    /// Abgeleitete Klassen müssen Process() implementieren.
    /// </summary>
    public abstract class PvaProcessor : ProcessorBase
    {
        #region Constructor & Init

        public PvaProcessor(PvaConfig pvaConfig) 
            : base(pvaConfig)
        {
            Log($"FTP Connect {Config.RemoteUser}@_config.FtpHost{(Verbose ? $" ({Config.RemotePw})" : "")}");

            try
            {
                Ftp = new FtpClient(Config.RemoteHost,
                                    Config.RemoteUser,
                                    Config.RemotePw,
                                    config: new FtpConfig() { SelfConnectMode = FtpSelfConnectMode.Always });
            }
            catch (Exception e) 
            {
                Exception _ex = new Exception("Verbindung zum FTP Server nicht möglich", e);
                Log(_ex, false);
                throw _ex;
            }

            Log($"UEBERWEISUNGEN eingehend -> {Config.LocalDir}");
        }

        #endregion

        #region Processing & Cleanup

        public override void Cleanup()
        {
            Log("FTP Disconnect", 0);

            Ftp.Disconnect();
            Ftp.Dispose();
        }

        #endregion

        #region Storage & Config

        public FtpClient Ftp { get; private init; }

        #endregion

    }
}
