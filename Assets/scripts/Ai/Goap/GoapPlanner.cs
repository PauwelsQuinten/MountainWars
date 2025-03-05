using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GoapPlanner : MonoBehaviour
{
    [SerializeField] private List<GoapAction>_allActionPrefabs ;
    [SerializeField] private List<GoapGoal>_allGoalPrefabs;
    private List<GoapAction>_allActions = new List<GoapAction>();
    private List<GoapGoal>_allGoals = new List<GoapGoal>();

    private WorldState _currentWorldState;
    private GoapGoal _activeGoal;
    private GoapAction _activeAction;
    private List<GoapAction> _actionPlan = new List<GoapAction>();
    private Dictionary<EWorldState, WorldStateValue> _comparedWorldState = new Dictionary<EWorldState, WorldStateValue>();

    void Start()
    {
        _currentWorldState = gameObject.AddComponent<WorldState>();
        _currentWorldState.SetActive(true);
        foreach(var action in _allActionPrefabs)
        {
            _allActions.Add(Instantiate(action, gameObject.transform));
        }
        foreach(var goal in _allGoalPrefabs)
        {
            _allGoals.Add(Instantiate(goal, gameObject.transform));
        }

    }

    void Update()
    {
        _currentWorldState.UpdateWorldState();

        if (_activeGoal == null || _actionPlan.Count == 0)
            _activeGoal = SelectCurrentGoal();

       
        if (_actionPlan.Count == 0 && !Plan(_activeGoal.DesiredWorldState))
            _activeGoal.SetInvallid();
        else
            ExecutePlan();

    }

    private bool Plan(WorldState desiredWorldState)
    {
        _comparedWorldState = _currentWorldState.CompareWorldState(desiredWorldState);

        foreach(KeyValuePair<EWorldState, WorldStateValue> desiredState in _comparedWorldState)
        {
            float lowestScore = 9000f;
            GoapAction cheapestAction = null;

            foreach( var action in _allActions)
            {
                if (action.IsVallid(_currentWorldState) && action.SatisfyingWorldState._worldStateValues.ContainsKey(desiredState.Key))
                {
                    float score = action.Cost + action.DesiredWorldState._worldStateValues.Count + action.DesiredWorldState._worldStateValues2.Count;
                    if (score < lowestScore && !_actionPlan.Contains(action))
                    {
                        lowestScore = score;
                        cheapestAction = action;
                    }

                    
                }
                else if (action.IsVallid(_currentWorldState) && action.SatisfyingWorldState._worldStateValues2.ContainsKey(desiredState.Key) &&
                    action.SatisfyingWorldState._worldStateValues2[desiredState.Key] == desiredState.Value)
                {
                    float score = action.Cost + action.DesiredWorldState._worldStateValues.Count + action.DesiredWorldState._worldStateValues2.Count;
                    if (score < lowestScore && !_actionPlan.Contains(action))
                    {
                        lowestScore = score;
                        cheapestAction = action;
                    }
                }
            }
            
            if (cheapestAction == null)
                return false;
            _actionPlan.Add(cheapestAction);
            Plan(cheapestAction.DesiredWorldState);
            _comparedWorldState = _currentWorldState.CompareWorldState(desiredWorldState);//reset here back to startvalue before recursion
        }
        return true;
    }

    private GoapGoal SelectCurrentGoal()
    {
        foreach(var goal in _allGoals)
        {
            if (goal.IsVallid(_currentWorldState))
                return goal;
        }
        return new GoapGoal();
    }

    private void ExecutePlan()
    {
        if (_activeAction)
            _activeAction.UpdateAction(_currentWorldState);
        else
        {
            _activeAction = _actionPlan[_actionPlan.Count - 1];
            _activeAction.StartAction();
        }

        if (_activeAction.IsCompleted(_currentWorldState, _comparedWorldState))
        {
            _actionPlan.RemoveAt(_actionPlan.Count - 1);
            _activeAction = null;
        }
    }

    //Update Worldstate from AIPerception components (eyes & hearing = Lockon test)
    public void SetTarget(GameObject target)
    {
        _currentWorldState.SetTargetValues(target);
    }

    public void UpdateSwingSpeed(float speed)
    {
        _currentWorldState.UpddateSwingSpeed(speed);
    }
}
