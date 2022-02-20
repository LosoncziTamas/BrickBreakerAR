using System.Collections.Generic;
using ArBreakout.Levels;
using ArBreakout.Misc;
using ArBreakout.PowerUps;
using UnityEngine;
using UnityEngine.Assertions;
using static ArBreakout.GamePhysics.BreakoutPhysics;

namespace ArBreakout.Game
{
    public class LevelRoot : MonoBehaviour
    {
        public const string ObjectName = "LevelRoot";
        private const int LevelDimension = 9;

        [SerializeField] private BrickPool _brickPoolPrefab;
        [SerializeField] private BallBehaviour _ballPrefab;
        [SerializeField] private GameObject _paddleParentPrefab;
        [SerializeField] private WallBehaviour _wallBehaviourPrefab;
        [SerializeField] private Gap _gapPrefab;
        [SerializeField] private ColorPalette _colorPalette;
        [SerializeField] private Levels.Levels _levels;
        [SerializeField] private BrickPool _brickPool;
        
        private readonly List<BrickBehaviour> _brickReferences = new();

        public BallBehaviour BallBehaviour { get; private set; }
        public PaddleBehaviour Paddle { get; private set; }
        public int InitialBrickCount => _brickReferences.Count;

        public bool Initialized { get; private set; }

        #if false
        public void InitWithLevel(Transform levelParent, LevelLoader.ParsedLevel level)
        {
            Assert.IsNull(_gameWorldRoot);

            _gameWorldRoot = new GameObject(ObjectName);
            _gameWorldRoot.transform.SetParent(levelParent);
            _gameWorldRoot.transform.localScale = Vector3.one;
            _gameWorldRoot.transform.localRotation = Quaternion.identity;
            _gameWorldRoot.transform.localPosition = Vector3.zero;

            _brickPool = Instantiate(_brickPoolPrefab, levelParent);
            _brickPool.gameObject.SetActive(false);

            InitWallsAndGap();
            Paddle = InitPaddle();
            BallBehaviour = InitBall(Paddle.transform);
            InitBricks(level);

            Initialized = true;
        }
        
        public void DestroySelf()
        {
            Assert.IsNotNull(_gameWorldRoot, "Trying to destroy a non-existing game world.");

            _brickReferences.Clear();
            Initialized = false;
            Paddle = null;
            BallBehaviour = null;

            Destroy(_gameWorldRoot);
            Destroy(_brickPool);
        }
        
        
        private void InitBricks(LevelLoader.ParsedLevel parsedLevel)
        {
            var count = parsedLevel.bricksProps.Count;
            var paletteColorCount = _colorPalette.Colors.Length;
            foreach (var bricksProp in parsedLevel.bricksProps)
            {
                var brick = _brickPool.GetBrick();
                brick.gameObject.name = $"Brick {bricksProp.Location}";
                var brickTransform = brick.transform;
                brickTransform.SetParent(_gameWorldRoot.transform, false);
                brickTransform.localPosition = bricksProp.Location;
                brickTransform.localRotation = Quaternion.identity;
                // Scale of the brick is set with the animation.
                var lineIndex = bricksProp.LineIdx;
                var color = _colorPalette.Colors[lineIndex % paletteColorCount];
                brick.Init(bricksProp, color, count);
                _brickReferences.Add(brick);
            }
        }
                
        public void SetupLevel(LevelLoader.ParsedLevel level)
        {
            Assert.IsTrue(Initialized);
            Assert.IsNotNull(Paddle);
            Assert.IsNotNull(BallBehaviour);

            foreach (var brick in _brickReferences)
            {
                if (brick)
                {
                    _brickPool.ReturnBrick(brick);
                }
            }

            _brickReferences.Clear();
            InitBricks(level);
            Paddle.ResetToDefaults();
            GamePlayUtils.AnchorBallToPaddle(BallBehaviour, Paddle);
        }
        
        
        #endif

        public void InitNewLevel()
        {
            InitWallsAndGap();
            Paddle = InitPaddle();
            BallBehaviour = InitBall(Paddle.transform);
            
            var toBeInitialized = _levels.Selected;
            var cells = toBeInitialized.Layout.GetCells();
            var rowCount = cells.GetLength(0);
            for (var row = 0; row < rowCount; row++) 
            {
                for (var col = 0; col < cells.GetLength(1); col++)
                {
                    // Starts at top left, goes to bottom right.
                    var c = cells[row, col];
                    if (!char.IsWhiteSpace(c))
                    {
                        var pos = new Vector3
                        {
                            x = -0.5f * LevelDimension + col + 0.5f,
                            y = 0.5f,
                            z = 0.5f * LevelDimension - row + 3.0f
                        };
                        
                        var brick = _brickPool.GetBrick();
                        brick.gameObject.name = $"Brick {pos}";
                        var brickTransform = brick.transform;
                        brickTransform.SetParent(transform, false);
                        brickTransform.localPosition = pos;
                        brickTransform.localRotation = Quaternion.identity;

                        var bricksProp = new LevelLoader.BrickProps
                        {
                            HitPoints = 3,
                            LineIdx = row,
                            PowerUp = PowerUp.None
                        };
                        // Scale of the brick is set with the animation.
                        var lineIndex = bricksProp.LineIdx;
                        var color = _colorPalette.Colors[lineIndex % 5];
                        brick.Init(bricksProp, color, rowCount);
                        _brickReferences.Add(brick);
                    }
                }
            }
        }



        private BallBehaviour InitBall(Transform paddleTransform)
        {
            var ballInstance = Instantiate(_ballPrefab, transform);
            GamePlayUtils.CenterAboveObject(ballInstance.transform, paddleTransform);
            ballInstance.transform.SetParent(paddleTransform.parent);

            return ballInstance;
        }

        private PaddleBehaviour InitPaddle()
        {
            var paddleParent = Instantiate(_paddleParentPrefab, transform);
            var playerInstance = paddleParent.GetComponentInChildren<PaddleBehaviour>();

            // Placing player at the bottom of the scene.
            var playerOffset = Vector3.forward * Mathf.Floor(LevelDimY * 0.5f) + Vector3.down * 0.5f;
            playerInstance.transform.Translate(playerInstance.transform.TransformVector(playerOffset), Space.Self);
            playerInstance.StoreCurrentPositionAsStartPosition();

            return playerInstance;
        }

        private void InitWallsAndGap()
        {
            var wall = Instantiate(_wallBehaviourPrefab, transform);
            var gap = Instantiate(_gapPrefab, transform);
        }
    }
}