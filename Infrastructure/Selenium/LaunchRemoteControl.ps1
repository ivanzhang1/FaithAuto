param($port)
# Get the IP address of the host
$hostIP = (gwmi Win32_NetworkAdapterConfiguration | ? { $_.IPAddress -ne $null }).ipaddress[0]
$hostName = [System.Net.Dns]::GetHostEntry($hostIP).HostName

# Launch a remote control
$java = 'C:\Program Files (x86)\Java\jre6\bin\java.exe'
$processArgs = "-classpath C:\Selenium\selenium-server-standalone-2.7.0.jar;C:\Selenium\selenium-grid-1.0.8\lib\selenium-grid-remote-control-standalone-1.0.8.jar com.thoughtworks.selenium.grid.remotecontrol.SelfRegisteringRemoteControlLauncher -port $port -host $hostName -huburl http://qats01.dev.corp.local:4444 -env *firefox"
start-process $java $processArgs