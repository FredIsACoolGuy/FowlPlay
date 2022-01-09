using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar : MonoBehaviour
{
    private int _score = 0;
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
        if (_percent < 1)
        {
            _scoreImg.fillAmount = Mathf.Lerp(0, 1, _percent);
            _percent += _speed * Time.deltaTime;
        }
        else
        {
            _scoreImg.fillAmount = 1;
        }

    }

    //takes max and divides it and compares percentage of current score to max
    public void SetScore(int score, int max, float speed)
    {
        _speed = speed;
        _score = score;
        _scoreInPercent = score / max;
    }
}
