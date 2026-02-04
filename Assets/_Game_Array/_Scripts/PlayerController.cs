using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Cài đặt chung")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Hệ thống Kỹ Năng")]
    public GameObject[] skillPrefabs;
    public Transform firePoint; 

    private int currentSkillIndex = 0; 


    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Debug.Log("Bắt đầu với kỹ năng: " + currentSkillIndex);
    }

    void Update()
    {

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput > 0) transform.localScale = new Vector3(1.2f, 1.2f, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1.2f, 1.3f, 1);

        if (anim != null) anim.SetFloat("Speed", Mathf.Abs(moveInput));

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            anim.SetBool("IsGrounded", false);
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeSkill();
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
            anim.SetBool("IsGrounded", true);
        }
    }


    void ChangeSkill()
    {

        currentSkillIndex++;


        if (currentSkillIndex >= skillPrefabs.Length)
        {
            currentSkillIndex = 0;
        }


        if (currentSkillIndex == 0) GetComponent<SpriteRenderer>().color = Color.white; 
        else GetComponent<SpriteRenderer>().color = Color.red;   

        Debug.Log("Đã đổi sang kỹ năng số: " + currentSkillIndex);
    }

    void Attack()
    {

        anim.SetTrigger("Attack");


        GameObject currentAttack = skillPrefabs[currentSkillIndex];

        if (currentAttack == null)
        {

            Debug.Log("Chém thường!");

        }
        else
        {

            Debug.Log("Phóng Kiếm Khí!");
            if (firePoint != null)
            {
                Quaternion bulletRotation;
                if (transform.localScale.x > 0)
                {
                    bulletRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    bulletRotation = Quaternion.Euler(0, 180, 0);
                }
                Instantiate(currentAttack, firePoint.position, bulletRotation);
            }
        }
    }
}