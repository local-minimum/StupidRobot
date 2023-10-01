using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TrapTile : MonoBehaviour
{
    [SerializeField]
    float delay = 0.5f;

    public void OnEnterTile(GameObject go)
    {
        var robot = go.GetComponent<RobotController>();
        robot.enabled = false;

        StartCoroutine(delayReload(go));
    }

    [SerializeField]
    float rotationSpeed = 5;

    [SerializeField]
    AnimationCurve rotator;

    [SerializeField]
    AnimationCurve scaler;

    IEnumerator<WaitForSeconds> delayReload(GameObject go)
    {
        var progress = 0f;
        var start = Time.timeSinceLevelLoad;
        while (progress < 1)
        {
            progress = (Time.timeSinceLevelLoad - start) / delay;
            go.transform.localScale = Vector3.one * scaler.Evaluate(progress);
            go.transform.Rotate(Vector3.forward, rotationSpeed * rotator.Evaluate(progress));
            yield return new WaitForSeconds(0.02f);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
