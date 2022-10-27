using UnityEngine;

// ReSharper disable Unity.InefficientPropertyAccess

namespace Carum.Interior
{
    public class Interior : MonoBehaviour, IPuttable
    {
        private long _furnitureId;
        private long _id;
        private bool _putMode;


        public void Init(long furnitureId, long id)
        {
            this._furnitureId = furnitureId;
            this._id = id;
        }

        public void Put(Vector3 position)
        {
            transform.SetPositionAndRotation(position, transform.rotation);
        }

        public void Move(Vector3 moveVector)
        {
            transform.SetPositionAndRotation(transform.position + moveVector, transform.rotation);
        }

        public void Rotate(Vector3 rotationVector)
        {
            transform.Rotate(rotationVector,Space.World);
            
        }

        public bool SetPutMode(bool mode)
        {
            throw new System.NotImplementedException();
        }
    }
}