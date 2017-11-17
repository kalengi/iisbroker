using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;

namespace IISBroker
{
    class IISAdmin
    {
        private string mServer;

        public IISAdmin(string server)
        {
            if (server.Trim().Length == 0)
            {
                throw (new System.Exception("Please provide the server name"));
            }

            mServer = server;
        }

        public IISAdmin()
        {
            mServer = "localhost";
        }

        public string Server
        {
            get
            {
                return mServer;
            }
            set
            {
                mServer = value;
            }
        }

        public string AddHostHeader(string websiteName, string ipAddress, string port, string hostname)
        {
            string websiteID = GetWebsiteID(websiteName);
            string websitePath = "IIS://" + mServer + "/w3svc/" + websiteID;
            DirectoryEntry website = null;

            try
            {
                website = new DirectoryEntry(websitePath);
            }
            catch (Exception error)
            {
                throw new Exception("Error creating the web directory entry.\n" + error.Message + "\n" + error.InnerException);
            }

            PropertyValueCollection websiteBindings = null;

            try
            {
                websiteBindings = website.Properties["ServerBindings"];
            }
            catch (Exception error)
            {
                throw new Exception("Error getting the host header listing.\n" + error.Message + "\n" + error.InnerException);
            }

            string hostHeader = string.Format("{0}:{1}:{2}", ipAddress, port, hostname);

            if (websiteBindings.Contains(hostHeader))
            {
                return "exists";
            }

            try
            {
                websiteBindings.Add(hostHeader);
                website.CommitChanges();
            }
            catch (System.Exception error)
            {
                throw new Exception("Error adding the host header.\n" + error.Message + "\n" + error.InnerException);
            }

            return "added";
        }

        public string RemoveHostHeader(string websiteName, string ipAddress, string port, string hostname)
        {
            string websiteID = GetWebsiteID(websiteName);
            string websitePath = "IIS://" + mServer + "/w3svc/" + websiteID;
            DirectoryEntry website = null;

            try
            {
                website = new DirectoryEntry(websitePath);
            }
            catch (Exception error)
            {
                throw new Exception("Error creating the web directory entry.\n" + error.Message + "\n" + error.InnerException);
            }

            PropertyValueCollection websiteBindings = null;
            
            try
            {
                websiteBindings = website.Properties["ServerBindings"];
            }
            catch (Exception error)
            {
                throw new Exception("Error getting the host header listing.\n" + error.Message + "\n" + error.InnerException);
            }

            string hostHeader = string.Format("{0}:{1}:{2}", ipAddress, port, hostname);

            if (!websiteBindings.Contains(hostHeader))
            {
                return "missing";
            }

            try
            {
                websiteBindings.Remove(hostHeader);
                website.CommitChanges();
            }
            catch (System.Exception error)
            {
                throw new Exception("Error removing the host header.\n" + error.Message + "\n" + error.InnerException);
            }

            return "removed";
        }

        public string GetWebsiteID(string websiteName)
        {
            string w3svcPath = "IIS://" + mServer + "/w3svc";
            DirectoryEntry w3svc = null;

            try
            {
                w3svc = new DirectoryEntry(w3svcPath);
            }
            catch (Exception error)
            {
                throw new Exception("Error creating the IIS directory entry.\n" + error.Message + "\n" + error.InnerException);
            }

            foreach (DirectoryEntry website in w3svc.Children)
            {
                if (website.SchemaClassName == "IIsWebServer" && website.Properties["ServerComment"][0].ToString() == websiteName)
                {
                    return website.Name;
                }

            }

            return "0";
        }
    }
}
