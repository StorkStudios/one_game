using DG.Tweening;
using StorkStudios.CoreNest;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FNFGame : MonoBehaviour
{
    [System.Serializable]
    private struct KeyFrame
    {
        public InputKey key;
        public float enemyDelay;
        public float playerDelay;
        public AudioClip clip;
    }

    [SerializeField]
    private SerializedDictionary<InputKey, RectTransform> enemyArrows;
    [SerializeField]
    private SerializedDictionary<InputKey, RectTransform> playerArrows;

    [SerializeField]
    private List<KeyFrame> keys;

    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private RectTransform spawnHeight;
    [SerializeField]
    private float hitboxHalfsize;
    [SerializeField]
    private float fadeInDuration;
    [SerializeField]
    private Image arrowPrefab;
    [SerializeField]
    private float moveDuration;
    [SerializeField]
    private FNFCharacter playerCharacter;
    [SerializeField]
    private FNFCharacter enemyCharacter;
    [SerializeField]
    private float animationDuration;
    [SerializeField]
    private AudioClip wrongClip;

    private List<(RectTransform transform, InputKey key)> enemy = new List<(RectTransform transform, InputKey key)>();
    private List<(RectTransform transform, InputKey key)> player = new List<(RectTransform transform, InputKey key)>();

    private float speed = 0;

    private int currentSoundIndex = 0;

    private bool isFinished = false;
    private bool isLost = false;
    private bool win = false;

    private float GetWorldSpaceHeight(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // corners[1] = top-left, corners[0] = bottom-left
        float worldHeight = Vector3.Distance(corners[1], corners[0]);

        return worldHeight;
    }

    private IEnumerator Start()
    {
        var a = SceneLoader.Instance;

        InputHelper.GetInputAction(InputKey.Up).performed += OnInputUp;
        InputHelper.GetInputAction(InputKey.Down).performed += OnInputDown;
        InputHelper.GetInputAction(InputKey.Left).performed += OnInputLeft;
        InputHelper.GetInputAction(InputKey.Right).performed += OnInputRight;

        source.Play();
        StartCoroutine(GameCoroutine());

        yield return null;

        RectTransform rect = enemyArrows[InputKey.Up];

        float height = GetWorldSpaceHeight(rect);

        /*clickRange.Min = rect.position.y;// - (height * hitboxHalfsize);
        clickRange.Max = rect.position.y;// + (height * hitboxHalfsize);*/

        float distance = Mathf.Abs(rect.position.y - spawnHeight.position.y);
        speed = distance / moveDuration;
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
        if (player.Count <= 0)
        {
            return;
        }

        float posY = enemyArrows[InputKey.Up].position.y;
        float y = posY - player[0].transform.position.y;
        float height = GetWorldSpaceHeight(enemyArrows[InputKey.Up]);

        if (y < -1 * height * hitboxHalfsize || height * hitboxHalfsize < y)
        {
            return;
        }

        if (player[0].key != key)
        {
            Lose();
        }
        else
        {
            playerCharacter.SetSpriteForDuration(FNFCharacter.KeyToState(key), animationDuration);
            source.PlayOneShot(keys[currentSoundIndex].clip);
            currentSoundIndex++;

            Destroy(player[0].transform.gameObject);
            player.RemoveAt(0);

            CheckWin();
        }
    }

    private Quaternion GetArrowRotation(InputKey key)
    {
        return key switch
        {
            InputKey.Up => Quaternion.Euler(0, 0, 180),
            InputKey.Down => Quaternion.Euler(0, 0, 0),
            InputKey.Left => Quaternion.Euler(0, 0, 270),
            InputKey.Right => Quaternion.Euler(0, 0, 90),
            _ => throw new System.NotImplementedException(),
        };
    }

    private IEnumerator GameCoroutine()
    {
        bool first = true;
        foreach (KeyFrame frame in keys)
        {
            if (first)
            {
                first = false;
                
                yield return new WaitWhile(() => source.time < frame.enemyDelay - moveDuration);
            }
            else
            {
                yield return new WaitForSeconds(frame.enemyDelay);
            }

            float x = enemyArrows[frame.key].position.x;
            float y = spawnHeight.position.y;

            Image image = Instantiate(arrowPrefab.gameObject, new Vector3(x, y, 0), GetArrowRotation(frame.key), transform).GetComponent<Image>();
            enemy.Add((image.transform as RectTransform, frame.key));
            image.color = Color.clear;
            image.DOColor(Color.white, fadeInDuration);
        }

        foreach (KeyFrame frame in keys)
        {
            yield return new WaitForSeconds(frame.playerDelay);

            float x = playerArrows[frame.key].position.x;
            float y = spawnHeight.position.y;

            Image image = Instantiate(arrowPrefab.gameObject, new Vector3(x, y, 0), GetArrowRotation(frame.key), transform).GetComponent<Image>();
            player.Add((image.transform as RectTransform, frame.key));
            image.color = Color.clear;
            image.DOColor(Color.white, fadeInDuration);
        }

        isFinished = true;
    }

    private void Update()
    {
        if (win && !source.isPlaying)
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

        foreach ((RectTransform arrow, InputKey key) in enemy)
        {
            arrow.position += speed * Time.deltaTime * Vector3.up;
        }

        if (enemy.Count > 0)
        {
            float posY = enemyArrows[InputKey.Up].position.y;
            float y = posY - enemy[0].transform.position.y;
            if (y <= 0)
            {
                enemyCharacter.SetSpriteForDuration(FNFCharacter.KeyToState(enemy[0].key), animationDuration);

                Destroy(enemy[0].transform.gameObject);
                enemy.RemoveAt(0);
            }
        }

        foreach ((RectTransform arrow, InputKey key) in player)
        {
            arrow.position += speed * Time.deltaTime * Vector3.up;
        }

        if (player.Count > 0)
        {
            float posY = enemyArrows[InputKey.Up].position.y;
            float y = posY - player[0].transform.position.y;
            float height = GetWorldSpaceHeight(enemyArrows[InputKey.Up]);
            if (y < -1 * height * hitboxHalfsize)
            {
                Lose();
            }
        }
    }

    private void CheckWin()
    {
        if (!isFinished)
        {
            return;
        }

        win = true;
    }

    private void Lose()
    {
        if (isLost)
        {
            return;
        }
        isLost = true;

        playerCharacter.SetSpriteForDuration(FNFCharacter.State.Wrong, 20);
        enemyCharacter.SetSpriteForDuration(FNFCharacter.State.Wrong, 20);

        source.PlayOneShot(wrongClip);

        this.CallDelayed(2, () => 
        {
            source.Stop();
            SceneLoader.Instance.ReloadScene();
        });
    }
}
