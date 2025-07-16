using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public Button startButton;
    public Button challengeButton;
    public Button creditsButton;
    public AudioClip menuMusic;
    public AudioClip clickSound;

    private static AudioSource backgroundMusicInstance;
    private Vector3 startOriginalScale;

    void Start()
    {
        startOriginalScale = startButton.transform.localScale;

        if (backgroundMusicInstance == null || !backgroundMusicInstance.isPlaying)
        {
            backgroundMusicInstance = gameObject.AddComponent<AudioSource>();
            backgroundMusicInstance.clip = menuMusic;
            backgroundMusicInstance.loop = true;
            backgroundMusicInstance.volume = 0.5f;
            backgroundMusicInstance.Play();
        }

        startButton.onClick.AddListener(() => {
            PlayClickSound();
            backgroundMusicInstance.Stop();
            SceneManager.LoadScene("GameScene");
        });
        
        challengeButton.onClick.AddListener(() => {
            PlayClickSound();
            backgroundMusicInstance.Stop();
            SceneManager.LoadScene("Challenge");
        });

        creditsButton.onClick.AddListener(() => {
            PlayClickSound();
            SceneManager.LoadScene("CreditsScene");
        });

        StartCoroutine(AnimateButtonYoyo(startButton));
        AddHoverListeners(startButton, 1.1f);
        AddHoverListeners(creditsButton, 1.08f);
    }

    void PlayClickSound()
    {
        AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
    }

    IEnumerator AnimateButtonYoyo(Button button)
    {
        Vector3 originalScale = button.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        float duration = 1f;

        while (true)
        {
            yield return ScaleButton(button, originalScale, targetScale, duration);
            yield return ScaleButton(button, targetScale, originalScale, duration);
        }
    }

    IEnumerator ScaleButton(Button button, Vector3 from, Vector3 to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            button.transform.localScale = Vector3.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        button.transform.localScale = to;
    }

    void AddHoverListeners(Button button, float hoverScaleMultiplier)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            button.transform.localScale = startOriginalScale * hoverScaleMultiplier;
        });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            button.transform.localScale = startOriginalScale;
        });
        trigger.triggers.Add(entryExit);
    }
}