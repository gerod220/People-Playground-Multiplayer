using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using UnityEngine;

namespace Multiplayer.Network
{
    internal class NetworkObjectController : MonoBehaviour
    {
        public static List<GameObject> Objects = new List<GameObject>();
        private static event Action<ObjectInfo> EUpdateObjectInfo;

        private SteamId _Owner => SteamClient.SteamId;
        private GameObject Object => gameObject;
        public string Name => Object.name;
        public Guid Guid = Guid.NewGuid();
        public bool isStatic = false;
        public Vector3 OldPosition = new Vector3(0, 0, 0);
        public Quaternion OldRotation = new Quaternion(0, 0, 0, 0);
        public Vector3 OldVelocity = new Vector3(0, 0, 0);

        private ObjectInfo _NewEObjectInfo = new ObjectInfo();
        private bool ApplyUpdate = false;

        private void Start()
        {
            Objects.Add(Object);
            EUpdateObjectInfo += OnUpdateObjectInfo;

            OldPosition = Object.transform.position;
            OldRotation = Object.transform.rotation;
        }

        private void OnUpdateObjectInfo(ObjectInfo @object)
        {
            if(@object.Guid == Guid)
            {
                ApplyUpdate = true;
                _NewEObjectInfo = @object;
                Object.transform.position = @object.Position;
                OldPosition = @object.Position;
            }
        }

        public static void CallUpdateObjectInfo(ObjectInfo obj)
        {
            EUpdateObjectInfo?.Invoke(obj);
        }

        private void Update()
        {
            var currentPos = Object.transform.position;
            if (OldPosition.x != currentPos.x || OldPosition.y != currentPos.y || OldPosition.z != currentPos.z)
            {
                ObjectInfo info = new ObjectInfo();
                info.Owner = _Owner;
                info.Name = Name;
                info.Guid = Guid;

                info.Position = currentPos;

                if (!ApplyUpdate) Events.SendNewObjectInfoToAll(Client.ClientManager.CurrentLobby, info);
                else ApplyUpdate = false;

                OldPosition = new Vector3(currentPos.x, currentPos.y, currentPos.z);
            }
        }

        private void Destroy()
        {

        }
    }
}
