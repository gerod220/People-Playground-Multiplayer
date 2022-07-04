using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.GUI
{
    internal class SettingsMenu
    {
        public static SettingsMenu Instance = null;
        private List<DialogButton> _dialogButtons = new List<DialogButton>();
        private DialogBox _dialog = null;
        public bool isOpen => _dialog != null;

        public Map LobbyMap = MapRegistry.GetAllMaps().First();
        private int _curMapInt = 1;
        private int _mapsCount = MapRegistry.GetAllMaps().Count();
        private List<Map> _maps = new List<Map>(MapRegistry.GetAllMaps());

        public byte MaxMembers = 2;
        public bool Lock = false;

        private SettingsMenu()
        {
            Instance = this;
        }

        public static void Init()
        {
            if(Instance == null)
            {
                new SettingsMenu();
            }
        }

        public static void Open()
        {
            Instance.CreateDialog();
        }

        public static void Close()
        {
            if(Instance.isOpen)
            {
                Instance._dialog.Close();
                Instance._dialog = null;
                Instance._dialogButtons.Clear();
            }
        }

        private void UpdateDialog()
        {
            _dialog.Close();
            CreateDialog();
        }

        private void CreateDialog()
        {
            _dialogButtons = new DialogButton[4]
            {
                new DialogButton("Change Map", false, new UnityEngine.Events.UnityAction(()=>
                {
                    _curMapInt++;
                    if(_curMapInt > _mapsCount) _curMapInt = 1;
                    LobbyMap = MapRegistry.GetAllMaps().ElementAt(_curMapInt-1);
                    UpdateDialog();
                })),
                new DialogButton((Lock ? "Unlock" : "Lock"), false, new UnityEngine.Events.UnityAction(()=>
                {
                    Lock = !Lock;
                    UpdateDialog();
                })),
                new DialogButton("Change MaxMembers", false, new UnityEngine.Events.UnityAction(()=>
                {
                    dialogSetMaxMembers = DialogBoxManager.TextEntry("MaxMembers", "Value", new DialogButton[1]
                    {
                        new DialogButton("Set", true, new UnityEngine.Events.UnityAction(()=>SetMaxMembers()))
                    });
                })),
                new DialogButton("Ready.", true, new UnityEngine.Events.UnityAction(()=>
                {
                    Close();
                    MainMenu.NotOpen();
                }))
            }.ToList();

            string lobbyInfo = $"Map:\t{LobbyMap.name}\nMaxMembers:\t{MaxMembers}\nLock:\t{(Lock ? GUI.SetColor("Yes", ConsoleColor.Green) : GUI.SetColor("No", ConsoleColor.Red))}";

            _dialog = DialogBoxManager.Dialog(lobbyInfo, _dialogButtons.ToArray());
            GUI.TextNormilizeByDialogBox(_dialog);
        }

        DialogBox dialogSetMaxMembers;
        private void SetMaxMembers()
        {
            byte value;
            var result = byte.TryParse(dialogSetMaxMembers.EnteredText, out value);

            if (result)
            {
                MaxMembers = value;
                UpdateDialog();
            }
        }
    }
}
