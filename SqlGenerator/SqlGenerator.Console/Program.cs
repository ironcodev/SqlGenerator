using SqlGenerator.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.Console
{
    class Program
    {
        static bool Silent = false;
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
        static string Version => "1.1.4";
        static void Help()
        {
            Log($@"SQL Script Generator v{Version}
Usage: sqlgen.exe [args]
    args:
        -v  display program version
        -c  specify connectionstring
        -d  specify database
        -s  silent
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
        -p  target path to create generated files in. default = ./Scripts
        -nw  do not overwrite existing files.
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
        static void Main(string[] args)
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
                var target = "";
                var overwriteExisting = true;

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
                            if (i + 1 < args.Length)
                            {
                                connectionString = args[i + 1];
                            }
                            break;
                        case "-d":
                            if (i + 1 < args.Length)
                            {
                                database = args[i + 1];
                            }
                            break;
                        case "-t":
                            if (i + 1 < args.Length)
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
                        case "-st":
                            if (i + 1 < args.Length)
                            {
                                subType = args[i + 1];
                            }
                            break;
                        case "-l":
                            if (i + 1 < args.Length)
                            {
                                var loggerType = args[i + 1];

                                switch (loggerType.Trim().ToLower())
                                {
                                    case "null":
                                        logger = new NullLogger();
                                        break;
                                    case "console":
                                        logger = new ConsoleLogger();
                                        break;
                                    case "file":    // default
                                        break;
                                    default:
                                        Log($"Invalid logger : {loggerType}. possible values are: null, console, file");
                                        ok = false;
                                        break;
                                }
                            }
                            break;
                        case "-gt":
                            if (i + 1 < args.Length)
                            {
                                if (!Enum.TryParse(args[i + 1], true, out generateType))
                                {
                                    Log($@"Invalid script generation type. possible values are Drop, Create, DropCreate");

                                    ok = false;
                                }
                            }
                            break;
                        case "-w":
                            if (i + 1 < args.Length)
                            {
                                var writerType = args[i + 1];

                                switch (writerType.Trim().ToLower())
                                {
                                    case "null":
                                        writer = new NullScriptWriter();
                                        break;
                                    case "console":
                                        writer = new ConsoleScriptWriter();
                                        break;
                                    case "file":    // default
                                        break;
                                    default:
                                        Log($"Invalid writer : {writerType}. possible values are: null, console, file");
                                        ok = false;
                                        break;
                                }
                            }
                            break;
                        case "-p":
                            if (i + 1 < args.Length)
                            {
                                target = args[i + 1];
                            }
                            break;
                        case "-nw":
                            overwriteExisting = false;
                            break;
                        case "-s":
                            Silent = true;
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
                        if (string.IsNullOrEmpty(connectionString))
                        {
                            Log($"Please specify database connection string");
                            break;
                        }

                        if (logger == null)
                        {
                            logger = new FileLogger { FileName = "sqlgen.log", Path = Environment.CurrentDirectory };
                        }
                        if (writer == null)
                        {
                            writer = new FileScriptWriter();
                        }
                        if (type == SqlObjectType.None)
                        {
                            Log($"Please specify what objects you intend their scripts be generated. (use -t and optional -st switches)");
                            break;
                        }
                        if (generateType == GenerationType.NotSpecified)
                        {
                            Log($"Please specify how scrpipts should be generated. possible values are Drop, Create, DropCreate.");
                            break;
                        }
                        if (!TestConnection(connectionString))
                        {
                            Log($"Connection failed");
                        }
                        if (string.IsNullOrEmpty(target))
                        {
                            target = Environment.CurrentDirectory;
                        }

                        try
                        {
                            Log("Script genertion started.");
                            Log("Logger= " + logger.GetType().Name);
                            Log("Writer= " + writer.GetType().Name);

                            var generator = new SqlGeneratorSimple(logger, writer);

                            generator.Options.ConnectionString = connectionString;
                            generator.Options.Database = database;
                            generator.Options.TargetPath = target;
                            generator.Options.OverwriteExisting = overwriteExisting;

                            generator.Generate(generateType, type, subType);
                            
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
    }
}
