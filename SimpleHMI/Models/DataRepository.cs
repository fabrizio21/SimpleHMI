using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHMI.Models
{
    /// <summary>
    /// Data read from the plc and put in this static (structure) class
    /// </summary>
    public static class DataRepository {
        public static int ctrlState;                            // DISABLE=0, ENABLE=1, ERROR=2, RESET ERROR=-2
        public static int ctrlMode;                             // MODE_JOG_AXIS=1, MODE_MOVE_AXIS=2, MODE_INT_REF=3, MODE_EXT_REF=4 
        public static int modeState;                            // PREOP_=0, READY_=1, RUNNING_=2, PAUSE_=3, STOP_=4
        public static int demoState;                            // DEMO_ON=1, DEMO_OFF=0
        
        public static double[] axis_FB;
        public static double[] axis_maxPos;
        public static double[] axis_maxSpeed;
        public static double[] axis_maxAcc;
        
        public static double[] INT_REF_A_pre;
        public static double[] INT_REF_F_pre;
        public static double[] INT_REF_O_pre;
        public static double[] INT_REF_P_pre;
        public static int[] INT_REF_syncState;                  // NO_MOTION=0, SYNC=1, RAMP_UP=2, RAMP_DOWN=-2, PAUSED=-1
        public static double[] INT_REF_sweepConstParam_pre;
        public static int INT_REF_sineLimitsViolated;
        public static int INT_REF_sweepLimitsViolated;
        
        public static int[]	EXT_REF_syncState;

        public static int ERROR_CODE;                           // ERROR_CODE

        public static int[] mot_SW;                             // mot_SW(0) = X1_SW (gantry master per asse X)
                                                                // mot_SW(1) = X2_SW(gantry follower per asse X)
                                                                // mot_SW(2) = X3_SW(gantry follower per asse X)
                                                                // mot_SW(3) = X4_SW(gantry follower per asse X)
                                                                // mot_SW(4) = Y1_SW(gantry master per asse Y)
                                                                // mot_SW(5) = Y2_SW(gantry follower per asse Y)
                                                                // mot_SW(6) = R1_SW(gantry master per asse ROT)
                                                                // mot_SW(7) = R2_SW(gantry follower per asse ROT)

                                                                // bit0 -> Amplifier fault
                                                                // bit1 -> Encoder loss detection
                                                                // bit2 -> Fatal following error
                                                                // bit3 -> Integrated current limit
                                                                // bit4 -> Software positive stroke limit
                                                                // bit5 -> Software negative stroke limit
                                                                // bit6 -> Hardware positive stroke limit
                                                                // bit7 -> Hardware negative stroke limit
                                                                // bit8 -> Auxiliary fault trigger

        public static int EXT_REF_processErr;                   // bit0 -> buffer active control failure
                                                                // bit1 -> ASC failure on axis X
                                                                // bit2 -> ASC failure on axis Y
                                                                // bit3 -> ASC failure on axis ROT

        public static double[] homePose;
        public static double[] axis_slowSpeed;

        public static double X_max_stroke;
        public static double X_max_speed;
        public static double X_max_acc;

        public static double Y_max_stroke;
        public static double Y_max_speed;
        public static double Y_max_acc;

        public static double ROT_max_stroke;        
        public static double ROT_max_speed;
        public static double ROT_max_acc;

        public static string Unsolicited;                       // last unsolicited message

        public static int JOG_AXIS_controlWord;                 // X - bit 0, Y - bit 1, Rot - bit 2

        /// <summary>
        /// Initialization. Usually done in the bootstrap
        /// </summary>
        /// <param name="numAxes"></param>
        public static void Init(int numAxes) {
            axis_FB = new double[numAxes];
            axis_maxPos = new double[numAxes];
            axis_maxSpeed = new double[numAxes];
            axis_maxAcc = new double[numAxes];
            INT_REF_A_pre = new double[numAxes];
            INT_REF_F_pre = new double[numAxes];
            INT_REF_O_pre = new double[numAxes];
            INT_REF_P_pre = new double[numAxes];
            INT_REF_syncState = new int[numAxes];
            INT_REF_sweepConstParam_pre = new double[numAxes];
            EXT_REF_syncState = new int[numAxes];
            mot_SW = new int[8];
            homePose = new double[numAxes];
            axis_slowSpeed = new double[numAxes];
        }
    }
}
