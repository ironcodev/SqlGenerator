@echo off

echo Concatenating Tables ...

rem --------- Tables -----------

IF EXIST ".\Scripts\Tables\Sql\All.sql" (
  del ".\Scripts\Tables\Sql\All.sql"
)

IF EXIST ".\Scripts\Tables\External\All.sql" (
  del ".\Scripts\Tables\External\All.sql"
)

IF EXIST ".\Scripts\Tables\File\All.sql" (
  del ".\Scripts\Tables\File\All.sql"
)

IF EXIST ".\Scripts\Tables\Graph\All.sql" (
  del ".\Scripts\Tables\Graph\All.sql"
)

IF EXIST ".\Scripts\Tables\Sql\*.sql" (
	copy Scripts\Tables\Sql\*.sql Scripts\Tables\Sql\All.sql > NUL
)
IF EXIST ".\Scripts\Tables\External\*.sql" (
	copy Scripts\Tables\External\*.sql Scripts\Tables\External\All.sql > NUL
)
IF EXIST ".\Scripts\Tables\File\*.sql" (
	copy Scripts\Tables\File\*.sql Scripts\Tables\File\All.sql > NUL
)
IF EXIST ".\Scripts\Tables\Graph\*.sql" (
	copy Scripts\Tables\Graph\*.sql Scripts\Tables\Graph\All.sql > NUL
)

rem ======== FINAL ==========

IF EXIST ".\Scripts\Tables.sql" (
  del ".\Scripts\Tables.sql"
)

copy	Scripts\Tables\Sql\All.sql 	+ ^
		Scripts\Tables\External\All.sql	+ ^
		Scripts\Tables\File\All.sql	+ ^
		Scripts\Tables\Graph\All.sql Scripts\Tables.sql  > NUL
		
IF EXIST ".\Scripts\Tables\Sql\All.sql" (
  del ".\Scripts\Tables\Sql\All.sql"
)

IF EXIST ".\Scripts\Tables\External\All.sql" (
  del ".\Scripts\Tables\External\All.sql"
)

IF EXIST ".\Scripts\Tables\File\All.sql" (
  del ".\Scripts\Tables\File\All.sql"
)

IF EXIST ".\Scripts\Tables\Graph\All.sql" (
  del ".\Scripts\Tables\Graph\All.sql"
)

echo Done
