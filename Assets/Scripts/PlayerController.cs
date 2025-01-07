using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    Vector2 moveInput;
    public float walkSpeed = 1.5f;
    public float airSpeed = 1.0f;
    public float jumpImpulse = 10f;
    public bool _isDead = false;
    public bool isDead { get { return _isDead; } private set { _isDead = value; } }

    Rigidbody2D rb;
    TouchDirection touchDirection;
    SpriteRenderer spriteRenderer;

    [SerializeField] private TextMeshPro playerNameText;
    [SerializeField] private TextMeshPro healthText;
    
    private PauseMenuUI pauseMenuUI;

    private bool _isMoving = false;
    public bool isMoving { get { return _isMoving; } private set { _isMoving = value; animator.SetBool("isMoving", value); } }

    [SerializeField]
    private bool _isFacingRight = true;
    public bool IsFacingRight { get { return _isFacingRight; } 
        private set {
            if(_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
                bow.ReverseFlip();
                playerNameText.transform.localScale *= new Vector2(-1, 1); //reverse the flip of the UI elements on the player
                healthText.transform.localScale *= new Vector2(-1, 1);
                //spriteRenderer.flipX = !value;
            }

            _isFacingRight = value;
        } 
    }

    Animator animator;

    [SerializeField]
    public Bow bow;

    //Network Varibales
    private NetworkVariable<int> health = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        health.OnValueChanged += (int previousValue, int newValue) =>
        {
            SetHealth(health.Value);
            Debug.Log("Client ID:" + OwnerClientId + "; Health:" + health.Value);
        };

        playerNameText.text = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId).playerName.ToString();

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId)
        {
            //Destroy Newtork Objects here
            //-->Destroy(spawnObjectTransform.gameObject);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchDirection = GetComponent<TouchDirection>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenuUI = GameObject.Find("PauseMenuUI").GetComponent<PauseMenuUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.Value == 0)
        {
            isDead = true;
            bow.lockBow = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenuUI.Show();
        }
    }

    private float currentSpeed()
    {
        if (isMoving && !touchDirection.IsTouchingWall)
        {
            if(touchDirection.IsGrounded)   return walkSpeed;
            else return airSpeed;
        }
        else
        {
            return 0;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * currentSpeed(), rb.linearVelocityY);

        animator.SetFloat("yVelocity", rb.linearVelocityY);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner || isDead) return;

        moveInput = context.ReadValue<Vector2>();

        isMoving = moveInput != Vector2.zero;

        SetFacingDirection(moveInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner || isDead) return;

        if (context.started && touchDirection.IsGrounded)
        {
            rb.linearVelocityY = jumpImpulse;
            animator.SetTrigger("jump");
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if(moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }

        else if(moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void TakeDamage()
    {
        if (!isDead)
        {
            health.Value--;
            rb.linearVelocity = new Vector2(-50, rb.linearVelocityY + 10);
        }
    }

    public void SetHealth(int health)
    {
        healthText.text = "Health: " + health; 
    }
}
