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
using System.Text;
using System.Threading.Tasks;

namespace Youtube_Contractor
{
    public class YoutubeViewerAuth
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

        public static async Task<int> GetMemberLevel(string youtubeToken, string viewerId)
        {
            string requestUrl = "https://www.googleapis.com/youtube/v3/members?part=snippet&mode=all_current&access_token=" + youtubeToken + "&client_id=245884125377-c6kqdrfpr602abhaa8m3g3cqeluctpod.apps.googleusercontent.com";
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
