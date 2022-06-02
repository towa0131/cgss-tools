using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace CGSSTools
{
    public class Base64
    {
        private Encoding encode;

        public Base64()
        {
            this.encode = Encoding.UTF8;
        }

        public Base64(Encoding encode)
        {
            this.encode = encode;
        }

        public string Encode(string str)
        {
            return Convert.ToBase64String(encode.GetBytes(str));
        }

        public string Encode(byte[] plain)
        {
            return Convert.ToBase64String(plain);
        }

        public string Decode(string str)
        {
            return encode.GetString(Convert.FromBase64String(str));
        }

        public string Decode(byte[] plain)
        {
            return encode.GetString(Convert.FromBase64String(encode.GetString(plain)));
        }
    }
}