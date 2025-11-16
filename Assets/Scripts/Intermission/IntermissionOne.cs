using DG.Tweening;
using StorkStudios.CoreNest;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntermissionOne : MonoBehaviour
{
    [System.Serializable]
    private struct Frame
    {
        [TextArea]
        public string text;
        public float duration;
    }

    [SerializeField]
    private List<Frame> frames;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private float fadeDuration;

    private IEnumerator Start()
    {
        var a = SceneLoader.Instance;

        Color color = text.color;
        foreach (Frame frame in frames)
        {
            text.text = frame.text;

            color.a = 1;
            yield return text.DOColor(color, fadeDuration).WaitForCompletion();

            yield return new WaitForSeconds(frame.duration);

            color.a = 0;
            yield return text.DOColor(color, fadeDuration).WaitForCompletion();
        }

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
