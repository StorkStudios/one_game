using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private List<InputKey> directions;
    [SerializeField]
    private List<Image> images;
    [SerializeField]
    private float delay;

    private void Start()
    {
        InputHelper.GetInputAction(InputKey.Up).performed += OnInputUp;
        InputHelper.GetInputAction(InputKey.Down).performed += OnInputDown;
        InputHelper.GetInputAction(InputKey.Left).performed += OnInputLeft;
        InputHelper.GetInputAction(InputKey.Right).performed += OnInputRight;

        StartCoroutine(Animation());
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
        SceneLoader.Instance.LoadScene(SceneSequence.Instance.FirstScene);
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

    private IEnumerator Animation()
    {
        while (true)
        {
            foreach (InputKey key in directions)
            {
                Vector3 euler = GetArrowEuler(key);
                foreach (Image image in images)
                {
                    image.transform.eulerAngles = euler;
                }

                yield return new WaitForSeconds(delay);
            }
        }
    }
}
