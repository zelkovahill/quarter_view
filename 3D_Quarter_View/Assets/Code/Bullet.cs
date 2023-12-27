using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
  public int damage;
  public bool isMelee;
  public bool isRock;
  private void OnCollisionEnter(Collision other)
  {
    if (!isRock && other.gameObject.CompareTag("Floor"))
    {
      Destroy(gameObject,3);
    }
    
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!isMelee && other.gameObject.CompareTag("Wall"))
    {
      Destroy(gameObject);
    }
  }
}
