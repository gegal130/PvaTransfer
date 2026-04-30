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
        const bool Verbose = true;

        static string LogDirectory { get; set; } = "";
        static string LogPath { get; set; } = "";

        static int Main(string[] args)
        {
            Console.WriteLine("VAN Dds Transfer - transfer new files VAN <-> DDS");

            PvaConfig _config = new PvaConfig();

            try
            {
                using (PvaUeberweisungEingehendProcessor _pocessor = new PvaUeberweisungEingehendProcessor(_config))
                {
                    try
                    {
                        _pocessor.Process();
                    }
                    catch (Exception ex)
                    {
                        _pocessor.Log(ex);
                        return 1;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"EXCEPTION creating PvaUeberweisungEingehendProcessor: {e}");
                return 1;
            }

            return 0;
        }
    }
}
