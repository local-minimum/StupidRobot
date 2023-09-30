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

        StartCoroutine(delayReload());
    }
    
    IEnumerator<WaitForSeconds> delayReload()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
