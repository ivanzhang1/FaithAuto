Clear-Host

Write-Host Setting working directory to (Get-Location)
Set-Location $psake.build_script_dir

# Add YAML functionality
Import-Module $env:WORKSPACE\ext\psyaml\yaml.powershell.dll

properties {
	$targetEnvironment = $env:DeployTo.ToUpper()

	switch ($targetEnvironment)
	{
		"QA" {$targetEnvironment = "LV_QA"}
		"UAT" {$targetEnvironment = "STAGING"}
		"PROD" {$targetEnvironment = "LV_PROD"}
	}

    $appConfigFile = "$env:WORKSPACE\Common\bin\Debug\Common.dll.config"
    $gallioPath = "C:\Program Files (x86)\Gallio\bin\Gallio.echo.exe"

	Write-Host "Loading config from $env:WORKSPACE\functional_ci.yml"	
	$ymlconfig = Import-YAML -Path $env:WORKSPACE\functional_ci.yml

	$reportArgs = $ymlconfig.properties.reportargs
	$smtpAddress = $ymlconfig.properties.smtpaddress
	$emailAddressList = @("FTQA@activenetwork.com")
    $emailAddressListApi = @("FTQA@activenetwork.com")
	$gridHost = $ymlconfig.properties.gridhost
	$port = $ymlconfig.properties.port
    $buildUser = $ymlconfig.properties.builduser
	$testRuns = $ymlconfig.properties.testruns
	$repeatonfail = $ymlconfig.properties.repeatonfail
	$amsEnabled = "true"
	$amsMigrated = "false"
	$amsChurches = "DC, QAEUNLX0C1, QAEUNLX0C2, QAEUNLX0C3, QAEUNLX0C4, QAEUNLX0C6, DCDMDFWTX"
	#Valid Test AMS Churches
	#DC, QAEUNLX0C1, QAEUNLX0C2, QAEUNLX0C3, QAEUNLX0C4, QAEUNLX0C6, DCDMDFWTX
	$amsTestChurches = "DC, QAEUNLX0C1, QAEUNLX0C2, QAEUNLX0C3, QAEUNLX0C4, QAEUNLX0C6, DCDMDFWTX"
	$jenkins = "true"
	$url =  $ymlconfig.portal.url -replace "targetenvironment",$env:DeployTo

}

task default -depend functionalCI

