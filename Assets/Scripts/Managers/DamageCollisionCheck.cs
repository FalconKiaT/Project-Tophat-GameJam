using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollisionCheck : MonoBehaviour
{
    public GameObject playerObject;
    Collider2D Collider;
    Collider2D playerCollider;
    PlayerController playerController;
    PlaySoundOnTrigger _PlayerHitSound;

    // Start is called before the first frame update
    void Start()
    {
        _PlayerHitSound = GetComponent<PlaySoundOnTrigger>();
        playerCollider = playerObject.GetComponent<Collider2D>();
        playerController = playerObject.GetComponent<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Check if object is player
        if (collision.gameObject.CompareTag("Player"))
        {
            //If player is dashing, ignore damage
            if (playerController.isDashing)
            {
                return;
            }

            //call the damage push animation, ignore if player is dead
            if (!PlayerStats.player.dead)
            {
                _PlayerHitSound.TriggerSound();
                StartCoroutine(playerController.TookDamage(this.transform));
            }

        }
    }
}
