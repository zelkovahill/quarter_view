using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
   public int maxHealth;
   public int currentHealth;
   public Transform Target;
   public bool isChase;
   
   private Rigidbody rb;
   BoxCollider boxCollider;
   Material material;
   NavMeshAgent nav;
   private Animator anim;
   
   
 private void Awake()
   {
      rb = GetComponent<Rigidbody>();
      boxCollider = GetComponent<BoxCollider>();
      material = GetComponentInChildren<MeshRenderer>().material;
      nav = GetComponent<NavMeshAgent>();
      anim = GetComponentInChildren<Animator>();
      
      Invoke(nameof(ChaseStart),2f);
   }
 
 void ChaseStart()
   {
      isChase = true;
      anim.SetBool("isWalk",true);
   }
 
   private void Update()
   {
      if(isChase)
         nav.SetDestination(Target.position);
   }

   private void FixedUpdate()
   {
      FreezeVelocity();
      
   }
   
   void FreezeVelocity()
   {
      if (isChase)
      {
         rb.velocity = Vector3.zero;
         rb.angularVelocity = Vector3.zero;
      }

   }

  

   private void OnTriggerEnter(Collider other)
   {
      if(other.gameObject.CompareTag("Melee"))
      {
         Weapon weapon = other.GetComponent<Weapon>();
         currentHealth-= weapon.damage;
         Vector3 reactVec = transform.position - other.transform.position;
         StartCoroutine(OnDamage(reactVec,false));
         print(currentHealth);
      }
      else if(other.gameObject.CompareTag("Bullet"))
      {
         Bullet bullet = other.GetComponent<Bullet>();
         currentHealth-= bullet.damage;
         Vector3 reactVec = transform.position - other.transform.position;
         Destroy(other.gameObject);
         StartCoroutine(OnDamage(reactVec,false));
         print(currentHealth);
      }
   }
   
   public void HitByGrenade(Vector3 explosionPos)
   {
      Vector3 reactVec = transform.position - explosionPos;
      currentHealth-= 100;
      StartCoroutine(OnDamage(reactVec,true));
   }

   IEnumerator OnDamage(Vector3 reactVec,bool isGrenade)
   {
      material.color = Color.red;
      yield return new WaitForSeconds(0.1f);

      if (currentHealth > 0)
      {
         material.color = Color.white;
      }

      else
      {
         material.color = Color.gray;
         gameObject.layer = 14;
         isChase=false;
         nav.enabled=false;
         anim.SetTrigger("doDie");

         if (isGrenade)
         {
            reactVec = reactVec.normalized;
            reactVec+= Vector3.up*3;
            
            rb.freezeRotation = false;
            rb.AddForce(reactVec*5,ForceMode.Impulse);
            rb.AddTorque(reactVec *15,ForceMode.Impulse);
         }
         else
         {
            reactVec = reactVec.normalized;
            reactVec+= Vector3.up;
            rb.AddForce(reactVec*5,ForceMode.Impulse);
         }
        
         Destroy(gameObject,4);
      }
      
   }
   
}
