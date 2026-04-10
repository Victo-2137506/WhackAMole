using UnityEngine;
using TMPro;

// Le code est inspiré des notes de cours : https://envimmersif-cegepvicto.github.io/exercice_ui_jeu_tri/

public class GameManager : MonoBehaviour
{
    public enum EtatJeu { Menu, EnJeu, GameOver }

    [Header("Canvas")]
    [SerializeField] private GameObject canvasMenu;
    [SerializeField] private GameObject canvasHUD;
    [SerializeField] private GameObject canvasGameOver;

    [Header("Textes")]
    [SerializeField] private TextMeshProUGUI texteTimer;
    [SerializeField] private TextMeshProUGUI texteScoreFinal;
    [SerializeField] private TextMeshProUGUI texteScore;

    [Header("Chronomètre")]
    [SerializeField] private float tempsDepart = 60f;

    [Header("Moles")]
    [SerializeField] private GameObject listeMoles;
    [SerializeField] private float intervaleDebut = 1.5f;
    [SerializeField] private float intervaleFin = 0.3f;

    [Header("Spawner")]
    [SerializeField] private MoleSpawner moleSpawner;

    private Mole[] moles;

    private EtatJeu etatActuel;
    private float tempsRestant;
    private bool timerActif;
    private int score;

    // Singleton de GameManager
    public static GameManager Instance { get; private set; }

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ChangerEtat(EtatJeu.Menu);
        moles = listeMoles.GetComponentsInChildren<Mole>();
    }

    void Update()
    {
        if (timerActif)
        {
            tempsRestant -= Time.deltaTime;

            if (tempsRestant <= 0f)
            {
                tempsRestant = 0f;
                timerActif = false;
                AfficherTimer();
                TerminerJeu();
            }
            else
            {
                AfficherTimer();
            }
        }
    }

    public void ChangerEtat(EtatJeu nouvelEtat)
    {
        etatActuel = nouvelEtat;
        canvasMenu.SetActive(etatActuel == EtatJeu.Menu);
        canvasHUD.SetActive(etatActuel == EtatJeu.EnJeu);
        canvasGameOver.SetActive(etatActuel == EtatJeu.GameOver);
    }

    public void CommencerJeu()
    {
        tempsRestant = tempsDepart;
        timerActif = true;
        score = 0;
        AfficherTimer();
        ChangerEtat(EtatJeu.EnJeu);
        moleSpawner.Demarrer();
    }

    public void CalculerPoints(int nbPoints)
    {
        score += nbPoints;
        texteScore.text = $"Score : {score}";
    }

    public void TerminerJeu()
    {
        timerActif = false;
        texteScoreFinal.text = $"Score : {score}";
        moleSpawner.Arreter();
        ChangerEtat(EtatJeu.GameOver);
    }

    public void Rejouer()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    private void AfficherTimer()
    {
        int minutes = Mathf.FloorToInt(tempsRestant / 60f);
        int secondes = Mathf.FloorToInt(tempsRestant % 60f);
        texteTimer.text = $"{minutes:00}:{secondes:00}";
    }
}