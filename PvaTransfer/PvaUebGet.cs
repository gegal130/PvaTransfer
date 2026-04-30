using FluentFTP;
using FluentFTP.Helpers;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace PvaTransfer
{
    internal class Program
    {
        /// <summary>
        /// PVA Transfer - Ueberweisungen (eingehend) PVA -> VAN.
        /// Kommandozeilen Tool, Keine Parameter.
        /// Konfiguration aus Config.json (im Verzeichnis des ausführbaren EXE)
        /// </summary>
        /// <returns>0 wenn alles ok, 1 bei Fehler (Exception)</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Einfache using-Anweisung verwenden", Justification = "<Ausstehend>")]
        static int Main()
        {
            Console.WriteLine("PVA Transfer - Ueberweisungen (eingehend) PVA -> VAN");

            PvaConfig _config = new ();

            try
            {
                using (PvaÜberweisungEinProcessor _pocessor = new(_config))
                {
                    try
                    {
                        _pocessor.Process();
                    }
                    catch (Exception ex)
                    {
                        // Exception in der Verarbeitung

                        _pocessor.Log(ex);
                        return 1;
                    }
                }
            }
            catch (Exception e)
            {
                // _processor kann nicht mal instantiiert werden

                Console.WriteLine($"EXCEPTION creating {nameof(PvaÜberweisungEinProcessor)}: {e}");
                return 1;
            }

            return 0;
        }
    }
}
