using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GManager : MonoBehaviour
{

    public static GManager instance;

    public GameObject player;
    public GameObject playerSpawnPOS;
    public PlayerController playerScript;

    public bool isPaused;
    float timeScaleOrig;
    int enemiesRemaining;
    public Image playerHPBar;
    
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] TMP_Text enemyCountText;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        //This tells other scripts- I have access
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPOS = GameObject.FindWithTag("Player Spawn POS");
        timeScaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") 
        && menuActive == null)
        {
            StatePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
    }

    public void StatePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0; //Pauses everything- Minus menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void UpdateGameGoal(int amount)
    {
        enemiesRemaining += amount;
        enemyCountText.text = enemiesRemaining.ToString("0");

        if (enemiesRemaining <= 0)
        {
            //You win!
            StatePaused();
            menuActive = menuWin;
            menuActive.SetActive(true);
            
        }
    }

    public void YouLose()
    {
        StatePaused();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
