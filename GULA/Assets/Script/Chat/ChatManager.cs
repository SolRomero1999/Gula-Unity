using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform chatContent;
    public GameObject chatMessagePrefab;
    public ScrollRect chatScrollRect;
    public int maxMessages = 50;

    private Queue<GameObject> messageQueue = new Queue<GameObject>();

    public void AddMessage(string userName, Color userColor, string messageText)
    {
        GameObject msgObj = Instantiate(chatMessagePrefab, chatContent);

        TMP_Text userTMP = msgObj.transform.Find("UserText")?.GetComponent<TMP_Text>();
        TMP_Text messageTMP = msgObj.transform.Find("MessageText")?.GetComponent<TMP_Text>();

        if (userTMP != null)
        {
            userTMP.text = userName;
            userTMP.color = userColor;
        }

        if (messageTMP != null)
        {
            messageTMP.text = messageText;
        }

        messageQueue.Enqueue(msgObj);

        if (messageQueue.Count > maxMessages)
        {
            Destroy(messageQueue.Dequeue());
        }

        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
    }
}