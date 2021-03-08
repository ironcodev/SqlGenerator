@echo off
echo Concatenating Types ...
rem --------- Types -----------

IF EXIST ".\Scripts\Types\Scaler\All.sql" (
  del ".\Scripts\Types\Scaler\All.sql"
)

IF EXIST ".\Scripts\Types\Clr\All.sql" (
  del ".\Scripts\Types\Clr\All.sql"
)

IF EXIST ".\Scripts\Types\Table\All.sql" (
  del ".\Scripts\Types\Table\All.sql"
)

IF EXIST ".\Scripts\Types\XmlSchemaCollection\All.sql" (
  del ".\Scripts\Types\XmlSchemaCollection\All.sql"
)

IF EXIST ".\Scripts\Types\Scaler\*.sql" (
	copy Scripts\Types\Scaler\*.sql Scripts\Types\Scaler\All.sql > NUL
)
IF EXIST ".\Scripts\Types\Clr\*.sql" (
	copy Scripts\Types\Clr\*.sql Scripts\Types\Clr\All.sql > NUL
)
IF EXIST ".\Scripts\Types\Table\*.sql" (
	copy Scripts\Types\Table\*.sql Scripts\Types\Table\All.sql > NUL
)
IF EXIST ".\Scripts\Types\XmlSchemaCollection\*.sql" (
	copy Scripts\Types\XmlSchemaCollection\*.sql Scripts\Types\XmlSchemaCollection\All.sql > NUL
)
rem ======== FINAL ==========
IF EXIST ".\Scripts\Types.sql" (
  del ".\Scripts\Types.sql"
)

copy	Scripts\Types\Scaler\All.sql 				+ ^
		Scripts\Types\Clr\All.sql					+ ^
		Scripts\Types\XmlSchemaCollection\All.sql	+ ^
		Scripts\Types\Table\All.sql Scripts\Types.sql  > NUL
		
IF EXIST ".\Scripts\Types\Scaler\All.sql" (
  del ".\Scripts\Types\Scaler\All.sql"
)

IF EXIST ".\Scripts\Types\Clr\All.sql" (
  del ".\Scripts\Types\Clr\All.sql"
)

IF EXIST ".\Scripts\Types\Table\All.sql" (
  del ".\Scripts\Types\Table\All.sql"
)

IF EXIST ".\Scripts\Types\XmlSchemaCollection\All.sql" (
  del ".\Scripts\Types\XmlSchemaCollection\All.sql"
)

echo Done
