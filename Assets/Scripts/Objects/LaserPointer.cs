using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    //Sound of laser
    public GameObject laserChargeSound;
    public GameObject laserFireSound;
    private AudioSource _charge;
    private AudioSource _fire;

    // Line OF Renderer
    public LineRenderer LineOfSight;

    public int reflections; //Max amount of reflections
    public float MaxRayDistance;
    public LayerMask LayerDetection;
    private PlayerController PlayerController;
    private Transform placeholder;
    public GameObject player;
    public bool flipLaser;
    private bool isMaterialized;
    bool isMirror;
    private float chargingDuration = 3f;

    private void Awake()
    {
        Physics2D.queriesStartInColliders = false; //Disables collisions inside starting colliders
        LineOfSight = GetComponent<LineRenderer>();
        PlayerController = player.GetComponent<PlayerController>();
        _fire = laserFireSound.GetComponent<AudioSource>();
        _charge = laserChargeSound.GetComponent<AudioSource>();

        //Had to figure out a way to stop outside lasers from firing :(
        if (this.transform.position.x < 2.9)
        {
            StartCoroutine(ChargeLaser());
        }
    }

    private void Update()
    {
        //Had to figure out a way to stop outside lasers from firing :(
        if (this.transform.position.x < 2.9)
        {
            DrawLaser(isMaterialized);
        }
    }

    private IEnumerator ChargeLaser()
    {
        //Draw the path of the laser first
        isMaterialized = false;
        _charge.Play();
        yield return new WaitForSeconds(chargingDuration);
        _charge.Stop();
        isMaterialized = true;
        _fire.Play();
    }

    private void DrawLaser(bool materialized)
    {
        //set the material of the line depending on state
        if (materialized)
        {
            LineOfSight.startColor = new Color(1, 0, 0, 1);
            LineOfSight.endColor = new Color(1, 0, 0, 1);
        }
        else
        {
            LineOfSight.startColor = new Color(1, 1, 1, 0.3f);
            LineOfSight.endColor = new Color(1, 1, 1, 0.3f);
        }
        
        
        LineOfSight.positionCount = 1;
        LineOfSight.SetPosition(0, transform.position); //Where the laser shoots from

        RaycastHit2D hitInfo;
        if (flipLaser)
        {
            hitInfo = Physics2D.Raycast(transform.position, transform.right * -1, MaxRayDistance, LayerDetection);
        }
        else
        {
            hitInfo = Physics2D.Raycast(transform.position, transform.right, MaxRayDistance, LayerDetection);
        }
        // Ray
        Ray2D ray = new Ray2D(transform.position, transform.right);

        //Variables to check if raycast hit mirror
        isMirror = false;
        Vector2 mirrorHitPoint = Vector2.zero;
        Vector2 mirrorHitNormal = Vector2.zero;


        for (int i = 0; i < reflections; i++)
        {
            LineOfSight.positionCount += 1;

            if (hitInfo.collider != null)
            {
                LineOfSight.SetPosition(LineOfSight.positionCount - 1, hitInfo.point - ray.direction * -0.1f);

                isMirror = false;
                if (hitInfo.collider.CompareTag("Mirror")) //Check if hits against mirror
                {
                    mirrorHitPoint = (Vector2)hitInfo.point;
                    mirrorHitNormal = (Vector2)hitInfo.normal;
                    hitInfo = Physics2D.Raycast((Vector2)hitInfo.point - ray.direction * -0.1f, Vector2.Reflect(hitInfo.point - ray.direction * -0.1f, hitInfo.normal), MaxRayDistance, LayerDetection);
                    isMirror = true;
                }
                else if (hitInfo.collider.CompareTag("Player") && materialized) //Check if colliding with player
                {
                    //Dont register hit if dashing
                    if (PlayerController.isDashing)
                    {
                        break;
                    }
                    StartCoroutine(PlayerController.TookDamage(null));
                    break;
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (isMirror)
                {
                    LineOfSight.SetPosition(LineOfSight.positionCount - 1, mirrorHitPoint + Vector2.Reflect(mirrorHitPoint, mirrorHitNormal) * MaxRayDistance);
                    break;
                }
                else //Hit nothing within range with 1st raycast
                {
                    LineOfSight.SetPosition(LineOfSight.positionCount - 1, transform.position + transform.right * MaxRayDistance);
                    break;
                }
            }
        }
    }
}
