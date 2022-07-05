using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    private bool Rotating;
    public bool PauseBGM = false;
    public float RotationSpeed = 90;

    private class MusicStructue
    {
        public GameObject Obj;
        public Vector3 Velocity;
        public float Counter, MaxCounter;
    }
    private List<MusicStructue> MSs;

    private void Awake()
    {
        MSs = new List<MusicStructue>();
    }


    private void Start()
    {
        Rotating = true;
        GetComponent<TouchControl>().ClickCallBacks.Add(Flip);

        StopAllCoroutines();
        if (PauseBGM)
            StartCoroutine("Runner");
    }

    private void OnEnable()
    {
        if (PauseBGM)
            StartCoroutine("Runner");
    }

    private void OnDisable()
    {
        for (int i = MSs.Count - 1; i >= 0; i--)
        {
            Destroy(MSs[i].Obj);
            MSs.RemoveAt(i); 
        }
    }

    public void Flip()
    {
        Rotating = !Rotating;

        if (PauseBGM)
        {
            if (Rotating)
                SoundManager.Instance.BackGroundMusic.UnPause();
            else
                SoundManager.Instance.BackGroundMusic.Pause();
        }
    }

    private void Update()
    {
        if (Rotating)
            transform.eulerAngles += Vector3.forward * RotationSpeed * Time.deltaTime;

        if (PauseBGM) 
        {
            for (int i = MSs.Count - 1; i >= 0; i--)
            {
                MSs[i].Obj.transform.position += MSs[i].Velocity * Time.deltaTime;
                MSs[i].Counter -= Time.deltaTime;
                Color color = MSs[i].Obj.GetComponent<SpriteRenderer>().color;
                MSs[i].Obj.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, MSs[i].Counter / MSs[i].MaxCounter);
                
                if (MSs[i].Counter <= 0)
                {
                    Destroy(MSs[i].Obj);
                    MSs.RemoveAt(i);
                }
            }
        }
    }

    private IEnumerator Runner()
    {
        while (true)
        {
            if (SoundManager.Instance.BackGroundMusic.isPlaying)
            {
                GameObject obj = new GameObject();
                obj.transform.position = transform.position;
                SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = GlobalParameter.Instance.MusicSymbols[Random.Range(0, 4)];
                sr.sortingLayerName = "Selection";
                sr.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                obj.transform.localScale = Vector3.one * .1f;

                MusicStructue ms = new MusicStructue();
                ms.Obj = obj;
                ms.Velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Random.Range(2f, 5f);
                ms.Counter = ms.MaxCounter = Random.Range(1.5f, 3f);

                MSs.Add(ms);
            }

            yield return new WaitForSeconds(Random.Range(.15f, .3f));
        }
    }
}
