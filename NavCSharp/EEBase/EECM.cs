using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Management;

namespace Enterprise
{
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class EEComputerManagement
    {

        // ###################################################################################
        // ###################################################################################
        // Public Constructors
        // ###################################################################################
        // ###################################################################################
        public EEComputerManagement()
        {
            InitializeMembers();
        }


        // ###################################################################################
        // ###################################################################################
        // Public Functions
        // ###################################################################################
        // ###################################################################################
        public void Refresh()
        {
            InitializeMembers();
        }


        // ###################################################################################
        // ###################################################################################
        // Private Functions
        // ###################################################################################
        // ###################################################################################
       // private void InitializeMembers()
        //{
            //ManagementObject objMgmt;
          //  ManagementObjectSearcher objMgmtSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            /*
                        foreach (var objMgmt in objMgmtSearcher.Get())
                        {
                            if ((Strings.InStr(objMgmt["name"].ToString(), "|") > 0))
                            {
                                m_strWindowsOS = Strings.Mid(objMgmt["name"].ToString(), 1, Strings.InStr(objMgmt["name"].ToString(), "|") - 2);
                                m_strWindowsLocation = Strings.Mid(objMgmt["name"].ToString(), Strings.InStr(objMgmt["name"].ToString(), "|") + 1);
                            }
                            else
                                m_strWindowsOS = objMgmt["name"].ToString();
                            m_strComputerName = objMgmt["csname"].ToString();
                            m_strWindowsRoot = objMgmt["windowsdirectory"].ToString();
                        }

                        objMgmtSearcher = null/* TODO Change to default(_) if this is not a reference type ;
                        objMgmtSearcher = new ManagementObjectSearcher("Select UUID From Win32_ComputerSystemProduct");
                        foreach (var objMgmt in objMgmtSearcher.Get())
                            m_strComputerUUID = objMgmt["UUID"].ToString();

                        objMgmtSearcher = null/* TODO Change to default(_) if this is not a reference type ;
                        objMgmtSearcher = new ManagementObjectSearcher("Select * From Win32_NetworkAdapterConfiguration");
                        foreach (var objMgmt in objMgmtSearcher.Get())
                        {
                            if ((objMgmt["IPEnabled"].ToString() == "True"))
                            {
                                string[] gateways = (string[])objMgmt["DefaultIPGateway"];
                                string[] addresses = (string[])objMgmt["IPAddress"];
                                if ((gateways[0].ToString() != ""))
                                {
                                    m_strNetworkIPAddress = addresses[0].ToString();
                                    m_strNetworkMACAddress = objMgmt["MacAddress"].ToString();
                                    break;
                                }
                            }
                        }
                    }
              */

            private void InitializeMembers()
            {
               // ManagementObject objMgmt;
                ManagementObjectSearcher objMgmtSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                foreach (var objMgmt in objMgmtSearcher.Get())
                {
                    if (((objMgmt["name"].ToString().IndexOf("|") + 1)
                                > 0))
                    {
                        m_strWindowsOS = objMgmt["name"].ToString().Substring(0, ((objMgmt["name"].ToString().IndexOf("|") + 1)
                                        - 2));
                        m_strWindowsLocation = objMgmt["name"].ToString().Substring((objMgmt["name"].ToString().IndexOf("|") + 1));
                    }
                    else
                    {
                        m_strWindowsOS = objMgmt["name"].ToString();
                    }

                    m_strComputerName = objMgmt["csname"].ToString();
                    m_strWindowsRoot = objMgmt["windowsdirectory"].ToString();
                }

                objMgmtSearcher = null;
                objMgmtSearcher = new ManagementObjectSearcher("Select UUID From Win32_ComputerSystemProduct");
                foreach (var objMgmt in objMgmtSearcher.Get())
                {
                    m_strComputerUUID = objMgmt["UUID"].ToString();
                }

                objMgmtSearcher = null;
                objMgmtSearcher = new ManagementObjectSearcher("Select * From Win32_NetworkAdapterConfiguration");
                foreach (var objMgmt in objMgmtSearcher.Get())
                {
                    if ((objMgmt["IPEnabled"].ToString() == "True"))
                    {
                        if ((objMgmt["DefaultIPGateway"].ToString() != ""))
                        {
                            m_strNetworkIPAddress = objMgmt["IPAddress"].ToString();
                            m_strNetworkMACAddress = objMgmt["MacAddress"].ToString();
                            break;
                        }

                    }

                }

            }

        // ###################################################################################
        // ###################################################################################
        // Property Functions
        // ###################################################################################
        // ###################################################################################
        public string ComputerName
        {
            get
            {
                return m_strComputerName;
            }
        }
        public string ComputerUUID
        {
            get
            {
                return m_strComputerUUID;
            }
        }

        public string WindowsOS
        {
            get
            {
                return m_strWindowsOS;
            }
        }
        public string WindowsRoot
        {
            get
            {
                return m_strWindowsRoot;
            }
        }

        public string IPAddress
        {
            get
            {
                return m_strNetworkIPAddress;
            }
        }
        public string MACAddress
        {
            get
            {
                return m_strNetworkMACAddress;
            }
        }


        // ###################################################################################
        // ###################################################################################
        // Member Variables
        // ###################################################################################
        // ###################################################################################
        private string m_strComputerName;
        private string m_strComputerUUID;

        private string m_strWindowsOS;
        private string m_strWindowsRoot;
        private string m_strWindowsLocation;

        private string m_strNetworkIPAddress;
        private string m_strNetworkMACAddress;
    }
}
