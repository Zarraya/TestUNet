using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class GameState : NetworkBehaviour {

    public TimeSpan GameLength = new TimeSpan(0,5,0);
    public readonly DateTime GameStartTime = DateTime.Now;
    public float timeLeft = 0;
    public Constants.GameMode GameMode = Constants.GameMode.menu;

	// Use this for initialization
	void Start () {

        Network.sendRate = 0;

        timeLeft += (float)GameLength.TotalSeconds;
	}
	
	// Update is called once per frame
	void Update () {

        if(GameMode != Constants.GameMode.menu)
        {
            timeLeft -= Time.deltaTime;
        }
    }
}
