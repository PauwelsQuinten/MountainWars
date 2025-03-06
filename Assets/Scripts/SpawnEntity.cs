using System.Collections;
using UnityEngine;

public class SpawnEntity : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private bool _canSpawnMore;
    private GameObject spawnedPrefabPrefab;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    private void Update()
    {
        if (!_canSpawnMore) return;
        if(Input.GetKeyDown(KeyCode.Space) && GameObject.Find("enemy(Clone)") == null) StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return null;
        spawnedPrefabPrefab = Instantiate(spawnPrefab, transform.position + spawnPos, Quaternion.identity);

        if(spawnedPrefabPrefab.GetComponent<AimingInput2>() != null) spawnedPrefabPrefab.GetComponent<AimingInput2>().initPlayer();
    }
}
