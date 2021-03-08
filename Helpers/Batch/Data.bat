@echo off
echo Concatenating Data Scripts ...
rem --------- Data -----------
IF EXIST ".\Scripts\Data\Seed.sql" (
  del ".\Scripts\Data\Seed.sql"
)

copy	.\Scripts\Data\Seed.*.sql Scripts\Data\Seed.sql > NUL

IF EXIST ".\Scripts\Data\Messages.sql" (
  del ".\Scripts\Data\Messages.sql"
)

copy	.\Scripts\Data\Messages.*.sql Scripts\Data\Messages.sql > NUL

IF EXIST ".\Scripts\Data\Texts.sql" (
  del ".\Scripts\Data\Texts.sql"
)

copy	.\Scripts\Data\Texts.*.sql Scripts\Data\Texts.sql > NUL

rem ======== FINAL ==========
IF EXIST ".\Scripts\Data.sql" (
  del ".\Scripts\Data.sql"
)

copy	.\Scripts\Data\Seed.sql 	+ ^
		.\Scripts\Data\Messages.sql + ^
		.\Scripts\Data\Texts.sql	Scripts\Data.sql > NUL
		
echo Done
