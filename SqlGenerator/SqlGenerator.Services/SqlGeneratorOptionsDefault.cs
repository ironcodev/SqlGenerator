using System.Linq;
using System.Collections.Generic;

namespace SqlGenerator.Services
{
    public class SqlGeneratorOptionsDefault: SqlGeneratorOptionsBase
    {
        public string BasePath { get; set; }
        public SqlGeneratorOptionsDefault(): this(null)
        { }
        public SqlGeneratorOptionsDefault(ILogger logger): base(logger)
        {
            BasePath = "/Scripts";
        }
        protected string GetKey(SqlObjectType type, object subType = null)
        {
            var result = "";

            switch (type)
            {
                case SqlObjectType.Sproc:
                    var sprocType = SqlGeneratorHelper.GetSubType<SqlSprocType>(subType);
                    result = $"/Procedures/{sprocType}";
                    break;
                case SqlObjectType.Udf:
                    var udfType = SqlGeneratorHelper.GetSubType<SqlUdfType>(subType);
                    result = $"/Functions/{udfType}";
                    break;
                case SqlObjectType.Table:
                    var tblType = SqlGeneratorHelper.GetSubType<SqlTableType>(subType);
                    result = $"/Tables/{tblType}";
                    break;
                case SqlObjectType.Type:
                    var udtType = SqlGeneratorHelper.GetSubType<SqlUdtType>(subType);
                    result = $"/Types/{udtType}";
                    break;
                case SqlObjectType.View:
                    var viewType = SqlGeneratorHelper.GetSubType<SqlViewType>(subType);
                    result = $"/Views/{viewType}";
                    break;
                case SqlObjectType.Constraint:
                    var constraintType = SqlGeneratorHelper.GetSubType<SqlConstraintType>(subType);
                    result = $"/Constraints/{constraintType}";
                    break;
                case SqlObjectType.Key:
                    var keyType = SqlGeneratorHelper.GetSubType<SqlKeyType>(subType);
                    result = $"/Keys/{keyType}";
                    break;
                case SqlObjectType.Index: result = "Indexes"; break;
                case SqlObjectType.Assembly: result = "/Assemblies"; break;
                case SqlObjectType.SecurityPolicy: result = "/SecurityPolicies"; break;
                default:
                    result = $"/{type}s{(subType == null ? "": $"/{subType}")}";
                    break;
            }

            return result;
        }
        public override string GetPath(SqlObjectType type, object subType = null)
        {
            var result = GetKey(type, subType);

            return BasePath + result;
        }
    }
}
