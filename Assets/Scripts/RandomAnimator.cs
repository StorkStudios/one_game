using StorkStudios.CoreNest;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomAnimator : MonoBehaviour
{
    [SerializeField]
    private float delay;
    [SerializeField]
    private List<Sprite> sprites;
    [SerializeField]
    private SpriteRenderer renderer;

    private void Start()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        while (true)
        {
            renderer.sprite = sprites.Where(e => e != renderer.sprite).GetRandomElement();
            yield return new WaitForSeconds(delay);
        }
    }
}
