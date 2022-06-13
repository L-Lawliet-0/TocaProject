using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class IslandSelectionControl : TocaFunction
{

    public int SceneIndex;
    public SkeletonAnimation IslandAnimation;
    public AnimationReferenceAsset HouseAnim;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TouchControl>().ClickCallBacks.Add(Click);
    }

    public void Click()
    {
        LoadingCtrl.Instance.FocusPosition = transform.position;
        LoadingCtrl.Instance.LoadScene(SceneIndex);
        StartCoroutine("HouseShake");
    }

    private IEnumerator HouseShake()
    {
        if (!HouseAnim)
            yield break;
        TrackEntry track = IslandAnimation.AnimationState.SetAnimation(3, HouseAnim, false);

        yield return new WaitForSeconds(HouseAnim.Animation.Duration);

        IslandAnimation.AnimationState.SetEmptyAnimation(3, 0);
    }
}
