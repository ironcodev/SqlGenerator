@ECHO OFF

IF "%~1"=="" (
	ECHO Please Specify version
) ELSE (
	IF NOT EXIST Builds MD Builds
	IF NOT EXIST Builds\%1 MD Builds\%1
	IF NOT EXIST Builds\%1\release MD Builds\%1\release

	ECHO Merging ...

	ilmerge /log:merge.log /v4 /target:exe /out:sqldepx.exe ^
			./bin/release/SqlGenerator.DependencyExtractor.exe ^
			./bin/release/SqlGenerator.Extensions.dll ^
			./bin/release/Newtonsoft.Json.dll

	move /Y sqldepx.* Builds\%1\release

	ECHO Done!
)

