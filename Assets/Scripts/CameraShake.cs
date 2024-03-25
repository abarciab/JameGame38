using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake: MonoBehaviour
{
    [SerializeField] Transform camTransform;

    [Header("Camera Shake")]
    [SerializeField] bool _2D;
    [SerializeField] float defaultShakeRadius, defaultShakeTime, defaultShakeSpeed, distCutoff = 0.01f, radiusDieMod, radiusThreshold = 0.01f;
    [SerializeField] AnimationCurve smoothnessCuve;
    bool shaking;
    Vector3 currentTarget;
    Vector3 oldPos;
    float currentDist, timeLeftThisShake, timeTotalThisShake;

    [ButtonMethod]
    public void ShakeFixed()
    {
        Shake(defaultShakeRadius, defaultShakeSpeed, defaultShakeTime);
    }

    [ButtonMethod]
    public void ShakeGradual()
    {
        Shake(defaultShakeRadius, defaultShakeSpeed);
    }

    public void Shake(float radius, float speed, float time = 0)
    {
        StopAllCoroutines();
        shaking = true;
        StartCoroutine(AnimateShake(radius, speed, time));
    }

    IEnumerator AnimateShake(float radius, float speed, float time = 0)
    {
        var originalPos = camTransform.localPosition;
        var newRadius = SetPos(radius, speed);
        if (time == 0) radius = newRadius;
        while (time == 0 ? radius > radiusThreshold : time > 0) {
            float dist = Vector3.Distance(camTransform.localPosition, currentTarget);
            if (dist < distCutoff) {
                newRadius = SetPos(radius, speed);
                if (time == 0) radius = newRadius;
            }

            float progress = smoothnessCuve.Evaluate(timeLeftThisShake / timeTotalThisShake);
            camTransform.localPosition = Vector3.Lerp(oldPos, currentTarget, progress);

            timeLeftThisShake -= Time.deltaTime;
            if (time != 0) time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        timeTotalThisShake = Vector3.Distance(camTransform.localPosition, originalPos) / speed;
        timeLeftThisShake = timeTotalThisShake;
        while (timeLeftThisShake > 0) {
            float progress = smoothnessCuve.Evaluate(1 - timeLeftThisShake / timeTotalThisShake);
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalPos, progress);
            timeLeftThisShake -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        camTransform.localPosition = originalPos;

        shaking = false;
    }

    float SetPos(float radius, float speed)
    {
        oldPos = camTransform.localPosition;
        var pos = Random.insideUnitSphere.normalized * radius;
        if (_2D) pos.z = 0;
        currentTarget = pos;
        currentDist = Vector3.Distance(oldPos, currentTarget);
        timeTotalThisShake = currentDist / speed;
        timeLeftThisShake = timeTotalThisShake;
        return radius * radiusDieMod;
    }
}
