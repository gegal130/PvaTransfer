using FluentFTP;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvaTransfer
{
    /// <summary>
    /// Klasse für die Verarbeitung von Überweisungen (eingehend) der PVA.
    /// Implementiert Process().
    /// </summary>
    /// <remarks>
    /// Instantiiert den Prozessor für eingehende PVA Überweisungen
    /// </remarks>
    /// <param name="config">Konfiguration (FTP Zugang, Verzeichnisse)</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Spellchecker", "CRRSP08:A misspelled word has been found", Justification = "<Ausstehend>")]
    public class PvaÜberweisungEinProcessor(PvaConfig config) : PvaProcessor(config)
    {
        public override string ProcessingId => "PvaUebEin";

        #region Verarbeitung

        /// <summary>
        /// Verarbeitung der von der PVA eingehenden Überweisungen
        /// </summary>
        public override void Process()
        {
            int _tarGzCount = 0;
            int _uebEinCount = 0;

            FtpListItem[] _ftpAllFiles = Ftp.GetListing(Config.RemoteDir);
            FtpListItem[] _ftpPvaUebEinGzItems = [.. _ftpAllFiles.Where(fi => fi.Name.StartsWith("BABILD") && fi.Name.EndsWith(".tar.gz"))];
            
            LogIndent = 2;
            Log($"ARCHIVE {Config.RemoteDir} (FTP Server):");

            foreach (FtpListItem pvaUebEinGzItem in _ftpPvaUebEinGzItems)
            {
                LogIndent = 5;
                _tarGzCount++;
                TransferInfo _ti = new(Config, pvaUebEinGzItem.Name);

                Log($"{_tarGzCount:00} {_ti.FileName}");

                // -- 1 -- tar.gz Datei in lokalen TEMP Ordner kopieren ----------------------------------------------------

                LogIndent += 3;
                Log($"+ {_ti.LocalTempDir} (DOWNLOAD)");

                Directory.CreateDirectory(_ti.LocalTempDir);

                Ftp.DownloadFile(_ti.LocalTempPath, _ti.FtpPath);

                // -- 2 -- tar.gz Datein aus lokelem TEMP Ordner in Unterordner entpacken ----------------------------------

                Directory.CreateDirectory(_ti.LocalTempUnpackDir);

                LogIndent += 2;
                Log($"+ {_ti.LocalTempUnpackDir} (UNPACK)");

                using (Stream inStream = File.OpenRead(_ti.LocalTempPath))
                using (Stream gzipStream = new GZipInputStream(inStream))
                using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream))
                {
                    // Die Dateien im Archiv sind in einem Unterverzeichnis mit dem gleichen Namen wir das Archiv.
                    // Deswegen wurde ins LocalTempDir entpackt -> die einzelnen Dateienh liegen im _localTempUnpackDir !

                    tarArchive.ExtractContents(_ti.LocalTempDir);
                }

                int _unpackedPdfFiles = 0;
                LogIndent += 2;
                string[] _unpackedFiles = Directory.GetFiles(_ti.LocalTempUnpackDir, "*.*");

                foreach (string unpackedFile in _unpackedFiles)
                {
                    if (unpackedFile.EndsWith("pdf", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _uebEinCount++;
                        Log($"{++_unpackedPdfFiles:00} {Path.GetFileName(unpackedFile)}");
                    }
                    else
                    {
                        Log($"   {Path.GetFileName(unpackedFile)}");
                    }
                }

                LogIndent -= 2;

                // -- 3 -- entpackte Dateien aus TEMP Unterordner in EINGANG kopieren --------------------------------------

                Log($"+ {_ti.LocalDir} (EIN)");

                Directory.CreateDirectory(_ti.LocalDir);

                foreach (string newPath in Directory.GetFiles(_ti.LocalTempUnpackDir, "*.*", SearchOption.TopDirectoryOnly))
                {
                    File.Copy(newPath, newPath.Replace(_ti.LocalTempUnpackDir, _ti.LocalDir), true);
                }

                // -- 4 -- entpackte Dateien aus TEMP Unterordner in ARCHIV kopieren ---------------------------------------

                Log($"+ {_ti.LocalArchiveDir} (ARCHIV)");

                Directory.CreateDirectory(_ti.LocalArchiveDir);

                foreach (string newPath in Directory.GetFiles(_ti.LocalTempUnpackDir, "*.*", SearchOption.TopDirectoryOnly))
                {
                    File.Copy(newPath, newPath.Replace(_ti.LocalTempUnpackDir, _ti.LocalArchiveDir), true);
                }

                // -- 5 -- Datei am FTP Server in ARCHIV am FTP Server verschieben -----------------------------------------

                Log($"> {_ti.FtpArchivePath} (FTP ARCHIVE)");
                Ftp.MoveFile(_ti.FtpPath, _ti.FtpArchivePath);

                // -- 6 -- Datei in TEMP und Unterordner von TEMP mit entpackten Dateien löschen ---------------------------

                Log($"- {_ti.LocalTempUnpackDir} (CLEANUP 1)");
                Directory.Delete(_ti.LocalTempUnpackDir, true);

                Log($"- {_ti.LocalTempPath} (CLEANUP 2)");
                File.Delete(_ti.LocalTempPath);
            }

            LogIndent = 2;
            Log($"TOTAL: {_tarGzCount} ARCHIVE, {_uebEinCount} UEBERWEISUNGEN (eingehend)");
        }

        #endregion
    }
}
