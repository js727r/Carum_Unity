using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Carum.Util
{
    public class PreviewUtil : MonoBehaviour
    {
        public static Sprite GetObjectPreview(GameObject targetObject)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(targetObject);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
            return sprite;
        }
    }
}