using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    public float AngleThreashold = 3;
    private Vector3 _bottomVert;
    private Vector3 _bottom;
    public float speed = 8;

    private Vector3 _upAxis = new Vector3(-0.5f, 0, -0.5f);
    private Vector3 _downAxis = new Vector3(0.5f, 0, 0.5f);
    private Vector3 _leftAxis = new Vector3(0.5f, 0, -0.5f);
    private Vector3 _rightAxis = new Vector3(-0.5f, 0, 0.5f);

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.cyan;

        _bottomVert = Utils.GetBottomPoint(GetComponent<MeshFilter>().mesh);
    }

    

    // Update is called once per frame
    void Update () {

        //_bottom = GetComponent<Transform>().TransformPoint(_bottomVert);

        if (!isLocalPlayer)
        {
            return;
        }


        // Generate a plane that intersects the transform's position with an upwards normal.
        var playerPlane = new Plane(Vector3.up, transform.position);

        // Generate a ray from the cursor position
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane.
        // This will be the point that the object must look towards to be looking at the mouse.
        // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
        //   then find the point along that ray that meets that distance.  This will be the point
        //   to look at.
        var hitdist = 0.0f;
        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(ray, out hitdist))
        {
            // Get the point along the ray that hits the calculated distance.
            var targetPoint = ray.GetPoint(hitdist);

            // Determine the target rotation.  This is the rotation if the transform looks at the target point.
            var targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }

        //Debug.Log(GetAngleToMouse());

        //var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        //var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        //transform.Rotate(0, GetAngleToMouse(), 0);
        //transform.Translate(0, 0, z);
        
        //todo transform the local transfomr to world space before the calculation
        if(Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") == 0)
        {
            transform.Translate(_rightAxis * speed * Time.deltaTime);
        }
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
    }

    [Command]
    void CmdFire()
    {
        //BulletPrefab.transform.Rotate(90f, 0f, 0f);
        GameObject bullet = Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * 15;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }

    float GetAngleToMouse()
    {
        

        //get the forward vector in screen space
        Vector3 forward = Camera.main.WorldToScreenPoint(GetComponent<Transform>().forward.normalized);
        Vector3 mouse = (Input.mousePosition - Camera.main.WorldToScreenPoint(_bottom.normalized));

        float angle = Vector3.Angle(forward, mouse);
        Debug.Log(GetComponent<Transform>().forward.normalized);

        if(angle <= AngleThreashold)
        {
            return 0;
        }

        return angle;
    }
}
