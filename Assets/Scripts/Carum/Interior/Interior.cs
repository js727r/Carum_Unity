using System;
using UnityEngine;

// ReSharper disable Unity.InefficientPropertyAccess

namespace Carum.Interior
{
    public class Interior : MonoBehaviour, IPuttable
    {
        private long _furnitureId;
        private long _id;
        private Outline _outline;
        private LayerMask _layerMask;

        private void Start()
        {
            _outline = GetComponent<Outline>();
            _layerMask = LayerMask.GetMask("PickedInterior");
        }

        public void Init(long furnitureId, long id)
        {
            this._furnitureId = furnitureId;
            this._id = id;
        }

        /// <summary>
        /// 해당 위치로 옮기기
        /// </summary>
        /// <param name="position">옮길 위치</param>
        public void Put(Vector3 position)
        {
            transform.SetPositionAndRotation(position, transform.rotation);
        }

        /// <summary>
        /// 레이캐스트가 맞은 지점에 배치
        /// </summary>
        /// <param name="hit">레이캐스트 지점</param>
        public void PutFromRaycast(RaycastHit hit)
        {
            // 히트지점 노멀벡터
            Vector3 hitNormal = hit.normal;
            Transform t = transform;
            Vector3 pivotPoint = t.position;

            Debug.DrawRay(pivotPoint - hitNormal * 5f, hitNormal * 5f, Color.green);

            Ray ray = new Ray(pivotPoint - hitNormal * 15f, hitNormal);

            if (Physics.SphereCast(ray,10f,out var hitInfo, 20f,_layerMask))
            {
                Vector3 delta = t.position-hitInfo.point;
                
                Put(hit.point+delta);
            }
        }

        /// <summary>
        /// 방향벡터만큼 이동
        /// </summary>
        /// <param name="moveVector">이동할 방향</param>
        public void Move(Vector3 moveVector)
        {
            transform.SetPositionAndRotation(transform.position + moveVector, transform.rotation);
        }

        /// <summary>
        /// 각도벡터만큼 각도 변경
        /// </summary>
        /// <param name="rotationVector">변경할 각도</param>
        public void Rotate(Vector3 rotationVector)
        {
            transform.Rotate(rotationVector, Space.World);
        }


        public void SetOutline(bool active)
        {
            if (_outline)
                _outline.enabled = active;
        }
    }
}