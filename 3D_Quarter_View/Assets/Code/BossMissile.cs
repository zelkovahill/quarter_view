using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BossMissile : Bullet
{
    public Transform target;
    private NavMeshAgent nav;
    
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

   
    void Update()
    {
        nav.SetDestination(target.position);
    }
}
