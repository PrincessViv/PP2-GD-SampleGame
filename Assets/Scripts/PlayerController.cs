using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header ("----- Components -----")]
    [SerializeField] CharacterController controller;
    
    [Header ("----- Player Stats -----")]
    [Range(1, 8)] [SerializeField] float playerSpeed;
    [Range(8, 30)] [SerializeField] float jumpHeight;
    [Range(-10, 40)] [SerializeField] float gravityValue;
    [Range(1.5f, 3)] [SerializeField] float sprintMod;
    [Range(1, 3)] [SerializeField] int jumpMax;
    [Range(1, 10)] [SerializeField] int HP;

        [Header ("----- Weapon Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] GameObject cube;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private int jumpCount;
    private bool isShooting;
    int HPOrig;

    private void Start()
    {
      HPOrig = HP;
      RespawnPlayer();
    }

    void Update()
    {
        Movement();
    }

    public void RespawnPlayer()
    {
        HP = HPOrig;
        UpdatePlayerUI();

        controller.enabled = false; //This prevents playerController from overriding
        transform.position = GManager.instance.playerSpawnPOS.transform.position;
        controller.enabled = true;
    }

    void Movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward *
        shootDist, Color.red);

        Sprint();

        if (!GManager.instance.isPaused && 
        Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(Shoot());
        }

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            //Prevents from going Neg
            jumpCount = 0;
        }

        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = Input.GetAxis("Horizontal") * transform.right +    //Only moves L/R
        Input.GetAxis("Vertical") * transform.forward;    //Only moves Forward/Back
     
        controller.Move(move * Time.deltaTime * playerSpeed);
        //deltaTime helps keep time the same

        // if (move != Vector3.zero)
        // {
        //     GameObject.transform.forward = move;
        //Allows player to rotate
        // }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            //playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            playerVelocity.y = jumpHeight;
            jumpCount++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new 
        Vector2(0.5f, 0.5f)), out hit, shootDist))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
          
            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
       
        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }

    void Sprint()
    {
        //If button is pressed, Player sprints
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
        }

        else if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed /= sprintMod;
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        UpdatePlayerUI();

        if (HP <= 0)
        {
            //Game over! :-(
            GManager.instance.YouLose();
        }
    }

    public void UpdatePlayerUI()
    {
         GManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }


}
