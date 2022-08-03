using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class EventModel : IEquatable<EventModel>
    {
        private string service;
        private EventData data;
        private string renderedText;

        public EventModel()
        {
        }

        public string Service { get => service; set => service = value; }
        public EventData Data { get => data; set => data = value; }
        public string RenderedText { get => renderedText; set => renderedText = value; }

        public override bool Equals(object obj)
        {
            return Equals(obj as EventModel);
        }

        public bool Equals(EventModel other)
        {
            return other != null &&
                   service == other.service &&
                   EqualityComparer<EventData>.Default.Equals(data, other.data) &&
                   renderedText == other.renderedText &&
                   Service == other.Service &&
                   EqualityComparer<EventData>.Default.Equals(Data, other.Data) &&
                   RenderedText == other.RenderedText;
        }

        public override int GetHashCode()
        {
            int hashCode = -161359128;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(service);
            hashCode = hashCode * -1521134295 + EqualityComparer<EventData>.Default.GetHashCode(data);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(renderedText);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Service);
            hashCode = hashCode * -1521134295 + EqualityComparer<EventData>.Default.GetHashCode(Data);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RenderedText);
            return hashCode;
        }
    }

    public class EventData : IEquatable<EventData>
    {
        private string kind;
        private string etag;
        private string id;
        private Snippet snippet;
        private AuthorDetails authorDetails;
        private string msgId;
        private string userId;
        private string nick;
        private List<string> badges;
        private string displayName;
        private bool isAction;
        private long time;
        private List<string> tags;
        private string displayColor;
        private string channel;
        private string text;
        private List<string> emotes;
        private string avatar;

        public EventData()
        {
        }

        public string Kind { get => kind; set => kind = value; }
        public string Etag { get => etag; set => etag = value; }
        public string Id { get => id; set => id = value; }
        public Snippet Snippet { get => snippet; set => snippet = value; }
        public AuthorDetails AuthorDetails { get => authorDetails; set => authorDetails = value; }
        public string MsgId { get => msgId; set => msgId = value; }
        public string UserId { get => userId; set => userId = value; }
        public string Nick { get => nick; set => nick = value; }
        public List<string> Badges { get => badges; set => badges = value; }
        public string DisplayName { get => displayName; set => displayName = value; }
        public bool IsAction { get => isAction; set => isAction = value; }
        public long Time { get => time; set => time = value; }
        public List<string> Tags { get => tags; set => tags = value; }
        public string DisplayColor { get => displayColor; set => displayColor = value; }
        public string Channel { get => channel; set => channel = value; }
        public string Text { get => text; set => text = value; }
        public List<string> Emotes { get => emotes; set => emotes = value; }
        public string Avatar { get => avatar; set => avatar = value; }

        public override bool Equals(object obj)
        {
            return Equals(obj as EventData);
        }

        public bool Equals(EventData other)
        {
            return other != null &&
                   kind == other.kind &&
                   etag == other.etag &&
                   id == other.id &&
                   EqualityComparer<Snippet>.Default.Equals(snippet, other.snippet) &&
                   EqualityComparer<AuthorDetails>.Default.Equals(authorDetails, other.authorDetails) &&
                   msgId == other.msgId &&
                   userId == other.userId &&
                   nick == other.nick &&
                   EqualityComparer<List<string>>.Default.Equals(badges, other.badges) &&
                   displayName == other.displayName &&
                   isAction == other.isAction &&
                   time == other.time &&
                   EqualityComparer<List<string>>.Default.Equals(tags, other.tags) &&
                   displayColor == other.displayColor &&
                   channel == other.channel &&
                   text == other.text &&
                   EqualityComparer<List<string>>.Default.Equals(emotes, other.emotes) &&
                   avatar == other.avatar &&
                   Kind == other.Kind &&
                   Etag == other.Etag &&
                   Id == other.Id &&
                   EqualityComparer<Snippet>.Default.Equals(Snippet, other.Snippet) &&
                   EqualityComparer<AuthorDetails>.Default.Equals(AuthorDetails, other.AuthorDetails) &&
                   MsgId == other.MsgId &&
                   UserId == other.UserId &&
                   Nick == other.Nick &&
                   EqualityComparer<List<string>>.Default.Equals(Badges, other.Badges) &&
                   DisplayName == other.DisplayName &&
                   IsAction == other.IsAction &&
                   Time == other.Time &&
                   EqualityComparer<List<string>>.Default.Equals(Tags, other.Tags) &&
                   DisplayColor == other.DisplayColor &&
                   Channel == other.Channel &&
                   Text == other.Text &&
                   EqualityComparer<List<string>>.Default.Equals(Emotes, other.Emotes) &&
                   Avatar == other.Avatar;
        }

        public override int GetHashCode()
        {
            int hashCode = -504636174;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(kind);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(etag);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
            hashCode = hashCode * -1521134295 + EqualityComparer<Snippet>.Default.GetHashCode(snippet);
            hashCode = hashCode * -1521134295 + EqualityComparer<AuthorDetails>.Default.GetHashCode(authorDetails);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(msgId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(userId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(nick);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(badges);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(displayName);
            hashCode = hashCode * -1521134295 + isAction.GetHashCode();
            hashCode = hashCode * -1521134295 + time.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(tags);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(displayColor);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(channel);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(text);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(emotes);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(avatar);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Kind);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Etag);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<Snippet>.Default.GetHashCode(Snippet);
            hashCode = hashCode * -1521134295 + EqualityComparer<AuthorDetails>.Default.GetHashCode(AuthorDetails);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MsgId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nick);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(Badges);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayName);
            hashCode = hashCode * -1521134295 + IsAction.GetHashCode();
            hashCode = hashCode * -1521134295 + Time.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(Tags);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayColor);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Channel);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(Emotes);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Avatar);
            return hashCode;
        }
    }


    public class TextMessageDetails
    {
        public string messageText { get; set; }
    }


    public class Snippet : IEquatable<Snippet>
    {
        private string type;
        private string liveChatId;
        private string authorChannelId;
        private DateTime publishedAt;
        private bool hasDisplayContent;
        private string displayMessage;
        private TextMessageDetails textMessageDetails;

        public Snippet()
        {
        }

        public string Type { get => type; set => type = value; }
        public string LiveChatId { get => liveChatId; set => liveChatId = value; }
        public string AuthorChannelId { get => authorChannelId; set => authorChannelId = value; }
        public DateTime PublishedAt { get => publishedAt; set => publishedAt = value; }
        public bool HasDisplayContent { get => hasDisplayContent; set => hasDisplayContent = value; }
        public string DisplayMessage { get => displayMessage; set => displayMessage = value; }
        public TextMessageDetails TextMessageDetails { get => textMessageDetails; set => textMessageDetails = value; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Snippet);
        }

        public bool Equals(Snippet other)
        {
            return other != null &&
                   type == other.type &&
                   liveChatId == other.liveChatId &&
                   authorChannelId == other.authorChannelId &&
                   publishedAt == other.publishedAt &&
                   hasDisplayContent == other.hasDisplayContent &&
                   displayMessage == other.displayMessage &&
                   EqualityComparer<TextMessageDetails>.Default.Equals(textMessageDetails, other.textMessageDetails) &&
                   Type == other.Type &&
                   LiveChatId == other.LiveChatId &&
                   AuthorChannelId == other.AuthorChannelId &&
                   PublishedAt == other.PublishedAt &&
                   HasDisplayContent == other.HasDisplayContent &&
                   DisplayMessage == other.DisplayMessage &&
                   EqualityComparer<TextMessageDetails>.Default.Equals(TextMessageDetails, other.TextMessageDetails);
        }

        public override int GetHashCode()
        {
            int hashCode = -1313760852;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(liveChatId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(authorChannelId);
            hashCode = hashCode * -1521134295 + publishedAt.GetHashCode();
            hashCode = hashCode * -1521134295 + hasDisplayContent.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(displayMessage);
            hashCode = hashCode * -1521134295 + EqualityComparer<TextMessageDetails>.Default.GetHashCode(textMessageDetails);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LiveChatId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AuthorChannelId);
            hashCode = hashCode * -1521134295 + PublishedAt.GetHashCode();
            hashCode = hashCode * -1521134295 + HasDisplayContent.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayMessage);
            hashCode = hashCode * -1521134295 + EqualityComparer<TextMessageDetails>.Default.GetHashCode(TextMessageDetails);
            return hashCode;
        }
    }

    public class AuthorDetails : IEquatable<AuthorDetails>
    {
        private string channelId;
        private string channelUrl;
        private string displayName;
        private string profileImageUrl;
        private bool isVerified;
        private bool isChatOwner;
        private bool isChatSponsor;
        private bool isChatModerator;

        public string ChannelId { get => channelId; set => channelId = value; }
        public string ChannelUrl { get => channelUrl; set => channelUrl = value; }
        public string DisplayName { get => displayName; set => displayName = value; }
        public string ProfileImageUrl { get => profileImageUrl; set => profileImageUrl = value; }
        public bool IsVerified { get => isVerified; set => isVerified = value; }
        public bool IsChatOwner { get => isChatOwner; set => isChatOwner = value; }
        public bool IsChatSponsor { get => isChatSponsor; set => isChatSponsor = value; }
        public bool IsChatModerator { get => isChatModerator; set => isChatModerator = value; }

        public AuthorDetails()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AuthorDetails);
        }

        public bool Equals(AuthorDetails other)
        {
            return other != null &&
                   channelId == other.channelId &&
                   channelUrl == other.channelUrl &&
                   displayName == other.displayName &&
                   profileImageUrl == other.profileImageUrl &&
                   isVerified == other.isVerified &&
                   isChatOwner == other.isChatOwner &&
                   isChatSponsor == other.isChatSponsor &&
                   isChatModerator == other.isChatModerator &&
                   ChannelId == other.ChannelId &&
                   ChannelUrl == other.ChannelUrl &&
                   DisplayName == other.DisplayName &&
                   ProfileImageUrl == other.ProfileImageUrl &&
                   IsVerified == other.IsVerified &&
                   IsChatOwner == other.IsChatOwner &&
                   IsChatSponsor == other.IsChatSponsor &&
                   IsChatModerator == other.IsChatModerator;
        }

        public override int GetHashCode()
        {
            int hashCode = -1288369912;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(channelId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(channelUrl);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(displayName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(profileImageUrl);
            hashCode = hashCode * -1521134295 + isVerified.GetHashCode();
            hashCode = hashCode * -1521134295 + isChatOwner.GetHashCode();
            hashCode = hashCode * -1521134295 + isChatSponsor.GetHashCode();
            hashCode = hashCode * -1521134295 + isChatModerator.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ChannelId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ChannelUrl);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProfileImageUrl);
            hashCode = hashCode * -1521134295 + IsVerified.GetHashCode();
            hashCode = hashCode * -1521134295 + IsChatOwner.GetHashCode();
            hashCode = hashCode * -1521134295 + IsChatSponsor.GetHashCode();
            hashCode = hashCode * -1521134295 + IsChatModerator.GetHashCode();
            return hashCode;
        }
    }    
}

