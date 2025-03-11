using UnityEngine;

public class Eyes : MonoBehaviour
{
    [SerializeField] private LayerMask _targetMasks;
    [SerializeField] private float _rangeOfSight = 1f;
    private SphereCollider _eyesight;
    private const string TARGET_TAG = "Player";
    private GameObject _target;
    
    void Start()
    {
        SetColliderForSight();
    }

    private void SetColliderForSight()
    {
        _eyesight = gameObject.AddComponent<SphereCollider>();
        _eyesight.radius = _rangeOfSight;
        _eyesight.isTrigger = true;
    }

    void Update()
    {
        MoveEyesightOnOrientation();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision is CharacterController && (_targetMasks == (_targetMasks & (1 << collision.gameObject.layer))) && !_target)
        {
            _target = collision.gameObject;
            GetComponent<LockOnTest1>().SetTarget(_target);
            var comp = GetComponent<GoapPlanner>();
                if (comp) comp.SetTarget(_target);
        }
        
    }
      private void OnTriggerExit(Collider collision)
    {
        //if (collision is CharacterController && (_targetMasks == (_targetMasks & (1 << collision.gameObject.layer))) )
        //{
        //    _target = null;
        //    GetComponent<LockOnTest1>().SetTarget(null);
        //    var comp = GetComponent<GoapPlanner>();
        //        if (comp) comp.SetTarget(null);
        //}
        
    }

    public void Rotate(float angle)
    {
        //_eyesight.transform.Rotate(Vector3.forward, angle);
    }


    //------------------------------------------
    //HelperFunctions

    private void MoveEyesightOnOrientation()
    {
        float orientation = GetComponent<WalkAnimate>().GetOrientation();
        _eyesight.center = new Vector3(Mathf.Cos(orientation) * _rangeOfSight, Mathf.Sin(orientation) * _rangeOfSight, 0.0f);

    }


}
