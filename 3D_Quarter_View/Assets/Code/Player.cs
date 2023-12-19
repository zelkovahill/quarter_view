using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private float hAxis;
    private float vAxis;
    private bool wDown;
    private bool jDown;

    private bool isJump;
    private bool isDodge;

    private Vector3 moveVec;
    private Vector3 DodgeVec;

    private Rigidbody rb;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
       GetInput();
       Move();
       Turn();
       Jump();
       Dodge();
    }
    
   private void GetInput()
   {
       hAxis = Input.GetAxisRaw("Horizontal");
       vAxis = Input.GetAxisRaw("Vertical");
       wDown = Input.GetButton("Walk");
       jDown = Input.GetButtonDown("Jump");
   }

   private void Move()
   {
       moveVec = new Vector3(hAxis, 0, vAxis).normalized;
       if (isDodge)
       {
           moveVec = DodgeVec;
       }
       transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
       anim.SetBool("isRun",moveVec != Vector3.zero);
       anim.SetBool("isWalk",wDown);
   }

   private void Turn()
   {
       transform.LookAt(transform.position + moveVec);
   }

   private void Jump()
   {
       if(jDown&&moveVec==Vector3.zero&&isJump!=true&&isDodge!=true)
       {
           rb.AddForce(Vector3.up * 15, ForceMode.Impulse);
           isJump = true;
           anim.SetBool("isJump",true);
           anim.SetTrigger("doJump");
       }
   }
   
   private void Dodge()
   {
       if(jDown&&moveVec!=Vector3.zero&&isJump!=true)
       {
           DodgeVec = moveVec;
           speed *= 2;
           anim.SetTrigger("doDodge");
           isDodge = true;

           Invoke(nameof(DodgeOut),0.5f);
       }
   }

   private void DodgeOut()
   {
       speed *= 0.5f;
       isDodge = false;
   }

   private void OnCollisionEnter(Collision other)
   {
       if (other.gameObject.CompareTag("Floor"))
       {
           anim.SetBool("isJump",false);
           isJump = false;
       }
   }
   
   
}
