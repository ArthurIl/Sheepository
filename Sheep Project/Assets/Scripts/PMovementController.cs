using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aurélien
/// </summary>
public class PMovementController : MonoBehaviour
{
    bool moving = false;

    public float normalMovementTime;

    public int fallingSinceXCases = 0;

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

        if (Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.blocMask))
        {
            StartCoroutine(BonkAgainstWall(direction));
        }
        else if (Physics.Raycast(transform.position, direction, out hit, 1f, LayerRefs.lR.sheepMask))
        {
            if (!Physics.Raycast(transform.position, Vector3.up, 1f, LayerRefs.lR.blocMask) && !Physics.Raycast(transform.position + Vector3.up, direction, 1f, LayerRefs.lR.blocMask))
            {
                StartCoroutine(MoveTowardSheep(direction, hit.collider.GetComponentInParent<SheepController>()));
            }
            else
            {
                StartCoroutine(BonkAgainstWall(direction));
            }
            
        }
        else if(Physics.Raycast(transform.position + direction, Vector3.down, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
        {
            StartCoroutine(NormalMove(direction));
        }
        else
        {
            StartCoroutine(MoveThenFall(direction));
        }

    }

    #region Normal Moves

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

        StartCoroutine(FallUntilSomething(direction, true));
    }

    IEnumerator FallUntilSomething(Vector3 direction, bool bounce)
    {
        fallingSinceXCases += 1;

        Vector3 originalPosition = transform.position;

        for (float i = 0; i < normalMovementTime/4f; i += Time.fixedDeltaTime)
        {
            transform.position += Vector3.down * Time.fixedDeltaTime*4f / (normalMovementTime);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + Vector3.down;

        if (Physics.Raycast(transform.position, Vector3.down, 1f, LayerRefs.lR.blocMask))
        {
            fallingSinceXCases = 0;

            moving = false;
        }
        else if(Physics.Raycast(transform.position, Vector3.down, 1f, LayerRefs.lR.sheepMask))
        {
            if(bounce)
            {
                StartCoroutine(BounceBackFromSheep(direction, fallingSinceXCases));
            }
            else
            {
                moving = false;
            }

            fallingSinceXCases = 0;
        }
        else
        {
            StartCoroutine(FallUntilSomething(direction, bounce));
        }
    }

    #endregion

    #region Element Interaction Moves

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

        bool sheepWillMove = true;

        RaycastHit hit;

        if(Physics.Raycast(sheepMovingTowardTo.transform.position, Vector3.up, out hit, 1f, LayerRefs.lR.sheepMask))
        {
            Vector3 closestPoint = hit.transform.GetComponent<SheepController>().col.ClosestPoint(hit.transform.position + direction);

            if (Physics.Raycast(closestPoint, direction, 0.5f, LayerRefs.lR.sheepMask))
            {
                sheepWillMove = false;
            }
        }
        else if (Physics.Raycast(sheepMovingTowardTo.transform.position + Vector3.up, direction, 1f, LayerRefs.lR.sheepMask))
        {
            sheepWillMove = false;
        }

        if (Physics.Raycast(sheepMovingTowardTo.transform.position, direction, 1f, LayerRefs.lR.blocMask))
        {
            sheepWillMove = false;
        }

        sheepMovingTowardTo.ChooseMovement(direction);

        if (sheepWillMove)
        {
            for (float i = 0; i < normalMovementTime * 6f / 8f; i += Time.fixedDeltaTime)
            {
                transform.position += direction * Time.fixedDeltaTime / normalMovementTime;

                yield return new WaitForFixedUpdate();
            }

            transform.position = originalPosition + direction;
        }
        else
        {
            if (Physics.Raycast(transform.position + Vector3.up, direction, 1f, LayerRefs.lR.sheepMask))
            {
                for (float i = 0; i < normalMovementTime * 3f / 8f; i += Time.fixedDeltaTime)
                {
                    transform.position -= direction * Time.fixedDeltaTime / normalMovementTime;

                    yield return new WaitForFixedUpdate();
                }

                transform.position = originalPosition;
            }
            else
            {
                for (float i = 0; i < normalMovementTime * 6f / 8f; i += Time.fixedDeltaTime)
                {
                    transform.position += direction * Time.fixedDeltaTime / normalMovementTime;
                    transform.position += Vector3.up * Time.fixedDeltaTime * 8f / (normalMovementTime * 6f);

                    yield return new WaitForFixedUpdate();
                }

                transform.position = originalPosition + direction + Vector3.up;
            }
        }

        moving = false;

        yield break;
    }

    IEnumerator BounceBackFromSheep(Vector3 direction, int casesToGain)
    {
        while (casesToGain > 0 && (!Physics.Raycast(transform.position, Vector3.up, 1f, LayerRefs.lR.blocMask)))
        {
            casesToGain -= 1;

            Vector3 originalPosition = transform.position;

            for (float i = 0; i < normalMovementTime / 4f; i += Time.fixedDeltaTime)
            {
                transform.position += Vector3.up * Time.fixedDeltaTime * 4f / (normalMovementTime);

                yield return new WaitForFixedUpdate();
            }

            transform.position = originalPosition + Vector3.up;
        }

        if(Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
        {
            StartCoroutine(FallUntilSomething(direction, false));
        }
        else
        {
            Vector3 originalPosition = transform.position;

            for (float i = 0; i < normalMovementTime / 4f; i += Time.fixedDeltaTime)
            {
                transform.position += direction * Time.fixedDeltaTime * 2f / normalMovementTime;
                transform.position += Vector3.up * Time.fixedDeltaTime * 2f / (normalMovementTime);

                yield return new WaitForFixedUpdate();
            }

            for (float i = 0; i < normalMovementTime / 4f; i += Time.fixedDeltaTime)
            {
                transform.position += direction * Time.fixedDeltaTime * 2f / normalMovementTime;
                transform.position -= Vector3.up * Time.fixedDeltaTime * 2f / (normalMovementTime);

                yield return new WaitForFixedUpdate();
            }

            transform.position = originalPosition + direction;

            if (Physics.Raycast(transform.position, Vector3.down, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
            {
                moving = false;
            }
            else
            {
                StartCoroutine(FallUntilSomething(direction, true));
            }
                
        }

        
    }

    #endregion
}
