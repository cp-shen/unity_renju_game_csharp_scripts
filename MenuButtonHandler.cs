using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour {
    [SerializeField] private Button _btnLocalGame;
    [SerializeField] private Button _btnVsBot;
    [SerializeField] private Button _btnNetwork;
    [SerializeField] private Button _btnExit;

    void Start() {
        _btnLocalGame.onClick.AddListener(delegate { SceneManager.LoadSceneAsync("LocalGame"); });
        _btnVsBot.onClick.AddListener(delegate { SceneManager.LoadSceneAsync("VsBot"); });
        _btnNetwork.onClick.AddListener(delegate { SceneManager.LoadSceneAsync("Network"); });
        _btnExit.onClick.AddListener(Application.Quit);
    } 
}
