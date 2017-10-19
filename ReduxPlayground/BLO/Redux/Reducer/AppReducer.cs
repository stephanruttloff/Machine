using System;
using System.Threading;
using Redux;
using ReduxPlayground.BLO.Redux.Actions;
using ReduxPlayground.BLO.Redux.State;

namespace ReduxPlayground.BLO.Redux.Reducer
{
    internal static class AppReducer
    {
        public static AppState Execute(AppState state, IAction action)
        {
            if (action is LongRunning)
            {
                Thread.Sleep(1000);
                return state;
            }

            throw new NotSupportedException();
        }
    }
}
