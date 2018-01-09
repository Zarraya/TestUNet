using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;

public class GameState : OsBase {

    public GameObject SpearPrefab = null;
    public TimeSpan GameLength = new TimeSpan(0,5,0);
    public readonly DateTime GameStartTime = DateTime.Now;
    public float timeLeft = 0;

    private Constants.GameMode _gameMode = Constants.GameMode.menu;
    private NetworkStartPosition[] _spearSpawns;

    public Constants.GameMode GameMode
    {
        get
        {
            return _gameMode;
        }
        set
        {
            _gameMode = value;
            FirePropertyChanged("GameMode");
        }
    }

	// Use this for initialization
	void Start ()
    {
        PropertyChanged += GameState_PropertyChanged;

        Network.sendRate = 0;

        timeLeft += (float)GameLength.TotalSeconds;

        _spearSpawns = FindObjectsOfType<NetworkStartPosition>().Where(o => o.gameObject.name.Contains("Spear")).ToArray();

        //GameObject.Find("RiggedCharacter").GetComponent<Animator>().Play("Grounded");
	}

    private void GameState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(e.PropertyName == "GameMode")
        {
            if((sender as GameState).GameMode == Constants.GameMode.match)
            {
                SpawnSpear();
            }
        }
    }

    // Update is called once per frame
    void Update () {

        if(GameMode != Constants.GameMode.menu)
        {
            timeLeft -= Time.deltaTime;
        }
    }

    public void SpawnSpear()
    {

        //todo create spawn locations with game objects and then get the list here. Choose one randomly with a rng.
        //potential idea: have a simple puzzle to get to the spear instead of it sitting out in the open.
        //Network.Instantiate(SpearPrefab);

        if(SpearPrefab != null)
        {
            Network.Instantiate(SpearPrefab, _spearSpawns[0].transform.position, _spearSpawns[0].transform.rotation, 0);
        }
    }
}
