TITLE Selenium Node
REM -trustAllSSLCertificates --disable-web-security
REM nssm install Selenium-Server-Node "C:\Program Files (x86)\Java\jre7\bin\java" -jar "C:\grid\selenium-server-standalone-2.42.0.jar" -nodeConfig "C:\grid\node.json" -role node
"C:\Program Files (x86)\Java\jre7\bin\java" -jar selenium-server-standalone-2.42.0.jar -role node -Dwebdriver.ie.driver="IEDriverServer.exe" -Dwebdriver.chrome.driver="chromedriver.exe" -nodeConfig selenium_all_config.json