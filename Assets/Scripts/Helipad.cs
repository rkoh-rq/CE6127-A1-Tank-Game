using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helipad : MonoBehaviour
{
    //delegate
    public delegate void ObjectEnters(Collider collide);  // Function pointer that is cascader/implemented in another script
    public ObjectEnters objectEnter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Called when collide with other collider
    protected void OnTriggerEnter(Collider other)
    {
        objectEnter.Invoke(other);
    }
}
