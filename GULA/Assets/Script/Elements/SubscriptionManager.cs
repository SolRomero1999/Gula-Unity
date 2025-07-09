using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubscriptionManager : MonoBehaviour
{
    [Header("References")]
    public MoneyManager moneyManager;
    public AudienceManager audienceManager;
    public RectTransform notificationParent;
    public GameObject subscriptionNotificationPrefab;

    private List<string> subscriberNames = new List<string>();
    private HashSet<string> usedNames = new HashSet<string>();
    private Queue<string> notificationQueue = new Queue<string>();

    private bool isShowingNotification = false;
    private float notificationOffset = 0f;

    private int subscribers = 0;
    public int Subscribers => subscribers;

    private int totalSubscriptions = 0;
    public int TotalSubscriptions => totalSubscriptions;

    private Coroutine subscriptionCheckCoroutine;

    private void Awake()
    {
        GenerateSubscriberNames(100);
    }

    private void Start()
    {
        subscriptionCheckCoroutine = StartCoroutine(CheckForSubscribersRoutine());
    }

    private void GenerateSubscriberNames(int count)
    {
        subscriberNames.Clear();
        for (int i = 0; i < count; i++)
            subscriberNames.Add(UserGenerator.GetRandomName());
    }

    private IEnumerator CheckForSubscribersRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            int viewers = audienceManager.CurrentAudience;
            if (viewers <= 0) continue;

            float chancePerViewer = Random.Range(0.001f, 0.005f);
            float totalChance = Mathf.Min(0.5f, viewers * chancePerViewer);

            if (Random.value < totalChance)
                AddSubscriber();
        }
    }

    public void AddSubscriber()
    {
        if (subscriberNames.Count == 0) return;

        List<string> availableNames = subscriberNames.FindAll(name => !usedNames.Contains(name));
        if (availableNames.Count == 0) return;

        string randomName = availableNames[Random.Range(0, availableNames.Count)];
        usedNames.Add(randomName);
        subscribers++;
        totalSubscriptions++;

        moneyManager.AddMoney(50);

        notificationQueue.Enqueue(randomName);
        StartCoroutine(ProcessNotificationQueue());
    }

    private IEnumerator ProcessNotificationQueue()
    {
        if (isShowingNotification || notificationQueue.Count == 0)
            yield break;

        isShowingNotification = true;

        while (notificationQueue.Count > 0)
        {
            string name = notificationQueue.Dequeue();
            yield return ShowSubscriberNotification(name);
            yield return new WaitForSeconds(0.3f);
        }

        isShowingNotification = false;
    }

    private IEnumerator ShowSubscriberNotification(string name)
    {
        GameObject notifGO = Instantiate(subscriptionNotificationPrefab, notificationParent);
        notifGO.transform.localPosition = new Vector3(0, 100f, 0);
        notificationOffset += 50f;

        TMP_Text messageText = notifGO.GetComponentInChildren<TMP_Text>();
        if (messageText != null)
            messageText.text = $"{name} subscribed!";

        RectTransform rt = notifGO.GetComponent<RectTransform>();
        Vector3 startPos = rt.localPosition;
        Vector3 targetPos = Vector3.zero;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rt.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.localPosition = targetPos;

        yield return new WaitForSeconds(3f);

        float fadeDuration = 0.5f;
        elapsed = 0f;
        CanvasGroup canvasGroup = notifGO.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = notifGO.AddComponent<CanvasGroup>();

        Vector3 fadeTargetPos = rt.localPosition + new Vector3(20, 0, 0);

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            rt.localPosition = Vector3.Lerp(targetPos, fadeTargetPos, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(notifGO);
        notificationOffset -= 50f;
    }

    public string GetRandomUsedSubscriber()
    {
        if (usedNames.Count == 0) return null;

        List<string> usedList = new List<string>(usedNames);
        return usedList[Random.Range(0, usedList.Count)];
    }

    public void Cleanup()
    {
        StopAllCoroutines();
        notificationQueue.Clear();
        isShowingNotification = false;
        notificationOffset = 0f;
    }
}