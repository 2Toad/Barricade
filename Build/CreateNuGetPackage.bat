@echo off
cls

echo Barricade
echo Copyright (C)2013 2Toad, LLC.
echo licensing@2toad.com
echo https://github.com/2Toad/Barricade

echo.
echo.
echo THIS BATCH FILE GENERATES A NUGET PACKAGE
echo.
echo.

echo Press CTRL+C to abort
pause
echo.
echo.

dotnet pack "../Src/Barricade/project.json" -c Release
pause