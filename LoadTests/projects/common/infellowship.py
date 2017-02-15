import mechanize
import time
import urllib
import urllib2
import testbase

class InFellowship(testbase.TestBase):
	"""Contains all the methods for load testing InFellowship pages"""
	def __init__(self):
		super(InFellowship, self).__init__()
		self.custom_timers = {}
		self.username = "msneeden@fellowshiptech.com"
		self.password = "Pa$$w0rd"
		# self.infellowship_url = "http://dc.infellowshipqa.dev.corp.local"
		self.infellowship_url = "https://dc.staging.infellowship.com"
	
	def open_login_page(self):
				
		# start the timer
		start_timer = time.time()
		
		# read the response, store it in a variable
		response = self.browser.open(self.infellowship_url)
		
		# stop the timer
		latency = time.time() - start_timer
		self.custom_timers['Load_Login_Page'] = latency
		
		# Print the page's title to console output
		print(self.browser.title())
		
		# assert the page loaded
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
	
	def login_infellowship(self):
		"""Logs into InFellowship"""
		self.open_login_page()
		
		# select the form
		self.browser.select_form(nr=1)
		
		# set form fields
		self.browser.form['username'] = self.username
		self.browser.form['password'] = self.password
		
		# start the timer and submit the form
		start_timer = time.time()
		loginResponse = self.browser.submit()
		
		# stop and store the custom timer
		latency = time.time() - start_timer
		self.custom_timers['Login'] = latency
		
		# assert the page loaded
		assert (loginResponse.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(loginResponse.code)
	
	def view_group(self,  group_name):
		self.browser.follow_link(text="Your groups")
		
		# print the title
		print(self.browser.title())
		
		# assert the page loaded
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
		
		# view the group
		response = self.browser.follow_link(text=group_name)
		
		# assert the page loaded
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
	
	def view_soc_groups(self, span_of_care_name):
		self.browser.follow_link(text="Your groups")
		
		# print the title
		print(self.browser.title())
		
		# assert the page loaded
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
		
		# view the group
		response = self.browser.follow_link(text=span_of_care_name)
		
		# assert the page loaded
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
