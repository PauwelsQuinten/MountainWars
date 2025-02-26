using UnityEngine;

public class LockOnTest1 : MonoBehaviour
{
    private SphereCollider _collider;
    private GameObject _lockonTarget;
    private WalkAnimate _walkAnimate;
    [SerializeField] private float _lockonRadius;
    [SerializeField] private LayerMask _layersToInteractWith;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _walkAnimate = GetComponent<WalkAnimate>();
        _collider = gameObject.AddComponent<SphereCollider>();
        _collider.radius = _lockonRadius;
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if ((_layersToInteractWith.value &(1<<collision.gameObject.layer)) != 0)
        {
            _lockonTarget = collision.gameObject;
            _walkAnimate.LockOn(collision.gameObject);
            //Debug.Log($"Enter {collision.gameObject.layer}");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (_lockonTarget == collision.gameObject)
        {
            _walkAnimate.LockOn(null);
        }
    }

}
