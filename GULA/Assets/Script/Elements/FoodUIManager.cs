using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class FoodUIManager : MonoBehaviour
{
    [Header("Ramen")]
    public Image ramenImage;
    public string ramenSpriteBase = "ramen_bowl";
    public int currentBowlLevel = 1;
    public int maxBowlLevel = 5;
    public string ramenSound = "sorbido";

    [Header("Sushi")]
    public Image sushiImage;
    public string sushiSpriteBase = "sushi";
    public int currentSushiLevel = 1;
    public int maxSushiLevel = 12;
    public string sushiSound = "bocado";

    [Header("Satiety Bar")]
    public Slider stomachBar;
    public Image stomachFillImage;
    public float stomachCapacity = 100f;
    public float foodConsumptionRate = 10f;
    public float sushiConsumptionRate = 3f;
    public float stomachRecoveryRate = 3f;

    [Header("References")]
    public AudienceManager audienceManager;
    public StreamerVisualManager streamerVisual;
    public MoneyManager moneyManager;

    [Header("Penalties")]
    public float inactivityThreshold = 3f;
    public float fullStomachThreshold = 3f;
    public float inactivityBaseLoss = 5f;
    public float inactivityMaxLoss = 25f;
    public float inactivityIncrementRate = 0.5f;
    public float fullStomachBaseLoss = 5f;
    public float fullStomachMaxLoss = 40f;
    public float fullStomachIncrementRate = 1f;

    float inactivityTimer = 0f;
    float fullStomachTimer = 0f;
    float inactivityCurrentLoss = 0f;
    float fullStomachCurrentLoss = 0f;
    float inactivityPenaltyTimer = 0f;
    float fullStomachPenaltyTimer = 0f;

    bool inactivityActive = false;
    bool fullStomachActive = false;
    float lastEatTime;
    float currentStomach = 0f;
    bool gameOverTriggered = false;

    Color originalAudienceTextColor;

    void Start()
    {
        UpdateRamenSprite();
        UpdateSushiSprite();
        AddClickListener(ramenImage.gameObject, OnClickRamen);
        AddClickListener(sushiImage.gameObject, OnClickSushi);
        currentStomach = 0f;
        UpdateStomachBar();
        lastEatTime = Time.time;
        if (audienceManager != null && audienceManager.goalText != null)
            originalAudienceTextColor = audienceManager.goalText.color;
    }

    void Update()
    {
        float timeSinceLastEat = Time.time - lastEatTime;

        if (timeSinceLastEat > inactivityThreshold)
        {
            inactivityTimer += Time.deltaTime;
            if (!inactivityActive)
            {
                inactivityActive = true;
                inactivityCurrentLoss = inactivityBaseLoss;
            }
            else
            {
                inactivityCurrentLoss = Mathf.Min(inactivityCurrentLoss + inactivityIncrementRate * Time.deltaTime, inactivityMaxLoss);
            }

            inactivityPenaltyTimer += Time.deltaTime;
            if (inactivityPenaltyTimer >= 1f)
            {
                inactivityPenaltyTimer = 0f;
                ApplyAudiencePenalty(-Mathf.RoundToInt(inactivityCurrentLoss));
            }
        }
        else if (inactivityActive)
        {
            inactivityActive = false;
            inactivityCurrentLoss = 0f;
            inactivityTimer = 0f;
            inactivityPenaltyTimer = 0f;
        }

        if (currentStomach >= stomachCapacity - 0.1f)
        {
            fullStomachTimer += Time.deltaTime;
            if (!fullStomachActive && fullStomachTimer > fullStomachThreshold)
            {
                fullStomachActive = true;
                fullStomachCurrentLoss = fullStomachBaseLoss;
            }
            else if (fullStomachActive)
            {
                fullStomachCurrentLoss = Mathf.Min(fullStomachCurrentLoss + fullStomachIncrementRate * Time.deltaTime, fullStomachMaxLoss);
            }

            fullStomachPenaltyTimer += Time.deltaTime;
            if (fullStomachPenaltyTimer >= 1f)
            {
                fullStomachPenaltyTimer = 0f;
                ApplyAudiencePenalty(-Mathf.RoundToInt(fullStomachCurrentLoss));
            }
        }
        else if (fullStomachActive)
        {
            fullStomachActive = false;
            fullStomachCurrentLoss = 0f;
            fullStomachTimer = 0f;
            fullStomachPenaltyTimer = 0f;
        }

        if (audienceManager != null && audienceManager.goalText != null)
        {
            audienceManager.goalText.color = (inactivityActive || fullStomachActive) ? Color.red : originalAudienceTextColor;
        }

        if (currentStomach > 0f)
        {
            currentStomach -= stomachRecoveryRate * Time.deltaTime;
            currentStomach = Mathf.Clamp(currentStomach, 0f, stomachCapacity);
            UpdateStomachBar();
        }

        if (!gameOverTriggered && currentStomach >= stomachCapacity - 0.1f)
        {
            gameOverTriggered = true;
            GameStatsManager.endType = "stomach";

            GameTimer timer = FindObjectOfType<GameTimer>();
            if (timer != null) GameStatsManager.streamTimeSec = timer.TotalElapsedSeconds;
            AudienceManager audienceManager = FindObjectOfType<AudienceManager>();
            if (audienceManager != null) GameStatsManager.peakViewers = audienceManager.PeakViewers;
            SubscriptionManager subscriptionManager = FindObjectOfType<SubscriptionManager>();
            if (subscriptionManager != null) GameStatsManager.subscribers = subscriptionManager.TotalSubscriptions;
            DonationManager donationManager = FindObjectOfType<DonationManager>();
            if (donationManager != null) GameStatsManager.totalDonations = donationManager.TotalDonations;
            GameStatsManager.totalEarnings = moneyManager.TotalEarnings;
            GameStatsManager.totalExpenses = moneyManager.TotalExpenses;
            GameStatsManager.totalEarnings -= GameStatsManager.totalExpenses;
            GameStatsManager.SaveCurrentStats();

            SceneManager.LoadScene("GameOver");
        }
    }

    void ApplyAudiencePenalty(int loss)
    {
        if (gameOverTriggered) return;
        audienceManager.ChangeRating(loss);
        if (audienceManager.rating <= 0)
        {
            gameOverTriggered = true;
            SceneManager.LoadScene("GameOver");
        }
    }

    void AddClickListener(GameObject obj, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((_) => action());
        trigger.triggers.Add(entry);
    }

    bool IncreaseStomach(float amount)
    {
        float oldStomach = currentStomach;
        currentStomach += amount;
        currentStomach = Mathf.Clamp(currentStomach, 0f, stomachCapacity);
        UpdateStomachBar();
        streamerVisual?.UpdateVisual(currentStomach, stomachCapacity);
        return currentStomach > oldStomach;
    }

    void OnClickRamen()
    {
        if (currentBowlLevel < maxBowlLevel)
        {
            currentBowlLevel++;
            UpdateRamenSprite();
            PlaySound(ramenSound);
            if (IncreaseStomach(foodConsumptionRate)) GameStatsManager.foodConsumed++;
            lastEatTime = Time.time;
            inactivityActive = false;
            inactivityCurrentLoss = 0f;
            inactivityTimer = 0f;
            int increase = Mathf.Max(1, Mathf.FloorToInt(audienceManager.rating * 0.1f));
            audienceManager.ChangeRating(increase);
        }
    }

    void OnClickSushi()
    {
        if (currentSushiLevel < maxSushiLevel)
        {
            currentSushiLevel++;
            UpdateSushiSprite();
            PlaySound(sushiSound);
            if (IncreaseStomach(sushiConsumptionRate)) GameStatsManager.foodConsumed++;
            lastEatTime = Time.time;
            inactivityActive = false;
            inactivityCurrentLoss = 0f;
            inactivityTimer = 0f;
            int increase = Mathf.Max(1, Mathf.FloorToInt(audienceManager.rating * 0.05f));
            audienceManager.ChangeRating(increase);
        }
    }

    public void UpdateRamenSprite()
    {
        string path = $"Sprites/{ramenSpriteBase}/{ramenSpriteBase}{currentBowlLevel}";
        ramenImage.sprite = Resources.Load<Sprite>(path);
    }

    public void UpdateSushiSprite()
    {
        string path = $"Sprites/{sushiSpriteBase}/{sushiSpriteBase}{currentSushiLevel}";
        sushiImage.sprite = Resources.Load<Sprite>(path);
    }

    void PlaySound(string soundName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Audio/{soundName}");
        if (clip != null) AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    void UpdateStomachBar()
    {
        if (stomachBar != null)
        {
            stomachBar.maxValue = stomachCapacity;
            stomachBar.value = currentStomach;
            if (stomachFillImage != null)
            {
                float t = currentStomach / stomachCapacity;
                stomachFillImage.color = Color.Lerp(Color.green, Color.red, t);
            }
        }
    }

    public void ResetFood()
    {
        currentBowlLevel = 1;
        currentSushiLevel = 1;
        UpdateRamenSprite();
        UpdateSushiSprite();
        currentStomach = 0f;
        UpdateStomachBar();
        streamerVisual?.UpdateVisual(currentStomach, stomachCapacity);
    }

    public bool IsRamenEmpty() => currentBowlLevel >= maxBowlLevel;
    public bool IsSushiEmpty() => currentSushiLevel >= maxSushiLevel;
    public bool InactivityIsActive() => inactivityActive;
    public float GetInactivityTime() => inactivityTimer;

    public void ResetStomach()
    {
        currentStomach = 0f;
        UpdateStomachBar();
        streamerVisual?.UpdateVisual(currentStomach, stomachCapacity);
    }

}