using Carum.Interior;
using Carum.Util;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Carum.Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        public string resource;
        private Image _image;
        private long _furnitureId;
        private GameObject _itemObject;

        private TMP_Text _tmPro;

        void Awake()
        {
            // resource에 해당하는 가구 프리팹 로드
            string path = resource;
            _itemObject = Resources.Load<GameObject>(path);
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            _tmPro = GetComponentInChildren<TMP_Text>();
            if (_itemObject && _tmPro)
            {
                _image = GetComponent<Image>();
                _image.sprite = PreviewUtil.GetObjectPreview(_itemObject);
                _tmPro.SetText(_itemObject.name);
            }
        }

        public void PickItem()
        {
            if (!_itemObject) return;

            InteriorManager interiorManager = InteriorManager.GetInstance();

            GameObject instance = Instantiate(_itemObject, Vector3.zero, Quaternion.identity);
            instance.AddComponent<Interior.Interior>();

            Interior.Interior interior = instance.GetComponent<Interior.Interior>();
            interior.Init(_furnitureId, 0);

            interiorManager.SetPickedInterior(interior);
            interiorManager.ChangeMode(PutMode.Raycast);
        }
    }
}