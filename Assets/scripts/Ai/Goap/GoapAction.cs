using UnityEngine;

public class GoapAction : MonoBehaviour
{
    public float Cost = 1.0f;
    private WorldState _desiredWorldStateRef;
    private WorldState _satisfyingWorldStateRef;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsVallid()
    {

        return true;
    }

    public bool IsCompleted(WorldState currentWorldState)
    {
        return true;
    }

}
