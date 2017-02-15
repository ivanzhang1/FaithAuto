import sys
sys.path.append('projects/common')
import weblink
import time

class Transaction(weblink.Weblink):
	
	def __init__(self):
		super(Transaction, self).__init__()
		self.custom_timers = {}
	
	def run(self):
		# start the timer
		start_time = time.time()
		
		# add immediate contribution
		self.add_online_contribution_immediate()
		
		# store the custom timer
		latency = time.time() - start_time
		self.custom_timers['Weblink -> Online Giving -> Immediate Contribution'] = latency
	
