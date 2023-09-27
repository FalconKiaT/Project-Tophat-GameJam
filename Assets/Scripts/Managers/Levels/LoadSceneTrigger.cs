using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneTrigger : MonoBehaviour
{

    public string SceneName;
    
    public void LoadBoss1()
    {
        SceneManager.LoadSceneAsync(SceneName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LoadBoss1();
    }

}
