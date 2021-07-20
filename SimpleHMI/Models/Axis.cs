using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHMI.Models
{
    public class Axis : BindableBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private double _maxPos;
        public double MaxPos
        {
            get { return _maxPos; }
            set { SetProperty(ref _maxPos, value); }
        }

        private double _homePose;
        public double HomePose
        {
            get { return _homePose; }
            set { SetProperty(ref _homePose, value); }
        }

        private double _stroke;
        public double Stroke
        {
            get { return _stroke; }
            set { SetProperty(ref _stroke, value); }
        }

        private double _speed;
        public double Speed
        {
            get { return _speed; }
            set { SetProperty(ref _speed, value); }
        }

        private double _acceleration;
        public double Acceleration
        {
            get { return _acceleration; }
            set { SetProperty(ref _acceleration, value); }
        }

        private double _maxStroke;
        public double MaxStroke
        {
            get { return _maxStroke; }
            set { SetProperty(ref _maxStroke, value); }
        }

        private double _maxSpeed;
        public double MaxSpeed
        {
            get { return _maxSpeed; }
            set { SetProperty(ref _maxSpeed, value); }
        }

        private double _maxAcceleration;
        public double MaxAcceleration
        {
            get { return _maxAcceleration; }
            set { SetProperty(ref _maxAcceleration, value); }
        }

        private double _setRestPoseJogAxis;
        public double SetRestPoseJogAxis
        {
            get { return _setRestPoseJogAxis; }
            set { SetProperty(ref _setRestPoseJogAxis, value); }
        }

        private double _setRestPoseMoveAxis;
        public double SetRestPoseMoveAxis
        {
            get { return _setRestPoseMoveAxis; }
            set { SetProperty(ref _setRestPoseMoveAxis, value); }
        }

        private double _setJogSpeed;
        public double SetJogSpeed
        {
            get { return _setJogSpeed; }
            set { SetProperty(ref _setJogSpeed, value); }
        }

        private string _um;
        public string UnitOfMeasure
        {
            get { return _um; }
            set { SetProperty(ref _um, value); }
        }

        private double _slowSpeed;
        public double SlowSpeed
        {
            get { return _slowSpeed; }
            set { SetProperty(ref _slowSpeed, value); }
        }

        private double _MOVE_AXIS_axisRef;
        public double MOVE_AXIS_axisRef
        {
            get { return _MOVE_AXIS_axisRef; }
            set { SetProperty(ref _MOVE_AXIS_axisRef, value); }
        }

        // Corrispondono JOG_AXIS_XVel, JOG_AXIS_YVel, JOG_AXIS_AVel
        private double _jogSpeed;
        public double JogSpeed
        {
            get { return _jogSpeed; }
            set { SetProperty(ref _jogSpeed, value); }
        }

        private int _EXT_REF_syncState;
        public int EXT_REF_syncState
        {
            get { return _EXT_REF_syncState; }
            set { SetProperty(ref _EXT_REF_syncState, value); }
        }

        private int _INT_REF_syncState;
        public int INT_REF_syncState
        {
            get { return _INT_REF_syncState; }
            set { SetProperty(ref _INT_REF_syncState, value); }
        }

        private double _INT_REF_A_pre;
        public double INT_REF_A_pre
        {
            get { return _INT_REF_A_pre; }
            set { SetProperty(ref _INT_REF_A_pre, value); }
        }

        private double _INT_REF_F_pre;
        public double INT_REF_F_pre
        {
            get { return _INT_REF_F_pre; }
            set { SetProperty(ref _INT_REF_F_pre, value); }
        }

        private double _INT_REF_O_pre;
        public double INT_REF_O_pre
        {
            get { return _INT_REF_O_pre; }
            set { SetProperty(ref _INT_REF_O_pre, value); }
        }

        private double _INT_REF_P_pre;
        public double INT_REF_P_pre
        {
            get { return _INT_REF_P_pre; }
            set { SetProperty(ref _INT_REF_P_pre, value); }
        }

        private double _INT_REF_sweepConstParam_pre;
        public double INT_REF_sweepConstParam_pre
        {
            get { return _INT_REF_sweepConstParam_pre; }
            set { SetProperty(ref _INT_REF_sweepConstParam_pre, value); }
        }

        // indica se è selezionato il bit dell'asse nella pagina EXT REF
        private bool _EXT_REF_activeAxis;
        public bool EXT_REF_activeAxis
        {
            get { return _EXT_REF_activeAxis; }
            set { SetProperty(ref _EXT_REF_activeAxis, value); }
        }

        // indica se è selezionato il bit dell'asse nella pagina INT REF
        private bool _INT_REF_activeAxis;
        public bool INT_REF_activeAxis
        {
            get { return _INT_REF_activeAxis; }
            set { SetProperty(ref _INT_REF_activeAxis, value); }
        }

        // valore di set che punta a modificare INT_REF_A_pre
        private double _set_INT_REF_A_pre;
        public double Set_INT_REF_A_pre
        {
            get { return _set_INT_REF_A_pre; }
            set { SetProperty(ref _set_INT_REF_A_pre, value); }
        }

        // valore di set che punta a modificare INT_REF_F_pre
        private double _set_INT_REF_F_pre;
        public double Set_INT_REF_F_pre
        {
            get { return _set_INT_REF_F_pre; }
            set { SetProperty(ref _set_INT_REF_F_pre, value); }
        }

        // valore di set che punta a modificare INT_REF_O_pre
        private double _set_INT_REF_O_pre;
        public double Set_INT_REF_O_pre
        {
            get { return _set_INT_REF_O_pre; }
            set { SetProperty(ref _set_INT_REF_O_pre, value); }
        }

        // valore di set che punta a modificare INT_REF_P_pre
        private double _set_INT_REF_P_pre;
        public double Set_INT_REF_P_pre
        {
            get { return _set_INT_REF_P_pre; }
            set { SetProperty(ref _set_INT_REF_P_pre, value); }
        }

        // stop mode (only linear sweep) -> connected to combobox
        private int _intRefStopModeSelected;
        public int IntRefStopModeSelected
        {
            get { return _intRefStopModeSelected; }
            set { SetProperty(ref _intRefStopModeSelected, value); }
        }

        // constant mode (only linear sweep) -> connected to combobox
        private int _intRefConstantModeSelected;
        public int IntRefConstantModeSelected
        {
            get { return _intRefConstantModeSelected; }
            set { SetProperty(ref _intRefConstantModeSelected, value); }
        }

        // SINE_WAVE oppure LIN_SWEEP oppure CONST_OFFSET 
        private int _INT_REF_type_pre;
        public int INT_REF_type_pre
        {
            get { return _INT_REF_type_pre; }
            set { SetProperty(ref _INT_REF_type_pre, value); }
        }
    }
}
