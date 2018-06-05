using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.InteropServices;


public class GameController : MonoBehaviour{
	[SerializeField] private CursorController _cursor;
	[SerializeField] private GameObject _whiteStonePrefab;
	[SerializeField] private GameObject _blackStonePrefab;
	[SerializeField] private Text _stateText;
	[SerializeField] private Camera _persCamera;
	[SerializeField] private Camera _orthoCamera;
	private Dictionary<Vector2Int, PlayTuple> _whitePlayTrace;
	private Dictionary<Vector2Int, PlayTuple> _blackPlayTrace;
	private Dictionary<int, Vector2Int> _playSequence;
	private GameState _gameState;
	private int _turn;
    [DllImport("Native_Ai_Module", CallingConvention = CallingConvention.Cdecl)]
    static extern void drive_ai(string json_in, StringBuilder json_out, int capacity);


    private string GameToJson() {
        var jsonWrapper = new JsonWrapper();
        jsonWrapper.gameTrace = new List<GameTrace>();
        foreach(var item in _blackPlayTrace){
            PlayTuple playTuple = item.Value;
            jsonWrapper.gameTrace.Add(new GameTrace(playTuple.Sequence, true, playTuple.BoardPos.x, 14 - playTuple.BoardPos.y));
        }
        foreach(var item in _whitePlayTrace){
            PlayTuple playTuple = item.Value;
            jsonWrapper.gameTrace.Add(new GameTrace(playTuple.Sequence, false, playTuple.BoardPos.x, 14 - playTuple.BoardPos.y));
        }
        return JsonConvert.SerializeObject(jsonWrapper);
    }

    private GameTrace GetAiStep() {
        StringBuilder json_out_builder = new StringBuilder(100);
        drive_ai(GameToJson(), json_out_builder, json_out_builder.Capacity);
        string json_out = json_out_builder.ToString();
        json_out = json_out.Trim();
        while (!json_out.StartsWith("{") && json_out.Length > 0) {
            json_out = json_out.Remove(0, 1);
        }
        while (!json_out.EndsWith("}") && json_out.Length > 0) {
            json_out = json_out.Remove(json_out.Length - 1, 1);
        }
        Debug.Log(GameToJson());
        Debug.Log(json_out);
        // deserialize the object from json text
        if(json_out.Length > 0) { 
            GameTrace gameTrace = JsonConvert.DeserializeObject<GameTrace>(json_out);
            return gameTrace;
        }
        else {
            return null;
        }
    }

    // Use this for initialization
    void Start (){
		_whitePlayTrace = new Dictionary<Vector2Int, PlayTuple>();
		_blackPlayTrace = new Dictionary<Vector2Int, PlayTuple>();
		_playSequence = new Dictionary<int, Vector2Int>();
		_gameState = GameState.BlackPlay;
		_turn = 0;
		_persCamera.enabled = false;
		_orthoCamera.enabled = true;
	}
	
