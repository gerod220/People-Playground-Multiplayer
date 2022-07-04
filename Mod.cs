using System;
using UnityEngine;
using Multiplayer.GUI;
using Multiplayer.Client;
using Multiplayer.Game;
using UnityEngine.TextCore;
using System.Collections.Generic;
using TMPro;

internal class Mod
{
    public GameObject MainGameObject;

    public static string MPVer => "0.4.0a";

    public static Mod Instance;
    public static string Title = "Multiplayer Mod";

    public static bool isFromSteamWorkshop = false;

    public static Sprite MainMenuIcon;
    public static Sprite CategoryPlayersIcon;
    public static Sprite PlayerIcon;
    public static Sprite CursorIcon;
    public static AudioClip NotifySound;

    public Mod()
    {
        Instance = this;
        MainGameObject = new GameObject("MP:ModManager");
        MainGameObject.AddComponent<ModManager>();
    }

    public static void LoadResourse()
    {
        MainMenuIcon = ModAPI.LoadSprite("mainMenuIcon.png");
        CategoryPlayersIcon = ModAPI.LoadSprite("playerList.png");
        PlayerIcon = ModAPI.LoadSprite("player.png");
        CursorIcon = ModAPI.LoadSprite("cursor.png");
        NotifySound = ModAPI.LoadSound("notifySound.mp3");
    }

    public static void FromSteamWorkshop()
    {
        isFromSteamWorkshop = true;
    }

    // Map Loaded;
    public static void Main()
    {
        if (ClientManager.isMPLoading)
        {
            MapManager.CreateCategory("Players", "Player list", CategoryPlayersIcon);
            Instance.MainGameObject.GetComponent<ClientManager>().OnMapLoaded();
        }
        else
        {
            try { MapManager.DeleteCategory(MapManager.PlayersCategory); } catch { }
        }
    }
}

internal class ModManager : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(base.gameObject);

        Notify.Show($"[{Mod.Title}] {GUI.SetColor("Client.dll", ConsoleColor.Yellow)} {GUI.SetColor("loaded.", ConsoleColor.Green)}", 4);

        GUIInit();
        NetworkInit();
        Multiplayer.API.APIController.Init();
        Multiplayer.Game.ModManager.InvokeToAllMods("MPLoaded");
    }

    private void GUIInit()
    {
        MainMenu.Init();
        SettingsMenu.Init();
    }

    private static bool playersUnVisibles = false;
    private static bool changeCategory = true;
    private async void Update()
    {
        if (Application.loadedLevelName == "Menu" && ClientManager.CurrentLobby.Id.IsValid)
        {
            ClientManager.LeaveLobby();

            playersUnVisibles = false;
            changeCategory = true;

            CatalogBehaviour.Main.SetCategory(CatalogBehaviour.Main.Catalog.Categories[1]);
        }

        if (Application.loadedLevelName == "Menu" && ClientManager.CurrentMap != null)
        {
            ClientManager.CurrentMap = null;
        }

        if (Application.loadedLevelName == "Main" && ClientManager.CurrentMap == null)
        {
            ClientManager.CurrentMap = MapLoaderBehaviour.CurrentMap;
        }

        if(Application.loadedLevelName == "Main" && CatalogBehaviour.Main.SelectedCategory == MapManager.PlayersCategory && changeCategory)
        {
            try { CatalogBehaviour.Main.SetCategory(CatalogBehaviour.Main.Catalog.Categories[1]); } catch { }
            changeCategory = false;
        }

        if (CatalogBehaviour.Main?.SelectedItem != null)
        {
            if (CatalogBehaviour.Main?.SelectedCategory == MapManager.PlayersCategory && !playersUnVisibles)
            {
                int curAsset = 1;
                SpawnableAsset asset = null;

                foreach (var member in ClientManager.CurrentLobby.Members)
                {
                    asset = CatalogBehaviour.Main.GetSpawnable("PlayerPrefabAsset" + curAsset);
                    asset.name = member.Name;
                    asset.VisibleInCatalog = true;
                    asset.ViewSprite = await GUI.GetSpriteByAvatarAsync(member.Id);
                    if (member.Id == ClientManager.CurrentLobby.Owner.Id) asset.Description = "Host";

                    curAsset++;
                }

                asset = CatalogBehaviour.Main.GetSpawnable("PlayerPrefabAsset" + curAsset);
                if(asset!=null)
                {
                    while(CatalogBehaviour.Main.GetSpawnable("PlayerPrefabAsset" + curAsset) != null)
                    {
                        CatalogBehaviour.Main.GetSpawnable("PlayerPrefabAsset" + curAsset).VisibleInCatalog = false;
                        curAsset++;
                    }
                }

                playersUnVisibles = true;
                try { CatalogBehaviour.Main.CreateItemButtons(); } catch { }
            }

            if (CatalogBehaviour.Main?.SelectedItem.Category == MapManager.PlayersCategory)
            {
                PlayerMenu.Open(CatalogBehaviour.Main?.SelectedItem.name);
                CatalogBehaviour.Main.SetItem(MapManager.PrewCurSelectItem);
            }
            else
            {
                MapManager.PrewCurSelectItem = CatalogBehaviour.Main?.SelectedItem;
            }
        }
    }

    private void NetworkInit()
    {
        Mod.Instance.MainGameObject.AddComponent<Multiplayer.Host.LobbyManager>();
        Mod.Instance.MainGameObject.AddComponent<ClientManager>();
    }
}