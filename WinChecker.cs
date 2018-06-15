using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class WinChecker{
	public static bool CheckWin(Dictionary<Vector2Int, GameTrace> playTrace, Vector2Int position){
		// from left to right
		Vector2Int checkPos = new Vector2Int(position.x - 4, position.y);
		for (int i = 0; i < 5; i++){
			if (CheckRenjuRightWard(playTrace, checkPos)){
				return true;
			}
			
			checkPos.x++;
		}
		// from bottom to top
		checkPos = new Vector2Int(position.x, position.y - 4);
		for (int i = 0; i < 5; i++){
			if (CheckRenjuUpWard(playTrace, checkPos)){
				return true;
			}
			
			checkPos.y++;
		}
		// from left bottom to right top
		checkPos = new Vector2Int(position.x - 4, position.y - 4);
		for (int i = 0; i < 5; i++){
			if (CheckRenjuRightUpWard(playTrace, checkPos)){
				return true;
			}

			checkPos.y++;
			checkPos.x++;
		}
		// from left top to right bottom
		checkPos = new Vector2Int(position.x - 4, position.y + 4);
		for (int i = 0; i < 5; i++){
			if (CheckRenjuRightDownWard(playTrace, checkPos)){
				return true;
			}

			checkPos.x++;
			checkPos.y--;
		}

		return false;
	}

	private static bool CheckRenjuRightWard(Dictionary<Vector2Int, GameTrace> playTrace, Vector2Int position){
		for (int i = 0; i < 5; i++){
			if (!playTrace.ContainsKey(position)){
				return false;
			} 
			position.x++;
		}
		return true;
	}

	private static bool CheckRenjuUpWard(Dictionary<Vector2Int, GameTrace> playTrace, Vector2Int position){
		for (int i = 0; i < 5; i++){
			if (!playTrace.ContainsKey(position)){
				return false;
			} 
			position.y++;
		}
		return true;
	}


	private static bool CheckRenjuRightUpWard(Dictionary<Vector2Int, GameTrace> playTrace, Vector2Int position){
		for (int i = 0; i < 5; i++){
			if (!playTrace.ContainsKey(position)){
				return false;
			} 
			position.x++;
			position.y++;
		}
		return true;
	}

	private static bool CheckRenjuRightDownWard(Dictionary<Vector2Int, GameTrace> playTrace, Vector2Int position){
		for (int i = 0; i < 5; i++){
			if (!playTrace.ContainsKey(position)){
				return false;
			} 
			position.x++;
			position.y--;
		}
		return true;
	}
}
