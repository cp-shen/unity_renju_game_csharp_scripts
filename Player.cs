using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour {
    private GameSceneTool _gameSceneTool;

    private bool _isBlack;
    public bool IsBlack {
        get { return _isBlack; }
    }

    [SerializeField] private bool _isBot = false;
    [SerializeField] private bool _isDoublePlayer = false;

    private GameDataHandler _gameDataHandler;

    private CursorController _cursor;
    public CursorController Cursor {
        get { return _cursor; }
    }

    private bool _isOnline;
    public bool IsOnline {
        get { return _isOnline; }
        set { _isOnline = value; }
    }

	// Use this for initialization
	void Awake () {
        _gameSceneTool = GameObject.Find("Game_Controller").GetComponent<GameSceneTool>();
        _gameDataHandler = GameDataHandler.Instance;
        _cursor = GameObject.Find("Cursor").GetComponent<CursorController>();
        _isOnline = false;

        if(_gameDataHandler.PlayerNum == 0) {
            _isBlack = true;
            _gameDataHandler.PlayerNum++;
        }
        else if(_gameDataHandler.PlayerNum == 1) {
            _isBlack = false;
            _gameDataHandler.PlayerNum++;
        }
        else {
            throw new MaxPlayerException();
        }
        Debug.Log("Player awaked, _isBlack = " + _isBlack);
	}

    public void PlayerAddStep(GameTrace gameTrace) {
        if ((_gameDataHandler.GameState != GameState.WhitePlay && gameTrace.player == "white")
            || (_gameDataHandler.GameState != GameState.BlackPlay && gameTrace.player == "black")) {
            return;
        }
        //Debug.Log("in PlayerAddStep:" + " GameState = " + _gameDataHandler.GameState + ", _isBlack = " + _isBlack +
        //    ", order = " + gameTrace.order + ", x = " + gameTrace.x + ", y = " + gameTrace.y + ", player = " + gameTrace.player); 
        try { 
            _gameDataHandler.AddStep(gameTrace);
            _gameSceneTool.AddGameObj(gameTrace);
            _gameDataHandler.UpdateGameState(); 

            if (_isDoublePlayer) {
                _isBlack = !_isBlack;
            }
            _gameSceneTool.LimitText.text = "";
        }
        catch (ArgumentException) {
            Debug.Log("already have a step on this position");
            // already have a step on this position
        }
        catch (PlayerLimitException) { 
            _gameSceneTool.LimitText.text = "黑棋禁手触发" + Environment.NewLine + "x = " + gameTrace.x + ", y = " + (14 - gameTrace.y);
        }
    }

    public void PlayerRegret() {
        if ((_gameDataHandler.GameState != GameState.BlackPlay && _isBlack)
            || (_gameDataHandler.GameState != GameState.WhitePlay && !_isBlack)
            || (_gameDataHandler.Turn <= 1)) {
            return;
        }
        try {
            // remove 2 step
            _gameDataHandler.RevertGameState();
            GameTrace gameTrace = _gameDataHandler.GetCurrentStep();
            _gameDataHandler.RemoveStep(gameTrace);
            _gameSceneTool.RemoveGameObj(gameTrace);

            _gameDataHandler.RevertGameState();
            gameTrace = _gameDataHandler.GetCurrentStep();
            _gameDataHandler.RemoveStep(gameTrace); 
            _gameSceneTool.RemoveGameObj(gameTrace);
        }
        catch (EmptyGameDataException) { 
            // no game date available
            Debug.Log("EmptyGameDataException");
        }
    }

    public void PlayerReset() {
        _gameDataHandler.ResetGameData();
        _gameSceneTool.ResetGameScene();
        if (_isDoublePlayer) {
            _isBlack = true;
        }
    }

    private void Update() {
        // online player is handled by class OnlinePlayer
        if (_isOnline) {
            return;
        }

        if (_isBot) {
            if ((_gameDataHandler.GameState != GameState.BlackPlay && _isBlack)
                || (_gameDataHandler.GameState != GameState.WhitePlay && !_isBlack)) {
                return;
            } 
            try {
                GameTrace gameTrace = AiDriver.GetAiStep(GameDataHandler.Instance.GameToJson());
                PlayerAddStep(gameTrace);
            } 
            catch (EmptyGameDataException) {
                Debug.Log("EmptyGameData");
            } 
            return;
        }

        // handle game playing
		if (_cursor.Enabled && Input.GetMouseButtonDown(0)){
            GameTrace gameTrace = new GameTrace(_gameDataHandler.Turn, _isBlack, _cursor.BoardPos.x, _cursor.BoardPos.y);
            PlayerAddStep(gameTrace);
		}
 

		// handle regret
		if (Input.GetKeyDown("z")){
            PlayerRegret();
		}

		// reset operation
		if(Input.GetKeyDown("escape")){
            PlayerReset();
		}
    }
}
