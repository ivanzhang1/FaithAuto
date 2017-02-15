<?php
require_once 'OAuth\AppConfig.php';
require_once 'F1Api.php';
require_once 'Person.php';
require_once 'OAuth\Util.php';
$accessToken = '61344b1a-0e88-4ccc-8854-cc6219d83642';
$tokenSecret = '7e01bf18-cc44-4332-b40e-dc4ae501098a';

$testClient = new UnitTests($accessToken, $tokenSecret);
$testClient->startTests();
class UnitTests {
    private $apiConsumer;
    private $success = "OK";
    private $failed = "FAILED";

    public function __construct($accessToken, $tokenSecret) {
        $this->apiConsumer = new F1Api();
        $this->apiConsumer->initAccessToken($accessToken, $tokenSecret);
    }

    public function startTests(){

        $personId = 24643475;
        
        $this->testListStatuses();
        $this->testListSubStatuses();
        $this->testListSchools();
        $this->testListOccupations();
        $this->testListDenominations();
        $this->testListCommunicationTypes();
        $this->testListCommunications($personId);
        $this->testNewCommunications();
        $this->testListAttributeGroups();
        $this->testListAddressTypes();
        return;
        
        //$this->testListHouseholdMemberTypes();
        $this->testHousehold();
        return;
        $this->testPerson($householdId);
        $this->testAddress();
        $this->testCommunication();

        $this->testGetPerson($personId);
        $firstName = "Jas";
        $lastName = "singh";
        $eMail = "jsingh@fellowshiptech.com";
        $this->testPeopleSearch($firstName ,$lastName,$eMail);

        $newHouseholdId = $this->testCreateHousehold();
        if($newHouseholdId > 0) {
            $this->testCreatePerson($newHouseholdId);
            $this->testCreatePersonWithAddress($newHouseholdId);
            $this->testCreatePersonWithCommunication($newHouseholdId);
            // People->Create F1API addPerson
            // Address->Create
            // Communication->Create
            $this->testCreatePersonWithAddressCommunication($newHouseholdId);
        }

        $this->testUpdatePerson($personId);
        $this->testUpdatePersonWithAddressCommunication($personId);

        $this->testPeopleSearch('joe', 'smith', 'jsmith@fellowshiptech.com');

        /*
        // None
        testListPeopleAttributes
        testShowPeopleAttributes
        testEditPeopleAttributes
        testNewPeopleAttributes
        testCreatePeopleAttributes
        testUpdatePeopleAttributes
        testDeletePeopleAttributes
        ////////////////////////////////////////
        testShowImage
        */
    }

