using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void LevelEndEvent();

public class LevelGoal : MonoBehaviour
{
    public static event LevelEndEvent OnLevelEnd;

    [SerializeField]
    string NextLevel;

    [SerializeField]
    float delay = 0.5f;

    [SerializeField]
    ParticleSystem flagsSplotion;

    public void OnEnterTile(GameObject go)
    {
        var robot = go.GetComponent<RobotController>();
        robot.enabled = false;

        flagsSplotion.Play();

        StartCoroutine(delayReload());
    }
    
    IEnumerator<WaitForSeconds> delayReload()
    {
        yield return new WaitForSeconds(delay);
        OnLevelEnd?.Invoke();
        SceneManager.LoadScene(NextLevel);
    }
}
