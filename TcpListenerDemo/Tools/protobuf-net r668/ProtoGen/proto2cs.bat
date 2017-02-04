@echo off
for /f "delims=" %%i in ('dir /b proto "proto/*.proto"') do protogen -i:proto/%%i -o:cs/%%~ni.cs
pause