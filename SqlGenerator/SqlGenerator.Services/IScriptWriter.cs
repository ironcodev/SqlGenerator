using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SqlGenerator.Services
{
    public class ScriptWriterOptions
    { }
    public class FileScriptWriterOptions: ScriptWriterOptions
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public bool OverwriteExisting { get; set; }
    }
    public interface IScriptWriter
    {
        ScriptWriterOptions Options { get; set; }
        void Write(string text);
        bool CanWrite();
    }
    public class ConsoleScriptWriter : IScriptWriter
    {
        public bool CanWrite()
        {
            return true;
        }
        public ScriptWriterOptions Options { get; set; }
        public void Write(string text)
        {
            Console.WriteLine(text);
        }
    }
    public class NullScriptWriter : IScriptWriter
    {
        public ScriptWriterOptions Options { get; set; }
        public bool CanWrite()
        {
            return true;
        }
        public void Write(string text)
        { }
    }
    public class StringScriptWriter : IScriptWriter
    {
        public ScriptWriterOptions Options { get; set; }
        private StringBuilder builder;
        private bool done;
        public StringScriptWriter()
        {
            Reset();
        }
        void Reset()
        {
            builder = new StringBuilder();
            done = false;
        }
        public bool CanWrite()
        {
            return true;
        }

        public void Write(string text)
        {
            if (done)
            {
                Reset();
            }

            builder.Append(text);
        }
        public override string ToString()
        {
            var result = builder.ToString();

            done = true;

            return result;
        }
    }
    public class FileScriptWriter : IScriptWriter
    {
        private ScriptWriterOptions options;
        public ScriptWriterOptions Options
        {
            get
            {
                if (options as FileScriptWriterOptions == null)
                    options = new FileScriptWriterOptions();

                return options;
            }
            set
            {
                if ((value as FileScriptWriterOptions) == null)
                    throw new ApplicationException("FileScriptWriterOptions instance expected");

                options = value;
            }
        }
        public FileScriptWriterOptions StrongOptions
        {
            get { return Options as FileScriptWriterOptions; }
        }
        public bool CanWrite()
        {
            return !File.Exists(StrongOptions.Path + "\\" + StrongOptions.FileName) || StrongOptions.OverwriteExisting;
        }
        public void Write(string text)
        {
            File.WriteAllText(StrongOptions.Path + "\\" + StrongOptions.FileName, text);
        }
    }
}
