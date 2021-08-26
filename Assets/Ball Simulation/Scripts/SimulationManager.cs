using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    //Singleton
    public static SimulationManager instance;
    
    //Spawner properties
    public GameObject ballPrefab;
    [SerializeField] private Vector3 spawnRange = new Vector3(25,25,25);
    [SerializeField] private float timeBeetwenBallSpawn = 0.25f;
    [SerializeField] private float ballsLimit = 250;
    private ElapsedTimeChecker elapsedTimeChecker;
    [SerializeField] private Text ballCounterText;
    private int ballCounter;

    //Gravity properties
    public static bool inverseGravity;
    private bool gravityInversed;

    //Camera properties
    [SerializeField] private float cameraDistance = 50;

    private void Awake()
    {
        instance = this;
        elapsedTimeChecker = new ElapsedTimeChecker(timeBeetwenBallSpawn);
        Camera.main.transform.position = Vector3.zero + spawnRange / 2 + new Vector3(0, 0, -cameraDistance);
    }

    [ContextMenu("InverseGavity")]
    public void InverseGravity()
    {
        inverseGravity = !inverseGravity;
    }

    public void SpawnBall()
    {
        GameObject ball = Instantiate(ballPrefab, new Vector3(Random.Range(0, spawnRange.x), Random.Range(0, spawnRange.y), Random.Range(0, spawnRange.z)), Quaternion.identity, this.transform);
        ball.GetComponent<GravityBall>().ballNumber = ballCounter;
        ballCounter++;
        ballCounterText.text = ballCounter.ToString();
    }

    private void Update()
    {
        if (elapsedTimeChecker.CheckElapsedTime())
        {
            if(GravityBall.GravityBalls.Count < ballsLimit)
            {
                elapsedTimeChecker.StartCountTime();
                SpawnBall();
            }
            else if (!gravityInversed)
            {
                InverseGravity();
                GravityBall.canBallsConnect = false;
                gravityInversed = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Spawning area
        Gizmos.DrawCube(Vector3.zero + spawnRange/2, spawnRange);
    }
}
