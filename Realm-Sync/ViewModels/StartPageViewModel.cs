using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Realm_Sync.Commands;
using Realm_Sync.Models;
using Realms;
using Realms.Sync;
using Realms.Sync.Exceptions;
using System;
using System.Net;

namespace Realm_Sync.ViewModels
{
    public class StartPageViewModel : ViewModelBase
    {
        public SyncCommand SyncCommand { get; set; }

        public CustomerModel Customer { get; set; }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");

            }
        }
        private string _title;
        public string Title
        {

            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        public Realm RealmContext { get; internal set; }

        public Realm LocalRealmContext { get; internal set; }

        public bool _waitingForLogin { get; private set; }

        public StartPageViewModel()
        {
            Customer = new CustomerModel();

            SyncCommand = new SyncCommand(this);
            LocalRealmInitialization();

            LoginToServerAsync();
        }

        internal async void LoginToServerAsync(User user = null)
        {


            _waitingForLogin = true;

            // assuming we are calling Login again after being logged in already, which has been guarded
            // to see that there IS a change in server/user, we need to logout
            try
            {
                foreach (var activeUser in User.AllLoggedIn)
                {
                    activeUser.LogOut();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected exception getting User.AllLoggedIn {e}");
            }

            //  var s = Settings;
            //// TODO allow entering Create User flag on credentials to pass in here instead of false
            Credentials credentials = user == null ? Credentials.UsernamePassword("realm-admin", "salsa.29", false) : null;
            try
            {
                if (user == null)
                {
                    user = await User.LoginAsync(credentials, new Uri("https://green-retail.us1.cloud.realm.io"));
                    Console.WriteLine($"Got user logged in with refresh token {user.RefreshToken}");
                }

                var loginConf = new SyncConfiguration(user, new Uri("realm://green-retail.us1.cloud.realm.io/~/Draw"));
                RealmContext = Realm.GetInstance(loginConf);
                //SetupPathChangeMonitoring();
            }
            catch (AuthenticationException)
            {
                Console.WriteLine( $"Unknown Username \nrealm-admin and Password \nsalsa.29 combination");
            }
            catch (System.Net.Sockets.SocketException sockEx)
            {
                Console.WriteLine($"Network error: {sockEx}");
            }
            catch (WebException webEx)
            {
                if (webEx.Status == System.Net.WebExceptionStatus.ConnectFailure)
                {
                    Console.WriteLine($"Unable to connect to https://green-retail.us1.cloud.realm.io - check address {webEx.Message}");
                }
                else
                {
                    Console.WriteLine($"Error trying to login to https://green-retail.us1.cloud.realm.io with realm-admin/salsa.29 {webEx.Message}");
                }
            }
            catch (Exception e)
            {
                if (user == null)
                {
                    Console.WriteLine($"Error trying to login to https://green-retail.us1.cloud.realm.io with realm-admin/salsa.29 {e.GetType().FullName} {e.Message}");
                }
                else
                {
                    Console.WriteLine($"Credentials accepted at https://green-retail.us1.cloud.realm.io but then failed to open Realm: {e.GetType().FullName} {e.Message}");
                }
            }

            

            _waitingForLogin = false;
        }

        public void LocalRealmInitialization()
        {
            var settingsConf = new RealmConfiguration("RealmSync.realm");
            settingsConf.ObjectClasses = new[] { typeof(CustomerModel) };
            settingsConf.SchemaVersion = 2;
            LocalRealmContext = Realm.GetInstance(settingsConf);
        }

    }
}