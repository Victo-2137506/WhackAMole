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

    [Header("Timer")]
    [SerializeField] private int minutesDepart = 1;
    [SerializeField] private int secondesDepart = 0;

    [Header("Moles")]
    [SerializeField] private MoleSpawner moleSpawner;
    [SerializeField] private float intervaleDebut = 1.5f;
    [SerializeField] private float intervaleFin = 0.3f;

    private EtatJeu etatActuel;
    private float tempsRestant;
    private float tempsTotal;
    private bool timerActif;
    private int score;
    private float intervaleTimer = 0f;

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
    }

    void Update()
    {
        if (!timerActif) return;

        tempsRestant -= Time.deltaTime;

        if (tempsRestant <= 0f)
        {
            tempsRestant = 0f;
            AfficherTimer();
            TerminerJeu();
            return;
        }

        AfficherTimer();

        // Progression de 0 (début) à 1 (fin)
        float progression = 1f - (tempsRestant / tempsTotal);
        float intervaleActuel = Mathf.Lerp(intervaleDebut, intervaleFin, progression);

        intervaleTimer += Time.deltaTime;
        if (intervaleTimer >= intervaleActuel)
        {
            intervaleTimer = 0f;
            moleSpawner.SpawnerUneMole();
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
        tempsTotal = minutesDepart * 60f + secondesDepart;
        tempsRestant = tempsTotal;
        intervaleTimer = 0f;
        score = 0;
        texteScore.text = $"Score : {score}";
        timerActif = true;
        AfficherTimer();
        ChangerEtat(EtatJeu.EnJeu);
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