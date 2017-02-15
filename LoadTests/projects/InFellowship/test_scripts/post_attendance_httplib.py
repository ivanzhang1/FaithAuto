import urllib2
import time


class Transaction(object):
    def __init__(self):
        self.custom_timers = {}
    
    def run(self):
        start_timer = time.time()
        resp = urllib2.urlopen('https://qaeunlx0c1.staging.infellowship.com/UserLogin/Index')
        content = resp.read()
        #print(content)
        latency = time.time() - start_timer
        
        self.custom_timers['Example_Homepage'] = latency
        
        assert (resp.code == 200), 'Bad HTTP Response'
        assert ('Fellowship One User Groups' in content), 'Failed Content Verification'

        values = {'username' : 'cgutekunst@fellowshiptech.com',
			'password' : '@FTAdmin0',
			'rememberme' : 'false',
			'btn_login' : 'Sign in'}
			
        data = urllib.urlencode(values)
        request = urllib2.request('/UserLogin/Attempt', data)
        response = urllib2.urlopen(request)

        content2 = response.read()
        print(content2)