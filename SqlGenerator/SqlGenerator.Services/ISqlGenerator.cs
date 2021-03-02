using System;
using System.Collections.Generic;
using System.Text;

namespace SqlGenerator.Services
{
    public interface ISqlGenerator
    {
        ILogger Logger { get; set; }
        IScriptWriter Writer { get; set; }
        SqlGeneratorOptions Options { get; set; }
        void Generate(GenerationType generateType, SqlObjectType type, object subType = null);
    }
}
