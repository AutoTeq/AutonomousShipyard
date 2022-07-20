using ProtoBuf;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.ModAPI;

namespace avaness.AutonomousShipyard
{
    public enum DataType
    {
        Sync,
        RequestSettings,
        SendSettings,
        DetailInfo,
    }

    [ProtoContract]
    public class ObjectContainer
    {
        [ProtoMember(1)] public BlockSettings settings;
        [ProtoMember(2)] public ulong steamId;
        [ProtoMember(3)] public long blockId;
    }

    [ProtoContract]
    public class CommsPackage
    {
        [ProtoMember(1)]
        public DataType Type;

        [ProtoMember(2)]
        public byte[] Data;

        public CommsPackage()
        {
            Type = DataType.Sync;
            Data = new byte[0];
        }

        public CommsPackage(DataType type, ObjectContainer oc)
        {
            Type = type;
            Data = MyAPIGateway.Utilities.SerializeToBinary(oc);
        }
    }


    public static class Comms
    {





        private static ushort networkId = 4327;

        public static void SyncSettings(BlockSettings settings)
        {
            ObjectContainer objectContainer = new ObjectContainer()
            {
                settings = settings
            };

            CommsPackage package = new CommsPackage(DataType.Sync, objectContainer);
            var sendData = MyAPIGateway.Utilities.SerializeToBinary(package);
            MyAPIGateway.Multiplayer.SendMessageToOthers(networkId, sendData);

            if (MyAPIGateway.Session.IsServer)
            {
                IMyEntity entity;
                if (!MyAPIGateway.Entities.TryGetEntityById(settings._blockId, out entity)) return;
                var logic = entity.GameLogic.GetAs<Logic>();
                if (logic == null) return;

                logic.SaveSettings(logic.settings);
            }
        }

        public static void RequestConfig(ulong steamId)
        {
            ObjectContainer objectContainer = new ObjectContainer()
            {
                steamId = steamId
            };

            CommsPackage package = new CommsPackage(DataType.RequestConfig, objectContainer);
            var sendData = MyAPIGateway.Utilities.SerializeToBinary(package);
            MyAPIGateway.Multiplayer.SendMessageToServer(networkId, sendData);
        }

        

        public static void RequestSettings(ulong steamId, long blockId)
        {
            ObjectContainer objectContainer = new ObjectContainer()
            {
                steamId = steamId,
                blockId = blockId
            };

            CommsPackage package = new CommsPackage(DataType.RequestSettings, objectContainer);
            var sendData = MyAPIGateway.Utilities.SerializeToBinary(package);
            MyAPIGateway.Multiplayer.SendMessageToServer(networkId, sendData);
        }

        public static void SendSettings(BlockSettings settings, long blockId, ulong steamId)
        {
            ObjectContainer objectContainer = new ObjectContainer()
            {
                settings = settings,
                blockId = blockId
            };

            CommsPackage package = new CommsPackage(DataType.SendSettings, objectContainer);
            var sendData = MyAPIGateway.Utilities.SerializeToBinary(package);
            MyAPIGateway.Multiplayer.SendMessageTo(networkId, sendData, steamId);
        }

        /*public static void SyncDetailInfo(string stringBuilder, long blockId)
        {
            ObjectContainer objectContainer = new ObjectContainer()
            {
                text = stringBuilder,
                blockId = blockId
            };

            CommsPackage package = new CommsPackage(DataType.DetailInfo, objectContainer);
            var sendData = MyAPIGateway.Utilities.SerializeToBinary(package);
            MyAPIGateway.Multiplayer.SendMessageToOthers(6200, sendData);
        }*/


        public static void MessageHandler(byte[] data)
        {
            try
            {
                var package = MyAPIGateway.Utilities.SerializeFromBinary<CommsPackage>(data);
                if (package == null) return;

                if (package.Type == DataType.Sync)
                {
                    var packet = MyAPIGateway.Utilities.SerializeFromBinary<ObjectContainer>(package.Data);
                    if (packet == null) return;

                    BlockSettings.SyncSettings(packet.settings);
                    return;
                }

                if (package.Type == DataType.RequestSettings)
                {
                    var packet = MyAPIGateway.Utilities.SerializeFromBinary<ObjectContainer>(package.Data);
                    if (packet == null) return;

                    IMyEntity entity;
                    if (!MyAPIGateway.Entities.TryGetEntityById(packet.blockId, out entity)) return;

                    var logic = entity.GameLogic.GetAs<Logic>();
                    if (logic == null) return;

                    SendSettings(logic.settings, packet.blockId, packet.steamId);
                    return;
                }

                if (package.Type == DataType.SendSettings)
                {
                    var packet = MyAPIGateway.Utilities.SerializeFromBinary<ObjectContainer>(package.Data);
                    if (packet == null) return;

                    IMyEntity entity;
                    if (!MyAPIGateway.Entities.TryGetEntityById(packet.blockId, out entity)) return;

                    var logic = entity.GameLogic.GetAs<Logic>();
                    if (logic == null) return;

                    logic.settings = packet.settings;
                    return;
                }

                /*if (package.Type == DataType.DetailInfo)
                {
                    var packet = MyAPIGateway.Utilities.SerializeFromBinary<ObjectContainer>(package.Data);
                    if (packet == null) return;

                    IMyEntity entity;
                    if (!MyAPIGateway.Entities.TryGetEntityById(packet.blockId, out entity)) return;

                    var logic = entity.GameLogic.GetAs<Logic>();
                    if (logic == null) return;

                    logic.detailInfo = packet.text;
                    var terminal = entity as IMyTerminalBlock;
                    if (terminal == null) return;

                    terminal.RefreshCustomInfo();
                    Session.RefreshControls(terminal);
                    return;
                }*/
            }
            catch (Exception ex)
            {

            }
        }
    }
}
