using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AudienceManager : MonoBehaviour
{
    public MoneyManager moneyManager;

    [Header("Audiencia")]
    public int rating = 100;
    public int[] goals = { 1000, 2500, 5000, 10000 };
    private int currentGoalIndex = 0;
    private int peakViewers = 100;
    public int PeakViewers => peakViewers;

    [Header("UI")]
    public TMP_Text goalText;
    public Image panelBackground;

    [Header("Decay Config")]
    public float decayInterval = 5f;
    public float decayPercentage = 0.05f;

    private float decayTimer;
    private bool gameOverTriggered = false;

    private bool isAnimating = false;
    private float animationDuration = 0.15f;
    private float animationTimer = 0f;
    private Vector3 originalGoalScale;

    void Start()
    {
        UpdateUI();
        originalGoalScale = goalText.transform.localScale;
    }

    void Update()
    {
        decayTimer += Time.deltaTime;

        if (decayTimer >= decayInterval)
        {
            decayTimer = 0f;
            int loss = Mathf.Max(1, Mathf.FloorToInt(rating * decayPercentage));
            ChangeRating(-loss);
        }

        if (isAnimating)
            AnimatePulse();
    }

    public void ChangeRating(int amount)
    {
        if (gameOverTriggered) return;

        int oldRating = rating;
        rating = Mathf.Max(0, rating + amount);

        if (rating > peakViewers)
            peakViewers = rating;

        UpdateUI();

        if (rating >= goals[currentGoalIndex])
            ReachNewGoal();

        if (amount > 0)
            AnimateUI();

        if (rating <= 0)
            TriggerGameOver();
    }

    void ReachNewGoal()
    {
        if (currentGoalIndex < goals.Length - 1)
            currentGoalIndex++;
    }

    void AnimateUI()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            animationTimer = 0f;
        }
    }

    void AnimatePulse()
    {
        animationTimer += Time.deltaTime;
        float halfDuration = animationDuration / 2f;

        if (animationTimer <= halfDuration)
        {
            float t = animationTimer / halfDuration;
            float scale = Mathf.Lerp(1f, 1.1f, t);
            goalText.transform.localScale = originalGoalScale * scale;
        }
        else if (animationTimer <= animationDuration)
        {
            float t = (animationTimer - halfDuration) / halfDuration;
            float scale = Mathf.Lerp(1.1f, 1f, t);
            goalText.transform.localScale = originalGoalScale * scale;
        }
        else
        {
            goalText.transform.localScale = originalGoalScale;
            isAnimating = false;
        }
    }

    void UpdateUI()
    {
        goalText.text = $"{rating} / {goals[currentGoalIndex]}";
    }

    void TriggerGameOver()
    {
        gameOverTriggered = true;
        GameStatsManager.endType = "audience";

        GameTimer timer = FindObjectOfType<GameTimer>();
        if (timer != null)
            GameStatsManager.streamTimeSec = timer.TotalElapsedSeconds;

        GameStatsManager.peakViewers = PeakViewers;

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

    public int CurrentAudience => rating;
}