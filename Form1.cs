using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HowickLtdCiA402
{
    public partial class Form1 : Form
    {
        // Declare states, transitions, motor and other variables that need to be accessed across different methods
        // The implementation for the State, Transition and Motor classes are available in the class files

        State currentState;
        State startState;
        State notReadyToSwitchOnState;
        State switchOnDisabledState;
        State readyToSwitchOnState;
        State switchedOnState;
        State operationEnabledState;
        State quickStopActiveState;
        State faultReactionActiveState;
        State faultState;

        Transition transition_xxxxx110;
        Transition transition_xxxxx111;
        Transition transition_xxxx1111;
        Transition transition_xxxx0111;
        Transition transition_xxxxx01x;
        Transition transition_xxxxxx0x;
        Transition transition_1xxxxxxx;

        Motor motor;

        bool initialised = false;
        int[] controlWord;
        int[] statusWord;

        int faultBit = 12;

        public Form1()
        {
            InitializeComponent();
        }

        public void Initialise()
        {
            // Sets up the states and transitions for the state machine

            startState = new State("Start", new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            notReadyToSwitchOnState = new State("Not ready to switch on", new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 0, 0, 0, 0 });
            switchOnDisabledState = new State("Switch on disabled", new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 0, 0, 0, 0 });
            readyToSwitchOnState = new State("Ready to switch on", new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 2, 0, 0, 0, 1 });
            switchedOnState = new State("Switched on", new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 2, 0, 0, 1, 1 });
            operationEnabledState = new State("Operation enabled", new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 2, 0, 1, 1, 1 });
            quickStopActiveState = new State("Quick stop active", new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 0, 1, 1, 1 });
            faultReactionActiveState = new State("Fault reaction active", new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 1, 1, 1, 1 });
            faultState = new State("Fault", new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 1, 0, 0, 0});

            transition_xxxxx110 = new Transition(new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 0 });
            transition_xxxxx111 = new Transition(new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1 });
            transition_xxxx1111 = new Transition(new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1 });
            transition_xxxx0111 = new Transition(new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0 });
            transition_xxxxx01x = new Transition(new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 2 });
            transition_xxxxxx0x = new Transition(new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 2 });
            transition_1xxxxxxx = new Transition(new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2 });

            motor = new Motor();

            currentState = startState;
            statusWord = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        }

        public bool ValidTransition(Transition transitionToCheck)
        {
            // Checks whether the current control word is a valid transition and will cause a change of state
            // Returns true if the transition is valid

            // For each bit in the transition that is not 'x', an exact match is required
            // 'x' characters are denoted as 2 in the control word and transition arrays
            for (int i = 0; i < controlWord.Length; i++)
            {
                if ((transitionToCheck.transitionArray[i] != 2) && (controlWord[i] != transitionToCheck.transitionArray[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public void UpdateStatusWord(State nextState)
        {
            // Updates the necessary bits in the status word

            for (int i = 0; i < statusWord.Length; i++)
            {
                if (nextState.statusWordArray[i] != 2)
                {
                    statusWord[i] = nextState.statusWordArray[i];
                }
            }
        }

        public string StatusWordToString()
        {
            // Converts the status word array into a string to use to display in output textbox

            string stringOutput = "";

            foreach (int bit in statusWord)
            {
                if (bit == 0)
                {
                    stringOutput += "0";
                } else if (bit == 1)
                {
                    stringOutput += "1";
                }
            }

            return stringOutput;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // To simulate a real-time system, a timer was used
            // The timer ticks every 100ms - which can be customised depending on the specifications of the system

            if (!initialised)
            {
                Initialise();
                initialised = true;
            } 

            controlWord = new int[] { (int)nud_cw_bit15.Value, (int)nud_cw_bit14.Value, (int)nud_cw_bit13.Value, (int)nud_cw_bit12.Value, (int)nud_cw_bit11.Value, (int)nud_cw_bit10.Value, (int)nud_cw_bit9.Value, (int)nud_cw_bit8.Value, (int)nud_cw_bit7.Value, (int)nud_cw_bit6.Value, (int)nud_cw_bit5.Value, (int)nud_cw_bit4.Value, (int)nud_cw_bit3.Value, (int)nud_cw_bit2.Value, (int)nud_cw_bit1.Value, (int)nud_cw_bit0.Value };

            // If a fault is detected in any state other than the fault states (bit 3 in the statusword is high)
            if ((statusWord[faultBit] == 1) && (currentState != faultReactionActiveState) && (currentState != faultState))
            {
                currentState = faultReactionActiveState;
                motor.TurnOffMotor();
                MessageBox.Show("Activate FaultReset(Bit 7) to reset system");
            }

            // A switch case is used as it provides a simple implementation for a state machine
            // For states that require transitions, the ValidTransition function is called to check if the control word will cause a state change
            switch (currentState.stateString)
            {
                case "Start":
                    currentState = notReadyToSwitchOnState;
                    break;
                case "Not ready to switch on":
                    currentState = switchOnDisabledState;
                    break;
                case "Switch on disabled":
                    if (ValidTransition(transition_xxxxx110))
                    {
                        currentState = readyToSwitchOnState;
                    }
                    break;
                case "Ready to switch on":
                    if (ValidTransition(transition_xxxxx111))
                    {
                        currentState = switchedOnState;
                    }
                    else if ((ValidTransition(transition_xxxxx01x)) || (ValidTransition(transition_xxxxxx0x)))
                    {
                        currentState = switchOnDisabledState;
                    }
                    break;
                case "Switched on":
                    if (ValidTransition(transition_xxxx1111))
                    {
                        currentState = operationEnabledState;
                    }
                    else if (ValidTransition(transition_xxxxx110))
                    {
                        currentState = readyToSwitchOnState;
                    }
                    else if ((ValidTransition(transition_xxxxx01x)) || (ValidTransition(transition_xxxxxx0x)))
                    {
                        currentState = switchOnDisabledState;
                    }
                    break;
                case "Operation enabled":
                    // The motor is only active in this state and takes an Int16 (short) data input
                    motor.SetTargetVelocity((short) nud_targetmotorvelocity.Value);

                    if (ValidTransition(transition_xxxxx110))
                    {
                        currentState = readyToSwitchOnState;
                        motor.TurnOffMotor();
                    }
                    else if (ValidTransition(transition_xxxxxx0x))
                    {
                        currentState = switchOnDisabledState;
                        motor.TurnOffMotor();
                    }
                    else if (ValidTransition(transition_xxxx0111))
                    {
                        currentState = switchedOnState;
                        motor.TurnOffMotor();
                    }
                    else if (ValidTransition(transition_xxxxx01x))
                    {
                        currentState = quickStopActiveState;
                        motor.TurnOffMotor();
                    }
                    break;
                case "Quick stop active":
                    if (ValidTransition(transition_xxxxxx0x))
                    {
                        currentState = switchOnDisabledState;
                    }
                    break;
                case "Fault reaction active":
                    currentState = faultState;
                    break;
                case "Fault":
                    if (ValidTransition(transition_1xxxxxxx))
                    {
                        currentState = switchOnDisabledState;
                    }
                    break;
            }

            UpdateStatusWord(currentState);

            txtbox_currentstate.Text = currentState.stateString;
            txtbox_motorvelocity.Text = motor.velocity.ToString();
            txtbox_statusword.Text = StatusWordToString();

        }

        private void btn_simulatefault_Click(object sender, EventArgs e)
        {
            // This button allows the user to observe the behaviour of the system when a fault is simulated by setting the fault bit to high

            statusWord[faultBit] = 1;
        }
    }
}
