using System;
using System.Collections.Generic;
using System.Text;

namespace SqlGenerator.Services
{
    public enum SqlObjectNativeType: int
    {
        AGGREGATE_FUNCTION = 1,
        CLR_SCALAR_FUNCTION = 2,
        CLR_STORED_PROCEDURE = 4,
        DEFAULT_CONSTRAINT = 8,
        EXTENDED_STORED_PROCEDURE = 16,
        FOREIGN_KEY_CONSTRAINT = 32,
        INTERNAL_TABLE = 64,
        PRIMARY_KEY_CONSTRAINT = 128,
        SERVICE_QUEUE = 256,
        SQL_INLINE_TABLE_VALUED_FUNCTION = 512,
        SQL_SCALAR_FUNCTION = 1024,
        SQL_STORED_PROCEDURE = 2048,
        SQL_TABLE_VALUED_FUNCTION = 4096,
        SQL_TRIGGER = 8132,
        SYNONYM = 16384,
        SYSTEM_TABLE = 32768,
        TYPE_TABLE = 65536,
        UNIQUE_CONSTRAINT = 131072,
        USER_TABLE = 262144,
        VIEW = 524288
    }
    public enum SqlObjectType
    {
        None,
        Sproc,
        Udf,
        Table,
        Type,
        Trigger,
        Index,
        View,
        Synonym,
        Constraint,
        FileGroup,
        File,
        Partition,
        Assembly,
        Rule,
        Sequence,
        Diagram,
        Schema,
        User,
        Role,
        Permission,
        Key,
        Certificate,
        SecurityPolicy,
        Audit,
        Default,
        Database

    /* other object types:
        AvailabilityGroup
        BrokerPriority
        Contract
        Credential
        CryptographicProvider
        DatabaseScopedCredential
        Endpoint
        EventNotification
        EventSession
        ExternalDataSource
        ExternalLanguage
        ExternalLibrary
        ExternalFileFormat
        ExternalResourcePool
        FullTextCatalog
        FullTextStopList
        Login
        MessageType
        PartitionFunction
        PartitionScheme
        Queue
        RemoteServiceBinding
        ResourcePool
        Route
        SearchPropertyList
        Service
        Statistics
        WorkLoadGroup
        XmlSchemaCollection
    */
    }
    public enum GenerationType
    {
        NotSpecified,
        DropCreate,
        Drop,
        Create
    }
    public enum SqlAuditType
    {
        DatabaseAuditSpec,
        Server,
        ServerAuditSpec
    }
    public enum SqlRoleType
    {
        Database,
        Server
    }
    public enum SqlIndexType
    {
        Clustered,
        NoneClustered,
        UniqueNoneClustered,
        ColumnStore,
        FullText,
        SelectiveXml, 
        Spatial, 
        Xml
    }
    public enum SqlKeyType
    {
        ColumnEncryptionKey,
        ColumnMasterKey,
        DatabaseEncryptionKey,
        DatabaseMasterKey,
        MasterKey,
        SymmetricKey,
        AsymmetricKey
    }
    public enum SqlConstraintType
    {
        PrimaryKey,
        ForeignKey,
        Default
    }
    public enum SqlTableType
    {
        Sql,
        External,
        File,
        Graph
    }
    public enum SqlViewType
    {
        Sql
    }
    public enum SqlSprocType
    {
        Sql,
        Clr,
        Extended
    }
    public enum SqlUdtType
    {
        Scaler,     // User-Defined Data Types
        Table,      // User-Defined Table Types
        Clr,        // User-Defined Types
        XmlSchemaCollection
    }
    public enum SqlUdfType
    {
        Scaler,
        Table,
        Inline,
        Clr,
        Aggregate
    }
    public enum SqlTriggerType
    {
        Database,
        Server
    }
}
