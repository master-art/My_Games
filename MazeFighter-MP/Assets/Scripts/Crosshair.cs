using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] Texture2D image;
    [SerializeField] int size;
    [SerializeField] float maxAngle;
    [SerializeField] float minAngle;

    float lookHeight;

    public void LookHeight(float value)
    {
        lookHeight += value;

        if(lookHeight > maxAngle || lookHeight < minAngle)
        {
            lookHeight -= value;
        }
    }

    private void OnGUI()
    {
        Vector3 screenPostion = Camera.main.WorldToScreenPoint(transform.position);
        screenPostion.y = Screen.height - screenPostion.y;
        GUI.DrawTexture(new Rect(screenPostion.x, screenPostion.y - lookHeight, size, size), image);
    }
}