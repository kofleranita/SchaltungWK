using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS_SchaltungWK
{
    static class GlobalClass
    {
        static private string m_globalVar = "";
        static private string m_password = "";
        static private int m_port = 17494;
        static private byte m_ip_input = 0;

        public static string ipaddress
        {
            get { return m_globalVar; }
            set { m_globalVar = value; }
        }
        public static int port
        {
            get { return m_port; }
            set { m_port = value; }
        }
        public static string password
        {
            get { return m_password; }
            set { m_password = value; }
        }
        public static byte ip_input
        {
            get { return m_ip_input; }
            set { m_ip_input = value; }
        }
    }
}
