using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiplayer.API
{
    public enum NetEventType
    {
        OnLobbyCreated,
    }

    public struct NetEvent
    {
        public ModMetaData ByMod;
        public string Name;
        public object Handle;
    }

    public enum NetVarType
    {
        String,
        Number,
    }

    public struct NetVar
    {
        public ModMetaData ByMod;
        public string Name;
        public NetVarType Type;
        public object Value;
    }

    public class APIController : MonoBehaviour
    {
        public static GameObject API = new GameObject("MP:API");
        public static APIController Instance = null;

        public static List<NetVar> NetworkVaribles = new List<NetVar>();
        public static List<NetEvent> NetworkEvents = new List<NetEvent>();

        public static void Init()
        {
            API.AddComponent<APIController>();
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(gameObject);

            Instance = this;
        }

        private void Update()
        {

        }

        private void Destroy()
        {

        }

        // =========== API =============
        private void GetMPVer(object[] args)
        {
            Action<string> getter = (Action<string>)args[0];
            getter.Invoke(Mod.MPVer);
        }

        private void RegisterNetVar(object[] args)
        {
            Action<bool> getter = (Action<bool>)args[0];
            string name = (string)args[1];
            int typeInt = (int)args[2];
            object value = args[3];
            ModMetaData meta = (ModMetaData)args[4];
            
            NetVarType type = (NetVarType)typeInt;

            foreach (var netVar in NetworkVaribles)
            {
                if(netVar.Name == name)
                {
                    Debug.LogWarning($"[MP] NetVar '{meta.Name}.{name}' conflicts with '{netVar.ByMod.Name}.{name}'");
                    getter.Invoke(false);
                    return;
                }
            }
            
            NetworkVaribles.Add(new NetVar()
            {
                ByMod = meta,
                Name = name,
                Type = type,
                Value = value,
            });

            Debug.Log($"[MP] NetVar '{meta.Name}.{name}' registered.");
            getter.Invoke(true);
        }

        private void RegisterEvent(object[] args)
        {
            Action<bool> getter = (Action<bool>)args[0];
            string eventName = (string)args[1];
            object eventHandle = args[3];
            ModMetaData meta = (ModMetaData)args[4];

            foreach (var @netEvent in NetworkEvents)
            {
                if(netEvent.Name == eventName)
                {
                    Debug.LogWarning($"[MP] NetEvent '{meta.Name}.{name}' conflicts with '{netEvent.ByMod.Name}.{name}'");
                    getter.Invoke(false);
                    return;
                }
            }

            var @event = new NetEvent()
            {
                ByMod = meta,
                Name = name,
                Handle = eventHandle
            };

            NetworkEvents.Add(@event);
            if(Client.ClientManager.CurrentLobby.Id.IsValid)
            {
                Network.Events.SendNetEventRegisterToAll(Client.ClientManager.CurrentLobby, @event);
            }
            
            Debug.Log($"[MP] NetEvent '{meta.Name}.{name}' registered.");
            getter.Invoke(true);
        }

        private void ListenEvent(object[] args)
        {
            Action<bool> getter = (Action<bool>)args[0];
            string eventName = (string)args[1];
            object called = args[2];
            ModMetaData meta = (ModMetaData)args[3];

            NetEventType netEvent = (NetEventType)Enum.Parse(typeof(NetEventType), eventName);

            if (netEvent == NetEventType.OnLobbyCreated)
            {
                //SteamMatchmaking.OnLobbyCreated += NetEventsController.LobbyCreated;
                NetEventsController.NetEventLobbyCreated.Add((Action<ulong>)called);
                Debug.Log($"[MP] NetEvent '{eventName}' listen by '{meta.Name}..'");
                getter.Invoke(true);
                return;
            }
            getter.Invoke(false);
        }
    }

    public class NetEventsController
    {
        public static List<Action<ulong>> NetEventLobbyCreated;
        internal static void LobbyCreated(Result arg1, Lobby arg2)
        {
            foreach (var func in NetEventLobbyCreated)
            {
                func?.Invoke(arg2.Id.Value);
            }
        }
    }
}
