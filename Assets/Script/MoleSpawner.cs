using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoleSpawner : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject molePrefab;

    [Header("Trous (trou1 à trou9)")]
    [SerializeField] private Transform[] trous; // Assigne trou1–trou9 dans l'Inspector

    [Header("Timing")]
    [SerializeField] private float intervaleSpawn = 1.5f;  // Temps entre chaque spawn
    [SerializeField] private float dureeVisible = 2f;      // Temps avant que la mole disparaisse

    [Header("Difficulté")]
    [SerializeField] private int maxMolesSimultanees = 3;

    private List<int> trouxDisponibles = new List<int>();
    private bool[] trouOccupe;
    private bool actif = false;

    void Start()
    {
        trouOccupe = new bool[trous.Length];
    }

    public void Demarrer()
    {
        actif = true;
        StartCoroutine(BoucleSpawn());
    }

    public void Arreter()
    {
        actif = false;
        StopAllCoroutines();
    }

    private IEnumerator BoucleSpawn()
    {
        while (actif)
        {
            yield return new WaitForSeconds(intervaleSpawn);

            int molesActives = 0;
            for (int i = 0; i < trouOccupe.Length; i++)
                if (trouOccupe[i]) molesActives++;

            if (molesActives < maxMolesSimultanees)
                SpawnerMole();
        }
    }

    private void SpawnerMole()
    {
        // Construire la liste des trous libres
        trouxDisponibles.Clear();
        for (int i = 0; i < trous.Length; i++)
            if (!trouOccupe[i]) trouxDisponibles.Add(i);

        if (trouxDisponibles.Count == 0) return;

        int index = trouxDisponibles[Random.Range(0, trouxDisponibles.Count)];
        trouOccupe[index] = true;

        // Spawn au-dessus du cylindre (ajuste le offset Y selon ta scène)
        Vector3 positionSpawn = trous[index].position + Vector3.up * -0.1f;
        GameObject obj = Instantiate(molePrefab, positionSpawn, trous[index].rotation);

        Mole mole = obj.GetComponent<Mole>();
        if (mole != null) mole.SortirTrou();

        StartCoroutine(GererDureeVie(obj, index, mole));
    }

    private IEnumerator GererDureeVie(GameObject obj, int index, Mole mole)
    {
        float elapsed = 0f;

        // Attendre que la mole soit frappée OU que le délai expire
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