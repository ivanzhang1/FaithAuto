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
		response = browser.open("https://dc.staging.infellowship.com")

		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Load_Login_Page'] = latency

		# select the form
		browser.select_form(nr=1)
		
		# set form fields
		browser.form['username'] = "msneeden@fellowshiptech.com"
		browser.form['password'] = "Pa$$w0rd"
		
		# submit the form
		start_timer = time.time()
		from urllib2 import HTTPError
		try:
			browser.submit()
		except HTTPError, e:
			print "Got error code", e.code
			pass
		
		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Login'] = latency

		print(browser.title())