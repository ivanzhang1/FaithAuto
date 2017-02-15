set pshell=%SystemRoot%\System32\WindowsPowerShell\v1.0\PowerShell.exe

if %USERNAME% EQU msneeden for /l %%x in (5555, 1, 5564) do PowerShell.exe -file LaunchRemoteControl.ps1 %%x