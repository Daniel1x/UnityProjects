using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    Level level = null;
    [SerializeField] Image image = null;
    [SerializeField] Slider slider = null;

    private void Start()
    {
        level = FindObjectOfType<Level>();
    }

    public void SetAutoPlay()
    {
        level.ChangeAutoPlayStatus();
        if (level.IsAutoPlayEnabled())
        {
            image.color = Color.green;
        }
        else
        {
            image.color = Color.red;
        }
    }

    public void SetTimeSpeed()
    {
        level.SetTimeSpeed(slider.value);
    }
}
