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
    public class EEBitwise
    {

        // Returns true if the bit specified in intBit is true, false if not.
        public bool ExamineBit(byte bytToCheck, int intBit)
        {
            double BitMask;
            BitMask = Math.Pow(2, (intBit - 1));            
            return ((bytToCheck & byte.Parse(BitMask.ToString())) > 0);
        }

        // Clears the bit specified in intBit in the byte given in bytByte
        public void ClearBit(ref byte bytByte, int intBit)
        {
            double BitMask;
            BitMask = Math.Pow(2, (intBit - 1));            
            int intByte = ~int.Parse(BitMask.ToString());            
            bytByte = byte.Parse(intByte.ToString());
        }

        // Sets the bit specified in intBit in the byte given in bytByte
        public void SetBit(ref object bytByte, object intBit)
        {
            double BitMask;
            BitMask = Math.Pow(2, (int.Parse(intBit.ToString()) - 1));            
            bytByte = int.Parse(bytByte.ToString()) | int.Parse(BitMask.ToString());            
        }

        // Toggles the bit specified in intBit in the byte given in bytByte
        public void ToggleBit(ref object bytByte, object intBit)
        {
            double BitMask;
            BitMask = Math.Pow(2, (int.Parse(intBit.ToString()) - 1));
            bytByte = int.Parse(bytByte.ToString()) ^ int.Parse(BitMask.ToString());
        }
    }

}
