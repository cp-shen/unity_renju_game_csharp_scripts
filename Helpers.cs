using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonWrapper{
    public IList<GameTrace> gameTrace { get; set; }
}

public class GameTrace {
    public int order { get; set; }
    public string player { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public GameTrace (int order, bool isBlack, int x, int y)
    {
        this.order = order;
        this.player = isBlack ? "black" : "white";
        this.x = x;
        this.y = y;
    }
}

public enum GameState{
    WhitePlay, BlackPlay, WhiteWin, BlackWin
}
    
public class PlayTuple{
    private Vector2Int _boardPos;
    private int _sequence;
    private GameObject _gameObject;
    
    public Vector2Int BoardPos{
        get{ return _boardPos; }
        set{ _boardPos = value; }
    }

    public int Sequence{
        get{ return _sequence; }
        set{ _sequence = value; }
    }

    public GameObject GameObject{
        get{ return _gameObject; }
        set{ _gameObject = value; }
    }

    public PlayTuple(Vector2Int boardPos, int sequence, GameObject gameObject){
        _boardPos = boardPos;
        _sequence = sequence;
        _gameObject = gameObject;
    }
}
