using UnityEngine;
using UnityEngine.UI;

public class TemporalWatch : MonoBehaviour
{
    public float battery = 100f;
    public bool isRecharging;
    public Slider TSVS;
    public Text timerText;
    public Image SFA;
    public bool canStopGame;
    public AudioSource AS;
    public AudioClip AC;
    public AudioClip beepSound;

    private bool beeped;
    public float rechargeRate = 20f; // units per second
    public float drainDuration = 300f; // seconds to fully drain (5 minutes)
    private float timer;
    private bool isPlayingAudio = false;
    private float zeroBatteryTime = 0f;

    private void Start()
    {
        battery = 100f;
    }

    private void Update()
    {
        if (isRecharging)
        {
            battery += rechargeRate * Time.deltaTime;
        }
        else if (battery > 0f)
        {
            float drainRate = 100f / drainDuration;
            battery -= drainRate * Time.deltaTime;
        }
        else
        {
            battery = 0f;
        }

        battery = Mathf.Clamp(battery, 0f, 100f);
        timer = battery * 3;
        TSVS.value = battery;

        if (battery <= 20.3f)
        {
            SFA.color = Color.red;
        }
        else
        {
            SFA.color = Color.green;
        }

        if (battery <= 10.5 && battery >= 10)
        {
            if (!beeped && !isRecharging) { AS.PlayOneShot(beepSound); }
            beeped = true;
        }
        else
        {
            beeped = false;
        }

            UpdateTimerUI();

        if (battery <= 0)
        {
            if (!isPlayingAudio)
            {
                AS.clip = AC;
                AS.Play();
                isPlayingAudio = true;
                canStopGame = true;
                zeroBatteryTime = Time.time; // Start tracking time at 0 battery
            }

            if (Time.time - zeroBatteryTime >= 4f)
            {
                StopGame();
            }
        }
        else
        {
            canStopGame = false;
            if (isPlayingAudio)
            {
                AS.Stop();
                isPlayingAudio = false;
            }
        }
    }

    private void StopGame()
    {
        if (canStopGame)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        FunctionData functionData = other.GetComponent<FunctionData>();
        if (functionData != null && functionData.function == FunctionData.FunctionType.Recharge)
        {
            isRecharging = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FunctionData functionData = other.GetComponent<FunctionData>();
        if (functionData != null && functionData.function == FunctionData.FunctionType.Recharge)
        {
            isRecharging = false;
        }
    }
}