    /*************************Statuses/Sub Statuses***********************************/
    public function testListStatuses(){
        $statuses = $this->apiConsumer->getStatuses();
        if(empty($statuses) || is_null($statuses))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('Statuses->List',$resultCode);
        //print $statuses;
        $statusId = null;
        $json_string = json_decode($statuses, true);
        $json_length = count($json_string['statuses']['status']);
        //print 'length:'.$json_length;
        print AppConfig::$lineBreak;
        $showStatusResultCode = true;
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_string['statuses']['status'][$i];
            $testStatusId =  $x['@id'];
            $result_code = $this->testShowStatus($testStatusId);
            if($result_code == false)
            $showStatusResultCode = false;
            if(strcmp(strtolower($x['name']), "new from facebook") == 0 ) {
                $statusId = $testStatusId;
            }
        }
        print AppConfig::$lineBreak.'StatusId of \'new from facebook\': '.$statusId.AppConfig::$lineBreak;
        $this->printStatus('Statuses->Show', $showStatusResultCode);
        return $resultCode;
    }

    private function testShowStatus($statusId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_statuses_show;
        $requestUrl = str_replace("{id}", $statusId, $requestUrl);

        $statuses = $this->apiConsumer->doRequest($requestUrl);
        if(empty($statuses) || is_null($statuses))
        $resultCode = false;
        else
        $resultCode = true;
        // $this->printStatus('showStatus StatusId: '.$statusId, $resultCode);
        $json_string = json_decode($statuses, true);
        $responseStatusId = $json_string['status']['@id'];
        //print AppConfig::$lineBreak;
        //print "<pre>".print_r($json_string, true)."</pre>";
        //print AppConfig::$lineBreak;
        return $resultCode;
    }
    /*
     * DO not use this function. ShowStatuses is tested using testShowStatus when testListStatuses is called
     */
    private function testShowStatuses() {
        $statuses = $this->apiConsumer->getStatuses();
        if(empty($statuses) || is_null($statuses))
        $resultCode = false;
        else
        $resultCode = true;
        if($resultCode) {
            $statusId = null;
            $json_string = json_decode($statuses, true);
            $json_length = count($json_string['statuses']['status']);
            print AppConfig::$lineBreak;
            for ($i = 0; $i < $json_length; $i++) {
                $x = $json_string['statuses']['status'][$i];
                $statusId = $x['@id'];
                $this->testShowStatus($statusId);
            }
        } else {
            $this->printStatus('showStatuses', $resultCode);
        }
    }

    public function testListSubStatuses(){
        $statuses = $this->apiConsumer->getStatuses();
        if(empty($statuses) || is_null($statuses))
        $resultCode = false;
        else
        $resultCode = true;
        if($resultCode) {
            $statusId = null;
            $json_string = json_decode($statuses, true);
            $json_length = count($json_string['statuses']['status']);
            print AppConfig::$lineBreak;
            for ($i = 0; $i < $json_length; $i++) {
                $x = $json_string['statuses']['status'][$i];
                $statusId = $x['@id'];
                $result_code = $this->testListSubStatus($statusId);
                if($result_code == false)
                $resultCode = false;
            }
            $this->printStatus('SubStatuses->List', $resultCode);
        } else {
            $this->printStatus('SubStatuses->List',$resultCode);
        }
        return $resultCode;
    }

    private function testListSubStatus($statusId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_substatuses_list;
        $requestUrl = str_replace("{statusID}", $statusId, $requestUrl);

        $subStatuses = $this->apiConsumer->doRequest($requestUrl);
        if(empty($subStatuses) || is_null($subStatuses))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('SubStatuses->List StatusId: '.$statusId, $resultCode);
        $json_string = json_decode($subStatuses, true);

        $json_length = count($json_string['subStatuses']['subStatus']);
        print AppConfig::$lineBreak;
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_string['subStatuses']['subStatus'][$i];
            $subStatusId = $x['@id'];
            $this->testShowSubStatus($statusId, $subStatusId);
        }

        //print AppConfig::$lineBreak;
        //print "<pre>".print_r($json_string, true)."</pre>";
        // print AppConfig::$lineBreak;
        return $resultCode;
    }

    public function testShowSubStatus($statusId, $subStatusId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_substatuses_show;
        $requestUrl = str_replace("{statusID}", $statusId, $requestUrl);
        $requestUrl = str_replace("{id}", $subStatusId, $requestUrl);
        $subStatuses = $this->apiConsumer->doRequest($requestUrl);
        if(empty($subStatuses) || is_null($subStatuses))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus("SubStatuses->Show. StatusId: ".$statusId." SubStatusId: ".$subStatusId , $resultCode);
        //$json_string = json_decode($subStatuses, true);
        //$responseStatusId = $json_string['subStatus']['@id'];
        //print AppConfig::$lineBreak;
        //print "<pre>".print_r($json_string, true)."</pre>";
        return $resultCode;
    }
    /*************************END Statuses/Sub Statuses*******************************/

    /*************************Schools*************************************************/
    private function testListSchools(){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_schools_list;
        $schools = $this->apiConsumer->doRequest($requestUrl);
        if(empty($schools) || is_null($schools))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('Schools->List',$resultCode);
        if($resultCode){
            // foreach school, show the school
            $json_string = json_decode($schools, true);
            $json_length = count($json_string['schools']['school']);
            if($json_length > 0) {
                print AppConfig::$lineBreak;
                for ($i = 0; $i < $json_length; $i++) {
                    $x = $json_string['schools']['school'][$i];
                    $schoolId = $x['@id'];
                    $result_code = $this->testShowShool($schoolId);
                    if($result_code == false)
                    $resultCode = false;
                }
                $this->printStatus('Schools->Show',$resultCode);
            } else {
                $this->printStatus('Schools->Show. NO DATA AVAILABLE FOR TEST> 0 SCHOOLS FOUND',false);
            }
        } else {
            $this->printStatus('Schhols->Show',$resultCode);
        }
    }

    private function testShowShool($schoolId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_schools_show;
        $requestUrl = str_replace("{id}", $schoolId, $requestUrl);
        $school = $this->apiConsumer->doRequest($requestUrl);
        if(empty($school) || is_null($school))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }
    /*************************END Schools*********************************************/

    /*************************Occupations*********************************************/
    private function testListOccupations () {
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_occupations_list;
        $occupations = $this->apiConsumer->doRequest($requestUrl);
        if(empty($occupations) || is_null($occupations))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('Occupations->List',$resultCode);

        if($resultCode){
            // foreach school, show the school
            $json_string = json_decode($occupations, true);
            $json_length = count($json_string['occupations']['occupation']);
            if($json_length > 0) {
                print AppConfig::$lineBreak;
                for ($i = 0; $i < $json_length; $i++) {
                    $x = $json_string['occupations']['occupation'][$i];
                    $occupationId = $x['@id'];
                    $result_code = $this->testShowOccupation($occupationId);
                    if($result_code == false)
                    $resultCode = false;
                }
                $this->printStatus('Occupations->Show',$resultCode);
            } else {
                $this->printStatus('Occupations->Show. NO DATA AVAILABLE FOR TEST. 0 OCCUPATIONS FOUND',false);
            }
        } else {
            $this->printStatus('Occupations->Show',$resultCode);
        }
    }

    private function testShowOccupation ($occupationId) {
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_occupations_show;
        $requestUrl = str_replace("{id}", $schoolId, $requestUrl);
        $occupation = $this->apiConsumer->doRequest($requestUrl);
        if(empty($occupation) || is_null($occupation))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }
    /*************************Occupations*********************************************/

    /*************************Denominations*******************************************/
    private function testListDenominations () {
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_denominations_list;
        $denominations = $this->apiConsumer->doRequest($requestUrl);
        if(empty($denominations) || is_null($denominations))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('Denominations->List',$resultCode);

        if($resultCode){
            // foreach school, show the school
            $json_string = json_decode($denominations, true);
            $json_length = count($json_string['denominations']['denomination']);
            if($json_length > 0) {
                print AppConfig::$lineBreak;
                for ($i = 0; $i < $json_length; $i++) {
                    $x = $json_string['denominations']['denomination'][$i];
                    $denominationId = $x['@id'];
                    $result_code = $this->testShowDenomination($denominationId);
                    if($result_code == false)
                    $resultCode = false;
                }
                $this->printStatus('Denominations->Show',$resultCode);
            } else {
                $this->printStatus('Denominations->Show. NO DATA AVAILABLE FOR TEST. 0 Denominations FOUND',false);
            }
        } else {
            $this->printStatus('Denominations->Show',$resultCode);
        }
    }

    private function testShowDenomination($denominationId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_denominations_show;
        $requestUrl = str_replace("{id}", $denominationId, $requestUrl);
        $denomination = $this->apiConsumer->doRequest($requestUrl);
        if(empty($denomination) || is_null($denomination))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }
    /*************************Denominations*******************************************/

    /*************************Communication Types ************************************/
    private function testListCommunicationTypes() {
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_communicationtypes;
        $commTypes = $this->apiConsumer->doRequest($requestUrl);
        if(empty($commTypes) || is_null($commTypes))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('CommunicationTypes->List',$resultCode);
        if($resultCode) {
            $json_string = json_decode($commTypes, true);
            $json_length = count($json_string['communicationTypes']['communicationType']);
            if($json_length > 0) {
                print AppConfig::$lineBreak;
                for ($i = 0; $i < $json_length; $i++) {
                    $x = $json_string['communicationTypes']['communicationType'][$i];
                    $communicationTypeId = $x['@id'];
                    $result_code = $this->testShowCommunicationType($communicationTypeId);
                    if($result_code == false)
                    $resultCode = false;
                }
                $this->printStatus('CommunicationTypes->Show',$resultCode);
            } else {
                $this->printStatus('CommunicationTypes->Show. NO DATA AVAILABLE FOR TEST. 0 CommunicationTypes FOUND',false);
            }
        } else {
            $this->printStatus('CommunicationTypes->Show',$resultCode);
        }
        return $resultCode;
    }

    private function testShowCommunicationType($communicationTypeId) {
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_communicationtypes_show;
        $requestUrl = str_replace("{id}", $communicationTypeId, $requestUrl);
        $commTypen = $this->apiConsumer->doRequest($requestUrl);
        if(empty($commTypen) || is_null($commTypen))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }
    /*************************Communication Types ************************************/

    /*************************Communications***** ************************************/
    private function testListCommunications($personId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_communications;
        $requestUrl = str_replace("{id}", $personId, $requestUrl);
        $communications = $this->apiConsumer->doRequest($requestUrl);
        if(empty($communications) || is_null($communications))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('Communications->List',$resultCode);
        if($resultCode) {
            $json_string = json_decode($communications, true);
            $json_length = count($json_string['communications']['communication']);
            if($json_length > 0) {
                print AppConfig::$lineBreak;
                for ($i = 0; $i < $json_length; $i++) {
                    $x = $json_string['communications']['communication'][$i];
                    $communicationId = $x['@id'];
                    $result_code = $this->testShowCommunication($communicationId);
                    if($result_code == false)
                    $resultCode = false;
                }
                $this->printStatus('Communications->Show',$resultCode);
            } else {
                $this->printStatus('Communications->Show. NO DATA AVAILABLE FOR TEST. 0 Communications FOUND',false);
            }
        } else {
            $this->printStatus('Communications->Show',$resultCode);
        }
        return $resultCode;
    }

    private function testShowCommunication($communicationId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_communications_show;
        $requestUrl = str_replace("{id}", $communicationId, $requestUrl);
        $comm = $this->apiConsumer->doRequest($requestUrl);
        if(empty($comm) || is_null($comm))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }

    private function testNewCommunications(){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_communications_new;
        $comm = $this->apiConsumer->doRequest($requestUrl);
        if(empty($comm) || is_null($comm))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('Communications->New',$resultCode);
        return $resultCode;
    }

    private function testEditCommunications(){

    }

    private function testCreateCommunications(){

    }

    private function testUpdateCommunications(){

    }

    private function testDeleteCommunications(){
    }
    /*************************Communications******************************************/

    /*************************Attributes**********************************************/
    public function testListAttributeGroups(){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_attributeGroups_list;
        $attributeGrps = $this->apiConsumer->doRequest($requestUrl);
        if(empty($attributeGrps) || is_null($attributeGrps))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('AttributeGroups->List',$resultCode);
        $attrGrpId = null;
        $json_string = json_decode($attributeGrps, true);
        $json_length = count($json_string['attributeGroups']['attributeGroup']);
        $showAttrGrpResultCode = true;
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_string['attributeGroups']['attributeGroup'][$i];
            $testAttrGrpId =  $x['@id'];
            $result_code = $this->testShowAttributeGroup($testAttrGrpId);
            if($result_code == false)
            $showAttrGrpResultCode = false;
        }
        $this->printStatus('AttributeGroups->Show', $showAttrGrpResultCode);
        return $resultCode;
    }

    private function testShowAttributeGroup($attrGrpId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_attributeGroups_show;
        $requestUrl = str_replace("{id}", $attrGrpId, $requestUrl);

        $attrGrp = $this->apiConsumer->doRequest($requestUrl);
        if(empty($attrGrp) || is_null($attrGrp))
        $resultCode = false;
        else
        $resultCode = true;
        $this->testListAttribute($attrGrpId);

        $json_string = json_decode($attrGrp, true);
        $json_length = count($json_string['attributeGroup']['attribute']);
        $show_sttribute_resultCode = true;
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_string['attributeGroup']['attribute'][$i];
            $attrId = $x['@id'];
            $result_code = $this->testShowAttribute($attrGrpId, $attrId);
            if($result_code == false)
            $show_sttribute_resultCode = false;
        }
        $this->printStatus("Attribute->Show. AttributeGroupId: ".$attrGrpId , $show_sttribute_resultCode);
        return $resultCode;
    }

    public function testListAttribute($attrGrpId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_attribute_list;
        $requestUrl = str_replace("{attributeGroupID}", $attrGrpId, $requestUrl);

        $attrGrp = $this->apiConsumer->doRequest($requestUrl);
        if(empty($attrGrp) || is_null($attrGrp))
        $resultCode = false;
        else
        $resultCode = true;
        $json_string = json_decode($attrGrp, true);
        $this->printStatus("Attribute->List. AttributeGroupId: ".$attrGrpId , $resultCode);
        return $resultCode;
    }

    /*
    public function testListSubStatuses(){
        $statuses = $this->apiConsumer->getStatuses();
        if(empty($statuses) || is_null($statuses))
        $resultCode = false;
        else
        $resultCode = true;
        if($resultCode) {
            $statusId = null;
            $json_string = json_decode($statuses, true);
            $json_length = count($json_string['statuses']['status']);
            print AppConfig::$lineBreak;
            for ($i = 0; $i < $json_length; $i++) {
                $x = $json_string['statuses']['status'][$i];
                $statusId = $x['@id'];
                $result_code = $this->testListSubStatus($statusId);
                if($result_code == false)
                $resultCode = false;
            }
            $this->printStatus('SubStatuses->List', $resultCode);
        } else {
            $this->printStatus('SubStatuses->List',$resultCode);
        }
        return $resultCode;
    }

    private function testListAttribute($statusId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_substatuses_list;
        $requestUrl = str_replace("{statusID}", $statusId, $requestUrl);

        $subStatuses = $this->apiConsumer->doRequest($requestUrl);
        if(empty($subStatuses) || is_null($subStatuses))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('SubStatuses->List StatusId: '.$statusId, $resultCode);
        $json_string = json_decode($subStatuses, true);

        $json_length = count($json_string['subStatuses']['subStatus']);
        print AppConfig::$lineBreak;
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_string['subStatuses']['subStatus'][$i];
            $subStatusId = $x['@id'];
            $this->testShowSubStatus($statusId, $subStatusId);
        }

        //print AppConfig::$lineBreak;
        //print "<pre>".print_r($json_string, true)."</pre>";
        // print AppConfig::$lineBreak;
        return $resultCode;
    }
    */
    public function testShowAttribute($attrGrpId, $attrId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_attribute_show;
        $requestUrl = str_replace("{attributeGroupID}", $attrGrpId, $requestUrl);
        $requestUrl = str_replace("{id}", $attrId, $requestUrl);
        $attribute = $this->apiConsumer->doRequest($requestUrl);
        if(empty($attribute) || is_null($attribute))
        $resultCode = false;
        else
        $resultCode = true;

        return $resultCode;
    }
    /*************************Attributes**********************************************/

    /*************************People Attributes***************************************/
    public function testNewPeopleAttribute($peopleId){
        $requestUrl = $this->baseUrl.AppConfig::$f1_peopleAttributes_new;
        $requestUrl = str_replace("{peopleID}", $peopleId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'People Attribute->New: ',$resultCode);
        return $resultCode;
    }

    public function testEditPeopleAttribute(){

    }

    public function testUpdatePeopleAttribute(){

    }

    public function testCreatePeopleAttribute($peopleId) {
        $requestBody = <<<EOD
        { "attribute": { "@id": "", "@uri": "", "person": { "@id": "$peopleId", "@uri": "" }, "attributeGroup": { "@id": "", "@uri": "", "name": null, "attribute": { "@id": "", "@uri": "", "name": null } }, "startDate": null, "endDate": null, "comment": null, "createdDate": null, "lastUpdatedDate": null } }
EOD;
    }

    public function testShowPeopleAttribute($peopleId){

    }
    /*************************People Attributes***************************************/

    /*************************Address Types*******************************************/

    public function testListAddressTypes(){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_addresstypes;
        $addressTypes = $this->apiConsumer->doRequest($requestUrl);
        if(empty($addressTypes) || is_null($addressTypes))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('AddressTypes->List', $resultCode);
        $json_string = json_decode($addressTypes, true);
        $json_length = count($json_string['addressTypes']['addressType']);
        $addresstype_show_resultcode = true;
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_string['addressTypes']['addressType'][$i];
            $addressTypeId =  $x['@id'];
            $result_code = $this->testShowAddressTypes($addressTypeId);
            if($result_code == false)
            $addresstype_show_resultcode = false;
        }

        $this->printStatus('AddressTypes->Show', $addresstype_show_resultcode);
        return $resultCode;
    }

    public function testShowAddressTypes($addressTypeId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_addresstype_show;
        $requestUrl = str_replace("{id}", $addressTypeId, $requestUrl);
        $addressTypes = $this->apiConsumer->doRequest($requestUrl);
        if(empty($addressTypes) || is_null($addressTypes))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }
    /*************************Address Types*******************************************/

    /*************************Household Member Types**********************************/
    /*************************List, Show**********************************************/
    //
    // GET HOUSEHOLD MEMBER TYPES
    //
    public function testListHouseholdMemberTypes(){
        $memberTypes = $this->apiConsumer->getHouseholdMemberTypes();
        if(empty($memberTypes) || is_null($memberTypes))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('HouseholdMemberTypes->List', $resultCode);
        if($resultCode) {
            $householdMemberTypeId = null;
            $json_string = json_decode($memberTypes, true);
            $json_length = count($json_string['householdMemberTypes']['householdMemberType']);
            for ($i = 0; $i < $json_length; $i++) {
                $x = $json_string['householdMemberTypes']['householdMemberType'][$i];
                $householdMemberTypeId = $x['@id'];
                $result_code = $this->testShowHouseholdMemberType($householdMemberTypeId);
                if($result_code == false)
                $resultCode = false;
            }
            $this->printStatus('HouseholdMemberType->Show',$resultCode);
        }
        return $resultCode;
    }

    public function testShowHouseholdMemberType($householdMemberTypeId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdMemberTypes_show;
        $requestUrl = str_replace("{id}", $householdMemberTypeId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }
    /*************************Household Member Types**********************************/

    //
    // Get a person
    //
    public function testGetPerson($personId){
        //[firstName] => Jas
        //[lastName] => from FB
        $get_person = $this->apiConsumer->getPerson($personId);
        if(empty($get_person->id) || is_null($get_person->id))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('getPerson',$resultCode);

        print "<pre>".print_r($get_person, true)."</pre>";
        return $resultCode;
    }

    
    /*************************Create Household****************************************/
    /*********Household->New, Create, Show, Search, Edit, Update**********************/

    public function testHousehold(){
        // Hosuehold->New
        $this->testNewHousehold();
        // Hosuehold->Create
        $householdFirstName = "Test HH";
        $householdLastName = gettimeofday(true);
        $household_id = 15809257;// $this->testCreateHousehold($householdFirstName, $householdLastName);

        // HouseholdAddress->Create
        $address2 = gettimeofday(true);
        $addressTypeId = 1; //1, Primary 2, Secondary 3, College 4, Vacation 5, Business 6, Org 7, Previous 8, Statement 101, Mail Returned Incorrect
        $address = new Address();
        $address->address1 = "6363 N State Hwy 161";
        $address->address2 = $address2;
        $address->city = "Irving";
        $address->stProvince = "Texas";
        $address->country = "USA";
        $address->county = "Dallas";
        $address->addressTypeID = $addressTypeId;
        $address->householdID = $household_id;
        $rawResponse = $this->testCreateHouseholdAddress($address);
        if(empty($rawResponse) || is_null($rawResponse))
        $createAddressResultCode = false;
        else {
            $address = $this->apiConsumer->populateAddress($rawResponse);
            if($address->addressID > 0) {
                // HouseholdAddress->Show
                $this->testShowHouseholdAddress($household_id, $address->addressID);
            } else {
                $createAddressResultCode = false;
            }
        }
        $this->printStatus( 'Household Address->Create: ',$createAddressResultCode);
        $found_addresses = $this->testListHouseholdAddresses($household_id);
        $householdAddressesListResultCode = false;
        if(count($found_addresses) > 0){
            for ($i = 0; $i < count($found_addresses); $i++) {
                $x = $found_addresses[$i];
                if($address->address2 == $address2) {
                    $householdAddressesListResultCode = true;
                    break;
                }
            }

            $householdAddressesListResultCode = false;
        }

        // HouseholdAddress->Show
        // HouseholdAddress->List
        $this->printStatus( 'Household Address-List: ',$householdAddressesListResultCode);

        if($household_id > 0) {
            // Hosuehold->Show
            $this->testShowHousehold($household_id);
            // Household->Search
            $this->testSearchHousehold($householdFirstName, $householdLastName);
            // Test Update and Edit in a single call to testUpdateHousehold
            // Household->Edit
            // Household->Update
            $this->testUpdateHousehold($household_id);
        }
        return $household_id;
    }
    //
    // CREATE A HOUSEHOLD
    // Returns HouseholdId of the newly created Household
    //
    private function testCreateHousehold($householdFirstName, $householdLastName) {
        $result = $this->apiConsumer->addHousehold($householdFirstName, $householdLastName, AppConfig::$f1_household_create);
        if(empty($result) || is_null($result))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus('Household->Create', $resultCode);
        // Print information about the Household
        $json_string = json_decode($result, true);
        $household_id =  $json_string["household"]["@id"];
        print 'New HouseholdID: '.$household_id;

        return $household_id;
    }

    private function testShowHousehold($householdId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_household_show;
        $requestUrl = str_replace("{id}", $householdId, $requestUrl);
        $household = $this->apiConsumer->doRequest($requestUrl);
        if(empty($household) || is_null($household))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }

    private function testCreateHouseholdAddress($address){
        $requestBody = <<<EOD
        {"address": {
        "@id": "",
        "@uri": "",
        "household": {
            "@id": "$address->householdID",
            "@uri": ""
        },
        "person": {
            "@id": "",
            "@uri": ""
        },
        "addressType": {
            "@id": "$address->addressTypeID",
            "@uri": "",
            "name": null
        },
        "address1": "$address->address1",
        "address2": "$address->address2",
        "address3": "$address->address3",
        "city": "$address->city",
        "postalCode": "$address->postalCode",
        "county": "$address->county",
        "country": "$address->country",
        "stProvince": "$address->stProvince",
        "carrierRoute": "$address->carrierRoute",
        "deliveryPoint": "$address->deliveryPoint",
        "addressDate": null,
        "addressComment": null,
        "uspsVerified": "false",
        "addressVerifiedDate": null,
        "lastVerificationAttemptDate": null,
        "createdDate": null,
        "lastUpdatedDate": null
    }}

EOD;
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_household_address;
        $requestUrl = str_replace("{householdID}", $address->householdID, $requestUrl);
        return $this->apiConsumer->postRequest($requestUrl, $requestBody);
    }

    private function testShowHouseholdAddress($householdId, $addressId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_household_address_show;
        $requestUrl = str_replace("{id}", $addressId, $requestUrl);
        $requestUrl = str_replace("{householdID}", $householdId, $requestUrl);
        $householdAddress = $this->apiConsumer->doRequest($requestUrl);
        if(empty($householdAddress) || is_null($householdAddress))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'Household Address-Show: ',$resultCode);
        return $resultCode;
    }

    private function testListHouseholdAddresses($householdId) {
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_household_address;
        $requestUrl = str_replace("{householdID}", $householdId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);

        $found_addresses[] = array();
        $json_array = json_decode(strstr($rawResponse, '{"addresses":{'), true);

        // Results found?
        if (isset($json_array['addresses']['address'])) {
            $json_length = count($json_array['addresses']['address']);
        }

        // Loop through each found household
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_array['addresses']['address'][$i];
            $address = $this->populateAddressFromJsonObject($x);
            $found_addresses[] = $address;
        }
        return $found_addresses;
    }

    private function testNewHouseholdAddress($householdID){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdAddress_new;
        $requestUrl = str_replace("{householdID}", $householdID, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'Household Address->New: ',$resultCode);
        return $resultCode;
    }

    private function testEditHouseholdAddress($householdID, $id, &$rawResponse){
        // $householdID, $id
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdAddress_edit;
        $requestUrl = str_replace("{householdID}", $householdID, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'Household Address->Edit: ',$resultCode);
        return $resultCode;
    }

    private function testUpdateHouseholdAddress($householdID, $id){
        $rawResponse = "";
        $resultCode = $this->testEditHouseholdAddress($householdID, $id, $rawResponse);
        if($resultCode) {
            $address =  $this->apiConsumer->populateAddress($rawResponse);
            $updatedAddress2 = gettimeofday(true);
            $address->address2 = $updatedAddress2;
            $rawResponse = $this->apiConsumer->updateAddress($address);
            if(empty($rawResponse) || is_null($rawResponse))
            $updateResultCode = false;
            else {
                $address =  $this->apiConsumer->populateAddress($rawResponse);
                if($address->address2 == $updatedAddress2)
                $updateResultCode = true;
            }
            $this->printStatus( 'Household Address->Update: ',$updateResultCode);
        }
    }

    private function testDeleteHouseholdAddress($householdID, $id){
        // Delete it
        // This method will return a 204 - No Content if a successful deletion occurs
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdAddress_delete;
        $requestUrl = str_replace("{householdID}", $householdID, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 204 );

        // Then Use Show to fetch it. We should not be able to get to it
        // If delete is called the resource will no longer be available from the API and will return a 410 - Entity is GONE if the resource is ever requested again
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdAddress_show;
        $requestUrl = str_replace("{householdID}", $householdID, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 410 );
    }

    private function testCreateHouseholdCommunication($communication) {
        $requestBody = <<<EOD
        {"communication": {
        "@id": "",
        "@uri": "",
        "household": {
            "@id": "$communication->householdID",
            "@uri": ""
        },
        "person": {
            "@id": "",
            "@uri": ""
        },
        "communicationType": {
            "@id": "$communication->communicationTypeID",
            "@uri": "",
            "name": null
        },
        "communicationGeneralType": null,
        "communicationValue": "$communication->communicationValue",
        "searchCommunicationValue": null,
        "listed": "true",
        "communicationComment": null,
        "createdDate": null,
        "lastUpdatedDate": null
    }}
EOD;
        $requestUrl = $this->baseUrl.AppConfig::$f1_household_communications;
        $requestUrl = str_replace("{householdID}", $communication->householdID, $requestUrl);
        return $this->apiConsumer->postRequest($requestUrl, $requestBody);
    }

    private function testShowHouseholdCommunication($householdId, $commId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_household_communication_show;
        $requestUrl = str_replace("{id}", $commId, $requestUrl);
        $requestUrl = str_replace("{householdID}", $householdId, $requestUrl);
        $householdComms = $this->apiConsumer->doRequest($requestUrl);
        if(empty($householdComms) || is_null($householdComms))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }

    private function testListHouseholdCommunications($householdId) {
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdCommunications_list;
        $requestUrl = str_replace("{householdID}", $householdId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);

        $found_communications[] = array();
        $json_array = json_decode(strstr($rawResponse, '{"communications":{'), true);

        // Results found?
        if (isset($json_array['communications']['communication'])) {
            $json_length = count($json_array['communications']['communication']);
        }

        // Loop through each found household
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_array['communications']['communication'][$i];
            $communication = $this->populateCommunicationFromJsonObject($x);
            $found_communications[] = $communication;
        }
        return $found_communications;
    }

    private function testNewHouseholdCommunication($householdID){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdCommunications_new;
        $requestUrl = str_replace("{householdID}", $householdID, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'Household Communication->New: ',$resultCode);
        return $resultCode;
    }

    private function testEditHouseholdCommunication($householdID, $id, &$rawResponse){
        // $householdID, $id
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdCommunications_edit;
        $requestUrl = str_replace("{householdID}", $householdID, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'Household Communication->Edit: ',$resultCode);
        return $resultCode;
    }

    private function testUpdateHouseholdCommunication($householdID, $id){
        $rawResponse = "";
        $resultCode = $this->testEditHouseholdCommunication($householdID, $id, $rawResponse);
        if($resultCode) {
            $communication =  $this->apiConsumer->populateCommunication($rawResponse);
            $updatedCommVal = gettimeofday(true)."@gmail.com";
            $communication->communicationValue = $updatedCommVal;

            $rawResponse = $this->apiConsumer->updateCommunication($communication);
            if(empty($rawResponse) || is_null($rawResponse))
            $updateResultCode = false;
            else {
                $communication =  $this->apiConsumer->populateCommunication($rawResponse);
                if($communication->communicationValue == $updatedCommVal)
                $updateResultCode = true;
            }
            $this->printStatus( 'Household Comunication->Update: ',$updateResultCode);
        }
    }

    private function testDeleteHouseholdCommunication($householdID, $id){
        // Delete it
        // This method will return a 204 - No Content if a successful deletion occurs
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdCommunications_delete;
        $requestUrl = str_replace("{householdID}", $householdID, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 204 );

        // Then Use Show to fetch it. We should not be able to get to it
        // If delete is called the resource will no longer be available from the API and will return a 410 - Entity is GONE if the resource is ever requested again
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_householdCommunications_show;
        $requestUrl = str_replace("{householdID}", $householdID, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 410 );
    }

    private function testNewHousehold(){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_household_new;
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'Household->New: ',$resultCode);
        return $resultCode;
    }

    private function testEditHousehold($householdId, &$rawResponse){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_household_edit;
        $requestUrl = str_replace("{id}", $householdId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }

    private function testUpdateHousehold($householdId){
        $rawResponse = "";
        $resultCode = $this->testEditHousehold($householdId, $rawResponse);
        $this->printStatus( 'Household->Edit: ',$resultCode);
        if($resultCode) {
            // Create a Household object first
            $household = $this->apiConsumer->populateHousehold($rawResponse);
            // Change something on the Household
            $household->householdFirstName .= "U";
            $updated_household = $this->apiConsumer->updateHousehold($household);
            $updateHouseholdResultcode = false;
            if(empty($updated_household) || is_null($updated_household))
            $updateHouseholdResultcode = false;
            else
            $updateHouseholdResultcode = true;
            if($updateHouseholdResultcode) {
                $updateHouseholdResultcode = false;
                $updated_household = $this->apiConsumer->populateHousehold($updated_household);
                if(substr($updated_household->householdFirstName, strlen($updated_household->householdFirstName)-1, 1) == "U")
                $updateHouseholdResultcode = true;
            }
            $this->printStatus( 'Household->Update: ',$updateHouseholdResultcode);
        }
    }

    private function testSearchHousehold($householdFirstName, $householdLastName){
        $fullname = $householdFirstName." ". $householdLastName;
        $searchParameter = "?searchfor=".Util::urlencode_rfc3986($fullname);
        $households = $this->apiConsumer->householdSearch($searchParameter);
        $resultCode = false;
        if(count($households) > 0){
            for ($i = 0; $i < count($households); $i++) {
                $household = $households[$i];
                if($household->$householdName == $fullname){
                    $resultCode = true;
                    break;
                }
            }
        }
        $this->printStatus( 'People-Search: ',$resultCode);
    }
    /*************************Create Household****************************************/

    /*************************PEOPLE**************************************************/

    public function testPerson($householdId){
        // Hosuehold->New
        $this->testNewHousehold();
        // Hosuehold->Create
        $householdFirstName = "Test HH";
        $householdLastName = gettimeofday(true);

        $firstName = 'Test';
        $lastName = 'FB'.date( "d/m/Y", time() );
        $household_id = $this->testCreatePerson($firstName, $lastName, $householdId );

        // HouseholdAddress->Create
        $address2 = gettimeofday(true);
        $addressTypeId = 1; //1, Primary 2, Secondary 3, College 4, Vacation 5, Business 6, Org 7, Previous 8, Statement 101, Mail Returned Incorrect
        $address = new Address();
        $address->address1 = "6363 N State Hwy 161";
        $address->address2 = $address2;
        $address->city = "Irving";
        $address->stProvince = "Texas";
        $address->country = "USA";
        $address->county = "Dallas";
        $address->addressTypeID = $addressTypeId;
        $rawResponse = $this->testCreateHouseholdAddress($address);
        if(empty($rawResponse) || is_null($rawResponse))
        $createAddressResultCode = false;
        else {
            $address = $this->apiConsumer->populateAddress($rawResponse);
            if($address->addressID > 0) {
                // HouseholdAddress->Show
                $this->testShowHouseholdAddress($household_id, $address->addressID);
            } else {
                $createAddressResultCode = false;
            }
        }
        $this->printStatus( 'Household Address-Show: ',$createAddressResultCode);
        $found_addresses = $this->testListHouseholdAddresses($household_id);
        $householdAddressesListResultCode = false;
        if(count($found_addresses) > 0){
            for ($i = 0; $i < count($found_addresses); $i++) {
                $x = $found_addresses[$i];
                if($address->address2 == $address2) {
                    $householdAddressesListResultCode = true;
                    break;
                }
            }

            $householdAddressesListResultCode = false;
        }

        // HouseholdAddress->Show
        // HouseholdAddress->List
        $this->printStatus( 'Household Address-List: ',$householdAddressesListResultCode);

        if($household_id > 0) {
            // Hosuehold->Show
            $this->testShowHousehold($household_id);
            // Household->Search
            $this->testSearchHousehold($householdFirstName, $householdLastName);
            // Test Update and Edit in a single call to testUpdateHousehold
            // Household->Edit
            // Household->Update
            $this->testUpdateHousehold($household_id);
        }
    }

    //
    // CREATE A Person
    // Returns id of the newly created person
    public function testCreatePerson($householdId) {

        $statusId = 15609; // New from Facebook
        $householdMemberTypeId = 101; // visitor

        // Create a person with no Address or Comm values
        $person = new Person();
        $person->firstName = 'Test';
        $person->lastName = 'FB'.date( "d/m/Y", time() );
        $person->gender = 'Male';
        $person->householdMemberTypeId = 101;
        $person->status = $statusId;
        $person->householdId = $householdId;
        $person->maritalStatus = "Married";
        $created_person = $this->apiConsumer->addPerson($person);
        if(empty($created_person->id) || is_null($created_person->id))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'People->Create: ',$resultCode);
        return $created_person->id;
    }

    private function testShowPerson($personId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_show;
        $requestUrl = str_replace("{id}", $personId, $requestUrl);
        $person = $this->apiConsumer->doRequest($requestUrl);
        if(empty($person) || is_null($person))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }

    private function testCreatePersonAddress($address){
        $requestBody = <<<EOD
        {"address": {
        "@id": "",
        "@uri": "",
        "household": {
            "@id": "",
            "@uri": ""
        },
        "person": {
            "@id": "$address->individualID",
            "@uri": ""
        },
        "addressType": {
            "@id": "$address->addressTypeID",
            "@uri": "",
            "name": null
        },
        "address1": "$address->address1",
        "address2": "$address->address2",
        "address3": "$address->address3",
        "city": "$address->city",
        "postalCode": "$address->postalCode",
        "county": "$address->county",
        "country": "$address->country",
        "stProvince": "$address->stProvince",
        "carrierRoute": "$address->carrierRoute",
        "deliveryPoint": "$address->deliveryPoint",
        "addressDate": null,
        "addressComment": null,
        "uspsVerified": "false",
        "addressVerifiedDate": null,
        "lastVerificationAttemptDate": null,
        "createdDate": null,
        "lastUpdatedDate": null
    }}

EOD;
        $requestUrl = $this->baseUrl.AppConfig::$f1_peopleAddress_create;
        $requestUrl = str_replace("{personID}", $address->individualID, $requestUrl);
        return $this->apiConsumer->postRequest($requestUrl, $requestBody);
    }

    private function testShowPersonAddress($personId, $addressId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleAddress_show;
        $requestUrl = str_replace("{id}", $addressId, $requestUrl);
        $requestUrl = str_replace("{personID}", $personId, $requestUrl);
        $personAddress = $this->apiConsumer->doRequest($requestUrl);
        if(empty($personAddress) || is_null($personAddress))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'Person Address-Show: ',$resultCode);
        return $resultCode;
    }

    private function testListPersondAddresses($personId) {
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleAddress_list;
        $requestUrl = str_replace("{personID}", $personId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);

        $found_addresses[] = array();
        $json_array = json_decode(strstr($rawResponse, '{"addresses":{'), true);

        // Results found?
        if (isset($json_array['addresses']['address'])) {
            $json_length = count($json_array['addresses']['address']);
        }

        // Loop through each found household
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_array['addresses']['address'][$i];
            $address = $this->populateAddressFromJsonObject($x);
            $found_addresses[] = $address;
        }
        return $found_addresses;
    }

    private function testNewPersonAddress($personId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleAddress_new;
        $requestUrl = str_replace("{personID}", personId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'People Address->New: ',$resultCode);
        return $resultCode;
    }

    private function testEditPeopleAddress($personId, $id, &$rawResponse){
        // $householdID, $id
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleAddress_edit;
        $requestUrl = str_replace("{personID}", $personId, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'People Address->Edit: ',$resultCode);
        return $resultCode;
    }

    private function testUpdatePeopleAddress($personId, $id){
        $rawResponse = "";
        $resultCode = $this->testEditPeopleAddress($personId, $id, $rawResponse);
        if($resultCode) {
            $address =  $this->apiConsumer->populateAddress($rawResponse);
            $updatedAddress2 = gettimeofday(true);
            $address->address2 = $updatedAddress2;
            $rawResponse = $this->apiConsumer->updateAddress($address);
            if(empty($rawResponse) || is_null($rawResponse))
            $updateResultCode = false;
            else {
                $address =  $this->apiConsumer->populateAddress($rawResponse);
                if($address->address2 == $updatedAddress2)
                $updateResultCode = true;
            }
            $this->printStatus( 'People Address->Update: ', $updateResultCode);
        }
    }

    private function testDeletePersonAddress($personId, $id){
        // Delete it
        // This method will return a 204 - No Content if a successful deletion occurs
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleAddress_delete;
        $requestUrl = str_replace("{personID}", $personId, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 204 );

        // Then Use Show to fetch it. We should not be able to get to it
        // If delete is called the resource will no longer be available from the API and will return a 410 - Entity is GONE if the resource is ever requested again
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleAddress_show;
        $requestUrl = str_replace("{personID}", $householdID, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 410 );
    }

    private function testCreatePersonCommunication($communication) {
        $requestBody = <<<EOD
        {"communication": {
        "@id": "",
        "@uri": "",
        "household": {
            "@id": "",
            "@uri": ""
        },
        "person": {
            "@id": "$communication->individualID",
            "@uri": ""
        },
        "communicationType": {
            "@id": "$communication->communicationTypeID",
            "@uri": "",
            "name": null
        },
        "communicationGeneralType": null,
        "communicationValue": "$communication->communicationValue",
        "searchCommunicationValue": null,
        "listed": "true",
        "communicationComment": null,
        "createdDate": null,
        "lastUpdatedDate": null
    }}
EOD;
        $requestUrl = $this->baseUrl.AppConfig::$f1_peopleCommunications_createt;
        $requestUrl = str_replace("{personID}", $communication->individualID, $requestUrl);
        return $this->apiConsumer->postRequest($requestUrl, $requestBody);
    }

    private function testShowPeopleCommunication($peopleId, $commId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleCommunications_show;
        $requestUrl = str_replace("{id}", $commId, $requestUrl);
        $requestUrl = str_replace("{personID}", $peopleId, $requestUrl);
        $peopleComms = $this->apiConsumer->doRequest($requestUrl);
        if(empty($peopleComms) || is_null($peopleComms))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }

    private function testListPeopleCommunications($peopleId) {
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleCommunications_list;
        $requestUrl = str_replace("{personID}", $peopleId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);

        $found_communications[] = array();
        $json_array = json_decode(strstr($rawResponse, '{"communications":{'), true);

        // Results found?
        if (isset($json_array['communications']['communication'])) {
            $json_length = count($json_array['communications']['communication']);
        }

        // Loop through each found household
        for ($i = 0; $i < $json_length; $i++) {
            $x = $json_array['communications']['communication'][$i];
            $communication = $this->populateCommunicationFromJsonObject($x);
            $found_communications[] = $communication;
        }
        return $found_communications;
    }

    private function testNewPeopleCommunication($peopleId){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleCommunications_new;
        $requestUrl = str_replace("{personID}", $peopleId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'People Communication->New: ',$resultCode);
        return $resultCode;
    }

    private function testEditPeopleCommunication($peopleId, $id, &$rawResponse){
        // $householdID, $id
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleCommunications_edit;
        $requestUrl = str_replace("{personID}", $peopleId, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'People Communication->Edit: ',$resultCode);
        return $resultCode;
    }

    private function testUpdatePeopleCommunication($peopleId, $id){
        $rawResponse = "";
        $resultCode = $this->testEditPeopleCommunication($peopleId, $id, $rawResponse);
        if($resultCode) {
            $communication =  $this->apiConsumer->populateCommunication($rawResponse);
            $updatedCommVal = gettimeofday(true)."@gmail.com";
            $communication->communicationValue = $updatedCommVal;

            $rawResponse = $this->apiConsumer->updateCommunication($communication);
            if(empty($rawResponse) || is_null($rawResponse))
            $updateResultCode = false;
            else {
                $communication =  $this->apiConsumer->populateCommunication($rawResponse);
                if($communication->communicationValue == $updatedCommVal)
                $updateResultCode = true;
            }
            $this->printStatus( 'People Comunication->Update: ',$updateResultCode);
        }
    }

    private function testDeletePeopleCommunication($peopleId, $id){
        // Delete it
        // This method will return a 204 - No Content if a successful deletion occurs
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleCommunications_delete;
        $requestUrl = str_replace("{personID}", $peopleId, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 204 );

        // Then Use Show to fetch it. We should not be able to get to it
        // If delete is called the resource will no longer be available from the API and will return a 410 - Entity is GONE if the resource is ever requested again
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_peopleCommunications_show;
        $requestUrl = str_replace("{personID}", $peopleId, $requestUrl);
        $requestUrl = str_replace("{id}", $id, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 410 );
    }

    private function testNewPerson(){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_new;
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        $this->printStatus( 'People->New: ',$resultCode);
        return $resultCode;
    }

    private function testEditPerson($peopleId, &$rawResponse){
        $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_people_edit;
        $requestUrl = str_replace("{id}", $peopleId, $requestUrl);
        $rawResponse = $this->apiConsumer->doRequest($requestUrl);
        if(empty($rawResponse) || is_null($rawResponse))
        $resultCode = false;
        else
        $resultCode = true;
        return $resultCode;
    }

    private function testUpdatePerson($peopleId){
        $rawResponse = "";
        $resultCode = $this->testEditHousehold($peopleId, $rawResponse);
        $this->printStatus( 'Person->Edit: ',$resultCode);
        if($resultCode) {
            // Create a Household object first
            $person = $this->apiConsumer->populatePerson($rawResponse);
            // Change something on the Household
            $person->firstName .= "U";
            $updated_person = $this->apiConsumer->updatePerson($person);
            $updatePersonResultcode = false;
            if(empty($updated_person) || is_null($updated_person))
            $updatePersonResultcode = false;
            else
            $updatePersonResultcode = true;
            if($updatePersonResultcode) {
                $updatePersonResultcode = false;
                $updated_person = $this->apiConsumer->populatePerson($updated_person);
                if(substr($updated_person->firstName, strlen($updated_person->firstName)-1, 1) == "U")
                $updatePersonResultcode = true;
            }
            $this->printStatus( 'Person->Update: ',$updatePersonResultcode);
        }
    }

    public function testPeopleSearch($firstName, $lastName, $eMail) {

        $found_individuals = $this->apiConsumer->peopleSearch($firstName, $lastName, $eMail);
        if(count($found_individuals) == 0)
        $resultCode = false;
        else
        $resultCode = true;

        $this->printStatus('peopleSearch for '.$firstName.', '.$lastName.', '.$eMail,$resultCode);
        print 'Found '.count($found_individuals).' individuals';

        if(count($found_individuals) > 0) {
            for($i=0; $i<count($found_individuals); $i++) {
                $person = $found_individuals[$i];
                //print $person->firstName.$person->lastName;
                if(strtolower($person->firstName) == strtolower($firstName) && strtolower($person->lastName) == strtolower($lastName)) {
                    //print 'Hiii';
                    $communications = $person->communications;
                    if(count($communications) > 0){
                        for($j=0; $j<count($communications); $j++) {
                            $communication = $communications[$j];
                            if(strtolower($communication->communicationValue) == strtolower($eMail)){
                                $found_individual = $person;
                            }
                        }
                    }
                }
            }
        }

        print '<pre>'.print_r($found_individual, true).'</pre>';
        return $resultCode;
    }
   /*************************PEOPLE**************************************************/

   /**************************Address/Communications***************************/
    public function testAddress(){
        // Address->New
        $this->testNewAddress();
        // Address->Create
        $address2 = gettimeofday(true);
        $addressTypeId = 1; //1, Primary 2, Secondary 3, College 4, Vacation 5, Business 6, Org 7, Previous 8, Statement 101, Mail Returned Incorrect
        $address = new Address();
        $address->address1 = "6363 N State Hwy 161";
        $address->address2 = $address2;
        $address->city = "Irving";
        $address->stProvince = "Texas";
        $address->country = "USA";
        $address->county = "Dallas";
        $address->addressTypeID = $addressTypeId;
        $rawResponse = $this->testCreateAddress($address);
        if(empty($rawResponse) || is_null($rawResponse))
        $createAddressResultCode = false;
        else {
            $address = $this->apiConsumer->populateAddress($rawResponse);
            if($address->addressID > 0) {
                // HouseholdAddress->Show
                $this->testShowAddress($address->addressID);
            } else {
                $createAddressResultCode = false;
            }
        }
        $this->printStatus( 'Address->Show: ',$createAddressResultCode);

        if($address->addressID > 0) {
            $this->testUpdateAddress($address->addressID);
            $this->testDeleteAddress($address->addressID);
        }
    }

    public function testCommunication(){
        // Communication->New
        $this->testNewCommunication();
        // Communication->Create
        $comm = new Communication();
        $comm->communicationTypeID = 4; // email
        $comm->communicationValue = "test@gmail.com";
        $rawResponse = $this->testCreateCommunication($address);
        if(empty($rawResponse) || is_null($rawResponse))
        $createCommResultCode = false;
        else {
            $comm = $this->apiConsumer->populateCommunication($rawResponse);
            if($comm->communicationId > 0) {
                // HouseholdAddress->Show
                $this->testShowCommunication($comm->communicationId);
            } else {
                $createCommResultCode = false;
            }
        }
        $this->printStatus( 'Communication->Show: ',$createCommResultCode);

        if($comm->communicationId > 0) {
            $this->testUpdateCommunication($comm->communicationId);
            $this->testDeleteCommunication($comm->communicationId);
        }
    }

    private function testCreateAddress($address){
   
    $requestBody = <<<EOD
        {"address": {
        "@id": "",
        "@uri": "",
        "household": {
            "@id": "",
            "@uri": ""
        },
        "person": {
            "@id": "$address->individualID",
            "@uri": ""
        },
        "addressType": {
            "@id": "$address->addressTypeID",
            "@uri": "",
            "name": null
        },
        "address1": "$address->address1",
        "address2": "$address->address2",
        "address3": "$address->address3",
        "city": "$address->city",
        "postalCode": "$address->postalCode",
        "county": "$address->county",
        "country": "$address->country",
        "stProvince": "$address->stProvince",
        "carrierRoute": "$address->carrierRoute",
        "deliveryPoint": "$address->deliveryPoint",
        "addressDate": null,
        "addressComment": null,
        "uspsVerified": "false",
        "addressVerifiedDate": null,
        "lastVerificationAttemptDate": null,
        "createdDate": null,
        "lastUpdatedDate": null
    }}

EOD;
    $requestUrl = $this->baseUrl.AppConfig::$f1_address_create;
    return $this->apiConsumer->postRequest($requestUrl, $requestBody);
}

    private function testShowAddress( $addressId){
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_address_show;
    $requestUrl = str_replace("{id}", $addressId, $requestUrl);
    $personAddress = $this->apiConsumer->doRequest($requestUrl);
    if(empty($personAddress) || is_null($personAddress))
    $resultCode = false;
    else
    $resultCode = true;
    $this->printStatus( 'Address->Show: ',$resultCode);
    return $resultCode;
}

    private function testNewAddress(){
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_address_new;
    $rawResponse = $this->apiConsumer->doRequest($requestUrl);
    if(empty($rawResponse) || is_null($rawResponse))
    $resultCode = false;
    else
    $resultCode = true;
    $this->printStatus( 'Address->New: ',$resultCode);
    return $resultCode;
}

