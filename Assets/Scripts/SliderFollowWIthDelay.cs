using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderFollowWIthDelay : MonoBehaviour
{
    [SerializeField] float waitTime, lerpFactor;
    float waitCooldown;
    [SerializeField] Slider lead, follower;

    private void Start()
    {
        if (follower && lead) follower.value = lead.value;
    }

    private void Update()
    {
        if (follower == null || lead == null) return;

        if (follower.value > lead.value) {
            waitCooldown -= Time.deltaTime;
            if (waitCooldown <= 0) {
                follower.value = Mathf.Lerp(follower.value, lead.value, lerpFactor * Time.deltaTime);
                if (Mathf.Abs(follower.value - lead.value) <= 0.001f) {
                    follower.value = lead.value;
                    waitCooldown = waitTime;
                }
            }
        }
        else {
            //lead.value = follower.value;
        }
    }

}
