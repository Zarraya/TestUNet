using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    public float AngleThreashold = 3;
    public float SpeedBase = 8;

    //global direction variables for movement. Only right and up are needed. Use negation for left and down.
    private readonly Vector3 _upAxis = new Vector3(-0.5f, 0, -0.5f);
    private readonly Vector3 _rightAxis = new Vector3(-0.5f, 0, 0.5f);
    private Constants.Direction lastDirection = Constants.Direction.up;

    //character instance specific variables
    private StaminaManager _staminaManager = null;

    public override void OnStartLocalPlayer()
    {
        GameObject.Find("NetworkManager").GetComponent<GameState>().GameMode = Constants.GameMode.match;

        GetComponent<MeshRenderer>().material.color = Color.cyan;
        //StaminaText = transform.parent.gameObject.GetComponent<Canvas>().GetComponent<Text>();
        _staminaManager = GetComponent<StaminaManager>();
    }

    // Update is called once per frame
    void Update () {

        //ensure that all following code will be for the users player and not the others on the network.
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

        //handle sprint functionality
        if(_staminaManager.Stamina > 9.9 && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            speed *= 2;
            _staminaManager.Decrease( 30 * Time.deltaTime) ;
        }
        
        //handle input direction. This code may be adjusted in the future. It currently will only support 8 axis movement.
        //note that there appears to be a floaty feeling when moving. This may be due to my keyboard.
        //I was forced to use transform.position because transform.translate causes issues when the characters forward
        //vector is locked to the mouse position.
        if (Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") > 0)
        {
            transform.position = transform.position + ((_rightAxis + _upAxis).normalized * speed * Time.deltaTime);
            lastDirection = Constants.Direction.upright;
        }
        else if (Input.GetAxis("Horizontal") < 0 && Input.GetAxis("Vertical") > 0)
        {
            transform.position = transform.position + ((-_rightAxis + _upAxis).normalized * speed * Time.deltaTime);
            lastDirection = Constants.Direction.upleft;
        }
        else if (Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") < 0)
        {
            transform.position = transform.position + ((_rightAxis + -_upAxis).normalized * speed * Time.deltaTime);
            lastDirection = Constants.Direction.downright;
        }
        else if (Input.GetAxis("Horizontal") < 0 && Input.GetAxis("Vertical") < 0)
        {
            transform.position = transform.position + ((-_rightAxis + -_upAxis).normalized * speed * Time.deltaTime);
            lastDirection = Constants.Direction.downleft;
        }
        else if(Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") == 0)
        {
            transform.position = transform.position + (_rightAxis * speed * Time.deltaTime);
            lastDirection = Constants.Direction.right;
        }
        //note that the right axis is not negated. Instead it is subtracted. this is more efficient than negating a vector then adding.
        else if(Input.GetAxis("Horizontal") < 0 && Input.GetAxis("Vertical") == 0)
        {
            transform.position = transform.position - (_rightAxis * speed * Time.deltaTime);
            lastDirection = Constants.Direction.left;
        }
        else if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") > 0)
        {
            transform.position = transform.position + (_upAxis * speed * Time.deltaTime);
            lastDirection = Constants.Direction.up;
        }
        else if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") < 0)
        {
            transform.position = transform.position - (_upAxis * speed * Time.deltaTime);
            lastDirection = Constants.Direction.down;
        }
        

        if (Input.GetMouseButtonDown(0))
        {
            CmdFire();
        }

        if (Input.GetMouseButtonDown(1))
        {
            NinjaRoll();
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


    public bool NinjaRoll()
    {
        //get the direction to move in based on the last movement direction
        Vector3 moveDir = Vector3.zero;

        switch (lastDirection)
        {
            case Constants.Direction.up:
                moveDir = _upAxis;
                break;
            case Constants.Direction.down:
                moveDir = -_upAxis;
                break;
            case Constants.Direction.left:
                moveDir = -_rightAxis;
                break;
            case Constants.Direction.right:
                moveDir = _rightAxis;
                break;
            case Constants.Direction.upleft:
                moveDir = (_upAxis + (-_rightAxis)).normalized;
                break;
            case Constants.Direction.upright:
                moveDir = (_upAxis + _rightAxis).normalized;
                break;
            case Constants.Direction.downleft:
                moveDir = (-_upAxis + (-_rightAxis)).normalized;
                break;
            case Constants.Direction.downright:
                moveDir = (-_upAxis + (_rightAxis)).normalized;
                break;
        };

        //move the character the appropriate amount based on stamina
        if(_staminaManager.Stamina == 100)
        {
            transform.position = transform.position + (moveDir * 10);
            _staminaManager.Decrease(_staminaManager.Stamina);
            return true;
        }

        if(_staminaManager.Stamina >= 60 && _staminaManager.Stamina < 100)
        {
            transform.position = transform.position + (moveDir * 5);
            _staminaManager.Stamina = 0;
            return true;
        }

        if(_staminaManager.Stamina >= 30 && _staminaManager.Stamina < 60)
        {
            transform.position = transform.position + (moveDir * 3);
            _staminaManager.Stamina = 0;
            return true;
        }

        if(_staminaManager.Stamina >= 1 && _staminaManager.Stamina < 30)
        {
            transform.position = transform.position + (moveDir * 1);
            _staminaManager.Stamina = 0;
            return true;
        }

        if(_staminaManager.Stamina < 1)
        {
            _staminaManager.Stamina = 0;
            return true;
        }

        return false;
    }
    
}
