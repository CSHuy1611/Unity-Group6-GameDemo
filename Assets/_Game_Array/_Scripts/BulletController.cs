using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Cài đặt Đạn")]
    public float speed = 30f; 
    public float lifeTime = 2f; 

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Bùm! Trúng Enemy rồi!");
            Destroy(gameObject);
            Destroy(other.gameObject);
            GameManager.Instance.AddKill();
        }

        else if (other.CompareTag("Ground") || other.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }
}
