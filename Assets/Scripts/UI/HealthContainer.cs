using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthContainer : MonoBehaviour
{
    public GameObject hitPoint1;
    public GameObject hitPoint2;
    public GameObject hitPoint3;
    public Sprite emptyHit;
    public Sprite fullHit;
    private int hitPoints;

    //Debugging vars
    public bool testHit;

    // Awake is called when an object with this script is initialized
    void Awake()
    {
        testHit = false;
        //Set the images on object instance
        hitPoint1.GetComponent<Image>().sprite = fullHit;
        hitPoint2.GetComponent<Image>().sprite = fullHit;
        hitPoint3.GetComponent<Image>().sprite = fullHit;
    }

    // Update is called once per frame
    void Update()
    {
        this.hitPoints = PlayerStats.player.GetHitPoints();
        //FIXME: Shouldnt have to draw every frame, only on update on hits but oh well
        drawHealth(this.hitPoints);

        //Debugging
        if (testHit)
        {
            PlayerStats.player.TakeHit();
            testHit = false;
        }
    }

    //Function that draws the health of the UI
    private void drawHealth(int var)
    {
        switch (hitPoints)
        {
            case 3:
                //3 hits left
                hitPoint1.GetComponent<Image>().sprite = fullHit;
                hitPoint2.GetComponent<Image>().sprite = fullHit;
                hitPoint3.GetComponent<Image>().sprite = fullHit;
                break;
            case 2:
                //2 Hits left
                hitPoint1.GetComponent<Image>().sprite = emptyHit;
                hitPoint2.GetComponent<Image>().sprite = fullHit;
                hitPoint3.GetComponent<Image>().sprite = fullHit;
                break;
            case 1:
                //1 Hit left
                hitPoint1.GetComponent<Image>().sprite = emptyHit;
                hitPoint2.GetComponent<Image>().sprite = emptyHit;
                hitPoint3.GetComponent<Image>().sprite = fullHit;
                break;
            case 0:
                //No hits left, should have died
                hitPoint1.GetComponent<Image>().sprite = emptyHit;
                hitPoint2.GetComponent<Image>().sprite = emptyHit;
                hitPoint3.GetComponent<Image>().sprite = emptyHit;
                Debug.Log("WARNING! SHOULD BE DEAD BY NOW!");
                break;
            default:
                //Debug Error message for unexpected cases
                Debug.Log("WARNING! DEFAULT CASE TRIGGERED IN HIT DISPLAY SCRIPT");
                break;
        }
    }
}
