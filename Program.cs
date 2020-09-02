using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using System.Xml;

namespace LIssSpider
{
    class Program
    {
        static void Main()
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
            //Regex Titlereg = new Regex(@"(?<=<h1 class=""entry-title"">)(.*)(?=</h1>)");

            //匹配出封面图
            //Regex TitleImgreg = new Regex(@"(?<=<!--// MetaSlider--><p><img class=""aligncenter size-full wp-image-\d*"" src="")(.*\.(jpg|png))(?="" )");

            //匹配出说明
            //Regex Desreg1 = new Regex(@"(?<=/>)(.*)(?=<span id=""more-\d*""></span></p>)");
            //Regex Desreg2 = new Regex(@"(?<=<span id=""more-\d*""></span></p>\n<p>)(.*)(?=</p>)");

            //匹配出配图
            //Regex DesImgreg = new Regex(@"(?<=</p>\n<p><img class=""aligncenter size-full wp-image-\d*"" src="")(.*\.(jpg|png))(?="" )");

            //匹配出标签
            //Regex Tagreg = new Regex(@"(?<=rel=""tag"">)(.{0,20})(?=</a>)");
            #endregion

            //string mainPage = "https://hacg.tw/";
            //https://www.liuli.se/wp/comic.html/page/2
            string mainPage = File.ReadAllText("MainPageUrl.txt");

            string readme = "D:\\桌面\\Repos\\1.DemoProjects(for test only)\\LIssSpider\\README.md";//修改git readme
            string viewed_Posts = Environment.CurrentDirectory + "\\viewed_Posts.txt";
            string magnet_urls = Environment.CurrentDirectory + "\\"+DateTime.Today.ToString("yyyyMMdd")+"-magnet_url.md";
            

