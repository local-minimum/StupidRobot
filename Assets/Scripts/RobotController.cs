using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private void OnEnable()
    {
        ActionsController.OnActionEvent += ActionsController_OnActionEvent;
        ActionsController.OnStatusEvent += ActionsController_OnStatusEvent;
    }


    private void OnDisable()
    {
        ActionsController.OnActionEvent -= ActionsController_OnActionEvent;
        ActionsController.OnStatusEvent -= ActionsController_OnStatusEvent;
    }

    bool eager;
    private void ActionsController_OnStatusEvent(StatusEventType eventType, bool enabled)
    {
        if (eventType == StatusEventType.Eager)
        {
            eager = enabled;
        }
    }

    [SerializeField]
    List<ActionEventType> recognizedActions = new List<ActionEventType>() { ActionEventType.Move };

    private void ActionsController_OnActionEvent(ActionEventType eventType)
    {
        if (recognizedActions.Contains(eventType))
        {
            nextAction = eventType;
        }
    }

    LevelTile currentTile;

    private void Start()
    {
        currentTile = GriddedLevel.GetTile(transform.position);
        if (currentTile == null)
        {
            Debug.LogError("Failed to find current tile!");
        } else
        {
            currentTile.Occupy(gameObject);
        }
    }

    ActionEventType nextAction = ActionEventType.None;

    [SerializeField]
    float tickDuration = 1f;

    float actionStart;
    ActionEventType currentAction = ActionEventType.None;

    Vector3 translationStart;
    Vector3 translationTarget;
    LevelTile targetTile;

    [SerializeField]
    AnimationCurve moveEasing;

    Quaternion rotationStart;
    Quaternion rotationTarget;
    [SerializeField]
    AnimationCurve rotationEasing;

    void EaseAction(ActionEventType action, float progress)
    {
        if (action == ActionEventType.None) return;

        if (action == ActionEventType.Move || action == ActionEventType.Dash)
        {
            if (progress == 1)
            {
                transform.position = translationTarget;
                currentTile = targetTile;
                currentTile.Arrive(gameObject);
            }
            else
            {
                transform.position = Vector3.Lerp(
                    translationStart,
                    translationTarget,
                    moveEasing.Evaluate(progress)
                );
            }
        } else if (action == ActionEventType.Left || action == ActionEventType.Right)
        {
            if (progress == 1)
            {
                transform.rotation = rotationTarget;
            } else
            {
                transform.rotation = Quaternion.Lerp(
                    rotationStart,
                    rotationTarget,
                    rotationEasing.Evaluate(progress)
                );
            }
        }
    }

    LevelTile ValidateTargetTile(Vector3 translationTarget)
    {
        var targetTile = GriddedLevel.GetTile(translationTarget);
        if (targetTile == null)
        {
            Debug.Log($"Cant move to {translationTarget} because there's no tile there");
            return null;
        } else if (!targetTile.Accessible)
        {
            Debug.Log($"Cant move to {targetTile} because it's not accessible");
            return null;
        } else if (targetTile.Elevation > currentTile.Elevation)
        {
            Debug.Log($"Cant move to {targetTile} because it's higher up");
            return null;
        }

        return targetTile;
    }

    void StartAction(ActionEventType action)
    {
        if (action == ActionEventType.None) return;

        Debug.Log($"Initiating action {action}");

        if (action == ActionEventType.Move)
        {
            translationStart = transform.position;
            targetTile = ValidateTargetTile(translationStart + transform.up * GriddedLevel.GridSize);
            if (targetTile == null)
            {
                currentAction = ActionEventType.None;
            } else
            {
                translationTarget = targetTile.transform.position;
                targetTile.Occupy(gameObject);
                currentTile.Free(gameObject);
            }
        } else if (action == ActionEventType.Dash)
        {

            translationStart = transform.position;
            targetTile = null;
            for (int i = 1; i <= 2; i++)
            {
                var tile = ValidateTargetTile(translationStart + transform.up * i * GriddedLevel.GridSize);
                if (tile == null)
                {
                    break;
                } else
                {
                    targetTile = tile;
                }
            }
            if (targetTile == null)
            {
                currentAction = ActionEventType.None;
            } else
            {
                translationTarget = targetTile.transform.position;
                targetTile.Occupy(gameObject);
                currentTile.Free(gameObject);
            }

        } else if (action == ActionEventType.Left || action == ActionEventType.Right)
        {
            rotationStart = transform.rotation;
            var direction = action == ActionEventType.Left ? 1 : -1;
            rotationTarget = rotationStart * Quaternion.Euler(0, 0, direction * 90);
        }
    }

    private void Update()
    {
        var tickProgress = Mathf.Clamp01((Time.timeSinceLevelLoad - actionStart) / tickDuration);

        EaseAction(currentAction, tickProgress);

        if (tickProgress >= 1)
        {
            actionStart = Time.timeSinceLevelLoad;
            currentAction = nextAction;
            if (!eager)
            {
                nextAction = ActionEventType.None;
            }
            StartAction(currentAction);
        }
    }

}
