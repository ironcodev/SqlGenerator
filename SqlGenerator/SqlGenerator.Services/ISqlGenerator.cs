using System;
using System.Collections.Generic;
using System.Text;

namespace SqlGenerator.Services
{
    public class SqlGeneratorOnGenerateEventArgs
    {
        public SqlObjectType Type { get; set; }
        public object SubType { get; set; }
        public string Keyword { get; set; }
        public int Count { get;set; }
        public int Index { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }
    }
    public delegate void SqlGeneratorOnGenerateEventHandler(ISqlGenerator sender, SqlGeneratorOnGenerateEventArgs e);
    public interface ISqlGenerator
    {
        ILogger Logger { get; set; }
        IScriptWriter Writer { get; set; }
        SqlGeneratorOptionsBase Options { get; set; }
        event SqlGeneratorOnGenerateEventHandler OnGenerate;
        void Generate(GenerationType generateType, SqlObjectType type, object subType = null, string keyword = "");
        Dictionary<string, int> Count(SqlObjectType type, object subType = null, string keyword = "");
    }
}
