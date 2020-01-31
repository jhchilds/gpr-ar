using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpdatePoseText : MonoBehaviour
{
    public Text pose;
    private void Update()
    {
        pose.text = "x: " + Pose.Instance.x.ToString() + "       y:" + Pose.Instance.y.ToString() + "        z:" + Pose.Instance.z.ToString();
    }
}
