using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Drivers 
{
    public class PmacDriver
    { 
        [DllImport("PowerPmacsshLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PPmacConnect(string IPAddress, string User, string Password, string Port);
        [DllImport("PowerPmacsshLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PPmacDisconnect();
        [DllImport("PowerPmacsshLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetResponse(string Cmd, StringBuilder strResponse, int length);
        [DllImport("PowerPmacsshLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PPmacIsConnected();
        [DllImport("PowerPmacsshLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPmacDownloadFile(string filepath);
        [DllImport("PowerPmacsshLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PPmacGetLastErrorStr(StringBuilder strError, int length);
        [DllImport("PowerPmacsshLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PPmacConnect_sendgetsends(string IPAddress, string User, string Password, string Port, int iBuffer);
        [DllImport("PowerPmacsshLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PPmacDisconnect_sendgetsends();
        [DllImport("PowerPmacsshLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PPmacRead_sendgetsends(StringBuilder strResponse, int length, int timeout);
    }
}
