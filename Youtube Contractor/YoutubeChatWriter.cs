using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Youtube_Contractor
{
    public class YoutubeChatWriter
    {
        private CancellationToken asdasd;

        public async Task WriteMessageAsync(string message)
        {
            try
            {
                UserCredential credential;
                using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        // This OAuth 2.0 access scope allows for full read/write access to the
                        // authenticated user's account.
                        new[] { YouTubeService.Scope.Youtube },
                        "user",
                        CancellationToken.None,                        
                        new FileDataStore(this.GetType().ToString())
                    );

                }

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = this.GetType().ToString()
                });



                var live = youtubeService.LiveBroadcasts.List("snippet");
                live.Mine = true;

                var liveX = await live.ExecuteAsync();


                LiveChatMessageSnippet mySnippet = new LiveChatMessageSnippet();
                LiveChatMessage comments = new LiveChatMessage();
                LiveChatTextMessageDetails txtDetails = new LiveChatTextMessageDetails();
                txtDetails.MessageText = message;
                mySnippet.TextMessageDetails = txtDetails;
                mySnippet.LiveChatId = liveX.Items[0].Snippet.LiveChatId;
                mySnippet.Type = "textMessageEvent";
                comments.Snippet = mySnippet;
                comments = await youtubeService.LiveChatMessages.Insert(comments, "snippet").ExecuteAsync();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("c:/logareYT/failed.txt", $"{DateTime.Now} - ERROR - {message} \r\n {ex.Message} \r\n");
            }
        }
    }
}
