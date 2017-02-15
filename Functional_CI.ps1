properties {
	$targetEnvironment = "QA"
    $appConfigFile = "$env:WORKSPACE\Common\bin\Debug\Common.dll.config"
    $gallioPath = "C:\Program Files\Gallio\bin\Gallio.Echo.exe"
    $testArgs = ""
	#########
    #$reportArgs = "/rt:XML /rt:HTML"
	$reportArgs = "/rt:XML"
    $testFailureThreshold = 10
	$url = "http://portalqa.dev.corp.local/login.aspx"	
	#old IP Proofpoint1d.dev.activenetwork.com
	#$smtpAddress = "10.50.1.250"
	#New Relay IP Addr lassmtpint.active.local
	$smtpAddress = "10.119.13.185"
	$emailAddressList = @("felix.gaytan@activenetwork.com", "david.martin@activenetwork.com", "stuart.platt@activenetwork.com", "karlie.smith@activenetwork.com")
    $emailAddressListApi = @("felix.gaytan@activenetwork.com", "david.martin@activenetwork.com", "stuart.platt@activenetwork.com", "tracy.mazelin@activenetwork.com", "karlie.smith@activenetwork.com")
	#$emailAddressList = @("felix.gaytan@activenetwork.com")
	#$gridHost = "qats01.dev.corp.local"
	$gridHost = "ftci01.ft.corp.local"
	#$gridHost = "ftci04.ft.corp.local"
	$port = "4444"
    $buildUser = "build-deploy"
	$SourcePath = ""
	$PhysicalPath = ""
	$ComputerName = "ftci01.ft.corp.local"
	$category = ""
    $repeatOnFail = "3"
	$testRuns = 1
	
	# Use Microsoft Web Deploy V2 if it's available.  Otherwise fall back to V1
	if (Test-Path -Path "C:\Program Files\IIS\Microsoft Web Deploy V2\msdeploy.exe") {
		Write-Host "Using Microsoft Web Deploy V2"
		$msdeploy = "C:\Program Files\IIS\Microsoft Web Deploy V2\msdeploy.exe"
	}
	elseif (Test-Path -Path "C:\Program Files\IIS\Microsoft Web Deploy\msdeploy.exe") {
		Write-Host "Using Microsoft Web Deploy V1.  Consider upgrading to V2"
		$msdeploy = "C:\Program Files\IIS\Microsoft Web Deploy\msdeploy.exe"
	}
	else {
		throw "FAILED: msdeploy.exe not found on this system"
	}
	
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

    if($env:DeployTo -ne $null){
	  if($env:DeployTo.Contains("dev")){
	   Write-Host "TARGET: $env:DeployTo"
	   $targetEnvironment = "DEV"
	  }
	}
	
	Write-Host "Env: $env:UPSTREAM_JOB_NAME"
	# Determine the target environment for the tests
	if ($env:UPSTREAM_JOB_NAME.Contains("Stage")) {
		$targetEnvironment = "STAGING"
	}                

	#New Grid - testing for Portal
	#if ($targetEnvironment -eq "QA" ){
    #        $gridHost = "ftci01.ft.corp.local"
    #}

	if ($env:UPSTREAM_JOB_NAME.Contains("AdhocReporting") -or $env:UPSTREAM_JOB_NAME.Contains("ReportLibrary") -or $env:UPSTREAM_JOB_NAME.Contains("Weblink")) {
		#$gridHost = "http://ftci02.ft.corp.local:4444/wd/hub"
		$gridHost = "ftci02.ft.corp.local"
	}



    #Directly connecting to node instead of hub
    if ($env:UPSTREAM_JOB_NAME.Contains("infellowship") ){
            #$gridHost = "qats02.dev.corp.local"
			#$gridHost = "ftci03.ft.corp.local"
			#$port = "5555"
			#Better Server
			$gridHost = "ftci04.ft.corp.local"
    }
	
	switch -wildcard ($env:UPSTREAM_JOB_NAME){
		    "Portal*Deploy" {
               $repeatOnFail = "3"	
			}
            default {
			   $repeatOnFail = "3"
			}			
	}
	
    Write-Host "Using $gridHost as Selenium Grid"
    Write-Host "Setting RepeatOnFail to $repeatOnFail"
   
    # Configure the app.config based on the target environment
	$appConfig = New-Object XML
	$appConfig.Load($appConfigFile)
	foreach($setting in $appConfig.configuration.appSettings.add) {
		if ($setting.key -eq 'FTTests.Environment') {
            $setting.value = $targetEnvironment
		}
		if ($setting.key -eq 'FTTests.Host') {
			$setting.value = $gridHost
		}
		
		if ($setting.key -eq 'FTTests.Port') {
			$setting.value = $port
		}
		
		#<add key="FTTests.RepeatOnFail" value="3" />
		if ($setting.key -eq 'FTTests.RepeatOnFail') {		
		  #let's run them max 3 times
		  #Portal - QA/DEV only 1 or 2 (still testing)
	      $setting.value = $repeatOnFail	  
		}
	}
	
	$appConfig.Save($appConfigFile)


	# Set the project variable based on the upstream job that initiated the build
	#Payment Processor needs to run Portal/Infellowship
	switch -wildcard ($env:UPSTREAM_JOB_NAME) {
	    "Api*Deploy*" {$project = "API"}
		"Portal*Deploy*" {$project = "Portal"}
		"infellowship*Deploy*" {$project = "inFellowship"}
		"AdhocReporting*Deploy*" {$project = "AdhocReporting"}
		"Weblink*Deploy*" {$project = "Weblink"}
		"PaymentProcessor*Deploy*" {$project = "Portal"; $testArgs = "/f:Category:PaymentProcessor"; $category = "Payment Processor"; $testRuns = 2}
		"RDCChecker*Deploy*" {$project = "Portal"; $testArgs = "/f:Category:RDCChecker"; $category = "RDCChecker"}
		"ReportLibrary*Deploy*" {$project = "ReportLibrary"}
		"DataExchange*Deploy*" {$project = "DataExchange"}
		default {Throw [system.ArgumentException]}
	}
	
	$testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"
	$SourcePath = "$env:WORKSPACE\"
	$PhysicalPath = "C:\hudson\jobs"
	
    #TEST Copy What we Need to Execute Portal Functional Tests
    #E:\hudson\jobs\Portal-FunctionalTest\workspace to ftci01
    #if(Test-Path "$env:WORKSPACE"){

	#  Write-Host "Deploying Functional Tests from $SourcePath to $PhysicalPath on $ComputerName"    

	  # Copy the application to the server
	#  [Array]$arguments = "-verb:sync","-source:dirPath=`"$SourcePath`"","-dest:dirPath=`"$PhysicalPath`",computerName=$ComputerName"
	#  $proc = Start-Process $msdeploy -ArgumentList $arguments -NoNewWindow -Wait -PassThru
	#  if($proc.ExitCode -ne 0) {
	#	throw "FAILED: Failed to synchronize files for application Functional Tests to server $Server"
	#  }	
	  
    #}


	# Send email to alert team of test run
	$subject = "Functional Test Execution Alert: Regression tests are beginning execution in $targetEnvironment against $project $category"
	$body = "Environment: $targetEnvironment<br>Project: $project $category<br>Version: $env:BUILD_VERSION<br>Started by user: $buildUser"
	
	if($category -eq "Payment Processor"){
	 $body = "<br><b>You will receive two execution report e-mails.<br>Portal Payment Processor<br>Infellowship Payment Processor.</b><br>"
	}
	
	Write-Host "Sending test run initiation email to team..."
	Send-MailMessage -SmtpServer $smtpAddress -To $emailAddressList -From "FTQA@activenetwork.com" -Subject $subject -Body $body -BodyAsHtml -Encoding ([System.Text.Encoding]::UTF8) #-Priority High

	# Make an initial web request
	if ($project -eq "AdhocReporting") {
        if ($targetEnvironment -eq "QA") {
            $url = "http://adhocqa.dev.corp.local/UserLogin/Index"
        }
        else {
            $url = "http://sweb01.dallas-dc.ft.com:8005/UserLogin/Index"
        }
    }

    if ($project -eq "inFellowship") {
        if ($targetEnvironment -eq "QA") {
            $url = "http://dc.infellowshipqa.dev.corp.local/UserLogin"
        }
        else {
            #$url = "https://dc.staging.infellowship.com/login.aspx"
			$url = "https://dc.staging.infellowship.com/UserLogin"
        }
    }
    
    if ($project -eq "Portal") {
        if ($targetEnvironment -eq "QA") {
            $url = "http://portalqa.dev.corp.local/login.aspx"
        }
        else {
            $url = "http://portal.staging.dallas-dc.ft.com/login.aspx"
        }
    }

	if ($project -eq "Weblink") {
		if ($targetEnvironment -eq "QA") {
			$url = "http://integrationqa.dev.corp.local/integration/login.aspx?cCode=alyq5rXA9igtXilwXAT+3Q=="
		}
		else {
			$url = "http://integration.staging.dallas-dc.ft.com/integration/login.aspx?cCode=alyq5rXA9igtXilwXAT+3Q=="
		}
	}

	if ($project -eq "ReportLibrary") {
		if ($targetEnvironment -eq "QA") {
			$url = "http://reportlibraryqa.dev.corp.local/ReportLibrary/Index.aspx"
		}
		else {
			$url = "http://reportlibrary.staging.dallas-dc.ft.com/ReportLibrary/Index.aspx"
		}
	}

	write-host "Making initial web request against $url"
	$request = [System.Net.WebRequest]::Create($url)
	$request.GetResponse()

#Big Loops Through due To Payment Processor having
#Portal, Infellowship, and Weblink Tests
#beg of TC For Loop
for($i = 0; $i -lt $testRuns; $i++)
{

    if($category -eq "Payment Processor"){
	   switch  ($i){
	      0 {
		      Write-Host "Running Portal Payment Processor tests ... "			  
			}
		  1 {
		      Write-Host "Running Infellowship Payment Processor tests ..."
		      $project = "inFellowship"
			  $testsPath = "$env:WORKSPACE\$project\bin\Debug\$project.dll"
			  #Write-Host "Removing old Reports from $env:WORKSPACE\Reports"
			  #Remove-Item -Recurse -Force "$env:WORKSPACE\Reports"
		    }			
		  default {Write-Host "Error running Payment Processor tests" Throw [System.ArgumentException]}
	   }
	   
	}
	
	# Execute the tests
	write-host "Running tests from $testsPath against $targetEnvironment."
	& $gallioPath $testsPath $testArgs $reportArgs

	# Get the generated report
	cd $env:WORKSPACE\Reports
	[array] $files = dir -filter test-report*.xml | sort name
	
	$testReportFileName = ($files[-1].name)
	Write-Host "Test Report Name: $testReportFileName"
    
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
	$subject = "Functional Test Execution Alert: Regression tests have completed execution in $targetEnvironment against $project $category"
	$body = "<html><body>Environment: $targetEnvironment<br>Project: $project $category<br>Version: $env:BUILD_VERSION<br>Started by user: $buildUser <p>Tests in Project: $testCount<br>Tests Run: $runCount, Passed: $passedCount, Failed: $failedCount, Repeat Passed: REPEAT_COUNT_TEMP, Skipped: $skippedCount<br>Duration [hr:min:sec]: $durationFormatted<br><p>"
	$failedTestList = $xml.SelectNodes("//e:outcome[@status='failed']/ancestor::e:testStepRun/e:testStep[@isTestCase='true']", $ns)
	$repeatTestList = $xml.SelectNodes("//e:outcome[@status='failed']/ancestor::e:testStepRun/e:testStep[@isTestCase='true']", $ns)
	$skippedTestList = $xml.SelectNodes("//e:outcome[@status='skipped']/ancestor::e:testStepRun/e:testStep[@isTestCase='true']", $ns)
    $streamsTestList = $xml.SelectNodes("//e:streams", $ns)
    
	#Generate Email Body for Failed Tests along with Errors
	$body += "<b>Failed Tests:</b><p>"
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
			   
				#Get the error logs				  
				if($streamsTestList.Count -ne 0){
				   $repeatCounter = 0
	               foreach ($streamNode in $streamsTestList){
				    
					#Ge only the error specific ones
                    foreach ($childStream in $streamNode){

					  $repeatCounter += 1
			          $matchTest = "Test Name: $($failedTestCaseNode.GetAttribute('name')):  Repetition #$repeatCounter"

	                  if( $($childStream.InnerText) -match $matchTest)
					  {
					    #clear out the extra logs. We just want the error logs
                        $code = $($childStream.InnerText)
						$errorCode = $code.Substring($code.IndexOf($matchTest))						
						Write-Host "Error Code $errorCode"
						
                        #Set error logs it in red						
						#$errorCode = [regex]::Replace($errorCode, $matchTest, "<br><b><font color=red>ERROR: $matchTest<br>")
				        #$body += "<code>$errorCode</code></font><br>"
						$errorCode = [regex]::Replace($errorCode, $matchTest, "<br><b><font color=red>Error: $matchTest</font>")
						$body += "<pre><text><code>$errorCode</code></text></pre>"
						break
				      }
					  
					  $repeatCounter = 0
	                }
					
	               }
	            }
			
			}
			#Fixuture Failures
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
			
				if($streamsTestList.Count -ne 0){
				   $repeatCounter = 0				   
	               foreach ($streamNode in $streamsTestList){
				    
                    foreach ($childStream in $streamNode){
			         #Write-Output "**** $counterStream $($childStream.InnerText) "
					  $repeatCounter += 1

			          $matchTest = "$($repeatTestCaseNode.GetAttribute('name')):  Repetition #$repeatCounter"
					  #Write-Output $matchTest
	                  if( $($childStream.InnerText) -match $matchTest)
					  {
                        $code = $($childStream.InnerText)
						$errorCode = $code.Substring($code.IndexOf($matchTest))						
						Write-Host "Error Code $errorCode"
						
						#$errorCode = [regex]::Replace($errorCode, $matchTest, "<br><b><font color=green>$matchTest<br>")
				        #$body += "<p><code><br>$errorCode</font></b></code><br><p>"
						$errorCode = [regex]::Replace($errorCode, $matchTest, "<br><b><font color=green>$matchTest")
						$body += "<pre><text><code>$errorCode</code></text></pre></font>"
						break
				      }
					  
					  $repeatCounter = 0
	                }
					
	               }
	            }			
		  }
		}
	}
	
	if($repeatAttempts -eq 0) {
		$body += "N/A<br>"
	}

    #replace, Repeat Passed: REPEAT_COUNT_TEMP with actual value since we know know that value
	Write-Host "Repeat Attempts: $repeatAttempts"
	$body = [regex]::Replace($body, "REPEAT_COUNT_TEMP", "$repeatAttempts")

	$body += "<p><b>Skipped Tests:</b><p>"
	if ($skippedTestList.Count -ne 0) {
		foreach ($skippedTestCaseNode in $skippedTestList) {
			$body += "<b><font color=red>$($skippedTestCaseNode.FirstChild.NextSibling.NextSibling.FirstChild.InnerText)</font>: $($skippedTestCaseNode.GetAttribute('name')):</b> $($skippedTestCaseNode.NextSibling.NextSibling.NextSibling.InnerText)<br><p>"
		}
	}
	else {
		$body += "N/A<br>"
	}

    Write-Host "Took $durationFormatted to Executed tests"
	Write-Host "Sending test run summary email to team..."
    Send-MailMessage -SmtpServer $smtpAddress -To $emailAddressList -From "FTQA@activenetwork.com" -Subject $subject -Body $body -BodyAsHtml -Encoding ([System.Text.Encoding]::UTF8) #-Priority High

    cd $env:WORKSPACE

#end of TC For Loop
}

	# Return the exit code
	exit $LastExitCode
}