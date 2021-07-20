using Prism.Mvvm;
using Prism.Regions;
using SimpleHMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator;

namespace SimpleHMI.ViewModels
{
    public class BaseViewModel : BindableBase, INavigationAware
    {
        IRegionManager _regionManager;
        private readonly PmacService _pmacService;
        private readonly ITranslationService _translationService;

        #region Navigation
        public virtual bool IsNavigationTarget(NavigationContext navigationContext) {
            throw new NotImplementedException();
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext) {
            throw new NotImplementedException();
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
