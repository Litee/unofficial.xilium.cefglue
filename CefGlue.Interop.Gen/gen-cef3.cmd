@echo off
C:\bin\miniconda3\envs\py27\python.exe -B cefglue_interop_gen.py --schema cef3 --cpp-header-dir include --cefglue-dir ..\CefGlue\ --no-backup
pause
