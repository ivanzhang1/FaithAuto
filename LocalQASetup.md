Prereqs – Should be in f1 dev setup i.e. chef and related docs.
  Local database
  Gallio Icarus Test Runner
  Visual Studio
  Java

One Time Setup

Install / Update Firefox
Download tests repo to c:\dev\tests

Restore from QA Backups at \\ftdata\Development\SQLFiles\QADatabase Backups
Rerun local update all script in database repo while on desired branch
Perform a SQL Data Compare to pull data from ChmApi

Add the following to your hosts file (Mine is at C:\Windows\System32\drivers\etc\hosts)
#API
127.0.0.1              global.fellowshiponeapi.local
127.0.0.1              demo.fellowshiponeapi.local
127.0.0.1              dc.fellowshiponeapi.local
127.0.0.1              dev.fellowshiponeapi.local
127.0.0.1              template.fellowshiponeapi.local
127.0.0.1   qaeunlx0c1.fellowshiponeapi.local
127.0.0.1              qaeunlx0c2.fellowshiponeapi.local
127.0.0.1   qaeunlx0c3.fellowshiponeapi.local
127.0.0.1   qaeunlx0c4.fellowshiponeapi.local
127.0.0.1   qaeunlx0c6.fellowshiponeapi.local
127.0.0.1   ftapi.fellowshiponeapi.local
127.0.0.1   dcdmdfwtx.fellowshiponeapi.local
127.0.0.1   heartlandcc.fellowshiponeapi.local
127.0.0.1   fbccdfwtx.fellowshiponeapi.local

#Infellowship
127.0.0.1              global.infellowship.local
127.0.0.1              demo.infellowship.local
127.0.0.1              dc.infellowship.local
127.0.0.1              dev.infellowship.local
127.0.0.1              template.infellowship.local
127.0.0.1   qaeunlx0c1.infellowship.local
127.0.0.1              qaeunlx0c2.infellowship.local
127.0.0.1   qaeunlx0c3.infellowship.local
127.0.0.1   qaeunlx0c4.infellowship.local
127.0.0.1   qaeunlx0c6.infellowship.local
127.0.0.1   ftapi.infellowship.local
127.0.0.1   dcdmdfwtx.infellowship.local
127.0.0.1   heartlandcc.infellowship.local
127.0.0.1   fbccdfwtx.infellowship.local

TODO
  Move and test the list of church codes into the dev-chef scripts
  C:\dev-chef\cookbooks\dev\attributes\default.rb
  set_unless[:dev][:church_codes] = [ "global", "dc", "demo", "dev", "template", "qaeunlx0c1", "qaeunlx0c2", "qaeunlx0c3", "qaeunlx0c4", "qaeunlx0c5", "qaeunlx0c6", "ftapi", "dcdmdfwtx", "heartlandcc", "fbccdfwtx" ]
  TODO: Get the dev-chef scripts into source

Not sure if needed
  Update Gallio to 3.4 (Seems to break debugging for me).
  Disable unneeded firefox plugins

Each time you change your test repo contents

Built FTTests.sln

Change values for following entries in c:\dev\tests\Common\bin\Debug\Common.dll.config
  <add key="FTTests.Host" value="localhost" />
  <add key="FTTests.Environment" value="LOCAL" />

To Run Tests After Setup

Open Gallio Icarus Test Runner
  Add Files -> c:\dev\tests\(Solution under Test)\bin\Debug\(Solution under test).dll
  (We used Portal for testing)
  Check desired tests
  Start or Debug
