using UnityEngine;
using TMPro;

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

    private EtatJeu etatActuel;
    private float tempsRestant;
    private bool timerActif;

    void Start()
    {
        ChangerEtat(EtatJeu.Menu);
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
                AfficherScore();
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
        AfficherTimer();
        AfficherScore();
        ChangerEtat(EtatJeu.EnJeu);
    }

    public void TerminerJeu()
    {
        timerActif = false;
        int score = CalculerScore();
        texteScoreFinal.text = $"Score : {score}";
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

    private void AfficherScore()
    {
        texteScore.text = $"Score : {CalculerScore()}";
    }

    private int CalculerScore()
    {
        float tempsEcoule = tempsDepart - tempsRestant;
        return Mathf.Max(100, 1000 - Mathf.FloorToInt(tempsEcoule) * 10);
    }
}