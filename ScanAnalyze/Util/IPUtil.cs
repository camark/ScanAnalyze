using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ScanAnalyze.Util
{
    class IPUtil
    {
        public string GetIpAddress()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostEntry(hostName);    //方法已过期，可以获取IPv4的地址
                                                                    //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];

            String ret = string.Empty;
            foreach (var la in localhost.AddressList)
            {
                if (la.ToString().StartsWith("10.10."))
                {
                    ret = la.ToString();
                }
            }

            return ret;
        }
    }
}
