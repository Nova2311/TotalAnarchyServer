using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer {
    class Logger{
        public static void WriteInfo(string text){
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[" + GetTimeStamp(DateTime.Now) + "] [INFO]" + text);
            Console.ResetColor();
        }

        public static void WriteDebug(string text){
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[" + GetTimeStamp(DateTime.Now) + "] [Debug]" + text);
            Console.ResetColor();
        }

        public static void WriteLog(string text){
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[" + GetTimeStamp(DateTime.Now) + "] [Log]" + text);
            Console.ResetColor();
        }

        public static void WriteError(string text) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[" + GetTimeStamp(DateTime.Now) + "] [Error]" + text);
            Console.ResetColor();
        }

        private static string GetTimeStamp(DateTime _time) {
            return _time.ToString("HH:mm:ss"); // Return the formatted timestamp
        }
    }
}
