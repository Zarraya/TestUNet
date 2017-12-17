using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    public float AngleThreashold = 3;
    public float SpeedBase = 8;

    //global direction variables for movement. Only right and up are needed. Use negation for left and down.
    private readonly Vector3 _upAxis = new Vector3(-0.5f, 0, -0.5f);
    private readonly Vector3 _rightAxis = new Vector3(-0.5f, 0, 0.5f);

    //character instance specific variables
    private float _stamina = 100f;

    #region Getters And Setters
    public float Stamina
    {
        get
        {
            return _stamina;
        }
    }
    #endregion

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.cyan;
    }

    

    // Update is called once per frame
    void Update () {

        //ensure that all following code will be for the users player and not the others on the network.
        if (!isLocalPlayer)
        {
            return;
        }

        //add to stamina with the frame time taken into consideration.
        if (_stamina < 100)
        {
            _stamina += 2 * Time.deltaTime;
        }

        if(_stamina > 100)
        {
            _stamina = 100;
        }

        // Generate a plane that intersects the transform's position with an upwards normal.
        var playerPlane = new Plane(Vector3.up, transform.position);

        // Generate a ray from the cursor position
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane.
        // This will be the point that the object must look towards to be looking at the mouse.
        // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
        // then find the point along that ray that meets that distance.  This will be the point
        // to look at.
        var hitdist = 0.0f;
        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(ray, out hitdist))
        {
            // Get the point along the ray that hits the calculated distance.
            var targetPoint = ray.GetPoint(hitdist);

            // Determine the target rotation.  This is the rotation if the transform looks at the target point.
            transform.rotation = Quaternion.LookRotation(targetPoint - transform.position);
            
        }

        float speed = SpeedBase;

        Debug.Log(_stamina);

        //handle sprint functionality
        if(_stamina > 9.9 && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            speed *= 2;
            _stamina -= 30 * Time.deltaTime;
        }
        
        //handle input direction. This code may be adjusted in the future. It currently will only support 8 axis movement.
        //note that there appears to be a floaty feeling when moving. This may be due to my keyboard.
        //I was forced to use transform.position because transform.translate causes issues when the characters forward
        //vector is locked to the mouse position.
        if (Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") > 0)
        {
            transform.position = transform.position + ((_rightAxis + _upAxis).normalized * speed * Time.deltaTime);
        }
        else if (Input.GetAxis("Horizontal") < 0 && Input.GetAxis("Vertical") > 0)
        {
            transform.position = transform.position + ((-_rightAxis + _upAxis).normalized * speed * Time.deltaTime);
        }
        else if (Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") < 0)
        {
            transform.position = transform.position + ((_rightAxis + -_upAxis).normalized * speed * Time.deltaTime);
        }
        else if (Input.GetAxis("Horizontal") < 0 && Input.GetAxis("Vertical") < 0)
        {
            transform.position = transform.position + ((-_rightAxis + -_upAxis).normalized * speed * Time.deltaTime);
        }
        else if(Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") == 0)
        {
            transform.position = transform.position + (_rightAxis * speed * Time.deltaTime);
        }
        //note that the right axis is not negated. Instead it is subtracted. this is more efficient than negating a vector then adding.
        else if(Input.GetAxis("Horizontal") < 0 && Input.GetAxis("Vertical") == 0)
        {
            transform.position = transform.position - (_rightAxis * speed * Time.deltaTime);
        }
        else if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") > 0)
        {
            transform.position = transform.position + (_upAxis * speed * Time.deltaTime);
        }
        else if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") < 0)
        {
            transform.position = transform.position - (_upAxis * speed * Time.deltaTime);
        }
        

        if (Input.GetMouseButtonDown(0))
        {
            CmdFire();
        }
    }

    [Command]
    void CmdFire()
    {
        GameObject bullet = Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * 15;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }
    
}
