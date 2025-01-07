using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    bool hasHit = false;

    private float lifeTime;
    private float maxLifeTime = 5.0f;

    private float angle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!hasHit)
        {
            angle = Mathf.Atan2(rb.linearVelocityY, rb.linearVelocityX) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (hasHit)
        {
            lifeTime += Time.deltaTime;
            if (lifeTime >= maxLifeTime)
            {
                DestroyArrow();
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (collision.gameObject.GetComponent<PlayerController>())
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage();
            DestroyArrow();
        }

        hasHit = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;
    }

    public void DestroyArrow()
    {
        this.GetComponent<NetworkObject>().Despawn(true);
        Destroy(this.gameObject);
    }
}
