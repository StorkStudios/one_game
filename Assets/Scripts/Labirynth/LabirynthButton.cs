using StorkStudios.CoreNest;
using UnityEngine;
using UnityEngine.Events;

public class LabirynthButton : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer renderer;
    [SerializeField]
    private Sprite clickedSprite;
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private AudioClip click;
    [SerializeField]
    private float delay;

    public UnityEvent OnClick = new UnityEvent();

    public void CLick()
    {
        if (renderer.sprite == clickedSprite)
        {
            return;
        }

        source.PlayOneShot(click);
        renderer.sprite = clickedSprite;
        this.CallDelayed(delay, OnClick.Invoke);
    }
}
