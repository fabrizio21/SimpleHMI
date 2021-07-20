using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace SimpleHMI.ViewModels
{
    public class LoginWindowViewModel : BindableBase, IDialogAware
    {
        #region Properties
        public string Title => string.Empty;
        private string _messageOk;
        public string MessageOk
        {
            get { return _messageOk; }
            set { SetProperty(ref _messageOk, value); }
        }

        private string _messageCancel;
        public string MessageCancel
        {
            get { return _messageCancel; }
            set { SetProperty(ref _messageCancel, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        #endregion

        #region Commands
        public ICommand LoginCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        #endregion

        #region Constructors
        public LoginWindowViewModel()
        {
            LoginCommand = new DelegateCommand(() =>
            {
                var parameters = new DialogParameters{
                    { "user", _userName },
                    { "pwd", _password }
                };
                var param = new DialogResult(ButtonResult.OK, parameters);
                RequestClose?.Invoke(param);
            });

            CancelCommand = new DelegateCommand(() =>
            {
                var param = new DialogResult(ButtonResult.Cancel, new DialogParameters());
                RequestClose?.Invoke(param);
            });
        }
        #endregion

        
        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            //throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters){
            MessageOk = parameters.GetValue<string>("Ok");
            MessageCancel = parameters.GetValue<string>("Cancel");
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
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
