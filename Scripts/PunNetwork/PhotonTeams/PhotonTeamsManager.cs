// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonTeamsManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
// Implements teams in a room/game with help of player properties.
// </summary>
// <remarks>
// Teams are defined by name and code. Change this to get more / different teams.
// There are no rules when / if you can join a team. You could add this in JoinTeam or something.
// </remarks>                                                                                           
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using PunNetwork.Services.CustomProperties;
using UnityEngine;
using Utils.Extensions;
using Zenject;
using static Utils.Enumerators;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace PunNetwork.PhotonTeams
{
    public interface IPhotonTeamsManager
    {
        Dictionary<byte, PhotonTeam> GetAllTeams();
        PhotonTeam GetAvailableTeam();
        bool TryGetTeamMatesOfPlayer(Player player, out Player[] teamMates);
        bool TryGetTeamByName(string teamName, out PhotonTeam team);
        bool TryGetTeamByCode(byte teamCode, out PhotonTeam team);
        event Action<Player, PhotonTeam> PlayerJoinedTeam;
        event Action<Player, PhotonTeam> PlayerLeftTeam;
        event Action TeamDeadEvent;
        void PlayerDeadHandler(Player player, bool state);
    }

    [Serializable]
    public class PhotonTeam
    {
        public string Name;
        public byte Code;
        public bool IsAlive;

        public override string ToString()
        {
            return string.Format("{0} [{1}], IsAlive {2}", this.Name, this.Code, this.IsAlive);
        }
    }

    /// <summary>
    /// Implements teams in a room/game with help of player properties. Access them by Player.GetTeam extension.
    /// </summary>
    /// <remarks>
    /// Teams are defined by enum Team. Change this to get more / different teams.
    /// There are no rules when / if you can join a team. You could add this in JoinTeam or something.
    /// </remarks>
    [DisallowMultipleComponent]
    public class PhotonTeamsManager : MonoBehaviour, IMatchmakingCallbacks, IInRoomCallbacks, IPhotonTeamsManager
    {
#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] private bool _listFoldIsOpen = true;
#pragma warning restore 0414
#endif

        [SerializeField] private List<PhotonTeam> _teamsList = new List<PhotonTeam>
        {
            new PhotonTeam { Name = "Blue", Code = 1, IsAlive = true },
            new PhotonTeam { Name = "Red", Code = 2, IsAlive = true }
        };

        private Dictionary<byte, PhotonTeam> TeamsByCode { get; set; }
        private Dictionary<string, PhotonTeam> TeamsByName { get; set; }

        /// <summary>The main list of teams with their player-lists. Automatically kept up to date.</summary>
        private Dictionary<byte, HashSet<Player>> _playersPerTeam;

        private ICustomPropertiesService _customPropertiesService;
        private static DiContainer _container;

        /// <summary>Defines the player custom property name to use for team affinity of "this" player.</summary>
        public const string TeamPlayerProp = "_pt";

        public event Action<Player, PhotonTeam> PlayerJoinedTeam;
        public event Action<Player, PhotonTeam> PlayerLeftTeam;
        public event Action TeamDeadEvent;

        public static IPhotonTeamsManager Instance
        {
            get
            {
                if (_container != null)
                    return _container.Resolve<IPhotonTeamsManager>();

                throw new Exception($"[{nameof(PhotonTeamsManager)} Di container is null yet.");
            }
        }

        [Inject]
        private void Construct
        (
            DiContainer container,
            ICustomPropertiesService customPropertiesService
        )
        {
            _container = container;
            _customPropertiesService = customPropertiesService;
        }

        #region MonoBehaviour

        public void PlayerDeadHandler(Player player, bool state)
        {
            var photonTeam = TeamsByCode[player.GetPhotonTeam().Code];
            var teamPlayers = _playersPerTeam[photonTeam.Code];

            var isAllTeamPlayersDead = teamPlayers.All(p =>
                p.TryGetCustomProperty(PlayerProperty.IsDead, out bool isDead) && isDead);

            photonTeam.IsAlive = !isAllTeamPlayersDead;
            if (!photonTeam.IsAlive) 
                TeamDeadEvent?.Invoke();
        }

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            this.ClearTeams();
        }

        private void Init()
        {
            TeamsByCode = new Dictionary<byte, PhotonTeam>(_teamsList.Count);
            TeamsByName = new Dictionary<string, PhotonTeam>(_teamsList.Count);
            _playersPerTeam = new Dictionary<byte, HashSet<Player>>(_teamsList.Count);
            for (int i = 0; i < _teamsList.Count; i++)
            {
                TeamsByCode[_teamsList[i].Code] = _teamsList[i];
                TeamsByName[_teamsList[i].Name] = _teamsList[i];
                _playersPerTeam[_teamsList[i].Code] = new HashSet<Player>();
            }
        }

        #endregion

        #region IMatchmakingCallbacks

        void IMatchmakingCallbacks.OnJoinedRoom()
        {
            Init();
            this.UpdateTeams();
        }

        void IMatchmakingCallbacks.OnLeftRoom()
        {
            this.ClearTeams();
        }

        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            object temp;
            if (changedProps.TryGetValue(TeamPlayerProp, out temp))
            {
                if (temp == null)
                {
                    foreach (byte code in _playersPerTeam.Keys)
                    {
                        if (_playersPerTeam[code].Remove(targetPlayer))
                        {
                            if (PlayerLeftTeam != null)
                            {
                                PlayerLeftTeam(targetPlayer, TeamsByCode[code]);
                            }

                            break;
                        }
                    }
                }
                else if (temp is byte)
                {
                    byte teamCode = (byte)temp;
                    // check if player switched teams, remove from previous team 
                    foreach (byte code in _playersPerTeam.Keys)
                    {
                        if (code == teamCode)
                        {
                            continue;
                        }

                        if (_playersPerTeam[code].Remove(targetPlayer))
                        {
                            if (PlayerLeftTeam != null)
                            {
                                PlayerLeftTeam(targetPlayer, TeamsByCode[code]);
                            }

                            break;
                        }
                    }

                    PhotonTeam team = TeamsByCode[teamCode];
                    if (!_playersPerTeam[teamCode].Add(targetPlayer))
                    {
                        Debug.LogWarningFormat(
                            "Unexpected situation while setting team {0} for player {1}, updating teams for all", team,
                            targetPlayer);
                        this.UpdateTeams();
                    }

                    if (PlayerJoinedTeam != null)
                    {
                        PlayerJoinedTeam(targetPlayer, team);
                    }
                }
                else
                {
                    Debug.LogErrorFormat(
                        "Unexpected: custom property key {0} should have of type byte, instead we got {1} of type {2}. Player: {3}",
                        TeamPlayerProp, temp, temp.GetType(), targetPlayer);
                }
            }
        }

        void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer.IsInactive)
            {
                return;
            }

            PhotonTeam team = otherPlayer.GetPhotonTeam();
            if (team != null && !_playersPerTeam[team.Code].Remove(otherPlayer))
            {
                Debug.LogWarningFormat(
                    "Unexpected situation while removing player {0} who left from team {1}, updating teams for all",
                    otherPlayer, team);
                // revert to 'brute force' in case of unexpected situation
                this.UpdateTeams();
            }
        }

        void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
        {
            PhotonTeam team = newPlayer.GetPhotonTeam();
            if (team == null)
            {
                return;
            }

            if (_playersPerTeam[team.Code].Contains(newPlayer))
            {
                // player rejoined w/ same team
                return;
            }

            // check if player rejoined w/ different team, remove from previous team 
            foreach (var key in TeamsByCode.Keys)
            {
                if (_playersPerTeam[key].Remove(newPlayer))
                {
                    break;
                }
            }

            if (!_playersPerTeam[team.Code].Add(newPlayer))
            {
                Debug.LogWarningFormat(
                    "Unexpected situation while adding player {0} who joined to team {1}, updating teams for all",
                    newPlayer, team);
                // revert to 'brute force' in case of unexpected situation
                this.UpdateTeams();
            }
        }

        #endregion

        #region Private methods

        private void UpdateTeams()
        {
            this.ClearTeams();
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];
                PhotonTeam playerTeam = player.GetPhotonTeam();
                if (playerTeam != null)
                {
                    _playersPerTeam[playerTeam.Code].Add(player);
                }
            }
        }

        private void ClearTeams()
        {
            foreach (var key in _playersPerTeam.Keys)
            {
                _playersPerTeam[key].Clear();
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Find a PhotonTeam using a team code.
        /// </summary>
        /// <param name="code">The team code.</param>
        /// <param name="team">The team to be assigned if found.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamByCode(byte code, out PhotonTeam team)
        {
            return TeamsByCode.TryGetValue(code, out team);
        }

        /// <summary>
        /// Find a PhotonTeam using a team name.
        /// </summary>
        /// <param name="teamName">The team name.</param>
        /// <param name="team">The team to be assigned if found.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamByName(string teamName, out PhotonTeam team)
        {
            return TeamsByName.TryGetValue(teamName, out team);
        }

        /// <summary>
        /// Gets all teams available.
        /// </summary>
        /// <returns>Returns all teams available.</returns>
        public PhotonTeam[] GetAvailableTeams()
        {
            if (_teamsList != null)
            {
                return _teamsList.ToArray();
            }

            return null;
        }

        public Dictionary<byte, PhotonTeam> GetAllTeams() => TeamsByCode;

        public PhotonTeam GetAvailableTeam()
        {
            if (_teamsList == null || _teamsList.Count == 0)
            {
                return null;
            }

            PhotonTeam leastPopulatedTeam = null;
            int minPlayerCount = int.MaxValue;

            foreach (PhotonTeam team in _teamsList)
            {
                int teamMemberCount = GetTeamMembersCount(team);
                if (teamMemberCount < minPlayerCount)
                {
                    leastPopulatedTeam = team;
                    minPlayerCount = teamMemberCount;
                }
            }

            return leastPopulatedTeam;
        }

        /// <summary>
        /// Gets all players joined to a team using a team code.
        /// </summary>
        /// <param name="code">The code of the team.</param>
        /// <param name="members">The array of players to be filled.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamMembers(byte code, out Player[] members)
        {
            members = null;
            HashSet<Player> players;
            if (this._playersPerTeam.TryGetValue(code, out players))
            {
                members = new Player[players.Count];
                int i = 0;
                foreach (var player in players)
                {
                    members[i] = player;
                    i++;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets all players joined to a team using a team name.
        /// </summary>
        /// <param name="teamName">The name of the team.</param>
        /// <param name="members">The array of players to be filled.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamMembers(string teamName, out Player[] members)
        {
            members = null;
            PhotonTeam team;
            if (this.TryGetTeamByName(teamName, out team))
            {
                return this.TryGetTeamMembers(team.Code, out members);
            }

            return false;
        }

        /// <summary>
        /// Gets all players joined to a team.
        /// </summary>
        /// <param name="team">The team which will be used to find players.</param>
        /// <param name="members">The array of players to be filled.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamMembers(PhotonTeam team, out Player[] members)
        {
            members = null;
            if (team != null)
            {
                return this.TryGetTeamMembers(team.Code, out members);
            }

            return false;
        }

        /// <summary>
        /// Gets all team mates of a player.
        /// </summary>
        /// <param name="player">The player whose team mates will be searched.</param>
        /// <param name="teamMates">The array of players to be filled.</param>
        /// <returns>If successful or not.</returns>
        public bool TryGetTeamMatesOfPlayer(Player player, out Player[] teamMates)
        {
            teamMates = null;
            if (player == null)
            {
                return false;
            }

            PhotonTeam team = player.GetPhotonTeam();
            if (team == null)
            {
                return false;
            }

            HashSet<Player> players;
            if (this._playersPerTeam.TryGetValue(team.Code, out players))
            {
                if (!players.Contains(player))
                {
                    Debug.LogWarningFormat(
                        "Unexpected situation while getting team mates of player {0} who is joined to team {1}, updating teams for all",
                        player, team);
                    // revert to 'brute force' in case of unexpected situation
                    this.UpdateTeams();
                }

                teamMates = new Player[players.Count - 1];
                int i = 0;
                foreach (var p in players)
                {
                    if (p.Equals(player))
                    {
                        continue;
                    }

                    teamMates[i] = p;
                    i++;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the number of players in a team by team code.
        /// </summary>
        /// <param name="code">Unique code of the team</param>
        /// <returns>Number of players joined to the team.</returns>
        public int GetTeamMembersCount(byte code)
        {
            PhotonTeam team;
            if (this.TryGetTeamByCode(code, out team))
            {
                return this.GetTeamMembersCount(team);
            }

            return 0;
        }

        /// <summary>
        /// Gets the number of players in a team by team name.
        /// </summary>
        /// <param name="name">Unique name of the team</param>
        /// <returns>Number of players joined to the team.</returns>
        public int GetTeamMembersCount(string name)
        {
            PhotonTeam team;
            if (this.TryGetTeamByName(name, out team))
            {
                return this.GetTeamMembersCount(team);
            }

            return 0;
        }

        /// <summary>
        /// Gets the number of players in a team.
        /// </summary>
        /// <param name="team">The team you want to know the size of</param>
        /// <returns>Number of players joined to the team.</returns>
        public int GetTeamMembersCount(PhotonTeam team)
        {
            HashSet<Player> players;
            if (team != null && this._playersPerTeam.TryGetValue(team.Code, out players) && players != null)
            {
                return players.Count;
            }

            return 0;
        }

        #endregion

        #region Unused methods

        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        void IMatchmakingCallbacks.OnCreatedRoom()
        {
        }

        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
        {
        }

        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
        {
        }

        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
        {
        }

        void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }

        void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
        {
        }

        #endregion
    }

    /// <summary>Extension methods for the Player class that make use of PhotonTeamsManager.</summary>
    public static class PhotonTeamExtensions
    {
        /// <summary>Gets the team the player is currently joined to. Null if none.</summary>
        /// <returns>The team the player is currently joined to. Null if none.</returns>
        public static PhotonTeam GetPhotonTeam(this Player player)
        {
            object teamId;
            PhotonTeam team;
            if (player.CustomProperties.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamId) &&
                PhotonTeamsManager.Instance.TryGetTeamByCode((byte)teamId, out team))
            {
                return team;
            }

            return null;
        }

        /// <summary>
        /// Join a team.
        /// </summary>
        /// <param name="player">The player who will join a team.</param>
        /// <param name="team">The team to be joined.</param>
        /// <returns></returns>
        public static bool JoinTeam(this Player player, PhotonTeam team)
        {
            if (team == null)
            {
                Debug.LogWarning("JoinTeam failed: PhotonTeam provided is null");
                return false;
            }

            PhotonTeam currentTeam = player.GetPhotonTeam();
            if (currentTeam != null)
            {
                Debug.LogWarningFormat(
                    "JoinTeam failed: player ({0}) is already joined to a team ({1}), call SwitchTeam instead", player,
                    team);
                return false;
            }

            return player.SetCustomProperties(new Hashtable { { PhotonTeamsManager.TeamPlayerProp, team.Code } });
        }

        /// <summary>
        /// Join a team using team code.
        /// </summary>
        /// <param name="player">The player who will join the team.</param>
        /// <param name="teamCode">The code fo the team to be joined.</param>
        /// <returns></returns>
        public static bool JoinTeam(this Player player, byte teamCode)
        {
            PhotonTeam team;
            return PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out team) && player.JoinTeam(team);
        }

        /// <summary>
        /// Join a team using team name.
        /// </summary>
        /// <param name="player">The player who will join the team.</param>
        /// <param name="teamName">The name of the team to be joined.</param>
        /// <returns></returns>
        public static bool JoinTeam(this Player player, string teamName)
        {
            PhotonTeam team;
            return PhotonTeamsManager.Instance.TryGetTeamByName(teamName, out team) && player.JoinTeam(team);
        }

        /// <summary>Switch that player's team to the one you assign.</summary>
        /// <remarks>Internally checks if this player is in that team already or not. Only team switches are actually sent.</remarks>
        /// <param name="player"></param>
        /// <param name="team"></param>
        public static bool SwitchTeam(this Player player, PhotonTeam team)
        {
            if (team == null)
            {
                Debug.LogWarning("SwitchTeam failed: PhotonTeam provided is null");
                return false;
            }

            PhotonTeam currentTeam = player.GetPhotonTeam();
            if (currentTeam == null)
            {
                Debug.LogWarningFormat(
                    "SwitchTeam failed: player ({0}) was not joined to any team, call JoinTeam instead", player);
                return false;
            }

            if (currentTeam.Code == team.Code)
            {
                Debug.LogWarningFormat("SwitchTeam failed: player ({0}) is already joined to the same team {1}", player,
                    team);
                return false;
            }

            return player.SetCustomProperties(new Hashtable { { PhotonTeamsManager.TeamPlayerProp, team.Code } },
                new Hashtable { { PhotonTeamsManager.TeamPlayerProp, currentTeam.Code } });
        }

        /// <summary>Switch the player's team using a team code.</summary>
        /// <remarks>Internally checks if this player is in that team already or not.</remarks>
        /// <param name="player">The player that will switch teams.</param>
        /// <param name="teamCode">The code of the team to switch to.</param>
        /// <returns>If the team switch request is queued to be sent to the server or done in case offline or not joined to a room yet.</returns>
        public static bool SwitchTeam(this Player player, byte teamCode)
        {
            PhotonTeam team;
            return PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out team) && player.SwitchTeam(team);
        }

        /// <summary>Switch the player's team using a team name.</summary>
        /// <remarks>Internally checks if this player is in that team already or not.</remarks>
        /// <param name="player">The player that will switch teams.</param>
        /// <param name="teamName">The name of the team to switch to.</param>
        /// <returns>If the team switch request is queued to be sent to the server or done in case offline or not joined to a room yet.</returns>
        public static bool SwitchTeam(this Player player, string teamName)
        {
            PhotonTeam team;
            return PhotonTeamsManager.Instance.TryGetTeamByName(teamName, out team) && player.SwitchTeam(team);
        }

        /// <summary>
        /// Leave the current team if any.
        /// </summary>
        /// <param name="player"></param>
        /// <returns>If the leaving team request is queued to be sent to the server or done in case offline or not joined to a room yet.</returns>
        public static bool LeaveCurrentTeam(this Player player)
        {
            PhotonTeam currentTeam = player.GetPhotonTeam();
            if (currentTeam == null)
            {
                Debug.LogWarningFormat("LeaveCurrentTeam failed: player ({0}) was not joined to any team", player);
                return false;
            }

            return player.SetCustomProperties(new Hashtable { { PhotonTeamsManager.TeamPlayerProp, null } },
                new Hashtable { { PhotonTeamsManager.TeamPlayerProp, currentTeam.Code } });
        }

        /// <summary>
        /// Try to get the team mates.
        /// </summary>
        /// <param name="player">The player to get the team mates of.</param>
        /// <param name="teamMates">The team mates array to fill.</param>
        /// <returns>If successful or not.</returns>
        public static bool TryGetTeamMates(this Player player, out Player[] teamMates)
        {
            return PhotonTeamsManager.Instance.TryGetTeamMatesOfPlayer(player, out teamMates);
        }
    }
}