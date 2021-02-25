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
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, 1f, blocMask))
        {
            StartCoroutine(BonkAgainstWall(direction));
        }
        else if (Physics.Raycast(transform.position, direction, out hit, 1f, sheepMask))
        {
            StartCoroutine(MoveTowardSheep(direction, hit.collider.GetComponentInParent<SheepController>()));
        }
        else if(Physics.Raycast(transform.position + direction, Vector3.down, 1f, blocMask))
        {
            StartCoroutine(NormalMove(direction));
        }
        else
        {
            StartCoroutine(MoveThenFall(direction));
        }

    }

    IEnumerator NormalMove(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for(float i = 0; i < normalMovementTime; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / normalMovementTime;

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + direction;

        moving = false;
    }

    IEnumerator BonkAgainstWall(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

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

        transform.position =  originalPosition;

        moving = false;
    }

    IEnumerator MoveThenFall(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < normalMovementTime; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / normalMovementTime;

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + direction;

        StartCoroutine(FallUntilSomething());
    }

    IEnumerator FallUntilSomething()
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < normalMovementTime/2f; i += Time.fixedDeltaTime)
        {
            transform.position += Vector3.down * Time.fixedDeltaTime*2f / (normalMovementTime);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + Vector3.down;

        if (Physics.Raycast(transform.position, Vector3.down, 1f, blocMask))
        {
            moving = false;
        }
        else
        {
            StartCoroutine(FallUntilSomething());
        }
    }

    IEnumerator MoveTowardSheep(Vector3 direction ,SheepController sheepMovingTowardTo)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < normalMovementTime/4f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / normalMovementTime;

            yield return new WaitForFixedUpdate();
        }

        for (float i = 0; i < normalMovementTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / (normalMovementTime*4f);

            yield return new WaitForFixedUpdate();
        }

        //Sheep Push Logic


        Destroy(sheepMovingTowardTo.gameObject);

        for (float i = 0; i < normalMovementTime*6f / 8f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / normalMovementTime;

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + direction;

        moving = false;

        yield break;
    }
}
