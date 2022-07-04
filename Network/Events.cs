using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Network
{
    public static class Events
    {
        public static bool SendJoinToAll(Lobby lobby)
        {
            Packet packetJoinEvent = new Packet();
            packetJoinEvent.Event = Packet.EventId.ClientJoined;
            return Packet.SendToAll(lobby, packetJoinEvent);
        }

        public static bool SendLeaveToAll(Lobby lobby)
        {
            Packet packetLeaveLobby = new Packet();
            packetLeaveLobby.Event = Packet.EventId.ClientLeaved;
            return Packet.SendToAll(lobby, packetLeaveLobby);
        }

        public static bool SendKickToClient(Lobby lobby, Friend client)
        {
            Packet packetKickSelf = new Packet();
            packetKickSelf.Event = Packet.EventId.KickSelf;
            return Packet.SendToMember(lobby, client, packetKickSelf);
        }

        public static bool SendNewObjectInfoToAll(Lobby lobby, ObjectInfo objectInfo)
        {
            Packet packetNewObjectInfo = new Packet();
            packetNewObjectInfo.Event = Packet.EventId.NewObjectInfo;
            packetNewObjectInfo.Object = objectInfo;
            return Packet.SendToAll(lobby, packetNewObjectInfo);
        }

        public static bool SendSpawnObjectToAll(Lobby lobby, ObjectInfo objectInfo)
        {
            Packet packetNewObject = new Packet();
            packetNewObject.Event = Packet.EventId.SpawnObject;
            packetNewObject.Object = objectInfo;
            return Packet.SendToAll(lobby, packetNewObject);
        }

        public static bool SendNetVarRegisterToAll(Lobby lobby, API.NetVar netVar)
        {
            Packet packetNetVarRegister = new Packet();
            packetNetVarRegister.Event = Packet.EventId.NetVarRegister;
            packetNetVarRegister.NetVar = netVar;
            return Packet.SendToAll(lobby, packetNetVarRegister);
        }

        public static bool SendNetEventRegisterToAll(Lobby lobby, API.NetEvent netEvent)
        {
            Packet packetNetEventRegister = new Packet();
            packetNetEventRegister.Event = Packet.EventId.NetVarRegister;
            packetNetEventRegister.NetEvent = netEvent;
            return Packet.SendToAll(lobby, packetNetEventRegister);
        }
    }
}
