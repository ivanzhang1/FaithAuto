import sys
import time
import mechanize

class Transaction(object):
	def __init__(self):
		self.custom_timers = {}

	def run(self):
		# login to portal
		browser = mechanize.Browser()
		browser.set_handle_robots(False)
		browser.addheaders = [('User-agent', 'Mozilla/5.0 Compatible')]
		
		# start the timer
		start_timer = time.time()
		
		# read the response, store it in a variable
		from urllib2 import HTTPError
		try:
			response = browser.open("https://portal.fellowshiptech.org/login.aspx")
		except HTTPError, e:
			print "Got error code", e.code
			pass
		
				
		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Load_Login_Page'] = latency


		# login
		# select the first form on the page
		browser.select_form(nr=0)
		
		# set form fields
		browser.form['ctl00$content$userNameText'] = "msneeden"
		browser.form['ctl00$content$passwordText'] = "Pa$$w0rd"
		browser.form['ctl00$content$churchCodeText'] = "DC"
		# start the timer
		start_timer = time.time()
		
		# submit the form
		loginResponse = browser.submit()
		
		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Login'] = latency

		print(browser.title())

		
		# start the timer
		start_time = time.time()
		
		# view an individual
		link = browser.find_link(text="Matthew Sneeden")
		
		# store link's base url in a variable, replace the Index.aspx path with the url of the link found above. This is necessary 
		# because it includes the session paramenters in the base url.  Once this string is built, open it.
		updated = link.base_url
		updated = updated.replace("/home.aspx", "/Groups/Group/ListAll.aspx")
		resp = browser.open(updated)

		#/InFellowship/Configuration/Features.aspx
		#/InFellowship/Branding/Index.aspx


		# store the custom timer
		latency = time.time() - start_time
		self.custom_timers['Groups View All'] = latency

		print(browser.title())
		print resp.geturl()
		#assert not containsAny('GeneralErrorPage.aspx', resp.geturl())