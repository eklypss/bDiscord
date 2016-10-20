using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace bDiscord.Classes
{
    public static class PasteManager
    {
        public static string CreatePaste(string name, string content)
        {
            NameValueCollection pastebinLogin = new NameValueCollection();
            pastebinLogin.Add("api_dev_key", APIKeys.Pastebin);
            pastebinLogin.Add("api_user_name", APIKeys.PastebinUser);
            pastebinLogin.Add("api_user_password", APIKeys.PastebinPassword);
            string key = APIKeys.Pastebin;
            using (WebClient web = new WebClient())
            {
                key = Encoding.UTF8.GetString(web.UploadValues("http://pastebin.com/api/api_login.php", pastebinLogin));
            }
            NameValueCollection pastebinQuery = new NameValueCollection();
            pastebinQuery.Add("api_dev_key", APIKeys.Pastebin);
            pastebinQuery.Add("api_option", "paste");
            pastebinQuery.Add("api_user_key", key);
            pastebinQuery.Add("api_paste_expire_date", "1H");
            pastebinQuery.Add("api_paste_private", "1");
            pastebinQuery.Add("api_paste_name", name);
            pastebinQuery.Add("api_paste_code", content);

            using (WebClient web = new WebClient())
            {
                string response = Encoding.UTF8.GetString(web.UploadValues("http://pastebin.com/api/api_post.php", pastebinQuery));
                return response;
            }
        }
    }
}