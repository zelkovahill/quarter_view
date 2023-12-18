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

    private Vector3 moveVec;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // if(wDown)
        // {
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        // }
        // else
        // {
        //     transform.position += moveVec * speed * Time.deltaTime;
        // }

        anim.SetBool("isRun",moveVec != Vector3.zero);
        anim.SetBool("isWalk",wDown);

        transform.LookAt(transform.position + moveVec);

    }
}
