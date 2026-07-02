using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "ActionFit/AnimationData")]
public class AnimationData : BaseSO
{
    public BoxAnimationData boxAnimation;
}

[Serializable]
public class BoxAnimationData
{
    [Header("Box Destroy Animation")]
    [Tooltip("박스가 사라지는데 걸리는 시간")]
    public float boxDestroyDuration = 0.25f;
    
    [Header("Box Lid Close Animation")]
    [Tooltip("박스 뚜껑의 투명도가 1이 될 때까지 걸리는 애니메이션 시간")]
    public float boxLidFadeInDuration = 0.2f;
    [Tooltip("박스 뚜껑의 아래로 내려가는데 걸리는 시간")]
    public float boxLidCloseDownDuration = 0.2f;
    [Tooltip("박스 뚜껑이 위로 올라가는데 걸리는 시간")]
    public float boxLidCloseUpDuration = 0.1f;
    
    [Tooltip("박스 뚜겅이 나타나는 위치")]
    public float boxLidStartY = 0.5f;
    [Tooltip("박스 뚜겅이 내려가는 높이")]
    public float boxLidDownY = -0.43f;
    [Tooltip("박스 뚜겅이 올라오는 높이")]
    public float boxLidUpY = 0.03f;
    
    [Header("Box Lid Shake Animation")]
    [Tooltip("박스 뚜껑이 흔들리는 애니메이션 시간")]
    public float boxLidShakeDuration = 0.3f;
    [Tooltip("박스 뚜껑이 흔들리는 애니메이션 강도")]
    public float boxLidShakeStrength = 0.05f;
    [Tooltip("박스 뚜껑이 흔들리는 애니메이션 횟수")]
    public int boxLidShakeCount = 20;
    [Tooltip("박스 뚜껑이 흔들리는 애니메이션 랜덤 범위")]
    public float boxLidShakeRandomness = 90f;
    [Tooltip("박스 뚜껑이 흔들리는 애니메이션 세기가 끝나갈수록 점점 낮아지게 하기")]
    public bool boxLidShakeFadeout = true;
    
    [Header("Box Lid Open Animation")]
    [Tooltip("박스 뚜껑이 날아가는 힘 세기")]
    public float boxLidOpenStrength = 2.5f;
    [Tooltip("박스 뚜껑의 가로,세로에 따른 날아가는 힘 보정 값")]
    public float boxLidOpenCorrectionFactor = 1.2f;
    [Tooltip("박스 뚜껑이 올라가는 위치 값")]
    public float boxLidOpenPositionY = 0.1f;
    [Tooltip("박스 뚜껑이 올라가는 애니메이션 시간")]
    public float boxLibOpenYDuration = 0.5f;
    [Tooltip("박스 뚜껑이 옆으로 밀려나는 애니메이션 시간")]
    public float boxLibPushDuration = 0.5f;
    [Tooltip("박스 뚜껑이 회전하는 애니메이션 시간")]
    public float boxLidRotationDuration = 0.5f;
}
