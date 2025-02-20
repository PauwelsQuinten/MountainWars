using UnityEngine;
public class SwordSwing : MonoBehaviour
{
    [SerializeField] GameObject _sword;
    [SerializeField] float _swingSpeed = 25f;
    [SerializeField] float _swingAngle = 90.0f;
    private WalkAnimate _animationRef;

    private bool _isSwinging = false;
    private Vector2 _startSwingDirection;
    private Vector2 _defaultPosition;
    private float  _startAngle = 0.0f;
    private float _swingDirection = 0.0f;
    private float _defaultAngle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animationRef = GetComponent<WalkAnimate>();
        _defaultAngle = _sword.transform.rotation.eulerAngles.z;
        _defaultPosition = _sword.transform.position;   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_isSwinging)
            return;

        Swing();
    }

    public void StartSwing(AttackType attackType, AttackStance attackHeight, int startDirection)
    {
        _isSwinging = true;

        _swingDirection = -startDirection;

        float height = gameObject.transform.position.y + gameObject.transform.localScale.y * (int)attackHeight * 0.5f;
        _sword.transform.position = new Vector3(_sword.transform.position.x, height, 0.0f);

        float orientation = (_animationRef)? _animationRef.GetOrientation() : 0.0f;
        _sword.transform.Rotate(0.0f, 0.0f, startDirection * _swingAngle);

        _startAngle = _sword.transform.rotation.eulerAngles.z;
    }

    private void Swing()
    {
        _sword.transform.Rotate(0.0f, 0.0f, _swingDirection * _swingSpeed * Time.fixedDeltaTime);
        float angle = _sword.transform.rotation.eulerAngles.z;
        float diff = _startAngle + _swingDirection * angle;

        if (diff >= _swingAngle * 2)
        {
            SetIdle();
        }


        return;
        Debug.Log($"angle: {angle}");
        Debug.Log($"diff: {diff}");

    }

    private void SetIdle()
    {
        _isSwinging = false;
        _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, _defaultAngle);
        _sword.transform.position = _defaultPosition;

    }
}
