using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Steamworks;
using Steamworks.Data;

using Multiplayer.GUI;
using Multiplayer.Host;
using Multiplayer.Game;
using Multiplayer.Network;

namespace Multiplayer.Client
{
    internal class ClientManager : MonoBehaviour
    {
        private static bool EventInited = false;

        public static event Action<Friend> OnClientConnected;
        public static event Action<Friend> OnClientDisconnected;

        public static Lobby CurrentLobby;
        public static Map CurrentMap = null;
        public bool isHost { get => CurrentLobby.Owner.Id == SteamClient.SteamId; }
        public static bool isMPLoading = false;

        public static List<GUI.Cursor> Cursors = new List<GUI.Cursor>();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (EventInited) return;
            else EventInited = true;

            SteamFriends.OnGameLobbyJoinRequested += OnJoinToFrined;
            SteamMatchmaking.OnLobbyEntered += OnJoinedToLobby;
            SteamMatchmaking.OnLobbyInvite += OnInviteToLobby;
            SteamMatchmaking.OnChatMessage += NewPacket;

            SteamMatchmaking.OnLobbyCreated += API.NetEventsController.LobbyCreated;

            OnClientConnected += ClientConnected;
            OnClientDisconnected += ClientDisconnected;
        }

        private void ClientConnected(Friend client)
        {
            Notify.Show(GUI.GUI.SetColor($"Client \"{client.Name}\" joined.", ConsoleColor.Green), 3);
            MapManager.AddToMap(client);
        }

        private void ClientDisconnected(Friend client)
        {
            Notify.Show(GUI.GUI.SetColor($"Client \"{client.Name}\" leaved.", ConsoleColor.Red), 3);
            MapManager.DeleteFromMap(client);
        }

        private void NewPacket(Lobby lobby, Friend client, string data)
        {
            if (client.Id == SteamClient.SteamId) return;

            Packet packet = Packet.ConvertFrom(data);

            if (packet.ToClient)
            {
                if (!(packet.Client.Id == SteamClient.SteamId)) return;

                if (packet.Event == Packet.EventId.KickSelf) LeaveLobby();

                return;
            }

            if (packet.Event == Packet.EventId.ClientJoined) OnClientConnected?.Invoke(client);

            else if (packet.Event == Packet.EventId.ClientLeaved) OnClientDisconnected?.Invoke(client);

            else if (packet.Event == Packet.EventId.SpawnObject)
            {

            }

            else if (packet.Event == Packet.EventId.NewObjectInfo)
            {
                if(packet.Object.Name == "Cursor")
                {
                    foreach (var cursor in Cursors)
                    {
                        if (cursor.Owner.Id == new Friend(SteamClient.SteamId).Id)
                        {
                            cursor.SetPos(packet.Object.Position);
                        }
                        break;
                    }
                }
            }

            else if (packet.Event == Packet.EventId.DeleteObject)
            {

            }

            else if (packet.Event == Packet.EventId.NetVarRegister)
            {

            }
        }

        private void JoinToLobby(Lobby lobby)
        {
            if (CurrentLobby.Id.IsValid)
            {
                Notify.Show(GUI.GUI.SetColor("Leave from current lobby!", ConsoleColor.Red), 3);
                return;
            }

            else if (CurrentMap != null)
            {
                Notify.Show(GUI.GUI.SetColor("Leave from current map!", ConsoleColor.Red), 3);
                return;
            }

            else
            {
                lobby.Join();
            }
        }

        public bool LoadLobby(Lobby lobby)
        {
            LobbySettings lobbyInfo = new LobbySettings().Load(lobby);

            Debug.Log($"lobbyVer '{lobbyInfo.GameVersion}' = gameVer '{GameVersion.Version}'");

            if (!lobbyInfo.GameVersion.Contains(GameVersion.Version))
            {
                Notify.Show(GUI.GUI.SetColor("Your version of the game will not match the version of the lobby creator.", ConsoleColor.Red), 5);
                return false;
            }

            else if (!lobbyInfo.MPVersion.Contains(Mod.MPVer))
            {
                Notify.Show(GUI.GUI.SetColor("Your multiplayer version will not match the version of the lobby creator.", ConsoleColor.Red), 5); ;
                return false;
            }

            else if (lobbyInfo.Lock)
            {
                Notify.Show(GUI.GUI.SetColor("No Access", ConsoleColor.Red), 3);
                return false;
            }

            else
            {
                return LoadMap(lobbyInfo.Map, lobby);
            }
        }

        public static void LeaveLobby()
        {
            Events.SendLeaveToAll(CurrentLobby);

            CurrentLobby.Leave();
            CurrentLobby = new Lobby();
            Notify.Show(GUI.GUI.SetColor("You leaved current lobby.", ConsoleColor.Red), 3);
        }

        public bool LoadMap(Map map, Lobby lobby)
        {
            ClientManager.isMPLoading = true;
            if (!MapManager.Load(map))
            {
                Notify.Show($"Map: {GUI.GUI.SetColor($"{map.name}", ConsoleColor.Yellow)} {GUI.GUI.SetColor("not found", ConsoleColor.Red)}!", 3);
                ClientManager.isMPLoading = false;
                return false;
            }
            else
            {
                lobbyEntered = lobby;
                return true;
            }
        }

        private void OnInviteToLobby(Friend friend, Lobby lobby)
        {
            DialogBoxManager.Dialog($"Invitation to lobby of \"{friend.Name}\"", new DialogButton[2]
            {
                new DialogButton("Join", true, new UnityEngine.Events.UnityAction(()=>
                {
                    JoinToLobby(lobby);
                })),

                new DialogButton("Cancel", true, new UnityEngine.Events.UnityAction(()=>{ }))
            });
        }

        private void OnJoinToFrined(Lobby lobby, SteamId friend)
        {
            JoinToLobby(lobby);
        }

        private static Lobby lobbyEntered;
        private void OnJoinedToLobby(Lobby lobby)
        {
            if (lobby.Owner.Id == SteamClient.SteamId)
            {
                Notify.Show($"You created and entered to lobby: {lobby.Id}", 3);
            }
            else
            {
                if (!LoadLobby(lobby))
                {
                    lobby.Leave();
                }
            }
        }

        public void OnMapLoaded()
        {
            CurrentLobby = lobbyEntered;
            CurrentMap = new LobbySettings().Load(CurrentLobby).Map;

            if (isHost)
            {
                CurrentLobby.SetPublic();
                Notify.Show(GUI.GUI.SetColor($"Lobby Ready: {CurrentLobby.Id}", ConsoleColor.Green), 3);

                string welcome = GUI.GUI.SetColor("Welcome to the first kind of multiplayer for People Playground.", ConsoleColor.Yellow);
                DialogBoxManager.Notification(welcome);
            }
            else
            {
                Notify.Show($"You joined to lobby: {CurrentLobby.Id}", 3);

                Events.SendJoinToAll(CurrentLobby);
            }
            isMPLoading = false;

            if(!MapManager.SpawnCursor(new Friend(SteamClient.SteamId), Input.mousePosition))
            {
                Notify.Show("Суки, урою", 3);
            }
            MapManager.RegisterPlayersListPrefabs();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}