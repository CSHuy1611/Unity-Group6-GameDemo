using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Đã tiêu diệt quái!");

            GameManager.Instance.AddKill();
            Destroy(gameObject);
        }
    }
}
