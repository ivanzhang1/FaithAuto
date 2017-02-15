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
		
		# groups -> administration -> search category
		self.open_groups_search_category("Load Testing SC")
		
		# store the custom timer
		latency = time.time() - start_time
		self.custom_timers['Groups -> View Search Category'] = latency
	
