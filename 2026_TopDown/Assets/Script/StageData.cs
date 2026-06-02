using UnityEngine;

[CreateAssetMenu(fileName = "NewStageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    public int stageIndex;

    [Header("이 스테이지에 나올 수 있는 맵 디자인들")]
    public GameObject[] mapDesigns;
}
