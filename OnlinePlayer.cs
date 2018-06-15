using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class OnlinePlayer : NetworkBehaviour {
    private Player _player; 
    
	void Start () {
        //Debug.Log("Start() of OnlinePlayer is called.");
        _player = gameObject.AddComponent<Player>();
        _player.IsOnline = true;
        Debug.Log("Player connected, is local? : " + isLocalPlayer);
	}
 
    [ClientRpc] public void RpcAddStep(GameTrace gameTrace) {
        //Debug.Log("In Rpc:");
        //Debug.Log("order = " + gameTrace.order + ", x = " + gameTrace.x + ", y = " + gameTrace.y + ", player = " + gameTrace.player);
        _player.PlayerAddStep(gameTrace);
    }

    [ClientRpc] public void RpcRegret() {
        _player.PlayerRegret();
    }

    [ClientRpc] public void RpcReset() {
        _player.PlayerReset();
    }

    [Command] public void CmdAddStep(GameTrace gameTrace) {
        //Debug.Log("In Command:");
        //Debug.Log("order = " + gameTrace.order + ", x = " + gameTrace.x + ", y = " + gameTrace.y + ", player = " + gameTrace.player);
        RpcAddStep(gameTrace);
    }

    [Command] public void CmdRegret() {
        RpcRegret();
    }

    [Command] public void CmdReset() {
        RpcReset();
    }

    [ClientCallback]
    private void Update() {
        if (!isLocalPlayer) {
            return;
        }

        // handle game playing
		if (_player.Cursor.Enabled && Input.GetMouseButtonDown(0)){
            GameTrace gameTrace = new GameTrace(GameDataHandler.Instance.Turn, _player.IsBlack, _player.Cursor.BoardPos.x, _player.Cursor.BoardPos.y);
            Debug.Log( "in OnlinePlayer:" + 
                "order = " + gameTrace.order + ", x = " + gameTrace.x + ", y = " + gameTrace.y + ", player = " + gameTrace.player);
            CmdAddStep(gameTrace);
		}
 

		// handle regret
		if (Input.GetKeyDown("z")){
            CmdRegret();
		}

		// reset operation
		if(Input.GetKeyDown("escape")){
            CmdReset();
		} 
    }
}
