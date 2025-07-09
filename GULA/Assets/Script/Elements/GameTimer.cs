using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public MoneyManager moneyManager;
    public float durationInMinutes = 5f;

    private float duration;
    private float startTime;
    private float elapsedTime;
    private bool isRunning = false;
    private bool timeUpTriggered = false;

    [Header("UI References")]
    public TextMeshProUGUI liveText;
    public Button liveButton;

    public int TotalElapsedSeconds => Mathf.FloorToInt(elapsedTime);

    void Start()
    {
        duration = durationInMinutes * 60f;
        SetupButton();
        StartTimer();
    }

    void SetupButton()
    {
        if (liveButton != null)
        {
            liveButton.onClick.AddListener(() =>
            {
                if (!timeUpTriggered)
                {
                    GameStatsManager.endType = "manual";
                    GameStatsManager.streamTimeSec = Mathf.FloorToInt(elapsedTime);
                    TriggerGameEnd("manual");
                }
            });
        }
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime = Time.time - startTime;
        float remaining = Mathf.Max(0, duration - elapsedTime);

        if (liveText != null)
        {
            liveText.text = $"LIVE {FormatTime(remaining)}";
            liveText.color = (remaining <= 30 && remaining > 0) ? new Color(1f, 0.61f, 0.29f) : Color.white;
        }

        if (remaining <= 0 && !timeUpTriggered)
        {
            GameStatsManager.endType = "time";
            GameStatsManager.streamTimeSec = Mathf.FloorToInt(elapsedTime);
            TriggerGameEnd("time");
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        elapsedTime = 0f;
        isRunning = true;
        timeUpTriggered = false;
    }

    string FormatTime(float time)
    {
        int totalSeconds = Mathf.FloorToInt(time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    void TriggerGameEnd(string reason)
    {
        timeUpTriggered = true;
        isRunning = false;

        GameStatsManager.streamTimeSec = Mathf.FloorToInt(elapsedTime);

        AudienceManager audienceManager = FindObjectOfType<AudienceManager>();
        if (audienceManager != null)
            GameStatsManager.peakViewers = audienceManager.PeakViewers;

        SubscriptionManager subscriptionManager = FindObjectOfType<SubscriptionManager>();
        if (subscriptionManager != null)
            GameStatsManager.subscribers = subscriptionManager.TotalSubscriptions;

        DonationManager donationManager = FindObjectOfType<DonationManager>();
        if (donationManager != null)
            GameStatsManager.totalDonations = donationManager.TotalDonations;

        GameStatsManager.totalEarnings = moneyManager.TotalEarnings;
        GameStatsManager.totalExpenses = moneyManager.TotalExpenses;
        GameStatsManager.totalEarnings -= GameStatsManager.totalExpenses;

        GameStatsManager.SaveCurrentStats();
        SceneManager.LoadScene("GameOver");
    }

    public void Cleanup()
    {
        if (liveButton != null)
            liveButton.onClick.RemoveAllListeners();
    }
}