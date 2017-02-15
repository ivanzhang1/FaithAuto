import mechanize
import time
import urllib
import urllib2
import testbase

class Weblink(testbase.TestBase):
	"""Contains all the methods for load testing Weblink pages"""
	def __init__(self):
		super(Weblink, self).__init__()
		self.username = "msneeden@fellowshiptech.com"
		self.password = "Pa$$w0rd"
		self.iCode = "&iCode=Ej5+mGgQ4Fq9hVd1sm0cHg%3d%3d"
		self.online_giving_url = "http://integrationdev.dev.corp.local/integration/contribution/onlinecontribution.aspx?cCode=alyq5rXA9igtXilwXAT+3Q=="
		# self.online_giving_url = "http://integrationqa.dev.corp.local/integration/contribution/onlinecontribution.aspx?cCode=alyq5rXA9igtXilwXAT+3Q=="
	
	def open_online_giving(self, username, password):
		"""Opens Online Giving in Weblink"""
		# start the timer
		start_timer = time.time()
		
		# read the response, store it in a variable
		urllib2.response = self.browser.open(self.onlne_giving_url)
		resp = urllib2.response
		resp.read()
		
		# If redirect occurs, call login page
		if resp.code == 302:
			self.login_weblink()
		
		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Load_Online_Giving_Page'] = latency
		
		# Print the page's title to console output
		print(self.browser.title())
		
		# assert the page loaded
		assert (resp.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(resp.code)
		assert ('Sign in' in resp.get_data()), 'Text Assertion Failed'
		
		# wait
		time.time(2)
		
		# select the first form on the page, populate fields and submit
		self.browser.select_form(nr=0)
		self.browser.form['txtUserName'] = self.username
		self.browser.form['txtPassword'] = self.password
		loginResponse = self.browser.submit()
	
	def add_online_contribution_immediate(self):
		"""Generates an immediate online contribution"""
		self.open_online_giving()
		
		# set the URL
		url = "http://integrationqa.dev.corp.local/integration/contribution/onlinecontribution.aspx?cCode=alyq5rXA9igtXilwXAT+3Q==" + self.iCode
		
		# select first form on the page
		browser.select_form(nr=0)
		
		# set the values for the immediate contribution
		values = {'__EVENTTARGET' : 'btnNew',
			'__EVENTARGUMENT' : '',
			'__LASTFOCUS' : '',
			'__VIEWSTATE' : browser.form['__VIEWSTATE'],
			'dtbMonthYearStartDate_PU_PN_Month' : '8',
			'dtbMonthYearStartDate_PU_PN_Year' : '2010',
			'_ctl1:DateTextBox_PU_PN_MYP_PN_Month' : '8',
			'_ctl1:DateTextBox_PU_PN_MYP_PN_Year' : '2010',
			'_ctl1:DateTextBox_PU_PN_SelDate' : '',
			'_ctl1:DateTextBox_PU_PN_MonthView' : '2010|8',
			'__EVENTVALIDATION' : browser.form['__EVENTVALIDATION'],
			'txtAmount:textBox' : '1.04',
			'ddlFrequency' : '1',
			'FundRadioButton' : 'rdbtnFund',
			'ddlFund:dropDownList' : '29378',
			'_ctl1:DateTextBox' : '',
			'dtbMonthYearStartDate' : '',
			'rblLength' : '1',
			'txtNumberOfGifts' : '',
			'_ctl3:DateTextBox' : '',
			'oneTime' : 'rbImmediate',
			'ddlPaymentMethod:dropDownList' : '1',
			'txtBankName:textBox' : '',
			'txtBankRoutingNumber:textBox' : '',
			'txtBankAccountNumber:textBox' : '',
			'txtReenterAccountNumber:textBox' : '',
			'txtHoldersName:textBox' : 'Matthew Sneeden',
			'txtCardNo:textBox' : '4111111111111111',
			'dtCCStartDate' : '',
			'mytExpirationDate' : '8/2011',
			'txtIssueNumber:textBox' : '',
			'txtcvcNumber:textBox' : '',
			'ctlAddress:ddlCountry:dropDownList' : 'US',
			'ctlAddress:txtAddress1:textBox' : '9616 Armour Dr',
			'ctlAddress:txtAddress2:textBox' : '',
			'ctlAddress:txtAddress3:textBox' : '',
			'ctlAddress:txtCity:textBox' : 'Fort Worth',
			'ctlAddress:txtState:textBox' : '',
			'ctlAddress:ddlState:dropDownList' : 'TX',
			'ctlAddress:txtPostalCode:textBox' : '76244-6085',
			'ctlAddress:txtPhoneNumber:textBox' : '',
			'txtBankAgree:textBox' : '',
			'ddlHistoryYear' : '2010',
			'hid1' : '',
			'hid2' : '2010',
			'hid3' : ''}
		
		# encode the data and create the request
		data = urllib.urlencode(values)
		request = urllib2.request(url, data)
		
		# make the request and read the response
		response = urllib2.urlopen(request)
		
		# assert the page loaded
		assert (resp.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(resp.code)		
	
	def login_weblink(self):
		"""Logs into Weblink"""
		self.browser.select_form(nr=0)
		self.browser.form['txtUserName'] = self.username
		self.browser.form['txtPassword'] = self.password
		
		# start the timer and submit the form
		start_time = time.time()
		loginResponse = self.browser.submit()
		
		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Login_Weblink'] = latency
		
		# assert the page loaded
		assert (resp.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(resp.code)
	
