using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Input.GetKeyDown(KeyCode.Space) && GameObject.Find("enemy(Clone)") == null || 
            Input.GetKeyDown(KeyCode.Space) && GameObject.Find("Square(Clone)") == null) 
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator Spawn()
    {
        yield return null;
        spawnedPrefabPrefab = Instantiate(spawnPrefab, transform.position + spawnPos, Quaternion.identity);

        if(spawnedPrefabPrefab.GetComponent<AimingInput2>() == null) GameObject.Find("Square(Clone)").GetComponent<AimingInput2>().initPlayer();
    }
}
