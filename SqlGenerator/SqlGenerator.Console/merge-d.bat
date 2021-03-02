@ECHO OFF

IF "%~1"=="" (
	ECHO Please Specify version
) ELSE (
	IF NOT EXIST Builds MD Builds
	IF NOT EXIST Builds\%1 MD Builds\%1
	IF NOT EXIST Builds\%1\Debug MD Builds\%1\Debug

	ECHO Merging ...

	ilmerge /log:merge.log /v4 /target:exe /out:sqlgen.exe ^
			./bin/debug/SqlGenerator.Console.exe ^
			./bin/debug/SqlGenerator.Services.dll ^
			./bin/debug/System.Data.SqlClient.dll

	move /Y sqlgen.* Builds\%1\Debug

	ECHO Done!
)

