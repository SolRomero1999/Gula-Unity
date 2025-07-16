using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelData
{
    public string nombre;
    public float duracion;
    public GameObject[] platillosDisponibles;
    public bool esTutorial;
    [TextArea(2, 4)]
    public string[] lineasTutorial;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Definición de niveles")]
    public LevelData[] niveles;

    [Header("Panel de calendario")]
    public GameObject panelCalendario;

    [Header("Nivel actual (solo lectura)")]
    public int nivelActual = 0;

    private Coroutine nivelCoroutine;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        IniciarNivel();
    }
    public void IniciarNivel()
    {
        if (nivelActual >= niveles.Length)
        {
            Debug.Log("No hay más niveles disponibles.");
            return;
        }

        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Platillo"))
            p.SetActive(false);

        foreach (GameObject platillo in niveles[nivelActual].platillosDisponibles)
            platillo.SetActive(true);

        if (panelCalendario != null)
            panelCalendario.SetActive(false);

        if (nivelCoroutine != null) StopCoroutine(nivelCoroutine);
        nivelCoroutine = StartCoroutine(TemporizadorNivel(niveles[nivelActual].duracion));

        AudienceManager audienceManager = FindObjectOfType<AudienceManager>();
        if (audienceManager != null)
        {
            if (niveles[nivelActual].esTutorial)
            {
                audienceManager.IsPaused = true; 
            }
            else
            {
                audienceManager.IsPaused = false; 
                audienceManager.ResetAudience();
            }
        }

        FoodUIManager foodUI = FindObjectOfType<FoodUIManager>();
        if (foodUI != null)
            foodUI.ResetStomach();

        GameTimer timer = FindObjectOfType<GameTimer>();
        if (timer != null)
        {
            timer.ResetTimer(niveles[nivelActual].duracion);

            if (!niveles[nivelActual].esTutorial)
                timer.StartTimerManual(); 
        }
    }

    private IEnumerator TemporizadorNivel(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        MostrarPanelCalendario();
    }

    public void MostrarPanelCalendario()
    {
        if (panelCalendario != null)
            panelCalendario.SetActive(true);
    }

    public void AvanzarNivel()
    {
        nivelActual++;
        IniciarNivel();
    }

    public void TerminarNivel()
    {
        MostrarPanelCalendario();
    }

}
