using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gère l'apparition des taupes dans les différents trous.
/// </summary>
public class MoleSpawner : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject molePrefab;

    [Header("Trous (trou1 à trou9)")]
    [SerializeField] private Transform[] trous;

    [Header("Timing")]
    [SerializeField] private float dureeVisible = 2f;
    [SerializeField] private float offsetHauteur = -0.1f;

    [Header("Difficulté")]
    [SerializeField] private int maxMolesSimultanees = 3;

    private bool[] trouOccupe; // Tableau indiquant quels trous sont occupés
    private bool actif = false; // Indique si le spawner est actif


    /// <summary>
    /// Initialisation du tableau des trous occupés.
    /// </summary>
    void Start()
    {
        trouOccupe = new bool[trous.Length];
    }

    /// <summary>
    /// Méthode qui permet d'arrêter complètement le système d'apparition
    /// </summary>
    public void Arreter()
    {
        actif = false;
        StopAllCoroutines();
    }

    /// <summary>
    /// Méthode permettant de faire apparaître une taupe dans un trou disponible.
    /// </summary>
    public void SpawnerUneMole()
    {
        actif = true;

        // Compter les moles actives
        int molesActives = 0;
        for (int i = 0; i < trouOccupe.Length; i++)
            if (trouOccupe[i]) molesActives++;

        // Si on atteint la limite, on ne spawn rien
        if (molesActives >= maxMolesSimultanees) return;

        // Trouver les trous disponibles
        List<int> trouxDisponibles = new List<int>();
        for (int i = 0; i < trous.Length; i++)
            if (!trouOccupe[i]) trouxDisponibles.Add(i);

        // Si aucun trou n'est libre, on arrête
        if (trouxDisponibles.Count == 0) return;

        // Choisir un trou aléatoire
        int index = trouxDisponibles[Random.Range(0, trouxDisponibles.Count)];
        trouOccupe[index] = true;

        // Calcul de la position de spawn de la taupe (code généré par ClaudeIA)
        Vector3 positionSpawn = trous[index].position + Vector3.up * offsetHauteur;
        GameObject obj = Instantiate(molePrefab, positionSpawn, trous[index].rotation);

        // Récupération du script Mole
        Mole mole = obj.GetComponent<Mole>();
        if (mole != null) mole.SortirTrou();

        StartCoroutine(GererDureeVie(obj, index, mole));
    }

    /// <summary>
    /// Gère la durée de vie d'une taupe.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="index"></param>
    /// <param name="mole"></param>
    /// <returns></returns>
    private IEnumerator GererDureeVie(GameObject obj, int index, Mole mole)
    {
        float tempsEcoule = 0f;

        // Tant que la durée n'est pas atteinte et que la taupe existe encore
        while (tempsEcoule < dureeVisible && obj != null && mole != null && mole.estSortie)
        {
            tempsEcoule += Time.deltaTime;
            yield return null;
        }

        // Libérer le trou
        trouOccupe[index] = false;

        // Détruire la taupe si elle existe encore
        if (obj != null)
            Destroy(obj);
    }
}