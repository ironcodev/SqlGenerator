using System;
using System.IO;
using System.Linq;

namespace SqlGenerator.Services
{
    public class SqlGeneratorOptionsFromArray : SqlGeneratorOptionsDefault
    {
        string[] options;
        public string[] Options
        {
            get
            {
                if (options == null)
                {
                    options = new string[] { };
                }

                return options;
            }
            set
            {
                options = value;

                LoadOptions();
            }
        }
        public SqlGeneratorOptionsFromArray() : this(null, null)
        { }
        public SqlGeneratorOptionsFromArray(ILogger logger) : this(logger, null)
        {
        }
        public SqlGeneratorOptionsFromArray(string[] options) : this(null, options)
        { }
        public SqlGeneratorOptionsFromArray(ILogger logger, string[] options) : base(logger)
        {
            Options = options;
        }
        T GetOption<T>(string key, T defaultValue)
        {
            var item = Options.FirstOrDefault(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase));
            var value = string.IsNullOrEmpty(item) || item.Length <= key.Length + 1 ? null : item.Substring(key.Length + 1).Trim();
            var result = defaultValue;

            if (!string.IsNullOrEmpty(value))
            {
                if (value.StartsWith(":") || value.StartsWith("="))
                {
                    value = value.Substring(1).Trim();
                }

                try
                {
                    result = (T)System.Convert.ChangeType(value, typeof(T));
                }
                catch (Exception e)
                {
                    Logger.Danger(e, $"Error reading Option {key}");
                }
            }

            return result;
        }
        private void LoadOptions()
        {
            if (Options != null && Options.Length > 0)
            {
                BasePath = GetOption("BasePath", "/Scripts");
                ConnectionString = GetOption("ConnectionString", "");
                OutputPath = GetOption("OutputPath", "");
                Database = GetOption("Database", "");
                OverwriteExisting = GetOption("OverwriteExisting", true);
            }
        }
        public override string GetPath(SqlObjectType type, object subType = null)
        {
            var key = GetKey(type, subType);
            var result = !string.IsNullOrEmpty(key) ? GetOption(key, "").Replace("{BasePath}", BasePath) : "";

            if (string.IsNullOrEmpty(result))
            { 
                result = base.GetPath(type, subType);
            }

            return result;
        }
    }
}
