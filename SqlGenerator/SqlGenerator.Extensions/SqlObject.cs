using System;
using System.Collections.Generic;
using System.Text;

namespace SqlGenerator.Extensions
{
    public class SqlObject
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public List<string> Dependencies { get; set; }
        public bool Processed { get; set; }
        public SqlObject()
        {
            Dependencies = new List<string>();
        }
    }
}
