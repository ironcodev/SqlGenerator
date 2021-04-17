using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlGenerator.Extensions;

namespace SqlScriptMerger
{
    class Program
    {
        static string Version => "1.1.0";
        static string DependenciesFile = "dependencies.json";
        static string OutputFile = "All.sql";
        static string basePath;
        static string scriptsBasePath;
        static string[] customMerges;
        static bool silent;
        static List<SqlObject> items;
        static List<string> temp = new List<string>();
        static bool Debugging;
        static void Log(string msg, Exception e = null)
        {
            Console.WriteLine(msg);

            if (e != null)
            {
                Console.WriteLine("\t" + e.ToString("\t\n"));
            }
        }
        static void Info(string msg)
        {
            if (!silent)
            { 
                Console.WriteLine(msg);
            }
        }
        static void Debug(string msg, Exception e = null)
        {
            if (Debugging)
            {
                Console.WriteLine(msg);

                if (e != null)
                {
                    Console.WriteLine("\t" + e.ToString("\t\n"));
                }
            }
        }
        static void Process(SqlObject obj, StringBuilder builder)
        {
            if (!obj.Processed)
            {
                if (obj.Dependencies != null && obj.Dependencies.Count > 0)
                {
                    foreach (var dependency in obj.Dependencies)
                    {
                        var dependencyObject = items.FirstOrDefault(x => string.Compare(x.Name, dependency, StringComparison.OrdinalIgnoreCase) == 0);

                        if (dependencyObject != null)
                        {
                            Process(dependencyObject, builder);
                        }
                        else
                        {
                            Log($"{obj.Name}: WARNING! dependency {dependency} not found!");
                        }
                    }
                }

                var path = Path.IsPathRooted(obj.FilePath) ? obj.FilePath : (string.IsNullOrEmpty(scriptsBasePath) ? basePath : scriptsBasePath) + "\\" + obj.FilePath;

                try
                {
                    if (File.Exists(path))
                    {
                        var content = File.ReadAllText(path);

                        builder.Append("-- \t\tObject Name: " + obj.Name);
                        builder.AppendLine();
                        builder.Append("-- \t\tObject Path: " + obj.FilePath);
                        builder.AppendLine();
                        builder.Append(content);
                        builder.AppendLine();
                        builder.AppendLine();
                        builder.Append(new string('-', 100));
                        builder.AppendLine();
                        builder.AppendLine();
                    }
                    else
                    {
                        Log($"{obj.Name}: WARNING! file {path} not found!");
                    }
                }
                catch (Exception e)
                {
                    Log($"{obj.Name}: ERROR! reading file {path} failed.", e);
                }

                obj.Processed = true;
            }
            else
            {
                Debug($"{obj.Name} already processed.");
            }
        }
        static void Merge(string type, ref int count, StringBuilder builder)
        {
            try
            {
                builder.AppendLine();
                builder.Append("--" + new string('=', 100));
                builder.AppendLine();
                builder.Append("--\t\t\t" + type);
                builder.AppendLine();
                builder.Append("--" + new string('=', 100));
                builder.AppendLine();

                Log($"Mergeing Objects of type '{type}'...");

                var filteredItems = items.Where(x => x.FilePath.IndexOf("\\" + type + "\\", StringComparison.OrdinalIgnoreCase) > 0);

                foreach (var item in filteredItems)
                {
                    Info($"{count++}. Processing {item.FilePath} ...");

                    Process(item, builder);
                }
            }
            catch (Exception e)
            {
                Log($"Error while merging objects {type} at {count}.", e);
            }
        }
        static void MergeFiles(string relativePath, ref int count, StringBuilder builder)
        {
            try
            {
                builder.AppendLine();
                builder.Append("--" + new string('=', 100));
                builder.AppendLine();
                builder.Append("--\t\t\t" + relativePath);
                builder.AppendLine();
                builder.Append("--" + new string('=', 100));
                builder.AppendLine();

                Log($"Merging Objects in '{relativePath}'");

                var files = null as string[];
                var path = (string.IsNullOrEmpty(scriptsBasePath) ? basePath : scriptsBasePath) + "\\" + relativePath;

                try
                {
                    files = Directory.GetFiles(path, "*.sql", SearchOption.AllDirectories);
                }
                catch (Exception e)
                {
                    Log($"\nCannot search files in {path}", e);
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            var content = File.ReadAllText(file);

                            Info($"{count++}. Appending {file} ...");

                            builder.AppendLine();
                            builder.Append("--" + new string(' ', 20) + file);
                            builder.AppendLine();
                            builder.Append(content);
                            builder.AppendLine();
                            builder.Append("\r\n" + new string('-', 100) + "\r\n");
                            builder.AppendLine();
                        }
                        catch (Exception e)
                        {
                            Log($"Error reading file: {file}", e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log($"Error while merging", e);
            }
        }
        static void Help()
        {
            Console.WriteLine($@"
SQL Script Merger {Version}
Usage:
    sqlmrgr.exe [-sbp:path] [-df:dependency-file-name] [-cm:path1,path2] [-o:output-filename] [-debug]
    -sbp  :   scripts base path (default = '' i.e. use current directory)
    -df   :   dependencies file (default = 'dependencies.json')
    -cm   :   custom merge directories to add to output
    -o    :   output file (default = All.sql)
    -s    :   silent
    -debug:   debug mode
");
        }
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Help();
            }
            else
            {
                Log($@"SQL Script Merger {Version}\n");

                basePath = Environment.CurrentDirectory;
                
                Info($"Base Path: {basePath}");

                foreach (var arg in args)
                {
                    if (!string.IsNullOrEmpty(arg))
                    {
                        if (arg.StartsWith("-df:"))
                        {
                            DependenciesFile = arg.Substring(4);
                            continue;
                        }

                        if (arg.StartsWith("-o:"))
                        {
                            OutputFile = arg.Substring(3);
                            continue;
                        }

                        if (arg.ToLower() == "-debug")
                        {
                            Debugging = true;
                            continue;
                        }

                        if (arg.StartsWith("-cm:"))
                        {
                            customMerges = arg.Substring(4).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            continue;
                        }

                        if (arg.StartsWith("-sbp:"))
                        {
                            scriptsBasePath = arg.Substring(5);
                            continue;
                        }

                        if (arg.ToLower() == "-s")
                        {
                            silent = true;
                            continue;
                        }
                    }
                }

                if (customMerges == null)
                {
                    customMerges = new string[] { };
                }

                var path = basePath + "\\" + DependenciesFile;

                if (!File.Exists(path))
                {
                    Log(DependenciesFile + " not found.");
                }
                else
                {
                    string content = null;

                    try
                    {
                        content = File.ReadAllText(path);

                        if (string.IsNullOrEmpty(content))
                        {
                            Log("dependencies file is empty.");
                        }
                    }
                    catch (Exception e)
                    {
                        Log($"Error while reading dependencies file ({DependenciesFile}).", e);
                    }

                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            items = JsonConvert.DeserializeObject<List<SqlObject>>(content);

                            if (items == null || items.Count == 0)
                            {
                                Log("No dependency found.");
                            }
                            else
                            {
                                Log("Merging started ...");

                                var sw = new Stopwatch();

                                sw.Start();

                                var count = 1;

                                var builder = new StringBuilder();

                                Merge("schemas", ref count, builder);
                                Merge("types", ref count, builder);
                                Merge("tables", ref count, builder);
                                Merge("functions", ref count, builder);
                                Merge("views", ref count, builder);
                                Merge("procedures", ref count, builder);
                                Merge("triggers", ref count, builder);

                                foreach (var dir in customMerges)
                                {
                                    MergeFiles(dir, ref count, builder);
                                }
                                
                                content = builder.ToString();

                                path = basePath + "\\" + OutputFile;

                                try
                                {
                                    File.WriteAllText(path, content);

                                    sw.Stop();

                                    var time = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);

                                    Log(string.Format("\nDone! Finished in {0} hours, {1} minutes and {2} seconds", time.Hours, time.Minutes, time.Seconds));
                                }
                                catch (Exception e)
                                {
                                    Log("Error generating output file (All.sql).", e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log($"Error while deserializing dependencies file ({DependenciesFile}).", e);
                        }
                    }
                }
            }
        }
    }
}
