<?php
require_once 'OAuth\AppConfig.php';
require_once 'F1Api.php';
require_once 'Person.php';
require_once 'OAuth\Util.php';
$accessToken = '61344b1a-0e88-4ccc-8854-cc6219d83642';
$tokenSecret = '7e01bf18-cc44-4332-b40e-dc4ae501098a';

$testClient = new UnitTests($accessToken, $tokenSecret);
$testClient->startTests();

class UnitTests
{
    private $apiConsumer;
    private $success = "OK";
    private $failed = "FAILED";

    public function __construct($accessToken, $tokenSecret)
    {
        $this->apiConsumer = new F1Api();
		
		// Change for different church code casing
	    $base_url = 'http://dc.f1api.com:8080';
	    //$base_url = 'http://DC.f1api.com:8080';
	    //$base_url = 'http://dC.f1api.com:8080';
		
	    //$base_url = 'http://dc.api.dev.corp.local';
	    //$base_url = 'http://dC.api.dev.corp.local';
	    //$base_url = 'http://DC.api.dev.corp.local';
	    
	    $consumer_key = '2';
	    $consumer_secret = 'f7d02059-a105-45e0-85c9-7387565f322b';
	    $accesstoken_path = "/v1/PortalUser/AccessToken";
	
	    /*********************2nd party authentication**************************/
	    // 2nd PARTY access token path
	    $apiConsumer = new OAuthClient($base_url, $consumer_key, $consumer_secret);
	    $apiConsumer->setAccessTokenPath($accesstoken_path);
	    // 2nd party consumer skips getting the request token part
	    // To authenticate the user and get the access token, the consumer posts the credentials to the service provider
	    $requestURL =  sprintf( "%s%s", $apiConsumer->getBaseUrl(), $accesstoken_path );
	    // SET the username and password
	    $requestBody = Util::urlencode_rfc3986(base64_encode( sprintf( "%s %s", "tcoulson", "FT.Admin1")));

	    // This is important. If we dont set this, the post will be sent using Content-Type: application/x-www-form-urlencoded (curl will do this automatically)
	    // Per OAuth specification, if the Content-Type is application/x-www-form-urlencoded, then all the post parameters also need to be part of the base signature string
	    // To override this, we need to set Content-type to something other than application/x-www-form-urlencoded
	    $getContentType = array("Accept: application/json",  "Content-type: application/json");
	    $requestBody = $apiConsumer->postRequest($requestURL, $requestBody, $getContentType,  200);
	    preg_match( "~oauth_token\=([^\&]+)\&oauth_token_secret\=([^\&]+)~i", $requestBody, $tokens );
	    if( !isset( $tokens[1] ) || !isset( $tokens[2] ) )
        {
		    //print 'Tokens are not set'; // Token are not set
		    $errors[] = 'Tokens are not set';
        }
    		
        $access_token = $tokens[1];
	    $token_secret = $tokens[2];
		
	    $this->apiConsumer->initAccessToken($access_token, $token_secret);
    }

    public function startTests()
    {
        $this->Addresses();
	    $this->Communications();
	    $this->SearchBarcode();
	    $this->ReturnCode_Uppercase_ChurchCode();
	    $this->ReturnCode_Mixed_Case_ChurchCode();

    }
	
    public function Addresses()
    {
        $requestUrlList = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleAddress_list;
        $requestUrlList = str_replace("{personID}", 22116049, $requestUrlList);
        $resultList = $this->apiConsumer->doRequest($requestUrlList);
        $json_stringList = json_decode($resultList, true);
        print $resultList;

        $searchParameter = "?searchfor=API%20Household&include=addresses";
        $requestUrlSearch = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
        $resultSearch = $this->apiConsumer->doRequest($requestUrlSearch);
        $json_stringSearch = json_decode($resultSearch, true);

        $countPassFlag = false;
        if (count($json_stringList['addresses']['address']) == count($json_stringSearch['results']['person'][0]['addresses']['address']) && count($json_stringList['addresses']['address']) == 3)
          $countPassFlag = true;
    	
        $address0PassFlag = false;
        if ($json_stringList['addresses']['address'][0]['addressType']['name'] == "Primary" && $json_stringSearch['results']['person'][0]['addresses']['address'][0]['addressType']['name'] == "Primary" && $json_stringList['addresses']['address'][0]['address1'] == "9616 Armour Dr" && $json_stringSearch['results']['person'][0]['addresses']['address'][0]['address1'] == "9616 Armour Dr")
          $address0PassFlag = true;

        $address1PassFlag = false;
        if ($json_stringList['addresses']['address'][1]['addressType']['name'] == "Previous" && $json_stringSearch['results']['person'][0]['addresses']['address'][1]['addressType']['name'] == "Previous" && $json_stringList['addresses']['address'][1]['address1'] == "1704 S Milbrook Ln" && $json_stringSearch['results']['person'][0]['addresses']['address'][1]['address1'] == "1704 S Milbrook Ln")
          $address1PassFlag = true;

        $address2PassFlag = false;
        if ($json_stringList['addresses']['address'][2]['addressType']['name'] == "Previous" && $json_stringSearch['results']['person'][0]['addresses']['address'][2]['addressType']['name'] == "Previous" && $json_stringList['addresses']['address'][2]['address1'] == "1515 Cannon Pkwy" && $json_stringSearch['results']['person'][0]['addresses']['address'][2]['address1'] == "1515 Cannon Pkwy")
          $address2PassFlag = true;

        if ($countPassFlag == true && $address0PassFlag == true && $address1PassFlag == true && $address2PassFlag == true)
        {
          $style = "style = \"color:green\"";
          print "<div $style>Pass!!!</div>";
        }
        else
        {
          $style = "style = \"color:red\"";
          print "Fail";
        }
    }
  	
