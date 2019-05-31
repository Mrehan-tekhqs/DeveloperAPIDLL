using System;
using System.Runtime.InteropServices;

[Guid("B307A0AA-E10F-49D6-81B4-840A2A0E2B86")]
public interface IEEGateway
{
    bool Authorize();
    bool Capture();
    bool DirectSale();
    bool TransactionDetails();

    bool Credit();
    bool Void();
    string Tokenize(string strPlainText);       // MPF20140310

    void AddNameValue(string strName, string strValue);
    string GetNameValue(string strName);
    void ClearNameValue();

    int GatewayID { get; set; }
    int PaymentType { get; set; }
    string GatewayURL { get; set; }
    string GatewayLogin { get; set; }
    string GatewayPassword { get; set; }
    string GatewaySecurityGUID { get; set; }
    int ResponseCode { get; }
    string ResponseDescription { get; }

    string GatewaySecurityProfile { get; set; }                     // MPF20140310
    bool Transactioninfo(string refNum);
    bool ReAuthTransaction(string refNum);
}
