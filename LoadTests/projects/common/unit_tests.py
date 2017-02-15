import unittest
import sys
import portal
import infellowship
import weblink

class Tests(unittest.TestCase):
	
	def setUp(self):
		self.p = portal.Portal()
		self.inf = infellowship.InFellowship()
		self.wl = weblink.Weblink()
	
	# Initialization tests
	def test_portal_object_not_null(self):
		self.assertNotEqual(None, self.p, "Portal object was null")
	
	def test_infellowship_object_not_null(self):
		self.assertNotEqual(None, self.inf, "InFellowship object was null")
	
	def test_weblink_object_not_null(self):
		self.assertNotEqual(None, self.wl, "Weblink object was null")
		
	def test_portal_browser_not_null(self):
		self.assertNotEqual(None, self.p.browser, "Portal browser object was null")
	
	def test_infellowship_browser_not_null(self):
		self.assertNotEqual(None, self.inf.browser, "InFellowship browser object was null")
	
	def test_weblink_browser_not_null(self):
		self.assertNotEqual(None, self.wl.browser, "Weblink browser object was null")
	
	# Config tests
	def test_potal_username_not_null(self):
		self.assertNotEqual(None, self.p.username, "Portal username was null")
	
	def test_portal_password_not_null(self):
		self.assertNotEqual(None, self.p.password, "Portal password was null")
	
	def test_portal_church_code_not_null(self):
		self.assertNotEqual(None, self.p.church_code, "Portal church code was null")
	
	def test_infellowship_username_not_null(self):
		self.assertNotEqual(None, self.inf.username, "InFellowship username was null")
	
	def test_infellowship_password_not_null(self):
		self.assertNotEqual(None, self.inf.password, "InFellowship password was null")
	
	def test_weblink_username_not_null(self):
		self.assertNotEqual(None, self.wl.username, "Weblink username was null")
	
	def test_weblink_password_not_null(self):
		self.assertNotEqual(None, self.wl.password, "Weblink password was null")


# runs the unit tests
suite = unittest.TestLoader().loadTestsFromTestCase(Tests)
unittest.TextTestRunner(verbosity=2).run(suite)