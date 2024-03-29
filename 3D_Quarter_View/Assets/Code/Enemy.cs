using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
   public enum Type
   {
      A,
      B,
      C,
      D
   }
   public Type enemyType;
   public int maxHealth;
   public int currentHealth;
   public int score;

   public GameManager manager;
   public Transform target;
   public BoxCollider meleeArea;
   public GameObject bullet;
   public GameObject[] coins;
   
   public bool isChase;
   public bool isAttack;
   public bool isDead;
   
   public Rigidbody rb;
   public BoxCollider boxCollider;
   public MeshRenderer[] material;
   public NavMeshAgent nav;
   public Animator anim;
   
   
 private void Awake()
   {
      rb = GetComponent<Rigidbody>();
      boxCollider = GetComponent<BoxCollider>();
      material = GetComponentsInChildren<MeshRenderer>();
      nav = GetComponent<NavMeshAgent>();
      anim = GetComponentInChildren<Animator>();
      
      if(enemyType!=Type.D)
         Invoke(nameof(ChaseStart),2f);
   }
 
 void ChaseStart()
   {
      isChase = true;
      anim.SetBool("isWalk",true);
   }
 
   private void Update()
   {
      if (enemyType!=Type.D && nav.enabled)
      {
         nav.SetDestination(target.position);
         nav.isStopped =!isChase;
      }
   }

   private void Targerting()
   {
      if (!isDead && enemyType != Type.D)
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

         RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange,
            LayerMask.GetMask("Player"));

         if (rayHits.Length > 0 && !isAttack)
         {
            StartCoroutine(Attack());
         }
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
      foreach (MeshRenderer mesh in material)
      {
         mesh.material.color = Color.red;
      }
     
      yield return new WaitForSeconds(0.1f);

      if (currentHealth > 0)
      {
         foreach (MeshRenderer mesh in material)
         {
            mesh.material.color = Color.white;
         }
         
      }

      else
      {
         foreach (MeshRenderer mesh in material)
         {
            mesh.material.color = Color.gray;
         }
         
         gameObject.layer = 14;
         isDead=true;
         isChase=false;
         nav.enabled=false;
         anim.SetTrigger("doDie");
         Player player = target.GetComponent<Player>();
         player.score += score;
         int ranCoin = Random.Range(0, 3);
         Instantiate(coins[ranCoin],transform.position + Vector3.up, Quaternion.identity);

         switch (enemyType)
         {
            case Type.A:
               manager.enemyCntA--;
               break;
            
            case Type.B:
               manager.enemyCntB--;
               break;
            
            case Type.C:
               manager.enemyCntC--;
               break;
            
            case Type.D:
               manager.enemyCntD--;
               break;
         }
        
         

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
