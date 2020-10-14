using UnityEngine;

namespace IonCubeGenerator.Mono
{
    internal class TriggerFixer : MonoBehaviour
    {
        private BoxCollider _collsion;
        private Pickupable _pickupable;

        private void OnEnable()
        {
            if (isActiveAndEnabled)
            {
                if (_collsion == null)
                {
                    _collsion = gameObject.GetComponentInChildren<BoxCollider>();
                }

                if (_pickupable == null)
                {
                    _pickupable = gameObject.GetComponentInChildren<Pickupable>();
                    if (!Player.main.IsInSub())
                    {
                        _collsion.isTrigger = false;
                    }
                    else
                    {
                        _pickupable.pickedUpEvent.AddHandler(gameObject, PickupEvent);
                    }

                }
            }
        }

        private void PickupEvent(Pickupable pickupable)
        {
            _collsion.isTrigger = false;
        }
    }
}
