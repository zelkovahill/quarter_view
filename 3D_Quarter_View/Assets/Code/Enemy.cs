using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
   public enum Type
   {
      A,
      B,
      C
   };
   public Type enemyType;
   public int maxHealth;
   public int currentHealth;
   public Transform Target;
   public BoxCollider meleeArea;
   public GameObject bullet;
   public bool isChase;
   public bool isAttack;
   
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
      if (nav.enabled)
      {
         nav.SetDestination(Target.position);
         nav.isStopped =!isChase;
      }
   }

   private void Targerting()
   {
      float targetRadius = 0f;
      float targetRange = 0f;

      switch (enemyType)
      {
         case Type.A:
            targetRadius = 1.5f;
            targetRange = 3f;
            break;
         
         case Type.B:
            targetRadius = 1.5f;
            targetRange = 3f;
            break;
         
         case Type.C:
            targetRadius = 0.5f;
            targetRange = 25f;
            break;
      }
      
      RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,targetRadius,transform.forward,targetRange,LayerMask.GetMask("Player"));
      
      if (rayHits.Length > 0 && !isAttack)
      {
         StartCoroutine(Attack());
      }
   }

   IEnumerator Attack()
   {
      isChase = false;
      isAttack = true;
      anim.SetBool("isAttack",true);

      switch (enemyType)
      {
         case Type.A:
            yield return new WaitForSeconds(0.2f);
            meleeArea.enabled = true;
      
            yield return new WaitForSeconds(1f);
            meleeArea.enabled = false;
      
            yield return new WaitForSeconds(1f);
            break;
         
         case Type.B:
            yield return new WaitForSeconds(0.1f);
            rb.AddForce(transform.forward * 20,ForceMode.Impulse);
            meleeArea.enabled = true;

            yield return new WaitForSeconds(0.5f);
            rb.velocity = Vector3.zero;
            meleeArea.enabled = false;

            yield return new WaitForSeconds(2f);
            break;
            
            case Type.C:
            yield return new WaitForSeconds(0.5f);
            GameObject instantBullet = Instantiate(bullet,transform.position,transform.rotation);
            Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = transform.forward * 20;
            
            yield return new WaitForSeconds(2f);
            break;
            
         
      }
      
      
      isChase = true;
      isAttack = false;
      anim.SetBool("isAttack",false);
      
   }
   private void FixedUpdate()
   {
      Targerting();
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
