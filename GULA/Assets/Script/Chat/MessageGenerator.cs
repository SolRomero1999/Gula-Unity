using UnityEngine;
using System.Collections;

public class MessageGenerator : MonoBehaviour
{
    public ChatManager chatManager;
    public FoodUIManager foodUIManager;

    private float messageInterval = 1f;
    private float boredomThreshold = 5f;
    private float angerThreshold = 10f;

    void Start()
    {
        StartCoroutine(RandomChatCoroutine());
    }

    IEnumerator RandomChatCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(messageInterval);

            string user = UserGenerator.GetRandomName();
            Color color = UserGenerator.GetRandomColor();
            string message = GenerateMessageBasedOnState();

            chatManager.AddMessage(user, color, message);
        }
    }

    string GenerateMessageBasedOnState()
    {
        bool inactivityActive = foodUIManager.InactivityIsActive();
        float inactivityTime = foodUIManager.GetInactivityTime();

        if (inactivityActive)
        {
            if (inactivityTime > angerThreshold) return AngryMessage();
            if (inactivityTime > boredomThreshold) return BoredMessage();
        }

        return StandardMessage();
    }

    string StandardMessage()
    {
        string[] messages =
        {
            "OMG that looks delicious!",
            "How can you eat so much?",
            "That's a big bite!",
            "Don't choke!",
            "Yummy yummy!",
            "I could never eat that much",
            "Bruh that's insane",
            "That's some next level eating",
            "How many calories is that?",
            "I'm getting full just watching",
            "That crunch tho!",
            "No way he finishes that",
            "Absolute madness!",
            "This is my dinner show",
            "I can't look away",
            "That sauce looks spicy!",
            "My stomach hurts watching this"
        };

        return messages[Random.Range(0, messages.Length)];
    }

    string BoredMessage()
    {
        string[] messages =
        {
            "Zzzz... so boring",
            "When will the eating start?",
            "I came here to watch mukbang...",
            "This is just sad",
            "Where's the food?",
            "Bruh... this ain't mukbang",
            "*checks watch*",
            "Did you forget what stream this is?",
            "I could be watching paint dry",
            "This is content?",
            "*yawns*",
            "Not what I subscribed for...",
            "Where's the content?",
            "*leaves to watch actual mukbang*",
            "This stream fell off",
            "Did streamer forget to eat?",
            "I'm here for FOOD not this",
            "*opens another stream*"
        };

        return messages[Random.Range(0, messages.Length)];
    }

    string AngryMessage()
    {
        string[] messages =
        {
            "This ain't mukbang!",
            "You call this eating?",
            "Pathetic portions",
            "My grandma eats more than this!",
            "*unsubs*",
            "Where's the challenge?",
            "This is baby food amounts",
            "Fake mukbanger",
            "I want my money back!",
            "Scam mukbang",
            "Not even trying!",
            "Worst mukbang ever",
            "You're embarrassing us",
            "Do you even mukbang bro?",
            "This is insulting to mukbang",
            "*throws tomatoes*",
            "0/10 would not watch again",
            "Where's the real streamer?"
        };

        return messages[Random.Range(0, messages.Length)];
    }
}