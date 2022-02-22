using Newtonsoft.Json;
using SqlGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SqlGenerator.DependencyExtractor
{
    class Program
    {
        static bool silent;
        static List<SqlObject> items;
        static List<SqlObject> existingItems;
        static List<SqlObject> allItems;
        static List<string> excludeNames = new List<string>
        {
            "select", "insert", "update", "from", "join", "inner", "outer", "procedure", "function", "trigger", "where", "varchar", "nvarchar", "int", "decimal", "return", "declare", "set", "go", "end", "alter", "create",
            "view", "delete", "begin", "as", "if", "exists", "and", "or", "null", "isnull", "drop", "returns", "right", "left", "trim", "nocount", "bit", "tinyint", "on", "is", "like", "not", "exec", "over", "order", "by",
            "asc", "desc", "case", "when", "then", "row_number", "group", "having", "pivot", "in", "len", "convert", "cast", "index", "references", "sys", "object_id", "schema_id", "object_name", "type", "off", "datetime",
            "smalldatetime", "time", "getdate", "else", "float", "while", "cursor", "count", "sum", "min", "max", "avg", "top", "into", "xml", "json", "out", "partition", "ntext", "text", "ansi_nulls", "table"
        };
        static List<string> excludeSchemas = new List<string>
        {
            "sys"
        };
        static string Version => "1.1.1";
        static string[] ExcludedFolders;
        static string[] ExcludedNames;
        static string basePath;
        static bool Debugging;
        static string DependenciesFile = "dependencies.json";
        static bool EqualsOIC(string a, string b)
        {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        }
        static bool ContainsOIC(string a, params string[] b)
        {
            var result = false;

            if (b != null && b.Length > 0)
            {
                foreach (var x in b)
                {
                    result = a.IndexOf(x, StringComparison.OrdinalIgnoreCase) > 0;

                    if (result)
                    {
                        break;
                    }
                }
            }

            return result;
        }
        enum PurifySqlStates
        {
            start,
            beginSingleLineComment,
            singleLineCommentBegan,
            beginMultiLineComment,
            multiLineCommentBegan,
            endMultiLineComment,
            multiLineCommentEnded,
            beginString,
            apostrophInString,
            beginWhitespace
        }
        enum ParseSqlStates
        {
            start,
            BSC,    // beginSingleLineComment,
            SCB,    // singleLineCommentBegan,
            BMC,    // beginMultiLineComment,
            MCB,    // multiLineCommentBegan,
            EMC,    // endMultiLineComment,
            BS,     // beginString,
            AIS,    // apostrophInString,
            BWS,    // beginWhitespace
            D1,     // [ 1
            D3,     // ] 1
            D31,    // . 1
            D32,    // N2
            D4,     // [ 2
            D5,     // ] 2
            D51,    // . 2
            D52,    // N3*
            D6,     // [ 3
            D7,     // N1
            D71
        }
        static string PurifyContent(string content)
        {
            var result = "";
            var state = PurifySqlStates.start;
            var i = 0;
            char ch;
            char? lastCh = null;

            while (i < content.Length)
            {
                if (lastCh.HasValue)
                {
                    ch = lastCh.Value;
                    lastCh = null;
                }
                else
                {
                    ch = content[i++];
                }

                switch (state)
                {
                    case PurifySqlStates.start:
                        switch (ch)
                        {
                            case '-':
                                state = PurifySqlStates.beginSingleLineComment;
                                break;
                            case '/':
                                state = PurifySqlStates.beginMultiLineComment;
                                break;
                            case '\'':
                                state = PurifySqlStates.beginString;
                                result += ch;
                                break;
                            default:
                                if (char.IsWhiteSpace(ch))
                                {
                                    state = PurifySqlStates.beginWhitespace;

                                    result += ch;
                                }
                                else
                                {
                                    result += ch;
                                }

                                break;
                        }

                        break;
                    case PurifySqlStates.beginSingleLineComment:
                        if (ch == '-')
                        {
                            state = PurifySqlStates.singleLineCommentBegan;
                        }
                        else
                        {
                            state = PurifySqlStates.start;
                            result += ch;
                            lastCh = ch;
                        }
                        break;
                    case PurifySqlStates.singleLineCommentBegan:
                        if (ch == '\r' || ch == '\n')
                        {
                            state = PurifySqlStates.start;
                        }
                        break;
                    case PurifySqlStates.beginMultiLineComment:
                        if (ch == '*')
                        {
                            state = PurifySqlStates.multiLineCommentBegan;
                        }
                        else
                        {
                            state = PurifySqlStates.start;
                            result += ch;
                            lastCh = ch;
                        }
                        break;
                    case PurifySqlStates.multiLineCommentBegan:
                        if (ch == '*')
                        {
                            state = PurifySqlStates.endMultiLineComment;
                        }
                        break;
                    case PurifySqlStates.endMultiLineComment:
                        if (ch == '/')
                        {
                            state = PurifySqlStates.start;
                        }
                        else
                        {
                            state = PurifySqlStates.multiLineCommentBegan;
                        }
                        break;
                    case PurifySqlStates.beginString:
                        if (ch == '\'')
                        {
                            state = PurifySqlStates.apostrophInString;
                        }
                        break;
                    case PurifySqlStates.apostrophInString:
                        if (ch == '\'')
                        {
                            state = PurifySqlStates.beginString;
                        }
                        else
                        {
                            state = PurifySqlStates.start;
                            result += '\'';
                            lastCh = ch;
                        }
                        break;
                    case PurifySqlStates.beginWhitespace:
                        if (!char.IsWhiteSpace(ch))
                        {
                            lastCh = ch;
                            state = PurifySqlStates.start;
                        }
                        break;
                }
            }

            return result;
        }
        static bool IsNameStarting(char ch)
        {
            return char.IsLetter(ch) || ch == '_';
        }
        static bool IsInName(char ch)
        {
            return char.IsLetterOrDigit(ch) || ch == '_';
        }
        static string[] ParseContent(string content)
        {
            var result = new List<string>();
            var name = "";
            var state = ParseSqlStates.start;
            var i = 0;
            char ch;
            char? lastCh = null;

            void Finish(bool saveLastChar = true, string info = "")
            {
                if (saveLastChar)
                {
                    lastCh = ch;
                }

                var n = name.ToLower();

                if (result.IndexOf(n) < 0)
                {
                    var dotIndex = n.IndexOf(".");
                    var schema = dotIndex > 0 ? n.Substring(0, dotIndex).ToLower() : "";

                    if (excludeNames.IndexOf(n) < 0 && excludeSchemas.IndexOf(schema) < 0)
                    {
                        result.Add(n);
                    }
                }
                //Console.WriteLine($"{info} ch: {ch}, name: {name}");
                name = "";
                state = ParseSqlStates.start;
            }

            try
            {
                while (i < content.Length)
                {
                    if (lastCh.HasValue)
                    {
                        ch = lastCh.Value;
                        lastCh = null;
                    }
                    else
                    {
                        ch = content[i++];
                    }

                    switch (state)
                    {
                        case ParseSqlStates.start:
                            switch (ch)
                            {
                                case '-':
                                    state = ParseSqlStates.BSC;
                                    break;
                                case '/':
                                    state = ParseSqlStates.BMC;
                                    break;
                                case '\'':
                                    state = ParseSqlStates.BS;
                                    break;
                                case '[':
                                    state = ParseSqlStates.D1;
                                    break;
                                default:
                                    if (char.IsWhiteSpace(ch))
                                    {
                                        state = ParseSqlStates.BWS;
                                    }
                                    else
                                    {
                                        if (IsNameStarting(ch))
                                        {
                                            state = ParseSqlStates.D7;
                                            name += ch;
                                        }
                                    }

                                    break;
                            }

                            break;
                        case ParseSqlStates.BSC:
                            if (ch == '-')
                            {
                                state = ParseSqlStates.SCB;
                            }
                            else
                            {
                                state = ParseSqlStates.start;
                                lastCh = ch;
                            }
                            break;
                        case ParseSqlStates.SCB:
                            if (ch == '\r' || ch == '\n')
                            {
                                state = ParseSqlStates.start;
                            }
                            break;
                        case ParseSqlStates.BMC:
                            if (ch == '*')
                            {
                                state = ParseSqlStates.MCB;
                            }
                            else
                            {
                                state = ParseSqlStates.start;
                                lastCh = ch;
                            }
                            break;
                        case ParseSqlStates.MCB:
                            if (ch == '*')
                            {
                                state = ParseSqlStates.EMC;
                            }
                            break;
                        case ParseSqlStates.EMC:
                            if (ch == '/')
                            {
                                state = ParseSqlStates.start;
                            }
                            else
                            {
                                state = ParseSqlStates.MCB;
                                lastCh = ch;
                            }
                            break;
                        case ParseSqlStates.BS:
                            if (ch == '\'')
                            {
                                state = ParseSqlStates.AIS;
                            }
                            break;
                        case ParseSqlStates.AIS:
                            if (ch == '\'')
                            {
                                state = ParseSqlStates.BS;
                            }
                            else
                            {
                                state = ParseSqlStates.start;
                                lastCh = ch;
                            }
                            break;
                        case ParseSqlStates.BWS:
                            if (!char.IsWhiteSpace(ch))
                            {
                                lastCh = ch;
                                state = ParseSqlStates.start;
                            }
                            break;
                        case ParseSqlStates.D1:
                            if (ch == ']')
                            {
                                state = ParseSqlStates.D3;
                            }
                            else
                            {
                                name += ch;
                            }
                            break;
                        case ParseSqlStates.D3:
                            if (IsNameStarting(ch))
                            {
                                state = ParseSqlStates.D32;
                                name += '.' + ch;
                            }
                            else if (ch == '.')
                            {
                                state = ParseSqlStates.D31;
                                name += '.';
                            }
                            else if (ch == '[')
                            {
                                state = ParseSqlStates.D4;
                                result.Add(name);
                                name = "";
                            }
                            else if (char.IsWhiteSpace(ch))
                            {
                                state = ParseSqlStates.D3;
                            }
                            else
                            {
                                Finish(true, "D3");
                            }
                            break;
                        case ParseSqlStates.D31:
                            if (char.IsWhiteSpace(ch))
                            {
                                state = ParseSqlStates.D31;
                            }
                            else if (IsNameStarting(ch))
                            {
                                state = ParseSqlStates.D32;
                                name += ch;
                            }
                            else if (ch == '[')
                            {
                                state = ParseSqlStates.D4;
                            }
                            else
                            {
                                Finish(true, "D31");
                            }
                            break;
                        case ParseSqlStates.D32:
                            if (IsInName(ch))
                            {
                                name += ch;
                            }
                            else if (ch == '.')
                            {
                                name += '.';
                                state = ParseSqlStates.D51;
                            }
                            else
                            {
                                Finish(true, "D32");
                            }
                            break;
                        case ParseSqlStates.D4:
                            if (ch == ']')
                            {
                                state = ParseSqlStates.D5;
                            }
                            else
                            {
                                name += ch;
                            }
                            break;
                        case ParseSqlStates.D5:
                            if (ch == '.')
                            {
                                state = ParseSqlStates.D51;
                                name += '.';
                            }
                            else if (ch == '[')
                            {
                                state = ParseSqlStates.D6;
                                result.Add(name);
                                name = "";
                            }
                            else
                            {
                                Finish(true, "D5");
                            }
                            break;
                        case ParseSqlStates.D51:
                            if (IsNameStarting(ch))
                            {
                                name += ch;
                                state = ParseSqlStates.D52;
                            }
                            else if (ch == '[')
                            {
                                state = ParseSqlStates.D6;
                            }
                            else
                            {
                                Finish(true, "D51");
                            }
                            break;
                        case ParseSqlStates.D52:
                            if (IsInName(ch))
                            {
                                name += ch;
                            }
                            else
                            {
                                Finish(true, "D52");
                            }
                            break;
                        case ParseSqlStates.D6:
                            if (ch == ']')
                            {
                                Finish(false, "D6");
                            }
                            else
                            {
                                name += ch;
                            }
                            break;
                        case ParseSqlStates.D7:
                            if (IsInName(ch))
                            {
                                name += ch;
                            }
                            else if (ch == '.')
                            {
                                name += '.';
                                state = ParseSqlStates.D31;
                            }
                            else if (ch == '[')
                            {
                                state = ParseSqlStates.D4;
                            }
                            else if (char.IsWhiteSpace(ch))
                            {
                                state = ParseSqlStates.D71;
                            }
                            else
                            {
                                Finish(true, "D7");
                            }
                            break;
                        case ParseSqlStates.D71:
                            if (ch == '.')
                            {
                                name += '.';
                                state = ParseSqlStates.D31;
                            }
                            if (char.IsWhiteSpace(ch))
                            {
                                state = ParseSqlStates.D71;
                            }
                            else
                            {
                                Finish(true, "D71");
                            }
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    result.Add(name);
                }
            }
            catch (Exception e)
            {
                Debug("Parse Error", e);
                throw;
            }

            return result.ToArray();
        }
        static void FillDependencies(SqlObject obj, int i)
        {
            string content = null;
            var path = Path.IsPathRooted(obj.FilePath) ? obj.FilePath : basePath + "\\" + obj.FilePath;

            try
            {
                content = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Log($"Error reading file {path}", e);
            }

            if (content != null)
            {
                var names = ParseContent(content);

                Debug($"Extracted dependencies ...");

                foreach (var name in names)
                {
                    Debug("\t" + name);
                }

                if (names.Length == 0)
                {
                    Debug($"{obj.Name} does not have any dependencies.");
                }
                else
                {
                    Debug($"Finding dependencies ...");

                    var isType = ContainsOIC(obj.FilePath, "\\types\\");

                    foreach (var x in allItems.Where(item => item.Name != obj.Name))
                    {
                        if (isType && ContainsOIC(x.FilePath, "\\tables\\", "\\procedures\\", "\\functions\\"))
                        {
                            continue;
                        }

                        var dotIndex = x.Name.IndexOf('.');
                        var pureName = dotIndex > 0 ? x.Name.Substring(dotIndex + 1) : x.Name;

                        foreach (var name in names)
                        {
                            if (EqualsOIC(x.Name, name) || EqualsOIC(name, pureName))
                            {
                                obj.Dependencies.Add(x.Name);

                                break;
                            }
                        }
                    }
                }

                Info(string.Format("{0}. {1} : {2}", i, obj.Name, obj.Dependencies.Count));
            }
        }
        static void Info(string msg)
        {
            if (!silent)
            {
                Console.WriteLine(msg);
            }
        }
        static void Log(string msg, Exception e = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
        {
            Console.WriteLine(msg);

            if (e != null)
            {
                Console.WriteLine("\t" + e.ToString("\t\n"));
                Console.WriteLine("\t" + memberName + "\n\t" + sourceLineNumber);
            }
        }
        static void Debug(string msg, Exception e = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (Debugging)
            {
                Console.WriteLine(msg);

                if (e != null)
                {
                    Console.WriteLine("\t" + e.ToString("\t\n"));
                    Console.WriteLine("\t" + memberName + "\n\t" + sourceLineNumber);
                }
            }
        }
        static void Help()
        {
            Console.WriteLine($@"
SQL Dependency Extractor {Version}
Usage:
    sqldepx.exe [-sf:source-folder] [-xf:excluded-folders] [-xn:excluded-names] [-df:dependencies-file] [-xd:] [-nw] [-s] [-sdf] [-debug]

    -sf   :   source folder to look for sql database objects' scripts (default = ./Scripts)
    -xf   :   excluded folders
    -xn   :   excluded file names
    -df   :   dependencies file (default = 'dependencies.json')
    -nw   :   do not overwrite existing dependencies file
    -s    :   silent
    -sdf  :   skip existing dependencies file
    -debug:   debug mode
");
        }
        static string RefinePath(string path)
        {
            var result = "";
            var count = 0;
            bool slash = false;
            bool dot = false;

            foreach (var ch in path)
            {
                if (ch == '/')
                {
                    if (!slash)
                    {
                        result += "/";
                        slash = true;
                    }

                    continue;
                }

                if (ch == '\\')
                {
                    if (!slash)
                    {
                        result += "/";
                        slash = true;
                    }

                    continue;
                }

                //if (ch == '.')
                //{
                //    if (count == path.Length - 1 || path[count + 1] == '/' || path[count + 1] == '\\')
                //    {
                //        continue;
                //    }
                //}

                result += ch;
                slash = false;
                count++;
            }

            return result;
        }
        static bool IsExcluded(string fullfilepath)
        {
            var result = false;

            //path = RefinePath(path);

            foreach (var name in ExcludedFolders)
            {
                var n = name.Replace("/", "\\");

                if (!name.StartsWith("\\"))
                {
                    n = "\\" + n;
                }

                if (!name.EndsWith("\\"))
                {
                    n = n + "\\";
                }

                if (fullfilepath.IndexOf(n) > 0)
                {
                    Debug(fullfilepath + " skipped because it is located in " + n);
                    result = true;
                    break;
                }
            }

            if (!result)
            {
                foreach (var name in ExcludedNames)
                {
                    if (fullfilepath.EndsWith(name))
                    {
                        Debug(fullfilepath + " is excluded name");
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
        static void Run(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Help();
            }
            else
            {
                Log($@"SQL Dependency Extractor {Version}");

                basePath = Environment.CurrentDirectory;

                var sourceFolder = "Scripts";
                var overwriteExisting = true;
                var skipExistingDependencies = false;

                foreach (var arg in args)
                {
                    if (!string.IsNullOrEmpty(arg))
                    {
                        if (arg.StartsWith("-sf:"))
                        {
                            sourceFolder = arg.Substring(4);
                            continue;
                        }

                        if (arg.StartsWith("-xf:"))
                        {
                            ExcludedFolders = arg.Substring(4).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            continue;
                        }

                        if (arg.StartsWith("-xn:"))
                        {
                            ExcludedNames = arg.Substring(4).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            continue;
                        }

                        if (arg.StartsWith("-df:"))
                        {
                            DependenciesFile = arg.Substring(4);
                            continue;
                        }

                        if (arg.ToLower() == "-nw")
                        {
                            overwriteExisting = false;
                            continue;
                        }

                        if (arg.ToLower() == "-s")
                        {
                            silent = true;
                            continue;
                        }

                        if (arg.ToLower() == "-sdf")
                        {
                            skipExistingDependencies = true;
                            continue;
                        }

                        if (arg.ToLower() == "-debug")
                        {
                            Debugging = true;
                            continue;
                        }
                    }
                }

                if (!Path.IsPathRooted(sourceFolder))
                {
                    sourceFolder = Path.Combine(basePath + "\\", sourceFolder);
                }

                sourceFolder = sourceFolder.Replace("/", "\\");

                if (ExcludedFolders == null)
                {
                    ExcludedFolders = new string[] { };
                }

                if (ExcludedNames == null)
                {
                    ExcludedNames = new string[] { };
                }

                if (!Directory.Exists(sourceFolder))
                {
                    Log("source folder not found.");
                }
                else
                {
                    string[] files = null;

                    try
                    {
                        files = Directory.GetFiles(sourceFolder, "*.sql", SearchOption.AllDirectories);
                    }
                    catch (Exception e)
                    {
                        Log("Error scanning source directory", e);
                    }

                    Log($"Base Path: {basePath}");

                    if (files != null)
                    {
                        Log(string.Format("Total Files: {0}", files.Length));

                        var path = basePath + "\\" + DependenciesFile;

                        if (File.Exists(path) && !skipExistingDependencies)
                        {
                            try
                            {
                                var content = File.ReadAllText(path);

                                existingItems = JsonConvert.DeserializeObject<List<SqlObject>>(content);
                            }
                            catch (Exception e)
                            {
                                Log($"Error reading existing dependencies in {DependenciesFile}", e);
                            }
                        }

                        if (existingItems == null)
                        {
                            existingItems = new List<SqlObject>();
                        }

                        if (existingItems.Count > 0)
                        {
                            Log($"Existing Objects: {existingItems.Count}");
                        }

                        Log($"Excluded Directories: {ExcludedFolders.Join(", ")}");
                        Log($"Excluded Names: {ExcludedNames.Join(", ")}");

                        if (files.Length > 0)
                        {
                            Log($"Gathering List of Objects ...");

                            allItems = files.Where(x => !IsExcluded(x)).Select(x => new SqlObject { FilePath = x.Replace(basePath + "\\", ""), Name = Path.GetFileNameWithoutExtension(x) }).ToList();
                            items = allItems.Where(x => !existingItems.Exists(item => string.Compare(item.FilePath, x.FilePath, true) == 0)).ToList();

                            Log(string.Format("All Objects: {0}\n", allItems.Count));
                            Log(string.Format("Total Objects: {0}\n", items.Count));

                            if (items.Count > 0)
                            {
                                var count = 1;
                                var sw = new Stopwatch();

                                sw.Start();

                                foreach (var item in items)
                                {
                                    try
                                    {
                                        FillDependencies(item, count);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log(string.Format("Error finding dependencies of {0}", item.FilePath), ex);
                                    }

                                    count++;
                                }

                                sw.Stop();

                                var time = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);

                                try
                                {
                                    var ok = true;

                                    if (File.Exists(path))
                                    {
                                        if (!overwriteExisting)
                                        {
                                            Console.Write($"File {DependenciesFile} already exists. Overwrite? (Y/N)");

                                            var r = Console.ReadKey();

                                            ok = r.KeyChar == 'y' || r.KeyChar == 'Y';
                                        }
                                    }

                                    if (ok)
                                    {
                                        foreach (var item in items)
                                        {
                                            existingItems.Add(item);
                                        }

                                        File.WriteAllText(path, JsonConvert.SerializeObject(existingItems, Formatting.Indented));
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log("Error writing dependencies file.", e);
                                }

                                Log(string.Format("\nFinished in {0} hours, {1} minutes and {2} seconds", time.Hours, time.Minutes, time.Seconds));
                            }
                            else
                            {
                                Log("No objects found to extract their dependencies.");
                            }
                        }
                        else
                        {
                            Log("No .sql file(s) found");
                        }
                    }
                }
            }
        }
        /*
        static void Test(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Help();
            }
            else
            {
                basePath = Environment.CurrentDirectory;

                var sourceFolder = "Scripts";
                var overwriteExisting = true;
                var skipExistingDependencies = false;
                var silent = false;

                foreach (var arg in args)
                {
                    if (!string.IsNullOrEmpty(arg))
                    {
                        if (arg.StartsWith("-sf:"))
                        {
                            sourceFolder = arg.Substring(4);
                            continue;
                        }

                        if (arg.StartsWith("-xf:"))
                        {
                            ExcludedFolders = arg.Substring(4).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            continue;
                        }

                        if (arg.StartsWith("-xn:"))
                        {
                            ExcludedNames = arg.Substring(4).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            continue;
                        }

                        if (arg.StartsWith("-df:"))
                        {
                            DependenciesFile = arg.Substring(4);
                            continue;
                        }

                        if (arg.ToLower() == "-nw")
                        {
                            overwriteExisting = false;
                            continue;
                        }

                        if (arg.ToLower() == "-s")
                        {
                            silent = true;
                            continue;
                        }

                        if (arg.ToLower() == "-sdf")
                        {
                            skipExistingDependencies = true;
                            continue;
                        }

                        if (arg.ToLower() == "-debug")
                        {
                            Debugging = true;
                            continue;
                        }
                    }
                }

                if (!Path.IsPathRooted(sourceFolder))
                {
                    sourceFolder = Path.Combine(basePath + "\\", sourceFolder);
                }

                sourceFolder = sourceFolder.Replace("/", "\\");

                if (ExcludedFolders == null)
                {
                    ExcludedFolders = new string[] { };
                }

                if (ExcludedNames == null)
                {
                    ExcludedNames = new string[] { };
                }

                if (!Directory.Exists(sourceFolder))
                {
                    Log("source folder not found.");
                }
                else
                {
                    string[] files = null;

                    try
                    {
                        files = Directory.GetFiles(sourceFolder, "*.sql", SearchOption.AllDirectories);
                    }
                    catch (Exception e)
                    {
                        Log("Error scanning source directory", e);
                    }

                    Log($"Base Path: {basePath}");

                    if (files != null)
                    {
                        Log(string.Format("Total Files: {0}", files.Length));

                        var path = basePath + "\\" + DependenciesFile;

                        if (File.Exists(path) && !skipExistingDependencies)
                        {
                            try
                            {
                                var content = File.ReadAllText(path);

                                existingItems = JsonConvert.DeserializeObject<List<SqlObject>>(content);
                            }
                            catch (Exception e)
                            {
                                Log($"Error reading existing dependencies in {DependenciesFile}", e);
                            }
                        }

                        if (existingItems == null)
                        {
                            existingItems = new List<SqlObject>();
                        }

                        if (existingItems.Count > 0)
                        {
                            Log($"Existing Objects: {existingItems.Count}");
                        }

                        Log($"Excluded Directories: {ExcludedFolders.Join(", ")}");
                        Log($"Excluded Names: {ExcludedNames.Join(", ")}");

                        var count = 0;

                        Log("\nfiles ...");

                        foreach (var item in files)
                        {
                            Log(item);

                            if (count++ > 10)
                                break;
                        }

                        Log("\nexistings ...");

                        count = 0;

                        foreach (var item in existingItems)
                        {
                            Log(item.FilePath);

                            if (count++ > 10)
                                break;
                        }

                        if (files.Length > 0)
                        {
                            allItems = files.Select(x => new SqlObject { FilePath = x.Replace(basePath + "\\", ""), Name = Path.GetFileNameWithoutExtension(x) }).ToList();
                            items = files.Where(x => !IsExcluded(x)).Select(x => new SqlObject { FilePath = x.Replace(basePath + "\\", ""), Name = Path.GetFileNameWithoutExtension(x) }).ToList();

                            count = 0;

                            Log("\nitems ...");

                            foreach (var item in items)
                            {
                                Log(item.FilePath);

                                if (count++ > 10)
                                    break;
                            }

                            Log(string.Format("Total Objects: {0}\n", items.Count));
                        }
                        else
                        {
                            Log("No .sql file(s) found");
                        }
                    }
                }
            }
        }
        */
        static void Test(string[] args)
        {
            foreach (var item in args)
            {
                Log(item);
            }
        }
        static void Main(string[] args)
        {
            Run(args);
            //Test(args);
        }
    }
}
