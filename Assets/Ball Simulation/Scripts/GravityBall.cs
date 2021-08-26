using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class GravityBall : MonoBehaviour
{
    //Ball properties
    [SerializeField] private float ballRadius = 1;
    private float ballField;
    public float ballNumber;
    private float startedBallRadius;
    private float startedBallMass;

    //Eating properties
    private int eatenBallsCount = 1;
    [SerializeField] private int maximumNumberOfBallsToEat = 15;

    //Permeability time after burst
    [SerializeField] private float permeableTime = 0.5f;
    private Vector3 maxForceAfterBallDestroy = new Vector3(2, 2, 2);

    //Air resistance properties
    private float densityOfAir = 1.225f;
    private float sphereDragCoefficient = .47f;
    private float sphereSectionalArea;

    //Determines whether the balls can connect with others
    public static bool canBallsConnect = true;
    
    public static float gravitationalConstant = 6.67f;

    //List of balls
    public static List<GravityBall> GravityBalls = new List<GravityBall>();

    //Needed Components
    private SphereCollider sphereCollider;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;

        sphereSectionalArea = CountSphereSectionalAreathod();

        ballField = CountSphereField(ballRadius);

        startedBallRadius = ballRadius;
        startedBallMass = _rigidbody.mass;

        SetStartValues();

        GravityBalls.Add(this);
    }

    void FixedUpdate()
    {
        foreach (var gravityBall in GravityBalls)
        {
            if (gravityBall != this)
            {
                Gravitate(gravityBall);
            }
        }

        HandleAirResitance();
    }

    public void SetStartValues()
    {
        ballRadius = startedBallRadius;
        ballField = CountSphereField(ballRadius);
        eatenBallsCount = 1;
        _rigidbody.mass = startedBallMass;
        transform.localScale = Vector3.one;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canBallsConnect)
        {
            GravityBall gravityBall = collision.transform.GetComponent<GravityBall>();
            HandleBallEating(gravityBall);
        }
    }

    private void HandleBallEating(GravityBall gravityBall)
    {
        if (gravityBall.ballRadius < ballRadius || (gravityBall.ballRadius == ballRadius && gravityBall.ballNumber > ballNumber))
        {
            if (eatenBallsCount < maximumNumberOfBallsToEat)
                EatBall(gravityBall);
            else
                BurstThisBall();
        }
    }

    private void OnDestroy()
    {
        GravityBalls.Remove(this);
    }

    public void EatBall(GravityBall gravityBall)
    {
        EnlargeBall(gravityBall.ballField, gravityBall._rigidbody.mass);
        gravityBall.transform.parent = transform;
        eatenBallsCount += gravityBall.eatenBallsCount;
        gravityBall.gameObject.SetActive(false);
    }

    public void EnlargeBall(float ballField, float ballMass)
    {
        this.ballField += ballField;
        ballRadius = CountSphereRadius(this.ballField);
        _rigidbody.mass += ballMass;
        sphereSectionalArea = CountSphereSectionalAreathod();
        transform.localScale = Vector3.one * ballRadius;
    }
  
    public void DistractThisBall()
    {
        sphereCollider.enabled = false;
        gameObject.SetActive(true);
        StartCoroutine(SetPermeable(permeableTime));
        SetStartValues();
        transform.parent = SimulationManager.instance.transform;
        _rigidbody.AddForce(new Vector3(Random.Range(-maxForceAfterBallDestroy.x, -maxForceAfterBallDestroy.x), Random.Range(-maxForceAfterBallDestroy.y, maxForceAfterBallDestroy.y), Random.Range(-maxForceAfterBallDestroy.z, maxForceAfterBallDestroy.z)));       
    }

    public void BurstThisBall()
    {
        GravityBall[] allChildren = GetComponentsInChildren<GravityBall>(true);

        if (allChildren != null)
        {
            foreach (GravityBall child in allChildren)
            {

                child.DistractThisBall();
            }
        }
    }

    IEnumerator SetPermeable(float time)
    {
        sphereCollider.enabled = false;
        yield return new WaitForSecondsRealtime(time);
        sphereCollider.enabled = true;
    }

    private void Gravitate(GravityBall gravityBall)
    {
        Vector3 direction = transform.position - gravityBall.transform.position;
        float distance = direction.magnitude;

        if (distance == 0f)
            return;

        float forceMagnitude = gravitationalConstant * (_rigidbody.mass * gravityBall._rigidbody.mass) / (distance * distance);
        Vector3 force = direction.normalized * forceMagnitude;

        if (SimulationManager.inverseGravity)
            force = -force;
        gravityBall._rigidbody.AddForce(force);
    }

    void HandleAirResitance()
    {
        var v = _rigidbody.velocity.magnitude;
        var direction = -_rigidbody.velocity.normalized;
        var forceAmount = (densityOfAir * v * v * sphereDragCoefficient * sphereSectionalArea) / 2;
        _rigidbody.AddForce(direction * forceAmount);
    }

    //The cross section of a sphere is a circle
    private float CountSphereSectionalAreathod()
    {
        return Mathf.PI * ballRadius * ballRadius;
    }

    public static float CountSphereField(float radius)
    {
        return 4 * Mathf.PI * radius * radius;
    }

    public static float CountSphereRadius(float field)
    {
        return Mathf.Sqrt(field / (4 * Mathf.PI));
    }
}
