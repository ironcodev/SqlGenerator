using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SqlGenerator.Services
{
    public enum LogType
    {
        Info,
        Success,
        Warning,
        Danger
    }
    public interface ILogger
    { 
        string Format { get; set; }
        void Log(LogType type, string data);
    }
    public abstract class BaseLogger: ILogger
    {
        public string Format { get; set; }

        public abstract void Log(LogType type, string data);
        public BaseLogger()
        {
            Format = "{date}: {data}";
        }
    }
    public class ConsoleLogger : BaseLogger
    {
        public override void Log(LogType type, string data)
        {
            var foreColor = Console.ForegroundColor;

            switch (type)
            {
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.Danger:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            Console.WriteLine(Format.Replace("{date}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fffff")).Replace("{data}", data));

            Console.ForegroundColor = foreColor;
        }
    }
    public class NullLogger : BaseLogger
    {
        public override void Log(LogType type, string data)
        { }
    }
    public class FileLogger : BaseLogger
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public override void Log(LogType type, string data)
        {
            var _data = Format.Replace("{date}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fffff")).Replace("{data}", data) + "\n";

            File.AppendAllText(Path + "\\" + FileName, _data);
        }
    }
}
