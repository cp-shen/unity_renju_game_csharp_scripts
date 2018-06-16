using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;


public sealed class GameDataHandler {
    // singleton
    private static readonly GameDataHandler _instance = new GameDataHandler(); 
    public static GameDataHandler Instance { get { return _instance; } }
    static GameDataHandler() { }

	private Dictionary<Vector2Int, GameTrace> _whitePlayTrace;
	private Dictionary<Vector2Int, GameTrace> _blackPlayTrace;
	private Dictionary<Vector2Int, GameTrace> _totalPlayTrace;
	private Dictionary<int, Vector2Int> _playSequence;

	private int _turn;
    public int Turn {
        get { return _turn; }
    }

	private GameState _gameState;
    public GameState GameState {
        get { return _gameState; }
    }

    private int _playerNum;
    // 0 for black, 1 for white
    public int PlayerNum {
        get { return _playerNum; }
        set { _playerNum = value; }
    }


    private GameDataHandler(){
		_whitePlayTrace = new Dictionary<Vector2Int, GameTrace>();
		_blackPlayTrace = new Dictionary<Vector2Int, GameTrace>();
		_totalPlayTrace = new Dictionary<Vector2Int, GameTrace>();
		_playSequence = new Dictionary<int, Vector2Int>();
		_gameState = GameState.BlackPlay;
		_turn = 0;
        _playerNum = 0;
	} 

    public string GameToJson() {
        var jsonWrapper = new JsonWrapper();
        jsonWrapper.gameTrace = new List<GameTrace>();
        foreach(var item in _totalPlayTrace){
            GameTrace gameTrace = item.Value;
            gameTrace.y = 14 - gameTrace.y;
            jsonWrapper.gameTrace.Add(gameTrace);
        }
        return JsonConvert.SerializeObject(jsonWrapper);
    } 

    // returns false if given step is not allowed
    private bool GetLimit(GameTrace gameTrace) {
        gameTrace.y = 14 - gameTrace.y;
        return LimitChecker.player_limit(GameToJson(), JsonConvert.SerializeObject(gameTrace));
    }

    public void AddStep(GameTrace gameTrace) {
        Debug.Log("Game_Data: " + GameToJson());
        Debug.Log("Step_Data: " + JsonConvert.SerializeObject(gameTrace));
        if (gameTrace.player == "black" && !GetLimit(gameTrace)){
            throw new PlayerLimitException();
        }
        try {
            if(gameTrace.player == "black") {
                _totalPlayTrace.Add(new Vector2Int(gameTrace.x, gameTrace.y), gameTrace); 
                _blackPlayTrace.Add(new Vector2Int(gameTrace.x, gameTrace.y), gameTrace); 
            }
            else if(gameTrace.player == "white") {
                _totalPlayTrace.Add(new Vector2Int(gameTrace.x, gameTrace.y), gameTrace); 
                _whitePlayTrace.Add(new Vector2Int(gameTrace.x, gameTrace.y), gameTrace); 
            }
            _playSequence.Add(gameTrace.order, new Vector2Int(gameTrace.x, gameTrace.y));
        }
        catch (ArgumentException exc) {
            // already have this key
            Debug.Log("ArgumentException caught in GameDataHandler.AddStep");
            throw exc;
        }
    }

    public void RemoveStep(GameTrace gameTrace) {
        _playSequence.Remove(gameTrace.order);
        Vector2Int boardPos = new Vector2Int(gameTrace.x, gameTrace.y);
        _totalPlayTrace.Remove(boardPos);
        _blackPlayTrace.Remove(boardPos);
        _whitePlayTrace.Remove(boardPos);
    }

    public GameTrace GetCurrentStep() {
        Vector2Int boardPos;
        GameTrace gameTrace;
        if( _playSequence.TryGetValue(_turn, out boardPos)
           && (_blackPlayTrace.TryGetValue(boardPos, out gameTrace)
           || _whitePlayTrace.TryGetValue(boardPos, out gameTrace ))) {
            return gameTrace;
        }
        else {
            throw new EmptyGameDataException();
        }
    }

    public void ResetGameData() { 
        _whitePlayTrace = new Dictionary<Vector2Int, GameTrace>();
        _blackPlayTrace = new Dictionary<Vector2Int, GameTrace>();
        _playSequence = new Dictionary<int, Vector2Int>();
        _totalPlayTrace = new Dictionary<Vector2Int, GameTrace>();
        _gameState = GameState.BlackPlay;
        _turn = 0;
    } 

    public void UpdateGameState() {
        GameTrace gameTrace = GetCurrentStep();
        Vector2Int boardPos = new Vector2Int(gameTrace.x, gameTrace.y);
		switch (GameState){
			case GameState.BlackPlay:
                _turn++;
                if(WinChecker.CheckWin(_blackPlayTrace, boardPos)){
                    _gameState = GameState.BlackWin;
                }
                else {
                    _gameState = GameState.WhitePlay;
                }
				break;
			case GameState.WhitePlay:
                _turn++;
                if(WinChecker.CheckWin(_whitePlayTrace, boardPos)){
                    _gameState = GameState.WhiteWin;
                }
                else {
                    _gameState = GameState.BlackPlay;
                }
				break;
			case GameState.BlackWin:
				break;
			case GameState.WhiteWin:
				break;
		} 
    }

    public void RevertGameState() {
        if(_turn == 0) {
            return;
        }
        else {
            _turn--;
        }
		switch (GameState){
			case GameState.BlackPlay:
                _gameState = GameState.WhitePlay;
				break;
			case GameState.WhitePlay:
                _gameState = GameState.BlackPlay;
				break;
			case GameState.BlackWin:
                _gameState = GameState.BlackPlay;
				break;
			case GameState.WhiteWin:
                _gameState = GameState.WhitePlay;
				break;
		} 
    }
} 
