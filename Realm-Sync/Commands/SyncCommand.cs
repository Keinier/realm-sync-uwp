using GalaSoft.MvvmLight.Command;
using Realm_Sync.ViewModels;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Realm_Sync.Commands
{
    public class SyncCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected StartPageViewModel _viewmodel;

       

        public SyncCommand(StartPageViewModel vm)
        {
            _viewmodel = vm;
        }


        public bool CanExecute(object parameter)
        {
            return _viewmodel.RealmContext != null;
        }

        public void Execute(object parameter)
        {
            
            _viewmodel.RealmContext.Write(() => {

                _viewmodel.RealmContext.Add(_viewmodel.Customer);
            });

        }
    }
}
