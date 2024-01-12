using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFPS : MonoBehaviour
{
    [SerializeField] private Text fpsText;

    private float frames = 0f;
    private float timeElap = 0f;
    private float frametime= 0f;
    
    void Update()
    {
        frames++;
        timeElap += Time.unscaledDeltaTime;
        if (timeElap > 1f)
        {
            frametime=timeElap/(float)frames;
            timeElap -= 1f;
            UpdateText();
            frames = 0f;
        }
    }

    void UpdateText()
    {
        fpsText.text =string.Format("FPS : {0} FrameTime : {1:F2} ms",frames,frametime*1000f);
    }
}
