using Multiplayer.Network;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiplayer.GUI
{
    public class Cursor : MonoBehaviour
    {
        public Friend Owner;
        private Vector3 OldPosition, CurrentPosition = new Vector3();

        private void Start()
        {
            CurrentPosition = Global.main.MousePosition;
            OldPosition = CurrentPosition;
            Owner = new Friend(SteamClient.SteamId);

            Client.ClientManager.Cursors.Add(this);
        }

        private void Update()
        {
            CurrentPosition = Global.main.MousePosition;
            if (CurrentPosition != OldPosition)
            {
                OldPosition = CurrentPosition;
                //gameObject.transform.position = CurrentPosition;

                ObjectInfo cursorInfo = new ObjectInfo();
                cursorInfo.Guid = Guid.NewGuid();
                cursorInfo.Name = "Cursor";
                cursorInfo.Owner = Owner.Id;
                cursorInfo.Position = CurrentPosition;

                Events.SendNewObjectInfoToAll(Client.ClientManager.CurrentLobby, cursorInfo);
            }
        }

        public void SetPos(Vector3 pos)
        {
            gameObject.transform.position = pos;
        }
    }
}
