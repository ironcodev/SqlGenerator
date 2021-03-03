using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace SqlGenerator.Services
{
    public class SqlGeneratorSimple : ISqlGenerator
    {
		public static string ObjectSeparator => new string('-', 100) + "\n";

		public SqlGeneratorSimple()
        { }
		public SqlGeneratorSimple(ILogger logger): this(logger, null)
		{ }
		public SqlGeneratorSimple(ILogger logger, IScriptWriter writer)
		{
			Logger = logger;
			Writer = writer;
		}
		private SqlGeneratorOptions options;
        public SqlGeneratorOptions Options
        {
            get
            {
                if (options == null)
                {
                    options = new SqlGeneratorOptions();
                }

                return options;
            }
            set
            {
                options = value;
            }
        }
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
		private IScriptWriter writer;
		public IScriptWriter Writer
		{
			get
			{
				if (writer == null)
				{
					writer = new NullScriptWriter();
				}

				return writer;
			}
			set
			{
				writer = value;
			}
		}
		string GetText(string schema, string name, out bool error)
		{
			var result = "";

			error = true;

			try
			{
				using (var con = new SqlConnection(Options.ConnectionString))
				{
					using (var cmd = new SqlCommand($"EXEC sp_helptext N'{schema}.{name}';", con))
					{
						cmd.CommandType = CommandType.Text;
						con.Open();

						if (!string.IsNullOrEmpty(Options.Database))
                        {
							var cmddb = new SqlCommand("use " + Options.Database, con);
							cmddb.ExecuteNonQuery();
                        }

						var sb = new StringBuilder();

						using (var reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								sb.Append(reader[0]?.ToString());
							}
						}

						sb.Append("\r\nGO\r\n");

						result = sb.ToString();
					}
				}

				error = false;
			}
			catch (Exception e)
			{
				Logger.Danger(e, $"GetText('{schema}', '{name}') failed");
			}

			return result;
		}
		void CreateDir(string path)
		{
			if (!Directory.Exists(Options.TargetPath + path))
			{
				Directory.CreateDirectory(Options.TargetPath + path);
			}
		}
		private void Generate(string path, GenerationType generateType, string nativeType)
		{
			try
			{
				Logger.Log($"-------------------------------------------------------------");

				if (!string.IsNullOrEmpty(nativeType))
				{
					Logger.Log($"Generating {nativeType} objects in {path} ...");

					using (var con = new SqlConnection(Options.ConnectionString))
					{
						using (var cmd = new SqlCommand($"select schema_name(schema_id), name from sys.all_objects where type_desc=N'{nativeType}' and is_ms_shipped = 0", con))
						{
							cmd.CommandType = CommandType.Text;
							con.Open();

							if (!string.IsNullOrEmpty(Options.Database))
							{
								var cmddb = new SqlCommand("use " + Options.Database, con);
								cmddb.ExecuteNonQuery();
							}

							var sb = new StringBuilder();

							using (var reader = cmd.ExecuteReader())
							{
								while (reader.Read())
								{
									var schema = reader[0]?.ToString();
									var name = reader[1]?.ToString();
									var text = "";
									var error = false;

									switch (generateType)
                                    {
										case GenerationType.Create:
											text = GetText(schema, name, out error);
											text += "\n" + ObjectSeparator;
											break;
										case GenerationType.Drop:
											text = $"drop {SqlGeneratorHelper.GetLogicalType(nativeType)} {schema}.{name}";

											break;
										case GenerationType.DropCreate:
											text = $@"
if exists (select 1 from sys.all_objects where name='{name}' and schema_id = schema_id('{schema}'))
	drop {SqlGeneratorHelper.GetLogicalType(nativeType)} {schema}.{name}
go
";
											text += GetText(schema, name, out error);
											text += "\n" + ObjectSeparator;
											break;
                                    }

									if (!error)
									{
										Writer.Options = new FileScriptWriterOptions
										{
											FileName = schema + "." + name + ".sql",
											Path = Options.TargetPath + path,
											OverwriteExisting = Options.OverwriteExisting
										};

										if (Writer.CanWrite())
										{
											Writer.Write(text);

											Logger.Log($"{schema}.{name} generated");
										}
										else
										{
											Logger.Log($"generaing {schema}.{name} skipped");
										}
									}
								}
							}
						}
					}
				}
				else
                {
					Logger.Warn($"no native type given.");
				}
			}
			catch (Exception e)
			{
				Logger.Danger(e, $"Generate('{path}', '{nativeType}') Error");
			}
		}
		private void GenerateInternal(GenerationType generateType, SqlObjectType type, object subType)
        {
			var nativeType = SqlGeneratorHelper.GetNativeType(subType);
			var path = Options.GetPath(type, subType);

			CreateDir(path);

			Generate(path, generateType, nativeType);
		}
		private void GenerateAll(GenerationType generateType, SqlObjectType type, Type subType)
		{
			var subTypes = Enum.GetValues(subType);

            foreach (var value in subTypes)
            {
				GenerateInternal(generateType, type, value);
            }
		}
		public void Generate(GenerationType generateType, SqlObjectType type, object subType = null)
        {
			var strSubType = subType?.ToString();

			switch (type)
            {
				case SqlObjectType.Sproc:
					if (string.IsNullOrEmpty(strSubType) || string.Compare(strSubType, "all", true) == 0)
					{
						GenerateAll(generateType, type, typeof(SqlSprocType));
					}
					else
					{
						var sprocType = SqlGeneratorHelper.GetSubType<SqlSprocType>(subType);

						if (sprocType.HasValue)
						{
							GenerateInternal(generateType, type, sprocType.Value);
						}
						else
                        {
							Logger.Danger($"No subtype specified or subtype ('{subType}') is invalid.");
                        }
					}

					break;
				case SqlObjectType.Udf:
					if (string.IsNullOrEmpty(strSubType) || string.Compare(strSubType, "all", true) == 0)
					{
						GenerateAll(generateType, type, typeof(SqlUdfType));
					}
					else
					{
						var udfType = SqlGeneratorHelper.GetSubType<SqlUdfType>(subType);

						if (udfType.HasValue)
						{
							GenerateInternal(generateType, type, udfType.Value);
						}
						else
						{
							Logger.Danger($"No subtype specified or subtype ('{subType}') is invalid.");
						}
					}

					break;
				case SqlObjectType.View:
					GenerateInternal(generateType, type, SqlViewType.Sql);

					break;
				case SqlObjectType.Trigger:
					var triggerType = SqlGeneratorHelper.GetSubType<SqlTriggerType>(subType);

					if (triggerType.HasValue)
					{
						GenerateInternal(generateType, type, triggerType.Value);
					}
					else
					{
						Logger.Danger($"No subtype specified or subtype ('{subType}') is invalid.");
					}

					break;
			}
		}
    }
}
