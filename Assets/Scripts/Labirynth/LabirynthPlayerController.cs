using StorkStudios.CoreNest;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class LabirynthPlayerController : Singleton<LabirynthPlayerController>
{
    [SerializeField]
    private float speed;

    private Rigidbody2D rigidbody;

    private bool isMoving = false;

    private void Start()
    {
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
        SnapToGrid();

        if (collision.gameObject.CompareTag(Tag.Death.GetTagString()))
        {
            SceneLoader.Instance.ReloadScene();
        }
        if (collision.gameObject.CompareTag(Tag.Win.GetTagString()))
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

        rigidbody.linearVelocityY = speed;
        isMoving = true;
    }

    private void OnInputDown(InputAction.CallbackContext obj)
    {
        if (isMoving)
        {
            return;
        }

        rigidbody.linearVelocityY = -speed;
        isMoving = true;
    }

    private void OnInputLeft(InputAction.CallbackContext obj)
    {
        if (isMoving)
        {
            return;
        }

        rigidbody.linearVelocityX = -speed;
        isMoving = true;
    }

    private void OnInputRight(InputAction.CallbackContext obj)
    {
        if (isMoving)
        {
            return;
        }

        rigidbody.linearVelocityX = speed;
        isMoving = true;
    }
}
