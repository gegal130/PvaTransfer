using FluentFTP.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvaTransfer
{
    /// <summary>
    /// Hilfsklasse für Bestimmung der Verzeichnisse und Pfade
    /// </summary>
    public  class TransferInfo
    {
        /// <summary>
        /// Erstelle neue Klasse mit Verzeichnissen und Pfaden für die Verarbeitung
        /// </summary>
        /// <param name="config">Konfiguration</param>
        /// <param name="fileName">zu verarbeitender Dateiname (ohne Pfad, mit Extension). Daraus werden die Verarbeitungs-Verzeichnisse und -Pfade abgeleitet.</param>
        public TransferInfo(ConfigBase config, string fileName)
        {
            FileName = fileName;
            FileJahr = FileName.Substring(23, 4);

            FtpDir = config.RemoteDir;
            FtpArchiveDir = config.RemoteArchiveDir;

            LocalDir = Path.Combine(config.LocalDir, FileJahr);
            LocalArchiveDir = Path.Combine(config.LocalArchiveDir, FileJahr);
            LocalTempDir = Path.Combine(config.LocalTempDir, FileJahr);
        }

        public string FileName { get; private init; }
        public string FileNameWoExtension => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(FileName));
        public string FileJahr { get; private init; }

        public string FtpDir { get; private init; }
        public string FtpPath => Path.Combine(FtpDir, FileName);
        public string FtpArchiveDir { get; private init; }
        public string FtpArchivePath => Path.Combine(FtpArchiveDir, FileName);

        
        public string LocalDir { get; private init; }
        public string LocalPath => Path.Combine(LocalDir, FileName);
        public string LocalArchiveDir { get; private init; }
        public string LocalArchivePath => Path.Combine(LocalArchiveDir, FileName);
        public string LocalTempDir { get; private init; }
        public string LocalTempPath => Path.Combine(LocalTempDir, FileName);
        public string LocalTempUnpachDir => Path.Combine(LocalTempDir, FileNameWoExtension);
    }
}
