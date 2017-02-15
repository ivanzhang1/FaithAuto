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
		
		print('Start test thread...')
		
		base_url = "http://portal.local"
        
		# read the response, store it in a variable
		from urllib2 import HTTPError
		try:
			response = browser.open(base_url + "/login.aspx")
		except HTTPError, e:
			print "Got error code", e.code
			pass
		
		# select the first form on the page
		browser.select_form(nr=0)
		
		# fill out login form
		browser.form['ctl00$content$userNameText'] = "tcoulson"
		browser.form['ctl00$content$passwordText'] = "FT.Admin1"
		browser.form['ctl00$content$churchCodeText'] = "DC"
		
		# login
		loginResponse = browser.submit()
		
		link = browser.find_link(text="Tara Coulson")
		
		view_all_link = link.base_url.replace("/home.aspx", "/Groups/Group/ListAll.aspx")
		groups_link = link.base_url.replace("/home.aspx", "/Groups/Group/ListAll.aspx?search_context=1")
		people_list_link = link.base_url.replace("/home.aspx", "/Groups/Group/ListAll.aspx?search_context=2")
		temporary_groups_link = link.base_url.replace("/home.aspx", "/Groups/Group/ListAll.aspx?search_context=3")
		soc_link = link.base_url.replace("/home.aspx", "/Groups/Group/ListAll.aspx?search_context=4")
		groups_search_link = link.base_url.replace("/home.aspx", "/Groups/Group/SearchList.aspx?search_context=0&group_name=bk&individual_name=&start_date_from=&start_date_to=&age_range_min=-1&age_range_max=-1&commit=Search")
		
		start_time = time.time()
		
		for i in range (1, 5):
			#go to view all page
			resp = browser.open(view_all_link)
			
			print resp.geturl()
			
			#go to groups page
			resp = browser.open(groups_link)
			
			print resp.geturl()
			
			#do a groups search by name
			resp = browser.open(groups_search_link)
			
			print resp.geturl()
			
			#go to people list page
			resp = browser.open(people_list_link)
			
			print resp.geturl()
			
			#go back to view all page
			resp = browser.open(view_all_link)
			
			print resp.geturl()
			
			#go to temporary groups page
			resp = browser.open(temporary_groups_link)
			
			print resp.geturl()
			
			#go back to view all page
			resp = browser.open(view_all_link)
			
			print resp.geturl()
			
			#go to soc page
			resp = browser.open(soc_link)
			
			print resp.geturl()
		
		latency = time.time() - start_time
		self.custom_timers['Mongo Pages'] = latency
