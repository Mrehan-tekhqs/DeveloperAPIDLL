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
using System.Net;

namespace EEPM
{
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class EEPMGWBase
    {

        // ###################################################################################
        // Constructors\Destructors
        // ###################################################################################
        public EEPMGWBase(int intGatewayID, string strGatewayURL, string strMerchantLogin, string strMerchantPassword, string strMerchantSecurityGUID, ref Enterprise.EELog objLog)
        {
            m_intGatewayID = intGatewayID;
            m_strGatewayResponseCode = "";
            m_strGatewayResponseDescription = "";
            m_intEEPGResponseCode = -1;
            m_strEEPGResponseDescription = "";
            m_strGatewayURL = strGatewayURL;
            m_strMerchantLogin = strMerchantLogin;
            m_strMerchantPassword = strMerchantPassword;
            m_strMerchantSecurityGUID = strMerchantSecurityGUID;
            m_objLog = objLog;

            m_objSetGW = null;

            m_objNSoftwareGW = null;
            m_objNSoftwareCard = null;
            m_objNSoftwareCustomer = null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            BuildSetGW();
            BuildScrubList();
        }

        ~EEPMGWBase()
        {
            m_intGatewayID = 0;
            m_strGatewayResponseCode = "";
            m_strGatewayResponseDescription = "";
            m_intEEPGResponseCode = 0;
            m_strEEPGResponseDescription = "";

            m_objSetGW = null;
            m_objScrubList = null;
            m_objScrubData = null;

            m_objNSoftwareGW = null;
            m_objNSoftwareCard = null;
            m_objNSoftwareCustomer = null;
        }

        // ###################################################################################
        // Public functions
        // ###################################################################################
        public virtual bool Authorize(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: Authorize()", 40);
            m_intEEPGResponseCode = 98009;
            m_strEEPGResponseDescription = "Required function Authorize not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: Authorize(): " + blnReturn, 40);
            return blnReturn;
        }
        public virtual bool TransactionDetails(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: TransactionDetails()", 40);
            m_intEEPGResponseCode = 98013;
            m_strEEPGResponseDescription = "Required function TransactionDetails not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: TransactionDetails(): " + blnReturn, 40);
            return blnReturn;
        }

        public virtual bool Capture(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: Capture()", 40);
            m_intEEPGResponseCode = 98010;
            m_strEEPGResponseDescription = "Required function Capture not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: Capture(): " + blnReturn, 40);
            return blnReturn;
        }

        public virtual bool DirectSale(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: DirectSale()", 40);
            m_intEEPGResponseCode = 98011;
            m_strEEPGResponseDescription = "Required function DirectSale not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: DirectSale(): " + blnReturn, 40);
            return blnReturn;
        }

        public virtual bool Notification(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: Notification()", 40);
            m_intEEPGResponseCode = 980123;
            m_strEEPGResponseDescription = "Required function Notification not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: Notification(): " + blnReturn, 40);
            return blnReturn;
        }
        public virtual bool Credit(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: Credit()", 40);
            m_intEEPGResponseCode = 98012;
            m_strEEPGResponseDescription = "Required function Credit not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: Credit(): " + blnReturn, 40);
            return blnReturn;
        }

        public virtual bool VoidTransaction(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: VoidTransaction()", 40);
            m_intEEPGResponseCode = 98019;
            m_strEEPGResponseDescription = "Required function VoidTransaction not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: VoidTransaction(): " + blnReturn, 40);
            return blnReturn;
        }

        public virtual bool TransactionInfo(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: Transactioninfo()", 40);
            m_intEEPGResponseCode = 98360;
            m_strEEPGResponseDescription = "Required function Transactioninfo not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: Transactioninfo(): " + blnReturn, 40);
            return blnReturn;
        }

        // MPF20140310 BEGIN
        public virtual string Tokenize(string strPlainText, ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            string strReturn = "";
            m_objLog.LogMessage("EEPMGWBase: Tokenize()", 40);
            m_intEEPGResponseCode = 98500;
            m_strEEPGResponseDescription = "Required function Tokenize() not Overridden";
            m_objLog.LogMessage("EEPMGWBase: Tokenize(): " + strReturn, 40);
            return strReturn;
        }
        // MPF20140310 END
        public virtual bool ReAuthTransaction(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: ReAuthTransaction()", 40);
            m_intEEPGResponseCode = 98361;
            m_strEEPGResponseDescription = "Required function ReAuthTransaction not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: ReAuthTransaction(): " + blnReturn, 40);
            return blnReturn;
        }
        // ###################################################################################
        // Public property functions
        // ###################################################################################
        public int GatewayID
        {
            get
            {
                return m_intGatewayID;
            }
        }

        public int ResponseCode
        {
            get
            {
                return m_intEEPGResponseCode;
            }
        }

        public string ResponseDescription
        {
            get
            {
                return m_strEEPGResponseDescription;
            }
        }

        // ###################################################################################
        // Protected functions
        // ###################################################################################
        protected virtual bool SetGatewayCredentials(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: SetGatewayCredentials()", 40);
            m_intEEPGResponseCode = 98006;
            m_strEEPGResponseDescription = "Required function SetGatewayCredentials not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: SetGatewayCredentials(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual bool PrepareGatewayMessage(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: PrepareGatewayMessage()", 40);
            m_intEEPGResponseCode = 98007;
            m_strEEPGResponseDescription = "Required function PrepareGatewayMessage not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: PrepareGatewayMessage(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual bool ReadGatewayResponse(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: ReadGatewayResponse()", 40);
            m_intEEPGResponseCode = 98008;
            m_strEEPGResponseDescription = "Required function ReadGatewayResponse not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: ReadGatewayResponse(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual bool GatewaySpecificMessageSetup(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificMessageSetup(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual bool GatewaySpecificAuthorize(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificAuthorize()", 40);
            m_intEEPGResponseCode = 98020;
            m_strEEPGResponseDescription = "Optional function GatewaySpecificAuthorize not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificAuthorize(): " + blnReturn, 40);
            return blnReturn;
        }
        protected virtual bool GatewaySpecificTransactionDetails(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificTransactionDetails()", 40);
            m_intEEPGResponseCode = 98025;
            m_strEEPGResponseDescription = "Optional function GatewaySpecificTransactionDetails not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificTransactionDetails(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual bool GatewaySpecificCapture(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificCapture()", 40);
            m_intEEPGResponseCode = 98021;
            m_strEEPGResponseDescription = "Optional function GatewaySpecificCapture not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificCapture(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual bool GatewaySpecificDirectSale(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificDirectSale()", 40);
            m_intEEPGResponseCode = 98022;
            m_strEEPGResponseDescription = "Optional function GatewaySpecificDirectSale not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificDirectSale(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual bool GatewaySpecificCredit(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificCredit()", 40);
            m_intEEPGResponseCode = 98023;
            m_strEEPGResponseDescription = "Optional function GatewaySpecificCredit not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificCredit(): " + blnReturn, 40);
            return blnReturn;
        }


        protected virtual bool GatewaySpecificVoidTransaction(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificVoidTransaction()", 40);
            m_intEEPGResponseCode = 98024;
            m_strEEPGResponseDescription = "Optional function GatewaySpecificVoidTransaction not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificVoidTransaction(): " + blnReturn, 40);
            return blnReturn;
        }
        protected virtual bool GatewaySpecificTransactionInfo(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificTransactionInfo()", 40);
            m_intEEPGResponseCode = 98360;
            m_strEEPGResponseDescription = "Optional function GatewaySpecificTransactionInfo not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificTransactionInfo(): " + blnReturn, 40);
            return blnReturn;
        }
        protected virtual bool GatewaySpecificReAuthTransaction(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GGatewaySpecificReAuthTransaction()", 40);
            m_intEEPGResponseCode = 98361;
            m_strEEPGResponseDescription = "Optional function GatewaySpecificReAuthTransaction not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificReAuthTransaction(): " + blnReturn, 40);
            return blnReturn;
        }

        // MPF20140310 START
        protected virtual string GatewaySpecificTokenize(string strPlainText, ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificTokenize()", 40);
            m_intEEPGResponseCode = 98750;
            m_strEEPGResponseDescription = "Function GatewaySpecificTokenize not Overridden";
            blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: GatewaySpecificTokenize(): " + blnReturn, 40);
            return blnReturn.ToString();
        }
        // MPF20140310 END

        protected virtual bool SetSpecialFields(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: SetSpecialFields(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual bool ContainKeyCheck(System.Collections.Generic.Dictionary<object, object> objProperties, string strKey)
        {
            bool blnReturn = false;
            m_objLog.LogMessage("EEPMGWBase: ContainKeyCheck(): " + strKey, 40);
            blnReturn = objProperties.ContainsKey(strKey);
            if ((blnReturn))
            {
                if ((objProperties[strKey] == ""))
                {
                    objProperties.Remove(strKey);
                    blnReturn = false;
                }
            }
            m_objLog.LogMessage("EEPMGWBase: ContainKeyCheck(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual string ProcessKey(ref System.Collections.Generic.Dictionary<object, object> objProperties, string strKey)
        {
            string strReturn = "";
            m_objLog.LogMessage("EEPMGWBase: ProcessKey(): " + strKey, 40);
            if ((objProperties.ContainsKey(strKey)))
            {
                if ((objProperties[strKey] != ""))
                    strReturn = System.Convert.ToString(objProperties[strKey]);
                objProperties.Remove(strKey);
            }
            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
            return strReturn;
        }

        protected virtual bool SetGWObject(string strCase)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPMGWBase: SetGWObject(): " + blnReturn, 40);
            return blnReturn;
        }

        protected virtual void BuildSetGW()
        {
            m_objLog.LogMessage("EEPMGWBase: BuildSetGW()", 40);
            m_objSetGW = new System.Collections.Generic.Dictionary<object, object>();
            m_objSetGW.Add("AUTHORIZE", "");
            m_objSetGW.Add("CAPTURE", "");
            m_objSetGW.Add("DIRECT", "");
            m_objSetGW.Add("CREDIT", "");
            m_objSetGW.Add("VOID", "");
            m_objSetGW.Add("TransactionDetails", "");
            m_objSetGW.Add("Notification", "");
        }

        /* protected virtual string FormatAmount(string strAmount)
         {
             string strReturn = strAmount;
             m_objLog.LogMessage("EEPMGWBase: FormatAmount()" + strReturn, 40);
             strReturn = Strings.Replace(strReturn, " ", "");
             strReturn = Strings.Replace(strReturn, ",", "");
             strReturn = decimal.Round(System.Convert.ToDecimal(strReturn), 2, MidpointRounding.AwayFromZero).ToString();

             if ((Strings.InStr(strReturn, ".") == (Strings.Len(strReturn) - 1)) & (Strings.Len(strReturn) > 1))
                 strReturn = strReturn + "0";
             else if (Strings.InStr(strReturn, ".") <= 0)
                 strReturn = strReturn + ".00";

             m_objLog.LogMessage("EEPMGWBase: FormatAmount()" + strReturn, 40);
             return strReturn;
         } 
         */

        protected virtual string FormatAmount(string strAmount)
        {
            string strReturn = strAmount;
            m_objLog.LogMessage(("EEPMGWBase: FormatAmount()" + strReturn), 40);
            strReturn = strReturn.Replace(" ", "");
            strReturn = strReturn.Replace(",", "");
            strReturn = Decimal.Round(Decimal.Parse(strReturn), 2, MidpointRounding.AwayFromZero).ToString();
            if ((((strReturn.IndexOf(".") + 1) == (strReturn.Length - 1)) && (strReturn.Length > 1)))
            {
                strReturn = (strReturn + "0");
            }
            else
            {
                if (((strReturn.IndexOf(".") + 1)
                            <= 0))
                {
                    strReturn = (strReturn + ".00");
                }

                m_objLog.LogMessage(("EEPMGWBase: FormatAmount()" + strReturn), 40);
            }
            return strReturn;

        }

        // This function is used to scrub log messages that could potentially have a credit card number.
        /*
        protected string ScrubForLog(string strMessage)
        {
            //string strValue = "";

            foreach (var strValue in m_objScrubData)
                // Replace key values set in the BuildScrubList function
                strMessage = Strings.Replace(strMessage, strValue, "xx-replaced-xx");
            // Always replace password
            strMessage = Strings.Replace(strMessage, m_strMerchantPassword, "xx-replaced-xx");
            return strMessage;
        }
        */

        protected string ScrubForLog(string strMessage)
        {
           // string strValue = "";
            foreach (var strValue in m_objScrubData)
            {
                //  Replace key values set in the BuildScrubList function
                strMessage = strMessage.Replace(strValue, "xx-replaced-xx");
            }

            //  Always replace password
            strMessage = strMessage.Replace(m_strMerchantPassword, "xx-replaced-xx");
            return strMessage;
        }
          protected void AddToScrub(ref System.Collections.Generic.Dictionary<object, object> objProperties)
          {
              //string strKey = "";

              foreach (var strKey in m_objScrubList)
              {
                  if ((ContainKeyCheck(objProperties, strKey)))
                  {
                      if (!(m_objScrubData.Contains(objProperties[strKey].ToString())))
                          m_objScrubData.Add(objProperties[strKey].ToString());
                  }
              }
          }
          
     

       

        protected void BuildScrubList()
        {
            m_objLog.LogMessage("EEPMGWBase: BuildScrubList()", 40);
            m_objScrubList = new System.Collections.Generic.List<string>();
            m_objScrubList.Add("CCNUMBER");
            m_objScrubList.Add("EXP_PASSWORD_EXT_1");
            m_objScrubList.Add("EXP_PASSWORD_EXT_2");
            m_objScrubList.Add("EXP_PASSWORD_EXT_3");
            m_objScrubData = new System.Collections.Generic.List<string>();
        }

        // ###################################################################################
        // Protected variables
        // ###################################################################################

        protected int m_intGatewayID; // Gateway ID used to know what gateway to create
        protected string m_strGatewayURL;
        protected string m_strMerchantLogin;
        protected string m_strMerchantPassword;
        protected string m_strMerchantSecurityGUID;

        protected string m_strGatewayResponseCode;
        protected string m_strGatewayResponseRawData;
        protected string m_strGatewayResponseDescription;

        protected int m_intEEPGResponseCode; // Internal response code to calling process.
        protected string m_strEEPGResponseDescription; // Internal response message to calling process.

        protected Enterprise.EELog m_objLog;

        protected System.Collections.Generic.Dictionary<object, object> m_objSetGW;

        protected System.Collections.Generic.List<string> m_objScrubList;
        protected System.Collections.Generic.List<string> m_objScrubData;

        protected eBizChargeGateway m_objNSoftwareGW;
        protected eBizChargeCard m_objNSoftwareCard;
        protected eBizChargeCustomer m_objNSoftwareCustomer;
    }
}
