using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Cài đặt tốc độ")]
    public float moveSpeed = 5f;
    [Header("Cài đặt nhảy")]
    public float jumpForce = 7f;

    [Header("Danh sách Vũ khí")]
    public GameObject[] weapons;

    private Rigidbody2D rb;
    private bool isGrounded = true;

    private int currentWeaponIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateWeaponDisplay();
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false; 
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchWeapon(); 
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }
    private void UpdateWeaponDisplay()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == currentWeaponIndex)
                weapons[i].SetActive(true);
            else
                weapons[i].SetActive(false);
        }
    }
    private void SwitchWeapon()
    {
        weapons[currentWeaponIndex].SetActive(false);

        currentWeaponIndex++;

        if (currentWeaponIndex >= weapons.Length)
        {
            currentWeaponIndex = 0; 
        }

        weapons[currentWeaponIndex].SetActive(true);

        Debug.Log("Đang dùng vũ khí số: " + currentWeaponIndex);
    }
}
