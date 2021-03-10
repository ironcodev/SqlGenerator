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
		private SqlGeneratorOptionsBase options;
        public SqlGeneratorOptionsBase Options
        {
            get
            {
                if (options == null)
                {
                    options = new SqlGeneratorOptionsDefault();
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
		private void InitDatabase(SqlConnection con)
        {
			if (!string.IsNullOrEmpty(Options.Database))
			{
				var cmd = new SqlCommand("use [" + Options.Database.Replace("'", "''") + "]", con);

				cmd.ExecuteNonQuery();
			}
		}
        #region Generate
        string GetText(SqlObjectType type, string schema, string name, out bool error)
		{
			var result = "";

			error = true;

			try
			{
				switch (type)
                {
					case SqlObjectType.Default:
						using (var con = new SqlConnection(Options.ConnectionString))
						{
							using (var cmd = new SqlCommand($@"
select
'
IF EXISTS(SELECT 1 FROM sys.default_constraints WHERE name = ''' + dc.name + ''' AND schema_id = ' + CAST(dc.schema_id AS VARCHAR(100)) + ')
	ALTER TABLE [' + SCHEMA_NAME(t.schema_id) + '].[' + t.name + '] DROP CONSTRAINT [' + dc.name + ']
GO
ALTER TABLE [' + SCHEMA_NAME(t.schema_id) + '].[' + t.name + '] ADD CONSTRAINT [' + dc.name + ']  DEFAULT ' + dc.definition + ' FOR [' + c.name + ']
GO
'
FROM			sys.default_constraints	dc
	INNER JOIN	sys.tables				t ON dc.parent_object_id = t.object_id
	INNER JOIN	sys.columns				c ON dc.parent_column_id = c.column_id and c.object_id = dc.parent_object_id
WHERE dc.name = @name", con))
							{
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.AddWithValue("@name", name);
								cmd.Parameters.AddWithValue("@schema", schema);
								con.Open();

								InitDatabase(con);

								result = cmd.ExecuteScalar()?.ToString();
							}
						}

						error = !string.IsNullOrEmpty(result);

						break;
					case SqlObjectType.Table:
						using (var con = new SqlConnection(Options.ConnectionString))
						{
							using (var cmd = new SqlCommand($"exec sp_GetDDL @name, 0;", con))
							{
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.AddWithValue("@name", $"[{schema}].[{name}]");
								con.Open();

								InitDatabase(con);

								var sb = new StringBuilder();

								using (var reader = cmd.ExecuteReader())
								{
									while (reader.Read())
									{
										sb.Append(reader["Item"]?.ToString());
									}
								}

								sb.Append("\r\nGO\r\n");

								result = sb.ToString();
							}
						}

						error = false;

						break;
					default:
						using (var con = new SqlConnection(Options.ConnectionString))
						{
							using (var cmd = new SqlCommand($"EXEC sp_helptext @name;", con))
							{
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.AddWithValue("@name", $"[{schema}].[{name}]");
								con.Open();

								InitDatabase(con);
								
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
						
						break;
                }
			}
			catch (Exception e)
			{
				Logger.Danger(e, $"GetText('{schema}', '{name}') failed");
			}

			return result;
		}
		void CreateDir(string path)
		{
			if (!Directory.Exists(Options.OutputPath + path))
			{
				Directory.CreateDirectory(Options.OutputPath + path);
			}
		}
		private void Generate(string path, GenerationType generateType, SqlObjectType type, string nativeType, string keyword)
		{
			try
			{
				Logger.Log($"-------------------------------------------------------------");

				if (!string.IsNullOrEmpty(nativeType))
				{
					Logger.Log($"Generating {nativeType} objects in {path} ...");

					using (var con = new SqlConnection(Options.ConnectionString))
					{
						using (var cmd = new SqlCommand($@" select schema_name(schema_id), name from sys.all_objects
															where	type_desc=@nativeType
																	and is_ms_shipped = 0
																	and (name like '%' + @keyword + '%' or len(rtrim(ltrim(isnull(@keyword, '')))) = 0)", con))
						{
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.AddWithValue("@nativeType", nativeType);
							cmd.Parameters.AddWithValue("@keyword", keyword);
							con.Open();

							InitDatabase(con);

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
											text = @"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

" + GetText(type, schema, name, out error);
											text += "\n" + ObjectSeparator;
											break;
										case GenerationType.Drop:
											text = $"drop {SqlGeneratorHelper.GetLogicalType(nativeType)} {schema}.{name}";

											break;
										case GenerationType.DropCreate:
											text = $@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select 1 from sys.all_objects where name='{name}' and schema_id = schema_id('{schema}'))
	drop {SqlGeneratorHelper.GetLogicalType(nativeType)} {schema}.{name}
go
";
											text += GetText(type, schema, name, out error);
											text += "\n" + ObjectSeparator;
											break;
                                    }

									if (!error)
									{
										Writer.Options = new FileScriptWriterOptions
										{
											FileName = schema + "." + name + ".sql",
											Path = Options.OutputPath + path,
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
				Logger.Danger(e, $"Generate('{path}', '{nativeType}', '{keyword}') Error");
			}
		}
		private void GenerateInternal(GenerationType generateType, SqlObjectType type, object subType, string keyword)
        {
			var nativeType = SqlGeneratorHelper.GetNativeType(subType);

			if (string.IsNullOrEmpty(nativeType))
			{
				Logger.Warn($"Warning: {type}.{subType} is not supported yet. Generating script for {type}.{subType} objects skipped.");
			}
			else
			{
				var path = Options.GetPath(type, subType);

				CreateDir(path);

				Generate(path, generateType, type, nativeType, keyword);
			}
		}
		private void GenerateAll(GenerationType generateType, SqlObjectType type, Type subType, string keyword)
		{
			var subTypes = Enum.GetValues(subType);

            foreach (var value in subTypes)
            {
				GenerateInternal(generateType, type, value, keyword);
            }
		}
		public void Generate(GenerationType generateType, SqlObjectType type, object subType = null, string keyword = "")
        {
			var strSubType = subType?.ToString();

			switch (type)
            {
				case SqlObjectType.Sproc:
					if (string.IsNullOrEmpty(strSubType) || string.Compare(strSubType, "all", true) == 0)
					{
						GenerateAll(generateType, type, typeof(SqlSprocType), keyword);
					}
					else
					{
						var sprocType = SqlGeneratorHelper.GetSubType<SqlSprocType>(subType);

						if (sprocType.HasValue)
						{
							GenerateInternal(generateType, type, sprocType.Value, keyword);
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
						GenerateAll(generateType, type, typeof(SqlUdfType), keyword);
					}
					else
					{
						var udfType = SqlGeneratorHelper.GetSubType<SqlUdfType>(subType);

						if (udfType.HasValue)
						{
							GenerateInternal(generateType, type, udfType.Value, keyword);
						}
						else
						{
							Logger.Danger($"No subtype specified or subtype ('{subType}') is invalid.");
						}
					}

					break;
				case SqlObjectType.Table:
					var sp_getDDL_exists = false;

					using (var con = new SqlConnection(Options.ConnectionString))
					{
						using (var cmd = new SqlCommand($"use master;select case when exists(select 1 from sys.procedures where name = 'sp_GetDDL') then 1 else 0 end", con))
						{
							con.Open();
							sp_getDDL_exists = ((int)cmd.ExecuteScalar()) == 1;
						}
					}

					if (!sp_getDDL_exists)
					{
						Logger.Warn($"sp_GetDDL was not found. It is required to generate scripts of Tables. You can download it from https://github.com/ironcodev/SqlGenerator");
					}
					else
                    {
						if (string.IsNullOrEmpty(strSubType) || string.Compare(strSubType, "all", true) == 0)
						{
							GenerateAll(generateType, type, typeof(SqlTableType), keyword);
						}
						else
						{
							var tableType = SqlGeneratorHelper.GetSubType<SqlTableType>(subType);

							if (tableType.HasValue)
							{
								GenerateInternal(generateType, type, tableType.Value, keyword);
							}
							else
							{
								Logger.Danger($"No subtype specified or subtype ('{subType}') is invalid.");
							}
						}
					}

					break;
				case SqlObjectType.View:
					GenerateInternal(generateType, type, SqlViewType.Sql, keyword);

					break;
				case SqlObjectType.Trigger:
					var triggerType = SqlGeneratorHelper.GetSubType<SqlTriggerType>(subType);

					if (triggerType.HasValue)
					{
						GenerateInternal(generateType, type, triggerType.Value, keyword);
					}
					else
					{
						Logger.Danger($"No subtype specified or subtype ('{subType}') is invalid.");
					}

					break;
			}
		}
        #endregion
        #region Count
        private int Count(string nativeType, string keyword)
		{
			var result = -1;

			try
			{
				Logger.Log($"-------------------------------------------------------------");

				if (!string.IsNullOrEmpty(nativeType))
				{
					Logger.Log($"Counting {nativeType} objects ...");

					using (var con = new SqlConnection(Options.ConnectionString))
					{
						using (var cmd = new SqlCommand($@" select count(*) from sys.all_objects
															where	type_desc=@nativeType
																	and is_ms_shipped = 0
																	and (name like '%' + @keyword + '%' or len(rtrim(ltrim(isnull(@keyword, '')))) = 0)", con))
						{
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.AddWithValue("@nativeType", nativeType);
							cmd.Parameters.AddWithValue("@keyword", keyword);
							con.Open();

							InitDatabase(con);
							
							result = (int)cmd.ExecuteScalar();
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
				Logger.Danger(e, $"Count(''{nativeType}', '{keyword}') Error");
			}

			return result;
		}
		private KeyValuePair<string, int> CountInternal(SqlObjectType type, object subType, string keyword)
		{
			var nativeType = SqlGeneratorHelper.GetNativeType(subType);
			KeyValuePair<string, int> result;

			if (string.IsNullOrEmpty(nativeType))
			{
				result = new KeyValuePair<string, int>(subType.ToString(), 0);

				Logger.Warn($"Warning: {type}.{subType} is not supported yet. Counting {type}.{subType} objects skipped.");
			}
			else
			{
				var count = Count(nativeType, keyword);
				result = new KeyValuePair<string, int>(subType.ToString(), count);
			}

			return result;
		}
		private Dictionary<string, int> CountAll(SqlObjectType type, Type subType, string keyword)
		{
			var result = new Dictionary<string, int>();
			var subTypes = Enum.GetValues(subType);

			foreach (var value in subTypes)
			{
				var kv = CountInternal(type, value, keyword);

				result.Add(kv.Key, kv.Value);
			}

			return result;
		}
		public Dictionary<string, int> Count(SqlObjectType type, object subType = null, string keyword = "")
		{
			var result = new Dictionary<string, int>();
			var strSubType = subType?.ToString();
			KeyValuePair<string, int> kv;

			switch (type)
			{
				case SqlObjectType.Sproc:
					if (string.IsNullOrEmpty(strSubType) || string.Compare(strSubType, "all", true) == 0)
					{
						result = CountAll(type, typeof(SqlSprocType), keyword);
					}
					else
					{
						var sprocType = SqlGeneratorHelper.GetSubType<SqlSprocType>(subType);

						if (sprocType.HasValue)
						{
							kv = CountInternal(type, sprocType.Value, keyword);

							result.Add(kv.Key, kv.Value);
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
						result = CountAll(type, typeof(SqlUdfType), keyword);
					}
					else
					{
						var udfType = SqlGeneratorHelper.GetSubType<SqlUdfType>(subType);

						if (udfType.HasValue)
						{
							kv = CountInternal(type, udfType.Value, keyword);

							result.Add(kv.Key, kv.Value);
						}
						else
						{
							Logger.Danger($"No subtype specified or subtype ('{subType}') is invalid.");
						}
					}

					break;
				case SqlObjectType.Table:
					{
						if (string.IsNullOrEmpty(strSubType) || string.Compare(strSubType, "all", true) == 0)
						{
							result = CountAll(type, typeof(SqlTableType), keyword);
						}
						else
						{
							var tableType = SqlGeneratorHelper.GetSubType<SqlTableType>(subType);

							if (tableType.HasValue)
							{
								kv = CountInternal(type, tableType.Value, keyword);

								result.Add(kv.Key, kv.Value);
							}
							else
							{
								Logger.Danger($"No subtype specified or subtype ('{subType}') is invalid.");
							}
						}
					}

					break;
				case SqlObjectType.View:
					kv = CountInternal(type, SqlViewType.Sql, keyword);

					result.Add(kv.Key, kv.Value);

					break;
				case SqlObjectType.Trigger:
					var triggerType = SqlGeneratorHelper.GetSubType<SqlTriggerType>(subType);

					if (triggerType.HasValue)
					{
						kv = CountInternal(type, triggerType.Value, keyword);

						result.Add(kv.Key, kv.Value);
					}
					else
					{
						Logger.Danger($"No subtype specified or subtype ('{subType}') is invalid.");
					}

					break;
			}

			return result;
		}
		#endregion
	}
}
