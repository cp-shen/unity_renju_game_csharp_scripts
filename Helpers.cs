using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class JsonWrapper{
    public IList<GameTrace> gameTrace { get; set; }
}

public struct GameTrace {
    public int order;
    public string player;
    public int x;
    public int y;
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

public static class LimitChecker {
    [DllImport("player_limit", EntryPoint="player_limit", CallingConvention=CallingConvention.Cdecl)]  
    [return:MarshalAs(UnmanagedType.I1)]  
    // returns false if given step is not allowed
    public static extern bool player_limit([MarshalAs(UnmanagedType.LPStr)] string game_data, [MarshalAs(UnmanagedType.LPStr)] string step_data); 
}

public class EmptyGameDataException : Exception { }
public class MaxPlayerException : Exception { }
public class PlayerLimitException : Exception { }
