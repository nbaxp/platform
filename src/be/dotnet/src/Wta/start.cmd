@echo off
%1 start mshta vbscript:createobject("wscript.shell").run("""%~0"" ::",0)(window.close)&&exit
cd /d %~dp0
start /b .\Wta.exe --urls http://*:5000
