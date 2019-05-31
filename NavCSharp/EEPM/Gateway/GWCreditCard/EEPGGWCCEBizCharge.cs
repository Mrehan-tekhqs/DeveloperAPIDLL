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

using System.Runtime.InteropServices;
//using NavCSharp.eBizCharge;
using Newtonsoft.Json;
using System.Security.Cryptography;
using NavCSharp.eBizDevService;

namespace EEPM
{
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class EEPGGWCCeBizCharge : EEPMGWCCGenericBase
    {
        public EEPGGWCCeBizCharge(int intGatewayID, string strGatewayURL, string strMerchantLogin, string strMerchantPassword, string strMerchantSecurityGUID,  ref Enterprise.EELog objLog) : base(intGatewayID, strGatewayURL, strMerchantLogin, strMerchantPassword, strMerchantSecurityGUID, ref objLog)
        {
        }

        protected override string GatewaySpecificTokenize(string strPlainText, ref Dictionary<object, object> objProperties)
        {
            m_objLog.LogMessage("EEPGGWCCeBizCharge: GatewaySpecificTokenize: ", 40);
         
            return m_objNSoftwareGW.SaveCard(strPlainText);
        }

        protected override bool SetGatewayCredentials(ref Dictionary<object, object> objProperties)
        {
            m_objLog.LogMessage("EEPGGWCCeBizCharge: SetGatewayCredentials: url=<" + m_strGatewayURL + "> login=<" + m_strMerchantLogin + ">", 40);
            m_objNSoftwareGW.GatewayURL = m_strGatewayURL;
            m_objNSoftwareGW.MerchantLogin = m_strMerchantLogin;
            m_objNSoftwareGW.MerchantPassword = m_strMerchantPassword;
            m_objNSoftwareGW.MerchantSecurityGUID = m_strMerchantSecurityGUID;
            return true;
        }

        protected override bool SetGWObject(string strCase)
        {
            bool blnReturn = true;
            m_objLog.LogMessage("EEPGGWCCeBizCharge: SetGWObject: " + strCase, 40);
            try
            {
                m_objNSoftwareGW = new eBizChargeGateway(ref m_objLog);
                m_objNSoftwareCard = new eBizChargeCard(ref m_objLog);
                m_objNSoftwareCustomer = new eBizChargeCustomer(ref m_objLog);
            }
            catch(Exception ex)
            {
                m_intEEPGResponseCode = 98062;
                //m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                blnReturn = false;
            }
            m_objLog.LogMessage("EEPGGWCCeBizCharge: SetGWObject: " + blnReturn, 40);
            return blnReturn;
        }

        protected override bool ReadGatewayResponse(ref System.Collections.Generic.Dictionary<object, object> objProperties)
        {
            bool blnReturn = true;

            m_objLog.LogMessage("EEPGGWCCeBizCharge: ReadGatewayResponse.", 40);

            try
            {
                m_objLog.LogMessage("EEPGGWCCeBizCharge: Response.Data: " + m_objNSoftwareGW.Data, 35);

                if (!(m_objNSoftwareGW.Approved))
                {
                    // The two following variables are included to log someday
                    m_strGatewayResponseCode = m_objNSoftwareGW.ErrorCode;
                    m_strGatewayResponseRawData = m_objNSoftwareGW.Data;
                    m_strGatewayResponseDescription = m_objNSoftwareGW.ErrorText;
                    // The two following variables are what is sent back to the caller
                    m_intEEPGResponseCode = 98013;
                    // m_strEEPGResponseDescription = "Error:  " & "Danger Will Robinson."
                    m_strEEPGResponseDescription = "Error:  " + m_objNSoftwareGW.ErrorCode + " : " + m_objNSoftwareGW.ErrorText;
                    blnReturn = false;
                }
                else
                {
                    if (objProperties.ContainsKey("TRANSACTIONID"))
                        objProperties.Remove("TRANSACTIONID");
                    if (objProperties.ContainsKey("AVSRESULT"))
                        objProperties.Remove("AVSRESULT");
                    objProperties.Add("TRANSACTIONID", m_objNSoftwareGW.TransactionId);
                    objProperties.Add("AVSRESULT", m_objNSoftwareGW.AVSResult);
                }
            }
            catch(Exception ex)
            {
                m_intEEPGResponseCode = 98014;
                // m_strEEPGResponseDescription = "Error:  " + Information.Err().Number + " : " + Information.Err().Description;
                m_strEEPGResponseDescription = "Error: " + ex.InnerException;
                blnReturn = false;
            }

            m_objLog.LogMessage("EEPGGWCCeBizCharge: ReadGatewayResponse: " + blnReturn + ": " + m_strEEPGResponseDescription, 40);

            return blnReturn;
        }

        protected override bool GatewaySpecificCapture(ref Dictionary<object, object> objProperties)
        {
            if ((ContainKeyCheck(objProperties, "TRANSACTIONORIGINALAMOUNT")))
            {
                foreach (object key in objProperties.Keys)
                {
                    object value = null;
                    objProperties.TryGetValue(key, out value);
                    m_objLog.LogMessage("EEPGGWCCeBizCharge: GatewaySpecificCapture: value for " + System.Convert.ToString(key) + ": " + System.Convert.ToString(value), 40);
                }
                m_objNSoftwareGW.TransactionOriginalAmount = FormatAmount(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONORIGINALAMOUNT")));
                m_objNSoftwareGW.RemainingTransactions = int.Parse(System.Convert.ToString(ProcessKey(ref objProperties, "REMAININGTRANSACTIONS")));
                m_objNSoftwareGW.Capture(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONID")), FormatAmount(System.Convert.ToString(ProcessKey(ref objProperties, "TRANSACTIONAMOUNT"))));
                // //EE13.0.40 Email Notification >>
                // m_objNSoftwareGW.SendEmailReceiptByName(CStr(ProcessKey(objProperties, "TRANSACTIONID")), CStr(ProcessKey(objProperties, "EMAILNOTIFICATION")), CStr(ProcessKey(objProperties, "EMAILRECIPIENTADDRESS")))
                // //EE13.0.40 Email Notification <<
                return true;
            }
            return false;
        }
    }

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class eBizChargeCard : CustomGWPostCard
    {
        public eBizChargeCard(ref Enterprise.EELog objLog) : base(ref objLog)
        {
            SetupPostValueArray();
        }

        protected override void SetupPostValueArray()
        {
            m_arrPostValues = new string[] { "", "", "", "", "" };
        }
    }

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class eBizChargeCustomer : CustomGWPostCustomer
    {
        public eBizChargeCustomer(ref Enterprise.EELog objLog) : base(ref objLog)
        {
        }

        protected override void SetupPostValueArray()
        {
            m_arrPostValues = new string[] { "", "", "", "", "", "", "", "", "", "", "" };
        }
    }

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class eBizChargeGateway : CustomGWPostGateway
    {
        private readonly IeBizService  m_service;
        private TransactionObject m_transactionObj;
        private string m_data;
        private string m_avsResult;
        private bool m_approved;
        private string m_errorCode;
        private string m_errorText;
        private string m_strTransactionOriginalAmount;
        private int m_remainingTransactions;
        private string m_EmailTransactionID;
        public PaymentMethodProfile[] m_PaymentMethod;


        public eBizChargeGateway(ref Enterprise.EELog objLog) : base(ref objLog)
        {
            Reset();
            m_service = GetEbizServiceClient();
        }

        private IeBizService  Client
        {
            get
            {
                //m_service.Url = GatewayURL;
                return m_service;
            }
        }

        public static IeBizServiceClient GetEbizServiceClient()
        {

            bool useStagingServer = false;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            var wsdl = "https://soap.ebizcharge.net/eBizService.svc";
            if (useStagingServer)
                wsdl = "https://ebizsoapapi1-staging.azurewebsites.net/eBizService.svc";
            var endpointAddress = new System.ServiceModel.EndpointAddress(new Uri(wsdl));
            var binding = new System.ServiceModel.BasicHttpBinding();
            binding.Name = "eBizService";
            binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;
            binding.ReaderQuotas.MaxBytesPerRead = 2147483647;

            binding.UseDefaultWebProxy = true;
            var client = new IeBizServiceClient(binding, endpointAddress);

            return client;
        }

        private void Reset()
        {
            m_data = "";
            m_avsResult = "";
            m_approved = false;
            m_errorCode = "";
            m_errorText = "";
        }


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
        public int RemainingTransactions
        {
            get
            {
                return m_remainingTransactions;
            }
            set
            {
                m_remainingTransactions = value;
            }
        }

        public string TransactionOriginalAmount
        {
            get
            {
                return m_strTransactionOriginalAmount;
            }
            set
            {
                m_strTransactionOriginalAmount = value;
            }
        }

        public string Data
        {
            get
            {
                return m_data;
            }
        }

        public string AVSResult
        {
            get
            {
                return m_avsResult;
            }
        }

        public bool Approved
        {
            get
            {
                return m_approved;
            }
        }

        public string ErrorCode
        {
            get
            {
                return m_errorCode;
            }
        }

        public string ErrorText
        {
            get
            {
                return m_errorText;
            }
        }

        //public string SaveCard(string strPlainText)
        //{
        //    m_objLog.LogMessage("SaveCard(): Begin");
        //    string custNum="";

        //    string customerId = GenerateNumberIfEmpty(Customer.Id);
        //    try
        //    {
        //        m_objLog.LogMessage("SaveCard(): searching for customer number for ID: " + customerId);
        //        m_objLog.LogMessage("SaveCard(): MerchantLogin: " + MerchantLogin);
        //        m_objLog.LogMessage("SaveCard(): MerchantPassword: " + MerchantPassword);
        //        m_objLog.LogMessage("SaveCard(): MerchantSecurityGUID: " + MerchantSecurityGUID);
        //        Customer[] Customers = Service.SearchCustomers(GenerateToken(),"",customerId,0,100, "FirstName");
        //        custNum = Customers[0].CustomerInternalId;
        //        int count = Customers.Count();
        //        m_objLog.LogMessage("SaveCard(): successfully found customer numer: " + count);
        //    }
        //    catch (System.Web.Services.Protocols.SoapHeaderException e)
        //    {
        //        m_objLog.LogMessage("SaveCard(): error: " + e.Message);
        //        if ("40030:" != e.Message.Substring(0, 6))
        //            throw e;
        //        Customer customerData = new Customer();
        //        Address billingAddress = new Address();
        //        billingAddress.FirstName = Customer.FirstName;
        //        billingAddress.LastName = Customer.LastName;
        //        billingAddress.Address1 = Customer.Address;
        //        billingAddress.City = Customer.City;
        //        billingAddress.State = Customer.State;
        //        billingAddress.ZipCode = Customer.Zip;
        //        billingAddress.Country = Customer.Country;
        //        // billingAddress.Address2 = Customer.Email;
        //        customerData.Email = Customer.Email;
        //        customerData.Phone = Customer.Phone;

        //        customerData.BillingAddress = billingAddress;
        //        customerData.CustomerId = customerId;
        //        try
        //        {
        //            m_objLog.LogMessage("SaveCard(): adding customer...");
        //            //custNum = Service.AddCustomer(GenerateToken(), customerData)
        //            CustomerResponse Custresponse = Service.AddCustomer(GenerateToken(), customerData);
        //            custNum = Custresponse.CustomerInternalId;
        //            m_objLog.LogMessage("SaveCard(): successfully added customer.");
        //        }
        //        finally
        //        {
        //            m_objLog.LogMessage("SaveCard(): addCustomer failed");
        //        }
        //    }
        //    catch (Exception e2)
        //    {
        //        m_objLog.LogMessage("SaveCard(): unknown error: " + e2.Message);
        //    }
        //    PaymentMethodProfile payment = new PaymentMethodProfile();
        //    //payment.MethodName = "CreditCard";
        //    payment.MethodType = "CreditCard";
        //    payment.CardNumber = strPlainText;
        //    m_objLog.LogMessage("SaveCard():  PaymentMethodProfile." + strPlainText);
        //    payment.CardExpiration = System.Convert.ToInt32(Card.ExpMonth).ToString("D2") + Card.ExpYear.Substring(Card.ExpYear.Length - 2, 2);
        //    m_objLog.LogMessage("SaveCard():  PaymentMethodProfile.CardExpiration" + payment.CardExpiration);
        //    payment.CardCode = Card.CVVData;
        //    m_objLog.LogMessage("SaveCard():  PaymentMethodProfile cvvdata." + payment.CardCode);
        //    payment.AvsStreet = Customer.Address;
        //    payment.AvsZip = Customer.Zip;
        //    m_objLog.LogMessage("SaveCard():  cvvdata.payment.AvsStreet" + payment.AvsStreet);
        //    //payment.AccountHolderName = Customer.FirstName + " " + Customer.LastName;//AccountHolderName field removed from web service, we will use MethodName instead
        //    payment.MethodName = Customer.FirstName + " " + Customer.LastName;
        //    m_objLog.LogMessage("SaveCard():  payment.MethodName" + payment.MethodName);

        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    dict.Add("CardHolder", Customer.FirstName + " " + Customer.LastName);
        //    string json = JsonConvert.SerializeObject(dict);
        //    payment.ReloadSchedule = json;

        //    //m_objLog.LogMessage("SaveCard(): PaymentMethod Customer Name: " + payment.AccountHolderName);
        //    m_objLog.LogMessage("SaveCard(): PaymentMethod Customer Name: " + payment.MethodName); //>>>>>>>>>>>>>>>>>>>>>

        //    try
        //    {
        //        m_objLog.LogMessage("SaveCard(): returning custNum: " + custNum);
        //        return custNum + "~" + Service.AddCustomerPaymentMethodProfile(GenerateToken(), custNum, payment);
        //    }
        //    finally
        //    {
        //        m_objLog.LogMessage("SaveCard(): addCustomerPaymentMethod failed");
        //    }
        //}

        public string SaveCard(string strPlainText)
        {
            m_objLog.LogMessage("SaveCard(): Begin");
            string custNum = "";

            string customerId = GenerateNumberIfEmpty(Customer.Id);
            try
            {
                //m_objLog.LogMessage("SaveCard(): searching for customer number for ID: " + customerId);
                //m_objLog.LogMessage("SaveCard(): MerchantLogin: " + MerchantLogin);
                //m_objLog.LogMessage("SaveCard(): MerchantPassword: " + MerchantPassword);
                //m_objLog.LogMessage("SaveCard(): MerchantSecurityGUID: " + MerchantSecurityGUID);
                //Service.GetCustomerToken(GenerateToken(), "", customerId);
                CustomerResponse custRsp = AddRetervieCustIntlID(Customer);
                custNum = custRsp.CustomerInternalId;
                
            }            
            catch (Exception e2)
            {
                m_objLog.LogMessage("SaveCard(): unknown error: " + e2.Message);
            }

            PaymentMethodProfile payment = new PaymentMethodProfile();
            //payment.MethodName = "CreditCard";
            payment.MethodType = "CreditCard";
            payment.CardNumber = strPlainText;
            m_objLog.LogMessage("SaveCard():  PaymentMethodProfile.CardNumber" + strPlainText);
            payment.CardExpiration = System.Convert.ToInt32(Card.ExpMonth).ToString("D2") + Card.ExpYear.Substring(Card.ExpYear.Length - 2, 2);
            m_objLog.LogMessage("SaveCard():  PaymentMethodProfile.CardExpiration" + payment.CardExpiration);
            payment.CardCode = Card.CVVData;
            m_objLog.LogMessage("SaveCard():  PaymentMethodProfile cvvdata." + payment.CardCode);
            payment.AvsStreet = Customer.Address;
            payment.AvsZip = Customer.Zip;
            m_objLog.LogMessage("SaveCard():  cvvdata.payment.AvsStreet" + payment.AvsStreet);
            payment.MethodName = Customer.FirstName + " " + Customer.LastName;
            m_objLog.LogMessage("SaveCard():  payment.MethodName" + payment.MethodName);

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("CardHolder", Customer.FirstName + " " + Customer.LastName);
            string json = JsonConvert.SerializeObject(dict);
            payment.ReloadSchedule = json;

            m_objLog.LogMessage("SaveCard(): PaymentMethod Customer Name: " + payment.MethodName); //>>>>>>>>>>>>>>>>>>>>>

            try
            {
                m_objLog.LogMessage("SaveCard(): returning custNum: " + custNum);
                return custNum + "~" + AddCustPymtProfile(custNum, payment);
            }
            finally
            {
                m_objLog.LogMessage("SaveCard(): addCustomerPaymentMethod failed");
            }
        }

        private CustomerResponse AddRetervieCustIntlID(CustomGWCustomer prmCustomer)
        {
            CustomerResponse custResponse = null;
            
            try
            {
                Customer cust = Client.GetCustomer(GenerateToken(), "", prmCustomer.Id);
                m_objLog.LogMessage("SaveCard(): GetCustomer Success" + cust.FirstName);
                custResponse = new CustomerResponse();
                custResponse.CustomerInternalId = cust.CustomerInternalId;
                m_objLog.LogMessage("SaveCard(): getCust Success" + custResponse.CustomerInternalId);
                if (cust.FirstName == "NotFound")
                {

                }
            }
            catch (Exception ex)
            {
                m_objLog.LogMessage("SaveCard(): getCustomer failed" + ex.Message);
                if (ex.Message == "Not Found")
                {
                    Customer customerData = new Customer();
                    Address billingAddress = new Address();
                    billingAddress.FirstName = prmCustomer.FirstName;
                    billingAddress.LastName = prmCustomer.LastName;
                    billingAddress.Address1 = prmCustomer.Address;
                    billingAddress.City = prmCustomer.City;
                    billingAddress.State = Customer.State;
                    billingAddress.ZipCode = Customer.Zip;
                    billingAddress.Country = Customer.Country;
                    // billingAddress.Address2 = Customer.Email;
                    customerData.Email = prmCustomer.Email;
                    customerData.Phone = prmCustomer.Phone;

                    customerData.BillingAddress = billingAddress;
                    customerData.CustomerId = prmCustomer.Id;
                    try
                    {
                        m_objLog.LogMessage("SaveCard(): adding customer...");
                        custResponse = Client.AddCustomer(GenerateToken(), customerData);

                        m_objLog.LogMessage("SaveCard(): successfully added customer." + custResponse.CustomerInternalId);
                    }
                    catch(Exception exc)
                    {
                        m_objLog.LogMessage("SaveCard(): addCustomer failed" + exc.Message);
                    }
                }
            }
            return custResponse;
        }

        private string AddCustPymtProfile(string prmCustNum, PaymentMethodProfile prmPymt)
        {
            string methodID = "";
            try
            {
                methodID = Client.AddCustomerPaymentMethodProfile(GenerateToken(), prmCustNum, prmPymt);

            }
            catch (Exception exp)
            {
                m_objLog.LogMessage("SaveCard(): addCustomer failed" + exp.Message);
            }
            

            return methodID;
        }
       
        public override void AuthOnly()
        {
            Reset();
            if (!Card.Number.Contains("~"))
            {
                m_objLog.LogMessage(" AuthOnly(): Invoic No: " + InvoiceNumber);
                ParseResponse(Client.runTransaction(GenerateToken(), GenerateTransactionRequest()));
                return;
            }
            string custNum = Card.Number.Split('~')[0];
            string paymentMethodId = Card.Number.Split('~')[1];

            ParseResponse(Client.runCustomerTransaction(GenerateToken(), custNum, paymentMethodId, GenerateCustomerTransactionRequest("AuthOnly")));
        }
        public override void TransactionDetailsOnly()
        {
            Reset();
            if (!Card.Number.Contains("~"))
            {
                ParseResponse(Client.runTransaction(GenerateToken(), GenerateTransactionRequest()));
                return;
            }
            string custNum = Card.Number.Split('~')[0];
            string paymentMethodId = Card.Number.Split('~')[1];
            ParseResponse(Client.runCustomerTransaction(GenerateToken(), custNum, paymentMethodId, GenerateCustomerTransactionRequest("AuthOnly")));
        }



        public override void Capture(string strTransactionID, string strTransactionAmount)
        {
            string strOriginalAmount = TransactionOriginalAmount;

            m_objLog.LogMessage("eBizChargeGateway: Capture: transaction ID: " + strTransactionID + ", original amount: " + strOriginalAmount + ", amount: " + strTransactionAmount + ", remaining transactions: " + m_remainingTransactions.ToString(), 40);

            Reset();

            TransactionId = strTransactionID;
            TransactionAmount = strTransactionAmount;

            double amount;
            double.TryParse(strTransactionAmount, out amount);
            double originalAmount;
            double.TryParse(strOriginalAmount, out originalAmount);
            if (m_remainingTransactions == 0)
            {
                if (amount > originalAmount)
                {
                    // need new authorization
                    m_objLog.LogMessage("eBizChargeGateway: Capture: Need new authorization: invoiced amount is greater than authorized amount: " + strTransactionAmount + " > " + strOriginalAmount + ", remaining transactions: " + m_remainingTransactions.ToString(), 40);
                    TransactionResponse newAuthResponse = Client.runTransaction(GenerateToken(), GenerateTransactionRequest("AuthOnly"));
                    if ("A" == newAuthResponse.ResultCode)
                    {
                        // reverse old authorization
                        Client.runTransaction(GenerateToken(), GenerateTransactionRequest("Void",strTransactionID));
                        m_objLog.LogMessage("eBizChargeGateway: Capture: new authroization succeeded. Auth amount: " + newAuthResponse.AuthAmount.ToString(), 40);
                        strTransactionID = newAuthResponse.RefNum;
                        TransactionId = strTransactionID;
                    }
                }
            }

            TransactionRequestObject transactionRequestObject = GenerateTransactionRequest();
            transactionRequestObject.Command = "capture";

            transactionRequestObject.RefNum = strTransactionID;
            transactionRequestObject.IfAuthExpired = "PostAuth";
            transactionRequestObject.Details.Amount = amount;
            //transactionRequestObject.Details.AmountSpecified = true; // Ryan
            
            ParseResponse(Client.runTransaction(GenerateToken(), transactionRequestObject));

            m_EmailTransactionID = strTransactionID;
        }

        public override void Sale()
        {
            Reset();
            m_objLog.LogMessage("eBizChargeGateway: Sale: Start", 40);
            if (!Card.Number.Contains("~"))
            {
                ParseResponse(Client.runTransaction(GenerateToken(), GenerateTransactionRequest("Sale")));
                return;
            }
            string custNum = Card.Number.Split('~')[0];
            string paymentMethodId = Card.Number.Split('~')[1];
            m_objLog.LogMessage("Generic: GenerateTransactionRequest Ryan Customer: " + custNum, 40);
            m_objLog.LogMessage("Generic: GenerateTransactionRequest Ryan Customerid: " + Customer.Id, 40);
           
            m_objLog.LogMessage("Generic: GenerateTransactionRequest Ryan PaymentMetodID:" + paymentMethodId, 40);

            ParseResponse(Client.runCustomerTransaction(GenerateToken(), custNum, paymentMethodId, GenerateCustomerTransactionRequest("Sale")));
            m_objLog.LogMessage("eBizChargeGateway: Sale: End", 40);
        }

        public override void Refund(string strTransactionID, string strTransactionAmount)
        {
            Reset();
            m_objLog.LogMessage("eBizChargeGateway: Refund: start ", 40);
           
            TransactionAmount = strTransactionAmount;
            double amount;
            double.TryParse(strTransactionAmount, out amount);

            TransactionRequestObject transactionRequestObject = GenerateTransactionRequest();
            transactionRequestObject.Command = "Credit";

            transactionRequestObject.RefNum = strTransactionID;
            transactionRequestObject.IfAuthExpired = "PostAuth";
            transactionRequestObject.Details.Amount = amount;

            ParseResponse(Client.runTransaction(GenerateToken(), transactionRequestObject));
            m_objLog.LogMessage("eBizChargeGateway: Refund: end ", 40);
        }

        public override void VoidTransaction(string strTransactionID)
        {
            //Reset();
            //TransactionId = strTransactionID;
           // m_approved = Service.runTransactionion(GenerateToken(), strTransactionID);

            TransactionRequestObject transactionRequestObject = GenerateTransactionRequest();
            transactionRequestObject.Command = "Void";
            transactionRequestObject.RefNum = strTransactionID;
            
            //m_approved = Service.runTransaction(GenerateToken(), transactionRequestObject);
            TransactionResponse response = Client.runTransaction(GenerateToken(), transactionRequestObject);
            if (response.ResultCode == "A")
            {
                //MessageBox.Show(string.Concat("Transaction Approved, RefNum: ",
                //        response.RefNum));
                m_approved = true;
            }
            else
            {
            //    MessageBox.Show(string.Concat("Transaction Failed: ",
            //            response.Error));
             m_approved = false;
            }

        }
        public override void TransactionInfo(string strTransactionID)
        {
            Reset();
            TransactionId = strTransactionID;
            m_transactionObj = Client.GetTransactionDetails(GenerateToken(), strTransactionID);
        }
        public override void ReAuthTransaction(string strTransactionID, double amount)
        {
            Reset();
            // ParseResponse(Service.captureTransaction(GenerateToken(), strTransactionID, amount));
            TransactionDetail transactiondetailObject = GenerateTransactionDetail();
            transactiondetailObject.Amount = amount;
            TransactionRequestObject transactionRequestObject = GenerateTransactionRequest();
            transactionRequestObject.Command = "Capture";
            transactionRequestObject.RefNum = strTransactionID;
            transactionRequestObject.Details = transactiondetailObject;
            ParseResponse(Client.runTransaction(GenerateToken(), transactionRequestObject));

        }

        public string AddCustomer(Customer objCustomer)
        {
            Reset();
            //return Service.addCustomer(GenerateToken(), objCustomer);
           return Client.AddCustomer(GenerateToken(),objCustomer).ToString();    
        }

        //public PaymentMethod[] GetCustomerPaymentMethod(string CustNum)
       // {
        //    Reset();
        //    return Service.getCustomerPaymentMethods(GenerateToken(), CustNum);
           
       // }
        public PaymentMethodProfile[] GetCustomerPaymentMethodProfile(string CustNum)
        {
            Reset();
            //return Service.getCustomerPaymentMethods(GenerateToken(), CustNum);
            return Client.GetCustomerPaymentMethodProfiles(GenerateToken(), CustNum);
        }
        /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
        public void SendEmailReceiptByValue(string strTransactionID, string strEmailNotification, string strEmailAddress)
        {
            Reset();
            bool result = false;
            EmailReceiptResponse emailresponseobj;



            if (strTransactionID.Length > 0)
            {
                if (string.IsNullOrEmpty(strEmailNotification) == false)
                {
                    if (string.IsNullOrEmpty(strEmailAddress) == false)
                        // result = Service.emailTransactionReceiptByName(GenerateToken(), strTransactionID, "vterm_customer", strEmailAddress);

                        emailresponseobj = Client.EmailReceipt(GenerateToken(), strTransactionID, "vterm_customer", "", strEmailAddress);
                    //if (emailresponseobj.StatusCode = 1)
                    //{
                    //    result = true;
                    //}
                    //else
                    //{
                    //    result = false;
                    //}
                }
            }

            //return result;
        }

        public string GetCustomer(string CustomerId)
        {
            Reset();
            //return Service.searchCustomerID(GenerateToken(), CustomerId);
            return Client.GetCustomerToken(GenerateToken(), CustomerId, ""); 
        }

        private void ParseResponse(TransactionResponse response)
        {
            m_data = response.Payload;
            m_avsResult = response.AvsResult;
            m_approved = ("A" == response.ResultCode);
            m_errorCode = response.ErrorCode;
            m_errorText = response.Error;
            TransactionId = response.RefNum;
        }

        private string GenerateNumberIfEmpty(string input)
        {
            return string.IsNullOrEmpty(input) ? (DateTime.UtcNow.Ticks % 10000).ToString() : input;
        }

        private CustomerTransactionRequest GenerateCustomerTransactionRequest(string command)
        {
            CustomerTransactionRequest ctr = new CustomerTransactionRequest();
            ctr.CardCode = Card.CVVData;
            ctr.Command = command;
            ctr.Details = GenerateTransactionDetail();
            ctr.LineItems = GenerateLineItems(ctr.Details.Amount);
            return ctr;
        }

        private TransactionDetail GenerateTransactionDetail()
        {
            TransactionDetail details = new TransactionDetail();
            double amount;
            double.TryParse(TransactionAmount, out amount);
            details.Amount = amount;
            //details.AmountSpecified = true;
            details.Invoice = GenerateNumberIfEmpty(InvoiceNumber);
            details.PONum = details.Invoice;
            details.OrderID = details.Invoice;
            details.Description = string.IsNullOrEmpty(TransactionDesc) ? "Description" : TransactionDesc;
            return details;
        }

        private TransactionDetail GenerateTransactionwithDetail(string strInvoiceNumber, string strExtInvoiceNumber)
        {
            TransactionDetail details = new TransactionDetail();
            double amount;
            double.TryParse(TransactionAmount, out amount);
            details.Amount = amount;
            //details.AmountSpecified = true;
            details.Invoice = strInvoiceNumber;
            details.PONum = strExtInvoiceNumber;
            m_objLog.LogMessage("GenerateTransactionwithDetail() Invoice Number : " + strInvoiceNumber);
            m_objLog.LogMessage("GenerateTransactionwithDetail() EXT Invoice Number : " + strExtInvoiceNumber);
            details.OrderID = details.Invoice;
            details.Description = string.IsNullOrEmpty(TransactionDesc) ? "Description" : TransactionDesc;
            return details;
        }

        private LineItem[] GenerateLineItems(double amount)
        {
            LineItem line = new LineItem();
            line.SKU = "SKU";
            line.CommodityCode = "0";
            line.ProductName = "ProductName";
            line.Description = "Description";
            line.DiscountAmount = "0.0";
            line.DiscountRate = "0.0";
            line.UnitOfMeasure = "EA";
            line.UnitPrice = amount.ToString();
            line.Qty = "1";
            line.Taxable = false;
            line.TaxAmount = "0";
            line.TaxRate = "0";
            return new[] { line };
        }

        private TransactionRequestObject GenerateTransactionRequest()
        {
            TransactionRequestObject tro = new TransactionRequestObject();
            tro.CreditCardData = new CreditCardData();
            tro.AccountHolder = Customer.FirstName + " " + Customer.LastName;

            Address address = new Address();
            address.FirstName = Customer.FirstName;
            address.LastName = Customer.LastName;
            address.Address1 = Customer.Address;
            address.City = Customer.City;
            address.State = Customer.State;
            address.ZipCode = Customer.Zip;
            address.Country = Customer.Country;
            
           // address.Email = Customer.Email;
            //address.Phone = Customer.Phone;
            //tro.BillingAddress = address;
            tro.CreditCardData.CardNumber = Card.Number;
            tro.CreditCardData.CardExpiration = Card.ExpMonth + Card.ExpYear;
            tro.CreditCardData.CardCode = Card.CVVData;

            tro.Details = GenerateTransactionwithDetail(InvoiceNumber, ExtInvoiceNumber);
            tro.CustomerID = GenerateNumberIfEmpty(Customer.Id);
            tro.LineItems = GenerateLineItems(tro.Details.Amount);

            LastRequest = tro;
            return tro;
        }
        private TransactionRequestObject GenerateTransactionRequest(string command)
        {
            TransactionRequestObject tro = new TransactionRequestObject();
            tro.CreditCardData = new CreditCardData();
            tro.AccountHolder = Customer.FirstName + " " + Customer.LastName;

            Address address = new Address();
            address.FirstName = Customer.FirstName;
            address.LastName = Customer.LastName;
            address.Address1 = Customer.Address;
            address.City = Customer.City;
            address.State = Customer.State;
            address.ZipCode = Customer.Zip;
            address.Country = Customer.Country;
            //address.Email = Customer.Email;
            //address.Phone = Customer.Phone;
            //tro.BillingAddress = address;
            tro.CreditCardData.CardNumber = Card.Number;
            tro.CreditCardData.CardExpiration = Card.ExpMonth + Card.ExpYear;
            tro.CreditCardData.CardCode = Card.CVVData;

            tro.Details = GenerateTransactionwithDetail(InvoiceNumber, ExtInvoiceNumber);
            tro.CustomerID = GenerateNumberIfEmpty(Customer.Id);
            tro.LineItems = GenerateLineItems(tro.Details.Amount);
            tro.Command = command;     //ryan
            LastRequest = tro;
            return tro;
        }
        private TransactionRequestObject GenerateTransactionRequest(string command, string strTransactionID)
        {
            TransactionRequestObject tro = new TransactionRequestObject();
            tro.CreditCardData = new CreditCardData();
            tro.AccountHolder = Customer.FirstName + " " + Customer.LastName;

            Address address = new Address();
            address.FirstName = Customer.FirstName;
            address.LastName = Customer.LastName;
            address.Address1 = Customer.Address;
            address.City = Customer.City;
            address.State = Customer.State;
            address.ZipCode = Customer.Zip;
            address.Country = Customer.Country;
            //address.Email = Customer.Email;
            //address.Phone = Customer.Phone;
            //tro.BillingAddress = address;
            tro.CreditCardData.CardNumber = Card.Number;
            tro.CreditCardData.CardExpiration = Card.ExpMonth + Card.ExpYear;
            tro.CreditCardData.CardCode = Card.CVVData;

            tro.Details = GenerateTransactionwithDetail(InvoiceNumber, ExtInvoiceNumber);
            tro.CustomerID = GenerateNumberIfEmpty(Customer.Id);
            tro.LineItems = GenerateLineItems(tro.Details.Amount);
            tro.Command = command;     //ryan
            tro.RefNum = strTransactionID;
            LastRequest = tro;
            return tro;
        }

        private TransactionRequestObject lastRequestBackingField;
        public TransactionRequestObject LastRequest
        {
            get
            {
                return lastRequestBackingField;
            }
            private set
            {
                lastRequestBackingField = value;
            }
        }

        private SecurityToken GenerateToken()
        {
            SecurityToken token = new SecurityToken();
            token.SecurityId = MerchantSecurityGUID;
            token.UserId = MerchantLogin;
            token.Password = MerchantPassword;
            //token.SourceKey = MerchantLogin;
            //NavCSharp.eBizCharge.ueHash pinHash = new NavCSharp.eBizCharge.ueHash();
            //pinHash.Seed = DateTime.Now.ToUniversalTime() + new System.Random().Next().ToString();
            //pinHash.Type = "md5";
            //using (MD5 md5Hasher = MD5.Create())
            //{
            //    byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(token.SourceKey + pinHash.Seed + MerchantPassword));
            //    StringBuilder sBuilder = new StringBuilder();
            //    int i;
            //    for (i = 0; i <= data.Length - 1; i++)
            //        sBuilder.Append(data[i].ToString("x2"));
            //    pinHash.HashValue = sBuilder.ToString();
            //    token.PinHash = pinHash;
            //}
            return token;
        }

        protected override void SetupPostValueArray()
        {
            m_arrPostValues = new string[] { "", "", "", "", "", "", "", "", "" };
        }

        public string UpdateCard(string paymentMethod, string firstName, string lastname)
        {
            m_objLog.LogMessage("UpdateCard(): Begin");
            //PaymentMethod payment = new PaymentMethod();
            PaymentMethodProfile paymentMthProfile = new PaymentMethodProfile();
            m_objLog.LogMessage("UpdateCard(): Payment Method: " + paymentMethod);
            var updatePaymentMethodprofile = new PaymentMethodProfile();

            string cNum = paymentMethod.Split('~')[0];
            string pMethodId = paymentMethod.Split('~')[1];
            string m_updatecard = "card update successfully";

            m_objLog.LogMessage("UpdateCard(): Cust Num: " + cNum, 40);
            m_objLog.LogMessage("UpdateCard(): Payment Method Id: " + pMethodId, 40);

            //payment = Service.getCustomerPaymentMethod(GenerateToken(), cNum, pMethodId);

            paymentMthProfile = Client.GetCustomerPaymentMethodProfile(GenerateToken(), cNum, pMethodId);
            
            //updatePayment.MethodID = payment.MethodID;
            updatePaymentMethodprofile.MethodID = paymentMthProfile.MethodID;
            //updatePayment.MethodName = payment.MethodName;
            //updatePayment.CardNumber = "XXXXXX" + payment.CardNumber.Substring(6, (payment.CardNumber.Length - 6));
            updatePaymentMethodprofile.CardNumber = "XXXXXX" + paymentMthProfile.CardNumber.Substring(6, (paymentMthProfile.CardNumber.Length -6));
            updatePaymentMethodprofile.CardType = paymentMthProfile.CardType; //payment.CardType;
            updatePaymentMethodprofile.CardExpiration = paymentMthProfile.CardExpiration;// payment.CardExpiration;

            updatePaymentMethodprofile.AvsStreet = paymentMthProfile.AvsStreet; //payment.AvsStreet;
            updatePaymentMethodprofile.AvsZip = paymentMthProfile.AvsZip; //payment.AvsZip;
            updatePaymentMethodprofile.Created = paymentMthProfile.Created; //payment.Created;
            updatePaymentMethodprofile.SecondarySort = paymentMthProfile.SecondarySort; //payment.SecondarySort;
            updatePaymentMethodprofile.Modified = paymentMthProfile.Modified; //payment.Modified;
            //updatePayment.AccountHolderName = firstName + " " + lastname; //AccountHolderName field removed from web service, we will use MethodName instead
            updatePaymentMethodprofile.MethodName = firstName + " " + lastname;

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("CardHolder", firstName + " " + lastname);
            string json = JsonConvert.SerializeObject(dict);
            updatePaymentMethodprofile.ReloadSchedule = json;

            // Dim dict As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            // Dim json As String = payment.ReloadSchedule
            // dict = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(json)

            // If dict.ContainsKey("cardholder") Then
            // Dim value As String = dict("cardholder")
            // updatePayment.ReloadSchedule = value
            // updatePayment.AccountHolderName = value
           
            // End If

            try
            {
                // m_objLog.LogMessage("SaveCard(): returning custNum: " & custNum)
                // addCustomerPaymentMethod(GenerateToken(), custNum, payment, False, False)
                m_objLog.LogMessage("update card ");
                //return Service.updateCustomerPaymentMethod(GenerateToken(), updatePayment, false).ToString();
                if (Client.UpdateCustomerPaymentMethodProfile(GenerateToken(), cNum, updatePaymentMethodprofile)) // Ray need to fix this
                    

                return m_updatecard;
            }

            catch (Exception ex)
            {
                m_objLog.LogMessage("UpdateCard(): UpdateCustomerPaymentMethod failed " + ex.GetBaseException().ToString());
                return "";
            }
            finally
            {
                m_objLog.LogMessage("UpdateCard(): UpdateCustomerPaymentMethod");
            }
        }
    }
}
