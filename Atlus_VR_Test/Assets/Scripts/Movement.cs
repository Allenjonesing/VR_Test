// This script moves your player automatically in the direction he is looking at. You can 
// activate the autowalk function by pull the cardboard trigger, by define a threshold angle 
// or combine both by selecting both of these options.
// The threshold is an value in degree between 0° and 90°. So for example the threshold is 
// 30°, the player will move when he is looking 31° down to the bottom and he will not move 
// when the player is looking 29° down to the bottom. This script can easally be configured
// in the Unity Inspector.Attach this Script to your CardboardMain-GameObject. If you 
// haven't the Cardboard Unity SDK, download it from https://developers.google.com/cardboard/unity/download

// Altered 10/18 by JonesingGames for Atlas VR Test. Changed cardboard to GvrViewer.Instance 
// and Cardboardhead to GvrHead. Changed name of class to Movement.
using UnityEngine;
using System.Collections;



public class Movement : MonoBehaviour
{
    private const int RIGHT_ANGLE = 90;

    // This variable determinates if the player will move or not 
    private bool isWalking = false;

    //public GameObject head;// = null;
    GvrHead head = null;

    //This is the variable for the player speed
    [Tooltip("With this speed the player will move.")]
    public float speed;

    [Tooltip("Activate this checkbox if the player shall move when the Cardboard trigger is pulled.")]
    public bool walkWhenTriggered;

    [Tooltip("Activate this checkbox if the player shall move when he looks below the threshold.")]
    public bool walkWhenLookDown;

    [Tooltip("This has to be an angle from 0° to 90°")]
    public double thresholdAngle;

    [Tooltip("Activate this Checkbox if you want to freeze the y-coordiante for the player. " +
             "For example in the case of you have no collider attached to your CardboardMain-GameObject" +
             "and you want to stay in a fixed level.")]
    public bool freezeYPosition;

    [Tooltip("This is the fixed y-coordinate.")]
    public float yOffset;

    [Tooltip("when true, the camera follows the height of the terrain")]
    public bool standOnGround;

    [Tooltip("The tallest peak in the terrain")]
    public float highestPointOfTerrain;

    [Tooltip("How tall the player is in unity units")]
    public float heightToAdd;

    [Tooltip("Additional height for saftey")]
    public float safeHeight;

    [Tooltip("Checks for walls")]
    public bool collideWithThings;

    [Tooltip("how far to come to a wall before stopping")]
    public float maxDistanceToWall;
    void Start()
    {
        head = GetComponentInChildren<StereoController>().Head;
       // head = ;
    }

    void Update()
    {
        // Walk when the Cardboard Trigger is used 
        if (walkWhenTriggered && !walkWhenLookDown && !isWalking && GvrViewer.Instance.Triggered)
        {
            isWalking = true;
        }
        else if (walkWhenTriggered && !walkWhenLookDown && isWalking && GvrViewer.Instance.Triggered)
        {
            isWalking = false;
        }

        // Walk when player looks below the threshold angle 
        if (walkWhenLookDown && !walkWhenTriggered && !isWalking &&
            head.transform.eulerAngles.x  >= thresholdAngle &&
            head.transform.eulerAngles.x <= RIGHT_ANGLE)
        {
            isWalking = true;
        }
        else if (walkWhenLookDown && !walkWhenTriggered && isWalking &&
                 (head.transform.eulerAngles.x <= thresholdAngle ||
                 head.transform.eulerAngles.x >= RIGHT_ANGLE))
        {
            isWalking = false;
        }

        // Walk when the Cardboard trigger is used and the player looks down below the threshold angle
        if (walkWhenLookDown && walkWhenTriggered && !isWalking &&
            head.transform.eulerAngles.x >= thresholdAngle &&
            GvrViewer.Instance.Triggered &&
            head.transform.eulerAngles.x <= RIGHT_ANGLE)
        {
            isWalking = true;
        }
        else if (walkWhenLookDown && walkWhenTriggered && isWalking &&
                 head.transform.eulerAngles.x >= thresholdAngle &&
                 (GvrViewer.Instance.Triggered ||
                 head.transform.eulerAngles.x >= RIGHT_ANGLE))
        {
            isWalking = false;
        }

        if (standOnGround)
        {
            RaycastHit hit;
            //var hit = Physics.Raycast;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                var distancetoground = hit.distance;
                if (distancetoground > heightToAdd)
                {
                    //var heightToAdd = transform.localScale.y;
                    transform.position = new Vector3(transform.position.x, (transform.position.y - distancetoground + heightToAdd + safeHeight), transform.position.z);
                }
                else if (distancetoground < heightToAdd * 0.8)
                {
                    isWalking = false;
                    transform.position = new Vector3(transform.position.x, (transform.position.y + (heightToAdd - distancetoground) + safeHeight), transform.position.z);
                }
                else if (distancetoground < heightToAdd)
                {

                    transform.position = new Vector3(transform.position.x, (transform.position.y + (heightToAdd - distancetoground) + safeHeight), transform.position.z);
                }
            }
        }


        if (collideWithThings)
        {
            RaycastHit hit;
            //var hit = Physics.Raycast;
            if (Physics.Raycast(transform.position, new Vector3(head.transform.forward.x, 0, head.transform.forward.z).normalized, out hit, maxDistanceToWall))
            {
                isWalking = false;
            }
        }

        if (isWalking)
        {
            Vector3 direction = new Vector3(head.transform.forward.x, 0, head.transform.forward.z).normalized * speed * Time.deltaTime;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, -transform.rotation.eulerAngles.y, 0));
            transform.Translate(rotation * direction);
        }

        if (freezeYPosition)
        {
            transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        }

        

    }
}