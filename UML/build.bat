@echo off

echo Obfuscating application...
"%USERPROFILE%\.nuget\packages\obfuscar\2.2.41\tools\Obfuscar.Console.exe" obfuscar.xml

pause