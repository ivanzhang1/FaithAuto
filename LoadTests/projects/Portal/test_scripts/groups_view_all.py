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
		
		# groups -> view all 
		self.open_groups_view_all()
		
		# store the custom timer
		latency = time.time() - start_time
		self.custom_timers['Groups -> View All'] = latency
	
