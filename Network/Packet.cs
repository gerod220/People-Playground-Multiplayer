using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace Multiplayer.Network
{
    public struct ObjectInfo
    {
        public SteamId Owner;
        public string Name;
        public Guid Guid;
        public Vector3 Position;
        public Vector3 Rotation;
    }

    public class Packet
    {
        public enum EventId
        {
            //==ForAll==
            //-Client-
            ClientJoined,
            ClientLeaved,

            //-Object-
            SpawnObject,
            NewObjectInfo,
            DeleteObject,

            //-NetApi-
            NetVarRegister,
            NetEventRegister,

            //==ForClient==
            KickSelf,
        }

        // =Packet INFO=
        public EventId Event;
        public Friend Client;
        public bool ToClient = false;
        // =============

        // =Object INFO=
        public ObjectInfo Object;
        // =============

        // ===NetApi====
        public API.NetVar NetVar;
        public API.NetEvent NetEvent;
        // =============

        public static bool SendToAll(Lobby lobby, Packet packet)
        {
            Packet packetToAll = packet;
            return lobby.SendChatString(ObjectToString(packetToAll));
        }

        public static bool SendToMember(Lobby lobby, Friend member, Packet packet)
        {
            Packet packetToClient = packet;
            packetToClient.Client = member;
            packetToClient.ToClient = true;

            return lobby.SendChatString(ObjectToString(packetToClient));
        }

        public static Packet ConvertFrom(string xml)
        {
            return StringToPacket(xml);
        }

        private static string ObjectToString(object @object)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Packet));
            using (var sww = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(sww) { Formatting = Formatting.Indented })
                {
                    serializer.Serialize(writer, @object);
                    return sww.ToString();
                }
            }
        }

        private static Packet StringToPacket(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Packet));

            using (TextReader reader = new StringReader(xml))
            {
                return (Packet)serializer.Deserialize(reader);
            }
        }
    }
}
