using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SitePluginCommon;
using System.Windows;

namespace MirrativSitePlugin
{
    public class MirrativServer : ServerBase, IDataServer
    {
        public async Task<string> GetAsync(string url, Dictionary<string, string> headers)
        {
            string text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "mr_id.txt")).Trim();
			if (text.Length < 5 || text.Length > 100)
			{
				throw new InvalidDataException("mr_id.txtの中身が空、または変です");
			}
            if (text.Length != 64)
            {
                MessageBox.Show("mr_id.txtの中身が64文字ではありません。もしかしたら誤った値をmr_id.txtに保存しているかもしれません");
            }
			string host = "www.mirrativ.com";

			using (TcpClient tcpClient = new TcpClient(host, 443))
			{
				using (SslStream sslStream = new SslStream(tcpClient.GetStream(), false))
				{
					sslStream.AuthenticateAsClient(host);
					StreamWriter streamWriter = new StreamWriter(sslStream, Encoding.ASCII);
					streamWriter.NewLine = "\r\n";
					streamWriter.AutoFlush = true;
					streamWriter.WriteLine("GET " + url + " HTTP/1.1");
					streamWriter.WriteLine("Host: " + host);
					streamWriter.WriteLine("User-Agent: MultiCommentViewerPatched/v6.5.1/contact_twitter.at_mark.y52_dev");
					streamWriter.WriteLine("Accept: application/json");
					streamWriter.WriteLine("Connection: close");
					streamWriter.WriteLine("cookie: mr_id=" + text);
					streamWriter.WriteLine();
					StreamReader streamReader = new StreamReader(sslStream);
					string header;
					while ((header = streamReader.ReadLine()) != null && !(header == ""))
					{
					}
					return streamReader.ReadToEnd();
				}
			}
        }
        public async Task<string> GetAsync(string url, Dictionary<string, string> headers, CookieContainer cc)
        {
            var result = await GetInternalAsync(new HttpOptions
            {
                Url = url,
                Cc = cc,
                Headers = headers,
            });
            var str = await result.Content.ReadAsStringAsync();
            return str;
        }
        public async Task<string> GetAsync(string url)
        {
            var result = await GetInternalAsync(new HttpOptions
            {
                Url = url,
            });
            var str = await result.Content.ReadAsStringAsync();
            return str;
        }
        public async Task<string> PostAsync(string url, Dictionary<string, string> data, CookieContainer cc)
        {
            var content = new FormUrlEncodedContent(data);
            var result = await PostInternalAsync(new HttpOptions
            {
                Url = url,
                Cc = cc,
            }, content);
            var str = await result.Content.ReadAsStringAsync();
            return str;
        }
    }
}
