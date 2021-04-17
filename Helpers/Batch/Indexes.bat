@echo off

echo Concatenating Indexes ...

rem --------- Indexes -----------
IF EXIST ".\Scripts\Indexes\All.sql" (
  del ".\Scripts\Indexes\All.sql"
)

IF EXIST ".\Scripts\Indexes\*.sql" (
	copy Scripts\Indexes\*.sql Scripts\Indexes\All.sql > NUL
)

rem ======== FINAL ==========

IF EXIST ".\Scripts\Indexes.sql" (
  del ".\Scripts\Indexes.sql"
)

copy Scripts\Indexes\All.sql Scripts\Indexes.sql  > NUL
		
IF EXIST ".\Scripts\Indexes\All.sql" (
  del ".\Scripts\Indexes\All.sql"
)

echo Done
