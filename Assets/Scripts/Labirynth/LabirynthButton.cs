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
    private float delay;

    public UnityEvent OnClick = new UnityEvent();

    public void CLick()
    {
        if (renderer.sprite == clickedSprite)
        {
            return;
        }

        renderer.sprite = clickedSprite;
        this.CallDelayed(delay, OnClick.Invoke);
    }
}
