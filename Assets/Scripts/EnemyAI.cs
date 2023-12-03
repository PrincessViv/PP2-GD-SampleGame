using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header ("----- Components -----")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;
    
    [Header ("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int viewCone;
    [SerializeField] int targetFaceSpeed;

    [Header ("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDir;

    //[SerializeField] int speed; //NavMesh- Gives us Speed

    // Start is called before the first frame update
    void Start()
    {
        GManager.instance.UpdateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position = Vector3.MoveTowards(transform.position, 
        // player.transform.position, speed * Time.deltaTime);

        if (playerInRange && CanSeePlayer())
        {
            
        }  
    }

    bool CanSeePlayer()
    {
        playerDir = GManager.instance.player.transform.position 
        - headPos.position; //Gets Direction
        angleToPlayer = Vector3.Angle(playerDir, transform.forward); //Gets Angle

        Debug.DrawRay(headPos.position, playerDir);
        Debug.Log(angleToPlayer);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                navAgent.SetDestination(GManager.instance.player.transform.position);

                if (!isShooting)
                {
                    StartCoroutine(Shoot());
                }

                if (navAgent.remainingDistance < navAgent.stoppingDistance)
                {
                    FaceTarget();
                }
                
                return true;   
            }
        }
        return false;
    }

    void FaceTarget() // or FaceTarget(Transform target)
    {
        Quaternion rot = Quaternion.LookRotation(playerDir); // or .LookRotation(target.position - headPos.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot,
        Time.deltaTime * targetFaceSpeed);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            GManager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
}
