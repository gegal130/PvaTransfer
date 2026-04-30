using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvaTransfer
{
    /// <summary>
    /// Basisklasse (abstrakt) für Konfigurationsdaten (Verzeichnisse, Pfade).
    /// Abgeleitete Klasse muss im Konstruktor die Initialisierung implementierung.
    /// </summary>
    public abstract class ConfigBase
    {
        public string JsonConfigPath { get; protected init; } = string.Empty;
        public string JsonConfig { get; protected init; } = string.Empty;

        public string RemoteHost { get; protected init; } = string.Empty;
        public string RemoteUser { get; protected init; } = string.Empty;
        public string RemotePw { get; protected init; } = string.Empty;

        public string RemoteDir { get; protected init; } = string.Empty;
        public string RemoteArchiveDir { get; protected init; } = string.Empty;
        public string LocalDir { get; protected init; } = string.Empty;
        public string LocalArchiveDir { get; protected init; } = string.Empty;
        public string LocalTempDir { get; protected init; } = string.Empty;

        public string LogDir { get; protected init; } = string.Empty;
    }
}
