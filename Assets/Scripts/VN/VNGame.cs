using DG.Tweening;
using StorkStudios.CoreNest;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class VNGame : MonoBehaviour
{
    [System.Serializable]
    private struct Part
    {
        public string name;
        [TextArea]
        public string text;
        public Transform actor;
        public bool askQuestion;
        public AudioClip clip;
        public bool switchKid;
    }

    [System.Serializable]
    private struct Question
    {
        public InputKey goodAnswer;
        public SerializedDictionary<InputKey, string> questions;
    }

    [System.Serializable]
    private struct AnswerRef
    {
        public CanvasGroup group;
        public TextMeshProUGUI text;
    }

    [SerializeField]
    private List<Part> parts;
    [SerializeField]
    private List<Question> questions;
    [SerializeField]
    private SerializedDictionary<InputKey, AnswerRef> answers;
    [SerializeField]
    private Part badAnswer;
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private CanvasGroup group;
    [SerializeField]
    private CanvasGroup kid;
    [SerializeField]
    private TextMeshProUGUI label;
    [SerializeField]
    private TextMeshProUGUI bigText;
    [SerializeField]
    private float fadeDuration;

    private InputKey? answer;

    private void Start()
    {
        var a = SceneLoader.Instance;

        InputHelper.GetInputAction(InputKey.Up).performed += OnInputUp;
        InputHelper.GetInputAction(InputKey.Down).performed += OnInputDown;
        InputHelper.GetInputAction(InputKey.Left).performed += OnInputLeft;
        InputHelper.GetInputAction(InputKey.Right).performed += OnInputRight;

        group.alpha = 0;
        kid.alpha = 0;

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
        if (!answer.HasValue)
        {
            return;
        }

        if (answer.Value == key)
        {
            answer = null;
        }
        else
        {
            answer = key;
        }
    }

    private IEnumerator RenderPart(Part part)
    {
        label.text = part.name;
        bigText.text = part.text;
        group.DOFade(1, fadeDuration);
        yield return new WaitForSeconds(0.5f);

        float duration = 7;
        if (part.clip != null)
        {
            duration = part.clip.length;
            source.PlayOneShot(part.clip);
        }
        if (part.actor != null)
        {
            part.actor.DOShakeRotation(duration);
        }
        if (part.switchKid)
        {
            float a = kid.alpha > 0 ? 0 : 1;
            kid.DOFade(a, fadeDuration);
        }
        yield return new WaitForSeconds(duration + 0.5f);
    }

    private IEnumerator Game()
    {
        yield return new WaitForSeconds(1);

        IEnumerator<Question> questionsEnumerator = questions.GetEnumerator();

        int skip = SceneLoader.Instance.flag ? 17 : 0;
        SceneLoader.Instance.flag = false;
        foreach (Part part in parts.Skip(skip))
        {
            yield return RenderPart(part);

            if (part.askQuestion && questionsEnumerator.MoveNext())
            {
                group.DOFade(0, fadeDuration);

                Question question = questionsEnumerator.Current;

                answer = question.goodAnswer;

                foreach ((InputKey key, AnswerRef answerRef) in answers)
                {
                    answerRef.text.text = question.questions[key];
                    answerRef.group.DOFade(1, fadeDuration);
                }

                yield return new WaitWhile(() => answer == question.goodAnswer);

                if (answer.HasValue)
                {
                    foreach ((InputKey key, AnswerRef answerRef) in answers.Where(e => e.Key != answer.Value))
                    {
                        answerRef.group.DOFade(0, fadeDuration);
                    }

                    yield return new WaitForSeconds(2);

                    answers[answer.Value].group.DOFade(0, fadeDuration);

                    yield return RenderPart(badAnswer);

                    group.alpha = 0;

                    SceneLoader.Instance.flag = true;
                    SceneLoader.Instance.ReloadScene();
                }
                else
                {
                    foreach ((InputKey key, AnswerRef answerRef) in answers.Where(e => e.Key != question.goodAnswer))
                    {
                        answerRef.group.DOFade(0, fadeDuration);
                    }

                    yield return new WaitForSeconds(2);

                    answers[question.goodAnswer].group.DOFade(0, fadeDuration);
                }
            }
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
