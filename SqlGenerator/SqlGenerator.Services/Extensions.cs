using System;
using System.Collections.Generic;
using System.Text;

namespace SqlGenerator.Services
{
    public static class Extensions
    {
        public static void GenerateProcedures(this ISqlGenerator generator, GenerationType generateType = GenerationType.DropCreate)
        {
            generator.Generate(generateType, SqlObjectType.Sproc, "Sql");
        }
        #region SqlGenerator
        public static void GenerateProcedures(this ISqlGenerator generator, GenerationType generateType, SqlObjectType type, SqlSprocType subType)
        {
            generator.Generate(generateType, type, subType);
        }
        public static void GenerateFunctions(this ISqlGenerator generator, GenerationType generateType, SqlObjectType type, SqlUdfType subType)
        {
            generator.Generate(generateType, type, subType);
        }
        public static void GenerateTables(this ISqlGenerator generator, GenerationType generateType, SqlObjectType type, SqlTableType subType)
        {
            generator.Generate(generateType, type, subType);
        }
        public static void GenerateViews(this ISqlGenerator generator, GenerationType generateType, SqlObjectType type, SqlViewType subType)
        {
            generator.Generate(generateType, type, subType);
        }
        public static void GenerateTypes(this ISqlGenerator generator, GenerationType generateType, SqlObjectType type, SqlUdtType subType)
        {
            generator.Generate(generateType, type, subType);
        }
        #endregion
        #region Logging
        public static void Log(this ILogger logger, string data)
        {
            logger.Info(data);
        }
        public static void Info(this ILogger logger, string data)
        {
            logger.Log(LogType.Info, data);
        }
        public static void Success(this ILogger logger, string data)
        {
            logger.Log(LogType.Success, data);
        }
        public static void Warn(this ILogger logger, string data)
        {
            logger.Log(LogType.Warning, data);
        }
        public static void Danger(this ILogger logger, string data)
        {
            logger.Log(LogType.Danger, data);
        }
        public static void Danger(this ILogger logger, Exception e, string data = "")
        {
            if (!string.IsNullOrEmpty(data))
            {
                logger.Log(LogType.Danger, data);
            }
            logger.Log(LogType.Danger, e.ToString("\n"));
            logger.Log(LogType.Danger, e.StackTrace);
        }
        #endregion
        public static string ToString(this Exception exception, string separator)
        {
            var sb = new StringBuilder();
            var current = exception;
            var i = 0;

            while (current != null)
            {
                sb.Append((i++ > 0 ? separator: "") + current.Message);

                current = current.InnerException;
            }

            return sb.ToString();
        }
    }
}
