using Prism.Commands;
using Prism.Mvvm;
using SimpleHMI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SimpleHMI.ViewModels
{
    public class FakeViewModel : BindableBase
    {

        public ICommand ToggleJogAxisBitCommand { get; private set; }

        ObservableCollection<SetupItem> _inputs;
        ObservableCollection<SetupItem> _outputs;

        public ObservableCollection<SetupItem> Inputs
        {
            get { return _inputs; }
            set { SetProperty(ref _inputs, value); }
        }

        public ObservableCollection<SetupItem> Outputs
        {
            get { return _outputs; }
            set { SetProperty(ref _outputs, value); }
        }

        private AxisList _axisList;
        public AxisList AxisList
        {
            get { return _axisList; }
            set { SetProperty(ref _axisList, value); }
        }

        private int _jogAxisControlWord;
        public int JogAxisControlWord
        {
            get { return _jogAxisControlWord; }
            set { SetProperty(ref _jogAxisControlWord, value); }
        }

        private double _translationSpeedMoveAxis;
        public double TranslationSpeedMoveAxis
        {
            get { return _translationSpeedMoveAxis; }
            set { SetProperty(ref _translationSpeedMoveAxis, value); }
        }

        private double _rotationSpeedMoveAxis;
        public double RotationSpeedMoveAxis
        {
            get { return _rotationSpeedMoveAxis; }
            set { SetProperty(ref _rotationSpeedMoveAxis, value); }
        }

        private int _ctrlMode;
        public int CtrlMode
        {
            get { return _ctrlMode; }
            set { SetProperty(ref _ctrlMode, value); }
        }

        private int _ctrlState;
        public int CtrlState
        {
            get { return _ctrlState; }
            set { SetProperty(ref _ctrlState, value); }
        }
        private int _demoState;
        public int DemoState
        {
            get { return _demoState; }
            set { SetProperty(ref _demoState, value); }
        }

        public FakeViewModel()
        {
            int i = 0;

            _jogAxisControlWord = 0;

            _rotationSpeedMoveAxis = 0;
            _translationSpeedMoveAxis = 0;

            _ctrlMode = 1;
            _ctrlState = 1;
            _demoState = 1;

            _axisList = new AxisList();
            _axisList.A0.Name = "A";
            _axisList.A0.MaxPos = 721.12;
            _axisList.A1.Name = "B";
            _axisList.A1.MaxPos = 471.84;
            _axisList.A2.Name = "C";
            _axisList.A2.MaxPos = 621.93;


            _inputs = new ObservableCollection<SetupItem>();
            for (i = 0; i < 10; i++) { 
                _inputs.Add(new SetupItem()
                {
                    Name = "I " + i.ToString(),
                    Description = "I " + i.ToString(),
                    MinValue = 0,
                    MaxValue = 100,
                    TypeValue = typeof(bool),
                    UnitMeasure = "",
                    IsReadOnly = true,
                    IsVisible = true,
                    WriteToPlc = false,
                    Format = "",
                    Category = 1,
                    Value = (i % 2).ToString(),                         // alla fine perchè viene validato con min e max
                    Criteria = "",
                    Default = "0"
                });

            }

            _outputs = new ObservableCollection<SetupItem>();
            for (i = 0; i < 10; i++) {
                _outputs.Add(new SetupItem()
                {
                    Name = "O " + i.ToString(),
                    Description = "O " + i.ToString(),
                    MinValue = 0,
                    MaxValue = 100,
                    TypeValue = typeof(bool),
                    UnitMeasure = "",
                    IsReadOnly = true,
                    IsVisible = true,
                    WriteToPlc = false,
                    Format = "",
                    Category = 1,
                    Value = (i % 2).ToString(),                         // alla fine perchè viene validato con min e max
                    Criteria = "",
                    Default = "0"
                });

            }
        }
    }
}
