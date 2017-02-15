import sys
#sys.path.append('projects/common')
#import adhoc
import time
import mechanize


class Transaction(object):
	def __init__(self):
		self.custom_timers = {}

	def run(self):
		browser = mechanize.Browser()
		browser.set_handle_robots(False)
		browser.addheaders = [('User-agent', 'Mozilla/5.0 Compatible')]
		
		# start the timer
		start_timer = time.time()
		
		# read the response, store it in a variable
		response = browser.open("https://adhoc.fellowshipone.com")
				
		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Load_Login_Page'] = latency
		
		# Print the page's title to console output
		print(browser.title())
		
		# assert the page loaded
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
		
		
		# select the first form on the page
		browser.select_form(nr=0)
		
		# set form fields
		browser.form['username'] = "msneeden"
		browser.form['password'] = "Pa$$w0rd"
		browser.form['churchcode'] = "dc"
		
		# start the timer
		start_timer = time.time()
		
		# submit the form
		loginResponse = browser.submit()
		#print loginResponse.read()
		
		print browser.title()
		# calculate the time and store it in a custom timer.
		latency = time.time() - start_timer
		self.custom_timers['Login'] = latency
		
		# verify responses are valid
		assert (loginResponse.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(loginResponse.code)
		assert ('Reports List' in loginResponse.get_data()), 'Text Assertion Failed'
		
		#print "debug!!"
		# View the report
		#link = browser.follow_link(text="LK_Contribution_Globe")
		link = browser.find_link(text="Giving_person")
		#link = browser.find_link(url="/ReportViewer.aspx?rn=Testing_giving%5cGiving_person")
		
		# store link's base url in a variable, replace the Index.aspx path with the url of the link found above. This is necessary 
		# because it includes the session paramenters in the base url.  Once this string is built, open it.
		updated = link.base_url
		updated = updated.replace("/Report/List", link.url)
		resp = browser.open(updated)
		print "Debug!!"
		
		#resp.read()
		#print resp.get_data()
		#print browser.title()
		
		# verify responses are valid
		assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)



		#link2 = browser.find_link(url_regex="javascript:responseServer.OpenUrl('rs.aspx?output=DOC', 'aspnetForm', 'reportFrame');")
		link2 = browser.find_link(nr=14)
		
		updated2 = link2.base_url
		updated2 = updated2.replace("/ReportViewer.aspx?rn=Testing_giving%5cGiving_person", "/rs.aspx?output=XLS(MIME)")
		print updated2
		resp2 = browser.open(updated2)


		
		
		
		
		
		
		
		
		
		
		# login to adhoc
		#self.login_adhoc()
		
		# select the first form on the page
		#self.browser.select_form(nr=0)
		
		# set form fields
		#self.browser.form['username'] = self.username
		#self.browser.form['password'] = self.password
		#self.browser.form['churchcode'] = self.church_code
		# start the timer
		#start_timer = time.time()
		
		# submit the form
		#loginResponse = self.browser.submit()
		
		# calculate the time and store it in a custom timer.
		#latency = time.time() - start_timer
		#self.custom_timers['Login'] = latency
		
		# verify responses are valid
		#assert (loginResponse.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(loginResponse.code)
		#assert ('Reports List' in loginResponse.get_data()), 'Text Assertion Failed'
		
		# start the timer
		#start_time = time.time()
		
		# view a report
		#self.open_report("Giving_person")
		
		#response = self.browser.follow_link(text="Giving_person") 
		#print self.browser.title()
		
		# verify responses are valid
		#assert (response.code == 200), "Bad HTTP Response. Expecting a 200.  Received a " + str(response.code)
		
		# store the custom timer
		#latency = time.time() - start_time
		#self.custom_timers['View report'] = latency
		
		# start the timer
		#report_load_timer = time.time()
		
		#print self.browser.title()
		
		# click to load the word document
		#for link in self.browser.links():
			#print link
		
		#link = self.browser.find_link(url="javascript:responseServer.OpenUrl('rs.aspx?output=DOC', 'aspnetForm', 'reportFrame');")
		#self.browser.open(link)
		
		#print "here"
		#print self.browser.title()
		
		#latency2 = time.time() - report_load_timer
		#self.custom_timers['Gen report'] = latency

	if __name__ == '__main__':
		trans = Transaction()
		trans.run()
		print trans.custom_timers  
