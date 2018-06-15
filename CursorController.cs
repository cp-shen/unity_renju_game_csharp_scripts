using UnityEngine;
using System;

public class CursorController : MonoBehaviour{

	private Vector2Int _boardPos;
	// record the position of the cursor
	// from 0 to 14
	public Vector2Int BoardPos{
		get{ return _boardPos; }
	}

    private bool _enabled;
    public bool Enabled { get { return _enabled; } }

	// Use this for initialization
	void Start (){ 
		// set the cursor to center
		_boardPos = new Vector2Int(7, 7);
        _enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        // move the cursor with keyboard
        //if (Input.GetKeyDown("left")) {
        //    _boardPos.x--;
        //}
        //if (Input.GetKeyDown("right")) {
        //    _boardPos.x++;
        //}
        //if (Input.GetKeyDown("down")) {
        //    _boardPos.y--;
        //}
        //if (Input.GetKeyDown("up")) {
        //    _boardPos.y++;
        //}

        // move the cursor with mouse
        if (Camera.current) { 
            Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.name == "Board" ) {
                _boardPos.x = (int) (Math.Round(hit.point.x / 7.14f) + 7); 
                _boardPos.y = (int) (Math.Round(hit.point.z / 7.14f) + 7);
                _enabled = true;
            }
            else {
                _enabled = false;
            }
        }

        // clamp the coordinate to 0 ~ 14
        _boardPos.x = _boardPos.x < 0 ? 0 : _boardPos.x;
        _boardPos.x = _boardPos.x > 14 ? 14 : _boardPos.x;
        _boardPos.y = _boardPos.y < 0 ? 0 : _boardPos.y;
        _boardPos.y = _boardPos.y > 14 ? 14 : _boardPos.y;
        // set the world position of cursor
        transform.position = new Vector3(CalcPosition(_boardPos.x), transform.position.y, CalcPosition(_boardPos.y));
	}

	public static float CalcPosition(int boardPos){
		return (boardPos - 7) * 7.14f;
	}
}
