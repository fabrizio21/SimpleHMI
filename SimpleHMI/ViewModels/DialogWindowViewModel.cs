using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleHMI.ViewModels
{

    public enum EnumDialogWindowMode { 
        None,
        Warning,
        Question,
        Success,
        Info
    }
    public class DialogWindowViewModel : BindableBase, IDialogAware
    {
        #region Attributes
        private string _message;
        private EnumDialogWindowMode _mode;
        private string _title;
        private string _button1Text;
        private string _button2Text;
        private string _button3Text;

        private DelegateCommand _btn1Cmd;
        private DelegateCommand _btn2Cmd;
        private DelegateCommand _btn3Cmd;
        #endregion

        #region "Commands"
        public DelegateCommand Button1Command {
            get => _btn1Cmd; 
            set => SetProperty(ref _btn1Cmd, value);
        }

        public DelegateCommand Button2Command
        {
            get => _btn2Cmd;
            set => SetProperty(ref _btn2Cmd, value); 
        }
        public DelegateCommand Button3Command
        {
            get => _btn3Cmd;
            set => SetProperty(ref _btn3Cmd, value);
        }
        #endregion

        #region Properties

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public EnumDialogWindowMode Mode {
            get { return _mode; }
            set { SetProperty(ref _mode, value);  }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Button1Text
        {
            get { return _button1Text; }
            set { SetProperty(ref _button1Text, value); }
        }

        public string Button2Text
        {
            get { return _button2Text; }
            set { SetProperty(ref _button2Text, value); }
        }

        public string Button3Text
        {
            get { return _button3Text; }
            set { SetProperty(ref _button3Text, value); }
        }
        #endregion

        #region "Cosntructors"
        public DialogWindowViewModel() {
            // creates the commands
            Button1Command = new DelegateCommand(() =>
            {
                var param = new DialogResult(ButtonResult.Yes, new DialogParameters("btn=button1"));
                RequestClose?.Invoke(param);
            });
            Button2Command = new DelegateCommand(() =>
            {
                var param = new DialogResult(ButtonResult.No, new DialogParameters("btn=button2"));
                RequestClose?.Invoke(param);
            });
            Button3Command = new DelegateCommand(() =>
            {
                var param = new DialogResult(ButtonResult.Cancel, new DialogParameters("btn=button3"));
                RequestClose?.Invoke(param);
            });
        }
        #endregion

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Opens the dialog
        /// </summary>
        /// <param name="parameters">Values like title, message and buttons caption</param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            string mode;
            Title = parameters.GetValue<string>("title");
            Message = parameters.GetValue<string>("message");
            Button1Text = parameters.GetValue<string>("button1Text");
            Button2Text = parameters.GetValue<string>("button2Text");
            Button3Text = parameters.GetValue<string>("button3Text");
            mode = parameters.GetValue<string>("mode");
            mode = mode.ToLower();
            switch (mode) {
                case "none":        Mode = EnumDialogWindowMode.None;       break;
                case "warning":     Mode = EnumDialogWindowMode.Warning;    break;
                case "question":    Mode = EnumDialogWindowMode.Question;   break;
                case "success":     Mode = EnumDialogWindowMode.Success;    break;
                case "info":        Mode = EnumDialogWindowMode.Info;       break;
            }
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult) {
            RequestClose?.Invoke(dialogResult);
        }

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
                result = ButtonResult.OK;
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }
    }
}
