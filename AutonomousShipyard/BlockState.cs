using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avaness.AutonomousShipyard
{
    public enum BlockStateId
    {
        Idle,
        

    }

    public interface BlockState
    {
        BlockStateId GetId();
        void Enter(Logic logic);
        void UpdateBlock(Logic logic);
        void Exit(Logic logic);
    }
}
