using DG.Tweening;
using StorkStudios.CoreNest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntermissionTwo : MonoBehaviour
{
    [System.Serializable]
    private struct Frame
    {
        public CanvasGroup group;
        public float duration;
    }

    [SerializeField]
    private List<InputKey> keys;
    [SerializeField]
    private float fadeDuration;
    [SerializeField]
    private List<Frame> frames;
    [SerializeField]
    private CanvasGroup game;
    [SerializeField]
    private RectTransform arrowParent;
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private List<Frame> winFrames;
    [SerializeField]
    private List<Frame> loseFrames;
    [SerializeField]
    private CanvasGroup lastText;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private float endDuration;
    [SerializeField]
    private CanvasGroup lastBackground;

    private List<InputKey> keysToPress = new List<InputKey>();
    private bool win = false;

    private IEnumerator Start()
    {
        var a = SceneLoader.Instance;

        InputHelper.GetInputAction(InputKey.Up).performed += OnInputUp;
        InputHelper.GetInputAction(InputKey.Down).performed += OnInputDown;
        InputHelper.GetInputAction(InputKey.Left).performed += OnInputLeft;
        InputHelper.GetInputAction(InputKey.Right).performed += OnInputRight;

        foreach (Frame frame in frames)
        {
            yield return frame.group.DOFade(1, fadeDuration).WaitForCompletion();
            yield return new WaitForSeconds(frame.duration);
            yield return frame.group.DOFade(0, fadeDuration).WaitForCompletion();
        }

        keysToPress = new List<InputKey>(keys);
        yield return game.DOFade(1, fadeDuration).WaitForCompletion();
        yield return new WaitWhile(() => keysToPress.Count > 0);
        yield return game.DOFade(0, fadeDuration).WaitForCompletion();

        IEnumerable<Frame> endFrames = win ? winFrames : loseFrames;
        foreach (Frame frame in endFrames)
        {
            yield return frame.group.DOFade(1, fadeDuration).WaitForCompletion();
            yield return new WaitForSeconds(frame.duration);
            yield return frame.group.DOFade(0, fadeDuration).WaitForCompletion();
        }

        if (!win)
        {
            SceneLoader.Instance.LoadScene(Scene.MainMenu);
        }

        yield return lastText.DOFade(1, fadeDuration).WaitForCompletion();
        yield return new WaitForSeconds(waitTime);
        lastBackground.DOFade(1, endDuration);
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

    private void OnKey(InputKey key)
    {
        if (keysToPress.Count <= 0)
        {
            return;
        }

        Instantiate(arrowPrefab, Vector3.zero, Quaternion.Euler(GetArrowEuler(key)), arrowParent);

        win = key == keysToPress[0];
        if (win)
        {
            keysToPress.RemoveAt(0);
        }
        else
        {
            keysToPress.Clear();
        }
    }
}
