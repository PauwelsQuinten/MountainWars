using UnityEngine;

public interface Goals
{
    bool IsVallid(WorldState currentWorldState);
}

public class GoapGoal : MonoBehaviour, Goals
{
    public WorldState DesiredWorldState;

    private void Start()
    {
        DesiredWorldState = GetComponent<WorldState>();    
    }

    public virtual bool IsVallid(WorldState currentWorldState)
    {

        return true; 
    }

}
