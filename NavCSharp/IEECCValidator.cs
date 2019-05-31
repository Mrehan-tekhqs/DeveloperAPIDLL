using System;
using System.Runtime.InteropServices;

[Guid("53567D59-C87F-4EB3-A994-8B68930051C4")]
public interface IEECCValidator
{
    int GetCardType(string strCCNumber);
    bool ValidateCard(string strCCNumber, int strCCExpMonth, int strCCExpYear);

    int ResponseCode { get; }
    string ResponseDescription { get; }
}