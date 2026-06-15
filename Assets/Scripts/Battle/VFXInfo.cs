using UnityEngine;

[System.Serializable]
public class VFXTransformInfo
{
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private Vector3 scale = Vector3.one;

    public Vector3 PositionOffset => positionOffset;
    public Quaternion Rotation => Quaternion.Euler(rotationOffset);
    public Vector3 Scale => scale;

    public void Apply(Transform targetTransform)
    {
        targetTransform.localPosition = positionOffset;
        targetTransform.localRotation = Quaternion.Euler(rotationOffset);
        targetTransform.localScale = scale;
    }
}

public class VFXInfo : MonoBehaviour
{
    [Header("Spawn On Owner")]
    [SerializeField] private VFXTransformInfo playerInfo;

    [Header("Spawn On Target")]
    [SerializeField] private VFXTransformInfo enemyInfo;

    public VFXTransformInfo PlayerInfo => playerInfo;
    public VFXTransformInfo EnemyInfo => enemyInfo;

    public void SpawnOnPlayer(Transform player)
    {
        transform.SetParent(player, false);
        playerInfo.Apply(transform);
    }

    public void SpawnOnEnemy(Transform owner)
    {
        transform.SetParent(owner, false);
        enemyInfo.Apply(transform);
    }
}
