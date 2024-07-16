using DG.Tweening;
using Photon.PhotonUnityNetworking.Code.Common;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.NetworkData;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.PlayersStats;
using PunNetwork.Services.RoomPlayer;
using Services.Input;
using Services.PhotonPool;
using UnityEngine;
using Utils.Extensions;
using Zenject;
using static Utils.Enumerators;

namespace PunNetwork.Views.Player
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerView : MonoBehaviour, IPunInstantiateMagicCallback
    {
        private const float MaxHealthPoints = 100;
        public TeamRole TeamRole { get; private set; }
        public PhotonView PhotonView { get; private set; }

        [SerializeField] private MeshRenderer _teamMarker;
        [SerializeField] private ParticleSystem _destruction;
        [SerializeField] private PlayerUI _playerUI;
        [SerializeField] private EnemiesTriggerCollider _enemiesTriggerCollider;
        [SerializeField] private Collider _collider;

        private IInputService _inputService;
        private IPhotonPoolService _photonPoolService;
        private IRoomPlayersService _roomPlayersService;

        private Rigidbody _rigidbody;
        private CharacterController _characterController;
        private MeshRenderer[] _renderers;

        private float _rotationSpeed = 150f;
        private float _speed = 3f;
        private float _initialShootingDelay;
        private float _rotation;
        private float _acceleration;
        private float _shootingTimer;
        private bool _controllable = true;
        private Tween _animationTween;
        private bool _isFiring;

        public float CurrentHealthPoints { get; private set; }
        public Photon.Realtime.Player Player;
        private IPlayersStatsService _playersStatsService;

        [Inject]
        private void Construct
        (
            IInputService inputService,
            IPhotonPoolService photonPoolService,
            IRoomPlayersService roomPlayersService,
            IPlayersStatsService playersStatsService
        )
        {
            _inputService = inputService;
            _photonPoolService = photonPoolService;
            _roomPlayersService = roomPlayersService;
            _playersStatsService = playersStatsService;
        }

        public void SubscribeOnInput()
        {
            if (PhotonView.IsMine)
                _inputService.FireTriggeredEvent += FireJoystickTriggered;
        }

        #region UNITY

        public void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            _rigidbody = GetComponent<Rigidbody>();
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _characterController = GetComponent<CharacterController>();

            Debug.Log("InputService" + nameof(_inputService));
        }

        private void OnDestroy()
        {
            if (PhotonView.IsMine)
                _inputService.FireTriggeredEvent -= FireJoystickTriggered;
        }

        private void FireJoystickTriggered(bool state)
        {
            if (state)
                StartFiring();
            else
            {
                _isFiring = false;
                if (_animationTween != null)
                {
                    _animationTween.Kill();
                    _animationTween = null;
                }
            }
        }

        private void StartFiring()
        {
            _isFiring = true;
            _initialShootingDelay = 0.25f;
        }

        private void Update()
        {
            if (!CanControl())
                return;

            HandleMovement();
            HandleShooting();
        }

        private bool CanControl()
        {
            return PhotonView.IsMine && gameObject.activeInHierarchy;
        }

        private void HandleMovement()
        {
            var move = new Vector3(_inputService.MoveAxis.x, 0, _inputService.MoveAxis.y);
            if (move.sqrMagnitude > 1)
                move.Normalize();

            move *= _speed;
            _characterController.Move(move * Time.deltaTime);

            HandleRotation(move);
        }

        private void HandleRotation(Vector3 move)
        {
            const float lookAxisThreshold = .1f;

            if (_isFiring)
            {
                if (_inputService.LookAxis.sqrMagnitude < lookAxisThreshold * lookAxisThreshold)
                {
                    if (_enemiesTriggerCollider.TryGetNearestEnemy(out var enemy))
                        RotateTowards(enemy.position - transform.position);
                }
                else
                    RotateTowards(new Vector3(_inputService.LookAxis.x, 0, _inputService.LookAxis.y));
            }
            else
                RotateTowards(move);
        }

        private void RotateTowards(Vector3 direction)
        {
            direction.y = 0;

            if (direction == Vector3.zero) return;
            var newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * _rotationSpeed);
        }

        private void HandleShooting()
        {
            if (!_isFiring)
                return;

            if (_initialShootingDelay > 0)
            {
                _initialShootingDelay -= Time.deltaTime;
                if (_initialShootingDelay <= 0)
                    _shootingTimer = 0;
                return;
            }

            if (_shootingTimer <= 0)
            {
                _shootingTimer = .2f;

                var position = transform.position;
                var rotation = transform.rotation;

                var bullet = _photonPoolService.ActivatePoolItem<Bullet.Bullet>(
                    Enumerators.GameObjectEntryKey.Bullet.ToString(),
                    position,
                    rotation);
                bullet.Fire(position);
            }

            if (_shootingTimer > 0)
                _shootingTimer -= Time.deltaTime;
        }

        #endregion

        #region PUN CALLBACKS

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            Player = info.Sender;
            _roomPlayersService.SendLocalPlayersSpawned(info.Sender, this);
            SetupInfo();
        }


        [PunRPC]
        public void RegisterHit(float damage)
        {
            if (!PhotonView.IsMine)
                return;
            var newHealthPoints = CurrentHealthPoints - damage;
            var resultHealthPoints = newHealthPoints <= 0 ? 0 : newHealthPoints;

            _playersStatsService.SendPlayerHp(resultHealthPoints);

            if (resultHealthPoints == 0)
                PhotonView.RPC(nameof(DestroyPlayer), RpcTarget.All);
        }

        [PunRPC]
        public void DestroyPlayer()
        {
            _playerUI.gameObject.SetActive(false);
            _collider.enabled = false;
            _characterController.enabled = false;
            _controllable = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            foreach (var meshRenderer in _renderers)
                meshRenderer.enabled = false;

            _destruction.Play();
        }

        #endregion

        public void SetTeamRole(TeamRole role)
        {
            TeamRole = role;
            Debug.Log($"{role}");
            var markerColor = TeamMarker.GetColor(role);
            _teamMarker.material.color = markerColor;
        }

        private void SetupInfo()
        {
            _playerUI.SetNickName(_roomPlayersService.GetPlayerInfo(Player).ImmutableDataVo.Nickname);
        }

        public void UpdateHealthPoints(float healthPoints)
        {
            CurrentHealthPoints = healthPoints;
            _playerUI.SetHealthPoints(healthPoints, MaxHealthPoints);
        }
    }

    public static class TeamMarker
    {
        public static Color GetColor(TeamRole role) =>
            role switch
            {
                TeamRole.MyPlayer => Color.green,
                TeamRole.AllyPlayer => Color.blue,
                TeamRole.EnemyPlayer => Color.red
            };
    }
}