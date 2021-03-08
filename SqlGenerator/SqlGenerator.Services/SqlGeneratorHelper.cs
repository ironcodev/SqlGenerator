using System;

namespace SqlGenerator.Services
{
    public static class SqlGeneratorHelper
    {
        public static T? GetSubType<T>(object subType) where T : struct
        {
            T? result = null;

            if (subType != null)
            {
                var type = subType.GetType();

                do
                {
                    if (type == typeof(T))
                    {
                        result = (T)subType;
                        break;
                    }

                    if (type == typeof(string))
                    {
                        T r;

                        if (Enum.TryParse(subType.ToString(), true, out r))
                        {
                            result = r;
                        }

                        break;
                    }

                    if (type == typeof(byte) || type == typeof(int) || type == typeof(short) || type == typeof(long) ||
                        type == typeof(sbyte) || type == typeof(uint) || type == typeof(ushort) || type == typeof(ulong) ||
                        type == typeof(decimal) || type == typeof(float) || type == typeof(double))
                    {
                        if (Enum.IsDefined(typeof(T), subType))
                        {
                            result = (T)subType;
                        }

                        break;
                    }
                } while (false);
            }

            return result;
        }
        public static string GetNativeType(object subType)
        {
            var result = "";
            var type = subType?.GetType();

            do
            {
                if (type == typeof(SqlSprocType))
                {
                    var sprocType = (SqlSprocType)subType;

                    switch (sprocType)
                    {
                        case SqlSprocType.Sql: result = "SQL_STORED_PROCEDURE"; break;
                        case SqlSprocType.Clr: result = "CLR_STORED_PROCEDURE"; break;
                        case SqlSprocType.Extended: result = "EXTENDED_STORED_PROCEDURE"; break;
                    }

                    break;
                }

                if (type == typeof(SqlUdfType))
                {
                    var udfType = (SqlUdfType)subType;

                    switch (udfType)
                    {
                        case SqlUdfType.Scaler: result = "SQL_SCALAR_FUNCTION"; break;
                        case SqlUdfType.Table: result = "SQL_TABLE_VALUED_FUNCTION"; break;
                        case SqlUdfType.Inline: result = "SQL_INLINE_TABLE_VALUED_FUNCTION"; break;
                        case SqlUdfType.Clr: result = "CLR_SCALAR_FUNCTION"; break;
                        case SqlUdfType.Aggregate: result = "AGGREGATE_FUNCTION"; break;
                    }

                    break;
                }

                if (type == typeof(SqlTableType))
                {
                    var tableType = (SqlTableType)subType;

                    switch (tableType)
                    {
                        case SqlTableType.Sql: result = "USER_TABLE"; break;
                        case SqlTableType.External: result = ""; break;
                        case SqlTableType.File: result = ""; break;
                        case SqlTableType.Graph: result = ""; break;
                    }

                    break;
                }

                if (type == typeof(SqlViewType))
                {
                    result = "VIEW";

                    break;
                }

                if (type == typeof(SqlTriggerType))
                {
                    result = "SQL_TRIGGER";

                    break;
                }
            } while (false);

            return result;
        }
        public static string GetLogicalType(string nativeType)
        {
            var result = "";

            switch (nativeType)
            {
                case "AGGREGATE_FUNCTION": result = "function"; break;
                case "CLR_SCALAR_FUNCTION": result = "function"; break;
                case "CLR_STORED_PROCEDURE": result = "procedure"; break;
                case "DEFAULT_CONSTRAINT": result = "constraint"; break;
                case "EXTENDED_STORED_PROCEDURE": result = "procedure"; break;
                case "FOREIGN_KEY_CONSTRAINT": result = "constraint"; break;
                case "INTERNAL_TABLE": result = "table"; break;
                case "PRIMARY_KEY_CONSTRAINT": result = ""; break;
                case "SERVICE_QUEUE": result = "service"; break;
                case "SQL_INLINE_TABLE_VALUED_FUNCTION": result = "function"; break;
                case "SQL_SCALAR_FUNCTION": result = "function"; break;
                case "SQL_STORED_PROCEDURE": result = "procedure"; break;
                case "SQL_TABLE_VALUED_FUNCTION": result = "function"; break;
                case "SQL_TRIGGER": result = "trigger"; break;
                case "SYNONYM": result = "synonym"; break;
                case "SYSTEM_TABLE": result = "table"; break;
                case "TYPE_TABLE": result = "type"; break;
                case "UNIQUE_CONSTRAINT": result = "constraint"; break;
                case "USER_TABLE": result = "table"; break;
                case "VIEW": result = "view"; break;
                default:
                    result = "Unknown";
                    break;
            }

            return result;
        }
    }
}
