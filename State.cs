using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowickLtdCiA402
{

    public class State
    {
        public string stateString;
        public int[] statusWordArray;

        public State(string stateToInitialise, int[] statusWordToInitialise)
        {
            stateString = stateToInitialise;
            statusWordArray = statusWordToInitialise;
        }
    }
}
