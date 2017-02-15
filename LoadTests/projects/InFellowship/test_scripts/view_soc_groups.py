import sys
sys.path.append('projects/common')
import infellowship
import time

class Transaction(infellowship.InFellowship):
	def __init__(self):
		super(Transaction, self).__init__()
		self.custom_timers = {}
	
	def run(self):
		# login to infellowship
		self.login_infellowship()
		
		# start the timer
		start_time = time.time()
		
		# view a span of care's groups
		self.view_span_of_care_group("Load Test SOC")
		
		# store the custom timer
		latency = time.time() - start_time
		self.custom_timers['View Span of Care\'s Groups'] = latency
	
