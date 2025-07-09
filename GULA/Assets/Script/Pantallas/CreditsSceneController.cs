using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class CreditsSceneController : MonoBehaviour
{
    public Button backButton;
    public Button twitterButton;
    public AudioClip clickSound;

    public TextMeshProUGUI[] clickableTexts;
    public string[] urls;

    public float delayBeforeSceneChange = 0.2f;

    void Start()
    {
        AddHoverScale(backButton.gameObject, 1f, 1.1f);
        backButton.onClick.AddListener(() =>
        {
            StartCoroutine(BackToMenuWithSound());
        });

        AddHoverScale(twitterButton.gameObject, 1f, 1.1f);
        twitterButton.onClick.AddListener(() =>
        {
            PlayClickSoundPersistent();
            Application.OpenURL("https://x.com/RosemaryTeamJam");
        });

        for (int i = 0; i < clickableTexts.Length && i < urls.Length; i++)
        {
            int index = i;
            clickableTexts[index].text = "<u><color=#00FFFF>Click aquí</color></u>";
            AddClickableLink(clickableTexts[index], urls[index]);
        }
    }

    IEnumerator BackToMenuWithSound()
    {
        PlayClickSoundPersistent();
        yield return new WaitForSeconds(delayBeforeSceneChange);
        SceneManager.LoadScene("MenuScene");
    }

    void PlayClickSoundPersistent()
    {
        GameObject tempGO = new GameObject("TempAudio");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clickSound;
        aSource.Play();
        DontDestroyOnLoad(tempGO);
        Destroy(tempGO, clickSound.length);
    }

    void AddClickableLink(TextMeshProUGUI text, string url)
    {
        var button = text.gameObject.GetComponent<Button>();
        if (button == null)
            button = text.gameObject.AddComponent<Button>();

        button.onClick.AddListener(() =>
        {
            PlayClickSoundPersistent();
            Application.OpenURL(url);
        });

        var colors = button.colors;
        colors.normalColor = Color.cyan;
        colors.highlightedColor = Color.magenta;
        button.colors = colors;
    }

    void AddHoverScale(GameObject target, float normalScale, float hoverScale)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = target.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryEnter.callback.AddListener((eventData) => {
            target.transform.localScale = Vector3.one * hoverScale;
        });

        EventTrigger.Entry entryExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        entryExit.callback.AddListener((eventData) => {
            target.transform.localScale = Vector3.one * normalScale;
        });

        trigger.triggers.Add(entryEnter);
        trigger.triggers.Add(entryExit);
    }
}