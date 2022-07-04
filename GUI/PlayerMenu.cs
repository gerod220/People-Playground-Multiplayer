using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.GUI
{
    public class PlayerMenu
    {
        public static void Open(string player)
        {
            if (Client.ClientManager.CurrentLobby.Owner.Id != SteamClient.SteamId) return;

            Friend client = new Friend();
            foreach (var member in Client.ClientManager.CurrentLobby.Members)
            {
                if (member.Name == player)
                {
                    client = member;
                    break;
                }
            }

            DialogBoxManager.Dialog($"Player: \"{client.Name}\"", new DialogButton[2]
            {
                new DialogButton("Kick", true, new UnityEngine.Events.UnityAction(()=>
                {
                    Network.Events.SendKickToClient(Client.ClientManager.CurrentLobby, client);
                })),
                new DialogButton("Close", true, new UnityEngine.Events.UnityAction(()=>{ }))
            });
        }
    }
}
