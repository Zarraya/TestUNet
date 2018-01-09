using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearController : OsBase {

    private bool _leftFirstCollision = false;
    private Constants.SpearState _spearState = Constants.SpearState.dropped;
    private GameObject _holder = null;
    private Rigidbody _rigidBody;

    public Constants.SpearState SpearState
    {
        get
        {
            return _spearState;
        }
        set
        {
            _spearState = value;

            if(value == Constants.SpearState.dropped)
            {
                _rigidBody.useGravity = false;
            }
            else
            {
                _rigidBody.useGravity = true;
            }

            FirePropertyChanged("SpearState");
        }
    }

    public GameObject Holder
    {
        get
        {
            return _holder;
        }
        set
        {
            _holder = value;
            FirePropertyChanged("Holder");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _leftFirstCollision = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_leftFirstCollision)
        {
            //Destroy(gameObject);
        }
    }

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
    }
}
