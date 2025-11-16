using DG.Tweening;
using StorkStudios.CoreNest;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class LabirynthPlayerController : Singleton<LabirynthPlayerController>
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float angleDuration;
    [SerializeField]
    private float angle;
    [SerializeField]
    private Transform rendererTransform;
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private AudioClip death;
    [SerializeField]
    private AudioClip move;
    [SerializeField]
    private AudioClip stop;
    [SerializeField]
    private AudioClip win;

    private Rigidbody2D rigidbody;

    private bool isMoving = false;

    private void Start()
    {
        var a = SceneLoader.Instance;

        rigidbody = GetComponent<Rigidbody2D>();

        InputHelper.GetInputAction(InputKey.Up).performed += OnInputUp;
        InputHelper.GetInputAction(InputKey.Down).performed += OnInputDown;
        InputHelper.GetInputAction(InputKey.Left).performed += OnInputLeft;
        InputHelper.GetInputAction(InputKey.Right).performed += OnInputRight;
    }

    protected override void OnDestroy()
    {
        InputHelper.GetInputAction(InputKey.Up).performed -= OnInputUp;
        InputHelper.GetInputAction(InputKey.Down).performed -= OnInputDown;
        InputHelper.GetInputAction(InputKey.Left).performed -= OnInputLeft;
        InputHelper.GetInputAction(InputKey.Right).performed -= OnInputRight;

        base.OnDestroy();
    }

    private void SnapToGrid()
    {
        rigidbody.position = new Vector2(Mathf.Round(rigidbody.position.x), Mathf.Round(rigidbody.position.y));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rigidbody.linearVelocity = Vector2.zero;
        isMoving = false;
        rendererTransform.DORotate(Vector3.zero, angleDuration / 2);
        SnapToGrid();

        if (collision.gameObject.CompareTag(Tag.Death.GetTagString()))
        {
            source.PlayOneShot(death);
            isMoving = true;

            this.CallDelayed(death.length, () => SceneLoader.Instance.ReloadScene());
        }
        else if (collision.gameObject.CompareTag(Tag.Win.GetTagString()))
        {
            source.PlayOneShot(win);
            isMoving = true;

            this.CallDelayed(win.length, () =>
            {
                Scene? scene = SceneSequence.Instance.GetNextScene(SceneLoader.Instance.CurrentScene);
                if (scene.HasValue)
                {
                    SceneLoader.Instance.LoadScene(scene.Value);
                }
                else
                {
                    print("bulech");
                }
            });
        }
        else
        {
            source.PlayOneShot(stop);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out LabirynthButton button))
        {
            button.CLick();
        }
    }

    private void OnInputUp(InputAction.CallbackContext obj)
    {
        if (isMoving)
        {
            return;
        }

        source.PlayOneShot(move);

        rigidbody.linearVelocityY = speed;
        isMoving = true;

        rendererTransform.DORotate(Vector3.forward * angle, angleDuration);
    }

    private void OnInputDown(InputAction.CallbackContext obj)
    {
        if (isMoving)
        {
            return;
        }

        source.PlayOneShot(move);

        rigidbody.linearVelocityY = -speed;
        isMoving = true;

        rendererTransform.DORotate(-Vector3.forward * angle, angleDuration);
    }

    private void OnInputLeft(InputAction.CallbackContext obj)
    {
        if (isMoving)
        {
            return;
        }

        source.PlayOneShot(move);

        rigidbody.linearVelocityX = -speed;
        isMoving = true;

        rendererTransform.DORotate(Vector3.forward * angle, angleDuration);
    }

    private void OnInputRight(InputAction.CallbackContext obj)
    {
        if (isMoving)
        {
            return;
        }

        source.PlayOneShot(move);

        rigidbody.linearVelocityX = speed;
        isMoving = true;

        rendererTransform.DORotate(-Vector3.forward * angle, angleDuration);
    }
}
