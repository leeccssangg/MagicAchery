using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TW.Reactive.CustomComponent;
using UnityEngine;

public class UILevelMileStone : MonoBehaviour
{
    [field: SerializeField] public int StartPosition {get; private set;}
    [field: SerializeField] public int CurrentLevel {get; private set;}
    [field: SerializeField] public GameObject PassLevelObj {get; private set;}
    [field: SerializeField] public GameObject CurrentLevelObj {get; private set;}
    [field: SerializeField] public GameObject LockLevelObj {get; private set;}
    
    public void SetupStart(int start)
    {
        StartPosition = start;
    }
    public void OnLevelChange(int level)
    {
        PassLevelObj.SetActive(StartPosition < (level - 1) % 10);
        CurrentLevelObj.SetActive(StartPosition == (level - 1) % 10);
        LockLevelObj.SetActive(StartPosition > (level - 1) % 10);
    }
}
