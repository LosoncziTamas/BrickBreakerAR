using ArBreakout.Game;
using ArBreakout.GamePhysics;
using ArBreakout.Misc;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.PowerUps
{
    public class Collectable : MonoBehaviour
    {
        public const string GameObjectTag = "Collectable";

        public PowerUp PowerUp { get; private set; }

        [SerializeField] private MovementProperties _movementProperties;
        [SerializeField] private bool _rotateAnimation;
        [SerializeField] private GameEntities _gameEntities;

        private Vector3 _velocity;
        private Vector3 _acceleration;
        private Vector3 _originalScale;

        private bool _destroyed;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _gameEntities.Add(this);
        }

        public void Init(PowerUp powerUp)
        {
            PowerUp = powerUp;
        }

        private void FixedUpdate()
        {
            if (_destroyed)
            {
                return;
            }

            _acceleration = Vector3.back;
            _acceleration *= _movementProperties.speed;
            _acceleration += -_movementProperties.drag * _velocity;

            _velocity += BreakoutPhysics.CalculateVelocityDelta(_acceleration);
            transform.localPosition += BreakoutPhysics.CalculateMovementDelta(_acceleration, _velocity);
            transform.Rotate(Vector3.right, _movementProperties.rotation, Space.Self);
        }

        public void AnimateAppearance()
        {
            transform.localScale = Vector3.one * 0.1f;
            transform.AnimatePunchScale(_originalScale * 0.9f, Ease.Linear, 0.4f);
            if (_rotateAnimation)
            {
                transform.DOPunchRotation(Vector3.forward * 360f, 1.0f, 1, 0.5f);
            }
        }

        public void Destroy()
        {
            if (!_destroyed)
            {
                Destroy(gameObject);
            }

            _destroyed = true;
        }

        private void OnDestroy()
        {
            _gameEntities.Remove(this);
        }
    }
}