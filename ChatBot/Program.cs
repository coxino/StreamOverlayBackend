using Google.Apis.Services;
using Google.Apis.Discovery;
using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;

using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using System.IO;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using System.Threading;
using Google.Apis.Util.Store;
using Youtube_Contractor;

namespace ChatBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            YoutubeChatWriterOther youtubeChatWriter = new YoutubeChatWriterOther();
             await youtubeChatWriter.WriteMessageAsync("hello");
        }
    }
}
