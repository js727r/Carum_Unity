using UnityEngine;

namespace Carum.Interior
{
    public interface IPuttable
    {
        public void Put(Vector3 position);
        public void Move(Vector3 position);
        public void Rotate(Vector3 rotationVector);
        public bool SetPutMode(bool mode);
    }
}
