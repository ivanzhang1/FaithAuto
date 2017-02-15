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
    
    
    
    
        //$this->apiConsumer = new F1Api();
        //$this->apiConsumer->initAccessToken($accessToken, $tokenSecret);
    }

    public function startTests()
    {
		// People
		
		$this->People_Search_id();
		$this->People_Search_id_includeAddresses();
		$this->People_Search_id_includeCommunications();
		$this->People_Search_id_includeAttributes();
		$this->People_Search_id_includeAll();
		
		$this->People_Search_id_array();
		$this->People_Search_id_array_includeAddresses();
		$this->People_Search_id_array_includeCommunications();
		$this->People_Search_id_array_includeAttributes();
		$this->People_Search_id_array_includeAll();
		
		// Household
		//$this->People_Search_householdid();
		//$this->People_Search_householdid_includeAddresses();
		//$this->People_Search_householdid_includeCommunications();
		//$this->People_Search_householdid_includeAttributes();
		//$this->People_Search_householdid_includeAll();
	}
	
	public function People_Search_id()
	{
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
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_id Fail!!!</div>";
		}
	}
	
	public function People_Search_id_includeAddresses()
	{
		// Prepare the request data
		$searchParameter = "?id=22116049&include=addresses";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
			
		if (count($json_string->results == 1) && $json_string->results->person[0]->firstName == "API" && $json_string->results->person[0]->lastName == "Household" && 
			count($json_string->results->person[0]->addresses->address) == 3 && count($json_string->results->person[0]->communications) == 0 && count($json_string->results->person[0]->attributes) == 0)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_id_includeAddresses Fail!!!</div>";
		}
	}
	
	public function People_Search_id_includeCommunications()
	{
		// Prepare the request data
		$searchParameter = "?id=22116049&include=communications";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
			
		if (count($json_string->results == 1) && $json_string->results->person[0]->firstName == "API" && $json_string->results->person[0]->lastName == "Household" && 
			count($json_string->results->person[0]->addresses) == 0 && count($json_string->results->person[0]->communications->communication) == 5 && count($json_string->results->person[0]->attributes) == 0)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_id_includeCommunications Fail!!!</div>";
		}
	}
	
	public function People_Search_id_includeAttributes()
	{
		// Prepare the request data
		$searchParameter = "?id=22116049&include=attributes";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
			
		if (count($json_string->results == 1) && $json_string->results->person[0]->firstName == "API" && $json_string->results->person[0]->lastName == "Household" && 
			count($json_string->results->person[0]->addresses) == 0 && count($json_string->results->person[0]->communications) == 0 && count($json_string->results->person[0]->attributes->attribute) == 1)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_id_includeAttributes Fail!!!</div>";
		}
	}
	
	public function People_Search_id_includeAll()
	{
		// Prepare the request data
		$searchParameter = "?id=22116049&include=addresses,communications,attributes";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
			
		if (count($json_string->results == 1) && $json_string->results->person[0]->firstName == "API" && $json_string->results->person[0]->lastName == "Household" && 
			count($json_string->results->person[0]->addresses->address) == 3 && count($json_string->results->person[0]->communications->communication) == 5 && count($json_string->results->person[0]->attributes->attribute) == 1)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_id_includeAll Fail!!!</div>";
		}
	}
	
    public function People_Search_id_array()
	{
		// Prepare the request data
		$searchParameter = "?id=22116049,22115163,22115144";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Arrays for the names
		$arrFirstName = array(0 => "API", 1=> "Apple", 2 => "Captain");
		$arrLastName = array(0 => "Household", 1=> "Crunch", 2 => "Crunch");
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
		
	
				
		for($i = 0; $i <= 3; $i++)
	    {
	        if (count($json_string->results == 1 && $json_string->results->person[$i]->firstName == $arrFirstName[$i] && $json_string->results->person[$i]->lastName == $arrayLastName[$i] && 
			count($json_string->results->person[$i]->addresses) == 0 && count($json_string->results->person[$i]->communications) == 0 && count($json_string->results->person[$i]->attributes) == 0))
		    {
			    $style = "style = \"color:green\"";
			    print "<div $style>Pass!!!</div>";
		    }
		    else
		   {
			    $style = "style = \"color:red\"";
			    print "<div $style>People_Search_id_array Fail!!!</div>";
		   }
	    
	    }

	}
	
	 public function People_Search_id_array_includeAddresses()
	{
		// Prepare the request data
		$searchParameter = "?id=22116049,22115163,22115144&include=addresses";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Arrays for the names
		$arrFirstName = array(0 => "API", 1=> "Apple", 2 => "Captain");
		$arrLastName = array(0 => "Household", 1=> "Crunch", 2 => "Crunch");
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
				
		
		
		for($i = 0; $i <= 3; $i++)
	    {
	        if (count($json_string->results == 1 && $json_string->results->person[$i]->firstName == $arrFirstName[$i] && $json_string->results->person[$i]->lastName == $arrayLastName[$i] && 
			count($json_string->results->person[$i]->addresses) == 3 && count($json_string->results->person[$i]->communications) == 0 && count($json_string->results->person[$i]->attributes) == 0))
		    {
			    $style = "style = \"color:green\"";
			    print "<div $style>Pass!!!</div>";
		    }
		    else
		   {
			    $style = "style = \"color:red\"";
			    print "<div $style>People_Search_id_array_includeAddresses Fail!!!</div>";
		   }
	    
	    }

	}
	
	public function People_Search_id_array_includeCommunications()
	{
		// Prepare the request data
		$searchParameter = "?id=22116049,22115163,22115144&include=communications";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
				
		// Arrays for the names
		$arrFirstName = array(0 => "API", 1=> "Apple", 2 => "Captain");
		$arrLastName = array(0 => "Household", 1=> "Crunch", 2 => "Crunch");
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
				
		
		
		for($i = 0; $i <= 4; $i++)
	    {
	        if (count($json_string->results == 1 && $json_string->results->person[$i]->firstName == $arrFirstName[$i] && $json_string->results->person[$i]->lastName == $arrayLastName[$i] && 
			count($json_string->results->person[$i]->addresses) == 0 && count($json_string->results->person[$i]->communications) == 5 && count($json_string->results->person[$i]->attributes) == 0))
		    {
			    $style = "style = \"color:green\"";
			    print "<div $style>Pass!!!</div>";
		    }
		    else
		   {
			    $style = "style = \"color:red\"";
			    print "<div $style>People_Search_id_array_includeCommunications Fail!!!</div>";
		   }
	    
	    }

	}
	
	public function People_Search_id_array_includeAttributes()
	{
		// Prepare the request data
		$searchParameter = "?id=22116049,22115163,22115144&include=attributes";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
				
		// Arrays for the names
		$arrFirstName = array(0 => "API", 1=> "Apple", 2 => "Captain");
		$arrLastName = array(0 => "Household", 1=> "Crunch", 2 => "Crunch");
		
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
				
		
		
		for($i = 0; $i <= 4; $i++)
	    {
	        if (count($json_string->results == 1 && $json_string->results->person[$i]->firstName == $arrFirstName[$i] && $json_string->results->person[$i]->lastName == $arrayLastName[$i] && 
			count($json_string->results->person[$i]->addresses) == 0 && count($json_string->results->person[$i]->communications) == 0 && count($json_string->results->person[$i]->attributes) == 1))
		    {
			    $style = "style = \"color:green\"";
			    print "<div $style>Pass!!!</div>";
		    }
		    else
		   {
			    $style = "style = \"color:red\"";
			    print "<div $style>People_Search_id_array_includeAttributes Fail!!!</div>";
		   }
	    
	    }

	}
	
		public function People_Search_id_array_includeAll()
	{
		// Prepare the request data
		$searchParameter = "?id=22116049,22115163,22115144&include=addresses,communications,attributes";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Arrays for the names
		$arrFirstName = array(0 => "API", 1=> "Apple", 2 => "Captain");
		$arrLastName = array(0 => "Household", 1=> "Crunch", 2 => "Crunch");
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
				
		
		
		for($i = 0; $i <= 4; $i++)
	    {
	        if (count($json_string->results == 1 && $json_string->results->person[$i]->firstName == $arrFirstName[$i] && $json_string->results->person[$i]->lastName == $arrayLastName[$i]  && 
			count($json_string->results->person[$i]->addresses) == 3 && count($json_string->results->person[$i]->communications) == 5 && count($json_string->results->person[$i]->attributes) == 1))
		    {
			    $style = "style = \"color:green\"";
			    print "<div $style>Pass!!!</div>";
		    }
		    else
		   {
			    $style = "style = \"color:red\"";
			    print "<div $style>People_Search_id_array_includeAll Fail!!!</div>";
		   }
	    
	    }

	}
	
	public function People_Search_householdid()
	{
		// Prepare the request data
		$searchParameter = "?householdid=14339591";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
				

		// Decode the response
		$json_string = json_decode($result);
		
		if (count($json_string->results == 1) && $json_string->results->household[0]->householdName == "API Household" && $json_string->results->household[0]->householdSortName == "Household" && 
			$json_string->results->household[0]->householdFirstName == "API" && count($json_string->results->household[0]->addresses->address) == 0 && count($json_string->results->household[0]->communications) == 0 && 
			count($json_string->results->household[0]->attributes) == 0)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_householdid Fail!!!</div>";
		}
	}
	
	public function People_Search_householdid_includeAddresses()
	{
		// Prepare the request data
		$searchParameter = "?householdid=14339591&include=addresses";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
		
		if (count($json_string->results == 1) && $json_string->results->household[0]->householdName == "API Household" && $json_string->results->household[0]->householdSortName == "Household" && 
			$json_string->results->household[0]->householdFirstName == "API" && count($json_string->results->household[0]->addresses->address) == 3 && count($json_string->results->household[0]->communications) == 0 && 
			count($json_string->results->household[0]->attributes) == 0)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_householdid_includeAddresses Fail!!!</div>";
		}
	}
	
	public function People_Search_householdid_includeCommunications()
	{
		// Prepare the request data
		$searchParameter = "?householdid=14339591&include=communications";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
		
		if (count($json_string->results == 1) && $json_string->results->household[0]->householdName == "API Household" && $json_string->results->household[0]->householdSortName == "Household" && 
			$json_string->results->household[0]->householdFirstName == "API" && count($json_string->results->household[0]->addresses) == 0 && count($json_string->results->household[0]->communications->communication) == 5 && 
			count($json_string->results->household[0]->attributes) == 0)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_householdid_includeCommunications Fail!!!</div>";
		}
	}
	
	public function People_Search_householdid_includeAttributes()
	{
		// Prepare the request data
		$searchParameter = "?householdid=14339591&include=attributes";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
		
		if (count($json_string->results == 1) && $json_string->results->household[0]->householdName == "API Household" && $json_string->results->household[0]->householdSortName == "Household" && 
			$json_string->results->household[0]->householdFirstName == "API" && count($json_string->results->household[0]->addresses) == 0 && count($json_string->results->household[0]->communications) == 0 && 
			count($json_string->results->household[0]->attributes->attribute) == 1)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_householdid_includeAttributes Fail!!!</div>";
		}
	}
	
	public function People_Search_householdid_includeAll()
	{
		// Prepare the request data
		$searchParameter = "?householdid=14339591&include=addresses,communications,attributes";
		$requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_search.$searchParameter;
		
		// Make the request
		$result = $this->apiConsumer->doRequest($requestUrl);
		
		// Decode the response
		$json_string = json_decode($result);
		
		if (count($json_string->results == 1) && $json_string->results->household[0]->householdName == "API Household" && $json_string->results->household[0]->householdSortName == "Household" && 
			$json_string->results->household[0]->householdFirstName == "API" && count($json_string->results->household[0]->addresses->address) == 3 && count($json_string->results->household[0]->communications->communication) == 5 && 
			count($json_string->results->household[0]->attributes->attribute) == 1)
		{
			$style = "style = \"color:green\"";
			print "<div $style>Pass!!!</div>";
		}
		else
		{
			$style = "style = \"color:red\"";
			print "<div $style>People_Search_householdid_includeAll Fail!!!</div>";
		}
	}
}