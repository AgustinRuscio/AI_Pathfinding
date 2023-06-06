using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    private States _currentState;

    private Dictionary<StatesEnum, States> _allStates = new Dictionary<StatesEnum, States>();

    public void AddState(StatesEnum key, States state)
    {
        if (_allStates.ContainsKey(key))
            _allStates[key] = state;

        _allStates.Add(key, state);

        if (_currentState == null)
            ChangeState(key);

        state.finiteStateMach = this;
    }
    public void ChangeState(StatesEnum state, params object[] parameters)
    {
        if (!_allStates.ContainsKey(state)) return;

        if (_currentState != null) _currentState.OnStop();
        _currentState = _allStates[state];

        _currentState.OnStart();
    }

    public void Update() => _currentState.Update();
}