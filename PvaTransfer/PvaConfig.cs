using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PvaTransfer
{
    /// <summary>
    /// Implemntierung der Sammlung der Konfigurationsdaten (FTP, Verzeichnisse) für PVA.
    /// </summary>
    public class PvaConfig : ConfigBase
    {
        public const string DefaultJsonConfigFile = "Config.json";

        /// <summary>
        /// Neue Klasse mit Standard-Konfigurationsdatei Config.Json (im Verzeichnis der EXE)
        /// </summary>
        public PvaConfig()
            : this(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, DefaultJsonConfigFile))
        { }

        /// <summary>
        /// Neue Klasse mit angegebener Konfiruationsdatei (json)
        /// </summary>
        /// <param name="jsonConfigFile">Pfad zur Konfigurationsdatei</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Einfache using-Anweisung verwenden", Justification = "<Ausstehend>")]
        public PvaConfig(string jsonConfigFile)
        {
            JsonConfigPath = Path.GetFullPath(jsonConfigFile);
            JsonConfig = File.ReadAllText(JsonConfigPath);

            using (JsonDocument document = JsonDocument.Parse(JsonConfig))
            {
                JsonElement _root = document.RootElement;
                LogDir = _root.GetProperty("LogDirectory").GetString()!;

                JsonElement _ftp = _root.GetProperty("Ftp");
                RemoteHost = _ftp.GetProperty("Host").GetString()!;
                RemoteUser = _ftp.GetProperty("User").GetString()!;
                RemotePw = _ftp.GetProperty("Password").GetString()!;

                JsonElement _pva2an = _root.GetProperty("PvaUeb2Van");
                RemoteDir = _pva2an.GetProperty("Pva2Van_UebEin_Ftp_Directory").GetString()!;
                RemoteArchiveDir = _pva2an.GetProperty("Pva2Van_UebEin_Ftp_Archive_Directory").GetString()!;
                LocalDir = _pva2an.GetProperty("Pva2Van_UebEin_Directory").GetString()!;
                LocalArchiveDir = _pva2an.GetProperty("Pva2Van_UebEin_Archive_Directory").GetString()!;
                LocalTempDir = _pva2an.GetProperty("Pva2Van_UebEin_Temp_Directory").GetString()!;
            }
        }

    }

}
