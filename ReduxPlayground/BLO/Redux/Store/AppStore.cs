using System;
using Redux;
using ReduxPlayground.BLO.Redux.Reducer;
using ReduxPlayground.BLO.Redux.State;

namespace ReduxPlayground.BLO.Redux.Store
{
    internal class AppStore
    {
        #region Singleton

        private readonly IStore<AppState> _store;

        private static readonly Lazy<AppStore> _instance = new Lazy<AppStore>(() => new AppStore());

        private AppStore()
        {
            _store = new Store<AppState>(AppReducer.Execute);
        }

        #endregion

        #region Methods

        public static IAction Dispatch(IAction action)
        {
            return _instance.Value._store.Dispatch(action);
        }

        public static AppState GetState()
        {
            return _instance.Value._store.GetState();
        }

        #endregion
    }
}
