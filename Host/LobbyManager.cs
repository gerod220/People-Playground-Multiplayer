using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

using Steamworks;
using Steamworks.Data;

using Multiplayer.Client;
using Multiplayer.GUI;
using Multiplayer.Game;

namespace Multiplayer.Host
{
    internal struct LobbySettings
    {
        public string Title;
        public Map Map;
        public bool Lock;
        public string Password;

        public string GameVersion;
        public string MPVersion;

        public void Save(Lobby lobby)
        {
            lobby.SetData("Title", $"{Title}");
            lobby.SetData("Map", $"{Map.name}");

            string @lock = Lock ? "Yes" : "No";
            lobby.SetData("Lock", $"{@lock}");
            lobby.SetData("Password", $"{Password}");

            lobby.SetData("GameVersion", $"{GameVersion}");
            lobby.SetData("MPVersion", $"{MPVersion}");

            Debug.Log($"\n[MP] Lobby Save:\nTitle:\t{Title}\nMap:\t{Map?.name}\nLock:\t{@lock}\nGVer:\t{GameVersion}\nMPVer:\t{MPVersion}");
        }

        public LobbySettings Load(Lobby lobby)
        {
            Title = lobby.GetData("Title");

            foreach (var map in MapRegistry.GetAllMaps())
            {
                if(map.name == lobby.GetData("Map"))
                {
                    Map = map;
                    break;
                }
            }

            string @lock = lobby.GetData("Lock");
            Lock = @lock == "Yes" ? true : false;

            Password = lobby.GetData("Password");

            GameVersion = lobby.GetData("GameVersion");
            MPVersion = lobby.GetData("MPVersion");

            Debug.Log($"\n[MP] Lobby Load:\nTitle:\t{Title}\nMap:\t{Map?.name}\nLock:\t{@lock}\nGVer:\t{GameVersion}\nMPVer:\t{MPVersion}");
            return this;
        }
    }

    internal class LobbyManager : MonoBehaviour
    {
        private static bool EventInited = false;

        private ClientManager Client { get => Mod.Instance.MainGameObject.GetComponent<ClientManager>(); }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (EventInited) return;
            else EventInited = true;

            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        }

        public bool CreateLobby()
        {
            if(ClientManager.CurrentLobby.Id.IsValid)
            {
                Notify.Show($"{GUI.GUI.SetColor("Leave from current lobby!", ConsoleColor.Red)}", 3);
                return false;
            }

            else if(ClientManager.CurrentMap != null)
            {
                Notify.Show($"{GUI.GUI.SetColor("Leave from current map!", ConsoleColor.Red)}", 3);
                return false;
            }

            else
            {
                SteamMatchmaking.CreateLobbyAsync(2);
                return true;
            }
        }

        private void OnLobbyCreated(Result result, Lobby lobby)
        {
            LobbySettings lobbySettings = new LobbySettings
            {
                Title = SteamClient.Name,
                Map = SettingsMenu.Instance.LobbyMap,
                Lock = SettingsMenu.Instance.Lock,
                GameVersion = GameVersion.Version,
                MPVersion = Mod.MPVer,
            };

            lobby.MaxMembers = SettingsMenu.Instance.MaxMembers;
            lobbySettings.Save(lobby);

            Map map = lobbySettings.Map;

            //ClientManager.isMPLoading = true;
            Client.LoadMap(map, lobby);
        }
    }
}
