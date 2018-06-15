using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text;

public static class AiDriver {
    [DllImport("Native_Ai_Module_hard", CallingConvention = CallingConvention.Cdecl)]
    static extern void drive_ai_hard(string json_in, StringBuilder json_out, int capacity);

    //[DllImport("Native_Ai_Module_normal", CallingConvention = CallingConvention.Cdecl)]
    //static extern void drive_ai_normal(string json_in, StringBuilder json_out, int capacity);

    public static GameTrace GetAiStep(string gameDataJson) {
        StringBuilder json_out_builder = new StringBuilder(100);
        drive_ai_hard(gameDataJson, json_out_builder, json_out_builder.Capacity);
        string json_out = json_out_builder.ToString();
        int pos = json_out.IndexOf('}');
        json_out = json_out.Remove(pos + 1, json_out.Length - 1 - pos);
        Debug.Log(gameDataJson);
        Debug.Log(json_out);
        // deserialize the object from json text
        if(json_out.Length > 0) { 
            GameTrace gameTrace = JsonConvert.DeserializeObject<GameTrace>(json_out);
            gameTrace.y = 14 - gameTrace.y;
            return gameTrace;
        }
        else {
            throw new EmptyGameDataException();
        }
    }
}
