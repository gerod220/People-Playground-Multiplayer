using Multiplayer.Client;
using Multiplayer.Host;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.GUI
{
    internal class MainMenu
    {
        public static MainMenu Instance = null;

        private DialogBox _dialog = null;
        private bool _open = false;
        public bool isOpen { get => _dialog == null; }

        private MainMenu()
        {
            Instance = this;
            MMButtonInit();
        }

        public static void Init()
        {
            if(Instance == null) new MainMenu();
        }

        public static void NotOpen()
        {
            Instance._open = !Instance._open;
        }

        private void MMButtonInit()
        {
            Button btn = GUI.CreateButton(Mod.MainMenuIcon, UIElements.CANVAS_TOP_CENTER-new Vector3(0,25), new Vector2(30f, 30f));
            var clickEvent = new Button.ButtonClickedEvent();
            clickEvent.AddListener(new UnityEngine.Events.UnityAction(Instance.OnClick));
            btn.onClick = clickEvent;

            new GameObject("MP:MMController").AddComponent<MMController>().btn = btn;
        }

        private void OnClick()
        {
            if (SettingsMenu.Instance.isOpen) return;

            _open = !_open;
        }

        private class MMController : MonoBehaviour
        {
            public Button btn;
            void Start()
            {
                DontDestroyOnLoad(this);
                DontDestroyOnLoad(gameObject);
            }

            void Update()
            {
                if(btn == null || btn.ToString() == "null")
                {
                    btn = GUI.CreateButton(Mod.MainMenuIcon, new Vector3(Screen.width / 2, Screen.height - 25, 0), new Vector2(30f, 30f));
                    var clickEvent = new Button.ButtonClickedEvent();
                    clickEvent.AddListener(new UnityEngine.Events.UnityAction(Instance.OnClick));
                    btn.onClick = clickEvent;
                }

                if(Instance._open && Instance._dialog == null)
                {
                    string onlineActive = SteamworksInitialiser.IsInitialised ? GUI.SetColor("On", ConsoleColor.Green) : GUI.SetColor("Off", ConsoleColor.Red);
                    Instance._dialog = DialogBoxManager.Dialog($"Author:\tGerod\nVersion:\t{Mod.MPVer}\nSteam:\t{onlineActive}", new DialogButton[2] 
                    {
                        new DialogButton("Start Host", false, ()=>
                        {
                            Mod.Instance.MainGameObject.AddComponent<LobbyManager>().CreateLobby();
                            Instance._open = !Instance._open;
                        }),
                        new DialogButton("Change Settings", false, ()=>
                        {
                            Instance._open = !Instance._open;
                            SettingsMenu.Open();
                        })
                    });

                    GUI.TextNormilizeByDialogBox(Instance._dialog);
                }

                if(!Instance._open && Instance._dialog != null)
                {
                    Instance._dialog.Close();
                    Instance._dialog = null;
                }
            }
        }
    }
}
