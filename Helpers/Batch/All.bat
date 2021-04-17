
echo Generating All script ...

call Schemas.bat
call Types.bat
call Tables.bat
call Indexes.bat
call Views.bat
call Functions.bat
call Triggers.bat
call Procedures.bat
call Relations.bat
call Data.bat

IF EXIST ".\Scripts\All.sql" (
  del ".\Scripts\All.sql"
)

copy	Scripts\Schemas.sql 		+ ^
		Scripts\Types.sql 			+ ^
		Scripts\Tables.sql			+ ^
		Scripts\Indexes.sql			+ ^
		Scripts\Functions-Base.sql	+ ^
		Scripts\Views.sql			+ ^
		Scripts\Functions.sql		+ ^
		Scripts\Triggers.sql		+ ^
		Scripts\Procedures.sql		+ ^
		Scripts\Relations.sql		+ ^
		Scripts\Data.sql			Scripts\All.sql

echo Finished!