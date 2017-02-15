import time
import urllib
import urllib2
import testbase

class Portal(testbase.TestBase):
	"""Contains all the methods for load testing Portal pages"""
	def __init__(self):
		super(Portal, self).__init__()
		self.custom_timers = {}
		self.username = "msneeden"
		self.password = "Pa$$w0rd"
		self.church_code = "dc"
		# self.portal_url = "http://portalqa.dev.corp.local"
		self.portal_url = "https://staging-www.fellowshipone.com"
	
	def open_login_page(self):
		
		# start the timer
		start_timer = time.time()
		
		# read the response, store it in a variable
		response = self.browser.open(self.portal_url)
				
		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Load_Login_Page'] = latency
		
		# Print the page's title to console output
		print(self.browser.title())
		
		# assert the page loaded
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
	
	def login_portal(self):
		"""Logs into Portal"""
		self.open_login_page()
				
		# select the first form on the page
		self.browser.select_form(nr=0)
		
		# set form fields
		self.browser.form['ctl00$content$userNameText'] = self.username
		self.browser.form['ctl00$content$passwordText'] = self.password
		self.browser.form['ctl00$content$churchCodeText'] = self.church_code
		# start the timer
		start_timer = time.time()
		
		# submit the form
		loginResponse = self.browser.submit()
		
		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Login'] = latency
		
		# verify responses are valid
		assert (loginResponse.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(loginResponse.code)
		assert ('My Tasks' in loginResponse.get_data()), 'Text Assertion Failed'
	
	def open_groups_view_all(self):
		"""Views the Group's View All Page"""
		response = self.browser.follow_link(text="View All") 
		print self.browser.title()
		
		# verify responses are valid
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
	
	def open_groups_search_category(self, search_category_name):
		"""View's a search category"""
		# follow the search categories link
		response = self.browser.follow_link(text="Search Categories")
		
		# print the title
		print(self.browser.title())
		
		# find the link that has the value of the text attribute equal to the name of the search category.
		link = self.browser.find_link(text=search_category_name)
		# store link's base url in a variable, replace the Index.aspx path with the url of the link found above. This is necessary 
		# because it includes the session paramenters in the base url.  Once this string is built, open it.
		updated = link.base_url
		updated = updated.replace("/Groups/GroupCategories/Index.aspx", link.url)
		self.browser.open(updated)
		
		# print the title
		print(self.browser.title())
		
		# verify responses are valid
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
	
	def open_groups_span_of_care(self, span_of_care_name):
		"""Views a Span of Care"""
		response = self.browser.follow_link(text="Span of Care") 
		
		# print the title
		print(self.browser.title())
		
		# find the link that has the value of the text attribute equal to the name of the span of care.
		link = self.browser.find_link(text=span_of_care_name)
		# store link's base url in a variable, replace the Index.aspx path with the url of the link found above. This is necessary 
		# because it includes the session paramenters in the base url.  Once this string is built, open it.
		updated = link.base_url
		updated = updated.replace("/Groups/GroupSoc/Index.aspx", link.url)
		self.browser.open(updated)
		
		# print the title
		print(self.browser.title())
		
		# verify responses are valid
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
	
	def giving_search_contributions(self):
		# follow the contributions search link
		response = self.browser.follow_link(url_regex="/giving/contributionsearch.aspx")
		
		# print the title
		print(self.browser.title())
		
		# verify responses are valid
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
		
		# search
		self.browser.select_form(nr=1)
		self.browser.submit()
	
	def giving_enter_contribution(self, fund, amount, reference):
		"""Enters a contribution."""
		# follow the enter contributions link
		response = self.browser.follow_link(text="Enter Contributions")
		
		# print the title
		print(self.browser.title())
		
		# verify responses are valid
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
		
		# select the second form
		self.browser.select_form(nr=1)
		
		# enter the contribution
		self.browser.form['ctl00$ctl00$MainContent$content$ddlFund$dropDownList'] = [fund]
		self.browser.form['ctl00$ctl00$MainContent$content$ctlCheck$txtAmount$textBox'] = amount
		self.browser.form['ctl00$ctl00$MainContent$content$ctlCheck$txtReference$textBox'] = reference
		
		# submit
		response = self.browser.submit(name='ctl00$ctl00$MainContent$content$btnSave')
		
		# verify responses are valid
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
	