task functionalCI {

    if($env:BUILD_VERSION -eq $null){
		Write-Host "Build Version Unknown"
    }

    if($env:BUILD_USER -ne $null){
		Write-Host "Started by: $env:BUILD_USER"
		$buildUser = "$env:BUILD_USER"
    }

	Write-Host "TARGET: $targetEnvironment"
	Write-Host "Env: $env:UPSTREAM_JOB_NAME"

	if($env:isAMSEnabled -ne $null){
	   Write-Host "Setting AMS passed in parameters"
	   $amsEnabled = "$env:isAMSEnabled"
	   $amsMigrated = "$env:isAMSMigrated"
	   $amsChurches = "$env:AMS_CHURCHES"
    }else{
	   #Enable AMS to all test churches against Staging
	   #Mady commented out below if condition to make ams payment enabled for all automation environments
	   #if($targetEnvironment -eq "STAGING"){
	     Write-Host "Setting AMS Enbled in Staging to all test churches: $amsTestChurches"
	     $amsEnabled = "true"		 
		 $amsMigrated = "$env:isAMSMigrated"		 
	     $amsChurches = $amsTestChurches
	   #}else{
	   #  Write-Host "Using default AMS parameters"
	   #}	   
	}
	  
	Write-Host "AMS: $amsEnabled"
	Write-Host "AMS Migrated: $amsMigrated"
	Write-Host "AMS Churches: $amsChurches"
	
    #Directly connecting to node instead of hub
    if ($env:UPSTREAM_JOB_NAME.Contains("infellowship") ){
		$gridHost = $ymlconfig.infellowship.gridhost
    }
	Write-Host "Using $gridHost as Selenium Grid"
	
	if($env:RepeatOnFail -ne $null){
	   Write-Host "Setting Repeat on Fail value passed in"
	   $repeatOnFail = "$env:RepeatOnFail"
	}	
    Write-Host "RepeatOnFail: $repeatOnFail"
   
    # Configure the app.config based on the target environment
	$appConfig = New-Object XML
	$appConfig.Load($appConfigFile)
	foreach($setting in $appConfig.configuration.appSettings.add) {
		if ($setting.key -eq 'FTTests.Environment') {
		    if($targetEnvironment -eq "LV_UAT"){
			  $setting.value = "STAGING"
			}else{
              $setting.value = $targetEnvironment
			}
		}
		
		if ($setting.key -eq 'FTTests.Host') {
			$setting.value = $gridHost
		}
		
		if ($setting.key -eq 'FTTests.Port') {
			$setting.value = $port
		}
		
		if ($setting.key -eq 'FTTests.RepeatOnFail') {		
	      $setting.value = $repeatOnFail	  
		}
		
		if ($setting.key -eq 'FTTests.AMSEnabled') {
	      $setting.value = $amsEnabled	  
		}
		
		if ($setting.key -eq 'FTTests.AMSMigrated') {
	      $setting.value = $amsMigrated	  
		}
		
		if ($setting.key -eq 'FTTests.AMSChurches') {		
	      $setting.value = $amsChurches	  
		}
		
		if ($setting.key -eq 'FTTests.Jenkins') {		
	      $setting.value = $jenkins	  
		}
	}
	
	$appConfig.Save($appConfigFile)

	# Set the project variable based on the upstream job that initiated the build
	#Payment Processor needs to run Portal/Infellowship
	switch -wildcard ($env:UPSTREAM_JOB_NAME) {
	    "Api*Deploy*" {$project = "API";}
		"Portal*Deploy*" {$project = "Portal"}
		"infellowship*Deploy*" {$project = "inFellowship"}
		"Weblink*Deploy*" {$project = "Weblink"}
		"PaymentProcessor*Deploy*" {$project = "Portal"; $testArgs = "/f:Category:PaymentProcessor"; $category = "Payment Processor"; $testRuns = 3}
		"RDCChecker*Deploy*" {$project = "Portal"; $testArgs = "/f:Category:RDCChecker"; $category = "RDCChecker"}
		"ReportLibrary*Deploy*" {$project = "ReportLibrary"}
		"DataExchange*Deploy*" {$project = "DataExchange"}
		"CheckInTeacherClient*Deploy*" {$project = "CheckInTeacherClient"}
		"DashboardClient*Deploy*" {$project = "DashboardClient"}
		"SelfCheckInClient*Deploy*" {$project = "SelfCheckInClient"}
		"SmokeTest*Deploy*" {$project = "Portal"; $testArgs = "/f:Category:SmokeTest"; $category = "Smoke Test"; $testRuns = 7}
		default {Throw [system.ArgumentException]}
	}
	
	$testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"

	# Send email to alert team of test run
	$subject = "Functional Test Execution Alert: Regression tests are beginning execution in $targetEnvironment against $project $category"
	$body = "Environment: $targetEnvironment<br>Project: $project $category<br>Version: $env:BUILD_VERSION<br>AMS Enabled: $amsEnabled<br>AMS Migrated: $amsMigrated<br>AMS Churches: $amsChurches<br>Started by user: $buildUser"

	if($category -eq "Payment Processor"){
		$body = "<br><b>You will receive $testRuns execution report e-mails.</b><br><ol><li>Portal $category</li><li>Infellowship $category.</li><li>Weblink $category.</li></ol><b><br>AMS Enabled: $amsEnabled<br>AMS Migrated: $amsMigrated<br>AMS Churches: $amsChurches</b>"
	}elseif($category -eq "Smoke Test"){
        $body = "<br><b>You will receive $testRuns execution report e-mails.</b><br><ol><li>Portal $category</li><li>Infellowship $category.</li><li>Weblink $category.</li><li>CheckInTeacherClient $category.</li><li>DashboardClient $category.</li><li>SelfCheckInClient $category.</li><li>Report Library $category.</li></ol><b><br>AMS Enabled: $amsEnabled<br>AMS Migrated: $amsMigrated<br>AMS Churches: $amsChurches</b>"	
	}
	
	Write-Host "Sending test run initiation email to team..."
	Send-MailMessage -SmtpServer $smtpAddress -To $emailAddressList -From "FTQA@activenetwork.com" -Subject $subject -Body $body -BodyAsHtml -Encoding ([System.Text.Encoding]::UTF8) #-Priority High

	# Make an initial web request
    if ($project -eq "inFellowship") {
		$url =  $ymlconfig.infellowship.url -replace "targetenvironment",$env:DeployTo
    }
    
    if ($project -eq "Portal"){
		$url =  $ymlconfig.portal.url -replace "targetenvironment",$env:DeployTo
    }

	if ($project -eq "Weblink") {
		$url =  $ymlconfig.weblink.url -replace "targetenvironment",$env:DeployTo
	}
	
	if ($project -eq "CheckInTeacherClient") {
		$url =  $ymlconfig.checkinteacherclient.url -replace "targetenvironment",$env:DeployTo
	}
	
	if ($project -eq "DashboardClient") {
		$url =  $ymlconfig.dashboardclient.url -replace "targetenvironment",$env:DeployTo
	}
	
	if ($project -eq "SelfCheckInClient") {
		$url =  $ymlconfig.selfcheckinclient.url -replace "targetenvironment",$env:DeployTo
	}

	if ($project -eq "ReportLibrary") {
		$url =  $ymlconfig.reportlibrary.url -replace "targetenvironment",$env:DeployTo
	}

    if($targetEnvironment -eq "STAGING"){
	    $url = $url -replace ".uat.", ".staging."
	}

	write-host "Making initial web request against $url"
	$request = [System.Net.WebRequest]::Create($url)
	$request.GetResponse()

#Big Loops Through due To Payment Processor having
#Portal, Infellowship, and Weblink Tests
#beg of TC For Loop
for($i = 0; $i -lt $testRuns; $i++)
{

    if( ($category -eq "Payment Processor") -or ($category -eq "Smoke Test")){
	   switch  ($i){
		  6 {
		      Write-Host "Running SelfCheckIn Client $category tests ... "
			  $project = "SelfCheckInClient"
			  $testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"
			}
		  5	{
		      Write-Host "Running Dashboard Client $category tests ... "
			  $project = "DashboardClient"
			  $testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"
			}
		  4 {
		      Write-Host "Running CheckInTeacher Client $category tests ... "
			  $project = "CheckInTeacherClient"
			  $testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"
			}
	      3 {
		      Write-Host "Running Report Library $category tests ... "
			  $project = "ReportLibrary"
			  $testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"
			}
	      2 {
		      Write-Host "Running Portal $category tests ... "
			  $project = "Portal"
			  $testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"
			}
		  1 {
		      Write-Host "Running Infellowship $category tests ..."
		      $project = "inFellowship"
			  $testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"
		    }
		  0 {
		      Write-Host "Running Weblink $category tests ..."
		      $project = "Weblink"
			  $testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"
		    }
		  default {Write-Host "Error running $category tests" Throw [System.ArgumentException]}
	   }
	   
	}
	
	# Execute the tests
	write-host "Running tests from $testsPath against $targetEnvironment."
	$reportHtmlArgs = "/rt:Html-Condensed"
	& $gallioPath $testsPath $testArgs $reportArgs $reportHtmlArgs
	

	# Get the generated report
	cd $env:WORKSPACE\Reports
	[array] $files = dir -filter test-report*.xml | sort name
	
	$testReportFileName = ($files[-1].name)
	Write-Host "Test Report Name: $testReportFileName"
    
	# Get the generated report
	cd $env:WORKSPACE\Reports
	[array] $filesHtml = dir -filter test-report*.html | sort name
	
	$testReportFileHtmlName = ($filesHtml[-1].name)
	Write-Host "Test Report HTML Name: $testReportFileHtmlName"
	
	#######################
	#adding generated report file as attachment
	#$att = new-object Net.Mail.Attachment("Reports\$($filesHtml[-1].name)")
	
	$xml = New-Object XML
	$xml.Load("Reports\$($files[-1].name)")
	$xml
    
	$ns = New-Object Xml.XmlNamespaceManager $xml.NameTable
	$ns.AddNamespace( "e", "http://www.gallio.org/" )

	# Collect the data from the 'statistics' element
	$statisticsElement = $xml.SelectSingleNode("//e:statistics", $ns)

	$duration = $statisticsElement.Attributes.ItemOf("duration").Value
	$testCount = $statisticsElement.Attributes.ItemOf("testCount").Value
	$runCount = $statisticsElement.Attributes.ItemOf("runCount").Value
	$passedCount = $statisticsElement.Attributes.ItemOf("passedCount").Value
	$failedCount = $statisticsElement.Attributes.ItemOf("failedCount").Value
	$skippedCount = $statisticsElement.Attributes.ItemOf("skippedCount").Value

    $ts = new-timespan -Seconds $duration
    $durationFormatted = '{0:00}:{1:00}:{2:00}' -f $ts.Hours,$ts.Minutes,$ts.Seconds

	# Send the summary email
	$workspaceRpt = "$env:JOB_URL/ws/Reports/HTML_FILE"
	$workspaceZip = "$env:JOB_URL/ws/temp/ZIP_FILE"
	$locationHtmlBody = "<br>Reports: <a href='$workspaceRpt'>$workspaceRpt</a><br>"
	$locationZipBody = "<br>Archive: <a href='$workspaceZip'>$workspaceZip</a><br>"
	$subject = "Functional Test Execution Alert: Regression tests have completed execution in $targetEnvironment against $project $category"
	$body = "<html><body>Environment: $targetEnvironment<br>Project: $project $category<br>Version: $env:BUILD_VERSION<br>AMS Enabled: $amsEnabled<br>AMS Migrated: $amsMigrated<br>AMS Churches: $amsChurches<br>Started by user: $buildUser <p>Tests in Project: $testCount<br>Tests Run: $runCount, <font color=green>Passed: $passedCount</font>, <font color=red><b>Failed: $failedCount</font></b>, <font color=green>Repeat Passed: REPEAT_COUNT_TEMP</font>, <font color=Maroon><b>Skipped: $skippedCount</font></b><br>Duration [hr:min:sec]: $durationFormatted<br>$locationHtmlBody $locationZipBody<p>"
	$failedTestList = $xml.SelectNodes("//e:outcome[@status='failed']/ancestor::e:testStepRun/e:testStep[@isTestCase='true']", $ns)
	$repeatTestList = $xml.SelectNodes("//e:outcome[@status='failed']/ancestor::e:testStepRun/e:testStep[@isTestCase='true']", $ns)
	$skippedTestList = $xml.SelectNodes("//e:outcome[@status='skipped']/ancestor::e:testStepRun/e:testStep[@isTestCase='true']", $ns)
    $streamsTestList = $xml.SelectNodes("//e:streams", $ns)

	#How many did not run due to Jenkins limitation
    $jenkinsCounter = 0
	if ($skippedTestList.Count -ne 0) {
		foreach ($skippedTestCaseNode in $skippedTestList) {
		    if( ($skippedTestCaseNode.NextSibling.NextSibling.NextSibling.InnerText) -match "This test will not run in Jenkins"){
               
			   #We found Jenkins test			   
               $jenkinsCounter += 1			   				
		    }
			
		}
	}

    Write-Output "Jenkins: $jenkinsCounter"
	if ($jenkinsCounter -gt 0){
		$body += "<b><font color=Maroon>REMINDER: Please Execute all skipped tests associated with Email and Captcha</b><br></font><br>"	
	    #$body += "<p><b>Not Execute Jenkins Tests:</b><p>"
	    if ($skippedTestList.Count -ne 0) {
		    foreach ($skippedTestCaseNode in $skippedTestList) {
			   if( ($skippedTestCaseNode.NextSibling.NextSibling.NextSibling.InnerText) -match "This test will not run in Jenkins"){
			     $body += "<b><font color=Maroon>$($skippedTestCaseNode.FirstChild.NextSibling.NextSibling.FirstChild.InnerText)</font>: $($skippedTestCaseNode.GetAttribute('name')):</b> This test did not execute in Jenkins. Please run manually through Gallio GUI.<br><p>"
			   }
		    }
		}
	}
	
	
	#Generate Email Body for Failed Tests along with Errors
	$body += "<br><b>Failed Tests:</b><p>"
	$failedCounter = 0
	
	if ($failedTestList.Count -ne 0) {
		foreach ($failedTestCaseNode in $failedTestList) {

		    #Write-Output "Error: $($failedTestCaseNode.NextSibling.NextSibling.NextSibling.NextSibling.InnterText)"
			#Write-Output "Test Name: $($failedTestCaseNode.GetAttribute('name'))"
			
			#"Failed on attempt" is set in SeleniumTestBase.cs under RepeatOnFailureAttribute
	        if( ($failedTestCaseNode.NextSibling.NextSibling.NextSibling.InnerText) -match "Failed on attempt"){
               
			   #We found failed test
			   $body += "<p><b><font color=red>$($failedTestCaseNode.FirstChild.NextSibling.NextSibling.FirstChild.InnerText)</font>: $($failedTestCaseNode.GetAttribute('name')):</b> $($failedTestCaseNode.NextSibling.NextSibling.NextSibling.InnerText)<br></p>"
               $failedCounter += 1			   				
			
			}
			#Fixture Failures
			elseif (($failedTestCaseNode.FirstChild.NextSibling.NextSibling.FirstChild.InnerText) -match "Fixture"){
				    $body += "<p><br><b><font color=red>$($failedTestCaseNode.FirstChild.NextSibling.NextSibling.FirstChild.InnerText)</font>: $($failedTestCaseNode.GetAttribute('name'))</b><br></p>"
                    $body += "<br><b><font color=red>Fixture Error:</font><pre><code>$($failedTestCaseNode.NextSibling.NextSibling.NextSibling.InnerText)</code></pre>"
			
			}
		}
	}
	
	if($failedCounter -eq 0) {
		$body += "N/A<br>"
	}

    #Build repeat attempts to report in email
	#Similar logic as in Failed Tests
	$body += "<p><b>Repeat Attempts Passed Tests:</b><p>"
	$repeatCounter = 0
	$repeatAttempts = 0
	
	if ($repeatTestList.Count -ne 0) {
		foreach ($repeatTestCaseNode in $repeatTestList) {
		  
		  #"The test passed on attempt" is set in SeleniumTestBase.cs under RepeatOnFailureAttribute
		  if( ($repeatTestCaseNode.NextSibling.NextSibling.NextSibling.InnerText) -match "The test passed on attempt"){
		    $body += "<p><b><font color=green>$($repeatTestCaseNode.FirstChild.NextSibling.NextSibling.FirstChild.InnerText)</font>: $($repeatTestCaseNode.GetAttribute('name')):</b> $($repeatTestCaseNode.NextSibling.NextSibling.NextSibling.InnerText)<br></p>"
            $repeatAttempts += 1										
		  }
		}
	}
	
	if($repeatAttempts -eq 0) {
		$body += "N/A<br>"
	}

    #replace, Repeat Passed: REPEAT_COUNT_TEMP with actual value since we know know that value
	Write-Host "Repeat Attempts: $repeatAttempts"
	$body = [regex]::Replace($body, "REPEAT_COUNT_TEMP", "$repeatAttempts")

	$body += "<p><b><font color=Maroon>Skipped Tests:</b><p></font>"
	if ($skippedTestList.Count -ne 0) {
		foreach ($skippedTestCaseNode in $skippedTestList) {
			$body += "<b><font color=Maroon>$($skippedTestCaseNode.FirstChild.NextSibling.NextSibling.FirstChild.InnerText)</font>: $($skippedTestCaseNode.GetAttribute('name')):</b> $($skippedTestCaseNode.NextSibling.NextSibling.NextSibling.InnerText)<br><p>"
		}
	}
	else {
		$body += "N/A<br>"
	}

    Write-Host "Took $durationFormatted to Executed tests"
	Write-Host "Sending test run summary email to team..."
	
    cd $env:WORKSPACE\Reports
	$targetDir = "$env:WORKSPACE\temp"
    if(!(Test-Path -Path $targetDir )){
      New-Item -ItemType directory -Path $targetDir | Out-Null
	  Write-Output "$targetDir created";
    }else{	
	  Write-Output "$targetDir already created";
    }

	[array] $dirNames = dir -filter test-report*.xml | sort name
	$testReportFileDirName = ($dirNames[-1].name).Replace(".xml", "")
	Write-Output "Report File Dir: $testReportFileDirName"
	
    $src_folder = "$env:WORKSPACE\Reports"
	Write-Output "Src Zip File Folder: $($src_folder)"
	
    $destfile = "$targetDir" + "\" + "$testReportFileDirName" + ".zip";
	Write-Output "Dest Zip File Attachment: $($destfile)"
	
	Write-Output "Sleep for medio minuto before zip file"
	Start-sleep -Seconds 30
	[System.Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" ) | Out-Null
	$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    $includebasedir = $false

    #[System.IO.Compression.ZipFile]::CreateFromDirectory($src_folder,$destfile,$compressionLevel, $includebasedir )
	[System.IO.Compression.ZipFile]::CreateFromDirectory($src_folder,$destfile)
	
	Write-Output "Sleep for dos minutos to finish zip file process"
	Start-sleep -Seconds 120
	
	#Display file size
	Write-Output "Size of $destfile"
    Get-ChildItem $destfile | Select-Object Name, CreationTime,  @{Name="Mbytes";Expression={$_.Length / 1MB}}

    #Send Email
	$body = [regex]::Replace($body, "HTML_FILE", "$testReportFileHtmlName")
	$body = [regex]::Replace($body, "ZIP_FILE", "$testReportFileDirName" + ".zip")
	if((Test-Path -Path $destfile ) -or (Test-Path -Path $destfile )){
	  #Send Zip File Attachment
	  try{
        Send-MailMessage -SmtpServer $smtpAddress -To $emailAddressList -From "FTQA@activenetwork.com" -Subject $subject -Body $body -BodyAsHtml -Encoding ([System.Text.Encoding]::UTF8) -Attachment "$($destfile)" #-Priority High
      }
	  catch [system.exception]
	  {
	    "Error sending attachment ..."
		
	    if((Test-Path -Path $destfile )){
		  Write-Output "Sending HMTL Log file only"
		  Send-MailMessage -SmtpServer $smtpAddress -To $emailAddressList -From "FTQA@activenetwork.com" -Subject $subject -Body $body -BodyAsHtml -Encoding ([System.Text.Encoding]::UTF8) -Attachment "$($filesHtml[-1].name)" #-Priority High
		}else{
		  Write-Output "Sending Legacy E-mail"
		  Send-MailMessage -SmtpServer $smtpAddress -To $emailAddressList -From "FTQA@activenetwork.com" -Subject $subject -Body $body -BodyAsHtml -Encoding ([System.Text.Encoding]::UTF8)
		}
	  }	  
	}
	
	cd $env:WORKSPACE
	

#end of TC For Loop
}

	# Return the exit code
	exit $LastExitCode
}