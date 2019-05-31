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
using Microsoft.Win32;

namespace Enterprise
{
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class EERegistry
    {

        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // 
        // Constructors\Destructors
        // 
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################

        public EERegistry()
        {
            InitializeMembers();
        }

        public EERegistry(string strRoot, string strProductHive, string strProductVersion)
        {
            InitializeMembers(strRoot, strProductHive, strProductVersion);
        }

        public EERegistry(string strRoot, string strProductHive, string strProductVersion, string strInstance)
        {
            InitializeMembers(strRoot, strProductHive, strProductVersion, strInstance);
        }



        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // 
        // Public Functions
        // 
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################

        public bool PathExists(string strPath)
        {
            bool blnReturn = false;
            lock (m_objLock)
                blnReturn = CheckPathExists(strPath);
            return blnReturn;
        }

        public bool ProductPathExists(string strRoot = "", string strProductHive = "", string strProductVersion = "")
        {
            string strCheckStr = "";
            lock (m_objLock)
            {
                if ((strRoot != ""))
                    Root = strRoot;
                if ((strProductHive != ""))
                    ProductHive = strProductHive;
                if ((strProductVersion != ""))
                    ProductVersion = strProductVersion;
            }
            return CheckPathExists(KeyPath, true);
        }

        public string ProductGetLatestVersion(string strRoot = "", string strInstance = "", string strProductHive = "")
        {
            string strReturn = "";
            strReturn = ReturnHighestVersion(strRoot, strInstance, strProductHive);
            return strReturn;
        }

        public string GetKeyValue(string strPath, string strKeyName)
        {
            string strReturn = "";
            lock (m_objLock)
                strReturn = GetValueEntry(strPath, strKeyName);
            return strReturn;
        }

        public string ProductGetKeyValue(string strKeyName, string strRoot = "", string strProductHive = "", string strProductVersion = "", string strInstance = "")
        {
            string strReturn = "";
            lock (m_objLock)
            {
                if ((strRoot != ""))
                    Root = strRoot;
                if ((strProductHive != ""))
                    ProductHive = strProductHive;
                if ((strProductVersion != ""))
                    ProductVersion = strProductVersion;
                if ((strInstance != ""))
                    Instance = strInstance;
                strReturn = GetValueEntry(KeyPath, strKeyName);
            }
            return strReturn;
        }

        public bool CreatePath(string strPath)
        {
            bool blnReturn = false;
            RegistryKey objRegKey = null/* TODO Change to default(_) if this is not a reference type */;
            objRegKey = CreateRegistryPath(strPath);
            if (!(objRegKey == null))
                blnReturn = true;
            return blnReturn;
        }

        public bool ProductCreatePath(string strRoot = "", string strProductHive = "", string strProductVersion = "", string strInstance = "")
        {
            bool blnReturn = false;
            RegistryKey objRegKey = null/* TODO Change to default(_) if this is not a reference type */;
            lock (m_objLock)
            {
                if ((strRoot != ""))
                    Root = strRoot;
                if ((strProductHive != ""))
                    ProductHive = strProductHive;
                if ((strProductVersion != ""))
                    ProductVersion = strProductVersion;
                objRegKey = CreateRegistryPath(KeyPath);
            }
            if (!(objRegKey == null))
                blnReturn = true;
            return blnReturn;
        }

        public bool CreateKey(string strPath, string strKeyName, string strKeyValue)
        {
            bool blnReturn = false;
            lock (m_objLock)
                blnReturn = CreateRegistryKey(strPath, strKeyName, strKeyValue);
            return blnReturn;
        }

        public bool ProductCreateKey(string strKeyName, string strKeyValue, string strRoot = "", string strProductHive = "", string strProductVersion = "", string strInstance = "")
        {
            bool blnReturn = false;
            lock (m_objLock)
            {
                if ((strRoot != ""))
                    Root = strRoot;
                if ((strProductHive != ""))
                    ProductHive = strProductHive;
                if ((strProductVersion != ""))
                    ProductVersion = strProductVersion;
                blnReturn = CreateRegistryKey(KeyPath, strKeyName, strKeyValue);
            }
            return blnReturn;
        }

        public bool ProductCreateValueEntry(string strValueName, string strValueValue, bool blnUpdateExisting = true, string strRoot = "", string strProductHive = "", string strProductVersion = "", string strInstance = "")
        {
            bool blnReturn = false;
            lock (m_objLock)
            {
                if ((strRoot != ""))
                    Root = strRoot;
                if ((strProductHive != ""))
                    ProductHive = strProductHive;
                if ((strProductVersion != ""))
                    ProductVersion = strProductVersion;
                blnReturn = (SetValueEntry(KeyPath, strValueName, strValueValue, blnUpdateExisting, false) != "");
            }
            return blnReturn;
        }



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

        protected void InitializeMembers(string strRoot = "", string strProductHive = "", string strProductVersion = "", string strInstance = "")
        {
            Root = strRoot;
            ProductHive = strProductHive;
            ProductVersion = strProductVersion;
            Instance = strInstance;
            if ((m_objLock == null))
                m_objLock = new object();
        }

        protected bool CheckPathExists(string strPath, bool blnAddBasePath = false)
        {
            bool blnReturn = false;
            RegistryKey objCheckKey;
            m_objRegKey = Registry.LocalMachine;
            if ((blnAddBasePath))
                strPath = m_strBaseKeyPath + strPath;
            try
            {
                objCheckKey = m_objRegKey.OpenSubKey(strPath, false);
                if (!(objCheckKey == null))
                    blnReturn = true;
            }
            catch
            {
            }
            return blnReturn;
        }

        protected string ReturnHighestVersion(string strRoot = "", string strInstance = "", string strProductHive = "")
        {
            string strReturn = "";
            RegistryKey objProductPath;
            if ((strRoot == ""))
                strRoot = Root;
            if ((strInstance == ""))
                strInstance = Instance;
            if ((strProductHive == ""))
                strProductHive = ProductHive;

            string strPath = m_strBaseKeyPath + @"\";
            if ((strRoot != ""))
                strPath += strRoot + @"\";
            if ((strInstance != ""))
                strPath += strInstance + @"\";
            //if ((Strings.Len(strPath) <= 1))
            if ((strPath.Length <= 1))
                strPath = "";
            

            if ((CheckPathExists(strPath + strProductHive)))
            {
                objProductPath = GetPath(strPath + strProductHive);
                string[] strSubKeys = objProductPath.GetSubKeyNames();
                // if ((Information.UBound(strSubKeys) >= 0))
                //   strReturn = strSubKeys[Information.UBound(strSubKeysif ((UBound(strSubKeys) >= 0)) {
                if ((strSubKeys).GetUpperBound(0) >= 0)
                    strReturn = (strSubKeys).GetUpperBound(0).ToString();

            }
            return strReturn;
        }

        protected RegistryKey GetPath(string strPath)
        {
            m_objRegKey = Registry.LocalMachine;
            try
            {
                m_objRegKey = m_objRegKey.OpenSubKey(strPath, true);
            }
            catch
            {
                m_objRegKey = null;
            }
            return m_objRegKey;
        }

        protected string GetValueEntry(string strPath, string strEntryName)
        {
            string strReturn = "";
            object objValue;
            RegistryKey objRegKey = GetPath(strPath);
            if ((objRegKey == null))
                return strReturn;
            try
            {
                objValue = objRegKey.GetValue(strEntryName);
                /*     if ((Strings.Left(strEntryName, 11) == "SpecificKey"))
                         strReturn = IlluminateKey(objValue.ToString());
                     else
                         strReturn = objValue.ToString();
                 } */
                if ((strEntryName.Substring(0, 11) == "SpecificKey"))

                    strReturn = IlluminateKey(objValue.ToString());
                else
                    strReturn = objValue.ToString();
            }
            catch
            {
                strReturn = "";
            }
            return strReturn;
        }

        protected string SetValueEntry(string strPath, string strEntryName, string strEntryValue, bool blnUpdateExisting = true, bool blnAddBasePath = true)
        {
            string strReturn = "";
            object objValue;
            RegistryKey objRegKey = GetPath(strPath);
            if ((objRegKey == null))
                return strReturn;
            try
            {
                objValue = objRegKey.GetValue(strEntryName);
                if ((objValue.ToString() != ""))
                {
                    if ((blnUpdateExisting))
                    {
                        // if ((Strings.Left(strEntryName, 11) == "SpecificKey"))
                        if ((strEntryName.Substring(0, 11) == "SpecificKey"))
                            objRegKey.SetValue(strEntryName, ObfuscateKey(strEntryValue));
                        else
                            objRegKey.SetValue(strEntryName, strEntryValue);
                    }
                }
                //   else if ((Strings.Left(strEntryName, 11) == "SpecificKey"))
                else if ((strEntryName.Substring(0, 11) == "SpecificKey"))
                    objRegKey.SetValue(strEntryName, ObfuscateKey(strEntryValue));
                else
                    objRegKey.SetValue(strEntryName, strEntryValue);
                objValue = objRegKey.GetValue(strEntryName);
                if ((objValue.ToString() != strEntryValue))
                    strReturn = "";
                else
                    strReturn = objValue.ToString();
            }
            catch
            {
                strReturn = "";
            }
            return strReturn;
        }

        // ###################################################################################################################################################################
        // Private Functions
        // ###################################################################################################################################################################

        private RegistryKey CreateRegistryKey(string strPath, string strKey)
        {
            RegistryKey objReturn;
            try
            {
                m_objRegKey = m_objRegKey.OpenSubKey(strPath, true);
                objReturn = m_objRegKey.CreateSubKey(strKey);
            }
            catch
            {
                objReturn = null/* TODO Change to default(_) if this is not a reference type */;
            }
            return objReturn;
        }

        private RegistryKey CreateRegistryPath(string strPath, bool blnAddBasePath = false)
        {
            RegistryKey objReturn = null/* TODO Change to default(_) if this is not a reference type */;
            int intCounter = 0;
            string strCurrentPath = "";
            string strLastKnownGoodPath = "";
            //string[] arrPathPieces = Strings.Split(strPath, @"\");
            string[] arrPathPieces = strPath.Split('\\');
            m_objRegKey = Registry.LocalMachine;
            if ((blnAddBasePath))
                strPath = m_strBaseKeyPath + strPath;
            objReturn = GetPath(strPath);
            if (!(objReturn == null))
                return objReturn;
            try
            {
                do
                {
                    // if ((intCounter <= Information.UBound(arrPathPieces)))
                    if ((intCounter <= (arrPathPieces).GetUpperBound(0)))
                    {
                        
                            if ((arrPathPieces[intCounter] != ""))
                            {
                                strCurrentPath = strCurrentPath + arrPathPieces[intCounter];
                                if (!(CheckPathExists(strCurrentPath)))
                                {
                                    objReturn = CreateRegistryKey(strLastKnownGoodPath, arrPathPieces[intCounter]);
                                    strLastKnownGoodPath = strLastKnownGoodPath + @"\" + arrPathPieces[intCounter];
                                }
                                else
                                    strLastKnownGoodPath = strCurrentPath;
                                strCurrentPath = strCurrentPath + @"\";
                                intCounter = intCounter + 1;
                            }
                        }
                    else
                        break;
                    }
                    while (true) ;
                }
                
            catch
            {
                objReturn = null/* TODO Change to default(_) if this is not a reference type */;
            }
            return objReturn;
        }

        private bool CreateRegistryKey(string strPath, string strKeyName, string strKeyValue)
        {
            bool blnReturn = false;
            RegistryKey objRegKey = GetPath(strPath);
            if ((objRegKey == null))
                objRegKey = CreateRegistryPath(strPath);
            if ((objRegKey == null))
                return blnReturn;
            try
            {
                //  if ((Strings.Left(strKeyName, 11) == "SpecificKey"))
                if ((strKeyName.Substring(0, 11) == "SpecificKey"))
                    objRegKey.SetValue(strKeyName, ObfuscateKey(strKeyValue));
                 
                else
                    objRegKey.SetValue(strKeyName, strKeyValue);
                objRegKey.Close();
                blnReturn = true;
            }
            catch
            {
            }
            return blnReturn;
        }

        private string ObfuscateKey(string strKeyValue)
        {
            int intChar;
            int intIndex;
            string strReturn = "";

            if ((strKeyValue == ""))
                return strReturn;

            /*     for (intIndex = 1; intIndex <= Strings.Len(strKeyValue); intIndex++)
                 {
                     intChar = Strings.Asc(Strings.Mid(strKeyValue, intIndex, 1));
                     intChar = intChar ^ 111;
                     strReturn += Strings.Right("0" + Conversion.Hex(intChar), 2);
                 }
                 */
            for (intIndex = 1; (intIndex <= strKeyValue.Length); intIndex++)
            {
                intChar = Convert.ToInt32(strKeyValue.Substring((intIndex - 1), 1));
                intChar = intChar ^ 111;
                strReturn += ("0" + int.Parse(intChar.ToString(), NumberStyles.HexNumber)).Substring((("0" + int.Parse(intChar.ToString(),NumberStyles.HexNumber)).Length - 2));
            }
            return strReturn;
        }

        private string IlluminateKey(string strKeyValue)
        {
            int intChar;
            int intIndex;
            string strReturn = "";

            if ((strKeyValue == ""))
                return strReturn;
            /*
                        for (intIndex = 1; intIndex <= Strings.Len(strKeyValue) / (double)2; intIndex++)
                        {
                            intChar = int.Parse("&H" + Strings.Mid(strKeyValue, (intIndex - 1) * 2 + 1, 2));
                            intChar = intChar ^ 111;
                            strReturn += Strings.Chr(intChar);
                        }
                        */
            for (intIndex = 1; (intIndex <= (strKeyValue.Length / 2)); intIndex++)
            {
                intChar = int.Parse("&H" + strKeyValue.Substring(((intIndex - 1)* 2), 2));
                intChar = intChar ^ 111;
                strReturn += ((char)(intChar));
            }


            return strReturn;
        }


        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // 
        // Member Variables\Functions
        // 
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################
        // ###################################################################################################################################################################

        // ###################################################################################################################################################################
        // Property Functions
        // ###################################################################################################################################################################

        public virtual string ProductHive
        {
            get
            {
                return m_strProductHive;
            }
            set
            {
                m_strProductHive = value;
            }
        }

        public virtual string ProductVersion
        {
            get
            {
                return m_strProductVersion;
            }
            set
            {
                value = value.ToString().Replace(".", "");
                if ((value.Length) > 3)
                    value = value.Substring(1, 3);
                m_strProductVersion = value;
            }
        }

        public virtual string Root
        {
            get
            {
                return m_strRoot;
            }
            set
            {
                m_strRoot = value;
            }
        }

        public virtual string Instance
        {
            get
            {
                return m_strInstance;
            }
            set
            {
                m_strInstance = value;
            }
        }

        public virtual string KeyPath
        {
            get
            {
                string strReturn = "";
                if ((m_strBaseKeyPath != ""))
                    strReturn += @"\\" + m_strBaseKeyPath;
                if ((m_strRoot != ""))
                    strReturn += @"\\" + m_strRoot;
                if ((m_strInstance != ""))
                    strReturn += @"\\" + m_strInstance;
                if ((m_strProductHive != ""))
                    strReturn += @"\\" + m_strProductHive;
                if ((m_strProductVersion != ""))
                    strReturn += @"\\" + m_strProductVersion;
                // if ((Strings.InStr(strReturn, @"\") == 1))
                //   strReturn = Strings.Mid(strReturn, 3);

                if (((strReturn.IndexOf("\\") + 1) == 1))
                    strReturn = strReturn.Substring(2);

                // if ((Strings.Len(strReturn) > 0))
                // strReturn += @"\\";
                if ((strReturn.Length > 0))
                strReturn = "\\\\";
                return strReturn;
            }
        }

        // ###################################################################################################################################################################
        // Private Member Variables
        // ###################################################################################################################################################################

        private RegistryKey m_objRegKey;

        private string m_strRoot;
        private string m_strInstance;
        private string m_strProductHive;
        private string m_strProductVersion;

        protected string m_strBaseKeyPath = "SOFTWARE";

        protected static object m_objLock;
    }
}
