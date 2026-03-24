using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SoundButton : MonoBehaviour
{
    [SerializeField]
    private Texture2D soundOnImage;

    [SerializeField]
    private Texture2D soundOffImage;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Image soundImage;
    private bool isPlayingPrev;
    private Sprite soundOnSprite;
    private Sprite soundOffSprite;

    private void Start() {
        soundOnSprite = CreateSprite(soundOnImage);
        soundOffSprite = CreateSprite(soundOffImage);
        soundImage.sprite = audioSource.isPlaying ? soundOffSprite : soundOnSprite;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (audioSource.isPlaying) {
                audioSource.Stop();
                soundImage.sprite = soundOnSprite;
            }
            else {
                audioSource.Play();
                soundImage.sprite = soundOffSprite;
            }
        });
    }

    private void OnDestroy() {
        if (soundOnSprite != null) {
            Destroy(soundOnSprite);
        }

        if (soundOffSprite != null) {
            Destroy(soundOffSprite);
        }
    }

    public void Pause() {
        isPlayingPrev = audioSource.isPlaying;
        if (audioSource.isPlaying) {
            audioSource.Pause();
        }
    }

    public void Resume() {
        if (isPlayingPrev) {
            audioSource.UnPause();
        }
    }

    private static Sprite CreateSprite(Texture2D texture) =>
    Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
}
