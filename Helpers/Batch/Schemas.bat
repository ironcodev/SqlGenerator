@echo off

echo Concatenating Schemas ...

rem --------- Schemas -----------
IF EXIST ".\Scripts\Schemas\All.sql" (
  del ".\Scripts\Schemas\All.sql"
)

IF EXIST ".\Scripts\Schemas\*.sql" (
	copy Scripts\Schemas\*.sql Scripts\Schemas\All.sql > NUL
)

rem ======== FINAL ==========

IF EXIST ".\Scripts\Schemas.sql" (
  del ".\Scripts\Schemas.sql"
)

copy Scripts\Schemas\All.sql Scripts\Schemas.sql  > NUL
		
IF EXIST ".\Scripts\Schemas\All.sql" (
  del ".\Scripts\Schemas\All.sql"
)

echo Done
