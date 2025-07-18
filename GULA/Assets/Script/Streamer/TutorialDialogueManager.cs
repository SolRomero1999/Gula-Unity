using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TutorialDialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelDialogo;
    public TMP_Text textoDialogo;
    public Button botonSiguiente;
    public Button botonFinalizar;

    [Header("Tipeo")]
    public float typingTime = 0.05f;

    private string[] lineas;
    private int index = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    public bool IsTutorialActive => panelDialogo != null && panelDialogo.activeSelf;

    void Start()
    {
        if (LevelManager.instance != null)
        {
            LevelData nivel = LevelManager.instance.niveles[LevelManager.instance.nivelActual];
            if (nivel.esTutorial && nivel.lineasTutorial != null && nivel.lineasTutorial.Length > 0)
            {
                lineas = nivel.lineasTutorial;
                index = 0;
                MostrarLineaActual();
                panelDialogo.SetActive(true);
                botonSiguiente.onClick.AddListener(Siguiente);
                botonFinalizar.onClick.AddListener(Finalizar);
                botonFinalizar.gameObject.SetActive(false);
            }
            else
            {
                panelDialogo.SetActive(false);
            }
        }
    }

    void MostrarLineaActual()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TipearLinea(lineas[index]));
    }

    IEnumerator TipearLinea(string linea)
    {
        isTyping = true;
        textoDialogo.text = "";

        foreach (char c in linea)
        {
            textoDialogo.text += c;
            yield return new WaitForSeconds(typingTime);
        }

        isTyping = false;
        ActualizarBotones();
    }

    void Siguiente()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            textoDialogo.text = lineas[index];
            isTyping = false;
            ActualizarBotones();
            return;
        }

        index++;
        if (index < lineas.Length)
        {
            MostrarLineaActual();
        }
        else
        {
            botonSiguiente.gameObject.SetActive(false);
            botonFinalizar.gameObject.SetActive(true);
        }
    }
    void Finalizar()
    {
        panelDialogo.SetActive(false);
        botonSiguiente.onClick.RemoveAllListeners();
        botonFinalizar.onClick.RemoveAllListeners();

        AudienceManager audienceManager = FindObjectOfType<AudienceManager>();
        if (audienceManager != null)
        {
            audienceManager.IsPaused = false;
            audienceManager.ResetAudience();
        }

        GameTimer timer = FindObjectOfType<GameTimer>();
        if (timer != null)
            timer.StartTimerManual();
    }

    void ActualizarBotones()
    {
        botonSiguiente.gameObject.SetActive(true);
        botonFinalizar.gameObject.SetActive(false);
    }
}