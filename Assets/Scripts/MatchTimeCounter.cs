using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchTimeCounter : MonoBehaviour {

    private GameState _gameState = null;

	// Use this for initialization
	void Start () {

        _gameState = GameObject.Find("NetworkManager").GetComponent<GameState>();
    }
	
	// Update is called once per frame
	void Update () {

        int minutes = (int)(_gameState.timeLeft / 60);
        int seconds = (int)(_gameState.timeLeft - (minutes * 60));

        GetComponent<Text>().text = "Time Left: " + minutes + ":" + seconds;
	}
}
