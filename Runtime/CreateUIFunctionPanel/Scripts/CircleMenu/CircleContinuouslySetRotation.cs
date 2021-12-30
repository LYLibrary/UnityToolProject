using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 连续设定旋转
/// </summary>
public class CircleContinuouslySetRotation : MonoBehaviour
{

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }
    
}


