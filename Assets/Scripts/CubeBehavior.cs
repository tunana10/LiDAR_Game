using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CubeBehavior : MonoBehaviour, IPointerEnterHandler
{
    public enum ColorType { Green, Red, Blue, Black }
    public ColorType colorType;
    public Image image;
    public CubeManager manager;

    [Header("音效設定")]
    public AudioSource successSound;
    public AudioSource failureSound;

    void Awake()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    public void SetColor(Color color)
    {
        image.color = color;

        if (color == Color.blue)
            colorType = ColorType.Blue;
        else if (color == Color.red)
            colorType = ColorType.Red;
        else if (color == Color.green)
            colorType = ColorType.Green;
        else
            colorType = ColorType.Black;
    }

    // 滑鼠移入觸發
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (colorType == ColorType.Blue)
        {
            SetColor(manager.blackColor); // 得分 cube → 黑色
            successSound.Play();
        }
        if(colorType == ColorType.Red)
        {
            SetColor(manager.blackColor); // 得分 cube → 黑色
            failureSound.Play();
        }
    }
}
