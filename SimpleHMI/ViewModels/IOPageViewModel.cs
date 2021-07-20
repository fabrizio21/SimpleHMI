using Prism.Commands;
using Prism.Mvvm;
using SimpleHMI.Models;
using SimpleHMI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Translator;

namespace SimpleHMI.ViewModels
{
    public class IOPageViewModel : BindableBase
    {
        PmacService _pmacService;
        ITranslationService _translationService;
        ObservableCollection<SetupItem> _inputs;
        ObservableCollection<SetupItem> _outputs;

        private SetupItem _selectedOutput;
        public SetupItem SelectedOutput
        {
            get { return _selectedOutput; }
            set {
                value.Value = value.Value == "0" ? "1": "0";
                SetProperty(ref _selectedOutput, value); 
                // qui mando il comando 
            }
        }

        public ObservableCollection<SetupItem> Inputs {
            get { return _inputs; }
        }

        public ObservableCollection<SetupItem> Outputs
        {
            get { return _outputs; }
        }

        public IOPageViewModel(PmacService pmacService,
                                ITranslationService translationManager)
        {
            int i = 0;
            _inputs = new ObservableCollection<SetupItem>();
            for (i = 0; i < 10; i++)
            {
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
            for (i = 0; i < 10; i++)
            {
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
