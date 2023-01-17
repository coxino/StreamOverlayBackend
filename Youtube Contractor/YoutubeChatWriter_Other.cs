using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Youtube_Contractor
{
    public class YoutubeChatWriterOther
    {

        public async Task WriteMessageAsync(string message)
        {
            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "YOUR_API_KEY",
                });

                LiveChatMessageSnippet mySnippet = new LiveChatMessageSnippet();
                LiveChatMessage comments = new LiveChatMessage();
                LiveChatTextMessageDetails txtDetails = new LiveChatTextMessageDetails();
                txtDetails.MessageText = message;
                mySnippet.TextMessageDetails = txtDetails;
                mySnippet.LiveChatId = "";
                mySnippet.Type = "textMessageEvent";
                comments.Snippet = mySnippet;
                comments = await youtubeService.LiveChatMessages.Insert(comments, "snippet").ExecuteAsync();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
