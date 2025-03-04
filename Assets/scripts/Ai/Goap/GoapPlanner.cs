using UnityEngine;

public class GoapPlanner : MonoBehaviour
{
    private WorldState _worldState;

    void Start()
    {
        _worldState = gameObject.AddComponent<WorldState>();
    }

    // Update is called once per frame
    void Update()
    {
        _worldState.UpdateWorldState();
    }

    public void SetTarget(GameObject target)
    {
        _worldState.SetTargetValues(target);
    }

    public void UpdateSwingSpeed(float speed)
    {
        _worldState.UpddateSwingSpeed(speed);
    }
}
