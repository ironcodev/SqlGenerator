using System;

namespace SqlGenerator.Services
{
    public class SqlGeneratorOptions
    {
        public string BasePath { get; set; }
        public string Database { get; set; }
        public string ConnectionString { get; set; }
        public string TargetPath { get; set; }
        public bool OverwriteExisting { get; set; }
        public SqlObjectType ObjectTypes { get; set; }
        public SqlGeneratorOptions()
        {
            BasePath = "/Scripts";
        }
        public virtual string GetPath(SqlObjectType type, object subType = null)
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
                case SqlObjectType.Trigger: result = $"/Triggers"; break;
                case SqlObjectType.Index: result = "Indexes"; break;
                case SqlObjectType.View:
                    var viewType = SqlGeneratorHelper.GetSubType<SqlViewType>(subType);
                    result = $"/Views/{viewType}";
                    break;
                case SqlObjectType.Synonym: result = "/Synonyms"; break;
                case SqlObjectType.Constraint:
                    var constraintType = SqlGeneratorHelper.GetSubType<SqlConstraintType>(subType);
                    result = $"/Constraints/{constraintType}";
                    break;
                case SqlObjectType.FileGroup: result = "/FileGroups"; break;
                case SqlObjectType.File: result = ""; break;
                case SqlObjectType.Partition: result = "/File"; break;
                case SqlObjectType.Assembly: result = "/Assemblies"; break;
                case SqlObjectType.Rule: result = "/Rules"; break;
                case SqlObjectType.Sequence: result = "/Sequences"; break;
                case SqlObjectType.Diagram: result = "/Diagrams"; break;
                case SqlObjectType.Schema: result = "/Schemas"; break;
                case SqlObjectType.User: result = "/Users"; break;
                case SqlObjectType.Role: result = "/Roles"; break;
                case SqlObjectType.Permission: result = "/Permissions"; break;
                case SqlObjectType.Certificate: result = "/Certificates"; break;
                case SqlObjectType.SecurityPolicy: result = "/SecurityPolicies"; break;
                case SqlObjectType.Key:
                    var keyType = SqlGeneratorHelper.GetSubType<SqlKeyType>(subType);
                    result = $"/Keys/{keyType}";
                    break;
                default:
                    break;
            }

            return BasePath + result;
        }
    }
}
