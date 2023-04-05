using DataLayer;
using Dynamitey;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Youtube_Contractor
{
    public class ViewerAPIRequest
    {
        public static async Task<string> GetYoutubeIdAsync(string token)
        {
            string url = "https://www.googleapis.com/youtube/v3/channels?mine=true&access_token=" + token + "&client_id=245884125377-c6kqdrfpr602abhaa8m3g3cqeluctpod.apps.googleusercontent.com";

            return GetUserId(await ExecuteRequest(url));
        }

        public static async Task<string> GetYoutubeProfilePictureAsync(string userId, string token)
        {
            string photoUrl = "https://www.googleapis.com/youtube/v3/channels?part=snippet&fields=items%2Fsnippet%2Fthumbnails%2Fdefault&id=" + userId + "&access_token=" + token + "&client_id=245884125377-c6kqdrfpr602abhaa8m3g3cqeluctpod.apps.googleusercontent.com";
            var response = await ExecuteRequest(photoUrl);

            var photo = Between(response, "\"url\": \"", "\",");
            return photo;
        }

        private static string GetUserId(string html)
        {
            Details details = JsonConvert.DeserializeObject<Details>(html);

            return details.Items[0].Id;
        }

        public static async Task<int> GetMemberLevel(string youtubeToken)
        {
            string requestUrl = $"https://www.googleapis.com/youtube/v3/members?part=snippet&mine=true&mode=all_current&access_token={youtubeToken}&client_id=245884125377-6jv9uqlej0ml5splevbnkpooc7hfclr8.apps.googleusercontent.com";
            var response = await ExecuteRequest(requestUrl);
            //TODO: WAIT FOR GOOGLE APROVAL
            //
            return 0;
        }

        private static async Task<string> ExecuteRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static string Between(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }

        public static async Task<List<TwitchSubscription>> GetTwitchMemberLevelAsync(string twitchChannelId, string token)
        {
            List<TwitchSubscription> toReturn = new List<TwitchSubscription>();
            var twitchHeaders = new Dictionary<string, string>{
                    { "Authorization", $"Bearer {token}" },
                    { "Client-Id", "nhtoulxff6s02iv9kw9ztfmmciqz2r" }
            };

            var url = $"https://api.twitch.tv/helix/subscriptions?broadcaster_id={twitchChannelId}&first=100";

            using (var client = new HttpClient())
            {
                foreach (var header in twitchHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                var response = await client.GetAsync(url);
                var page = await response.Content.ReadFromJsonAsync<TwitchSubscriptionResponse>();
                toReturn = page.data;
                if (page.pagination.cursor != "")
                {
                    do
                    {
                        var recursive_url = $"https://api.twitch.tv/helix/subscriptions?broadcaster_id={twitchChannelId}&first=100&after={page.pagination.cursor}";
                        var recursive_response = await client.GetAsync(recursive_url);
                        page = await recursive_response.Content.ReadFromJsonAsync<TwitchSubscriptionResponse>();
                        toReturn.AddRange(page.data);

                    } while (!string.IsNullOrWhiteSpace(page.pagination.cursor));
                }
            }

            return toReturn;
        }
    }

    public class Details : IEquatable<Details>
    {
        public Details()
        {
        }

        public Item[] Items { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Details);
        }

        public bool Equals(Details other)
        {
            return other is not null &&
                   EqualityComparer<Item[]>.Default.Equals(Items, other.Items);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Items);
        }
    }

    public class Item : IEquatable<Item>
    {
        public Item()
        {
        }

        public string Id { get; set; }
        public string Etag { get; set; }
        public string Kind { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Item);
        }

        public bool Equals(Item other)
        {
            return other is not null &&
                   Id == other.Id &&
                   Etag == other.Etag &&
                   Kind == other.Kind;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Etag, Kind);
        }
    }
}
