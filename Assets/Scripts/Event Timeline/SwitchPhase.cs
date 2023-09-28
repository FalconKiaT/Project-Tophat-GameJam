using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPhase : MonoBehaviour
{
    public GameObject phase1;
    public GameObject phase2;
    public GameObject phase3;
    public GameObject phaseChangeSound;
    private AudioSource _change;

    private void Awake()
    {
        _change = phaseChangeSound.GetComponent<AudioSource>();
    }


    public void TriggerPhase1()
    {
        phase1.SetActive(true);
        phase2.SetActive(false);
        phase3.SetActive(false);
    }

    public void TriggerPhase2()
    {
        phase1.SetActive(false);
        phase2.SetActive(true);
        phase3.SetActive(false);
        _change.Play();
    }

    public void TriggerPhase3()
    {
        phase1.SetActive(false);
        phase2.SetActive(false);
        phase3.SetActive(true);
        _change.Play();
    }
}
