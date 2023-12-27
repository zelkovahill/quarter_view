using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
  public int damage;
  public bool isMelee;
  private void OnCollisionEnter(Collision other)
  {
    if (other.gameObject.tag == "Floor")
    {
      Destroy(gameObject,3);
    }
    
  }

  private void OnTriggerEnter(Collider other)
  {
    if (isMelee && other.gameObject.CompareTag("Wall"))
    {
      Destroy(gameObject);
    }
  }
}
