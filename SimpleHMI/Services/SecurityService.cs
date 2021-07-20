using DBModel;
using Prism.Mvvm;
using SimpleHMI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleHMI.Services
{
    public enum EnumPermission : short
    {
        ADMIN = 1,  //Administrator
        EDITOR = 2, //Editor
        USER = 99 //normal user
    }
    /// <summary>
    /// Classe per la gestione dei livelli di accesso
    /// </summary>
    public class SecurityService : BindableBase {

        #region Attributes
        private readonly HMIEntities _entities;
        #endregion

        #region Constructors
        public SecurityService(HMIEntities entities) {
            _entities = entities;
            _currentUser = new User(); 
        }
        #endregion

        #region Properties
        private User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set { SetProperty(ref _currentUser, value); }
        }

        private DateTime _loginTime;
        public DateTime LoginTime
        {
            get { return _loginTime; }
            set { SetProperty(ref _loginTime, value); }
        }

        private TimeSpan _sessionTime;
        public TimeSpan SessionTime
        {
            get { return _sessionTime; }
            set { SetProperty(ref _sessionTime, value); }
        }

        public int Level {
            get { return _currentUser.Level;  }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Logs in
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>True if logged in</returns>
        public bool Login(string userName, string password) {
            bool retValue = false;
            // prova a collegarsi
            var user = _entities.Users.FirstOrDefault(x => x.Name == userName && x.Password == password);
            if (user != null) {

                _currentUser.Name = user.Name;
                _currentUser.Password = user.Password;
                _currentUser.Level = user.Level;
                _currentUser.AccessMask = (uint)user.Mask;

                LoginTime = DateTime.Now;

                retValue = true;
            }
            return retValue;
        }

        /// <summary>
        /// Logs out, removes the information about the user
        /// </summary>
        public void Logout() {
            CurrentUser.Reset();
        }

        #endregion

    }
}
