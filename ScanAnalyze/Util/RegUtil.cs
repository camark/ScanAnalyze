using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScanAnalyze.Util
{
    class RegUtil
    {
        private string reg_root = @"software\emz\scananalyze";
        private string _lastusername = "lastUserName";

        private String ReadString(String key)
        {
            String ret = String.Empty;
            using (RegistryKey reg_key = Registry.CurrentUser.CreateSubKey(reg_root))
            {
                if (isStringInArray(reg_key.GetValueNames(),key))
                {
                    ret = reg_key.GetValue(key).ToString();
                }
            }


            return ret;
        }

        private bool isStringInArray(string[] arraystring,string ss)
        {
            bool bRet = false;
            foreach(var s in arraystring)
            {
                if (s.Equals(ss))
                {
                    bRet = true;
                }
            }

            return bRet;
        }


        private void WriteString(String key, String value)
        {
            using (RegistryKey reg_key = Registry.CurrentUser.CreateSubKey(reg_root))
            {

                reg_key.SetValue(key, value);

            }
        }

        public String ReadLastUserName()
        {
            return ReadString(_lastusername);
        }

        public void WriteLastUserName(string username)
        {
            WriteString(_lastusername, username);
        }
    }
}
