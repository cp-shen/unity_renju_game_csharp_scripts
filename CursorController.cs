using UnityEngine;

public class CursorController : MonoBehaviour{
	private Vector2Int _boardPos;
	// record the position of the cursor
	// from 0 to 14
	public Vector2Int BoardPos{
		get{ return _boardPos; }
	}

	// Use this for initialization
	void Start (){ 
		// set the cursor to center
		_boardPos = new Vector2Int(7, 7);
	}
	
	// Update is called once per frame
	void Update () {
		// set the cursor position and move the cursor in the game
		if (Input.GetKeyDown("left") || Input.GetKeyDown("right") || Input.GetKeyDown("up") || Input.GetKeyDown("down")){
            if (Input.GetKeyDown("left") && _boardPos.x > 0){
                _boardPos.x--;
            }
            if (Input.GetKeyDown("right") && _boardPos.x < 14){
                _boardPos.x++;
            }
            if (Input.GetKeyDown("down") && _boardPos.y > 0){
                _boardPos.y--;
            }
            if (Input.GetKeyDown("up") && _boardPos.y < 14){
                _boardPos.y++;
            }
			transform.position = new Vector3(CalcPosition(_boardPos.x), transform.position.y, CalcPosition(_boardPos.y));
		}
	}

	public static float CalcPosition(int boardPos){
		return (boardPos - 7) * 7.14f;
	}
}
