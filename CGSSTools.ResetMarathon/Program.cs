using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using LitJson;
using CGSSTools;

namespace CGSSTools.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            string udid = Guid.NewGuid().ToString();
            int viewerId = 0;
            int userId = 0;

            CGSSAPI api = new CGSSAPI(udid, viewerId, userId);

            Dictionary<string, object> arg = new Dictionary<string, object>()
            {
                { "device_name", "Nexus 42" },
                { "client_type", "1" },
                { "os_version", "Android OS 13.3.7 / API-42 (XYZZ1Y/74726f6c6c)" },
                { "resource_version", "Android OS 13.3.7 / API-42 (XYZZ1Y/74726f6c6c)" },
                { "app_version", "7.8.0" }
            };

            var dic = JsonMapper.ToObject(api.Call(arg, "/tool/signup"));

            if ((int)dic["data_headers"]["result_code"] != 1)
            {
                Console.WriteLine("[-] アカウント作成に失敗しました");
                Console.ReadLine();
                return;
            }

            viewerId = (int)dic["data_headers"]["viewer_id"];
            userId = (int)dic["data_headers"]["user_id"];

            Console.WriteLine("[+] アカウント作成に成功しました");
            Console.WriteLine("[*] UDID     : " + udid);
            Console.WriteLine("[*] ViewerID : " + viewerId);
            Console.WriteLine("[*] UserID   : " + userId);

            api = new CGSSAPI(udid, viewerId, userId);

            arg = new Dictionary<string, object>()
            {
                { "app_type", 0 },
                { "campaign_data", "" },
                { "campaign_user", 1468 },
                { "campaign_sign", Binary.md5("All your APIs are belong to us.") }
            };

            api.Call(arg, "/load/check");

            dic = JsonMapper.ToObject(api.Call(arg, "/load/index"));

            if ((int)dic["data_headers"]["result_code"] != 1)
            {
                Console.WriteLine("[-] ログインに失敗しました");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("[+] ログインに成功しました");

            List<int> tutorialId = new List<int>()
            {
                10, 20, 30, 40, 50, 55, 60, 70, 73, 77, 80, 90
            };

            foreach (int i in tutorialId)
            {
                arg = GetTutorialArgs(i);
                switch (i)
                {
                    case 20:
                        Console.WriteLine("[+] センターアイドルを選択してください");
                        Console.WriteLine("[1] : 島村 卯月");
                        Console.WriteLine("[2] : 渋谷 凛");
                        Console.WriteLine("[3] : 本田 未央");
                        Console.Write(">> ");
                        arg["type"] = int.Parse(Console.ReadLine());
                        break;
                    case 90:
                        Console.WriteLine("[+] プロデューサー名を入力してください");
                        Console.Write(">> ");
                        arg["name"] = Console.ReadLine();
                        break;
                }

                Thread.Sleep(2000);
                dic = JsonMapper.ToObject(api.Call(arg, "/tutorial/update_step"));

                if ((int)dic["data_headers"]["result_code"] == 1)
                {
                    Console.WriteLine("[+] チュートリアル " + i + " をクリア");
                }
                else
                {
                    Console.WriteLine("[-] チュートリアル " + i + " を失敗");
                    Console.ReadLine();
                    return;
                }
            }

            Console.WriteLine("[+] 引継ぎパスワードを設定します");
            Console.WriteLine("[*] 数字、大文字、小文字を組み合わせた8文字以上16文字以下が条件です");
            Console.Write(">> ");
            string strPasswd = Console.ReadLine();

            byte[] passwd = Encoding.UTF8.GetBytes(strPasswd);
            byte[] key = Encoding.UTF8.GetBytes("0Z1rnpGSYEOLwqNLeYguqAZQS6gqcXR5");
            byte[] msg_iv = Encoding.UTF8.GetBytes("JVZxnYTh9bhmfZ2M");

            string encryptedPasswd = Rijndael.Encrypt256(passwd, key, msg_iv, 256);

            arg = new Dictionary<string, object>()
                        {
                            { "openid_type", 99 },
                            { "openid", "" },
                            { "password", encryptedPasswd }
                        };

            dic = JsonMapper.ToObject(api.Call(arg, "/personal_id/setting/exec"));

            if ((int)dic["data_headers"]["result_code"] == 1)
            {
                Console.WriteLine("[+] 引継ぎパスワードの設定に成功しました");
                Console.WriteLine("=== アカウント引継ぎ情報 ===");
                Console.WriteLine("[+] ID       : " + viewerId);
                Console.WriteLine("[+] Password : " + strPasswd);
            }
            else
            {
                Console.WriteLine("[-] 引継ぎパスワードの設定に失敗しました");
                Console.ReadLine();
                return;
            }

            api = new CGSSAPI(udid, viewerId, userId);

            arg = new Dictionary<string, object>()
            {
                { "app_type", 0 },
                { "campaign_data", "" },
                { "campaign_user", 1468 },
                { "campaign_sign", Binary.md5("All your APIs are belong to us.") }
            };

            api.Call(arg, "/load/check");
            dic = JsonMapper.ToObject(api.Call(arg, "/load/index"));

            if (dic["data_headers"].ContainsKey("required_res_ver"))
            {
                string resVer = (string)dic["data_headers"]["required_res_ver"];
                Console.WriteLine("[*] 新しいリソースバージョンを検出しました");
                Console.WriteLine("[*] " + api.GetResourceVersion() + " -> " + resVer);
                api.SetResourceVersion(int.Parse(resVer));
                dic = JsonMapper.ToObject(api.Call(arg, "/load/index"));
            }

            if ((int)dic["data_headers"]["result_code"] != 1)
            {
                Console.WriteLine("[-] ログインに失敗しました");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("[+] ログインに成功しました");

            arg = new Dictionary<string, object>()
            {
                { "time_filter", 0 },
                { "type_filter", 0 },
                { "attr_info", 0 },
                { "rare_info", 0 },
                { "desc_flag", 1 },
                { "page", 1 },
                { "check_all_present", 1 }
            };

            dic = JsonMapper.ToObject(api.Call(arg, "/present/index2"));

            if ((int)dic["data_headers"]["result_code"] == 1)
            {
                Console.WriteLine("[+] プレゼントのチェックに成功しました");
            }
            else
            {
                Console.Write("[-] プレゼントのチェックに失敗しました");
                Console.ReadLine();
                return;
            }

            arg = new Dictionary<string, object>()
            {
                { "time_filter", 0 },
                { "type_filter", 0 },
                { "attr_info", 0 },
                { "rare_info", 0 },
                { "desc_flag", 1 },
                { "unopened_count_under_100", 0 }
            };

            dic = JsonMapper.ToObject(api.Call(arg, "/present/receive_all"));

            if ((int)dic["data_headers"]["result_code"] == 1)
            {
                Console.WriteLine("[+] プレゼントの受け取りに成功しました");
            }
            else
            {
                Console.Write("[-] プレゼントの受け取りに失敗しました");
                Console.ReadLine();
                return;
            }

            arg = new Dictionary<string, object>()
            {
                { "time_filter", 0 },
                { "type_filter", 0 },
                { "attr_info", 0 },
                { "rare_info", 0 },
                { "desc_flag", 1 },
                { "page", 1 },
                { "check_all_present", 0 }
            };

            Console.WriteLine("[+] ガシャ一覧を取得します");

            arg = new Dictionary<string, object>()
            {
                { "need_latte_art", true }
            };

            dic = JsonMapper.ToObject(api.Call(arg, "/gacha/index"));
            int gachaId = -1;

            if ((int)dic["data_headers"]["result_code"] == 1)
            {
                List<int> gachaList = new List<int>();
                for (int i = 0; i < dic["data"]["open_gacha_list"].Count; i++)
                {
                    int id = (int)dic["data"]["open_gacha_list"][i]["gacha_id"];
                    Console.WriteLine("[*] " + id);
                    gachaList.Add(id);
                }

                while (true)
                {
                    Console.WriteLine("[+] どのガシャを引きますか？");
                    Console.Write(">> ");
                    gachaId = int.Parse(Console.ReadLine());
                    if (gachaList.Contains(gachaId)) break;
                    else Console.WriteLine("[-] ガシャ ID が見つかりません");
                }
            }
            else
            {
                Console.Write("[-] ガシャ一覧の取得に失敗しました");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("[+] ガシャを実行します");
            int jewels = 8300; // TODO: Get jewels from webapis

            for (int i = jewels; i >= 2500; i -= 2500)
            {
                arg = new Dictionary<string, object>()
                {
                    { "gacha_id", gachaId }, // TODO: 30765 recommend
                    { "number", 10 },
                    { "draw_type", 2 },
                    { "own_num", i },
                    { "load_source_gacha_id", 0 },
                    { "filter_card_list", null }
                };
                dic = JsonMapper.ToObject(api.Call(arg, "/gacha/exec"));

                if((int)dic["data_headers"]["result_code"] == 1)
                {
                    Console.WriteLine("=== ガシャ結果 ===");
                    for (int card = 0; card < dic["data"]["card_list"].Count; card++)
                    {
                        Console.WriteLine("[*] カードID : " + dic["data"]["card_list"][card]["card_id"]);
                    }
                    Console.WriteLine("==================");
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("[-] ガシャを引くことができませんでした");
                    Console.ReadLine();
                    return;
                }
            }

            Console.WriteLine("[+] ガシャを終了します");
            Console.ReadLine();
        }

        static Dictionary<string, object> GetTutorialArgs(int step)
        {
            Dictionary<string, object> tutorialArgs = new Dictionary<string, object>()
            {
                { "step", step },
                { "type", 0 },
                { "skip", 0 },
                { "room_info", new Dictionary<string, object>()
                    {
                        { "floor", new List<Dictionary<string, int>>() {
                                new Dictionary<string, int>() {
                                    { "serial_id", 1 },
                                    { "index", 110 },
                                    { "reversal", 0 },
                                    { "sort_num", 2 }
                                },
                                new Dictionary<string, int>() {
                                    { "serial_id", 2 },
                                    { "index", 62 },
                                    { "reversal", 0 },
                                    { "sort_num", 3 }
                                },
                                new Dictionary<string, int>() {
                                    { "serial_id", 3 },
                                    { "index", 196 },
                                    { "reversal", 0 },
                                    { "sort_num", 4 }
                                },
                                new Dictionary<string, int>() {
                                    { "serial_id", 4 },
                                    { "index", 199 },
                                    { "reversal", 0 },
                                    { "sort_num", 5 }
                                },
                                new Dictionary<string, int>() {
                                    { "serial_id", 5 },
                                    { "index", 170 },
                                    { "reversal", 0 },
                                    { "sort_num", 7 }
                                },
                                new Dictionary<string, int>() {
                                    { "serial_id", 9 },
                                    { "index", 1 },
                                    { "reversal", 1 },
                                    { "sort_num", 6 }
                                },
                        }},
                        { "wall", new List<Dictionary<string, int>>() {
                        }},
                        { "theme", new List<Dictionary<string, int>>() {
                            new Dictionary<string, int>() {
                                { "serial_id", 6 },
                                { "index", 0 },
                                { "reversal", 0 },
                                { "sort_num", 0 }
                            },
                            new Dictionary<string, int>() {
                                { "serial_id", 7 },
                                { "index", 1 },
                                { "reversal", 0 },
                                { "sort_num", 0 }
                            },
                            new Dictionary<string, int>() {
                                { "serial_id", 8},
                                { "index", 2 },
                                { "reversal", 0 },
                                { "sort_num", 0 }
                            },
                        }}
                    }
                },
                { "storage_info", new Dictionary<string, object>() {
                        { "diff_in", new List<int>() { } },
                        { "diff_out", new List<int>() { } }
                    }
                },
                { "name", "" }
            };
            return tutorialArgs;
        }
    }
}
