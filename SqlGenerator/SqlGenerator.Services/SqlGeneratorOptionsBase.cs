namespace SqlGenerator.Services
{
    public abstract class SqlGeneratorOptionsBase
    {
        public string Database { get; set; }
        public string ConnectionString { get; set; }
        public string OutputPath { get; set; }
        public bool OverwriteExisting { get; set; }
        string db;
        public string Db
        {
            get
            {
                if (string.IsNullOrEmpty(db))
                {
                    do
                    {

                        if (!string.IsNullOrEmpty(Database))
                        {
                            db = Database;
                            break;
                        }

                        if (!string.IsNullOrEmpty(ConnectionString))
                        {
                            foreach (var key in new string[] { "initial catalog", "database" })
                            {
                                var keyIndex = ConnectionString.IndexOf(key, System.StringComparison.OrdinalIgnoreCase);

                                if (keyIndex >= 0)
                                {
                                    var semicolonIndex = ConnectionString.IndexOf(';', keyIndex + key.Length);

                                    if (semicolonIndex > 0)
                                    {
                                        db = ConnectionString.Substring(keyIndex + key.Length + 1, semicolonIndex - keyIndex - key.Length - 1);
                                    }
                                    else
                                    {
                                        db = ConnectionString.Substring(keyIndex + key.Length + 1, ConnectionString.Length - keyIndex - key.Length - 1);
                                    }

                                    db = db.Trim();

                                    if (!string.IsNullOrEmpty(db))
                                    {
                                        if (db[0] == '=')
                                        {
                                            db = db.Length > 1 ? db.Substring(1).Trim(): "";
                                        }
                                    }


                                    break;
                                }
                            }
                        }
                    } while (false);
                }

                return db;
            }
        }
        public abstract string GetPath(SqlObjectType type, object subType = null);
        private ILogger logger;
        public ILogger Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = new NullLogger();
                }

                return logger;
            }
            set
            {
                logger = value;
            }
        }
        public SqlGeneratorOptionsBase() : this(null)
        { }
        public SqlGeneratorOptionsBase(ILogger logger)
        {
            Logger = logger;
            OverwriteExisting = true;
        }
    }
}
