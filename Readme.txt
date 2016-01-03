Hosts Manager v1.0

Explanation --------------------------------------------------------------------

This is a simple app that sits in your system tray and lets you create and
manage different hosts files. If you don't know why you'd want this, you 
probably don't need it. If you want to know why you'd want it, read this:

http://16bit.orderinchaos.org/2009/10/26/hosts-files/

Important Stuff ----------------------------------------------------------------

If you're going to use this program, you probably don't want to edit your hosts
file directly, unless you're making a one-time temporary change.

If you want this to start at startup, stick a shortcut to the executable in your
Start Menu's startup folder.

Less Important Stuff -----------------------------------------------------------

This program aims to be lightweight and straightforward in a "do one thing and
do it well" fashion. Hopefully, this means that I don't have to explain the user
interface. However, it does some interesting stuff under the hood that you may
or may not care about:

-When you start it for the first time, it copies your current hosts file into
  Default.hosts. This file and all other alternative hosts files live in the
  same directory as the executable.
  
-When you switch to a different hosts file though the manager, the command
   ipconfig /flushdns 
  is run. A command line window may flash into view when this happens. This
  command is not run automatically if you edit the system's hosts file directly.
  
-If you create a new hosts file in the program folder without going through the
  manager, it will still be automatically picked up and added to the hosts menu.
  
-When you create or edit a file through the manager, it will try to open the
  file with an external program. If you don't have .hosts files associated with
  a text editor, you will be prompted to do so.
  
-If the .hosts file that's being used is changed, those changes will also be
  made to the hosts file. However, it doesn't work the other way around; if you
  edit the hosts file directly, the changes won't be made to the .hosts file.
  This is by design, in case you want to make temporary changes. If you do, be
  warned that you'll have to flush the DNS yourself, and your changes will be
  overwritten if you switch to a different file in the manager.

Requirements -------------------------------------------------------------------

Windows
.NET Framework 3.5

Contact Info -------------------------------------------------------------------

If this program does something that it shouldn't or doesn't do something that it
should, fire me an email or something at lippertz@users.sourceforge.net

Of course, source code and the most recent installer can be found at:
https://sourceforge.net/projects/hostsmanager/