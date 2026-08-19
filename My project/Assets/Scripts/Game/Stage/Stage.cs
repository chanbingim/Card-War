using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private Transform _PlayerTransformAnchor;
    [SerializeField] private Transform _EnemyTransformAnchor;

    public void Initalize()
    {
        Utility.CHECK(_PlayerTransformAnchor);
        Utility.CHECK(_EnemyTransformAnchor);

        // 여기서 Map에 필요한 기능 전부 세팅
    }

    public Vector3 GetPlayerWorldPosition(Vector3 LocalPos)
    {
        return _PlayerTransformAnchor.TransformPoint(LocalPos);
    }

    public Vector3 GetEnemyWorldPosition(Vector3 LocalPos)
    {
        return _EnemyTransformAnchor.TransformPoint(LocalPos);
    }
}
