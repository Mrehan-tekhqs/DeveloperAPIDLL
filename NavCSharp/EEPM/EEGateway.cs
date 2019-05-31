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
using NavCSharp.eBizDevService;

namespace EEPM
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("EF401E1D-FF5B-48BC-B7A7-D11AD55D0C41")]
    //[ProgId("EEPaymentManagerv1")]
   
    
    public partial class EEGateway : Enterprise.EEBase, IEEGateway
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

        public EEGateway()
        {
            m_intGatewayID = -1;
            m_strGatewayURL = "";
            m_strMerchantLogin = "";
            m_strMerchantPassword = "";
            m_intPaymentType = -1;
            m_intResponseCode = -1;
            m_strResponseDescription = "";
            m_objProperties = new System.Collections.Generic.Dictionary<object, object>();
            SetupBase("PCICharge", "EEPM", 2);
            m_objEEPG = null;
        }

        ~EEGateway()
        {
            m_intGatewayID = 0;
            m_strGatewayURL = "";
            m_strMerchantLogin = "";
            m_strMerchantPassword = "";
            m_intPaymentType = 0;
            m_intResponseCode = 0;
            m_strResponseDescription = "";
            m_objProperties = null;
            m_objEEPG = null;
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
        private TransactionObject m_transactionObj;
        
        public TransactionObject TransactionObj
        {
            get
            {
                return m_transactionObj;
            }
            set
            {
                m_transactionObj = value;
            }
        }
        public virtual bool Authorize()
        {
            m_objLog.LogMessage("EEGateway: Authorize()", 40);

            if (!(CheckInternalLicenses(4)))
                return false;
            if (!(CheckReady()))
                return false;
            if (!(DetermineGatewayObject()))
                return false;
            return m_objEEPG.Authorize(ref m_objProperties);
        }

        public virtual bool TransactionDetails()
        {
            m_objLog.LogMessage("EEGateway: TransactionDetails()", 40);

            if (!(CheckInternalLicenses(14, System.Convert.ToInt32(GetNameValue("TRANSACTIONID")))))
                return false;
            if (!(CheckReady()))
                return false;
            if (!(DetermineGatewayObject()))
                return false;
            return m_objEEPG.TransactionDetails(ref m_objProperties);
        }


        public virtual bool Capture()
        {
            m_objLog.LogMessage("EEGateway: Capture()", 40);

            if (!(CheckInternalLicenses(14, System.Convert.ToInt32(GetNameValue("TRANSACTIONAMOUNT")))))
                return false;
            if (!(CheckReady()))
                return false;
            if (!(DetermineGatewayObject()))
                return false;
            return m_objEEPG.Capture(ref m_objProperties);
        }



        public virtual bool DirectSale()
        {
            m_objLog.LogMessage("EEGateway: DirectSale()", 40);

            if (!(CheckInternalLicenses(52, System.Convert.ToInt32(GetNameValue("TRANSACTIONAMOUNT")))))
                return false;
            if (!(CheckReady()))
                return false;
            if (!(DetermineGatewayObject()))
                return false;
            return m_objEEPG.DirectSale(ref m_objProperties);
        }

        public virtual bool Credit()
        {
            m_objLog.LogMessage("EEGateway: Credit()", 40);

            if (!(CheckInternalLicenses(74)))
                return false;
            if (!(CheckReady()))
                return false;
            if (!(DetermineGatewayObject()))
                return false;
            return m_objEEPG.Credit(ref m_objProperties);
        }



        public virtual bool Void()
        {
            m_objLog.LogMessage("EEGateway: Void()", 40);

            if (!(CheckInternalLicenses(83)))
                return false;
            if (!(CheckReady()))
                return false;
            if (!(DetermineGatewayObject()))
                return false;
            return m_objEEPG.VoidTransaction(ref m_objProperties);
        }

        // MPF20140310 START
        public virtual string Tokenize(string strPlainText)
        {
            m_objLog.LogMessage("EEGateway: Tokenize()", 40);

            if (!(CheckInternalLicenses(29)))
                return "";
            if (!(CheckReady()))
                return "";
            if (!(DetermineGatewayObject()))
                return "";



            return m_objEEPG.Tokenize(strPlainText, ref m_objProperties);
        }
        // MPF20140310 END

        public void AddNameValue(string strName, string strValue)
        {
            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
            if ((strName == "!#INSTANCE"))
                m_strProductInstance = strValue;
            else
            {
                if ((m_objProperties.ContainsKey(strName)))
                    m_objProperties.Remove(strName);
                m_objProperties.Add(strName, strValue);
            }
        }

        // Public Sub AddNameValue(ByVal strName As String, ByVal intValue As Integer)
        // 'm_objLog.LogMessage("EEGateway: AddNameValue(): " & strName & "-" & intValue, 40)
        // If (m_objProperties.ContainsKey(strName)) Then m_objProperties.Remove(strName)
        // m_objProperties.Add(strName, intValue)
        // End Sub

        // Public Sub AddNameValue(ByVal strName As String, ByVal dblValue As Double)
        // 'm_objLog.LogMessage("EEGateway: AddNameValue(): " & strName & "-" & dblValue, 40)
        // If (m_objProperties.ContainsKey(strName)) Then m_objProperties.Remove(strName)
        // m_objProperties.Add(strName, dblValue)
        // End Sub

        public string GetNameValue(string strName)
        {
            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
            if ((m_objProperties.ContainsKey(strName)))
                return System.Convert.ToString(m_objProperties[strName]);
            else
                return "";
        }

        public void ClearNameValue()
        {
            m_objLog.LogMessage("EEGateway: ClearNameValue()", 40);
            m_objProperties.Clear();
        }
        public virtual bool Transactioninfo(string refNum)
        {
            m_objLog.LogMessage("EEGateway: Transactioninfo()", 40);

            if (!(CheckInternalLicenses(4)))
                return false;
            // hsard cohe payment type for time being - Ryan
            m_intPaymentType = 0;
            m_intGatewayID = 7;
            if (!(CheckReady()))
                return false;
            if (!(DetermineGatewayObject()))
                return false;

            return m_objEEPG.TransactionInfo(ref m_objProperties);
        }
        public virtual bool ReAuthTransaction(string refNum)
        {
            m_objLog.LogMessage("EEGateway: ReAuthTransaction()", 400);

            if (!(CheckInternalLicenses(4)))
                return false;
            m_intPaymentType = 0;
            m_intGatewayID = 7;
            if (!(CheckReady()))
                return false;
            if (!(DetermineGatewayObject()))
                return false;

            return m_objEEPG.ReAuthTransaction(ref m_objProperties);
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
        protected virtual bool CheckReady()
        {
            bool blnReturn = true;

            m_objLog.LogMessage("EEGateway: CheckReady()", 40);

            // Check if needed information has been setup to process the request
            if ((m_intGatewayID == -1) | (m_intPaymentType == -1))
            {
                m_intResponseCode = 98000;
                m_strResponseDescription = "Needed variable(s) not set: ";
                if ((m_intGatewayID == -1))
                    m_strResponseDescription = m_strResponseDescription + "Gateway ID";
                if ((m_intPaymentType == -1))
                    m_strResponseDescription = m_strResponseDescription + "Payment Type";
                blnReturn = false;
            }

            m_objLog.LogMessage("EEGateway: CheckReady(): " + blnReturn, 40);

            return blnReturn;
        }

        protected virtual bool DetermineGatewayObject()
        {
            bool blnReturn = true;

            m_objLog.LogMessage("EEGateway: DetermineDatewayObject: " + m_intGatewayID + " - " + m_intPaymentType + " - " + m_strGatewaySecurityProfile + ".", 40);

            switch (m_intGatewayID)
            {
                case 7    // eBizCharge
               :
                    {
                        // if ((Strings.InStr("," + System.Convert.ToString(m_intPaymentType) + ",", ",0,")) == 1)
                        //if ((String.IndexOf("," + System.Convert.ToString(m_intPaymentType) + ",", ",0,")) == 1)
                        if(((("," + (System.Convert.ToString(m_intPaymentType) + ",")).IndexOf(",0,") + 1)) == 1)
                        {
                            switch (m_intPaymentType)
                            {
                                case 0:
                                    {
                                        // Dim skc As New SourceKeyChecker()
                                        // If Not skc.CheckSourceKey(m_strMerchantLogin) Then
                                        // m_intResponseCode = 98032
                                        // m_strResponseDescription = skc.LastResponse
                                        // blnReturn = False
                                        // Else
                                        m_objEEPG = new EEPGGWCCeBizCharge(m_intGatewayID, m_strGatewayURL, m_strMerchantLogin, m_strMerchantPassword, m_strMerchantSecurityGUID, ref m_objLog);
                                        break;
                                    }

                                default:
                                    {
                                        errorPaymentTypeUnsupported(m_intPaymentType);
                                        blnReturn = false;
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            errorGatewayUnsupported(m_intGatewayID);
                            blnReturn = false;
                        }

                        break;
                    }

                default:
                    {
                        errorGatewayUnsupported(m_intGatewayID);
                        blnReturn = false;
                        break;
                    }
            }

            m_objLog.LogMessage("EEGateway: DetermineDatewayObject: " + blnReturn, 40);

            return blnReturn;
        }

        // ###################################################################################################################################################################
        // Private Functions
        // ###################################################################################################################################################################

        private void errorGatewayUnsupported(int intGateway)
        {
            m_objLog.LogMessage("EEGateway: errorGatewayUnsupported: " + intGateway, 40);
            try
            {
                m_intResponseCode = 98001;
                m_strResponseDescription = "Gateway " + System.Convert.ToString(intGateway) + " not supported";
            }
            catch
            {
            }
        }

        private void errorPaymentTypeUnsupported(int intPaymentType)
        {
            m_objLog.LogMessage("EEGateway: errorPaymentTypeUnsupported: " + intPaymentType, 40);
            try
            {
                m_intResponseCode = 98002;
                m_strResponseDescription = "Payment type " + System.Convert.ToString(intPaymentType) + " not supported";
            }
            catch
            {
            }
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
        public int GatewayID
        {
            get
            {
                return m_intGatewayID;
            }
            set
            {
                m_intGatewayID = value;
            }
        }

        public int PaymentType
        {
            get
            {
                return m_intPaymentType;
            }
            set
            {
                m_intPaymentType = value;
            }
        }

        public string GatewayURL
        {
            get
            {
                return m_strGatewayURL;
            }
            set
            {
                m_strGatewayURL = value;
            }
        }

        public string GatewayLogin
        {
            get
            {
                return m_strMerchantLogin;
            }
            set
            {
                m_strMerchantLogin = value;
            }
        }

        public string GatewayPassword
        {
            get
            {
                return m_strMerchantPassword;
            }
            set
            {
                m_strMerchantPassword = value;
            }
        }
        public string GatewaySecurityGUID
        {
            get
            {
                return m_strMerchantSecurityGUID;
            }
            set
            {
                m_strMerchantSecurityGUID = value;
            }
        }

        // MPF20140310 BEGIN
        public string GatewaySecurityProfile
        {
            get
            {
                return m_strGatewaySecurityProfile;
            }
            set
            {
                m_strGatewaySecurityProfile = value;
            }
        }
        // MPF20140310 END

        public override int ResponseCode
        {
            get
            {
                if ((m_objEEPG == null))
                    return m_intResponseCode;
                else
                    return m_objEEPG.ResponseCode;
            }
        }

        public override string ResponseDescription
        {
            get
            {
                string strResponse = "";
                if ((m_objEEPG == null))
                    //strResponse = Strings.Replace(m_strResponseDescription, "nsoftware", "PCICharge", 1, Compare: CompareMethod.Text);
                    strResponse = m_strResponseDescription.Replace("nsoftware", "PCICharge");
               //String.Replace(m_strResponseDescription, "nsoftware", "PCICharge", 1, Compare.Text);
                else
                    //strResponse = Strings.Replace(m_objEEPG.ResponseDescription, "nsoftware", "PCICharge", 1, Compare: CompareMethod.Text);
                    strResponse = m_objEEPG.ResponseDescription.Replace("nsoftware", "PCICharge");
                return strResponse;
            }
        }


        // ###################################################################################################################################################################
        // Private Member Variables
        // ###################################################################################################################################################################
        protected EEPGGWCCeBizCharge m_objEEPG; // Gateway object.

        protected int m_intGatewayID; // Gateway ID used to know what gateway to create
        protected int m_intPaymentType; // Payment Type is in terms of 1=CreditCard, 2=ECheck, etc....
        protected string m_strGatewayURL; // Gateway URL
        protected string m_strMerchantLogin; // Gateway Login
        protected string m_strMerchantPassword; // Gateway Password
        protected string m_strMerchantSecurityGUID; // Gateway Security GUID
        protected string m_strGatewaySecurityProfile;  // Security Profile ('Encryption', 'Tokens')  ' MPF20140310        

        protected System.Collections.Generic.Dictionary<object, object> m_objProperties;

        protected const int m_intPaymentTypeCC = 0;
        protected const int m_intPaymentTypeECheck = 0;
    }
}
