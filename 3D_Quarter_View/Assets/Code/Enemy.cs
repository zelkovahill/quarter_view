using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   public int maxHealth;
   public int currentHealth;

   private Rigidbody rb;
   BoxCollider boxCollider;
   Material material;

   private void Awake()
   {
      rb = GetComponent<Rigidbody>();
      boxCollider = GetComponent<BoxCollider>();
      material = GetComponent<MeshRenderer>().material;
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
