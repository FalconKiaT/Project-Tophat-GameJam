using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class handles all outside interactions that need to alter
 * anything contained on PlayerStats and the UI.
 * Use PlayerStats.player.method to access anything here from within
 * other scripts around the game.
*/

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats player;
    public GameObject playerObject;
    //Player hit points
    public int hitPoints;
    public int ticketCount;
    public float iFramesSecs = 0.7f;
    public bool dead;

    void Start()
    {
        hitPoints = 3;
        ticketCount = 0;
        dead = false;
        //Creates a global reference so we can interact with the player stats from anywhere
        player = this;
        //TODO: WRITE STATS TO A FILE
    }

    //Function to call when player is hit, returns hit points left
    public int TakeHit()
    {
        hitPoints--;
        //Check for death
        if (hitPoints <= 0)
        {
            //Triggers for death animation and other events
            this.dead = true;
        }
        return hitPoints;
    }

    //Function that heals a hit for the player
    public int HealHit()
    {
        hitPoints++;
        return hitPoints;
    }

    //Function to call when player gains a ticker, returns tickets
    public int AddTicket()
    {
        ticketCount++;
        return ticketCount;
    }

    //Function that removes a ticket, just in case its needed
    public int RemoveTicket()
    {
        ticketCount--;
        return ticketCount;
    }

    //Getters and setters
    public int GetHitPoints()
    {
        return hitPoints;
    }

    public int GetTicketCount()
    {
        return ticketCount;
    }
    
    public void SetHitPoints(int var)
    {
        hitPoints = var;
    }

    public void SetTicketCount(int var)
    {
        ticketCount = var;
    }
}
