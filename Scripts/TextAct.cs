using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAct : MonoBehaviour
{
    int frame;
    int rate;

    Font setofont;
    Font hiraginofont;

    // Start is called before the first frame update
    void Start()
    {
        setofont = Resources.Load<Font>("Fonts/setofont");
        hiraginofont = Resources.Load<Font>("Fonts/hiragino");

        this.rate = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (frame % rate == 0) {
            if ((frame / rate) % 2 == 0)
            {
                gameObject.GetComponent<Text>().font = setofont;
            }
            else
            {
                gameObject.GetComponent<Text>().font = hiraginofont;
            }
        }

        frame++;
    }
}
