using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fps : MonoBehaviour
{
    private Text _text;
    void Start()
    {
        _text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        float current = 0;
        current = (int)(1f / Time.unscaledDeltaTime);
        var avgFrameRate = (int)current;
        _text.text = avgFrameRate.ToString() + " FPS";
    }
}
