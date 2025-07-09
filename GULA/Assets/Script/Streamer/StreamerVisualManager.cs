using UnityEngine;
using UnityEngine.UI;

public class StreamerVisualManager : MonoBehaviour
{
    public Image streamerImage;

    [Header("Sprite Settings")]
    public string spriteFolder = "Sprites/Streamer/";
    public string normalSprite = "streamer_normal";
    public string midSprite = "streamer_mid";
    public string fullSprite = "streamer_full";

    [Header("Stomach Thresholds")]
    public float midThreshold = 40f;
    public float fullThreshold = 80f;

    private float lastStomachLevel = -1f;

    public void UpdateVisual(float currentStomach, float maxStomach)
    {
        float percentage = (currentStomach / maxStomach) * 100f;

        // Evitar actualizaciones innecesarias
        if (Mathf.Approximately(percentage, lastStomachLevel)) return;
        lastStomachLevel = percentage;

        if (percentage >= fullThreshold)
        {
            SetSprite(fullSprite);
        }
        else if (percentage >= midThreshold)
        {
            SetSprite(midSprite);
        }
        else
        {
            SetSprite(normalSprite);
        }
    }

    void SetSprite(string spriteName)
    {
        Sprite newSprite = Resources.Load<Sprite>($"{spriteFolder}{spriteName}");
        if (newSprite != null)
        {
            streamerImage.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"⚠️ Sprite no encontrado: {spriteFolder}{spriteName}");
        }
    }
}
