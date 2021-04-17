@ECHO OFF

IF "%~1"=="" (
	ECHO Please Specify version
) ELSE (
	REM ilmerge does not supporte .NET Standard libraries. So, we need to delete the following .pdb files.
	
	IF EXIST ".\bin\debug\SqlGenerator.Extensions.pdb" (
	  del ".\bin\debug\SqlGenerator.Extensions.pdb"
	)
	IF EXIST ".\bin\release\SqlGenerator.Extensions.pdb" (
	  del ".\bin\release\SqlGenerator.Extensions.pdb"
	)
	
	merge-d.bat %1
	merge-r.bat %1
	
	IF NOT EXIST ..\..\Builds MD ..\..\Builds
	IF NOT EXIST ..\..\Builds\sqldepx MD ..\..\Builds\sqldepx
	IF NOT EXIST ..\..\Builds\sqldepx\%1 MD ..\..\Builds\sqldepx\%1
	
	xcopy .\Builds\%1 ..\..\Builds\sqldepx\%1 /Y/S > NUL
	
	echo Finished.
)

