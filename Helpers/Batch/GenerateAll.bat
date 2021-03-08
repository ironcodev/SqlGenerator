@echo off

echo
echo Generating Tables ...
Generate.bat Table Create

echo
echo Generating Procedures ...
Generate.bat Sproc

echo
echo Generating Functions ...
Generate.bat Udf

echo
echo Generating Triggers ...
Generate.bat Trigger

echo
echo Generating Views ...
Generate.bat View

echo
echo Finished