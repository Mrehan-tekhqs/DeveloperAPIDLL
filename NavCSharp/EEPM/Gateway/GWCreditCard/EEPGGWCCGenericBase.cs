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
using Newtonsoft.Json;
using System.Threading.Tasks;
//using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using NavCSharp.eBizDevService;

namespace EEPM
{

    // Partial Public Class EEGateway
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class EEPMGWCCGenericBase : EEPMGWBase
    {
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
        // ###################################################################################
        // Constructors\Destructors
        // ###################################################################################
        public EEPMGWCCGenericBase(int intGatewayID, string strGatewayURL, string strMerchantLogin, string strMerchantPassword, string strMerchantSecurityGUID, ref Enterprise.EELog objLog) : base(intGatewayID, strGatewayURL, strMerchantLogin, strMerchantPassword, strMerchantSecurityGUID, ref objLog)
        {
        }

        // ###################################################################################
        // Public functions
        // ###################################################################################
        public override bool Authorize(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;

            m_objLog.LogMessage("Generic: Authorize.");

            AddToScrub(ref objProperties);
            if ((blnReturn))
                blnReturn = SetGWObject("AUTHORIZE");
            if ((blnReturn))
                blnReturn = SetGatewayCredentials(ref objProperties);
            if ((blnReturn))
                blnReturn = PrepareGatewayMessage(ref objProperties);
            if ((blnReturn))
                blnReturn = GatewaySpecificAuthorize(ref objProperties);
            if (!(blnReturn))
            {
                // If the function GatewaySpecificAuthorize has not been overriden then we make the generic call.
                // A non-overridden GatewaySpecificAuthorize is expected to return Error 98020
                if ((m_intEEPGResponseCode == 98020))
                {
                    m_intEEPGResponseCode = -1;
                    m_strEEPGResponseDescription = "";
                    blnReturn = true;
                    if ((blnReturn))
                    {
                        m_objLog.LogMessage("Generic: Authorize Calling.");
                        try
                        {
                            if (ContainKeyCheck(objProperties, "TRANSACTIONAMOUNT"))
                            {
                                m_objNSoftwareGW.TransactionAmount = FormatAmount(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONAMOUNT")));
                                if (ContainKeyCheck(objProperties, "TRANSACTIONDESC"))
                                    m_objNSoftwareGW.TransactionDesc = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDESC"));
                            }

                            if (ContainKeyCheck(objProperties, "INVOICENUMBER"))
                            {
                                m_objNSoftwareGW.InvoiceNumber = System.Convert.ToString(ProcessKey(ref objProperties, "INVOICENUMBER"));
                                m_objLog.LogMessage("INVOICE NUMBER: " + m_objNSoftwareGW.InvoiceNumber);

                                if (ContainKeyCheck(objProperties, "EXTINVOICENUMBER"))
                                {
                                    m_objNSoftwareGW.ExtInvoiceNumber = System.Convert.ToString(ProcessKey(ref objProperties, "EXTINVOICENUMBER"));
                                    m_objLog.LogMessage("EXT INVOICE NUMBER: " + m_objNSoftwareGW.ExtInvoiceNumber);
                                }

                                /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
                                m_objNSoftwareGW.AuthOnly();
                                m_objLog.LogMessage(ScrubForLog("Generic: Authorize. Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                                objProperties.Clear();
                            }
                            else
                            {
                                m_intEEPGResponseCode = 98016;
                                m_strEEPGResponseDescription = "Amount must be specified for an Authorization.";
                                blnReturn = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            // m_intEEPGResponseCode = 98025;
                            // m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                            // m_strEEPGResponseDescription = ("Error:  " + (Information.Err().Number + (" : " + Information.Err().Description)));
                            // m_objLog.LogMessage(ScrubForLog("Generic: Authorize: Exception: Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                            // blnReturn = false;
                            m_intEEPGResponseCode = 98025;
                            m_strEEPGResponseDescription = "Error: " + ex.InnerException; 
                            m_objLog.LogMessage(ScrubForLog(("Generic: Authorize: Exception: Raw Request: " + m_objNSoftwareGW.Config("RawRequest"))), 50);
                            blnReturn = false;
                        }
                    }
                }
            }
            if ((blnReturn))
                blnReturn = ReadGatewayResponse(ref objProperties);
            return blnReturn;
        }
        // ###################################################################################
        // Public functions
        // ###################################################################################
        public override bool TransactionDetails(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;

            m_objLog.LogMessage("Generic: TransactionDetails.");

            AddToScrub(ref objProperties);
            if ((blnReturn))
                blnReturn = SetGWObject("TransactionDetails");
            if ((blnReturn))
                blnReturn = SetGatewayCredentials(ref objProperties);
            if ((blnReturn))
                blnReturn = PrepareGatewayMessage(ref objProperties);
            if ((blnReturn))
                blnReturn = GatewaySpecificAuthorize(ref objProperties);
            if (!(blnReturn))
            {
                // If the function GatewaySpecificAuthorize has not been overriden then we make the generic call.
                // A non-overridden GatewaySpecificAuthorize is expected to return Error 98020
                if ((m_intEEPGResponseCode == 98020))
                {
                    m_intEEPGResponseCode = -1;
                    m_strEEPGResponseDescription = "";
                    blnReturn = true;
                    if ((blnReturn))
                    {
                        m_objLog.LogMessage("Generic: TransactionDetails Calling.");
                        try
                        {
                            if (ContainKeyCheck(objProperties, "TRANSACTIONAMOUNT"))
                            {
                                m_objNSoftwareGW.TransactionAmount = FormatAmount(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONAMOUNT")));
                                if (ContainKeyCheck(objProperties, "TRANSACTIONDESC"))
                                    m_objNSoftwareGW.TransactionDesc = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDESC"));
                                if (ContainKeyCheck(objProperties, "TRANSACTIONDOCUMENTNUMBER"))
                                    m_objNSoftwareGW.InvoiceNumber = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDOCUMENTNUMBER"));
                                /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
                                m_objNSoftwareGW.AuthOnly();
                                m_objLog.LogMessage(ScrubForLog("Generic: Authorize. Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                                objProperties.Clear();
                            }
                            else
                            {
                                m_intEEPGResponseCode = 98016;
                                m_strEEPGResponseDescription = "Amount must be specified for an Authorization.";
                                blnReturn = false;
                            }
                        }
                        catch(Exception ex)
                        {
                            m_intEEPGResponseCode = 98025;
                            //m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                            m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                            m_objLog.LogMessage(ScrubForLog("Generic: Authorize: Exception: Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                            blnReturn = false;
                        }
                    }
                }
            }
            if ((blnReturn))
                blnReturn = ReadGatewayResponse(ref objProperties);
            return blnReturn;
        }

        public override bool Capture(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            string strTransactionID = "";
            string strTransactionAmount = "";
            string strEmailNotificaction = "";
            // Dim strEmailRecipentName As String = ""
            string strEmailRecipentAddress = "";

            m_objLog.LogMessage("EEPMGWCCGenericBase: Capture(): Entering", 40);

            AddToScrub(ref objProperties);
            if ((blnReturn))
                blnReturn = SetGWObject("CAPTURE");
            if ((blnReturn))
                blnReturn = SetGatewayCredentials(ref objProperties);
            if ((blnReturn))
                blnReturn = PrepareGatewayMessage(ref objProperties);
            if ((blnReturn))
                blnReturn = GatewaySpecificCapture(ref objProperties);
            m_objLog.LogMessage("CAPTURE: Set GWObject: " + blnReturn, 40);
            if (!(blnReturn))
            {
                // If the function GatewaySpecificCapture has not been overriden then we make the generic call.
                // A non-overridden GatewaySpecificCapture is expected to return Error 98021
                if ((m_intEEPGResponseCode == 98021))
                {
                    m_intEEPGResponseCode = -1;
                    m_strEEPGResponseDescription = "";
                    blnReturn = true;
                    if ((blnReturn))
                    {
                        try
                        {
                            if (ContainKeyCheck(objProperties, "TRANSACTIONAMOUNT"))
                            {
                                strTransactionAmount = FormatAmount(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONAMOUNT")));

                                if (ContainKeyCheck(objProperties, "TRANSACTIONID"))
                                {
                                    strTransactionID = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONID"));

                                    if (ContainKeyCheck(objProperties, "TRANSACTIONDESC"))
                                        m_objNSoftwareGW.TransactionDesc = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDESC"));



                                    // If ContainKeyCheck(objProperties, "TRANSACTIONDOCUMENTNUMBER") Then
                                    // m_objNSoftwareGW.InvoiceNumber = CStr(ProcessKey(objProperties, "TRANSACTIONDOCUMENTNUMBER"))
                                    // End If

                                    if (ContainKeyCheck(objProperties, "TRANSACTIONORDERNO"))
                                    {
                                        m_objNSoftwareGW.InvoiceNumber = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONORDERNO"));
                                        m_objLog.LogMessage("iNVOICE nUMBER: " + m_objNSoftwareGW.InvoiceNumber);
                                    }





                                    /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
                                    m_objNSoftwareGW.Capture(strTransactionID, strTransactionAmount);
                                    m_objLog.LogMessage(ScrubForLog("Generic: Capture. Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);

                                    objProperties.Clear();
                                }
                                else
                                {
                                    m_intEEPGResponseCode = 98018;
                                    m_strEEPGResponseDescription = "Transaction ID must be specified for an Capture.";
                                    blnReturn = false;
                                }
                            }
                            else
                            {
                                m_intEEPGResponseCode = 98017;
                                m_strEEPGResponseDescription = "Amount must be specified for an Capture.";
                                blnReturn = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            m_intEEPGResponseCode = 98026;
                            //m_strEEPGResponseDescription = Information.Err().Number + " : " + Information.Err().Description;
                            m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                            m_objLog.LogMessage(ScrubForLog("EEPMGWCCGenericBase: Capture: Exception: URL: " + System.Convert.ToString(m_objNSoftwareGW.GatewayURL)), 50);
                            m_objLog.LogMessage(ScrubForLog("EEPMGWCCGenericBase: Capture: Exception: Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                            blnReturn = false;
                        }
                    }
                }
            }

            if ((blnReturn))
                blnReturn = ReadGatewayResponse(ref objProperties);
            // //EE13.0.40 Email Notification >>
            string ResponseTransactionID = "";

            if (ContainKeyCheck(objProperties, "TRANSACTIONID"))
                ResponseTransactionID = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONID"));

            if (ContainKeyCheck(objProperties, "EMAILNOTIFICATION"))
                strEmailNotificaction = System.Convert.ToString(ProcessKey(ref objProperties, "EMAILNOTIFICATION"));

            if (ContainKeyCheck(objProperties, "EMAILRECIPIENTADDRESS"))
                strEmailRecipentAddress = System.Convert.ToString(ProcessKey(ref objProperties, "EMAILRECIPIENTADDRESS"));

            // m_objLog.LogMessage("EEPMGWCCGenericBase: Capture(): SendEmailReceiptByValue Rehan-->: " & ResponseTransactionID, 40)

            if (m_objNSoftwareGW != null)
            {
                if ((blnReturn))
                    m_objNSoftwareGW.SendEmailReceiptByValue(ResponseTransactionID, strEmailNotificaction, strEmailRecipentAddress);
            }

            // m_objLog.LogMessage("EEPMGWCCGenericBase: Capture(): Exiting: " & blnReturn, 40)
            return blnReturn;
        }

        public override bool DirectSale(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            string strTransactionAmount = "";

            AddToScrub(ref objProperties);
            if ((blnReturn))
                blnReturn = SetGWObject("DIRECT");
            if ((blnReturn))
                blnReturn = SetGatewayCredentials(ref objProperties);
            if ((blnReturn))
                blnReturn = PrepareGatewayMessage(ref objProperties);
            if ((blnReturn))
                blnReturn = GatewaySpecificDirectSale(ref objProperties);
            if (!(blnReturn))
            {
                // If the function GatewaySpecificDirectSale has not been overriden then we make the generic call.
                // A non-overridden GatewaySpecificDirectSale is expected to return Error 98022
                if ((m_intEEPGResponseCode == 98022))
                {
                    m_intEEPGResponseCode = -1;
                    m_strEEPGResponseDescription = "";
                    blnReturn = true;
                    if ((blnReturn))
                    {
                        try
                        {
                            if (ContainKeyCheck(objProperties, "TRANSACTIONAMOUNT"))
                            {
                                m_objNSoftwareGW.TransactionAmount = FormatAmount(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONAMOUNT")));
                                if (ContainKeyCheck(objProperties, "TRANSACTIONDESC"))
                                    m_objNSoftwareGW.TransactionDesc = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDESC"));
                                if (ContainKeyCheck(objProperties, "TRANSACTIONDOCUMENTNUMBER"))
                                    m_objNSoftwareGW.InvoiceNumber = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDOCUMENTNUMBER"));
                                // //EE13.0.40 Email Notification >>
                                if (ContainKeyCheck(objProperties, "EMAILNOTIFICATION"))
                                    m_objNSoftwareGW.EmailNotificaction = System.Convert.ToString(ProcessKey(ref objProperties, "EMAILNOTIFICATION"));
                                if (ContainKeyCheck(objProperties, "EMAILRECIPIENTADDRESS"))
                                    m_objNSoftwareGW.EmailRecipentAddress = System.Convert.ToString(ProcessKey(ref objProperties, "EMAILRECIPIENTADDRESS"));
                                if (ContainKeyCheck(objProperties, "EMAILRECIPIENTADDRESS"))
                                    m_objNSoftwareGW.EmailRecipentAddress = System.Convert.ToString(ProcessKey(ref objProperties, "EMAILRECIPIENTADDRESS"));

                                // m_objNSoftwareGW.UpdateCard(CStr(ProcessKey(objProperties, "CCNUMBER")))


                                // //EE13.0.40 Email Notification >>
                                /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
                                m_objNSoftwareGW.Sale();
                                m_objLog.LogMessage(ScrubForLog("Generic: DirectSale. Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                                objProperties.Clear();
                            }
                            else
                            {
                                m_intEEPGResponseCode = 98027;
                                m_strEEPGResponseDescription = "Amount must be specified for an Direct Sale.";
                                blnReturn = false;
                            }
                        }
                        catch(Exception ex)
                        {
                            m_intEEPGResponseCode = 98028;
                            m_strEEPGResponseDescription = "Error1:  " + " : " + ex.Message;
                            m_strEEPGResponseDescription = "Error2: " + ex.InnerException;
                            // m_objLog.LogMessage("Generic: DirectSale. Exception: " + Information.Err().Number + " : " + Information.Err().Description, 1);
                            m_objLog.LogMessage("Error: " + ex.InnerException);
                            m_objLog.LogMessage(ScrubForLog("Generic: DirectSale. Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                            blnReturn = false;
                        }
                    }
                }
            }
            if ((blnReturn))
                blnReturn = ReadGatewayResponse(ref objProperties);
            // //EE13.0.40 Email Notification >>
            string DsaleTransactionID = "";
            if (ContainKeyCheck(objProperties, "TRANSACTIONID"))
                DsaleTransactionID = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONID"));
            objProperties.Add("TRANSACTIONID", DsaleTransactionID);
            if (m_objNSoftwareGW != null)
            {
                if ((blnReturn))
                    m_objNSoftwareGW.SendEmailReceiptByValue(DsaleTransactionID, m_objNSoftwareGW.EmailNotificaction, m_objNSoftwareGW.EmailRecipentAddress);
            }
            // //EE13.0.40 Email Notification >>
            return blnReturn;
        }


        public override bool Credit(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            string strTransactionID = "";
            string strTransactionAmount = "";

            AddToScrub(ref objProperties);
            if ((blnReturn))
                blnReturn = SetGWObject("CREDIT");
            if ((blnReturn))
                blnReturn = SetGatewayCredentials(ref objProperties);
            if ((blnReturn))
                blnReturn = PrepareGatewayMessage(ref objProperties);
            if ((blnReturn))
                blnReturn = GatewaySpecificCredit(ref objProperties);
            if (!(blnReturn))
            {
                // If the function GatewaySpecificCredit has not been overriden then we make the generic call.
                // A non-overridden GatewaySpecificCredit is expected to return Error 98023
                if ((m_intEEPGResponseCode == 98023))
                {
                    m_intEEPGResponseCode = -1;
                    m_strEEPGResponseDescription = "";
                    blnReturn = true;
                    if ((blnReturn))
                    {
                        try
                        {
                            if (ContainKeyCheck(objProperties, "TRANSACTIONAMOUNT"))
                            {
                                strTransactionAmount = FormatAmount(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONAMOUNT")));
                                if (ContainKeyCheck(objProperties, "TRANSACTIONID"))
                                {
                                    strTransactionID = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONID"));
                                    if (ContainKeyCheck(objProperties, "TRANSACTIONDESC"))
                                        m_objNSoftwareGW.TransactionDesc = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDESC"));
                                    if (ContainKeyCheck(objProperties, "TRANSACTIONDOCUMENTNUMBER"))
                                        m_objNSoftwareGW.InvoiceNumber = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDOCUMENTNUMBER"));
                                    // //EE13.0.40 Email Notification >>
                                    if (ContainKeyCheck(objProperties, "EMAILNOTIFICATION"))
                                        m_objNSoftwareGW.EmailNotificaction = System.Convert.ToString(ProcessKey(ref objProperties, "EMAILNOTIFICATION"));
                                    if (ContainKeyCheck(objProperties, "EMAILRECIPIENTADDRESS"))
                                        m_objNSoftwareGW.EmailRecipentAddress = System.Convert.ToString(ProcessKey(ref objProperties, "EMAILRECIPIENTADDRESS"));
                                    // //EE13.0.40 Email Notification >>
                                    /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
                                    m_objNSoftwareGW.Refund(strTransactionID, strTransactionAmount);
                                    m_objLog.LogMessage(ScrubForLog("Generic: Credit: Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                                    objProperties.Clear();
                                }
                                else
                                {
                                    m_intEEPGResponseCode = 98030;
                                    m_strEEPGResponseDescription = "Transaction ID must be specified for a Credit.";
                                    blnReturn = false;
                                }
                            }
                            else
                            {
                                m_intEEPGResponseCode = 98029;
                                m_strEEPGResponseDescription = "Amount must be specified for a Credit.";
                                blnReturn = false;
                            }
                        }
                        catch(Exception ex)
                        {
                            m_intEEPGResponseCode = 98031;
                            //  m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                            m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                            m_objLog.LogMessage(ScrubForLog("Generic: Credit: Exception: Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                            blnReturn = false;
                        }
                    }
                }
            }
            if ((blnReturn))
                blnReturn = ReadGatewayResponse(ref objProperties);
            // //EE13.0.40 Email Notification >>
            string RefundTransactionID = "";
            if (ContainKeyCheck(objProperties, "TRANSACTIONID"))
                RefundTransactionID = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONID"));
            objProperties.Add("TRANSACTIONID", RefundTransactionID);
            if (m_objNSoftwareGW != null)
            {
                if ((blnReturn))
                    m_objNSoftwareGW.SendEmailReceiptByValue(RefundTransactionID, m_objNSoftwareGW.EmailNotificaction, m_objNSoftwareGW.EmailRecipentAddress);
            }
            return blnReturn;
        }

        public override bool VoidTransaction(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            string strTransactionID = "";
            string strTransactionAmount = "";

            AddToScrub(ref objProperties);
            if ((blnReturn))
                blnReturn = SetGWObject("VOID");
            if ((blnReturn))
                blnReturn = SetGatewayCredentials(ref objProperties);
            if ((blnReturn))
                blnReturn = PrepareGatewayMessage(ref objProperties);
            if ((blnReturn))
                blnReturn = GatewaySpecificVoidTransaction(ref objProperties);
            if (!(blnReturn))
            {
                // If the function GatewaySpecificVoidTransaction has not been overriden then we make the generic call.
                // A non-overridden GatewaySpecificVoidTransaction is expected to return Error 98024
                if ((m_intEEPGResponseCode == 98024))
                {
                    m_intEEPGResponseCode = -1;
                    m_strEEPGResponseDescription = "";
                    blnReturn = true;
                    if ((blnReturn))
                    {
                        try
                        {
                            if (ContainKeyCheck(objProperties, "TRANSACTIONID"))
                            {
                                strTransactionID = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONID"));
                                if (ContainKeyCheck(objProperties, "TRANSACTIONAMOUNT"))
                                    m_objNSoftwareGW.TransactionAmount = FormatAmount(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONAMOUNT")));
                                if (ContainKeyCheck(objProperties, "TRANSACTIONDESC"))
                                    m_objNSoftwareGW.TransactionDesc = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDESC"));
                                if (ContainKeyCheck(objProperties, "TRANSACTIONDOCUMENTNUMBER"))
                                    m_objNSoftwareGW.InvoiceNumber = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONDOCUMENTNUMBER"));
                                /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
                                m_objNSoftwareGW.VoidTransaction(strTransactionID);
                                m_objLog.LogMessage(ScrubForLog("Generic: Void: Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                                objProperties.Clear();
                            }
                            else
                            {
                                m_intEEPGResponseCode = 98048;
                                m_strEEPGResponseDescription = "Transaction ID must be specified for a Void.";
                                blnReturn = false;
                            }
                        }
                        catch(Exception ex)
                        {
                            m_intEEPGResponseCode = 98049;
                            // m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                            m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                            m_objLog.LogMessage(ScrubForLog("Generic: Void: Exception: Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                            blnReturn = false;
                        }
                    }
                }
            }
            if ((blnReturn))
                blnReturn = ReadGatewayResponse(ref objProperties);
            return blnReturn;
        }



        public override bool TransactionInfo(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            string strTransactionID = "";

            // Dim strTransactionAmount As String = ""

            AddToScrub(ref objProperties);
            if ((blnReturn))
                blnReturn = SetGWObject("TRANSACTIONINFO");
            if ((blnReturn))
                blnReturn = SetGatewayCredentials(ref objProperties);
            if ((blnReturn))
                blnReturn = PrepareGatewayMessage(ref objProperties); // ' Need to comment this code Ryan
            if ((blnReturn))
                blnReturn = GatewaySpecificTransactionInfo(ref objProperties);

            if (!(blnReturn))
            {
                // If the function GatewaySpecificVoidTransaction has not been overriden then we make the generic call.
                // A non-overridden GatewaySpecificVoidTransaction is expected to return Error 98024
                if ((m_intEEPGResponseCode == 98360))
                {
                    m_intEEPGResponseCode = -1;
                    m_strEEPGResponseDescription = "";
                    blnReturn = true;
                    if ((blnReturn))
                    {
                        try
                        {
                            if (ContainKeyCheck(objProperties, "TRANSACTIONID"))
                            {
                                strTransactionID = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONID"));


                                /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
                                m_objNSoftwareGW.TransactionInfo(strTransactionID);
                                m_transactionObj = m_objNSoftwareGW.TransactionObj;
                                // m_objLog.LogMessage(ScrubForLog("Generic: TransactionInfo: Raw Request: " & m_objNSoftwareGW.Config("RawRequest")), 50)
                                objProperties.Clear();

                                PaymentMethodProfile[] paymentMethods;

                                // Dim custNum = m_objNSoftwareGW.GetCustomer(m_transactionObj.CustomerID)
                                m_objLog.LogMessage("createing customer");
                                var custNum = AddUpdateCustomer(objProperties, m_transactionObj.CustomerID);
                                // m_objLog.LogMessage("customer created" + custNum)




                                // m_objNSoftwareGW.AddCustomer()

                                // m_objLog.LogMessage("Generic: CustNum : " & custNum, 35)

                                paymentMethods = m_objNSoftwareGW.GetCustomerPaymentMethodProfile(custNum);

                                string paymentMethodId = "";
                                string cardExpMonth;
                                string cardExpYear;
                                string CCNumber;

                                CCNumber = m_transactionObj.CreditCardData.CardNumber;

                                if (paymentMethods.Length == 0)
                                    paymentMethodId = "0";

                                cardExpMonth = DateTime.Now.Month.ToString();
                                cardExpYear = DateTime.Now.AddYears(5).Year.ToString();

                                //NavCSharp.eBizCharge.PaymentMethod paymentmethod;

                                foreach (var paymentMethod in paymentMethods)
                                {
                                    // Replace key values set in the BuildScrubList function
                                    /* if ((Strings.Right(paymentMethod.CardNumber, 4) == Strings.Right(m_transactionObj.CreditCardData.CardNumber, 4)))
                                     {
                                         string[] strCardExp;
                                         strCardExp = paymentMethod.CardExpiration.Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                         if ((strCardExp.Length >= 2))
                                         {
                                             cardExpMonth = strCardExp[1];
                                             cardExpYear = strCardExp[0];
                                         }
                                         CCNumber = paymentMethod.CardNumber;
                                         paymentMethodId = paymentMethod.MethodID;
                                     }
                                 }
                                 */

                                    if ((paymentMethod.CardNumber.Substring((paymentMethod.CardNumber.Length - 4)) == m_transactionObj.CreditCardData.CardNumber.Substring((m_transactionObj.CreditCardData.CardNumber.Length - 4))))
                                    {
                                        string[] strCardExp;

                                        strCardExp = paymentMethod.CardExpiration.Split('-');
                                        if ((strCardExp.Length >= 2))
                                        {
                                            cardExpMonth = strCardExp[1];
                                            cardExpYear = strCardExp[0];
                                        }

                                        CCNumber = paymentMethod.CardNumber;
                                        paymentMethodId = paymentMethod.MethodID;
                                    }
                                }

                                    objProperties = new Dictionary<object, object>();

                                // objProperties.Add("PAYMENTGUID", m_transactionObj.p
                                objProperties.Add("STATUS", m_transactionObj.Status);
                                // objProperties.Add("EISUSERGUID", m_transactionObj.
                                // objProperties.Add("ENTERPRISEINSTANCE", m_transactionObj.
                                string tempDateTime;
                                tempDateTime = Convert.ToDateTime(m_transactionObj.DateTime).ToString("MM/dd/yy hh:mm:ss");

                                objProperties.Add("PAYMENTDATETIME", tempDateTime);
                                objProperties.Add("CUSTOMERREFERENCENO.", m_transactionObj.CustomerID);

                                // objProperties.Add("DOCUMENTGUID", m_transactionObj.do
                                // objProperties.Add("LEDGERPAYMENT", m_transactionObj.
                                // objProperties.Add("PAYMENTTYPE", m_transactionObj.CreditCardData.CardType)
                                // objProperties.Add("TRANSACTIONSIGNATURE", m_transactionObj.

                                objProperties.Add("TRANSACTIONID", strTransactionID);



                                CCNumber = CCNumber.Replace("XXXXXXXXXXXX", "XXXXXXXXXXXX-");

                                objProperties.Add("ENCRYPTEDCREDITCARDMASK", CCNumber);



                                objProperties.Add("ENCRYPTEDCREDITCARDTYPE", m_transactionObj.CreditCardData.CardType);
                                // do a call to Tokenize function with card number, expiration date and month and get the encrypte string and pass it below

                                objProperties.Add("ENCRYPTEDCREDITCARDNO.", custNum + "~" + paymentMethodId);
                                objProperties.Add("TRANSACTIONAMOUNT", m_transactionObj.Details.Amount);
                                objProperties.Add("CREDITCARDEXPIRATIONMONTH", cardExpMonth);
                                objProperties.Add("CREDITCARDEXPIRATIONYEAR", cardExpYear);


                                // objProperties.Add("BILL-TOADDRESSNUMBER", m_transactionObj.BillingAddress.
                                objProperties.Add("BILL-TOADDRESS", m_transactionObj.BillingAddress.Street);
                                objProperties.Add("BILL-TOADDRESS2", m_transactionObj.BillingAddress.Street2);
                                objProperties.Add("BILL-TOCITY", m_transactionObj.BillingAddress.City);
                                objProperties.Add("BILL-TOSTATE", m_transactionObj.BillingAddress.State);
                                objProperties.Add("BILL-TOZIP", m_transactionObj.BillingAddress.Zip);
                                objProperties.Add("BILL-TOCOUNTRYCODE", m_transactionObj.BillingAddress.Country);
                                objProperties.Add("BILL-TOPHONE", m_transactionObj.BillingAddress.Phone);
                                objProperties.Add("BILL-TOEMAIL", m_transactionObj.BillingAddress.Email);
                                objProperties.Add("FIRSTNAME", m_transactionObj.BillingAddress.FirstName);
                                objProperties.Add("LASTNAME", m_transactionObj.BillingAddress.LastName);
                                objProperties.Add("YOURCUSTOMERREFERENCE", m_transactionObj.Details.PONum);
                                // objProperties.Add("BANKACCOUNTNO.", m_transactionObj.
                                // objProperties.Add("BANKACCOUNTOWNER", m_transactionObj.
                                // objProperties.Add("BANKREG.NO.", m_transactionObj.

                                blnReturn = true;
                            }
                            else
                            {
                                m_intEEPGResponseCode = 98048;
                                m_strEEPGResponseDescription = "Transaction ID must be specified for TransactionInfo.";
                                blnReturn = false;
                            }
                        }
                        catch(Exception ex)
                        {
                            m_intEEPGResponseCode = 98049;
                            //  m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                            m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                            m_objLog.LogMessage(ScrubForLog("Generic: TransactionInfo: Exception: Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                            blnReturn = false;
                        }
                    }
                }
            }

            return blnReturn;
        }
        // MPF20140310 BEGIN
        public override bool ReAuthTransaction(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            string strTransactionID = "";
            string strTransactionAmount = "";
            // Dim strTransactionAmount As String = ""

            AddToScrub(ref objProperties);
            if ((blnReturn))
                blnReturn = SetGWObject("REAUTHTRANSACTION");
            if ((blnReturn))
                blnReturn = SetGatewayCredentials(ref objProperties);
            if ((blnReturn))
                blnReturn = PrepareGatewayMessage(ref objProperties); // ' Need to comment this code Ryan
            if ((blnReturn))
                blnReturn = GatewaySpecificReAuthTransaction(ref objProperties);

            if (!(blnReturn))
            {
                // If the function GatewaySpecificVoidTransaction has not been overriden then we make the generic call.
                // A non-overridden GatewaySpecificVoidTransaction is expected to return Error 98024
                if ((m_intEEPGResponseCode == 98361))
                {
                    m_intEEPGResponseCode = -1;
                    m_strEEPGResponseDescription = "";
                    try
                    {
                        if (ContainKeyCheck(objProperties, "TRANSACTIONID"))
                        {
                            strTransactionID = System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONID"));

                            if (ContainKeyCheck(objProperties, "TRANSACTIONAMOUNT"))
                                strTransactionAmount = FormatAmount(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONAMOUNT")));


                            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
                            m_objLog.LogMessage("Doing ReAuth");

                            double amount = 0.0;

                            amount = System.Convert.ToDouble(strTransactionAmount);

                            m_objNSoftwareGW.ReAuthTransaction(strTransactionID, amount);
                            m_transactionObj = m_objNSoftwareGW.TransactionObj;
                            objProperties.Clear();

                            m_objLog.LogMessage("ReAuth Completed");

                            blnReturn = true;
                        }
                        else
                        {
                            m_intEEPGResponseCode = 98048;
                            m_strEEPGResponseDescription = "Transaction ID must be specified for ReAuthTransaction.";
                            blnReturn = false;
                        }
                    }
                    catch(Exception ex)
                    {
                        m_intEEPGResponseCode = 98048;
                        // m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                        m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                        m_objLog.LogMessage(ScrubForLog("Generic: ReAuth: Exception: Raw Request: " + m_objNSoftwareGW.Config("RawRequest")), 50);
                        blnReturn = false;
                    }
                }
            }

            if ((blnReturn))
                blnReturn = ReadGatewayResponse(ref objProperties);
            return blnReturn;
        }

        public string AddUpdateCustomer(System.Collections.Generic.Dictionary<object, object> objProperties, string custid)
        {
            m_objLog.LogMessage("adding customer before Try-CustID" + custid);
            string custNum = "";
            try
            {
                custNum = m_objNSoftwareGW.GetCustomer(custid);
            }
            catch (Exception ex)
            {
                m_objLog.LogMessage("adding customer in Try-CustID" + custid);

                Customer customerData = new Customer();
                customerData.CustomerId = custid;

                Address address = new Address();
                address.FirstName = "Web";
                address.LastName = "Order";
                address.CompanyName  = "Social Studies School Services";
                address.Address1 = "10200 Jefferson Blvd.";
                address.City = "Culver City";
                address.State = "CA";
                address.Country = "USA";
                address.ZipCode = "90232";
                customerData.CellPhone = "8004214246";
                customerData.Email = "customerservice@socialstudies.com";
                customerData.BillingAddress = address;

                try
                {
                    m_objLog.LogMessage("SaveCard(): adding customer after manual init...");
                    custNum = m_objNSoftwareGW.AddCustomer(customerData);
                    m_objLog.LogMessage("SaveCard(): successfully added customer.");
                }
                catch (Exception ex2)
                {
                    m_objLog.LogMessage("Error" + ex2.Message.ToString() + " **** " + ex2.InnerException.ToString());
                }

                finally
                {
                    m_objLog.LogMessage("SaveCard(): addCustomer failed");
                }
            }

            return custNum;
        }

        public string AddUpdateCustomerBackup(System.Collections.Generic.Dictionary<object, object> objProperties, string custid)
        {
            var custNum = m_objNSoftwareGW.GetCustomer(custid);

            if (custNum == string.Empty)
            {
                Customer customerData = new Customer();

                PaymentMethodProfile[] payMethod = new PaymentMethodProfile[1];

                payMethod[0] = new PaymentMethodProfile();
                payMethod[0].CardExpiration = "1299";
                payMethod[0].CardNumber = "4444555566667779";

                customerData.PaymentMethodProfiles = payMethod;

                customerData.CustomerId = custid;
                try
                {
                    m_objLog.LogMessage("SaveCard(): adding customer...");
                    custNum = m_objNSoftwareGW.AddCustomer(customerData);
                    m_objLog.LogMessage("SaveCard(): successfully added customer.");
                }
                catch (Exception ex)
                {
                    m_objLog.LogMessage("Error" + ex.Message.ToString() + " **** " + ex.InnerException.ToString());
                }

                finally
                {
                    m_objLog.LogMessage("SaveCard(): addCustomer failed");
                }
            }

            return custNum;
        }

        public override string Tokenize(string strPlainText, ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            string strReturn = "";
            bool blnContinue = true;
            m_objLog.LogMessage("EEPMGWCCGenericBase: Tokenize(): Entering", 40);

            AddToScrub(ref objProperties);
            if ((blnContinue)) 
                blnContinue = SetGWObject("TOKENIZE");
            if ((blnContinue))
                blnContinue = SetGatewayCredentials(ref objProperties);
            if ((blnContinue))
                blnContinue = PrepareGatewayMessage(ref objProperties);
            if ((blnContinue))
                strReturn = GatewaySpecificTokenize(strPlainText, ref objProperties);

            m_objLog.LogMessage("EEPMGWCCGenericBase: Tokenize(): Exiting: " + strReturn, 40);
            return strReturn;
        }
        // MPF20140310 END

        // ###################################################################################
        // Protected functions
        // ###################################################################################
        protected override bool SetGatewayCredentials(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;
            try
            {
                m_objNSoftwareGW.Gateway = m_intGatewayID.ToString();
                // Allow for a special key to change the default gateway URL.
                if ((ContainKeyCheck(objProperties, "!#GATEWAY")))
                    m_objNSoftwareGW.GatewayURL = System.Convert.ToString(ProcessKey(ref objProperties, "!#GATEWAY"));
                else if ((m_strGatewayURL != ""))
                    m_objNSoftwareGW.GatewayURL = m_strGatewayURL;
                m_objNSoftwareGW.MerchantLogin = m_strMerchantLogin;
                m_objNSoftwareGW.MerchantPassword = m_strMerchantPassword;
                m_objNSoftwareGW.MerchantSecurityGUID = m_strMerchantSecurityGUID;
            }
            catch(Exception ex)
            {
                m_intEEPGResponseCode = 98003;
                // m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                blnReturn = false;
            }
            return blnReturn;
        }

        protected override bool PrepareGatewayMessage(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;

            m_objLog.LogMessage("Generic: PrepareGatewayMessage.", 40);

            // Build the card object
            try
            {
                if (ContainKeyCheck(objProperties, "CCTYPE"))
                    m_objNSoftwareCard.CardType = ProcessKey(ref objProperties, "CCTYPE");
                if (ContainKeyCheck(objProperties, "CCNUMBER"))
                    m_objNSoftwareCard.Number = System.Convert.ToString(ProcessKey(ref objProperties, "CCNUMBER"));
                if (ContainKeyCheck(objProperties, "CCEXPMONTH"))
                    m_objNSoftwareCard.ExpMonth = ProcessKey(ref objProperties, "CCEXPMONTH");
                if (ContainKeyCheck(objProperties, "CCEXPYEAR"))
                    m_objNSoftwareCard.ExpYear = ProcessKey(ref objProperties, "CCEXPYEAR");
                if (ContainKeyCheck(objProperties, "CCCCV"))
                    m_objNSoftwareCard.CVVData = System.Convert.ToString(ProcessKey(ref objProperties, "CCCCV"));
                m_objNSoftwareGW.Card = m_objNSoftwareCard;
            }
            catch(Exception ex)
            {
                m_intEEPGResponseCode = 98004;
                //m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                blnReturn = false;
            }
            // Build the customer object
            try
            {
                if (ContainKeyCheck(objProperties, "FNAME"))
                    m_objNSoftwareCustomer.FirstName = System.Convert.ToString(ProcessKey(ref objProperties, "FNAME"));
                if (ContainKeyCheck(objProperties, "LNAME"))
                    m_objNSoftwareCustomer.LastName = System.Convert.ToString(ProcessKey(ref objProperties, "LNAME"));
                m_objLog.LogMessage("Generic: PrepareGatewayMessage. CustomerFirstName:" + m_objNSoftwareCustomer.FirstName, 40);
                m_objLog.LogMessage("Generic: PrepareGatewayMessage. CustomerFirstName:" + m_objNSoftwareCustomer.LastName, 40);
                // Address number and Adress passed in seperately because certain gateways will require this.  They can override this function.
                // Address2 might also be passed, but there is no place for it in the default Customer object.  To use this override this function
                if ((ContainKeyCheck(objProperties, "ADDRESSNUMBER")) & (ContainKeyCheck(objProperties, "ADDRESS")))
                    m_objNSoftwareCustomer.Address = System.Convert.ToString(ProcessKey(ref objProperties, "ADDRESSNUMBER")) + " " + System.Convert.ToString(ProcessKey(ref objProperties, "ADDRESS"));
                if (ContainKeyCheck(objProperties, "CITY"))
                    m_objNSoftwareCustomer.City = System.Convert.ToString(ProcessKey(ref objProperties, "CITY"));
                if (ContainKeyCheck(objProperties, "STATE"))
                    m_objNSoftwareCustomer.State = System.Convert.ToString(ProcessKey(ref objProperties, "STATE"));
                if (ContainKeyCheck(objProperties, "ZIPCODE"))
                    m_objNSoftwareCustomer.Zip = System.Convert.ToString(ProcessKey(ref objProperties, "ZIPCODE"));
                if (ContainKeyCheck(objProperties, "COUNTRYCODE"))
                    m_objNSoftwareCustomer.Country = System.Convert.ToString(ProcessKey(ref objProperties, "COUNTRYCODE"));
                if (ContainKeyCheck(objProperties, "PHONE"))
                    m_objNSoftwareCustomer.Phone = System.Convert.ToString(ProcessKey(ref objProperties, "PHONE"));
                if (ContainKeyCheck(objProperties, "EMAIL"))
                    m_objNSoftwareCustomer.Email = System.Convert.ToString(ProcessKey(ref objProperties, "EMAIL"));
                if (ContainKeyCheck(objProperties, "CUSTOMERNUMBER"))
                    m_objNSoftwareCustomer.Id = System.Convert.ToString(ProcessKey(ref objProperties, "CUSTOMERNUMBER"));
                // If ContainKeyCheck(objProperties, "PAYMENTMETHOD") Then m_objNSoftwareCustomer.PaymentMethodId = CStr(ProcessKey(objProperties, "PAYMENTMETHOD"))

                // m_objLog.LogMessage("Generic: PrepareGatewayMessage PAYMENTMETHOD: " & m_objNSoftwareCustomer.PaymentMethodId, 40)

                m_objNSoftwareGW.Customer = m_objNSoftwareCustomer;

                // Update Customer Name
                var result = m_objNSoftwareGW.UpdateCard(m_objNSoftwareGW.Card.Number, m_objNSoftwareCustomer.FirstName, m_objNSoftwareCustomer.LastName);
            }
            catch(Exception ex)
            {
                m_intEEPGResponseCode = 98005;
                // m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                blnReturn = false;
            }


            blnReturn = GatewaySpecificMessageSetup(ref objProperties);
            blnReturn = SetSpecialFields(ref objProperties);

            m_objLog.LogMessage("Generic: PrepareGatewayMessage : " + blnReturn, 40);

            return blnReturn;
        }


        protected override bool ReadGatewayResponse(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;

            m_objLog.LogMessage("Generic: ReadGatewayResponse.", 40);

            try
            {
                CustomGWResponse objNSoftwareResponse = m_objNSoftwareGW.Response;

                m_objLog.LogMessage("Generic: Response.Data: " + objNSoftwareResponse.Data, 35);

                if (!(objNSoftwareResponse.Approved))
                {
                    // The two following variables are included to log someday
                    m_strGatewayResponseCode = objNSoftwareResponse.ErrorCode;
                    m_strGatewayResponseRawData = objNSoftwareResponse.Data;
                    // m_strGatewayResponseDescription = objNSoftwareResponse.ErrorText
                    m_strGatewayResponseDescription = objNSoftwareResponse.Text;
                    // The two following variables are what is sent back to the caller
                    m_intEEPGResponseCode = 98013;
                    m_strEEPGResponseDescription = "Error:  " + objNSoftwareResponse.Code + " : " + objNSoftwareResponse.Text;
                    m_objLog.LogMessage("Generic: ReadGatewayResponse: ResponseCode: " + m_strGatewayResponseCode, 50);
                    m_objLog.LogMessage("Generic: ReadGatewayResponse: ResponseText: " + m_strGatewayResponseDescription, 50);
                    m_objLog.LogMessage("Generic: ReadGatewayResponse: ResponseErrorText: " + objNSoftwareResponse.ErrorText, 50);
                    m_objLog.LogMessage("Generic: ReadGatewayResponse: ResponseRawData: " + m_strGatewayResponseRawData, 50);
                    blnReturn = false;
                }
                else
                {
                    objProperties.Add("TRANSACTIONID", objNSoftwareResponse.TransactionId);
                    objProperties.Add("AVSRESULT", objNSoftwareResponse.AVSResult);
                }
            }
            catch(Exception ex)
            {
                m_intEEPGResponseCode = 98014;
                //m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                blnReturn = false;
            }

            m_objLog.LogMessage("Generic: ReadGatewayResponse: " + blnReturn, 40);

            return blnReturn;
        }

        protected override bool SetSpecialFields(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;

            m_objLog.LogMessage("Generic: SetSpecialFields: Entering.", 40);

            try
            {
                if (objProperties.Count > 0)
                {
                    foreach (KeyValuePair<object, object> kvPair in objProperties)
                    {
                        if ((kvPair.Key != "TRANSACTIONID") & (kvPair.Key != "TRANSACTIONAMOUNT") & (kvPair.Key != "TRANSACTIONDESC") & (kvPair.Key != "TRANSACTIONDOCUMENTNUMBER") & (kvPair.Key != "TRANSACTIONORIGINALAMOUNT"))
                            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
                            m_objNSoftwareGW.AddSpecialField(System.Convert.ToString(kvPair.Key), System.Convert.ToString(kvPair.Value));
                    }
                }
            }
            // objProperties.Clear()
            catch(Exception ex)
            {
                m_intEEPGResponseCode = 98015;
                //m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                blnReturn = false;
            }

            m_objLog.LogMessage("Generic: SetSpecialFields: Exiting : " + blnReturn, 40);

            return blnReturn;
        }

        protected override bool SetGWObject(string strCase)
        {
            return true;
        }
    }
}
