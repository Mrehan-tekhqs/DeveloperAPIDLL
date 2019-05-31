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

namespace EEPM
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("A8433CCE-0573-4E26-BE67-D571CCC326C6")]
    public class EECCValidator : Enterprise.EEBase, IEECCValidator
    {

        // ###################################################################################
        // Constructors\Destructors
        // ###################################################################################
        public EECCValidator() : base()
        {
            m_intResponseCode = -1;
            m_strResponseDescription = "";
            SetupBase("PCICharge", "EEPM", 2);
            m_objNSoftwareCCValidator = new nsoftware.InPay.Cardvalidator();
            m_objNSoftwareCCValidator.RuntimeLicense = "42504E3641413153554252413153554243483945353033300000000000000000000000000000000058584436334D594500005357595253564B4D5A5338580000"; // Version 6 license
        }

        ~EECCValidator()
        {
            m_objNSoftwareCCValidator = null;
        }

        // ###################################################################################
        // Public functions
        // ###################################################################################
        /*  public virtual int GetCardType(string strCCNumber)
          {
              int intReturn = 0;
              try
              {
                  m_objNSoftwareCCValidator.CardNumber = strCCNumber;
                  m_objNSoftwareCCValidator.CardExpMonth = 1;
                  m_objNSoftwareCCValidator.CardExpYear = System.Convert.ToInt32(Strings.Right(System.Convert.ToDouble(DateTime.Now.Year.ToString()).ToString() + 1, 2));
                  m_objNSoftwareCCValidator.ValidateCard();
                  intReturn = int.Parse(m_objNSoftwareCCValidator.CardType.ToString());
              }
              catch
              {
                  DetermineError(Information.Err());
                  intReturn = -1;
              }
              return intReturn;
          }  */

        public virtual int GetCardType(string strCCNumber)
        {
            int intReturn = 0;
            string strCardType = "";
            try
            {
                m_objNSoftwareCCValidator.CardNumber = strCCNumber;
                m_objNSoftwareCCValidator.CardExpMonth = 1;
                m_objNSoftwareCCValidator.CardExpYear = int.Parse((DateTime.Now.Year.ToString() + 1).Substring(((DateTime.Now.Year.ToString() + 1).Length - 2)));
                         
                m_objNSoftwareCCValidator.ValidateCard();
                strCardType = m_objNSoftwareCCValidator.CardType.ToString();
                m_objLog.LogMessage("GetCardType():Get card TYPE BEFORE ..." + strCardType);
                switch (strCardType)
                {
                    case "vctVisa":
                        intReturn = 1;
                        break;
                }
               
               // m_objLog.LogMessage("GetCardType():Get card TYPE BEFORE ..." + intReturn);
            }
            catch (System.Exception ex)
            {
                
                intReturn = -1;
            }

            return intReturn;
        }
      
        public virtual bool ValidateCard(string strCCNumber, int strCCExpMonth, int strCCExpYear)
        {
            bool blnReturn = true;
            m_objNSoftwareCCValidator.CardNumber = strCCNumber;
            m_objNSoftwareCCValidator.CardExpMonth = strCCExpMonth;
            m_objNSoftwareCCValidator.CardExpYear = strCCExpYear;
            m_objNSoftwareCCValidator.ValidateCard();
            if ((blnReturn))
                blnReturn = m_objNSoftwareCCValidator.DateCheckPassed;
            if ((blnReturn))
                blnReturn = m_objNSoftwareCCValidator.DigitCheckPassed;
            return blnReturn;
        }

        // ###################################################################################
        // Public property functions
        // ###################################################################################
        public override int ResponseCode
        {
            get
            {
                return m_intResponseCode;
            }
        }

        public override string ResponseDescription
        {
            get
            {
                if ((m_strResponseDescription != ""))
                    // m_strResponseDescription = Strings.Replace(m_strResponseDescription, "nsoftware", "PCICharge", 1, Compare: CompareMethod.Text);
                    m_strResponseDescription = m_strResponseDescription.Replace("nsoftware", "PCICharge");
                return m_strResponseDescription;
            }
        }

        // ###################################################################################
        // Protected functions
        // ###################################################################################

        protected virtual void DetermineError(Microsoft.VisualBasic.ErrObject objError)
        {
            if ((objError.Number == 504 | objError.Number == 5))
            {
                m_intResponseCode = 98032;
                m_strResponseDescription = "Card failed the Luhn digit check.  Check the number entered.";
                return;
            }
            if ((objError.Number == 505))
            {
                m_intResponseCode = 98035;
                m_strResponseDescription = "Expiration month entered is invalid.";
                return;
            }
            if ((objError.Number == 506))
            {
                m_intResponseCode = 98036;
                m_strResponseDescription = "Expiration year entered is invalid.";
                return;
            }
            if ((objError.Number == 703))
            {
                m_intResponseCode = 98033;
                m_strResponseDescription = "Invalid characters entered into the card number.";
                return;
            }
            if ((objError.Number == 704))
            {
                m_intResponseCode = 98034;
                m_strResponseDescription = "Number appears valid, but card type was not determined.";
                return;
            }
            m_intResponseCode = 99000 + objError.Number;
            m_strResponseDescription = objError.Description;
        }

        // ###################################################################################
        // Public variables
        // ###################################################################################


        // ###################################################################################
        // Protected variables
        // ###################################################################################
        protected nsoftware.InPay.Cardvalidator m_objNSoftwareCCValidator;
    }
}
