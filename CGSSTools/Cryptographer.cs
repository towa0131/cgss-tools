using System;
using System.Collections;
using System.Collections.Generic;

namespace CGSSTools
{
    public class Cryptographer
    {
        public static string Encode(string data)
        {
            string str = "";
            Random rand = new Random();
            for (int i = 0; i < data.Length; i++)
            {
                str += rand.Next(0, 10).ToString()
                    + rand.Next(0, 10).ToString()
                    + (char)((int)data[i] + 10)
                    + rand.Next(0, 10);
            }

            return System.Convert.ToString(data.Length, 16).PadLeft(4, '0') + str
                + rand.Next(10000000, 100000000) + rand.Next(10000000, 100000000);
        }

        public static string Decode(string data)
        {
            int num = System.Convert.ToInt32(data.Substring(0, 4), 16);
            string result = "";
            for (int i = 6; i < data.Length && result.Length < num; i += 4)
            {
                result += (char)((int)(data[i]) - 10);
            }
            return result;
        }
    }
}