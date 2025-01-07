using UnityEngine;

public class TouchDirection : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;

    CapsuleCollider2D collider;

    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[10];
    RaycastHit2D[] wallHits = new RaycastHit2D[10];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[10];

    [SerializeField]
    private bool _isGrounded;
    public bool IsGrounded { get { return _isGrounded; } set {  _isGrounded = value; animator.SetBool("isGrounded", value); } }

    [SerializeField]
    private bool _isTouchingWall;
    public bool IsTouchingWall { get { return _isTouchingWall; } set { _isTouchingWall = value; } }

    [SerializeField]
    private bool _isTouchingCeiling;
    public bool IsTouchingCeiling { get { return _isTouchingCeiling; } set { _isTouchingCeiling = value; } }
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    private void Awake()
    {
        collider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        //cast returns how many 
        IsGrounded = collider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsTouchingWall = collider.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsTouchingCeiling = collider.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;

    }
}
