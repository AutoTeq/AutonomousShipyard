using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;

namespace avaness.AutonomousShipyard
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Projector), false, "AutonomousShipyardLarge", "AutonomousShipyardSmall")]
    public class Logic : MyGameLogicComponent
    {
        public bool isServer;
        public bool isDedicated;
        public IMyProjector projector;
        public BlockSettings settings;
        private float welderSpeed;
        private StateMachine stateMachine;
        private BlockStateId initialState = BlockStateId.Idle;
        private readonly Guid cpmID = new Guid("801f61c8-140e-4f2e-9a0a-a24289376562");

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);

            NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
            NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
            NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            isServer = MyAPIGateway.Session.IsServer;
            isDedicated = MyAPIGateway.Utilities.IsDedicated;
            projector = Entity as IMyProjector;
            //gameThreadId = Environment.CurrentManagedThreadId;
            if (projector.CubeGrid?.Physics == null) return;
            //mgpAPI = new MultigridProjectorModAgent();

            //if (mgpAPI != null)
            //isMGPInstalled = mgpAPI.Available;

            //projector.AppendingCustomInfo += CustomInfo;
            AutonomousShipyardSession.Instance.InitControls();
            welderSpeed = MyAPIGateway.Session.WelderSpeedMultiplier;

            if (isServer)
            {
                //projector.IsWorkingChanged += CheckIsWorking;
                settings = LoadSettings();

                stateMachine = new StateMachine(this);

                // Register all states here!!
                //stateMachine.RegisterState(new Idle_State());

                stateMachine.ChangeState(initialState);
            }
            //else
                //Comms.RequestSettings(MyAPIGateway.Multiplayer.MyId, projector.EntityId);

            //Session.Instance.projectorBlocks.Add(projector);
        }

        public override void UpdateBeforeSimulation10()
        {
            stateMachine.Update();
        }

        private BlockSettings LoadSettings()
        {
            try
            {
                BlockSettings data = new BlockSettings(projector.EntityId);
                if (projector.Storage != null)
                {
                    byte[] byteData;

                    string storage = projector.Storage[cpmID];
                    byteData = Convert.FromBase64String(storage);
                    data = MyAPIGateway.Utilities.SerializeFromBinary<BlockSettings>(byteData);

                    data._blockId = projector.EntityId;
                    return data;
                }

                return data;
            }
            catch (Exception ex)
            {
                BlockSettings data = new BlockSettings(projector.EntityId);
                return data;
            }
        }

        public void SaveSettings(BlockSettings settings)
        {
            IMyEntity entity = null;
            MyAPIGateway.Entities.TryGetEntityById(settings._blockId, out entity);
            if (entity == null) return;

            if (entity.Storage != null)
            {
                var newByteData = MyAPIGateway.Utilities.SerializeToBinary(settings);
                var base64string = Convert.ToBase64String(newByteData);
                entity.Storage[cpmID] = base64string;
            }
            else
            {
                entity.Storage = new MyModStorageComponent();

                var newByteData = MyAPIGateway.Utilities.SerializeToBinary(settings);
                var base64string = Convert.ToBase64String(newByteData);
                entity.Storage[cpmID] = base64string;
            }
        }
    }
}
