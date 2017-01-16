using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using SyndicateLogic.Entities;

namespace SyndicateLogic
{
    public class DataLayer
    {
        public static void LogMessage(LogLevel level, string message)
        {
            if (Environment.UserInteractive)
                Console.WriteLine(message);
            using (var _logContext = new Db())
            {
                //if (_logContext.Logs.Count() > 1000000)
                //    Environment.Exit(0);
                _logContext.Logs.Add(
                    new Log
                    {
                        Message = message,
                        Severity = (byte)level
                    });
                _logContext.SaveChanges();
            }
        }

        public static void LogException(Exception e)
        {
            string msg = e.Message + e.StackTrace;
            if (e.InnerException != null)
            {
                msg += e.InnerException.Message;
                if (e.InnerException.InnerException != null)
                    msg += e.InnerException.InnerException.Message;
            }
            if (Environment.UserInteractive)
                Console.WriteLine(msg);
            using (var _logContext = new Db())
            {
                //if (_logContext.Logs.Count() > 1000000)
                //    Environment.Exit(0);
                _logContext.Logs.Add(
                    new Log
                    {
                        Message = msg,
                        Severity = (byte)LogLevel.Error
                    });
                _logContext.SaveChanges();
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    }
}