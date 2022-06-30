using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackFX : MonoBehaviour
{
    public Sprite[] Clouds;
    private SpriteRenderer m_SpriteRenderer;
    private void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = Clouds[Random.Range(0, Clouds.Length)];

        //m_SpriteRenderer.size = sprite.bounds.size;
        m_SpriteRenderer.sprite = sprite;

        transform.localScale = Vector3.one * .8f;
        StartCoroutine("Fade");
    }

    private IEnumerator Fade()
    {
        float counter = .5f;
        while (counter > 0)
        {
            transform.localScale -= Time.deltaTime * 1.5f * Vector3.one;
            transform.position += Vector3.up * 8 * Time.deltaTime;
            yield return null;
            counter -= Time.deltaTime;
        }

        Destroy(gameObject);
    }
}
