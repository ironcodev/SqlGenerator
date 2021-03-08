@echo off

echo Concatenating Views ...

rem --------- Views -----------
IF EXIST ".\Scripts\Views\Sql\All.sql" (
  del ".\Scripts\Views\Sql\All.sql"
)

IF EXIST ".\Scripts\Views\Sql\*.sql" (
	copy Scripts\Views\Sql\*.sql Scripts\Views\Sql\All.sql > NUL
)

rem ======== FINAL ==========

IF EXIST ".\Scripts\Views.sql" (
  del ".\Scripts\Views.sql"
)

copy Scripts\Views\Sql\All.sql Scripts\Views.sql  > NUL
		
IF EXIST ".\Scripts\Views\Sql\All.sql" (
  del ".\Scripts\Views\Sql\All.sql"
)

echo Done
