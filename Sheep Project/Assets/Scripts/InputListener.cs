using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aurélien
/// </summary>

public class InputListener : MonoBehaviour
{
    public static InputListener iL;

   public  Vector2 touchPositionBegin;

    public Vector2 swipeDirection;

    float swipeSince = 0;

    private void Awake()
    {
        iL = this;
    }

    private void Update()
    {
        if(Input.touches.Length == 0)
        {
            touchPositionBegin = Vector2.zero;
            swipeDirection = Vector2.zero;
            swipeSince = 0;
            return;
        }

        if(Input.touches[0].phase == TouchPhase.Began)
        {
            touchPositionBegin = Input.touches[0].position;
        }

        if(Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary)
        {
            swipeSince += Time.deltaTime;

            if ((Input.touches[0].position - touchPositionBegin).magnitude > 50f)
            {
                swipeDirection = (Input.touches[0].position - touchPositionBegin).normalized;
            }
        }

        if(Input.touches[0].phase == TouchPhase.Ended)
        {
            touchPositionBegin = Vector2.zero;
            swipeDirection = Vector2.zero;
            swipeSince = 0;
        }
    }
}
