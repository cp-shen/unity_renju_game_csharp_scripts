using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioBtnHandler : MonoBehaviour {
    private Button _audioBtn;
    private Image _btnImage;
    private AudioSource _bgm;
    [SerializeField] private Sprite _playSprite;
    [SerializeField] private Sprite _muteSprite;

	// Use this for initialization
	void Start () {
        _audioBtn = GetComponent<Button>();
        _bgm = GetComponent<AudioSource>();
        _btnImage = GetComponent<Image>();

        _btnImage.sprite = _muteSprite;
        _bgm.Stop();
        _audioBtn.onClick.AddListener(SetBgm);
	}

    void SetBgm() {
        if (_bgm.isPlaying) {
            _bgm.Pause();
            _btnImage.sprite = _muteSprite;
        }
        else {
            _bgm.Play();
            _btnImage.sprite = _playSprite;
        }
    }
}
