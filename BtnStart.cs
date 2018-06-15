using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BtnStart : MonoBehaviour {
    [SerializeField] private Button _btnPlayAsWhite;
    [SerializeField] private Button _btnPlayAsBlack;

    // Use this for initialization
    void Start() {
        _btnPlayAsWhite.onClick.AddListener(StartAsWhite);
        _btnPlayAsBlack.onClick.AddListener(StartAsBlack);
    }

    void StartAsWhite() {
        Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/BotPlayer"));
        Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Player"));
        DisableBtns();
    }

    void StartAsBlack() {
        Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Player"));
        Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/BotPlayer"));
        DisableBtns();
    }

    void DisableBtns() {
        _btnPlayAsBlack.gameObject.SetActive(false);
        _btnPlayAsWhite.gameObject.SetActive(false);
    } 
}