    public function Communications()
    {
        $requestUrlList = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleCommunications_list;
        $requestUrlList = str_replace("{personID}", 22116049, $requestUrlList);
        $resultList = $this->apiConsumer->doRequest($requestUrlList);
        $json_stringList = json_decode($resultList, true);

        $searchParameter = "?searchfor=API%20Household&include=communications";
        $requestUrlSearch = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
        $resultSearch = $this->apiConsumer->doRequest($requestUrlSearch);
        $json_stringSearch = json_decode($resultSearch, true);

        $countPassFlag = false;
        if (count($json_stringList['communications']['communication']) == 5 && count($json_stringSearch['results']['person'][0]['communications']['communication']) == 5)
          $countPassFlag = true;

        $communication0PassFlag = false;
        if($json_stringList['communications']['communication'][0]['communicationGeneralType'] == "Telephone" && $json_stringSearch['results']['person'][0]['communications']['communication'][0]['communicationGeneralType'] == "Telephone" && $json_stringList['communications']['communication'][0]['communicationValue'] == 2627191190 && $json_stringSearch['results']['person'][0]['communications']['communication'][0]['communicationValue'] == 2627191190)
          $communication0PassFlag = true;

        $communication1PassFlag = false;
        if($json_stringList['communications']['communication'][1]['communicationGeneralType'] == "Telephone" && $json_stringSearch['results']['person'][0]['communications']['communication'][1]['communicationGeneralType'] == "Telephone" && $json_stringList['communications']['communication'][1]['communicationValue'] == 6302172170 && $json_stringSearch['results']['person'][0]['communications']['communication'][1]['communicationValue'] == 6302172170)
          $communication1PassFlag = true;

        $communication2PassFlag = false;
        if($json_stringList['communications']['communication'][2]['communicationGeneralType'] == "Email" && $json_stringSearch['results']['person'][0]['communications']['communication'][2]['communicationGeneralType'] == "Email" && $json_stringList['communications']['communication'][2]['communicationValue'] == "sneedenm@msoe.edu" && $json_stringSearch['results']['person'][0]['communications']['communication'][2]['communicationValue'] == "sneedenm@msoe.edu")
          $communication2PassFlag = true;

        $communication3PassFlag = false;
        if($json_stringList['communications']['communication'][3]['communicationGeneralType'] == "Telephone" && $json_stringSearch['results']['person'][0]['communications']['communication'][3]['communicationGeneralType'] == "Telephone" && $json_stringList['communications']['communication'][3]['communicationValue'] == 8479520499 && $json_stringSearch['results']['person'][0]['communications']['communication'][3]['communicationValue'] == 8479520499)
          $communication3PassFlag = true;

        $communication4PassFlag = false;
        if($json_stringList['communications']['communication'][4]['communicationGeneralType'] == "Email" && $json_stringSearch['results']['person'][0]['communications']['communication'][4]['communicationGeneralType'] == "Email" && $json_stringList['communications']['communication'][4]['communicationValue'] == "msneeden@fellowshiptech.com" && $json_stringSearch['results']['person'][0]['communications']['communication'][4]['communicationValue'] == "msneeden@fellowshiptech.com")
          $communication4PassFlag = true;

        if ($countPassFlag == true && $communication0PassFlag == true && $communication1PassFlag == true && $communication2PassFlag == true && $communication3PassFlag == true && $communication4PassFlag == true)
        {
          $style = "style = \"color:green\"";
          print "<div $style>Pass!!!</div>";
        }
        else
        {
          $style = "style = \"color:red\"";
          print "<div $style>Fail!!!</div>";
        }
    }
  	
    public function SearchBarcode()
    {
          $searchParameter = "?barcode=123456789";
          $requestUrlSearch = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
          $resultSearch = $this->apiConsumer->doRequest($requestUrlSearch);
          $json_string = json_decode($resultSearch, true);

          if ($json_string['results']['@count'] == 1 && $json_string['results']['person'][0]['firstName'] == "Bar" && $json_string['results']['person'][0]['lastName'] == "Code" && $json_string['results']['person'][0]['barCode'] =='123456789')
          {
	          $style = "style = \"color:green\"";
	          print "<div $style>Pass!!!</div>";
          }
          else
          {
	          $style = "style = \"color:red\"";
	          print "Fail";
          }
    }

	// Bryan Mikaelian (F1-1474)
	public function ReturnCode_Uppercase_ChurchCode()
	{
	    $base_url = 'http://DC.f1api.com:8080';
	    
		// Prepare the request data
		$searchParameter = "?id=22116049";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
		
		if (count($json_string->results == 1) && $json_string->results->person[0]->firstName == "API" && $json_string->results->person[0]->lastName == "Household" && 
			count($json_string->results->person[0]->addresses) == 0 && count($json_string->results->person[0]->communications) == 0 && count($json_string->results->person[0]->attributes) == 0)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!! (ReturnCode_Uppercase_ChurchCode)</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>ReturnCode_Uppercase_ChurchCode Fail!!!</div>";
		}	
	}
	
	// Bryan Mikaelian (F1-1474)
	public function ReturnCode_Mixed_Case_ChurchCode()
	{
	    $base_url = 'http://dC.f1api.com:8080';
	    
		// Prepare the request data
		$searchParameter = "?id=22116049";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
		
		if (count($json_string->results == 1) && $json_string->results->person[0]->firstName == "API" && $json_string->results->person[0]->lastName == "Household" && 
			count($json_string->results->person[0]->addresses) == 0 && count($json_string->results->person[0]->communications) == 0 && count($json_string->results->person[0]->attributes) == 0)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!  (ReturnCode_Mixed_Case_ChurchCode)</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>ReturnCode_Uppercase_ChurchCode Fail!!!</div>";
		}	
	}
}
?>