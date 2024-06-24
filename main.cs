using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;

namespace HttpRequest_Bai1_GetDataFrom_HowKteam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Crawl
            /*
             HttpClient
             HttpWebClient
             WebClient
             HttpWebRequest
             HttpRequest
             */

            var html = GetData("https://bodergatez.dsrcgoms.net/user/login.aspx");
            TestData(html);
        }

        void TestData(string html)
        {
            File.WriteAllText("res.html", html);
            Process.Start("res.html");
        }
              

        private void button2_Click(object sender, EventArgs e)
        {
            string cookie = "";
            var html = GetData("https://bodergatez.dsrcgoms.net/user/login.aspx", null, null, cookie);
            TestData(html);
        }

        #region Kteam code
        void AddCookie(HttpRequest http, string cookie)
        {
            var temp = cookie.Split(';');
            foreach (var item in temp)
            {
                var temp2 = item.Split('=');
                if (temp2.Count() > 1)
                {
                    http.Cookies.Add(temp2[0], temp2[1]);
                }
            }
        }

        string GetData(string url, HttpRequest http = null, string userArgent = "", string cookie = null)
        {
            if (http == null)
            {
                http = new HttpRequest();
                http.Cookies = new CookieDictionary();
            }

            if (!string.IsNullOrEmpty(cookie))
            {
                AddCookie(http, cookie);
            }

            if (!string.IsNullOrEmpty(userArgent))
            {
                http.UserAgent = userArgent;
            }
            string html = http.Get(url).ToString();
            return html;
        }

        string GetLoginDataToken(string html)
        {
            string token = "";

            var res =  Regex.Matches(html, @"(?<=__RequestVerificationToken"" type=""hidden"" value="").*?(?="")", RegexOptions.Singleline);

            if (res !=null && res.Count > 0)
            {
                token = res[1].ToString();
            }

            return token;
        }

        string PostData(HttpRequest http, string url, string data = null, string contentType = null, string userArgent = "", string cookie = null)
        {
            if (http == null)
            {
                http = new HttpRequest();
                http.Cookies = new CookieDictionary();
            }

            if (!string.IsNullOrEmpty(cookie))
            {
                AddCookie(http, cookie);
            }

            if (!string.IsNullOrEmpty(userArgent))
            {
                http.UserAgent = userArgent;
            }

            string html = http.Post(url, data, contentType).ToString();
            return html;
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            HttpRequest http = new HttpRequest();
            http.Cookies = new CookieDictionary();
            string userArgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";

            var html = GetData("https://bodergatez.dsrcgoms.net/user/login.aspx", http, userArgent);


            string token = GetLoginDataToken(html);

            string userName = "";
            string password = "";
            string data = "__RequestVerificationToken="+token+"&Email="+ WebUtility.UrlEncode(userName) + "&Password="+ WebUtility.UrlEncode(password) + "&RememberMe=true&RememberMe=false";
            html = PostData(http, "https://bodergatez.dsrcgoms.net/user/login.aspx", data, "application/x-www-form-urlencoded; charset=UTF-8").ToString();

            html = GetData("https://bodergatez.dsrcgoms.net/user/login.aspx", http, userArgent);

            File.WriteAllText(html["r_token"], html);
            Process.Start(html["r_token"]);
        }
    }
}
