#!/usr/bin/env python
# encoding: utf-8
"""
common.py

Created by Bryan Mikaelian
Copyright (c) 2010 Fellowship Technologies. All rights reserved.
"""
import mechanize
import sys
#import yaml

class TestBase(object):
	"""Provides the basic configuration for browser configuration"""
	def __init__(self):
		self.browser = mechanize.Browser()
		self.browser.set_handle_robots(False)
		self.browser.addheaders = [('User-agent', 'Mozilla/5.0 Compatible')]
		self.browser.name = "Bill"
