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
		
		# giving -> enter contributions -> specify a contribution
		self.giving_enter_contribution("29378", "100", "Load")
		
		# store the custom timer
		latency = time.time() - start_time
		self.custom_timers['Giving -> Enter Contribution'] = latency
	
