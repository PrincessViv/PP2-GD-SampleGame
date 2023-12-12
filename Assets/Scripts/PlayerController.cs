using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;

    [Header("----- Player Stats -----")]
    [Range(1, 8)][SerializeField] float playerSpeed;
    [Range(8, 30)][SerializeField] float jumpHeight;
    [Range(-10, 40)][SerializeField] float gravityValue;
    [Range(1.5f, 3)][SerializeField] float sprintMod;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(1, 10)][SerializeField] int HP;

    [Header("----- Weapon Stats -----")]
    [SerializeField]
    List<GunStats> gunList =
    new List<GunStats>();
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    //[SerializeField] GameObject cube;
    [SerializeField] GameObject gunModel;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] soundHurt;
    [Range(0, 1)][SerializeField] float soundHurtVol;
    [SerializeField] AudioClip[] soundSteps;
    [Range(0, 1)][SerializeField] float soundStepsVol;


    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private int jumpCount;
    private bool isShooting;
    int HPOrig;
    int selectedGun;
    bool isPlayingSteps;
    bool isSprinting;

    private void Start()
    {
        HPOrig = HP;
        RespawnPlayer();
    }

    void Update()
    {
        if (!GManager.instance.isPaused)
        {
            if (Input.GetButton("Shoot") && !isShooting)
            {
                StartCoroutine(Shoot());
                SelectGun();
            }

            Movement();
        }

    }

    IEnumerator PlaySteps()
    {
        isPlayingSteps = true;

        aud.PlayOneShot(soundSteps[Random.Range(9, soundSteps.Length - 1)],
        soundStepsVol);
        if (!isSprinting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }

        isPlayingSteps = false;
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

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && move.normalized.magnitude > 0.3f
        && !isPlayingSteps)
        {
            StartCoroutine(PlaySteps());
        }

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
        if (gunList[selectedGun].ammoCurr > 0)
        {
            isShooting = true;

            gunList[selectedGun].ammoCurr--;
            aud.PlayOneShot(gunList[selectedGun].shootSound,
            gunList[selectedGun].shootSoundVol);

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new
            Vector2(0.5f, 0.5f)), out hit, shootDist))
            {
                Debug.Log(hit.transform.name);
                Instantiate(gunList[selectedGun].hitEffect,
                hit.point, transform.rotation);

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (hit.collider != transform || dmg != null)
                {
                    dmg.TakeDamage(shootDamage);
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }

    }

    void Sprint()
    {
        //If button is pressed, Player sprints
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
            isSprinting = true;
        }

        else if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed /= sprintMod;
            isSprinting = false;
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        aud.PlayOneShot(soundHurt[Random.Range(0, soundHurt.Length - 1)],
         soundHurtVol);

        UpdatePlayerUI();
        StartCoroutine(PlayerFlashDamage());


        if (HP <= 0)
        {
            //Game over! :-(
            GManager.instance.YouLose();
        }
    }

    IEnumerator PlayerFlashDamage()
    {
        GManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GManager.instance.playerDamageScreen.SetActive(false);
    }

    public void UpdatePlayerUI()
    {
        GManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    public void GetGunStats(GunStats gun)
    {

        gunList.Add(gun);

        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh =
        gun.model.GetComponent<MeshFilter>().sharedMesh;

        gunModel.GetComponent<MeshRenderer>().sharedMaterial =
        gun.model.GetComponent<MeshRenderer>().sharedMaterial;

        selectedGun = gunList.Count - 1;
    }

    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun
        < gunList.Count - 1)
        {
            selectedGun++;
            ChangeGun();
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            ChangeGun();
        }
    }

    void ChangeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDist = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh =
        gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;

        gunModel.GetComponent<MeshRenderer>().sharedMaterial =
        gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;

        isShooting = false;
    }

}
