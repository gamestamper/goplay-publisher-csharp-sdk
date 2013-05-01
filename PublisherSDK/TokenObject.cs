using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay
{
    public class TokenObject
    {
        public TokenObject()
        {
        }

        public TokenObject(string token)
        {
            Token = token;
            Failures = 0;
        }

        public TokenObject(string token, int failures)
        {
            Token = token;
            Failures = failures;
        }

        public string Token
        {
            get;
            set;
        }

        public int Failures
        {
            get;
            set;
        }
    }
}
