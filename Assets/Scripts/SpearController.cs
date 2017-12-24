using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearController : MonoBehaviour {

    private bool _leftFirstCollision = false;

    private void OnCollisionExit(Collision collision)
    {
        _leftFirstCollision = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_leftFirstCollision)
        {
            Destroy(gameObject);
        }
    }
}
