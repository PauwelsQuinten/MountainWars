using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Actions
{
    void UpdateAction(WorldState currentWorldState);
    bool IsVallid(WorldState currentWorldState);
    bool IsCompleted(WorldState current, WorldState activeActionDesiredState);
    void StartAction(WorldState currentWorldState);
    bool IsInterupted(WorldState currentWorldState);
}

public class GoapAction : MonoBehaviour, Actions
{
    public float Cost = 1.0f;
    public WorldState DesiredWorldState;
    public WorldState SatisfyingWorldState;
    [SerializeField] protected float _actionMaxRunTime = 3f;
    protected bool _isActivated = false;
    protected Coroutine _actionCoroutine;

    virtual protected void Start()
    {
        WorldState[] states = GetComponents<WorldState>();
        foreach (var item in states)
        {
            if (item._worldStateType == StateType.Desired)
                DesiredWorldState = item;
            else
                SatisfyingWorldState = item;
        }
    }
    public virtual void StartAction(WorldState currentWorldState)
    {
        if (_isActivated)
            return;
        _isActivated = true;
        _actionCoroutine = StartCoroutine(StartTimer(_actionMaxRunTime));
    }


    public virtual void UpdateAction(WorldState currentWorldState)
    {

    }

    virtual public bool IsVallid(WorldState currentWorldState)
    {
        return true;
    }

    public virtual bool IsInterupted(WorldState currentWorldState)
    {
        return false; 
    }


    virtual public bool IsCompleted(WorldState currentWorldState, WorldState activeActionDesiredState)
    {
        //set to complete if runtime runs out, done by coroutine started at startAction()
        //set to complete by UpdateAction()
        if (!_isActivated)
        {
            return true;
        }

        if (SatisfyingWorldState._worldStateValues.Count != 0)
        {
            foreach (KeyValuePair<EWorldState, float> updatingState in SatisfyingWorldState._worldStateValues)
            {
                if (/*_comparedWorldState.ContainsKey(updatingState.Key) && */Mathf.Abs(updatingState.Value - currentWorldState._worldStateValues[updatingState.Key]) >= 0.1f)
                    return false;
            }
        }
        if (SatisfyingWorldState._worldStateValues2.Count != 0)
        {
            foreach (KeyValuePair<EWorldState, WorldStateValue> updatingState in SatisfyingWorldState._worldStateValues2)
            {
                if (/*_comparedWorldState.ContainsKey(updatingState.Key) &&*/ updatingState.Value - currentWorldState._worldStateValues2[updatingState.Key] != 0)
                    return false;
            }
        }

        //Action finished
        StopCoroutine(_actionCoroutine);
        return true;
    }

    protected IEnumerator StartTimer(float runTime)
    {
        yield return new WaitForSeconds(runTime);
        Debug.Log("Courotine Has Run");
        _isActivated = false;
    }

    public void ActionCompleted()
    {
        _isActivated = false;
        StopCoroutine(_actionCoroutine);
    }
}
