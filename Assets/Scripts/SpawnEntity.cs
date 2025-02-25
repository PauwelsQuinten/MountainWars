using System.Collections;
using UnityEngine;

public class SpawnEntity : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private GameObject spawnPrefab;
    void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return null;
        Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
    }
}
