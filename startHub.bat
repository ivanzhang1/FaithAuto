TITLE Selenium Hub
"C:\Program Files (x86)\Java\jre7\bin\java" -jar ..\..\..\selenium-server-standalone-2.42.0.jar -role hub maxSession=15,maxInstances=15,
REM platform=WINDOWS,ensureCleanSession=true" -hubConfig ..\..\..\hubconfig.json
REM nssm install Selenium-Server-Hub "C:\Program Files (x86)\Java\jre7\bin\java" -jar "C:\grid\selenium-server-standalone-2.42.0.jar" -role hub -hubConfig "C:\grid\hub.json"