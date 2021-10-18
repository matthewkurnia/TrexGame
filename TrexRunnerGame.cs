using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TrexGame.Entities;
using TrexGame.Extensions;
using TrexGame.Graphics;
using TrexGame.System;

namespace TrexGame
{
    public enum GameState
    {
        Initial,
        Transition,
        Playing,
        GameOver
    }

    public class TrexRunnerGame : Game
    {
        public enum DisplayMode
        {
            Default,
            Zoomed
        }

        public const string GameTitle = "T-Rex Runner";

        private const string AssetNameSpritesheet = "TrexSpritesheet";
        private const string AssetNameHitSound = "hit";
        private const string AssetNameScoreReachedSound = "score-reached";
        private const string AssetNameButtonPressedSound = "button-press";

        private Matrix _transformMatrix = Matrix.Identity;
        private const int _ZoomFactor = 2;
        public const int WindowHeight = 150;
        public const int WindowWidth = 600;
        public float ZoomFactor => WindowDisplayMode == DisplayMode.Default ? 1 : _ZoomFactor;

        public const int TrexStartPosY = WindowHeight - 16;
        public const int TrexStartPosX = 1;
        private const string SavePath = "Save.dat";
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Trex _trex;
        private ScoreBoard _scoreBoard;

        private SoundEffect _sfxHit;
        private SoundEffect _sfxScoreReached;
        private SoundEffect _sfxButtonPressed;

        private Texture2D _spriteSheetTexture;
        private Texture2D _fadeInTexture;
        private Texture2D _invertedSpriteSheet;

        private float _fadeInTexturePosX;

        private InputController _inputController;
        private EntityManager _entityManager;
        private GroundManager _groundManager;
        private ObstacleManager _obstacleManager;
        private SkyManager _skyManager;

        private KeyboardState _previousKeyboardState;

        private DateTime _highScoreDate;

        private GameOverScreen _gameOverScreen;

        public GameState State;

        public DisplayMode WindowDisplayMode { get; set; } = DisplayMode.Default;

        public TrexRunnerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _entityManager = new EntityManager();
            State = GameState.Initial;
            _fadeInTexturePosX = 44;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Window.Title = GameTitle;
            _graphics.PreferredBackBufferHeight = WindowHeight;
            _graphics.PreferredBackBufferWidth = WindowWidth;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _sfxButtonPressed =  Content.Load<SoundEffect>(AssetNameButtonPressedSound);
            _sfxHit = Content.Load<SoundEffect>(AssetNameHitSound);
            _sfxScoreReached = Content.Load<SoundEffect>(AssetNameScoreReachedSound);

            _spriteSheetTexture = Content.Load<Texture2D>(AssetNameSpritesheet);
            _invertedSpriteSheet = _spriteSheetTexture.InvertColors(Color.Transparent);

            _fadeInTexture = new Texture2D(GraphicsDevice, 1, 1);
            _fadeInTexture.SetData(new Color[] { Color.White });

            _trex = new Trex(_spriteSheetTexture, new Vector2(TrexStartPosX, TrexStartPosY - 52), _sfxButtonPressed);
            _trex.DrawOrder = 10;
            _trex.JumpComplete += _trex_JumpComplete;
            _trex.Died += _trex_Died;
            _inputController = new InputController(_trex);

            _scoreBoard = new ScoreBoard(_spriteSheetTexture, new Vector2(WindowWidth - 130, 10), _trex, _sfxScoreReached);
            _groundManager = new GroundManager(_spriteSheetTexture, _entityManager, _trex);
            _groundManager.Initialize();
            _obstacleManager = new ObstacleManager(_entityManager, _trex, _scoreBoard, _spriteSheetTexture);

            _skyManager = new SkyManager(_trex, _spriteSheetTexture, _invertedSpriteSheet, _entityManager, _scoreBoard);

            _gameOverScreen = new GameOverScreen(_spriteSheetTexture, this);
            _gameOverScreen.Position = new Vector2((WindowWidth - GameOverScreen.GameOverSpriteWidth) * 0.5f, WindowHeight * 0.5f - 30);

            _entityManager.AddEntity(_trex);
            _entityManager.AddEntity(_groundManager);
            _entityManager.AddEntity(_obstacleManager);
            _entityManager.AddEntity(_scoreBoard);
            _entityManager.AddEntity(_gameOverScreen);
            _entityManager.AddEntity(_skyManager);

            LoadSaveState();
        }

