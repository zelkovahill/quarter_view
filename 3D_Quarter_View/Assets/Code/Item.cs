using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
   public enum Type
   {
      Ammo,
      Coin,
      Grenade,
      Heart,
      Weapon
   }

   public Type type;
   public int value;

   private Rigidbody rb;
   SphereCollider sphereCollider;

   private void Awake()
   {
      rb = GetComponent<Rigidbody>();
      sphereCollider = GetComponent<SphereCollider>();
   }

   private void Update()
   {
      transform.Rotate(Vector3.up * 20 * Time.deltaTime);
   }

   private void OnCollisionEnter(Collision other)
   {
      if (other.gameObject.CompareTag("Floor"))
      {
         rb.isKinematic = true;
         sphereCollider.enabled = false;
      }
   }
}