private function testEditAddress($id, &$rawResponse){
    // $householdID, $id
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_address_edit;
    $requestUrl = str_replace("{id}", $id, $requestUrl);
    $rawResponse = $this->apiConsumer->doRequest($requestUrl);
    if(empty($rawResponse) || is_null($rawResponse))
    $resultCode = false;
    else
    $resultCode = true;
    $this->printStatus( 'Address->Edit: ',$resultCode);
    return $resultCode;
}

private function testUpdateAddress($id){
    $rawResponse = "";
    $resultCode = $this->testEditAddress($id, $rawResponse);
    if($resultCode) {
        $address =  $this->apiConsumer->populateAddress($rawResponse);
        $updatedAddress2 = gettimeofday(true);
        $address->address2 = $updatedAddress2;
        $rawResponse = $this->apiConsumer->updateAddress($address);
        if(empty($rawResponse) || is_null($rawResponse))
        $updateResultCode = false;
        else {
            $address =  $this->apiConsumer->populateAddress($rawResponse);
            if($address->address2 == $updatedAddress2)
            $updateResultCode = true;
        }
        $this->printStatus( 'Address->Update: ', $updateResultCode);
    }
}

private function testDeleteAddress($id){
    // Delete it
    // This method will return a 204 - No Content if a successful deletion occurs
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_address_delete;
    $requestUrl = str_replace("{id}", $id, $requestUrl);
    $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 204 );

    // Then Use Show to fetch it. We should not be able to get to it
    // If delete is called the resource will no longer be available from the API and will return a 410 - Entity is GONE if the resource is ever requested again
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_address_show;
    $requestUrl = str_replace("{id}", $id, $requestUrl);
    $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 410 );
}

