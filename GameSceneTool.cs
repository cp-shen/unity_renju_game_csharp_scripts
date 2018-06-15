using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneTool : MonoBehaviour {
	[SerializeField] private GameObject _whiteStonePrefab;
	[SerializeField] private GameObject _blackStonePrefab;
	[SerializeField] private Text _stateText;
	[SerializeField] private Text _limitText;
	[SerializeField] private Camera _persCamera;
	[SerializeField] private Camera _orthoCamera;

    private Dictionary<Vector2Int, GameObject> _gameObjects;
    public Text LimitText { get { return _limitText; } }

	// Use this for initialization
	void Start () {
        _gameObjects = new Dictionary<Vector2Int, GameObject>();
		_persCamera.enabled = false;
		_orthoCamera.enabled = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
	}

    // reset all data
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        GameDataHandler.Instance.ResetGameData();
        GameDataHandler.Instance.PlayerNum = 0;
    }
	
	// Update is called once per frame
	void Update () {
		// set state display in UI
		switch (GameDataHandler.Instance.GameState){
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
        // handle camera oparation 
        if (Input.GetKeyDown("return")) {
            _persCamera.enabled = !_persCamera.enabled;
            _orthoCamera.enabled = !_orthoCamera.enabled;
        }
    }

    public void AddGameObj(GameTrace gameTrace) {
        Vector2Int boardPos = new Vector2Int(gameTrace.x, gameTrace.y);
        if (_gameObjects.ContainsKey(boardPos)) {
            // already have a step on this position
            Debug.Log("in AddGameObj: already have this key");
            return;
        }
        Vector3 worldPos = new Vector3(CursorController.CalcPosition(gameTrace.x), 1, CursorController.CalcPosition(gameTrace.y));
        GameObject newStone = null;
        if (gameTrace.player == "black") { 
            newStone = Instantiate(_blackStonePrefab, worldPos, Quaternion.LookRotation(Vector3.up));
        }
        else if (gameTrace.player == "white"){ 
            newStone = Instantiate(_whiteStonePrefab, worldPos, Quaternion.LookRotation(Vector3.up));
        }
        if (newStone != null) {
            _gameObjects.Add(new Vector2Int(gameTrace.x, gameTrace.y), newStone); 
            newStone.transform.localScale = new Vector3(2, 2, 2);
        }
    }

    public void RemoveGameObj(GameTrace gameTrace) {
        Vector2Int boardPos = new Vector2Int(gameTrace.x, gameTrace.y);
        GameObject gameObject = null;
        if (_gameObjects.TryGetValue(boardPos, out gameObject)) {
            Destroy(gameObject);
            _gameObjects.Remove(boardPos);
        }
    }

    public void ResetGameScene() {
        foreach (var item in _gameObjects) {
            Destroy(item.Value);
        }
        _gameObjects = new Dictionary<Vector2Int, GameObject>();
    }
}
