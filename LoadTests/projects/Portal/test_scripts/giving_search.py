import sys
sys.path.append('projects/common')
import portal
import time

class Transaction(portal.Portal):
	def __init__(self):
		super(Transaction, self).__init__()
		self.custom_timers = {}
	
	def run(self):
		# login to portal
		self.login_portal()
		
		# start the timer
		start_time = time.time()
		
		# giving -> search
		self.giving_search_contributions()
		
		# store the custom timer
		latency = time.time() - start_time
		self.custom_timers['Giving -> Search'] = latency
	