private function testCreateCommunication($communication) {
    $requestBody = <<<EOD
        {"communication": {
        "@id": "",
        "@uri": "",
        "household": {
            "@id": "",
            "@uri": ""
        },
        "person": {
            "@id": "$communication->individualID",
            "@uri": ""
        },
        "communicationType": {
            "@id": "$communication->communicationTypeID",
            "@uri": "",
            "name": null
        },
        "communicationGeneralType": null,
        "communicationValue": "$communication->communicationValue",
        "searchCommunicationValue": null,
        "listed": "true",
        "communicationComment": null,
        "createdDate": null,
        "lastUpdatedDate": null
    }}
EOD;
    $requestUrl = $this->baseUrl.AppConfig::$f1_communications_create;
    return $this->apiConsumer->postRequest($requestUrl, $requestBody);
}
/*
private function testShowCommunication($commId){
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_communications_show;
    $requestUrl = str_replace("{id}", $commId, $requestUrl);
    $peopleComms = $this->apiConsumer->doRequest($requestUrl);
    if(empty($peopleComms) || is_null($peopleComms))
    $resultCode = false;
    else
    $resultCode = true;
    return $resultCode;
}*/


private function testNewCommunication(){
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_communications_new;
    $rawResponse = $this->apiConsumer->doRequest($requestUrl);
    if(empty($rawResponse) || is_null($rawResponse))
    $resultCode = false;
    else
    $resultCode = true;
    $this->printStatus( 'Communication->New: ',$resultCode);
    return $resultCode;
}

