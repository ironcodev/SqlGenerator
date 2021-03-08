@echo off
echo Generating All script ...

Types.bat
Tables.bat
Functions.bat
Triggers.bat
Views.bat
Procedures.bat
Data.bat

IF EXIST ".\Scripts\All.sql" (
  del ".\Scripts\All.sql" > NUL
)

copy	Scripts\Types.sql 		+ ^
		Scripts\Tables.sql		+ ^
		Scripts\Functions.sql	+ ^
		Scripts\Triggers.sql	+ ^
		Scripts\Views.sql		+ ^
		Scripts\Procedures.sql	+ ^
		Scripts\Data.sql	Scripts\All.sql > NUL
	
echo Done!