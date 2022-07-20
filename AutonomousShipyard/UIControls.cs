using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Utils;

namespace avaness.AutonomousShipyard
{
    public static class UIControls
    {
        private static bool controlsCreated;
        private static IMyTerminalBlock current;
        private static string blockSubtype = "AutonomousShipyard";
        private static IMyTerminalControlOnOffSwitch refreshToggle;

        public static void CreateControls(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            if (block == null || block as IMyProjector == null) return;
            var logic = block.GameLogic.GetAs<Logic>();
            if (logic == null) return;
            if (logic.settings == null)
            {
                Comms.RequestSettings(MyAPIGateway.Multiplayer.MyId, block.EntityId);
                return;
            }


            current = block;
            GetRefreshToggle();

            // Update Labels here
            /*foreach (var control in controls)
            {
                if (control.Id.Contains("Label"))
                {
                    var label = control as IMyTerminalControlLabel;
                    if (label == null) continue;

                    string info = label.Label.ToString();
                    if (info.Contains("Spend"))
                    {
                        label.Label = MyStringId.GetOrCompute($"Spend {GetTokenAmount()} {token}\nfor {TimeSpan.FromSeconds(config.boostTime * GetTokenAmount())} of boost");
                        label.UpdateVisual();
                        label.RedrawControl();
                        break;
                    }
                }
            }*/

            if (controlsCreated) return;
            controlsCreated = true;

            // Seperate A
            var sepA = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSeparator, IMyProjector>("SepARepair");
            sepA.Enabled = Block => true;
            sepA.SupportsMultipleBlocks = false;
            sepA.Visible = Block => IsProjector(Block);
            MyAPIGateway.TerminalControls.AddControl<IMyProjector>(sepA);
            controls.Add(sepA);

            // AutoRepair Switch
            var repairSwitch = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, IMyProjector>("AutoRepairEnable");
            repairSwitch.Enabled = Block => true;
            repairSwitch.SupportsMultipleBlocks = false;
            repairSwitch.Visible = Block => IsProjector(Block);
            repairSwitch.Title = MyStringId.GetOrCompute("Enable AutoShipyard");
            repairSwitch.OnText = MyStringId.GetOrCompute("On");
            repairSwitch.OffText = MyStringId.GetOrCompute("Off");
            repairSwitch.Getter = Block => IsAutoEnabled(Block);
            repairSwitch.Setter = (Block, Builder) => SetAutoEnabled(Block, Builder);
            MyAPIGateway.TerminalControls.AddControl<IMyProjector>(repairSwitch);
            controls.Add(repairSwitch);

            
        }

        private static bool IsProjector(IMyTerminalBlock block)
        {
            if (block as IMyProjector != null)
            {
                if (block.BlockDefinition.SubtypeName.Contains(blockSubtype))
                    return true;
            }

            return false;
        }

        private static bool IsAutoEnabled(IMyTerminalBlock block)
        {
            var logic = block.GameLogic.GetAs<Logic>();
            if (logic == null) return false;

            //InControlPanel = true;
            return logic.settings.Enabled;
        }

        private static void SetAutoEnabled(IMyTerminalBlock block, bool value)
        {
            var logic = block.GameLogic.GetAs<Logic>();
            if (logic == null) return;

            logic.settings.Enabled = value;

            RefreshControls(block);
        }

        private static void GetRefreshToggle()
        {
            return;
            List<IMyTerminalControl> items;
            MyAPIGateway.TerminalControls.GetControls<IMyTerminalBlock>(out items);

            foreach (var item in items)
            {
                if (item.Id == "ShowInToolbarConfig")
                {
                    refreshToggle = (IMyTerminalControlOnOffSwitch)item;
                    break;
                }
            }
        }

        private static void RefreshControls(IMyTerminalBlock b)
        {
            /*if (MyAPIGateway.Gui.GetCurrentScreen != MyTerminalPageEnum.ControlPanel) return;
            if (refreshtoggle != null)
            {
                var originalSetting = refreshtoggle.Getter(b);
                refreshtoggle.Setter(b, !originalSetting);
                refreshtoggle.Setter(b, originalSetting);
            }*/

            /*if (MyAPIGateway.Gui.GetCurrentScreen == MyTerminalPageEnum.ControlPanel)
            {
                var myCubeBlock = b as MyCubeBlock;

                if (myCubeBlock.IDModule != null)
                {

                    var share = myCubeBlock.IDModule.ShareMode;
                    var owner = myCubeBlock.IDModule.Owner;
                    myCubeBlock.ChangeOwner(owner, share == MyOwnershipShareModeEnum.None ? MyOwnershipShareModeEnum.All : MyOwnershipShareModeEnum.None);
                    myCubeBlock.ChangeOwner(owner, share);
                }
            }*/
        }
    }
}
