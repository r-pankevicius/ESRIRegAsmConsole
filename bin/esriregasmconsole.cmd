@rem Send Y<ENTER> to standard input to bypass "Terminate batch job (Y/N)?" prompt.
call %~dp0esriregasmconsole0.cmd %* < %~dp0esriregasmconsole-stdin-Y.txt
