using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avaness.AutonomousShipyard
{
    public class StateMachine
    {
        public BlockState[] states;
        public Logic logic;
        public BlockStateId currentState;

        public StateMachine(Logic logic)
        {
            this.logic = logic;
            int numStates = System.Enum.GetNames(typeof(BlockStateId)).Length;
            states = new BlockState[numStates];
        }

        public void RegisterState(BlockState state)
        {
            int index = (int)state.GetId();
            states[index] = state;
        }

        public BlockState GetState(BlockStateId stateId)
        {
            int index = (int)stateId;
            return states[index];
        }

        public void Update()
        {
            GetState(currentState)?.UpdateBlock(logic);
        }

        public void ChangeState(BlockStateId newState)
        {
            GetState(currentState)?.Exit(logic);
            currentState = newState;
            GetState(currentState)?.Enter(logic);
        }
    }
}
