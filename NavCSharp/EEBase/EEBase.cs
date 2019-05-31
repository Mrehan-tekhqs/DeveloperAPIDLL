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

namespace Enterprise
{
    public class EEBase
    {

        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // 
        // Protected\Private Functions
        // 
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################

        // ###################################################################################################################################################################
        // Protected Functions
        // ###################################################################################################################################################################

        /*   protected virtual void SetProductInfo(string strProductRoot, string strProductHive, int strProductCipherLength)
           {
               m_strProductRoot = strProductRoot;
               m_strProductHive = strProductHive;
               m_strProductCipherLength = strProductCipherLength;
               m_strProductVersion = Strings.Left(Strings.Replace(ObjectVersion, ".", ""), 3);
           } */
        protected virtual void SetProductInfo(string strProductRoot, string strProductHive, int strProductCipherLength)
        {
            m_strProductRoot = strProductRoot;
            m_strProductHive = strProductHive;
            m_strProductCipherLength = strProductCipherLength;
            m_strProductVersion = ObjectVersion.Replace(".", "").Substring(0, 3);
        }


        protected virtual void SetupBase(string strProductRoot = "", string strProductHive = "", int strProductCipherLength = 0)
        {
            // SetupBase("PCICharge", "EEPM", 2)
            SetProductInfo(strProductRoot, strProductHive, strProductCipherLength);

            m_objLog = new Enterprise.EELog(m_strProductRoot, m_strProductHive, m_strProductVersion, "");
            m_objComputerManagement = new Enterprise.EEComputerManagement();
            m_objLog.LogMessage("EEBase : SetupBase : Log Initialized", 35);
        }

        protected virtual void SetupBase(string strProductRoot, string strProductHive, int strProductCipherLength, string strBaseKey, string strSpecificKey, string strKeyDat, string strCustomerData, string strLogPath = "", bool blnLogEnabled = false, int intLogLevel = 0)
        {
            SetProductInfo(strProductRoot, strProductHive, strProductCipherLength);
            m_objLog = new Enterprise.EELog();
            m_objLog.OverrideRegistryInformation(strLogPath, blnLogEnabled, intLogLevel);
            m_objLog.LogMessage("EEBase : SetupBase : Log Initialized", 35);
            m_objComputerManagement = new Enterprise.EEComputerManagement();
        }

        protected bool CheckInternalLicenses(int intCheckLocation, int intCapturedAmount = 0, string strInstance = "")
        {
            return true;
        }

        // ###################################################################################################################################################################
        // Property Functions
        // ###################################################################################################################################################################

        protected string ObjectVersion
        {
            get
            {
                Version objVersionInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return objVersionInfo.Major + "." + objVersionInfo.Minor + "." + objVersionInfo.Build + "." + objVersionInfo.Revision;
            }
        }

        public virtual int ResponseCode
        {
            get
            {
                return m_intResponseCode;
            }
        }

        public virtual string ResponseDescription
        {
            get
            {
                return m_strResponseDescription;
            }
        }

        // ###################################################################################################################################################################
        // Private\Protected Member Variables
        // ###################################################################################################################################################################

        protected string m_strProductRoot;
        protected string m_strProductHive;
        protected string m_strProductVersion;
        protected string m_strProductInstance;
        protected int m_strProductCipherLength;

        protected Enterprise.EELog m_objLog;
        protected Enterprise.EERegistry m_objRegistry;
        protected Enterprise.EEComputerManagement m_objComputerManagement;

        protected int m_intResponseCode; // Internal response code to calling process.
        protected string m_strResponseDescription; // Internal response message to calling process.
    }

}
