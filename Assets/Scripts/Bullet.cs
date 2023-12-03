using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    
    [SerializeField] int damageAmount;
    [SerializeField] int destroyTime;
    [SerializeField] int speed;
    
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        
        if (other.isTrigger)
        {
            return;
        }
        
        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.TakeDamage(damageAmount);
        }

        Destroy(gameObject);
    }
}
