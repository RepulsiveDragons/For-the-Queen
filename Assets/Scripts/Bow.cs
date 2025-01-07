using Unity.Netcode;
using UnityEngine;

public class Bow : NetworkBehaviour
{
    private Vector2 bowPosition;
    private Vector2 mousePosition;
    private Vector2 directionVector;

    public GameObject arrow;
    public Transform shotPoint;
    [SerializeField]
    private float chargeSpeed;
    private bool isCharging = false;
    private bool _lockBow = false;
    public bool lockBow { get { return _lockBow; } set { _lockBow = value; } }

    public GameObject dot;
    private GameObject[] dots;
    [SerializeField]
    public int numberOfDots;
    [SerializeField]
    public float dotsSpacing;

    private float launchForce = 10f;
    [SerializeField]
    public float maxLaunchForce = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dot, new Vector3(100000f,100000f,100000f), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        bowPosition = transform.position;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directionVector = mousePosition - bowPosition; 
        transform.right = directionVector;

        if (lockBow) return;

        if (Input.GetMouseButton(0))
        {            
            isCharging = true;

            if (launchForce < maxLaunchForce)
            {
                launchForce += chargeSpeed * Time.deltaTime;
            }

            for (int i = 0; i < numberOfDots; i++)
            {
                dots[i].transform.position = DotPosition(i * dotsSpacing);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ShootArrowServerRpc();
            launchForce = 5f;

            for (int i = 0; i < numberOfDots; i++)
            {
                dots[i].transform.position = new Vector2(-100000f,-1000000f);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void ShootArrowServerRpc()
    {
        GameObject newArrow = Instantiate(arrow, shotPoint.position, Quaternion.identity);
        newArrow.GetComponent<Rigidbody2D>().linearVelocity = transform.right * launchForce;
        //newArrow.GetComponent<Rigidbody2D>().AddForce(transform.right * launchForce, ForceMode2D.Impulse);
        newArrow.GetComponent<NetworkObject>().Spawn(true);
    }

    private Vector2 DotPosition(float t)
    {
        Vector2 Position = (Vector2)shotPoint.position + (directionVector.normalized * launchForce * t) + (0.5f * Physics2D.gravity * (t * t));
        return Position;
    }

    public void ReverseFlip()
    {
        this.transform.localScale *= new Vector2(-1, 1);
    }
}
