import urllib2
import httplib
import time


class Transaction(object):
    def __init__(self):
        self.custom_timers = {}
    
    def run(self):
        post_body=urllib.urlencode({'username': 'cgutekunst@fellowshiptech.com','password': '@FTAdmin0'})
        headers = {'Content-type': 'application/x-www-form-urlencoded'}            
        
        start_timer = time.time()
        conn = httplib.HTTPConnection('https://qaeunlx0c1.staging.infellowship.com')
        conn.request('POST', '/UserLogin/Attempt', post_body, headers)
        resp = conn.getresponse()
        content = resp.read()
        latency = time.time() - start_timer
        
        self.custom_timers['LOGIN'] = latency
        
        print resp.status