private function testEditCommunication($id, &$rawResponse){
    // $householdID, $id
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_communications_edit;
    $requestUrl = str_replace("{id}", $id, $requestUrl);
    $rawResponse = $this->apiConsumer->doRequest($requestUrl);
    if(empty($rawResponse) || is_null($rawResponse))
    $resultCode = false;
    else
    $resultCode = true;
    $this->printStatus( 'Communication->Edit: ',$resultCode);
    return $resultCode;
}

private function testUpdateCommunication($id){
    $rawResponse = "";
    $resultCode = $this->testEditCommunication($id, $rawResponse);
    if($resultCode) {
        $communication =  $this->apiConsumer->populateCommunication($rawResponse);
        $updatedCommVal = gettimeofday(true)."@gmail.com";
        $communication->communicationValue = $updatedCommVal;

        $rawResponse = $this->apiConsumer->updateCommunication($communication);
        if(empty($rawResponse) || is_null($rawResponse))
        $updateResultCode = false;
        else {
            $communication =  $this->apiConsumer->populateCommunication($rawResponse);
            if($communication->communicationValue == $updatedCommVal)
            $updateResultCode = true;
        }
        $this->printStatus( 'Comunication->Update: ',$updateResultCode);
    }
}

private function testDeleteCommunication($id){
    // Delete it
    // This method will return a 204 - No Content if a successful deletion occurs
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_communications_delete;
    $requestUrl = str_replace("{id}", $id, $requestUrl);
    $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 204 );

    // Then Use Show to fetch it. We should not be able to get to it
    // If delete is called the resource will no longer be available from the API and will return a 410 - Entity is GONE if the resource is ever requested again
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_communications_show;
    $requestUrl = str_replace("{id}", $id, $requestUrl);
    $rawResponse = $this->apiConsumer->doRequest($requestURL, array(), 410 );
}
   /**************************Address/Communications***************************/


