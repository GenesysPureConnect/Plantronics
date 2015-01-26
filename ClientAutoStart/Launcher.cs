using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAutoStart
{
    static class Launcher
    {

        private static string GetServerStationFlags(string server, string station)
        {
            return String.Format("-server {0} -workstation {1}", server, station);
        }

        public static void LaunchWithNtAuth(string server, string station)
        {
            LaunchProcess(GetServerStationFlags(server, station), "-windowAuthentication");
        }

        public static void LaunchWithIcAuth(string server, string station)
        {
            LaunchProcess(GetServerStationFlags(server, station),String.Empty);
        }

        private static void LaunchProcess(string serverAndStationArgs, string authArgs)
        {
            Process.Start(@"C:\Program Files (x86)\Interactive Intelligence\ICUserApps\InteractionDesktop.exe", String.Join(" ","-silent", serverAndStationArgs, authArgs));
        }
    }
}
