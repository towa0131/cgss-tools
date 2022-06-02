using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using MessagePack;
using LitJson;

namespace CGSSTools
{
    public class CGSSAPI
    {
        public const string BASE_URL = "https://apis.game.starlight-stage.jp";
        public const int RES_VER = 10096600;
        public const string APP_VER = "7.8.0";
        public const string WC_VER = "2020.3.8f1";

        public const string VIEWER_ID_KEY = "s%5VNQ(H$&Bqb6#3+78h29!Ft4wSg)ex";
        public const string SID_SALT = "r!I@nt8e5i=";

        protected string udid = "";
        protected int viewerId = 0;
        protected int userId = 0;
        protected string sid = "";

        protected PostParams Params = new PostParams();

        private static HttpClient client = new HttpClient();

        public CGSSAPI(string udid, int viewerId, int userId)
        {
            this.udid = udid;
            this.viewerId = viewerId;
            this.userId = userId;
            this.sid = viewerId.ToString() + udid.ToString();
        }

        public string Call(Dictionary<string, object> args, string endpoint)
        {
            Random rand = new Random();
            Base64 b64 = new Base64();
            string vid_iv = string.Empty;
            for (int i = 0; i < 16; i++)
            {
                vid_iv += rand.Next(1, 10).ToString();
            }

            args["timezone"] = "09:00:00";
            args["viewer_id"] = vid_iv + Rijndael.Encrypt256(
                Encoding.UTF8.GetBytes(this.viewerId.ToString()),
                Encoding.UTF8.GetBytes(VIEWER_ID_KEY),
                Encoding.UTF8.GetBytes(vid_iv));
            ;
            byte[] inArray = MessagePackSerializer.Serialize(args);

            string plain = b64.Encode(inArray);

            string key = string.Empty;
            for (int i = 0; i < 32; i++)
            {
                key += rand.Next(0, 10).ToString();
            }

            byte[] msg_iv = Binary.StringToBytes(this.udid);

            byte[] e256 = System.Convert.FromBase64String(Rijndael.Encrypt256(
                        Encoding.UTF8.GetBytes(plain),
                        Encoding.UTF8.GetBytes(key),
                        msg_iv));
            byte[] b64key = Encoding.UTF8.GetBytes(key);
            byte[] rv = new byte[e256.Length + b64key.Length];

            System.Buffer.BlockCopy(e256, 0, rv, 0, e256.Length);
            System.Buffer.BlockCopy(b64key, 0, rv, e256.Length, b64key.Length);

            string body = b64.Encode(rv);

            Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { "Host", "apis.game.starlight-stage.jp" },
            { "User-Agent", "Dalvik/2.1.0 (Linux; U; Android 8.1.0; Nexus 4 Build/XYZZ1Y)" },
            { "Connection", "keep-alive" },
            { "Accept", "*/*" },
            { "Accept-Encoding", "gzip, deflate" },
            { "Accept-Language", "en-us" },
            { "X-Unity-Version", CGSSAPI.WC_VER },
            { "UDID", Cryptographer.Encode(this.udid) },
            { "USER-ID", Cryptographer.Encode(this.userId.ToString()) },
            { "SID", Binary.md5(this.sid + CGSSAPI.SID_SALT) },
            { "PARAM", Binary.sha1(this.udid + this.viewerId.ToString() + endpoint + plain) },
            { "DEVICE", "1" },
            { "APP-VER", CGSSAPI.APP_VER },
            { "RES-VER", CGSSAPI.RES_VER.ToString() },
            { "DEVICE-ID", Binary.md5("Totally a real Android") },
            { "DEVICE-NAME", "Nexus 42" },
            { "GRAPHICS-DEVICE-NAME", "3dfx Voodoo2 (TM)" },
            { "IP-ADDRESS", "127.0.0.1" },
            { "PLATFORM-OS-VERSION", "Android OS 13.3.7 / API-42 (XYZZ1Y/74726f6c6c)" },
            { "CARRIER", "docomo" },
            { "KEYCHAIN", "" },
            { "PROCESSOR-TYPE", "ARMv7 VFPv3 NEON" },
            { "IDFA", "" }
        };

            string res = string.Empty;

            string response = this.Post(CGSSAPI.BASE_URL + endpoint, headers, body);

            byte[] src = System.Convert.FromBase64String(response);
            byte[] bytekey = new byte[32];
            byte[] context = new byte[src.Length - 32];
            System.Buffer.BlockCopy(src, src.Length - 32, bytekey, 0, 32);
            System.Buffer.BlockCopy(src, 0, context, 0, src.Length - 32);

            plain = Rijndael.Decrypt256(context, bytekey, msg_iv);

            byte[] plainBytes = System.Convert.FromBase64String(plain);

            var result = MessagePackSerializer.Deserialize<dynamic>(plainBytes);


            if (result["data_headers"]["result_code"] == 1)
            {
                this.sid = result["data_headers"]["sid"];
            }

            res = JsonMapper.ToJson(result);

            return res;
        }

        public string Post(string url, Dictionary<string, string> headers, string body)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            client.DefaultRequestHeaders.Clear();

            foreach (KeyValuePair<string, string> header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            request.Content = new StringContent(body);

            var response = client.SendAsync(request).Result;

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