//
// CREATE A Person with 1 Address with no Comm values
//
public function testCreatePersonWithAddress($householdId) {
    $statusId = 15609; // New from Facebook
    $householdMemberTypeId = 101; // visitor
    $addressTypeId = 1; //1, Primary 2, Secondary 3, College 4, Vacation 5, Business 6, Org 7, Previous 8, Statement 101, Mail Returned Incorrect
    $person = new Person();
    $person->firstName = 'Test';
    $person->goesByName = "1 Address, no Comm";
    $person->lastName = 'FB'.date( "d/m/Y", time() );
    $person->gender = 'Male';
    $person->householdMemberTypeId = $householdMemberTypeId;
    $person->status = $statusId;
    $person->householdId = $householdId;
    $person->maritalStatus = "Married";
    $address = new Address();
    $address->address1 = "6363 N State Hwy 161";
    $address->address2 = "Suite 200";
    $address->city = "Irving";
    $address->stProvince = "Texas";
    $address->country = "USA";
    $address->county = "Dallas";
    $address->addressTypeID = $addressTypeId;
    $person->addresses[] = $address;
    $created_person = $this->apiConsumer->addPerson($person);
    if(empty($created_person->id) || is_null($created_person->id))
    $resultCode = false;
    else {
        $resultCode = true;
        // We also need to check if the address also got created
        $addresses = $this->apiConsumer->getAddresses($created_person->id);
        if(count($address) == 0) {
            $resultCode = false;
        } else {
            $add = $addresses[0];
            if($add->city != $address->city || $add->stProvince != $address->stProvince || $add->country != $address->country || $add->address1 != $address->address1)
            $resultCode = false;
        }
    }
    $this->printStatus('1 Address, no Comm values addPerson: ', $resultCode);
    print '<pre>'.print_r($created_person, true).'</pre>';
    return $created_person->id;
}

