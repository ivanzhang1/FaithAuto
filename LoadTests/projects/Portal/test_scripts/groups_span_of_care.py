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
		
		# groups -> administration -> span of care
		self.open_groups_span_of_care("Load Testing SoC")
		
		# store the custom timer
		latency = time.time() - start_time
		self.custom_timers['Groups -> View Span of Care'] = latency
	
