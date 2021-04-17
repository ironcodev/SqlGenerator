@echo off

echo Concatenating Relations ...

rem --------- Relations -----------
IF EXIST ".\Scripts\Relations\All.sql" (
  del ".\Scripts\Relations\All.sql"
)

IF EXIST ".\Scripts\Relations\*.sql" (
	copy Scripts\Relations\*.sql Scripts\Relations\All.sql > NUL
)

rem ======== FINAL ==========

IF EXIST ".\Scripts\Relations.sql" (
  del ".\Scripts\Relations.sql"
)

copy Scripts\Relations\All.sql Scripts\Relations.sql  > NUL
		
IF EXIST ".\Scripts\Relations\All.sql" (
  del ".\Scripts\Relations\All.sql"
)

echo Done