//
// Create a person with no address and 1 Comm value
//
public function testCreatePersonWithCommunication($householdId) {
    $statusId = 15609; // New from Facebook
    $householdMemberTypeId = 101; // visitor
    $communicationTypeId = 1;//1:Home Phone 2:Work Phone 3:Mobile Phone 4:Email 12:Alternate Email 13:Vacation Phone 14:Pager 15:Children Phone 101:Fax 102:Web Address 103:Previous Phone 106:CR Safe Phone 126:School Phone 127:Work Email 128:School Email 129:IM Address 133:Alternate Phone 138:Emergency Phone
    $person = new Person();
    $person->firstName = 'Test';
    $person->goesByName = "No Address, 1 Phone";
    $person->lastName = 'FB'.date( "d/m/Y", time() );
    $person->gender = 'Male';
    $person->householdMemberTypeId = 101;
    $person->status = $statusId;
    $person->householdId = $householdId;
    $person->maritalStatus = "Married";
    $comm = new Communication();
    $comm->communicationTypeID = 1; // Home Phone
    $comm->communicationValue = "1234567890";
    $person->communications[] = $comm;
    $created_person = $this->apiConsumer->addPerson($person);
    if(empty($created_person->id) || is_null($created_person->id))
    $resultCode = false;
    else{
        $resultCode = true;
        $communications = $this->apiConsumer->getCommunications($created_person->id);
        if(count($communications) == 0) {
            $resultCode = false;
        } else {
            $myComm = $communications[0];
            if($myComm->communicationTypeID != $comm->communicationTypeID || $myComm->communicationValue != $comm->communicationValue )
            $resultCode = false;
        }

    }
    $this->printStatus('No Address, 1 Comm addPerson: ', $resultCode);
    print '<pre>'.print_r($created_person, true).'</pre>';
    return $created_person->id;
}

