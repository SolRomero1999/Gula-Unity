using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DonationManager : MonoBehaviour
{
    [Header("References")]
    public SubscriptionManager subscriptionManager;
    public MoneyManager moneyManager;
    public RectTransform notificationParent;
    public GameObject donationNotificationPrefab;

    private Queue<(string name, int amount)> notificationQueue = new Queue<(string, int)>();
    private bool isShowingNotification = false;
    private float notificationOffset = 0f;

    private Coroutine donationCheckCoroutine;
    private int totalDonations = 0;
    public int TotalDonations => totalDonations;

    void Start()
    {
        donationCheckCoroutine = StartCoroutine(CheckForDonationsRoutine());
    }

    IEnumerator CheckForDonationsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            int subs = subscriptionManager.Subscribers;
            if (subs <= 0) continue;

            float chancePerSub = Random.Range(0.01f, 0.05f); 
            float totalChance = Mathf.Min(0.5f, subs * chancePerSub);

            if (Random.value < totalChance)
                GenerateDonation();
        }
    }

    void GenerateDonation()
    {
        string donorName = subscriptionManager.GetRandomUsedSubscriber();
        if (string.IsNullOrEmpty(donorName)) return;

        int donationRoll = Random.Range(1, 101);
        int amount;

        if (donationRoll <= 5)
            amount = Random.Range(20, 26);
        else if (donationRoll <= 20)
            amount = Random.Range(15, 20);
        else
            amount = Random.Range(5, 15);

        moneyManager.AddMoney(amount);
        totalDonations += amount;

        notificationQueue.Enqueue((donorName, amount));
        StartCoroutine(ProcessNotificationQueue());
    }

    IEnumerator ProcessNotificationQueue()
    {
        if (isShowingNotification || notificationQueue.Count == 0)
            yield break;

        isShowingNotification = true;

        while (notificationQueue.Count > 0)
        {
            var (name, amount) = notificationQueue.Dequeue();
            yield return ShowDonationNotification(name, amount);
            yield return new WaitForSeconds(0.3f);
        }

        isShowingNotification = false;
    }

    IEnumerator ShowDonationNotification(string name, int amount)
    {
        GameObject notifGO = Instantiate(donationNotificationPrefab, notificationParent);
        notifGO.transform.localPosition = new Vector3(0, 100f, 0);
        notificationOffset += 70f;

        TMP_Text text = notifGO.GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = GetDonationMessage(name, amount);

        RectTransform rt = notifGO.GetComponent<RectTransform>();
        Vector3 startPos = rt.localPosition;
        Vector3 targetPos = Vector3.zero;

        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rt.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.localPosition = targetPos;

        yield return new WaitForSeconds(3f);

        float fadeDuration = 0.6f;
        elapsed = 0f;
        CanvasGroup cg = notifGO.GetComponent<CanvasGroup>();
        if (cg == null) cg = notifGO.AddComponent<CanvasGroup>();
        Vector3 fadeTarget = rt.localPosition + new Vector3(20, 0, 0);

        while (elapsed < fadeDuration)
        {
            cg.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            rt.localPosition = Vector3.Lerp(targetPos, fadeTarget, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(notifGO);
        notificationOffset -= 70f;
    }

    string GetDonationMessage(string name, int amount)
    {
        if (amount >= 20)
            return $"{name} is insanely generous!\nDonated ${amount}!";
        else if (amount >= 15)
            return $"{name} is making it rain!\nSent ${amount}!";
        else if (amount >= 10)
            return $"{name} shows some love!\n${amount} donated!";
        else if (amount >= 7)
            return $"{name} tipped ${amount}!\nThanks for the support!";
        else
            return $"{name} dropped ${amount}!\nEvery bit helps!";
    }

    public void Cleanup()
    {
        if (donationCheckCoroutine != null)
            StopCoroutine(donationCheckCoroutine);

        notificationQueue.Clear();
        isShowingNotification = false;
        notificationOffset = 0f;
    }
}