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

    private EtatJeu etatActuel;
    private float tempsEcoule;
    private bool timerActif;

    void Start()
    {
        ChangerEtat(EtatJeu.Menu);
    }

    void Update()
    {
        if (timerActif)
        {
            tempsEcoule += Time.deltaTime;
            AfficherTimer();
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
        tempsEcoule = 0f;
        timerActif = true;
        AfficherTimer();
        ChangerEtat(EtatJeu.EnJeu);
    }

    public void TerminerJeu()
    {
        timerActif = false;
        int score = Mathf.Max(100, 1000 - Mathf.FloorToInt(tempsEcoule) * 10);
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
        int minutes = Mathf.FloorToInt(tempsEcoule / 60f);
        int secondes = Mathf.FloorToInt(tempsEcoule % 60f);
        texteTimer.text = $"{minutes:00}:{secondes:00}";
    }
}