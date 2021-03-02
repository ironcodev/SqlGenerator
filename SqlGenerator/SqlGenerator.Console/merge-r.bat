@ECHO OFF

IF "%~1"=="" (
	ECHO Please Specify version
) ELSE (
	IF NOT EXIST Builds MD Builds
	IF NOT EXIST Builds\%1 MD Builds\%1
	IF NOT EXIST Builds\%1\Release MD Builds\%1\Release
	
	ECHO Merging ...

	ilmerge /log:merge.log /v4 /target:exe /out:sqlgen.exe ^
			./bin/release/SqlGenerator.Console.exe ^
			./bin/release/SqlGenerator.Services.dll ^
			./bin/release/System.Data.SqlClient.dll

	move /Y sqlgen.* Builds\%1\Release

	ECHO Done!
)

