using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using System.Net;


/*
 * Expected arguments:
 * -action [add|remove]
 * -server [server_name] (default is localhost)
 * -website [website_description]
 * -hostname [host_name]
 * -port [port_number] (default is 80)
 * -ip [ip_address]
 */
namespace IISBroker
{
    class Program
    {
        static void Main(string[] args)
        {

            string result = "";
            CommandLineArgs hostHeaderArgs = new CommandLineArgs(args);

            if (hostHeaderArgs["action"].Length == 0)
            {
                Console.WriteLine("Action not specified: -action [add|remove]");
                return;
            }

            string serverName = (hostHeaderArgs["server"].Length == 0) ? "localhost" : hostHeaderArgs["server"];
            

            if (hostHeaderArgs["website"].Length == 0)
            {
                Console.WriteLine("Website name missing: -website [iis_website_description]");
                return;
            }
            string websiteName = hostHeaderArgs["website"];

            if (hostHeaderArgs["ip"].Length == 0)
            {
                Console.WriteLine("IP address missing: -ip [ip_address]");
                return;
            }
            string ipAddress = hostHeaderArgs["ip"];
            if (!IsValidIP(ipAddress))
            {
                Console.WriteLine("Invalid IP address");
                return;
            }

            string port = (hostHeaderArgs["port"].Length == 0) ? "80" : hostHeaderArgs["port"];

            if (hostHeaderArgs["hostname"].Length == 0)
            {
                Console.WriteLine("Hostname missing: -hostname [host_name]");
                return;
            }
            string hostname = hostHeaderArgs["hostname"];
            if (!IsValidHostname(hostname))
            {
                Console.WriteLine("Invalid Hostname");
                return;
            }

            IISAdmin webServer = null;

            try
            {
                webServer = new IISAdmin(serverName);
            }
            catch (Exception error)
            {
                Console.WriteLine("Error creating IIS broker.\n" + error.Message + "\n" + error.InnerException);
                return;
            }

            if(hostHeaderArgs["action"].ToLower().Equals("add"))
            {
                try
                {
                    result = webServer.AddHostHeader(websiteName, ipAddress, port, hostname);
                
                }
                catch (Exception error)
                {
                    Console.WriteLine("IIS broker error adding host header.\n" + error.Message + "\n" + error.InnerException);
                    return;
                }
            }
            else
            {
                if(hostHeaderArgs["action"].ToLower().Equals("remove"))
                {
                    try
                    {
                        result = webServer.RemoveHostHeader(websiteName, ipAddress, port, hostname);
                    
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine("IIS broker error removing host header.\n" + error.Message + "\n" + error.InnerException);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Unrecognized action: -action [add|remove]");
                    return;
                }
            }
            Console.WriteLine(result);
            return;
        }

        private static bool IsValidHostname(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
            {
                return false;
            }
            else
            {
                UriHostNameType hostnameType = Uri.CheckHostName(hostname);
                if (hostnameType == UriHostNameType.Dns)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static bool IsValidIP(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                return false;
            }
            else
            {
                IPAddress validatedIP = null;
                return IPAddress.TryParse(ipAddress, out validatedIP);
            }
        }
    }
}
