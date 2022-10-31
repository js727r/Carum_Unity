using System;
using System.Collections;
using Carum.Interior;
using Carum.Util;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Carum.Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        public Image image;
        public TMP_Text tmPro;

        private Furniture _furniture;
        private GameObject _itemObject;
        [SerializeField]
        private Texture2D _texture2D;

        // void Awake()
        // {
        //     // resource에 해당하는 가구 프리팹 로드
        //     string path = resource;
        //     _itemObject = Resources.Load<GameObject>(path);
        //     
        // }

        /// <summary>
        /// 아이템 프리뷰 설정
        /// </summary>
        // void OnEnable()
        // {
        //     if (_itemObject && tmPro)
        //     {
        //         _image = GetComponent<Image>();
        //         _image.sprite = PreviewUtil.GetObjectPreview(_itemObject);
        //         tmPro.SetText(_itemObject.name);
        //     }
        // }

        /// <summary>
        /// 아이템 선택
        /// </summary>
        public void PickItem()
        {
            if (!_itemObject) return;

            InteriorManager interiorManager = InteriorManager.GetInstance();

            GameObject instance = Instantiate(_itemObject, Vector3.zero, Quaternion.Euler(0, 135, 0));
            instance.AddComponent<Interior.Interior>();
            instance.AddComponent<Outline>();

            Interior.Interior interior = instance.GetComponent<Interior.Interior>();
            interior.Init(_furniture.id, 0);

            interiorManager.SetPickedInterior(interior);
            interiorManager.ChangeMode(PutMode.Raycast);
        }

        public void SetItem(Furniture furniture)
        {
            _furniture = furniture;
            Debug.Log("Set item");
        }

        void OnEnable()
        {
            // set preview
            StartCoroutine(LoadAsset());
        }

        // private void Update()
        // {
        //     if (_itemObject && !_texture2D && !AssetPreview.IsLoadingAssetPreview(_itemObject.GetInstanceID()))
        //     {
        //         Debug.Log("request preview:"+_itemObject.GetInstanceID());
        //         _texture2D = PreviewUtil.GetObjectPreview(_itemObject);
        //     }
        //
        //     if (_texture2D && !image.sprite)
        //     {
        //         image.sprite = Sprite.Create(_texture2D, new Rect(0, 0, _texture2D.width, _texture2D.height),
        //         new Vector2(0.5f, 0.5f));
        //     }
        // }

        public IEnumerator LoadAsset()
        {
            if (_furniture == null) yield break;
            Debug.Log("path:"+_furniture.GetResourcePath());
            Debug.Log("forced path:"+"Assets/Prefabs/Furnitures/Interior2/Armchair_1.prefab");
            AsyncOperationHandle<GameObject> opHandle = Addressables.LoadAssetAsync<GameObject>(_furniture.GetResourcePath());
            // AsyncOperationHandle<GameObject> opHandle = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Furnitures/Interior2/Armchair_1.prefab");

            // ReSharper disable once HeapView.BoxingAllocation
            yield return opHandle;
            if (opHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _itemObject = opHandle.Result;
                tmPro.SetText(_furniture.name);
            }
        }
    }
}