using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawner : MonoBehaviour
{
    public GameObject targetSpawner;
    private EnemySpawner spawnerScript;

    private void Start()
    {
        spawnerScript = targetSpawner.GetComponent<EnemySpawner>();
    }

    public void SpawnTrigger()
    {
        spawnerScript.SpawnEnemy();
    }

    public void KillSpawner()
    {
        spawnerScript.RemoveEnemy();
    }
}
