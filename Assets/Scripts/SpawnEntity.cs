using System.Collections;
using UnityEngine;

public class SpawnEntity : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private bool _canSpawnMore;
    void Start()
    {
        StartCoroutine(Spawn());
    }

    private void Update()
    {
        if (!_canSpawnMore) return;
        if(Input.GetKeyDown(KeyCode.Space)) StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return null;
        Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
    }
}
