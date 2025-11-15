using StorkStudios.CoreNest;
using UnityEngine;
using UnityEngine.UI;

public class FNFCharacter : MonoBehaviour
{
    public enum State
    {
        Idle,
        Up,
        Down,
        Left,
        Right,
        Wrong
    }

    [SerializeField]
    private SerializedDictionary<State, Sprite> sprites;
    [SerializeField]
    private Image image;

    private Coroutine currentCoroutine = null;

    public static State KeyToState(InputKey key)
    {
        return key switch
        {
            InputKey.Up => FNFCharacter.State.Up,
            InputKey.Down => FNFCharacter.State.Down,
            InputKey.Left => FNFCharacter.State.Left,
            InputKey.Right => FNFCharacter.State.Right,
            _ => throw new System.NotImplementedException(),
        };
    }

    private void Start()
    {
        image.sprite = sprites[State.Idle];
    }

    public void SetSpriteForDuration(State state, float duration)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        image.sprite = sprites[state];

        currentCoroutine = this.CallDelayed(duration, () =>
        {
            image.sprite = sprites[State.Idle];
            currentCoroutine = null;
        });
    }
}
