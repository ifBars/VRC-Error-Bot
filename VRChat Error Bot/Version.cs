using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VRChat_Error_Bot
{
    class Version
    {

        public string v;
        public string vType;
        public string newV;
        public string url;
        public Release latestRelease;

        public async Task CheckAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("my-user-agent-string");
                var response = await client.GetAsync("https://api.github.com/repos/ifBars/ProTVConverter/releases");
                var content = await response.Content.ReadAsStringAsync();
                JArray releases = JArray.Parse(content);
                string htmlUrl = releases[0]["html_url"].Value<string>();
                int startIndex = htmlUrl.IndexOf("tag/") + "tag/".Length;
                string version = htmlUrl.Substring(startIndex);
                newV = version;
                url = htmlUrl;
            }
        }

        public async Task<bool> CheckUpdateAsync()
        {

            await CheckAsync();

            if (v == newV)
            {
                return false;
            } else
            {
                return true;
            }

        }

    }
}
