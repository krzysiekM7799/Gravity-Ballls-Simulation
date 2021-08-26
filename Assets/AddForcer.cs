using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForcer : MonoBehaviour
{
    Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    [ContextMenu("addforce")]
    public void AddForce()
    {
        _rigidbody.AddForce(Vector3.forward* 20);
    }
    [ContextMenu("InverseGavity")]
    public void InverseGravity()
    {
        SimulationManager.inverseGravity = !SimulationManager.inverseGravity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
