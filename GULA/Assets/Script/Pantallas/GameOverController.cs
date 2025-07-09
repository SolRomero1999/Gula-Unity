using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Fondo y títulos")]
    public Image backgroundImage;
    public Sprite stomachSprite;
    public Sprite audienceSprite;
    public Sprite manualSprite;
    public Sprite timeSprite;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;

    [Header("Estadísticas actuales")]
    public TextMeshProUGUI[] currentStatTexts;
    [Header("Récords")]
    public TextMeshProUGUI[] bestStatTexts;
    [Header("Estrellas de récord")]
    public GameObject[] recordStars;

    [Header("Botones")]
    public Button mainMenuButton;
    public Button playAgainButton;

    void Start()
    {
        SetBackgroundAndTitle();
        DisplayStats();

        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
        playAgainButton.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));
    }

    void SetBackgroundAndTitle()
    {
        string endType = GameStatsManager.endType;

        switch (endType)
        {
            case "stomach":
                backgroundImage.sprite = stomachSprite;
                titleText.text = "STREAM INTERRUPTED!";
                titleText.color = Color.white;
                subtitleText.text = "You ate too much and had to stop!";
                break;
            case "audience":
                backgroundImage.sprite = audienceSprite;
                titleText.text = "STREAM FAILED!";
                titleText.color = Color.white;
                subtitleText.text = "Your audience left the stream!";
                break;
            case "manual":
                backgroundImage.sprite = manualSprite;
                titleText.text = "STREAM ENDED";
                titleText.color = new Color(1f, 0.66f, 0.29f);
                subtitleText.text = "You decided to end the stream early.";
                break;
            case "time":
            default:
                backgroundImage.sprite = timeSprite;
                titleText.text = "STREAM COMPLETED!";
                titleText.color = new Color(0.29f, 1f, 0.4f);
                subtitleText.text = "You finished your stream successfully!";
                break;
        }
    }

    void DisplayStats()
    {
        int timeSec = GameStatsManager.streamTimeSec;
        currentStatTexts[0].text = FormatTime(timeSec);
        bestStatTexts[0].text = FormatTime(GameStatsManager.GetBest("StreamTime"));
        UpdateStar(0, timeSec > GameStatsManager.PreviousBest.StreamTime);

        currentStatTexts[1].text = FormatNumber(GameStatsManager.peakViewers);
        bestStatTexts[1].text = FormatNumber(GameStatsManager.GetBest("PeakViewers"));
        UpdateStar(1, GameStatsManager.peakViewers > GameStatsManager.PreviousBest.PeakViewers);

        currentStatTexts[2].text = FormatNumber(GameStatsManager.subscribers);
        bestStatTexts[2].text = FormatNumber(GameStatsManager.GetBest("Subscribers"));
        UpdateStar(2, GameStatsManager.subscribers > GameStatsManager.PreviousBest.Subscribers);

        currentStatTexts[3].text = $"${FormatNumber(GameStatsManager.totalDonations)}";
        bestStatTexts[3].text = $"${FormatNumber(GameStatsManager.GetBest("Donations"))}";
        UpdateStar(3, GameStatsManager.totalDonations > GameStatsManager.PreviousBest.Donations);

        currentStatTexts[4].text = $"${FormatNumber(GameStatsManager.totalEarnings)}";
        bestStatTexts[4].text = $"${FormatNumber(GameStatsManager.GetBest("Earnings"))}";
        UpdateStar(4, GameStatsManager.totalEarnings > GameStatsManager.PreviousBest.Earnings);

        currentStatTexts[5].text = FormatNumber(GameStatsManager.foodConsumed);
        bestStatTexts[5].text = FormatNumber(GameStatsManager.GetBest("Food"));
        UpdateStar(5, GameStatsManager.foodConsumed > GameStatsManager.PreviousBest.Food);
    }

    void UpdateStar(int index, bool isRecord)
    {
        if (recordStars != null && index < recordStars.Length)
        {
            recordStars[index].SetActive(isRecord);
        }
    }

    string FormatTime(int totalSeconds)
    {
        int mins = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return $"{mins:00}:{secs:00}";
    }

    string FormatNumber(int num)
    {
        return num.ToString("N0");
    }
}