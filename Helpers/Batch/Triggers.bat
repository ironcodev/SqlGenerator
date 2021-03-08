@echo off

echo Concatenating Triggers ...

rem --------- Triggers -----------

IF EXIST ".\Scripts\Triggers\All.sql" (
  del ".\Scripts\Triggers\All.sql"
)

IF EXIST ".\Scripts\Triggers\Database\*.sql" (
	copy Scripts\Triggers\Database\*.sql Scripts\Triggers\All.sql > NUL
)

rem ======== FINAL ==========

IF EXIST ".\Scripts\Triggers.sql" (
  del ".\Scripts\Triggers.sql"
)

copy Scripts\Triggers\All.sql Scripts\Triggers.sql  > NUL
		
IF EXIST ".\Scripts\Triggers\All.sql" (
  del ".\Scripts\Triggers\All.sql"
)

echo Done
