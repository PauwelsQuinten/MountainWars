using UnityEngine;

public interface Goals
{
    bool IsVallid(WorldState currentWorldState);
    void SetInvallid();
    float GoalScore(CharacterMentality menatlity, WorldState currentWorldState);
}

public class GoapGoal : MonoBehaviour, Goals
{
    public WorldState DesiredWorldState;
    protected bool _isVallid = true;

    private void Start()
    {
        DesiredWorldState = GetComponent<WorldState>();    
    }

    public virtual bool IsVallid(WorldState currentWorldState)
    {

        return _isVallid; 
    }
    public void SetInvallid()
    {
        _isVallid = false;
    }

    public virtual float GoalScore(CharacterMentality menatlity, WorldState currentWorldState)
    {
        return 0.75f;
    }
}
