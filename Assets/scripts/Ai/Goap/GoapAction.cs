using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Actions
{
    void UpdateAction(WorldState currentWorldState);
    bool IsVallid(WorldState currentWorldState);
    bool IsCompleted(WorldState currentWorldState, Dictionary<EWorldState, WorldStateValue> _comparedWorldState);
}

public class GoapAction : MonoBehaviour, Actions
{
    public float Cost = 1.0f;
    public WorldState DesiredWorldState;
    public WorldState SatisfyingWorldState;
    [SerializeField] float _actionMaxRunTime = 3f;
    private bool _isActivated = false;
    private Coroutine _actionCoroutine;

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

    public virtual void UpdateAction(WorldState currentWorldState)
    {

    }

    virtual public bool IsVallid(WorldState currentWorldState)
    {

        return true;
    }

    virtual public bool IsCompleted(WorldState currentWorldState, Dictionary<EWorldState, WorldStateValue> _comparedWorldState)
    {
        if (!_isActivated)
        {
            //set to complete if runtime runs out
            Debug.Log("action finished");
            return true;
        }


        if (SatisfyingWorldState._worldStateValues.Count != 0)
        {
            foreach (KeyValuePair<EWorldState, float> updatingState in SatisfyingWorldState._worldStateValues)
            {
                if (Mathf.Abs(updatingState.Value - currentWorldState._worldStateValues[updatingState.Key]) >= 0.1f)
                    return false;
            }
        }
        if (SatisfyingWorldState._worldStateValues2.Count != 0)
        {
            foreach (KeyValuePair<EWorldState, WorldStateValue> updatingState in SatisfyingWorldState._worldStateValues2)
            {
                if (updatingState.Value - currentWorldState._worldStateValues2[updatingState.Key] != 0)
                    return false;
            }
        }

        //Action finished
        StopCoroutine(_actionCoroutine);
        Debug.Log("action finished");
        return true;
    }

    public void StartAction()
    {
        if (_isActivated)
            return;
        _isActivated = true;
        _actionCoroutine = StartCoroutine(StartTimer(_actionMaxRunTime));
    }

    IEnumerator StartTimer(float runTime)
    {
        yield return new WaitForSeconds(runTime);
        _isActivated = false;
    }
}
