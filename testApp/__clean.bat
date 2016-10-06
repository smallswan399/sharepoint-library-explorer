@echo off
del *.dsk
del *.dcu
del *.ddp
del *.drc
del *.identcache
del *.local
del *.~*
del *.backup*
del *.backup_*

if exist .\_DCU\NUL goto 1
goto end

:1
cd _dcu
del *.dsk
del *.dcu
del *.ddp
del *.drc
del *.identcache
del *.local
del *.~*
cd..

:end

