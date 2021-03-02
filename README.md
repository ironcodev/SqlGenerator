# SqlGenerator
This is a tool to generate scripts for objects in SQL Server databases.

Current Version = 1.0.0

Usage:
```
sqlgen.exe [args]
```

    args:
        -v  display program version
        -c  specify connectionstring
        -d  specify database
        -s  silent
        -t  specify object types to generate script for. possible values are
                Table, Sproc, Udf, Type, Trigger, Index, View, Constraint, FileGroup, File
                Partition, Assembly, Rule, Sequence, Diagram, Schema, User, Role, Synonym, 
                Permission, Key, Certificate, SecurityPolicy, Audit, Default, Database
        -st specify subtype for chosen type. values depend on chosen object type in -t arg.
            Table: Sql (default), External, File, Graph, All
            Sproc: Sql (default), Clr, Extended, All
            Udf: Scaler, Table, Inline, Clr, Aggregate, UserDefined (default), All
            Trigger: Database (default), Server, All
            Type: Scaler, Table, Clr, Extended, UserDefined (default), All
            View: Sql (default), All,
            Constraint: PrimaryKey, ForeignKey, Default, AllButPk (default), All
            Index: Clustered, NoneClustered, UniqueNoneClustered, ColumnStore, FullText, SelectiveXml, Spatial, Xml, AllButClustered (default), All
            Audit: DatabaseAuditSpec, Server, ServerAuditSpec, All (default)
            Role: Database (default), Server, All
        -gt generation type. possible values are Drop, Create, DropCreate.
        -l  specify logger. possible values are: file (default), console, null
        -w  specify writer. possible values are: file (default), console, null
        -p  target path to create generated files in. default = ./Scripts
        -nw  do not overwrite existing files.
Example
```
    sqlgen.exe -c ""Server=.;Database=MyDb;User Id=myuser;Password=mypass"" -t Sproc
```
```
    sqlgen.exe -c ""Server=.;Database=MyDb;User Id=myuser;Password=mypass"" -t Sproc -w console
```

Currently, the tool only supports generating SPROCs and UDFs.
