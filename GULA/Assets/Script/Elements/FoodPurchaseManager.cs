using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FoodPurchaseManager : MonoBehaviour
{
    public FoodUIManager foodUIManager;
    public MoneyManager moneyManager;

    [Header("Precios")]
    public int ramenPrice = 50;
    public int sushiPrice = 200;

    [Header("Botones UI")]
    public Button buyRamenButton;
    public Button buySushiButton;

    private bool ramenButtonShown = false;
    private bool sushiButtonShown = false;

    private Coroutine ramenAnimCoroutine = null;
    private Coroutine sushiAnimCoroutine = null;

    void Start()
    {
        buyRamenButton.onClick.AddListener(BuyRamen);
        buySushiButton.onClick.AddListener(BuySushi);

        buyRamenButton.gameObject.SetActive(false);
        buySushiButton.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckButtonsVisibility();
    }

    void CheckButtonsVisibility()
    {
        if (foodUIManager.IsRamenEmpty())
        {
            if (!ramenButtonShown)
            {
                ShowButtonWithAnimation(buyRamenButton, ref ramenAnimCoroutine);
                ramenButtonShown = true;
            }
        }
        else if (ramenButtonShown)
        {
            HideButtonWithAnimation(buyRamenButton, ref ramenAnimCoroutine);
            ramenButtonShown = false;
        }

        if (foodUIManager.IsSushiEmpty())
        {
            if (!sushiButtonShown)
            {
                ShowButtonWithAnimation(buySushiButton, ref sushiAnimCoroutine);
                sushiButtonShown = true;
            }
        }
        else if (sushiButtonShown)
        {
            HideButtonWithAnimation(buySushiButton, ref sushiAnimCoroutine);
            sushiButtonShown = false;
        }
    }

    void ShowButtonWithAnimation(Button button, ref Coroutine coroutine)
    {
        button.gameObject.SetActive(true);
        button.transform.localScale = Vector3.one;
        coroutine = StartCoroutine(ScaleYoyo(button.transform));
    }

    void HideButtonWithAnimation(Button button, ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        button.transform.localScale = Vector3.one;
        button.gameObject.SetActive(false);
    }

    IEnumerator ScaleYoyo(Transform target)
    {
        float minScale = 1f;
        float maxScale = 1.2f;
        float duration = 0.5f;

        while (true)
        {
            yield return ScaleOverTime(target, minScale, maxScale, duration);
            yield return ScaleOverTime(target, maxScale, minScale, duration);
        }
    }

    IEnumerator ScaleOverTime(Transform target, float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float scale = Mathf.Lerp(from, to, t);
            target.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    void BuyRamen()
    {
        if (moneyManager.Money >= ramenPrice && moneyManager.SpendMoney(ramenPrice))
        {
            foodUIManager.currentBowlLevel = 1;
            foodUIManager.UpdateRamenSprite();
            HideButtonWithAnimation(buyRamenButton, ref ramenAnimCoroutine);
            ramenButtonShown = false;
        }
    }

    void BuySushi()
    {
        if (moneyManager.Money >= sushiPrice && moneyManager.SpendMoney(sushiPrice))
        {
            foodUIManager.currentSushiLevel = 1;
            foodUIManager.UpdateSushiSprite();
            HideButtonWithAnimation(buySushiButton, ref sushiAnimCoroutine);
            sushiButtonShown = false;
        }
    }
}