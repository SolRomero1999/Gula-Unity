using UnityEngine;
using System.Collections;

public class MessageGenerator : MonoBehaviour
{
    public ChatManager chatManager;
    public FoodUIManager foodUIManager;

    private float messageInterval = 1f;
    private float boredomThreshold = 5f;
    private float angerThreshold = 10f;

    private bool usandoMensajesBienvenida = false;

    void Start()
    {
        // Determinar si es el primer tutorial
        if (LevelManager.instance != null)
        {
            var nivel = LevelManager.instance.niveles[LevelManager.instance.nivelActual];
            if (LevelManager.instance.nivelActual == 0 && nivel.esTutorial)
            {
                usandoMensajesBienvenida = true;
            }
        }

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
        if (usandoMensajesBienvenida)
            return WelcomeMessage();

        bool inactivityActive = foodUIManager.InactivityIsActive();
        float inactivityTime = foodUIManager.GetInactivityTime();

        if (inactivityActive)
        {
            if (inactivityTime > angerThreshold) return AngryMessage();
            if (inactivityTime > boredomThreshold) return BoredMessage();
        }

        return StandardMessage();
    }

    string WelcomeMessage()
    {
        string[] messages =
        {
            "A new mukbang streamer? I'm in!",
            "Looks like someone's starting their journey!",
            "This is gonna be fun to watch!",
            "Welcome to Gula!",
            "First stream hype!",
            "New streamer? Let's gooo!",
            "Yay! Excited to be here!",
            "I love finding new streamers!",
            "Already a fan!",
            "Hope they can eat a lot!",
            "Bring on the food!",
            "Good luck, streamer!",
            "Let's make this viral!",
            "I'm staying for the whole thing",
            "Wishing you a great first stream!"
        };

        return messages[Random.Range(0, messages.Length)];
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