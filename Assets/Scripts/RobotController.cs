using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private void OnEnable()
    {
        ActionsController.OnActionEvent += ActionsController_OnActionEvent;
    }

    private void OnDisable()
    {
        ActionsController.OnActionEvent -= ActionsController_OnActionEvent;
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

    ActionEventType nextAction = ActionEventType.None;

    [SerializeField]
    float tickDuration = 1f;

    float actionStart;
    ActionEventType currentAction = ActionEventType.None;

    Vector3 translationStart;
    Vector3 translationTarget;

    [SerializeField]
    AnimationCurve moveEasing;

    Quaternion rotationStart;
    Quaternion rotationTarget;
    [SerializeField]
    AnimationCurve rotationEasing;

    void EaseAction(ActionEventType action, float progress)
    {
        if (action == ActionEventType.None) return;

        if (action == ActionEventType.Move)
        {
            if (progress == 1)
            {
                transform.position = translationTarget;
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

    void StartAction(ActionEventType action)
    {
        if (action == ActionEventType.None) return;

        Debug.Log($"Initiating action {action}");

        if (action == ActionEventType.Move)
        {
            translationStart = transform.position;
            translationTarget = translationStart + transform.up * GriddedLevel.GridSize;
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
            nextAction = ActionEventType.None;
            StartAction(currentAction);
        }
    }

}
