@echo off

set SRC=ShaderSources\
set DEST=CompiledShaders\

"C:\Program Files (x86)\Windows Kits\8.1\bin\x86\fxc.exe" /T ps_3_0 %SRC%ps1.fx /Fo %DEST%ps1.fxc
"C:\Program Files (x86)\Windows Kits\8.1\bin\x86\fxc.exe" /T ps_3_0 %SRC%ps2.fx /Fo %DEST%ps2.fxc