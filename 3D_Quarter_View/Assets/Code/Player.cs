using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;

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
    private bool iDown;
    private bool sDown1;
    private bool sDown2;
    private bool sDown3;

    
    private bool isJump;
    private bool isDodge;
    private bool isSwap;
    

    private Vector3 moveVec;
    private Vector3 DodgeVec;

    private Rigidbody rb;
    private Animator anim;

    private GameObject nearObject;
    private GameObject equipWeapon;
    private int equipWeaponIndex = -1;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
       GetInput();
       Move();
       Turn();
       Jump();
       Dodge();
       Interation();
       Swap();
    }
    
   private void GetInput()
   {
       hAxis = Input.GetAxisRaw("Horizontal");
       vAxis = Input.GetAxisRaw("Vertical");
       wDown = Input.GetButton("Walk");
       jDown = Input.GetButtonDown("Jump");
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

       if (isSwap)
       {
           moveVec = Vector3.zero;
       }
       
       transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
       anim.SetBool("isRun",moveVec != Vector3.zero);
       anim.SetBool("isWalk",wDown);
   }

   private void Turn()
   {
       transform.LookAt(transform.position + moveVec);
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
               equipWeapon.SetActive(false);
           }

           equipWeaponIndex = weaponIndex;
           equipWeapon = weapons[weaponIndex];
           equipWeapon.SetActive(true);
           
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