//
// Create a person with address and comm values
//
public function testCreatePersonWithAddressCommunication($householdId) {
    $statusId = 15609; // New from Facebook
    $householdMemberTypeId = 101; // visitor
    $communicationTypeId = 1;//1:Home Phone 2:Work Phone 3:Mobile Phone 4:Email 12:Alternate Email 13:Vacation Phone 14:Pager 15:Children Phone 101:Fax 102:Web Address 103:Previous Phone 106:CR Safe Phone 126:School Phone 127:Work Email 128:School Email 129:IM Address 133:Alternate Phone 138:Emergency Phone
    $person = new Person();
    $person->firstName = 'Test';
    $person->goesByName = "2Address, email";
    $person->lastName = 'FB'.date( "d/m/Y", time() );
    $person->gender = 'Male';
    $person->householdMemberTypeId = 101;
    $person->status = $statusId;
    $person->householdId = $householdId;
    $person->maritalStatus = "Married";

    $address = new Address();
    $address->address1 = "6363 N State Hwy 161";
    $address->address2 = "Suite 200";
    $address->city = "Irving";
    $address->stProvince = "Texas";
    $address->country = "USA";
    $address->county = "Dallas";
    $address->addressTypeID = 5; // Business
    $person->addresses[] = $address;

    $address = new Address();
    $address->address1 = "5200 N State Hwy 161";
    $address->address2 = "Suite 200";
    $address->city = "Irving";
    $address->stProvince = "Texas";
    $address->country = "USA";
    $address->county = "Dallas";
    $address->addressTypeID = 1; // Primary
    $person->addresses[] = $address;

    $phoneComm = new Communication();
    $phoneComm->communicationTypeID = 1; // Home Phone
    $phoneComm->communicationValue = "1234567890";
    $person->communications[] = $phoneComm;


    $emailComm = new Communication();
    $emailComm->communicationTypeID = 4; // Home Phone
    $emailComm->communicationValue = "test@fellowshiptech.com";
    $person->communications[] = $emailComm;

    $created_person = $this->apiConsumer->addPerson($person);
    if(empty($created_person->id) || is_null($created_person->id))
    $resultCode = false;
    else {
        $resultCode = true;
        // Make sure that we have 2 addresses for thie person
        $addresses = $this->apiConsumer->getAddresses($created_person->id);
        if(count($addresses) != 2) {
            $resultCode = false;
        } else {
            $add = $addresses[0];
            if($add->city != $address->city || $add->stProvince != $address->stProvince || $add->country != $address->country || $add->address1 != $address->address1)
            $resultCode = false;
        }
        // Make sure that we have 2 Communication values for this person
        $communications = $this->apiConsumer->getCommunications($created_person->id);
        if(count($communications) != 2) {
            $resultCode = false;
        } else {
            for ($i = 0; $i < count($communications); $i++) {
                $myComm = $communications[$i];

                $add = $addresses[$i];
                switch ($myComm->communicationTypeID) {
                    case 1:
                        if($myComm->communicationValue != $phoneComm->communicationValue)
                        $resultCode = false;
                        break;
                    case 4:
                        if($myComm->communicationValue != $emailComm->communicationValue)
                        $resultCode = false;
                        break;
                }
            }
        }
    }
    $this->printStatus('Address, Comm addPerson: ', $resultCode);
    print '<pre>'.print_r($created_person, true).'</pre>';
    return $created_person->id;
}


//
// Update a person and address and comm values
//
public function testUpdatePersonWithAddressCommunication($personId) {
    $personId = 24643475;
    $person = $this->apiConsumer->getPerson($personId, true);
    $uniqueValue = mktime();
    // updste the person now
    $person->firstName = "TestU";
    $person->lastName = $uniqueValue;
    $person->maritalStatus = "Married";
    // since we dont want to update the addresses and comm values, set the entity state to unchanged
    if(count($person->addresses) > 0) {
        $addresses = $person->addresses;
        for($i=0; $i<count($addresses); $i++) {
            $address = $addresses[$i];
            // update the address
            $address->address1 = $uniqueValue;
        }
    }
    // Communications
    if(count($person->communications) > 0) {
        $communications = $person->communications;
        for($i=0; $i<count($communications); $i++) {
            $communication = $communications[$i];
            $communication->communicationValue = $uniqueValue.'@ft.com';
        }
    }
    $updated_person = $this->apiConsumer->updatePerson($person);
    if(empty($updated_person->id) || is_null($updated_person->id))
    $resultCode = false;
    else {
        $resultCode = true;
        if($updated_person->lastName != $uniqueValue)
        $resultCode = false;
        // Make sure the addresses got updated
        $addresses = $this->apiConsumer->getAddresses($updated_person->id);
        for ($i = 0; $i < count($addresses); $i++) {
            if($addresses[$i]->address1 != $uniqueValue)
            $resultCode = false;
        }
        // Make sure the communications got updated
        $communications = $this->apiConsumer->getCommunications($updated_person->id);
        for ($j = 0; $j < count($communications); $j++) {
            if($communications[$j]->communicationValue != $uniqueValue.'@ft.com')
            $resultCode = false;
        }

    }
    $this->printStatus('updatePerson: ', $resultCode);
    print '<pre>'.print_r($updated_person, true).'</pre>';
    return $resultCode;
}


       

//
// Get Household Members
//
public function testGetHouseholdMembers($householdId) {
    $requestUrl = $this->apiConsumer->getBaseUrl().AppConfig::$f1_household_people;
    $requestUrl = str_replace("{householdID}", $household_id, $requestUrl);
    $result = $this->apiConsumer->doRequest($requestUrl);
    if(empty($result) || is_null($result))
    $resultCode = false;
    else
    $resultCode = true;
    $this->printStatus('getHouseholdMembers', $resultCode);

    $json_array = json_decode(strstr($result, '{"people":{'), true);

    // Results found?
    if (isset($json_array['people']['person'])) {
        $json_length = count($json_array['people']['person']);

    }

    for ($i = 0; $i < $json_length; $i++) {
        $x = $json_array['people']['person'][$i];

        $first_name = $x['firstName'];
        $last_name = $x['lastName'];
        $marital = $x['maritalStatus'];
        $gender = $x['gender'];
        $birthdate = $x['dateOfBirth'];

        $h = '';
        $h .=	$first_name.', ';
        $h .=	$last_name.', ';
        $h .=	$marital.', ';
        $h .=	$gender.', ';
        $h .=	$birthdate;

        print $h;
    }
    return $resultCode;
}

private function printStatus($message, $resultCode){
    $style = "style = \"color:red\"";
    $status = "FAILED";
    if($resultCode == true) {
        $style = "style = \"color:green\"";
        $status = "OK";
    }
    print "<div $style>$message: $status</div>";
}

    /*
     Households

testSearchHouseholds
testShowHousehold
testEditHousehold
testNewHousehold
testCreateHousehold
testUpdateHousehold


    * Households: Search | Show | Edit | New | Create | Update
    * Household Member Types: List | Show
testListHouseholdMemberTypes
testShowHouseholdMemberTypes

People

    * People: Search | List | Show | Edit | New | Create | Update
testSearchPeople
testListPeople
testShowPeople
testEditPeople
testNewPeople
testCreatePeople
testUpdatePeople


    * People Attributes: List | Show | Edit | New | Create | Update | Delete
testListPeopleAttributes
testShowPeopleAttributes
testEditPeopleAttributes
testNewPeopleAttributes
testCreatePeopleAttributes
testUpdatePeopleAttributes
testDeletePeopleAttributes

    * People Images: Show
testShowImage

Addresses

    * Addresses: List | Show | Edit | New | Create | Update | Delete
testListAddresses
testShowAddresses
testEditAddresses
testNewAddresses
testCreateAddresses
testUpdateAddresses
testDeleteAddresses
    * Address Types: List | Show
testListAddressTypes
testShowAddressTypes

Attributes

    * Attribute Groups: List | Show
testListAttributes
testShowAttributes

    * Attributes: List | Show

Communications

    * Communications: List | Show | Edit | New | Create | Update | Delete
testListCommunications
testShowCommunications
testEditCommunications
testNewCommunications
testCreateCommunications
testUpdateCommunications
testDeleteCommunications

    * Communication Types: List | Show
testListCommunicationTypes
testShowCommunicationTypes

Denominations

    * Denominations: List | Show
testListDenominations
testShowDenominations

Occupations

    * Occupations: List | Show
testListOccupations
testShowOccupations

Schools

    * Schools: List | Show
testListSchools
testShowSchools

Statuses

    * Statuses: List | Show
testListStatuses
testShowStatuses

    * Sub Statuses: List | Show
testListSubStatuses
testShowSubStatuses
     */
}
?>
