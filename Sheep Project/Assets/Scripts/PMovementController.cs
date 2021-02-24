using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aurélien
/// </summary>
public class PMovementController : MonoBehaviour
{
    bool moving = false;

    public LayerMask blocMask;
    public LayerMask sheepMask;

    public float normalMovementTime;

    void Update()
    {
        if(InputListener.iL.swipeDirection != Vector2.zero)
        {
            if (moving == false)
                OneMovement(InputListener.iL.swipeDirection);
        }
    }

    void OneMovement(Vector2 direction)
    {
        moving = true;

        float directionAngle = Vector2.Angle(Vector2.right, direction);

        if (direction.y < 0)
            directionAngle = -directionAngle;

        if (-45 < directionAngle && directionAngle <= 45)
            direction = Vector2.right;
        else if (45 < directionAngle && directionAngle <= 135)
            direction = Vector2.up;
        else if (135 < directionAngle || directionAngle <= -135)
            direction = Vector2.left;
        else if (-135 < directionAngle && directionAngle <= -45)
            direction = Vector2.down;

        Vector3 directionConverted = new Vector3(direction.x, 0f, direction.y);

        DetectAroundInDirection(directionConverted);
    }

    void DetectAroundInDirection(Vector3 direction)
    {
        if(Physics.Raycast(transform.position, direction, 1f, blocMask))
        {
            StartCoroutine(BonkAgainstWall(direction));
        }
        else if(Physics.Raycast(transform.position + direction, Vector3.down, 1f, blocMask))
        {
            StartCoroutine(NormalMove(direction));
        }
        else
        {
            StartCoroutine(MoveThenFall(direction));
        }
        
        if(Physics.Raycast(transform.position, direction, 1f, sheepMask))
        {
            Debug.Log("DontMove");
        }

    }

    IEnumerator NormalMove(Vector3 direction)
    {
        for(float i = 0; i < normalMovementTime; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / normalMovementTime;

            yield return new WaitForFixedUpdate();
        }

        moving = false;
    }

    IEnumerator BonkAgainstWall(Vector3 direction)
    {
        for (float i = 0; i < 0.12f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / normalMovementTime;

            yield return new WaitForFixedUpdate();
        }

        for (float i = 0; i < 0.12f; i += Time.fixedDeltaTime)
        {
            transform.position -= direction * Time.fixedDeltaTime / normalMovementTime;

            yield return new WaitForFixedUpdate();
        }

        moving = false;
    }

    IEnumerator MoveThenFall(Vector3 direction)
    {
        for (float i = 0; i < normalMovementTime; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / normalMovementTime;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(FallUntilSomething());
    }

    IEnumerator FallUntilSomething()
    {
        for (float i = 0; i < normalMovementTime/2f; i += Time.fixedDeltaTime)
        {
            transform.position += Vector3.down * Time.fixedDeltaTime / (normalMovementTime/2f);

            yield return new WaitForFixedUpdate();
        }

        if (Physics.Raycast(transform.position, Vector3.down, 1f, blocMask))
        {
            moving = false;
        }
        else
        {
            StartCoroutine(FallUntilSomething());
        }
    }
}
