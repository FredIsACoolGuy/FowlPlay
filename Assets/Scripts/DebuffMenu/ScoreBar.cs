using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar : MonoBehaviour
{
    private float _score = 0;
    private float _scoreInPercent = 0;

    Image _scoreImg;
    float _percent = 0;
    float _speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        _scoreImg = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_percent < _scoreInPercent)
        {
            _scoreImg.fillAmount = Mathf.Lerp(0, _scoreInPercent, _percent);
            _percent += _speed * Time.deltaTime;
        }
        else
        {
            _scoreImg.fillAmount = _scoreInPercent;
        }
    }

    //takes max and divides it and compares percentage of current score to max
    public void SetScore(float score, float max, float speed)
    {
        _speed = speed;
        _score = score;
        _scoreInPercent = (score / max);
    }
}
