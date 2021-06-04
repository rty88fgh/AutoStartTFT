using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartTFT.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CustomSpectator
    {
        public bool AllowedChangeActivity { get; set; }
        public bool AllowedInviteOthers { get; set; }
        public bool AllowedKickOthers { get; set; }
        public bool AllowedStartActivity { get; set; }
        public bool AllowedToggleInvite { get; set; }
        public bool AutoFillEligible { get; set; }
        public bool AutoFillProtectedForPromos { get; set; }
        public bool AutoFillProtectedForSoloing { get; set; }
        public bool AutoFillProtectedForStreaking { get; set; }
        public int BotChampionId { get; set; }
        public string BotDifficulty { get; set; }
        public string BotId { get; set; }
        public string FirstPositionPreference { get; set; }
        public bool IsBot { get; set; }
        public bool IsLeader { get; set; }
        public bool IsSpectator { get; set; }
        public string Puuid { get; set; }
        public bool Ready { get; set; }
        public string SecondPositionPreference { get; set; }
        public bool ShowGhostedBanner { get; set; }
        public int SummonerIconId { get; set; }
        public int SummonerId { get; set; }
        public string SummonerInternalName { get; set; }
        public int SummonerLevel { get; set; }
        public string SummonerName { get; set; }
        public int TeamId { get; set; }
    }

    public class CustomTeam100
    {
        public bool AllowedChangeActivity { get; set; }
        public bool AllowedInviteOthers { get; set; }
        public bool AllowedKickOthers { get; set; }
        public bool AllowedStartActivity { get; set; }
        public bool AllowedToggleInvite { get; set; }
        public bool AutoFillEligible { get; set; }
        public bool AutoFillProtectedForPromos { get; set; }
        public bool AutoFillProtectedForSoloing { get; set; }
        public bool AutoFillProtectedForStreaking { get; set; }
        public int BotChampionId { get; set; }
        public string BotDifficulty { get; set; }
        public string BotId { get; set; }
        public string FirstPositionPreference { get; set; }
        public bool IsBot { get; set; }
        public bool IsLeader { get; set; }
        public bool IsSpectator { get; set; }
        public string Puuid { get; set; }
        public bool Ready { get; set; }
        public string SecondPositionPreference { get; set; }
        public bool ShowGhostedBanner { get; set; }
        public int SummonerIconId { get; set; }
        public int SummonerId { get; set; }
        public string SummonerInternalName { get; set; }
        public int SummonerLevel { get; set; }
        public string SummonerName { get; set; }
        public int TeamId { get; set; }
    }

    public class CustomTeam200
    {
        public bool AllowedChangeActivity { get; set; }
        public bool AllowedInviteOthers { get; set; }
        public bool AllowedKickOthers { get; set; }
        public bool AllowedStartActivity { get; set; }
        public bool AllowedToggleInvite { get; set; }
        public bool AutoFillEligible { get; set; }
        public bool AutoFillProtectedForPromos { get; set; }
        public bool AutoFillProtectedForSoloing { get; set; }
        public bool AutoFillProtectedForStreaking { get; set; }
        public int BotChampionId { get; set; }
        public string BotDifficulty { get; set; }
        public string BotId { get; set; }
        public string FirstPositionPreference { get; set; }
        public bool IsBot { get; set; }
        public bool IsLeader { get; set; }
        public bool IsSpectator { get; set; }
        public string Puuid { get; set; }
        public bool Ready { get; set; }
        public string SecondPositionPreference { get; set; }
        public bool ShowGhostedBanner { get; set; }
        public int SummonerIconId { get; set; }
        public int SummonerId { get; set; }
        public string SummonerInternalName { get; set; }
        public int SummonerLevel { get; set; }
        public string SummonerName { get; set; }
        public int TeamId { get; set; }
    }

    public class GameConfig
    {
        public List<int> AllowablePremadeSizes { get; set; }
        public string CustomLobbyName { get; set; }
        public string CustomMutatorName { get; set; }
        public List<string> CustomRewardsDisabledReasons { get; set; }
        public string CustomSpectatorPolicy { get; set; }
        public List<CustomSpectator> CustomSpectators { get; set; }
        public List<CustomTeam100> CustomTeam100 { get; set; }
        public List<CustomTeam200> CustomTeam200 { get; set; }
        public string GameMode { get; set; }
        public bool IsCustom { get; set; }
        public bool IsLobbyFull { get; set; }
        public bool IsTeamBuilderManaged { get; set; }
        public int MapId { get; set; }
        public int MaxHumanPlayers { get; set; }
        public int MaxLobbySize { get; set; }
        public int MaxTeamSize { get; set; }
        public string PickType { get; set; }
        public bool PremadeSizeAllowed { get; set; }
        public int QueueId { get; set; }
        public bool ShowPositionSelector { get; set; }
    }

    public class Invitation
    {
        public string InvitationId { get; set; }
        public string InvitationType { get; set; }
        public string State { get; set; }
        public string Timestamp { get; set; }
        public int ToSummonerId { get; set; }
        public string ToSummonerName { get; set; }
    }

    public class LocalMember
    {
        public bool AllowedChangeActivity { get; set; }
        public bool AllowedInviteOthers { get; set; }
        public bool AllowedKickOthers { get; set; }
        public bool AllowedStartActivity { get; set; }
        public bool AllowedToggleInvite { get; set; }
        public bool AutoFillEligible { get; set; }
        public bool AutoFillProtectedForPromos { get; set; }
        public bool AutoFillProtectedForSoloing { get; set; }
        public bool AutoFillProtectedForStreaking { get; set; }
        public int BotChampionId { get; set; }
        public string BotDifficulty { get; set; }
        public string BotId { get; set; }
        public string FirstPositionPreference { get; set; }
        public bool IsBot { get; set; }
        public bool IsLeader { get; set; }
        public bool IsSpectator { get; set; }
        public string Puuid { get; set; }
        public bool Ready { get; set; }
        public string SecondPositionPreference { get; set; }
        public bool ShowGhostedBanner { get; set; }
        public int SummonerIconId { get; set; }
        public int SummonerId { get; set; }
        public string SummonerInternalName { get; set; }
        public int SummonerLevel { get; set; }
        public string SummonerName { get; set; }
        public int TeamId { get; set; }
    }

    public class Member
    {
        public bool AllowedChangeActivity { get; set; }
        public bool AllowedInviteOthers { get; set; }
        public bool AllowedKickOthers { get; set; }
        public bool AllowedStartActivity { get; set; }
        public bool AllowedToggleInvite { get; set; }
        public bool AutoFillEligible { get; set; }
        public bool AutoFillProtectedForPromos { get; set; }
        public bool AutoFillProtectedForSoloing { get; set; }
        public bool AutoFillProtectedForStreaking { get; set; }
        public int BotChampionId { get; set; }
        public string BotDifficulty { get; set; }
        public string BotId { get; set; }
        public string FirstPositionPreference { get; set; }
        public bool IsBot { get; set; }
        public bool IsLeader { get; set; }
        public bool IsSpectator { get; set; }
        public string Puuid { get; set; }
        public bool Ready { get; set; }
        public string SecondPositionPreference { get; set; }
        public bool ShowGhostedBanner { get; set; }
        public int SummonerIconId { get; set; }
        public int SummonerId { get; set; }
        public string SummonerInternalName { get; set; }
        public int SummonerLevel { get; set; }
        public string SummonerName { get; set; }
        public int TeamId { get; set; }
    }

    public class RestrictionArgs
    {
    }

    public class Restriction
    {
        public int ExpiredTimestamp { get; set; }
        public RestrictionArgs RestrictionArgs { get; set; }
        public string RestrictionCode { get; set; }
        public List<int> SummonerIds { get; set; }
        public string SummonerIdsString { get; set; }
    }

    public class Warning
    {
        public int ExpiredTimestamp { get; set; }
        public RestrictionArgs RestrictionArgs { get; set; }
        public string RestrictionCode { get; set; }
        public List<int> SummonerIds { get; set; }
        public string SummonerIdsString { get; set; }
    }

    public class LobbyResponse
    {
        public bool CanStartActivity { get; set; }
        public string ChatRoomId { get; set; }
        public string ChatRoomKey { get; set; }
        public GameConfig GameConfig { get; set; }
        public List<Invitation> Invitations { get; set; }
        public LocalMember LocalMember { get; set; }
        public List<Member> Members { get; set; }
        public string PartyId { get; set; }
        public string PartyType { get; set; }
        public List<Restriction> Restrictions { get; set; }
        public List<Warning> Warnings { get; set; }
    }


}
