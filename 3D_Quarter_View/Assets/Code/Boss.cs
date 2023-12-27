using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook;
    
    private Vector3 lookVec;
    private Vector3 tauntVec;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();


        nav.isStopped = true;
        StartCoroutine(Think());
    }

    
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        print("생각중");
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 5);
        
        switch (ranAction)
        {
            case 0:
                StartCoroutine(MissileShot());
                break;
            case 1:
                // 미사일 발사 패턴
                StartCoroutine(MissileShot());
                print("미사일 발사 패턴");
                break;
            case 2:
                StartCoroutine(RockShot());
                break;
            case 3:
                StartCoroutine(RockShot());
                // 돌 굴러가는 패턴
                print("돌 굴러가는 패턴");
                break;
            case 4:
                StartCoroutine(Taunt());
                // 점프 공격 패턴
                print("점프 공격 패턴");
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile,missilePortA.position,missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;
        
        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile,missilePortB.position,missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;
        
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(Think());
    }
    
    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }
    
    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");
        
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;
        
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        
        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }
    
    
}
