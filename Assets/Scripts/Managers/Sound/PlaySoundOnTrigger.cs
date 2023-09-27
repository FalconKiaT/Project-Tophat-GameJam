using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject AudioContainer;
    private AudioSource _audio;

    void Start()
    {
        _audio = AudioContainer.GetComponent<AudioSource>();
    }

    public void TriggerSound()
    {
        SoundManager.Instance.PlaySound(_audio.clip);
    }

}
