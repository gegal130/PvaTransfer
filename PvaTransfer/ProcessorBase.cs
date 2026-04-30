using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PvaTransfer
{
    /// <summary>
    /// Basisklasse für Verarbeitung.
    /// Abgeleitete Klasse(n) müssen Process() und Cleanup Implementieren.
    /// </summary>
    public abstract class ProcessorBase : IDisposable
    {
        #region Constructor

        public ProcessorBase(ConfigBase config)
        {
            Config = config;

            LogDir = config.LogDir;
            LogPath = Path.Combine(LogDir, $"LOG {ProcessingId} {DateTime.Now.Year}-{DateTime.Now.Month}.txt");

            Console.WriteLine($"LOG -> {LogPath}");
            Log("START", 0);
        }

        #endregion

        #region Storage

        public abstract string ProcessingId { get; }

        public bool Verbose { get; init; } = false;
        public ConfigBase Config { get; private init; }

        public string LogDir { get; private init; }
        public string LogPath { get; private init; }
        public int LogIndent { get; protected set; } = 0;

        #endregion

        #region Processing

        public abstract void Process();

        #endregion & Cleanup

        // Detect redundant Dispose() calls in a thread-safe manner.
        // _isDisposed == 0 means Dispose(bool) has not been called yet.
        // _isDisposed == 1 means Dispose(bool) has been already called.
        private bool _isDisposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange<bool>(ref _isDisposed, true, false) == false)
            {
                if (disposing)
                {
                    // Dispose managed state.
                    Cleanup();

                    Finish();
                }
            }
        }

        public virtual void Finish()
        {
            Log("FINISH", 0);
        }

        public abstract void Cleanup();

        #region Tools
        public void Log(Exception ex, bool enableLoggingException = true)
        {
            Log(ex, LogIndent, enableLoggingException);
        }

        public void Log(Exception ex, int logIndent, bool enableLoggingException = true)
        {
            Logg($"EXCEPTION: {ex}", LogIndent, enableLoggingException);
        }

        public void Log(string msg, bool enableLoggingException = true)
        {
            Log(msg, LogIndent, enableLoggingException);
        }

        public void Log(string msg, int logIndent, bool enableLoggingException = true)
        {
            Logg(msg, logIndent, enableLoggingException);
        }

        private void Logg(string msg, int logIndent, bool enableLoggingException = true)
        {
            string _msg = $"{new string(' ', logIndent)}{msg}";

            Console.WriteLine(_msg);

            try
            {
                using (StreamWriter writer = new StreamWriter(LogPath, true))
                {
                    writer.WriteLine($"{DateTime.Now:G} [{ProcessingId}] {_msg}");
                }
            }
            catch (Exception e)
            {
                Exception _ex = new Exception("Fehler beim Erstellen des Protokolls", e);
                Console.WriteLine($"EXCEPTION: {e}");

                if (enableLoggingException)
                    throw _ex;
            }
        }

        #endregion
    }
}
