using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;
    public Camera followCamera;

    public int ammo;
    public int coin;
    public int health;
  
    
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    
    private float hAxis;
    private float vAxis;
    
    
    private bool wDown;
    private bool jDown;
    private bool fDown;
    private bool gDown;
    private bool rDown;
    private bool iDown;
    private bool sDown1;
    private bool sDown2;
    private bool sDown3;

    
    private bool isJump;
    private bool isDodge;
    private bool isSwap;
    private bool isReload;
    private bool isFireReady=true;
    private bool isBorder;
    private bool isDamage;
    

    private Vector3 moveVec;
    private Vector3 DodgeVec;

    private Rigidbody rb;
    private Animator anim;
    MeshRenderer[] meshs;

    private GameObject nearObject;
    private Weapon equipWeapon;
    private int equipWeaponIndex = -1;
    private float fireDelay;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        meshs= GetComponentsInChildren<MeshRenderer>();
    }


    private void Update()
    {
       GetInput();
       Move();
       Turn();
       Jump();
       Grenade();
       Attack();
       Reload();
       Dodge();
       Interation();
       Swap();
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
    
    void FreezeRotation()
    {
        rb.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position,transform.forward,5,LayerMask.GetMask("Wall"));
    }

    private void GetInput()
   {
       hAxis = Input.GetAxisRaw("Horizontal");
       vAxis = Input.GetAxisRaw("Vertical");
       wDown = Input.GetButton("Walk");
       jDown = Input.GetButtonDown("Jump");
       fDown = Input.GetButton("Fire1");
       gDown = Input.GetButtonDown("Fire2");
       rDown = Input.GetButtonDown("Reload");
       iDown = Input.GetButtonDown("Interation");
       sDown1 = Input.GetButtonDown("Swap1");
       sDown2 = Input.GetButtonDown("Swap2");
       sDown3 = Input.GetButtonDown("Swap3");
       
   }

   private void Move()
   {
       moveVec = new Vector3(hAxis, 0, vAxis).normalized;
       if (isDodge)
       {
           moveVec = DodgeVec;
       }

       if (isSwap || isFireReady!=true || isReload)
       {
           moveVec = Vector3.zero;
       }

       if (isBorder != true)
       {
           transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
       }

       anim.SetBool("isRun",moveVec != Vector3.zero);
       anim.SetBool("isWalk",wDown);
   }

   private void Turn()
   {
       //#1. 키보드에 의한 회전
       transform.LookAt(transform.position + moveVec);
       
       //#2. 마우스에 의한 회전
       if(fDown)
       {
           Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
           RaycastHit rayHit;
           if (Physics.Raycast(ray, out rayHit, 100))
           {
               Vector3 nextVec = rayHit.point -transform.position;
               nextVec.y = 0;
               transform.LookAt(transform.position + nextVec);
           }
       }
      
   }

   private void Jump()
   {
       if (jDown && moveVec == Vector3.zero && isJump != true && isDodge != true && isSwap != true) 
       {
           rb.AddForce(Vector3.up * 15, ForceMode.Impulse);
           isJump = true;
           anim.SetBool("isJump",true);
           anim.SetTrigger("doJump");
       }
   }

   private void Grenade()
   {
       if(hasGrenades==0)
           return;

       if (gDown && isReload != true && isSwap != true)
       {
           Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
           RaycastHit rayHit;
           if (Physics.Raycast(ray, out rayHit, 100))
           {
               Vector3 nextVec = rayHit.point -transform.position;
               nextVec.y = 10;
               
                GameObject instantGrenade = Instantiate(grenadeObj,transform.position,transform.rotation);
                Rigidbody rbGrenade = instantGrenade.GetComponent<Rigidbody>();
                rbGrenade.AddForce(nextVec,ForceMode.Impulse);
                rbGrenade.AddTorque(Vector3.back * 10 , ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
           }
       }
   }

   private void Attack()
   {
       if (equipWeapon == null)
       {
           return;
       }

       fireDelay += Time.deltaTime;
       isFireReady = equipWeapon.rate < fireDelay;

       if (fDown && isFireReady && isDodge != true && isSwap != true) 
       {
           equipWeapon.Use();
           anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
           fireDelay = 0;
       }
   }

   private void Reload()
   {
       if(equipWeapon == null)
           return;
       
       if(equipWeapon.type==Weapon.Type.Melee)
           return;
       
       if(ammo == 0)
           return;
       
       if(rDown && isJump!=true && isDodge != true && isSwap != true && isFireReady)
       {
           anim.SetTrigger("doReload");
           isReload = true;
           Invoke(nameof(ReloadOut),3f);
       }
   }

   private void ReloadOut()
   {
       int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
       equipWeapon.curAmmo = reAmmo;
       ammo -= reAmmo;
       isReload = false;
   }
   
   private void Dodge()
   {
       if (jDown && moveVec != Vector3.zero && isJump != true && isDodge != true && isSwap != true) 
       {
           DodgeVec = moveVec;
           speed *= 2;
           anim.SetTrigger("doDodge");
           isDodge = true;

           Invoke(nameof(DodgeOut),0.5f);
       }
   }

   private void DodgeOut()
   {
       speed *= 0.5f;
       isDodge = false;
   }

   private void Swap()
   {
       if (sDown1 && (hasWeapons[0] != true || equipWeaponIndex == 0))
       {
           return;
       }
       if (sDown2 && (hasWeapons[1] != true || equipWeaponIndex == 1))
       {
           return;
       }
       if (sDown3 && (hasWeapons[2] != true || equipWeaponIndex == 2))
       {
           return;
       }
           
       
       int weaponIndex = -1;
       if (sDown1) weaponIndex = 0;
       if (sDown2) weaponIndex = 1;
       if (sDown3) weaponIndex = 2;
       
       if ((sDown1 || sDown2 || sDown3) && isJump != true && isDodge != true)
       {
           if (equipWeapon != null)
           {
               equipWeapon.gameObject.SetActive(false);
           }

           equipWeaponIndex = weaponIndex;
           equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
           equipWeapon.gameObject.SetActive(true);
           
           anim.SetTrigger("doSwap");

           isSwap = true;
           
           Invoke(nameof(SwapOut),0.4f);
       }
   }
   
   private void SwapOut()
   {
       isSwap = false;
   }

   private void Interation()
   {
       if (iDown && nearObject != null && isJump != true && isDodge != true)
       {
           if (nearObject.tag == "Weapon")
           {
               Item item = nearObject.GetComponent<Item>();
               int weaponIndex = item.value;
               hasWeapons[weaponIndex] = true;

               Destroy(nearObject);
           }
       }
   }

   private void OnCollisionEnter(Collision other)
   {
       if (other.gameObject.CompareTag("Floor"))
       {
           anim.SetBool("isJump",false);
           isJump = false;
       }
   }

   private void OnTriggerEnter(Collider other)
   {
       if (other.CompareTag("Item"))
       {
           Item item = other.GetComponent<Item>();
           switch (item.type)
           {
               case Item.Type.Ammo:
                   ammo+=item.value;
                   if(ammo>maxAmmo)
                       ammo=maxAmmo;
                   break;
               
               case Item.Type.Coin:
                     coin+=item.value;
                     if(coin>maxCoin)
                         coin=maxCoin;
                   break;
               
               case Item.Type.Heart:
                     health+=item.value;
                        if(health>maxHealth)
                            health=maxHealth;
                   break;
               
               case Item.Type.Grenade:
                   grenades[hasGrenades].SetActive(true);
                   hasGrenades+=item.value;
                   if(hasGrenades>maxHasGrenades) 
                       hasGrenades=maxHasGrenades;
                   break;
           }
           Destroy(other.gameObject);
       }
       
       else if (other.CompareTag("EnemyBullet"))
       {
           if (!isDamage)
           {
               Bullet enemyBullet = other.GetComponent<Bullet>();
               health -= enemyBullet.damage;
               
               if(other.GetComponent<Rigidbody>()!=null)
                   Destroy(other.gameObject);
               
               StartCoroutine(OnDanage());
           }
       }
       
   }

   IEnumerator OnDanage()
   {
       isDamage= true;
       foreach(MeshRenderer mesh in meshs)
       {
           mesh.material.color = Color.yellow;
       }
       
       yield return new WaitForSeconds(1f);
       isDamage = false;
       foreach(MeshRenderer mesh in meshs)
       {
           mesh.material.color = Color.white;
       }

   }

   private void OnTriggerStay(Collider other)
   {
       if (other.CompareTag("Weapon"))
       {
           nearObject = other.gameObject;
           
           print(nearObject.name);
       }
   }

   private void OnTriggerExit(Collider other)
   {
       if (other.CompareTag("Weapon"))
       {
           nearObject = null;
       }
   }
   
}
