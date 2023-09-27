using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private GameObject newEnemy;
    private List<GameObject> enemysAlive = new List<GameObject>();

    public void SpawnEnemy()
    {
        newEnemy = Instantiate(enemyPrefab, this.transform.position, Quaternion.identity);
        enemysAlive.Add(newEnemy);
    }

    public void RemoveEnemy()
    {
        if (enemysAlive.Count > 0)
        {
            for (int i = 0; i < enemysAlive.Count; i++)
            {
                Destroy(enemysAlive[i]);
            }
            enemysAlive.Clear();
        }
        else
        {
            Debug.Log("WARNING! ATTEMPTED TO DELETE A NON EXISTANT ENEMY");
        }
    }
}
