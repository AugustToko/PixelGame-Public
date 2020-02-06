using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QInventory;

[RequireComponent(typeof(Collider2D))]
public class EnemyInfo : MonoBehaviour {
    public float health = 100f;
    [SerializeField]
    private GameObject deathParticle = null;
    private DropItem dropItem;

    private void Start()
    {
        dropItem = GetComponent<DropItem>();
    }

    private void Update()
    {
        if(health <= 0)
        {
            Death();
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Weapon")
        {
            Attacked();
        }
    }

    public void Attacked()
    {
        health -= InventoryManager.GetPlayerAttributeMaxValue("Damage");
        Debug.Log("Enemy Health: " + health);
    }

    void Death()
    {
        GameObject _deathParticle = Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(_deathParticle, 2f);

        ChestDrop chestDrop = GetComponent<ChestDrop>();
        if(chestDrop)
        {
            chestDrop.DropChest();
        }
    }
}
