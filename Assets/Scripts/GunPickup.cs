using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] GunStats gun;
    bool triggerSet;
    
    // Start is called before the first frame update
    void Start()
    {
        gun.ammoCurr = gun.ammoMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerSet = true;
            //Give Stats to Player
            GManager.instance.playerScript.GetGunStats(gun);
            Destroy(gameObject);
        }
    }
}