            if (!File.Exists(viewed_Posts))
            {
                File.Create(viewed_Posts).Dispose();
            }
            if (!File.Exists(magnet_urls))
            {
                File.Create(magnet_urls).Dispose();
                StreamWriter sw_md = File.AppendText(magnet_urls);
                sw_md.WriteLine(@$"---
title: LLSS {DateTime.Now:yyyyMMdd}
date: {DateTime.Now}
tags:  琉璃神社
---");
                sw_md.Flush();
                sw_md.Close();
            }


            
            List<string> PostUrls = new List<string>();
            List<string> urls = new List<string>
            {
                mainPage + "wp/category/all/comic/"//第一页的地址
            };
            for (int i = 2; i < 5; i++)
            {
                urls.Add(@$"{mainPage}wp/category/all/comic/page/{i}/");//第二页后的地址拼接
            }

            foreach (string url in urls)
            {//拿取文章实际链接
                Thread.Sleep(500);
                string html = GetHtml(url, out string msg);
                
                Regex PostsUrlreg = new Regex(@"(?<=<h1 class=""entry-title""><a href="")(.*)(?="" rel=""bookmark"">(.*)</a></h1>)");
                foreach (object Url in PostsUrlreg.Matches(html))
                {
                    PostUrls.Add(Url.ToString());
                }
                if (PostUrls.Count==0)
                {
                    break;
                }
                if (File.ReadAllText(viewed_Posts).Contains(PostUrls[PostUrls.Count - 1]))
                {
                    break;
                }
            }

            foreach (string PostsUrl in PostUrls)
            {
                string vieweing_Post_url = PostsUrl.Replace($"{mainPage}/wp/", "");
                if (File.ReadAllText(viewed_Posts).Contains(vieweing_Post_url))
                {//判断存档中是否包含
                    continue;
                }
                Thread.Sleep(500);
                StreamWriter sw_viewed_Posts = File.AppendText(viewed_Posts);
                StreamWriter sw_magnet_urls = File.AppendText(magnet_urls);
                string postshtml = GetHtml(PostsUrl,out _);


                Regex Titlereg = //标题
                    new Regex(@"(?<=<h1 class=""entry-title"">)(.*)(?=</h1>)");
                Regex Authorreg = //作者
                    new Regex(@"(?<=rel=""author"">)(.*)(?=</a>)");
                Regex TitleImgreg = //标题配图
                    new Regex(@"(?<=<!--// MetaSlider--><p><img class=""aligncenter size-full wp-image-\d*"" src="")(.*\.(jpg|png))(?="" )");
                Regex Desreg1 = //描述
                    new Regex(@"(?<=<p>)(.*)(?=<span id=""more-\d*""></span></p>)");
                Regex Desreg2 = //简介
                    new Regex(@"(?<=</p>\n<p>)(.*)(?=</p>\n<p>)");
                Regex DesImgreg = //简介配图
                    new Regex(@"(?<=</p>\n<p><img class=""aligncenter size-full wp-image-\d*"" src="")(.*\.(jpg|png))(?="" )");
                Regex Tagreg = //标签
                    new Regex(@"(?<=rel=""tag"">)(.{0,20})(?=</a>)");
                Regex Magreg = //磁链
                    new Regex(@"(?<=\>)([0-9a-zA-Z]{40})(?=\<)");

                
                foreach (var Title in Titlereg.Matches(postshtml))
                {//标题
                    sw_magnet_urls.WriteLine("### 标题： " + Title);
                }
                foreach (var Author in Authorreg.Matches(postshtml))
                {//作者
                    sw_magnet_urls.WriteLine("作者： "+Author);
                }
                foreach (var TitleImg in TitleImgreg.Matches(postshtml))
                {//标题图
                    sw_magnet_urls.WriteLine("标题图： \n![Alt text](" + TitleImg + ")");
                }
                foreach (object des1 in Desreg1.Matches(postshtml))
                {//描述
                    sw_magnet_urls.WriteLine("* 描述： " + des1);
                }
                foreach (object DesImg in DesImgreg.Matches(postshtml))
                {//简介配图
                    sw_magnet_urls.WriteLine("简介配图： \n![Alt text](" + DesImg + ")");
                }
                string Tags = "";
                foreach (object Tag in Tagreg.Matches(postshtml))
                {//标签
                    Tags += Tag+" ";
                }
                sw_magnet_urls.WriteLine(Tags);
                foreach (object des2 in Desreg2.Matches(postshtml.Replace("<br />\n","")))
                {//简介
                    sw_magnet_urls.WriteLine("* 简介： " + des2);
                }
                foreach (object magnet in Magreg.Matches(postshtml))
                {//磁链
                    sw_magnet_urls.WriteLine("magnet:?xt=urn:btih:" + magnet);
                }
                sw_magnet_urls.WriteLine("* 原文地址：" + PostsUrl + "\n");
                sw_viewed_Posts.WriteLine(vieweing_Post_url);//相对地址写进已读存档
                
                sw_magnet_urls.Flush();
                sw_magnet_urls.Close();
                sw_viewed_Posts.Flush();
                sw_viewed_Posts.Close();
                Console.WriteLine(PostsUrl);
            }

            string writeline = $"\r\n[{DateTime.Today:yyyyMMdd}集合](https://github.com/YatesGuo/LLSS-Spider/blob/master/bin/Debug/netcoreapp3.0/{DateTime.Today.ToString("yyyyMMdd")}-magnet_url.md)";
            if (File.ReadAllLines(magnet_urls).Length<=5)
            {
                File.Delete(magnet_urls);
            }
            if (File.Exists(magnet_urls) && !File.ReadAllText(readme).Contains(writeline))
            {
                StreamWriter ReadmeMd = File.AppendText(readme);
                
                ReadmeMd.WriteLine(writeline);
                ReadmeMd.Flush();
                ReadmeMd.Close();
                File.Copy(magnet_urls, "F:\\A\\source\\_posts\\" + DateTime.Today.ToString("yyyyMMdd") + "-magnet_url.md",true);
                //拷贝到博客
            }


            static string GetHtml(string url,out string msg)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.92 Safari/537.36";
                    request.AllowAutoRedirect = true;
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
