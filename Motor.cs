using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowickLtdCiA402
{
    public class Motor
    {
        public short velocity;

        public Motor()
        {
            velocity = 0;
        }

        public void SetTargetVelocity(short targetVelocity)
        {
            // Assuming perfect and instant motor

            velocity = targetVelocity;
        }

        public void TurnOffMotor()
        {
            velocity = 0;
        }
    }
}
