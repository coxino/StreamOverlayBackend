using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings
{
    public class ProjectSettings
    {
        public static string DatabaseFolder = "C:/API/database/";
        public static string User_folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public static string CustomThemeFile = "/customTheme.json";
        public static string GamesFile = "/Games.json";
        public static string ProvidersFile = "/Providers.json";
        public static string LiveBonusHuntFile = "/BonusHunts/live.json";    
        public static string TurneeFolder = "/Turnee/";
        public static string TranzactiiFile = "/Tranzactii.json";
        public static string LiveBetting = "/currentBetting.json";
        public static string LiveBettingUserOptions = "/currentBettingUserOptions/";

        public static string InPlayGame = "/LiveSeed/inplay.json";
        public static string TournamentFile = "/Turnee/meciuri.json";
        public static string TournamentLiveGameFile = "/Turnee/livegame.json";

        public static string RoundsFolder = "/Rounds/";

        public static string HotWords = "/HotWords.json";

        public static string LoyalityRanking = "/Loyality/usersPoints.json";
        public static string LoyalityGivewayTokens = "/Loyality/givewayTokens.json";

        public static string JackpotFile = "/Loyality/Jackpot.json";

        public static string CooldownFolder = "/cooldowns/";
        public static string CooldownFile = "_cd.json";

        public static string Shop = "/shop/shop.json";

        public static string RedeemsFile = "/redeems/redeems{0}.json";

        public static string LigaFile = "/liga/ligacurenta.json";
    }
}
