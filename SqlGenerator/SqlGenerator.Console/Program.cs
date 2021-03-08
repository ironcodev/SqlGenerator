using SqlGenerator.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Console
{
    class Program
    {
        static bool Silent = false;
        static string[] options = new string[] { };
        #region Main Logger
        static ConsoleLogger Logger => new ConsoleLogger { Format = "{data}" };
        static void Log(string data)
        {
            if (!Silent)
            {
                Logger.Log(data);
            }
        }
        static void Danger(Exception e, string data = "")
        {
            if (!Silent)
            {
                Logger.Danger(e, data);
            }
        }
        static void Success(string data)
        {
            if (!Silent)
            {
                Logger.Success(data);
            }
        }
        static void Warn(string data)
        {
            if (!Silent)
            {
                Logger.Warn(data);
            }
        }
        #endregion
        static string Version => "2.0.0";
        static void Help()
        {
            Log($@"SQL Script Generator v{Version}
Usage: sqlgen.exe [args]
    args:
        -v  display program version
        -c  specify connectionstring
        -d  specify database
        -s  silent
        -o  options filename (default = 'sqlgen.options')
        -t  specify object types to generate script for. possible values are
                Table, Sproc, Udf, Type, Trigger, Index, View, Constraint, FileGroup, File
                Partition, Assembly, Rule, Sequence, Diagram, Schema, User, Role, Synonym, 
                Permission, Key, Certificate, SecurityPolicy, Audit, Default, Database
        -st specify subtype for chosen type. values depend on chosen object type in -t arg.
            Table: Sql (default), External, File, Graph, All
            Sproc: Sql (default), Clr, Extended, All
            Udf: Scaler, Table, Inline, Clr, Aggregate, UserDefined (default), All
            Trigger: Database (default), Server, All
            Type: Scaler, Table, Clr, Extended, UserDefined (default), All
            View: Sql (default), All,
            Constraint: PrimaryKey, ForeignKey, Default, AllButPk (default), All
            Index: Clustered, NoneClustered, UniqueNoneClustered, ColumnStore, FullText, SelectiveXml, Spatial, Xml, AllButClustered (default), All
            Audit: DatabaseAuditSpec, Server, ServerAuditSpec, All (default)
            Role: Database (default), Server, All
        -gt generation type. possible values are Drop, Create, DropCreate.
        -l  specify logger. possible values are: file (default), console, null
        -w  specify writer. possible values are: file (default), console, null
        -p  output path to create generated files in. default = ./Scripts
        -k  keyword filter
        -nw  do not overwrite existing files.
        -i  show report regarding selected object types
Example:
    sqlgen.exe -c ""Server=.;Database=MyDb;User Id=myuser;Password=mypass"" -t Sproc
    sqlgen.exe -c ""Server=.;Database=MyDb;User Id=myuser;Password=mypass"" -t Sproc -w console
");
            

        }
        static bool TestConnection(string constr)
        {
            var result = false;

            try
            {
                using (var con = new SqlConnection(constr))
                {
                    con.Open();

                    result = true;
                }
            }
            catch (Exception e)
            {
                Danger(e);
            }

            return result;
        }
        static ILogger GetLogger(string loggerType)
        {
            var result = null as ILogger;

            switch (loggerType.Trim().ToLower())
            {
                case "null":
                    result = new NullLogger();
                    break;
                case "console":
                    result = new ConsoleLogger();
                    break;
                case "file":
                    result = new FileLogger { FileName = "sqlgen.log", Path = Environment.CurrentDirectory };
                    break;
                default:
                    Log($"Invalid logger : {loggerType}. possible values are: null, console, file");
                    break;
            }

            return result;
        }
        static IScriptWriter GetWriter(string writerType)
        {
            var result = null as IScriptWriter;

            switch (writerType.Trim().ToLower())
            {
                case "null":
                    result = new NullScriptWriter();
                    break;
                case "console":
                    result = new ConsoleScriptWriter();
                    break;
                case "file":    // default
                    result = new FileScriptWriter();
                    break;
                default:
                    Log($"Invalid writer : {writerType}. possible values are: null, console, file");
                    break;
            }

            return result;
        }
        static T GetOption<T>(string key, T defaultValue)
        {
            var item = options.FirstOrDefault(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase));
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
                    Danger(e, $"Error reading Option {key}");
                }
            }

            return result;
        }
        static void Start(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Help();
            }
            else
            {
                var connectionString = "";
                var database = "";
                var logger = null as ILogger;
                var writer = null as IScriptWriter;
                var type = SqlObjectType.None;
                var subType = "";
                var ok = true;
                var generateType = GenerationType.NotSpecified;
                var outputPath = "";
                var keyword = "";
                var optionsFileName = "";
                bool? overwriteExisting = null;
                var showInfo = false;

                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i].Trim().ToLower();

                    switch (args[i])
                    {
                        case "-v":
                            Log($"SQL Script Generator v{Version}");
                            ok = false;

                            break;
                        case "-c":
                            if (i + 1 < args.Length && args[i + 1][0] != '-')
                            {
                                connectionString = args[i + 1];
                            }

                            break;
                        case "-d":
                            if (i + 1 < args.Length && args[i + 1][0] != '-')
                            {
                                database = args[i + 1];
                            }

                            break;
                        case "-t":
                            if (i + 1 < args.Length && args[i + 1][0] != '-')
                            {
                                if (!Enum.TryParse(args[i + 1], true, out type))
                                {
                                    Log($@"Invalid object type. possible values are
    Table, Sproc, Udf, Type, Trigger, Index, View, Constraint, FileGroup, File
    Partition, Assembly, Rule, Sequence, Diagram, Schema, User, Role, Synonym, 
    Permission, Key, Certificate, SecurityPolicy, Audit, Default, Database");

                                    ok = false;
                                }
                            }

                            break;
                        case "-o":
                            if (i + 1 < args.Length && args[i + 1][0] != '-')
                            {
                                optionsFileName = args[i + 1];
                            }
                            else
                            {
                                optionsFileName = "sqlgen.options";
                            }

                            if (!Path.IsPathRooted(optionsFileName))
                            {
                                optionsFileName = Environment.CurrentDirectory + "\\" + optionsFileName;
                            }

                            if (File.Exists(optionsFileName))
                            {
                                try
                                {
                                    options = File.ReadAllLines(optionsFileName);
                                }
                                catch (Exception e)
                                {
                                    Danger(e, "Error reading options file {optionsFileName}");
                                }
                            }
                            else
                            {
                                System.Console.WriteLine($"Options file '{optionsFileName}' not found");
                            }

                            break;
                        case "-st":
                            if (i + 1 < args.Length)
                            {
                                subType = args[i + 1];
                            }

                            break;
                        case "-l":
                            if (i + 1 < args.Length && args[i + 1][0] != '-')
                            {
                                var loggerType = args[i + 1];

                                logger = GetLogger(loggerType);
                            }

                            break;
                        case "-gt":
                            if (i + 1 < args.Length && args[i + 1][0] != '-')
                            {
                                if (!Enum.TryParse(args[i + 1], true, out generateType))
                                {
                                    Log($@"Invalid script generation type. possible values are Drop, Create, DropCreate");

                                    ok = false;
                                }
                            }

                            break;
                        case "-w":
                            if (i + 1 < args.Length && args[i + 1][0] != '-')
                            {
                                var writerType = args[i + 1];

                                writer = GetWriter(writerType);
                            }

                            break;
                        case "-p":
                            if (i + 1 < args.Length && args[i + 1][0] != '-')
                            {
                                outputPath = args[i + 1];
                            }

                            break;
                        case "-nw":
                            overwriteExisting = false;

                            break;
                        case "-s":
                            Silent = true;

                            break;
                        case "-i":
                            showInfo = true;

                            break;
                        case "-k":
                            if (i + 1 < args.Length && args[i + 1][0] != '-')
                            {
                                keyword = args[i + 1];
                            }

                            break;
                        default:
                            if (arg.Length > 0 && arg[0] == '-')
                            {
                                Log($"Invalid argument {arg}");
                            }

                            break;
                    }
                }

                if (ok)
                {
                    do
                    {
                        try
                        {
                            if (logger == null)
                            {
                                var loggerType = GetOption("Logger", "file");

                                logger = GetLogger(loggerType);
                            }

                            if (writer == null)
                            {
                                var writerType = GetOption("Writer", "file");

                                writer = GetWriter(writerType);
                            }

                            var generator = new SqlGeneratorSimple(logger, writer);

                            if (options.Length > 0)
                            {
                                generator.Options = new SqlGeneratorOptionsFromArray(logger, options);
                            }
                            if (string.IsNullOrEmpty(generator.Options.ConnectionString))
                            {
                                generator.Options.ConnectionString = connectionString;
                            }
                            if (string.IsNullOrEmpty(generator.Options.Database))
                            {
                                generator.Options.Database = database;
                            }
                            if (string.IsNullOrEmpty(generator.Options.OutputPath))
                            {
                                generator.Options.OutputPath = outputPath;
                            }
                            if (string.IsNullOrEmpty(generator.Options.OutputPath))
                            {
                                generator.Options.OutputPath = Environment.CurrentDirectory;
                            }
                            if (overwriteExisting.HasValue)
                            {
                                generator.Options.OverwriteExisting = overwriteExisting.Value;
                            }
                            if (string.IsNullOrEmpty(generator.Options.ConnectionString))
                            {
                                Log($"Please specify database connection string");
                                break;
                            }
                            if (type == SqlObjectType.None)
                            {
                                Log($"Please specify what objects you intend their scripts be generated. (use -t and optional -st switches)");
                                break;
                            }
                            if (generateType == GenerationType.NotSpecified && !showInfo)
                            {
                                Log($"Please specify how scrpipts should be generated. possible values are Drop, Create, DropCreate.");
                                break;
                            }
                            if (!TestConnection(generator.Options.ConnectionString))
                            {
                                Log($"Connection failed");
                                break;
                            }
                            
                            Log($"SQL Script Generator v{Version}\n");

                            if (showInfo)
                            {
                                var data = generator.Count(type, subType, keyword);

                                Log(string.Format("Number of Objects in the Database '{0}'", generator.Options.Db));
                                Log(string.Format("\tObject Type: {0}", type));

                                if (!string.IsNullOrEmpty(keyword))
                                {
                                    Log(string.Format("\tFilter keyword: '{0}'", keyword));
                                }

                                Log("");
                                Log(string.Format("{0,-20}{1,-5}{2,8}{3,5}", "Type".PadLeft(8), "|", "Count", ""));
                                Log(string.Format("{0,-20}{1,-5}{2,8}{3,5}", new string('-', 20), "+" + new string('-', 4), new string('-', 8), new string('-', 5)));

                                foreach (var item in data)
                                {
                                    Log(string.Format("{0,-20}{1,-5}{2,8:N0}{3,5}", new string(' ', 4) + item.Key, "|", item.Value, ""));
                                }

                                Log(string.Format("{0,-20}{1,-5}{2,8}{3,5}\n", new string('-', 20), "+" + new string('-', 4), new string('-', 8), new string('-', 5)));
                            }
                            else
                            {
                                Log("Script genertion started ...\n");
                                Log("Logger= " + logger.GetType().Name);
                                Log("Writer= " + writer.GetType().Name + "\n");

                                generator.Generate(generateType, type, subType, keyword);
                            }

                            Log("Done.");
                        }
                        catch (Exception e)
                        {
                            Danger(e, "Script generation failed!");
                        }
                    } while (false);
                }
            }
        }
        static void Main(string[] args)
        {
            Start(args);

            //System.Console.ReadKey();
        }
    }
}
