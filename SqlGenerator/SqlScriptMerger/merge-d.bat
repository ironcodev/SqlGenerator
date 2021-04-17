@ECHO OFF

IF "%~1"=="" (
	ECHO Please Specify version
) ELSE (
	IF NOT EXIST Builds MD Builds
	IF NOT EXIST Builds\%1 MD Builds\%1
	IF NOT EXIST Builds\%1\Debug MD Builds\%1\Debug

	ECHO Merging ...

	ilmerge /log:merge.log /v4 /target:exe /out:sqlmrgr.exe ^
			./bin/debug/SqlScriptMerger.exe ^
			./bin/debug/SqlGenerator.Extensions.dll ^
			./bin/debug/Newtonsoft.Json.dll

	move /Y sqlmrgr.* Builds\%1\Debug

	ECHO Done!
)

