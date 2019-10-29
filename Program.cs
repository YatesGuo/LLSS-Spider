using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace LIssSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            #region
            //comic主页面
            //http://www.llss.life/wp/category/all/comic/
            //第二页
            //http://www.llss.life/wp/category/all/comic/page/2/

            //匹配每篇文章实际地址
            //http://www.llss.life/wp/all/comic/%e7%a8%b2%e8%91%89-%e3%81%bf%e3%81%ae%e3%82%8a-%e6%ba%90%e5%90%9b%e7%89%a9%e8%af%ad%e3%80%90%e5%ae%8c%e7%bb%93%e3%80%91%ef%bc%881-358%e8%af%9d%ef%bc%89/
            //Regex PostsUrlreg = new Regex(@"(?<=<h1 class=""entry-title""><a href="")(.*)(?="" rel=""bookmark"">(.*)</a></h1>)");

            //匹配出磁链 magnet:?xt=urn:btih:
            //Regex Magreg = new Regex(@"(?<=\>)([0-9a-zA-Z]{40})(?=\<)");
            
            //匹配出标题
            Regex Titlereg = new Regex(@"(?<=<h1 class=""entry-title"">)(.*)(?=</h1>)");
           
            //匹配出封面图
            Regex TitleImgreg = new Regex(@"(?<=<!--// MetaSlider--><p><img class=""aligncenter size-full wp-image-\d*"" src="")(.*\.(jpg|png))(?="" )");
            
            //匹配出说明
            Regex Desreg1 = new Regex(@"(?<=/>)(.*)(?=<span id=""more-\d*""></span></p>)");
            Regex Desreg2 = new Regex(@"(?<=<span id=""more-\d*""></span></p>\n<p>)(.*)(?=</p>)");
            
            //匹配出配图
            Regex DesImgreg = new Regex(@"(?<=</p>\n<p><img class=""aligncenter size-full wp-image-\d*"" src="")(.*\.(jpg|png))(?="" )");
            
            //匹配出标签
            Regex Tagreg = new Regex(@"(?<=rel=""tag"">)(.{0,20})(?=</a>)");
            #endregion

            List<string> PostUrls = new List<string>();
            List<string> urls = new List<string>();
            List<string> Megs = new List<string>();
            urls.Add("http://www.llss.life/wp/category/all/comic/");
            for (int i = 2; i < 2; i++)
            {
                urls.Add(@$"http://www.llss.life/wp/category/all/comic/page/{i}/");
            }

            foreach (string url in urls)
            {
                Thread.Sleep(500);
                string html = GetHtml(url, out string msg);
                
                Regex PostsUrlreg = new Regex(@"(?<=<h1 class=""entry-title""><a href="")(.*)(?="" rel=""bookmark"">(.*)</a></h1>)");
                foreach (object Url in PostsUrlreg.Matches(html))
                {
                    PostUrls.Add(Url.ToString());
                }
            }

            foreach (string PostsUrl in PostUrls)
            {
                string postshtml = GetHtml(PostsUrl,out string msg);
                Regex Magreg = new Regex(@"(?<=\>)([0-9a-zA-Z]{40})(?=\<)");
                foreach (object magnet in Magreg.Matches(postshtml))
                {
                    Megs.Add("magnet:?xt=urn:btih:" + magnet.ToString());
                }
            }

            static string GetHtml(string url,out string msg)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.92 Safari/537.36";
                    request.AllowAutoRedirect = false;
                    request.Timeout = 30000;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string html = sr.ReadToEnd();
                    msg = response.StatusCode.ToString();
                    sr.Close();
                    return html;
                }
                catch (Exception e)
                {
                    msg = e.ToString();
                    return e.Message;
                }
            }
        }
    }
}
