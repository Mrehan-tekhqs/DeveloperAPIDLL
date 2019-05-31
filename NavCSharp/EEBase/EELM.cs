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

namespace Enterprise
{
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class EELog
    {

        // ###################################################################################
        // ###################################################################################
        // Public Constructors
        // ###################################################################################
        // ###################################################################################
        public EELog()
        {
            InitializeMembers();
        }

        public EELog(string strRoot, string strProductHive, string strProductVersion, string strInstance)
        {
            InitializeMembers(strRoot, strProductHive, strProductVersion, strInstance);
        }

        // ###################################################################################
        // ###################################################################################
        // Public Functions
        // ###################################################################################
        // ###################################################################################
        public void LogMessage(string strMessage)
        {
            LogMessage(strMessage, "");
        }

        public void LogMessage(string strMessage, int intLogLevel)
        {
            LogMessage(strMessage, "", intLogLevel);
        }

        public void LogMessage(string strMessage, string strThreadName)
        {
            LogMessage(strMessage, "", 10);
        }

        public void LogMessage(string strMessage, string strThreadName, int intLogLevel)
        {
            LogEnabled = true;
            // SetRegistryInformation("Log_stat", strMessage, strThreadName, "My_instance")
            // If LogEnabled Then
            // If (intLogLevel > 0) And (intLogLevel <= LogLevel) Then
            SetStreamObject();
            strThreadName = strThreadName.PadRight(8, ' ');
            string strLogMessage;
            strLogMessage = System.Convert.ToString(DateTime.Now.Hour).PadLeft(2, '0') + ":" + System.Convert.ToString(DateTime.Now.Minute).PadLeft(2, '0') + ":" + System.Convert.ToString(DateTime.Now.Second).PadLeft(2, '0') + "-" + System.Convert.ToString(DateTime.Now.Millisecond).PadLeft(4, '0');
            //strLogMessage = strLogMessage + Constants.vbTab + strThreadName + Constants.vbTab + Constants.vbTab + strMessage;
            strLogMessage = (strLogMessage + ('\t' + (strThreadName + ('\t' + ('\t' + strMessage)))));

            lock (m_objLock)
            {
                m_objFileStream.WriteLine(strLogMessage);
                m_objFileStream.Flush();
            }
            CloseStreamObject();
        }

        public virtual void SetRegistryInformation(string strRoot = "", string strProductHive = "", string strProductVersion = "", string strInstance = "")
        {
            if ((strRoot != ""))
                m_objRegistry.Root = strRoot;
            if ((strProductHive != ""))
                m_objRegistry.ProductHive = strProductHive;
            if ((strProductVersion != ""))
                m_objRegistry.ProductVersion = strProductVersion;
            if ((strInstance != ""))
                m_objRegistry.Instance = strInstance;
            LogPath = m_objRegistry.ProductGetKeyValue("LogLocation");
            if ((LogPath == ""))
                LogPath = @"C:\EEPaymentManager\Logs\EEPM\";
            if ((m_objRegistry.ProductGetKeyValue("LogEnabled") != ""))
                LogEnabled = int.Parse(m_objRegistry.ProductGetKeyValue("LogEnabled")) == 1;
            if ((m_objRegistry.ProductGetKeyValue("LogLevel") != ""))
                LogLevel = int.Parse(m_objRegistry.ProductGetKeyValue("LogLevel"));
        }

        public virtual void OverrideRegistryInformation(string strLogPath = @"c:\Logs\", bool blnLogEnabled = false, int intLogLevel = 0)
        {
            LogPath = strLogPath;
            LogEnabled = blnLogEnabled;
            LogLevel = intLogLevel;
        }

        // ###################################################################################
        // ###################################################################################
        // Private Functions
        // ###################################################################################
        // ###################################################################################
        private void InitializeMembers(string strRoot = "", string strProductHive = "", string strProductVersion = "", string strInstance = "")
        {
            m_objFileStream = null;
            m_objLock = new object();
            m_strCurrentLogDate = DateTime.Now;
            m_objRegistry = new Enterprise.EERegistry();
            LogLevel = 0;
            LogEnabled = false;
            SetRegistryInformation(strRoot, strProductHive, strProductVersion, strInstance);
        }

        private void SetFileName()
        {
            string strYear = DateTime.Now.Year.ToString();
            string strMonth = System.Convert.ToString(DateTime.Now.Month).PadLeft(2, '0');
            string strDay = System.Convert.ToString(DateTime.Now.Day).PadLeft(2, '0');
            FileName = "EELog_" + m_objRegistry.ProductHive + "_" + strYear + strMonth + strDay + ".esl";
        }

        private void SetStreamObject()
        {
            CloseStreamObject();
            SetFileName();
            m_objFileStream = new System.IO.StreamWriter(LogPath + @"\" + FileName, true);
        }

        private void CloseStreamObject()
        {
            if (!(m_objFileStream == null))
                m_objFileStream.Close();
            m_objFileStream = null;
        }

        private bool CheckLogDate()
        {
            if ((m_strCurrentLogDate.Month != DateTime.Now.Month) | (m_strCurrentLogDate.Day != DateTime.Now.Day) | (m_strCurrentLogDate.Year != DateTime.Now.Year))
            {
                SetStreamObject();
                return true;
            }
            else
            {
                return false;
            }
                
        }

        // ###################################################################################
        // ###################################################################################
        // Property Functions
        // ###################################################################################
        // ###################################################################################
        public string LogPath
        {
            get
            {
                return m_strLogPath;
            }
            set
            {
                m_strLogPath = value;
            }
        }

        public bool LogEnabled
        {
            get
            {
                return m_blnLogEnabled;
            }
            set
            {
                m_blnLogEnabled = value;
            }
        }

        public int LogLevel
        {
            get
            {
                return m_blnLogLevel;
            }
            set
            {
                m_blnLogLevel = value;
            }
        }

        protected string FileName
        {
            get
            {
                return m_strFileName;
            }
            set
            {
                m_strFileName = value;
            }
        }

        protected DateTime CurrentLogDate
        {
            get
            {
                return m_strCurrentLogDate;
            }
            set
            {
                m_strCurrentLogDate = value;
            }
        }

        // ###################################################################################
        // ###################################################################################
        // Member Variables
        // ###################################################################################
        // ###################################################################################
        private string m_strLogPath;
        private bool m_blnLogEnabled;
        private int m_blnLogLevel;
        private string m_strFileName;
        private DateTime m_strCurrentLogDate;

        private Enterprise.EERegistry m_objRegistry;

        protected object m_objLock;
        protected System.IO.StreamWriter m_objFileStream;
    }
}
