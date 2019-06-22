using JoyconLib;
using System.Collections.Generic;
using UnityEngine;
using Htw.Cave.Joycon;
using Htw.Cave.Projector;
using Htw.Cave.Kinect;

public class ShootController : MonoBehaviour
{
    [Tooltip("Prefab for the Ammunition")]
    public GameObject Ammunition;
    //[Tooltip("The sound playing when a ball is thrown")]
    //public GameObject ThrowSound;
    [Tooltip("Define how many balls should persist")]
    public int maxBalls = 300;
    [HideInInspector]
    public static float speed = 600;
    [Tooltip("Crosshair Prefab")]
    public GameObject crosshairPrefab;

    private Queue<GameObject> ballQueue;

    //Kinect
    private KinectActor kinectActor;
    //Crosshair
    private GameObject leftCrosshair;
    private GameObject rightCrosshair;
    //Offsets
    private float playerOffsetY = 0.7f;

    public void Start()
    {
        ballQueue = new Queue<GameObject>();

        leftCrosshair = Instantiate(crosshairPrefab, transform);
        leftCrosshair.transform.position = new Vector3(0, -100, 0);
        rightCrosshair = Instantiate(crosshairPrefab, transform);
        rightCrosshair.transform.position = new Vector3(0, -100, 0);

        kinectActor = GetComponent<KinectActor>();
    }

    public void Update()
    {
        // do mouse and keyboard
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowBall(null);
        }

        //do joycons
        //Aim
        if (JoyconHelper.GetJoyconsCount() == 0)
            return;

        Joycon left = JoyconHelper.GetLeftJoycon();
        Joycon right = JoyconHelper.GetRightJoycon();

        if (left != null)
        {
            if(kinectActor != null)
            {
                Vector3 leftElbow = kinectActor.GetJointPosition(Windows.Kinect.JointType.ElbowLeft);
                Vector3 leftWrist = kinectActor.GetJointPosition(Windows.Kinect.JointType.WristLeft);
                Vector3 direction = Vector3.Normalize(leftWrist - leftElbow);
                direction = transform.root.TransformDirection(direction);
                leftCrosshair.transform.GetChild(1).transform.forward = direction;
            }
            else
            {
                kinectActor = GetComponent<KinectActor>();
            }

            if (left.GetButton(Joycon.Button.SHOULDER_1))
            {
                PlaceLaser(left, leftCrosshair);
            }
            if (left.GetButtonUp(Joycon.Button.SHOULDER_1))
            {
                leftCrosshair.transform.position = new Vector3(0, -100, 0);
            }
            //Shoot
            if (left.GetButtonDown(Joycon.Button.SHOULDER_2))
            {

                left.SetRumble(160.0f, 320.0f, 0.6f, 150);
                ThrowBall(left);
            }
        }

        if (right != null)
        {
            if(kinectActor != null)
            {
                Vector3 rightElbow = kinectActor.GetJointPosition(Windows.Kinect.JointType.ElbowRight);
                Vector3 rightWrist = kinectActor.GetJointPosition(Windows.Kinect.JointType.WristRight);
                Vector3 direction = Vector3.Normalize(rightWrist - rightElbow);
                direction = transform.root.TransformDirection(direction);
                rightCrosshair.transform.GetChild(1).transform.forward = direction;
            }
            else
            {
                kinectActor = GetComponent<KinectActor>();
            }

            if (right.GetButton(Joycon.Button.SHOULDER_1))
            {
                PlaceLaser(right, rightCrosshair);
            }
            if (right.GetButtonUp(Joycon.Button.SHOULDER_1))
            {
                rightCrosshair.transform.position = new Vector3(0, -100, 0);
            }
            //Shoot
            if (right.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
                right.SetRumble(160.0f, 320.0f, 0.6f, 150);
                ThrowBall(right);
            }
        }
    }



    private void PlaceLaser(Joycon joy, GameObject crosshair)
    {
        Transform shootdirection = crosshair.transform.GetChild(1).transform;
        RaycastHit hit;
        Vector3 start = transform.position;
        start.y += playerOffsetY;
        start += shootdirection.forward;
        if (Physics.Raycast(start, shootdirection.forward, out hit, Mathf.Infinity))
        {
            crosshair.transform.GetChild(0).transform.rotation = Quaternion.Euler(hit.normal);
            crosshair.transform.position = hit.point;
        }
    }

    private void ThrowBall(Joycon joy)
    {
        if (ballQueue.Count >= maxBalls)
        {
            GameObject ball = ballQueue.Dequeue();
            ball.GetComponent<TennisballBehaviour>().Disabled = false;
            ball.GetComponent<TennisballBehaviour>().MakeSound();
            ball.transform.SetPositionAndRotation(GetPlayerBallSpawnPosition(joy), transform.rotation);
            ball.GetComponent<Rigidbody>().AddForce(GetForce(joy));
            ballQueue.Enqueue(ball);
        }
        else
        {
            GameObject go = Instantiate(Ammunition, GetPlayerBallSpawnPosition(joy), transform.rotation);
            go.GetComponent<TennisballBehaviour>().MakeSound();
            go.GetComponent<Rigidbody>().AddForce(GetForce(joy));
            ballQueue.Enqueue(go);
        }
    }

    private Vector3 GetPlayerBallSpawnPosition(Joycon joy)
    {
        Vector3 position = transform.position;

        //always add a little forward offset so the player is not pushed back by force
        position += transform.root.forward;
        if(joy != null)
        {
            if (joy.isLeft)
            {
                position = kinectActor.GetJointPosition(Windows.Kinect.JointType.HandLeft);
                position = transform.root.TransformPoint(position);
            }
            else
            {
                position = kinectActor.GetJointPosition(Windows.Kinect.JointType.HandRight);
                position = transform.root.TransformPoint(position);
            }

        }

        return position;
    }

    private Vector3 GetForce(Joycon joy)
    {
        Vector3 force;
        force = transform.root.forward;
        force.y = 0.3f;
        if (joy != null && kinectActor != null)
        {
            if (joy.isLeft)
            {
                force = leftCrosshair.transform.GetChild(1).transform.forward;
            }
            else
            {
                force = rightCrosshair.transform.GetChild(1).transform.forward;
            }
        }

        force *= speed;
        return force;
    }
}
