using System;
using System.Collections.Generic;

namespace xAPI.Sync
{
    [Obsolete("Use Servers class instead")]
    public class ServerData
    {
        private static string XAPI_A = "xapia.x-station.eu";
	    private static string XAPI_B = "xapib.x-station.eu";

	    private static int[] PORTS_REAL = {5112, 5113};
	    private static int[] PORTS_REAL_NO_SSL = {5108, 5109};
	    private static int[] PORTS_DEMO = {5124, 5125};
	    private static int[] PORTS_DEMO_NO_SSL = {5122, 5123};
	    private static int[] PORTS_UAT = {5116, 5117};
	    private static int[] PORTS_UAT_NO_SSL = {5106, 5107};
        private static int[] PORTS_DEV = { 23460, 23461 };
        private static int[] PORTS_DEMO_META = { 23460, 23461 };

        private static Dictionary<string, string> xapiList;

        public ServerData()
        {
            SetUpList();
        }

        private static void SetUpList()
        {
            xapiList = new Dictionary<string, string>();
            xapiList.Add("A", XAPI_A);
            xapiList.Add("B", XAPI_B);
        }

        /// <summary>
        /// Static method which receives map of development servers.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Server> DevelopmentServers
        {
            get 
            {
                Dictionary<string, Server> dict = new Dictionary<string, Server>();

                dict = AddServers(dict, PORTS_DEMO_NO_SSL, false, "DEMO_NO_SSL");
                dict = AddServers(dict, PORTS_REAL_NO_SSL, false, "REAL_NO_SSL");
                dict = AddServers(dict, PORTS_UAT, true, "UAT");
                dict = AddServers(dict, PORTS_UAT_NO_SSL, false, "UAT_NO_SSL");
                dict = AddServers(dict, PORTS_DEV, false, "DEV");

                return dict;
            }
            
        }

        /// <summary>
        /// Static method which receives map of production servers
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Server> ProductionServers
        {
            get
            {
                Dictionary<string, Server> dict = new Dictionary<string, Server>();

                dict = AddServers(dict, PORTS_DEMO, true, "DEMO");
                dict = AddServers(dict, PORTS_REAL, true, "REAL");

                return dict;
            }
        }

        /// <summary>
        /// Static method that adds a server
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="portsArray"></param>
        /// <param name="isSsl"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        private static Dictionary<string, Server> AddServers(Dictionary<string, Server> dict, int[] portsArray, bool isSsl, string desc)
        {
    	    if(xapiList == null)
    	    {
    		    SetUpList();
    	    }
    	
    	    int mainPort = portsArray[0];
    	    int streamingPort = portsArray[1];

            if (desc.Equals("DEV"))
            {
                string address = "192.168.9.11";
                string dictKey = "XSERVER_" + desc;
                string dictDesc = "xServer " + desc;
                dict.Add(dictKey, new Server(address, mainPort, streamingPort, isSsl, dictDesc));
            }
            else
            {
                foreach (String xapiKey in xapiList.Keys)
                {
                    string address = xapiList[xapiKey];
                    string dictKey = "XSERVER_" + desc + "_" + xapiKey;
                    string dictDesc = "xServer " + desc + " " + xapiKey;
                    dict.Add(dictKey, new Server(address, mainPort, streamingPort, isSsl, dictDesc));
                }
            }
    	    return dict;
        }
    }
}
