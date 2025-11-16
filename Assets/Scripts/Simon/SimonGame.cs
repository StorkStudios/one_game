using StorkStudios.CoreNest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimonGame : MonoBehaviour
{
    [SerializeField]
    private List<SerializationArrayWrapper<InputKey>> keys;
    [SerializeField]
    private float onTime;
    [SerializeField]
    private float offTime;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private RectTransform arrow;
    [SerializeField]
    private RectTransform arrowParent;
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private AudioClip win;
    [SerializeField]
    private AudioClip wrong;
    [SerializeField]
    private AudioClip click;

    private List<InputKey> keysToPress;
    private bool input = false;

    private void Start()
    {
        var a = SceneLoader.Instance;

        InputHelper.GetInputAction(InputKey.Up).performed += OnInputUp;
        InputHelper.GetInputAction(InputKey.Down).performed += OnInputDown;
        InputHelper.GetInputAction(InputKey.Left).performed += OnInputLeft;
        InputHelper.GetInputAction(InputKey.Right).performed += OnInputRight;

        StartCoroutine(Game());
    }

    private void OnDestroy()
    {
        InputHelper.GetInputAction(InputKey.Up).performed -= OnInputUp;
        InputHelper.GetInputAction(InputKey.Down).performed -= OnInputDown;
        InputHelper.GetInputAction(InputKey.Left).performed -= OnInputLeft;
        InputHelper.GetInputAction(InputKey.Right).performed -= OnInputRight;
    }

    private void OnInputUp(InputAction.CallbackContext obj)
    {
        OnKey(InputKey.Up);
    }

    private void OnInputDown(InputAction.CallbackContext obj)
    {
        OnKey(InputKey.Down);
    }

    private void OnInputLeft(InputAction.CallbackContext obj)
    {
        OnKey(InputKey.Left);
    }

    private void OnInputRight(InputAction.CallbackContext obj)
    {
        OnKey(InputKey.Right);
    }

    private void OnKey(InputKey key)
    {
        if (!input)
        {
            return;
        }

        source.PlayOneShot(click);
        Instantiate(arrowPrefab, Vector3.zero, Quaternion.Euler(GetArrowEuler(key)), arrowParent);

        if (key == keysToPress[0])
        {
            keysToPress.RemoveAt(0);
        }
        else
        {
            source.PlayOneShot(wrong);
            input = false;
            this.CallDelayed(wrong.length, () => SceneLoader.Instance.ReloadScene());
        }
    }

    private Vector3 GetArrowEuler(InputKey key)
    {
        return key switch
        {
            InputKey.Up => new Vector3(0, 0, 180),
            InputKey.Down => new Vector3(0, 0, 0),
            InputKey.Left => new Vector3(0, 0, 270),
            InputKey.Right => new Vector3(0, 0, 90),
            _ => throw new System.NotImplementedException(),
        };
    }

    private IEnumerator Game()
    {
        foreach (SerializationArrayWrapper<InputKey> round in keys)
        {
            keysToPress = new List<InputKey>(round.Array);

            yield return new WaitForSeconds(waitTime);

            foreach (Transform child in arrowParent)
            {
                Destroy(child.gameObject);
            }

            foreach (InputKey key in round.Array)
            {
                yield return new WaitForSeconds(offTime);

                source.PlayOneShot(click);
                arrow.gameObject.SetActive(true);
                arrow.eulerAngles = GetArrowEuler(key);

                yield return new WaitForSeconds(onTime);

                arrow.gameObject.SetActive(false);
            }

            input = true;
            yield return new WaitWhile(() => keysToPress.Count > 0);
            input = false;
        }

        source.PlayOneShot(win);

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
}
