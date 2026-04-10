using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private bool[] trouOccupe;
    private bool actif = false;

    void Start()
    {
        trouOccupe = new bool[trous.Length];
    }

    public void Arreter()
    {
        actif = false;
        StopAllCoroutines();
    }

    public void SpawnerUneMole()
    {
        actif = true;

        // Compter les moles actives
        int molesActives = 0;
        for (int i = 0; i < trouOccupe.Length; i++)
            if (trouOccupe[i]) molesActives++;

        if (molesActives >= maxMolesSimultanees) return;

        // Trouver les trous disponibles
        List<int> trouxDisponibles = new List<int>();
        for (int i = 0; i < trous.Length; i++)
            if (!trouOccupe[i]) trouxDisponibles.Add(i);

        if (trouxDisponibles.Count == 0) return;

        // Choisir un trou aléatoire
        int index = trouxDisponibles[Random.Range(0, trouxDisponibles.Count)];
        trouOccupe[index] = true;

        // Spawner la mole
        Vector3 positionSpawn = trous[index].position + Vector3.up * offsetHauteur;
        GameObject obj = Instantiate(molePrefab, positionSpawn, trous[index].rotation);

        Mole mole = obj.GetComponent<Mole>();
        if (mole != null) mole.SortirTrou();

        StartCoroutine(GererDureeVie(obj, index, mole));
    }

    private IEnumerator GererDureeVie(GameObject obj, int index, Mole mole)
    {
        float elapsed = 0f;

        while (elapsed < dureeVisible && obj != null && mole != null && mole.estSortie)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        trouOccupe[index] = false;

        if (obj != null)
            Destroy(obj);
    }
}