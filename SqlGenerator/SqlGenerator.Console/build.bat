@ECHO OFF

IF "%~1"=="" (
	ECHO Please Specify version
) ELSE (
	REM ilmerge does not supporte .NET Standard libraries. So, we need to delete the following .pdb files.
	
	IF EXIST ".\bin\debug\SqlGenerator.Services.pdb" (
	  del ".\bin\debug\SqlGenerator.Services.pdb"
	)
	IF EXIST ".\bin\release\SqlGenerator.Services.pdb" (
	  del ".\bin\release\SqlGenerator.Services.pdb"
	)
	
	merge-d.bat %1
	merge-r.bat %1
	
	IF NOT EXIST ..\..\Builds MD ..\..\Builds
	IF NOT EXIST ..\..\Builds\%1 MD ..\..\Builds\%1
	
	xcopy .\Builds\%1 ..\..\Builds\%1 /Y/S > NUL
	
	echo Finished.
)

