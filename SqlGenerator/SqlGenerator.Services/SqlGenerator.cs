using System;
using System.Collections.Generic;
using System.Text;

namespace SqlGenerator.Services
{
    public class SqlGenerator
    {
        private static ISqlGenerator instance;
        public static ISqlGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SqlGeneratorSimple();
                    instance.Writer = new StringScriptWriter();
                    instance.Logger = new NullLogger();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }
    }
}
