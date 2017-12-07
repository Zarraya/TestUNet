using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public GameObject BulletPrefab;
    public Transform BulletSpawn;

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.cyan;
    }

    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer)
        {
            return;
        }

        Quaternion rotation = GetAngleToMouse();

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
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

    Quaternion GetAngleToMouse()
    {
        Transform trans = GetComponent<Transform>();



        return new Quaternion();
    }
}
