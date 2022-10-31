using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Carum.Util
{
    public class PreviewUtil : MonoBehaviour
    {
        public static Texture2D GetObjectPreview(GameObject targetObject)
        {
            if (!targetObject)
                Debug.LogError("프리뷰 로드할 오브젝트가 없는데?");

            Texture2D texture = null;
            int counter = 0;
            while (texture == null && counter++ < 20)
                texture = AssetPreview.GetAssetPreview(targetObject);
            return texture;

            // if (!texture)
            //     Debug.LogError("텍스쳐 로드 안됐는데?");
            //
            //
            // Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            //     new Vector2(0.5f, 0.5f));
            // return sprite;
        }
    }
}