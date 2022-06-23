using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class OpenScene : MonoBehaviour
{
    public AnimationReferenceAsset Loop, Feiji1;

    private void Start()
    {
        GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, Loop, true, 0);

        StartCoroutine("Feiji");
    }

    private IEnumerator Feiji()
    {
        while (true)
        {
            TrackEntry track = GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(1, Feiji1, false);
            //track.TimeScale = .25f;
            yield return new WaitForSeconds(Random.Range(15f, 30f));

            GetComponent<SkeletonAnimation>().AnimationState.ClearTrack(1);
        }
    }
}