	// Update is called once per frame
	void Update (){ 
		
		// set state display in UI
		switch (_gameState){
			case GameState.BlackPlay:
				_stateText.text = "BlackPlay";
				break;
			case GameState.WhitePlay:
				_stateText.text = "WhitePlay";
				break;
			case GameState.BlackWin:
				_stateText.text = "BlackWin";
				break;
			case GameState.WhiteWin:
				_stateText.text = "WhiteWin";
				break;
		}
		
        // handle game playing
		if (Input.GetKeyDown("space") && !_whitePlayTrace.ContainsKey(_cursor.BoardPos) && !_blackPlayTrace.ContainsKey(_cursor.BoardPos)){
			Vector3 worldPos = new Vector3(CursorController.CalcPosition(_cursor.BoardPos.x), 1, CursorController.CalcPosition(_cursor.BoardPos.y));
			if (_gameState == GameState.BlackPlay){ 
				// set the new gameObject in the scene
				GameObject newStone = Instantiate(_blackStonePrefab, worldPos, Quaternion.LookRotation(Vector3.up));
				newStone.transform.localScale = new Vector3(2, 2, 2);
				
				// add a new element to the dictionary
				PlayTuple playTuple = new PlayTuple(_cursor.BoardPos, _turn, newStone);
				_blackPlayTrace.Add(_cursor.BoardPos, playTuple);
				_playSequence.Add(_turn, _cursor.BoardPos); 
				
				// check if black wins
				if (WinChecker.CheckWin(_blackPlayTrace, _cursor.BoardPos)){
					_gameState = GameState.BlackWin;
                    _turn++;
				}
				else{
                    _gameState = GameState.WhitePlay;
                    _turn++;
				} 
			}else if (_gameState == GameState.WhitePlay){
				// set the new gameObject in the scene
				GameObject newStone = Instantiate(_whiteStonePrefab, worldPos, Quaternion.LookRotation(Vector3.up));
				newStone.transform.localScale = new Vector3(2, 2, 2);
				
				// add a new element to the dictionary
				PlayTuple playTuple = new PlayTuple(_cursor.BoardPos, _turn, newStone);
				_whitePlayTrace.Add(_cursor.BoardPos, playTuple);
				_playSequence.Add(_turn, _cursor.BoardPos);
				
				// check if white wins
				if (WinChecker.CheckWin(_whitePlayTrace, _cursor.BoardPos)){
					_gameState = GameState.WhiteWin;
                    _turn++;
				}
				else{
                    _gameState = GameState.BlackPlay;
                    _turn++;
				} 
			}
		}

        // handle AI playing
		if (Input.GetKeyDown("x") && (_gameState == GameState.BlackPlay || _gameState == GameState.WhitePlay)){

            GameTrace gameTrace = GetAiStep();
            if(gameTrace == null) {
                return;
            } 
            // set the new gameObject in the scene
            Vector2Int boardPos = new Vector2Int(gameTrace.x, 14 - gameTrace.y);
            if(_whitePlayTrace.ContainsKey(boardPos) || _blackPlayTrace.ContainsKey(boardPos)) {
                return;
            }
			Vector3 worldPos = new Vector3(CursorController.CalcPosition(boardPos.x), 1, CursorController.CalcPosition(boardPos.y));
            GameObject newStone = Instantiate(gameTrace.player == "black" ? _blackStonePrefab : _whiteStonePrefab, worldPos, Quaternion.LookRotation(Vector3.up));
            newStone.transform.localScale = new Vector3(2, 2, 2);
            
            // add a new element to the dictionary
            PlayTuple playTuple = new PlayTuple(boardPos, _turn, newStone);
            if(gameTrace.player == "black") {
                _blackPlayTrace.Add(boardPos, playTuple);
            }
            else {
                _whitePlayTrace.Add(boardPos, playTuple); 
            }
            _playSequence.Add(_turn, boardPos); 

			if (_gameState == GameState.BlackPlay){ 
				// check if black wins
				if (WinChecker.CheckWin(_blackPlayTrace, boardPos)){
					_gameState = GameState.BlackWin;
				}
				else{
                    _gameState = GameState.WhitePlay;
				} 
                    _turn++;
			}else if (_gameState == GameState.WhitePlay){
				// check if white wins
				if (WinChecker.CheckWin(_whitePlayTrace, boardPos)){
					_gameState = GameState.WhiteWin;
				}
				else{
                    _gameState = GameState.BlackPlay;
				} 
                    _turn++;
			}
		}
		
		// reset operation
		if(Input.GetKeyDown("escape")){
			foreach (var ele in _whitePlayTrace){
				Destroy(ele.Value.GameObject);
			}
			foreach (var ele in _blackPlayTrace){
				Destroy(ele.Value.GameObject);
			}
            _whitePlayTrace = new Dictionary<Vector2Int, PlayTuple>();
            _blackPlayTrace = new Dictionary<Vector2Int, PlayTuple>();
            _playSequence = new Dictionary<int, Vector2Int>();
            _gameState = GameState.BlackPlay;
            _turn = 0;
		}
		
		// handle camera oparation
		if (Input.GetKeyDown("return")){
			_persCamera.enabled = !_persCamera.enabled;
			_orthoCamera.enabled = !_orthoCamera.enabled;
		}
		
		// handle regret
		if (Input.GetKeyDown("z")){
			Vector2Int pos;
			PlayTuple playTuple = null;
			if (_playSequence.TryGetValue(_turn - 1, out pos)){
				_playSequence.Remove(_turn - 1);
                if (_blackPlayTrace.TryGetValue(pos, out playTuple)){
	                _blackPlayTrace.Remove(pos);
	                Destroy(playTuple.GameObject);
	                _turn--;
	                _gameState = GameState.BlackPlay;
                }
                else if (_whitePlayTrace.TryGetValue(pos, out playTuple)){
	                _whitePlayTrace.Remove(pos);
	                Destroy(playTuple.GameObject);
	                _turn--;
	                _gameState = GameState.WhitePlay;
                }
			}
		}
	}
}
