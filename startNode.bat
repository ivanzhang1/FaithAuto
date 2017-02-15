TITLE Selenium Node
set BROWSERFIRE=browserName=firefox,maxInstances=5,platform=WINDOWS
set BROWSERCHROME=browserName=chrome,maxInstances=5,platform=WINDOWS
set BROWSERIE="browserName=internet explorer,maxInstances=2,platform=WINDOWS"
"C:\Program Files (x86)\Java\jre7\bin\java" -jar ..\..\..\selenium-server-standalone-43.0.jar -role node -Dwebdriver.ie.driver="..\..\..\IEDriverServer.exe" -Dwebdriver.chrome.driver="..\..\..\chromedriver.exe" -hub http://localhost:4444/grid/register -browser %BROWSERFIRE% -browser %BROWSERCHROME% -browser %BROWSERIE% -maxSession 8
