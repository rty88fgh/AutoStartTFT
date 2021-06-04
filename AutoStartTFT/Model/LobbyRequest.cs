using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartTFT.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class GameTypeConfig
    {
        public bool AdvancedLearningQuests { get; set; }
        public bool AllowTrades { get; set; }
        public string BanMode { get; set; }
        public int BanTimerDuration { get; set; }
        public bool BattleBoost { get; set; }
        public bool CrossTeamChampionPool { get; set; }
        public bool DeathMatch { get; set; }
        public bool DoNotRemove { get; set; }
        public bool DuplicatePick { get; set; }
        public bool ExclusivePick { get; set; }
        public string GameModeOverride { get; set; }
        public int Id { get; set; }
        public bool LearningQuests { get; set; }
        public int MainPickTimerDuration { get; set; }
        public int MaxAllowableBans { get; set; }
        public string Name { get; set; }
        public int NumPlayersPerTeamOverride { get; set; }
        public bool OnboardCoopBeginner { get; set; }
        public string PickMode { get; set; }
        public int PostPickTimerDuration { get; set; }
        public bool Reroll { get; set; }
        public bool TeamChampionPool { get; set; }
    }

    public class Mutators
    {
        public bool AdvancedLearningQuests { get; set; }
        public bool AllowTrades { get; set; }
        public string BanMode { get; set; }
        public int BanTimerDuration { get; set; }
        public bool BattleBoost { get; set; }
        public bool CrossTeamChampionPool { get; set; }
        public bool DeathMatch { get; set; }
        public bool DoNotRemove { get; set; }
        public bool DuplicatePick { get; set; }
        public bool ExclusivePick { get; set; }
        public string GameModeOverride { get; set; }
        public int Id { get; set; }
        public bool LearningQuests { get; set; }
        public int MainPickTimerDuration { get; set; }
        public int MaxAllowableBans { get; set; }
        public string Name { get; set; }
        public int NumPlayersPerTeamOverride { get; set; }
        public bool OnboardCoopBeginner { get; set; }
        public string PickMode { get; set; }
        public int PostPickTimerDuration { get; set; }
        public bool Reroll { get; set; }
        public bool TeamChampionPool { get; set; }
    }

    public class Configuration
    {
        public string GameMode { get; set; }
        public string GameServerRegion { get; set; }
        public GameTypeConfig GameTypeConfig { get; set; }
        public int MapId { get; set; }
        public int MaxPlayerCount { get; set; }
        public Mutators Mutators { get; set; }
        public string SpectatorPolicy { get; set; }
        public int TeamSize { get; set; }
        public string TournamentGameMode { get; set; }
        public string TournamentPassbackDataPacket { get; set; }
        public string TournamentPassbackUrl { get; set; }
    }

    public class PositionPreferences
    {
        public string FirstPreference { get; set; }
        public string SecondPreference { get; set; }
    }

    public class Spectator
    {
        public bool AutoFillEligible { get; set; }
        public bool AutoFillProtectedForPromos { get; set; }
        public bool AutoFillProtectedForSoloing { get; set; }
        public bool AutoFillProtectedForStreaking { get; set; }
        public int BotChampionId { get; set; }
        public string BotDifficulty { get; set; }
        public bool CanInviteOthers { get; set; }
        public string ExcludedPositionPreference { get; set; }
        public int Id { get; set; }
        public bool IsBot { get; set; }
        public bool IsOwner { get; set; }
        public bool IsSpectator { get; set; }
        public PositionPreferences PositionPreferences { get; set; }
        public bool ShowPositionExcluder { get; set; }
        public string SummonerInternalName { get; set; }
    }

    public class TeamOne
    {
        public bool AutoFillEligible { get; set; }
        public bool AutoFillProtectedForPromos { get; set; }
        public bool AutoFillProtectedForSoloing { get; set; }
        public bool AutoFillProtectedForStreaking { get; set; }
        public int BotChampionId { get; set; }
        public string BotDifficulty { get; set; }
        public bool CanInviteOthers { get; set; }
        public string ExcludedPositionPreference { get; set; }
        public int Id { get; set; }
        public bool IsBot { get; set; }
        public bool IsOwner { get; set; }
        public bool IsSpectator { get; set; }
        public PositionPreferences PositionPreferences { get; set; }
        public bool ShowPositionExcluder { get; set; }
        public string SummonerInternalName { get; set; }
    }

    public class TeamTwo
    {
        public bool AutoFillEligible { get; set; }
        public bool AutoFillProtectedForPromos { get; set; }
        public bool AutoFillProtectedForSoloing { get; set; }
        public bool AutoFillProtectedForStreaking { get; set; }
        public int BotChampionId { get; set; }
        public string BotDifficulty { get; set; }
        public bool CanInviteOthers { get; set; }
        public string ExcludedPositionPreference { get; set; }
        public int Id { get; set; }
        public bool IsBot { get; set; }
        public bool IsOwner { get; set; }
        public bool IsSpectator { get; set; }
        public PositionPreferences PositionPreferences { get; set; }
        public bool ShowPositionExcluder { get; set; }
        public string SummonerInternalName { get; set; }
    }

    public class CustomGameLobby
    {
        public Configuration Configuration { get; set; }
        public int GameId { get; set; }
        public string LobbyName { get; set; }
        public string LobbyPassword { get; set; }
        public List<string> PracticeGameRewardsDisabledReasons { get; set; }
        public List<Spectator> Spectators { get; set; }
        public List<TeamOne> TeamOne { get; set; }
        public List<TeamTwo> TeamTwo { get; set; }
    }

    public class GameCustomization
    {
    }

    public class LobbyRequest
    {
        public CustomGameLobby CustomGameLobby { get; set; }
        public GameCustomization GameCustomization { get; set; }
        public bool IsCustom { get; set; }
        public int QueueId { get; set; }
    }


}
