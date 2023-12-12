using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] Collider weaponCol;
    [SerializeField] Collider damageCol;

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int viewCone;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] float animLerpSpeed;

    [Header("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;

    bool isShooting;
    bool playerInRange;
    bool destinationChosen;
    float angleToPlayer;
    Vector3 playerDir;
    Vector3 startingPos;
    float stoppingDistOrig;

    //[SerializeField] int speed; //NavMesh- Gives us Speed

    // Start is called before the first frame update
    void Start()
    {
        GManager.instance.UpdateGameGoal(1);
        startingPos = transform.position;
        stoppingDistOrig = navAgent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position = Vector3.MoveTowards(transform.position, 
        // player.transform.position, speed * Time.deltaTime);

        if (navAgent.isActiveAndEnabled)
        {
            float animSpeed = navAgent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed,
            Time.deltaTime * animLerpSpeed));

            if (playerInRange && !CanSeePlayer())
            {
                StartCoroutine(Roam());
            }
            else if (!playerInRange)
            {
                StartCoroutine(Roam());
            }
        }
    }

    IEnumerator Roam()
    {
        if (navAgent.remainingDistance < 0.05f && !destinationChosen)
        {
            destinationChosen = true;
            navAgent.stoppingDistance = 0; //Right to Position .
            yield return new WaitForSeconds(roamPauseTime);

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos; //Keeps Enemy from going too Far

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            navAgent.SetDestination(hit.position);

            destinationChosen = false;
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

                navAgent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }

        navAgent.stoppingDistance = 0;
        return false;
    }

    void FaceTarget() // or FaceTarget(Transform target)
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x,
        transform.position.y, playerDir.z));
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
            navAgent.stoppingDistance = 0;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void CreateBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void WeaponColOn()
    {
        weaponCol.enabled = true;
    }

    public void WeaponColOff()
    {
        weaponCol.enabled = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StopAllCoroutines();

        if (HP <= 0)
        {
            GManager.instance.UpdateGameGoal(-1);
            anim.SetBool("Dead", true);
            navAgent.enabled = false;
            damageCol.enabled = false;
        }

        else
        {
            isShooting = false;
            anim.SetTrigger("Damage");
            destinationChosen = false;
            StartCoroutine(FlashRed());

            //if (navAgent.isActiveAndEnabled)
            //{
            navAgent.SetDestination(GManager.instance.
            player.transform.position);
            //}
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
}
