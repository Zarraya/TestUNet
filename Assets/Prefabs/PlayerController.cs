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

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.cyan;

        _bottomVert = Utils.GetBottomPoint(GetComponent<MeshFilter>().mesh);
    }

    // Update is called once per frame
    void Update () {

        _bottom = GetComponent<Transform>().TransformPoint(_bottomVert);

        if (!isLocalPlayer)
        {
            return;
        }

        //Debug.Log(GetAngleToMouse());

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, GetAngleToMouse(), 0);
        transform.Translate(0, 0, z);

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