        private void _trex_Died(object sender, global::System.EventArgs e)
        {
            State = GameState.GameOver;
            _obstacleManager.IsEnabled = false;
            _gameOverScreen.IsEnabled = true;

            _sfxHit.Play();
            Debug.WriteLine("Game Over");

            if (_scoreBoard.DisplayScore > _scoreBoard.HighScore)
            {
                // Updates highscore and saves it in Save.dat
                Debug.WriteLine("New highscore set: " + _scoreBoard.DisplayScore);
                _scoreBoard.HighScore = _scoreBoard.DisplayScore;
                _highScoreDate = DateTime.Now;
                SaveGame();
            }
        }

        private void _trex_JumpComplete(object sender, global::System.EventArgs e)
        {
            if (State == GameState.Transition)
            {
                State = GameState.Playing;
                _trex.Initialize();

                _obstacleManager.IsEnabled = true;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);

            var keyboardState = Keyboard.GetState();
            
            if (State == GameState.Playing)
                _inputController.ProcessControls(gameTime);
            else if (State == GameState.Transition)
                _fadeInTexturePosX += (float)gameTime.ElapsedGameTime.TotalSeconds * 800f;
            else if (State == GameState.Initial)
            {
                if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
                {
                    StartGame();
                }
            }
            _entityManager.Update(gameTime);

            if (keyboardState.IsKeyDown(Keys.F8) && !_previousKeyboardState.IsKeyDown(Keys.F8))
                ResetSaveState();

            if (keyboardState.IsKeyDown(Keys.F12) && !_previousKeyboardState.IsKeyDown(Keys.F12))
                ToggleDisplayMode();

            _previousKeyboardState = keyboardState;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_skyManager == null)
                GraphicsDevice.Clear(Color.White);
            else
                GraphicsDevice.Clear(_skyManager.ClearColor);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _transformMatrix);

            _entityManager.Draw(_spriteBatch, gameTime);
            if (State == GameState.Initial || State == GameState.Transition)
            {
                _spriteBatch.Draw(_fadeInTexture, new Rectangle((int)_fadeInTexturePosX, 0, WindowWidth, WindowHeight), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool StartGame()
        {
            if (State != GameState.Initial) return false;

            State = GameState.Transition;
            _trex.BeginJump();
            return true;
        }

        public bool Replay()
        {
            if (State != GameState.GameOver)
                return false;

            State = GameState.Playing;
            _trex.Initialize();
            _obstacleManager.IsEnabled = true;
            _obstacleManager.Reset();
            _gameOverScreen.IsEnabled = false;
            _scoreBoard.Score = 0;
            _groundManager.Initialize();
            _inputController.BlockInputFrame();

            return true;
        }

        private void SaveGame()
        {
            SaveState saveState = new SaveState
            {
                HighScore = _scoreBoard.HighScore,
                HighScoreDate = _highScoreDate
            };

            try
            {
                using (FileStream fileStream = new FileStream(SavePath, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, saveState);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occured while saving the game: " + ex.Message);
            }
        }

        private void LoadSaveState()
        {
            try
            {
                using (FileStream fileStream = new FileStream(SavePath, FileMode.OpenOrCreate))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    SaveState saveState = binaryFormatter.Deserialize(fileStream) as SaveState;
                    if (saveState != null && _scoreBoard != null)
                    {
                        _scoreBoard.HighScore = saveState.HighScore;
                        _highScoreDate = saveState.HighScoreDate;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occured while loading the game: " + ex.Message);
            }
        }

        private void ResetSaveState()
        {
            _scoreBoard.HighScore = 0;
            _highScoreDate = default(DateTime);
            SaveGame();
        }

        private void ToggleDisplayMode()
        {
            if (WindowDisplayMode == DisplayMode.Default)
            {
                WindowDisplayMode = DisplayMode.Zoomed;
                _graphics.PreferredBackBufferHeight = WindowHeight * _ZoomFactor;
                _graphics.PreferredBackBufferWidth = WindowWidth * _ZoomFactor;
                _transformMatrix = Matrix.Identity * Matrix.CreateScale(_ZoomFactor);
            }
            else
            {
                WindowDisplayMode = DisplayMode.Default;
                _graphics.PreferredBackBufferHeight = WindowHeight;
                _graphics.PreferredBackBufferWidth = WindowWidth;
                _transformMatrix = Matrix.Identity;
            }
            _graphics.ApplyChanges();
        }

    }
}
