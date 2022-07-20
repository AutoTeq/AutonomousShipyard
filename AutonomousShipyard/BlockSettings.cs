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
    [ProtoContract]
    public class BlockSettings
    {
        [ProtoMember(1)] public bool? _enabled = null;
        [ProtoMember(2)] public long? _blockId = null;

        public BlockSettings() { }

        public BlockSettings(long id)
        {
            _blockId = id;
        }

        public bool Enabled
        {
            get { return _enabled ?? false; }
            set
            {
                _enabled = value;
                BlockSettings settings = new BlockSettings(_blockId ?? 0)
                {
                    _enabled = value
                };

                Comms.SyncSettings(settings);
            }
        }

        public static void SyncSettings(BlockSettings settings)
        {
            IMyEntity entity;
            if (!MyAPIGateway.Entities.TryGetEntityById(settings._blockId, out entity)) return;

            var logic = entity.GameLogic.GetAs<Logic>();
            if (logic == null) return;

            if (settings._enabled.HasValue)
                logic.settings._enabled = settings._enabled;

            if (MyAPIGateway.Session.IsServer)
                logic.SaveSettings(logic.settings);
        }
    }
}
