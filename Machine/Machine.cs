using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Interfaces;

namespace Machine
{
    public sealed class Machine<TState> where TState : ICloneable
    {
        #region Singleton

        private static readonly Lazy<Machine<TState>> _instance = new Lazy<Machine<TState>>(() => new Machine<TState>());

        private Machine() { }

        #endregion

        #region Provided Events

        public static event Action<TState> StateChanged;

        #endregion

        #region Fields

        private readonly object
            _dispatchLock = new object();

        private bool
            _initialized;

        private TState
            _state;

        private Dictionary<Type, Func<IAction, TState, TState>>
            _reducers;

        private Dictionary<Type, KeyValuePair<Func<TState, object>, Func<IAction, object, object>>>
            _selectorReducers;

        #endregion

        #region Methods

        public static Machine<TState> InitializeState(TState defaultState = default(TState))
        {
            if(_instance.Value._initialized)
                throw new InvalidOperationException(@"State already initialized!");

            _instance.Value._initialized = true;
            _instance.Value._state = defaultState;

            return _instance.Value;
        }

        public static TState GetState()
        {
            if(!_instance.Value._initialized)
                throw new InvalidOperationException(@"State is not initialized!");

            return _instance.Value._state;
        }

        private static void SetState(TState state)
        {
            lock (_instance.Value._dispatchLock)
            {
                _instance.Value._state = state;
            }

            StateChanged?.Invoke(state); 
        }

        public static void RegisterAction(Type actionType, Func<IAction, TState, TState> reducer)
        {
            if(actionType == null)
                throw new ArgumentNullException(nameof(actionType));

            if(reducer == null)
                throw new ArgumentNullException(nameof(reducer));

            if(!actionType.GetInterfaces().Contains(typeof(IAction)))
                throw new ArgumentException($"Value of parameter {nameof(actionType)} doesn't implement {nameof(IAction)}!");

            if(_instance.Value._reducers == null)
                _instance.Value._reducers = new Dictionary<Type, Func<IAction, TState, TState>>();

            if(_instance.Value._reducers.ContainsKey(actionType))
                throw new InvalidOperationException($"Reducer already registered for action type {actionType}!");

            _instance.Value._reducers.Add(actionType, reducer);
        }

        public static void RegisterAction<T>(Type actionType, Func<TState, T> selector, Func<IAction, T, T> reducer)
        {
            if (actionType == null)
                throw new ArgumentNullException(nameof(actionType));

            if(selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (reducer == null)
                throw new ArgumentNullException(nameof(reducer));

            if (!actionType.GetInterfaces().Contains(typeof(IAction)))
                throw new ArgumentException($"Value of parameter {nameof(actionType)} doesn't implement {nameof(IAction)}!");

            if (_instance.Value._selectorReducers == null)
                _instance.Value._selectorReducers = new Dictionary<Type, KeyValuePair<Func<TState, object>, Func<IAction, object, object>>>();

            if (_instance.Value._selectorReducers.ContainsKey(actionType))
                throw new InvalidOperationException($"Reducer already registered for action type {actionType}!");

            object SelectorObjectified(TState state) => selector(state);
            object ReducerObjectified(IAction action, object o) => reducer(action, (T) o);

            _instance.Value._selectorReducers.Add(actionType, new KeyValuePair<Func<TState, object>, Func<IAction, object, object>>(SelectorObjectified, ReducerObjectified));
        }

        public static void DeregisterAction(Type actionType)
        {
            if(actionType == null)
                throw new ArgumentNullException(nameof(actionType));

            if (_instance.Value._reducers?.ContainsKey(actionType) == true)
                _instance.Value._reducers.Remove(actionType);

            if (_instance.Value._selectorReducers?.ContainsKey(actionType) == true)
                _instance.Value._selectorReducers.Remove(actionType);
        }

        public static TState Dispatch(IAction action)
        {
            if(action == null)
                throw new ArgumentNullException(nameof(action));

            var actionType = action.GetType();

            if (_instance.Value._reducers?.ContainsKey(actionType) == true)
            {
                var reducer = _instance.Value._reducers[actionType];
                var alteredState = Dispatch(action, reducer);
                SetState(alteredState);
                return alteredState;
            }

            if (_instance.Value._selectorReducers?.ContainsKey(actionType) == true)
            {
                var selectorReducerPair = _instance.Value._selectorReducers[actionType];
                var alteredState = Dispatch(action, selectorReducerPair.Key, selectorReducerPair.Value);
                SetState(alteredState);
                return alteredState;
            }

            throw new InvalidOperationException($"Action of type {actionType} not registered!");
        }

        private static TState Dispatch(IAction action, Func<IAction, TState, TState> reducer)
        {
            var state = GetState();
            var clone = (TState) state.Clone();
            var altered = reducer(action, clone);

            return altered;
        }

        private static TState Dispatch<T>(IAction action, Func<TState, T> selector, Func<IAction, T, T> reducer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
