using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NavCSharp
{
    [ComVisible(false)]
    public class Class1
    {

        public string printMe(string text)
        {
            return "Hi " + text;
        }
    }
}
