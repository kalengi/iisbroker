# README #

IISBroker is a .NET program written in C# that handles the task of adding or removing host headers on IIS. It is designed to work with the WP-IIS plugin (https://bitbucket.org/KalenGi/wp3-iis).

To activate IISBroker do the following: 

*i) Install it by running the setup program 

*ii) Add it's installed location to the system PATH variable 

*iii) Update the IIS metabase access rights to allow the website user account (eg My_Web_Server\example.com_user) permission to change host headers 

*iv) Grant the website user account rights to access cmd.exe. Without this change, PHP would no be able to execute the IISBroker.


The server needs to be restarted before all the changes take effect.