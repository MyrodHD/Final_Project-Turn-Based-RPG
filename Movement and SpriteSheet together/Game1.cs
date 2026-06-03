using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Movement_and_SpriteSheet_together
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public enum GameState
        {
            MainMenu,
            Level1,
            Controls,
            Battle
        }

        GameState _currentState = GameState.MainMenu;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        MenuManager _menuManager;
        SpriteManager _playerSprite;
        MovementManager _movement;
        ParticleSystem _particleSystem;
        BattleMenu _battleMenu;

        Texture2D playerTexture;
        Texture2D rectangleTexure;
        Texture2D particleTexure;
        Texture2D heroTexture;
        Texture2D enemyTexture;
        Texture2D backgroundTexture;

        Rectangle backgroundRect;

        Texture2D battleHeroTexture;
        Rectangle battleHeroRect;

        SpriteFont _font;
        SpriteFont _battleFont;
        SpriteFont _menuFont;

        Hero _hero;

        BattleSystem _battleSystem;
        private bool _battleStarted = false;

        private List<Encounter> _encounters = new List<Encounter>();
        private int _playerFrameWidth;
        private int _playerFrameHeight;

        private EncounterManager _encounterManager;
        private LevelManager _levelManager;

        private Vector2 _cameraPosition = Vector2.Zero;
        private const int WorldWidth = 1200;
        private const int WorldHeight = 1200;
        private const float CameraSmoothSpeed = 3f;

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _currentState = GameState.MainMenu;

            battleHeroRect = new Rectangle(120,155,65,75);

            backgroundRect = new Rectangle(0, 0, WorldWidth, WorldHeight);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            playerTexture = Content.Load<Texture2D>("blonde_man");
            rectangleTexure = Content.Load<Texture2D>("rectangle");
            particleTexure = Content.Load<Texture2D>("circle");
            battleHeroTexture = Content.Load<Texture2D>("blonde");
            backgroundTexture = Content.Load<Texture2D>("Overworld");

            _font = Content.Load<SpriteFont>("TitleFont");
            _battleFont = Content.Load<SpriteFont>("BattleFont");
            _menuFont = Content.Load<SpriteFont>("BattleMenu");

            List<string> menuItems = new List<string> { "Start Game", "Controls" };
            
            _menuManager = new MenuManager(_font, menuItems);

            _playerSprite = new SpriteManager(playerTexture, 4, 4);
            
            _particleSystem = new ParticleSystem(particleTexure);

            _movement = new MovementManager(new Vector2(100,100), _particleSystem);

            _battleSystem = new BattleSystem();

            _battleMenu = new BattleMenu(_menuFont, new List<string> { "Attack", "Heal" }, new Vector2(640, 290));

            _playerFrameWidth = playerTexture.Width / 4;
            _playerFrameHeight = playerTexture.Height / 4;

            //Below is where the hero and enemies are created for player to fight.

            _hero = new Hero("Hero", 30, 4, battleHeroTexture, battleHeroRect);

            _encounterManager = new EncounterManager();
            _levelManager = new LevelManager();

            _encounters = _encounterManager.GetEncountersForLevel(GameState.Level1);

            //Camera
            _cameraPosition = GetClampedCameraTarget(_movement.position);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            switch (_currentState)
            {
                case GameState.MainMenu:
                    _menuManager.Update(gameTime, ref _currentState);

                    if (_currentState == GameState.Level1)
                    {
                        LoadLevel(_currentState);
                    }
                    break;

                case GameState.Level1:
                    _movement.Update(gameTime);
                    _particleSystem.Update(gameTime);

                    if (_movement.currentDirection != Vector2.Zero)
                    {
                        _playerSprite.Update(gameTime);

                        if (_movement.currentDirection.Y > 0)
                            _playerSprite.currentRow = 0;
                        else if (_movement.currentDirection.X < 0)
                            _playerSprite.currentRow = 1;
                        else if (_movement.currentDirection.X > 0)
                            _playerSprite.currentRow = 2;
                        else if (_movement.currentDirection.Y < 0)
                            _playerSprite.currentRow = 3;
                    }
                    else
                        _playerSprite.Reset();

                    var playerRect = new Rectangle((int)_movement.position.X, (int)_movement.position.Y, _playerFrameWidth, _playerFrameHeight);
                    foreach (var enc in _encounters)
                    {
                        if (enc.Active && enc.Hitbox.Intersects(playerRect))
                        {
                            _battleSystem.BattleStart(_hero, enc.Enemy);
                            enc.Active = false;
                            _currentState = GameState.Battle;
                            _battleStarted = true;
                            break;
                        }
                    }

                    UpdateCameraSmooth(gameTime);

                    break;

                case GameState.Controls:
                    if (Keyboard.GetState().IsKeyDown(Keys.R))
                        _currentState = GameState.MainMenu;
                    break;
                
                case GameState.Battle:
                    _battleSystem.Update(gameTime);

                    _battleMenu.Update(gameTime);

                    // Consume menu selection and map to actions
                    if (_battleMenu.ConsumeSelection(out var selected))
                    {
                        switch (selected)
                        {
                            case 0: // Attack
                                _battleSystem.HeroAttack();
                                break;
                            case 1: // Heal
                                _battleSystem.HeroHeal();
                                break;

                        }
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        _battleSystem.HeroAttack();
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.H))
                    {
                        _battleSystem.HeroHeal();
                    }

                    // Award XP immediately when a win occurs (only once per battle)
                    if (_battleSystem.State == BattleState.Win && _battleStarted)
                    {
                        int xp = _battleSystem.Enemy?.XPValue ?? 0;
                        if (xp > 0)
                            _hero.AddXP(xp);

                        // prevent awarding more than once
                        _battleStarted = false;
                    }

                    if ((_battleSystem.State == BattleState.Win || _battleSystem.State == BattleState.Lose))
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.R))
                        {
                            if (_battleSystem.State == BattleState.Lose)
                            {
                                // Player lost: return to main menu and reset the playable state so game must be started again.
                                ResetGame();
                                _currentState = GameState.MainMenu;
                            }
                            else
                            {
                                // Player won: return to level1 and reload its data
                                _currentState = GameState.Level1;
                            }
                        }

                        _battleStarted = false;
                    }
                        
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_currentState == GameState.MainMenu)
            {
                _spriteBatch.Begin();
                _menuManager.Draw(_spriteBatch);
                _spriteBatch.End();

                base.Draw(gameTime);
                return;
            }

            if (_currentState == GameState.Level1)
            {
                var transform = Matrix.CreateTranslation(new Vector3(-_cameraPosition, 0f));
                _spriteBatch.Begin(transformMatrix: transform);

                _spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);

                _playerSprite.Draw(_spriteBatch, _movement.position);
                _particleSystem.Draw(_spriteBatch);
                foreach (var enc in _encounters)
                {
                    var color = enc.Active ? Color.Red * 0.6f : Color.Gray * 0.4f;
                    _spriteBatch.Draw(rectangleTexure, new Rectangle(enc.Hitbox.X, enc.Hitbox.Y, enc.Hitbox.Width, enc.Hitbox.Height), color);
                }

                _spriteBatch.End();

                base.Draw(gameTime);
                return;
            }

            if (_currentState == GameState.Controls)
            {

            }

            if (_currentState == GameState.Battle)
            {
                _spriteBatch.Begin();

                var hero = _battleSystem.Hero;
                var enemy = _battleSystem.Enemy;

                _spriteBatch.Draw(battleHeroTexture, battleHeroRect, Color.White);
                _spriteBatch.DrawString(_battleFont, $"HP: {hero.HP}", new Vector2(158, 240), Color.White);

                _spriteBatch.DrawString(_battleFont, $"HP: {enemy.HP}", new Vector2(450, 190), Color.White);
                
                if (_battleSystem.State == BattleState.PlayerTurn || _battleSystem.State == BattleState.EnemyTurn)
                    _spriteBatch.DrawString(_battleFont, $"Turn: {_battleSystem.State}", new Vector2(50, 50), Color.Yellow);
                
                _spriteBatch.DrawString(_battleFont, $"Action: {_battleSystem.LastAction}", new Vector2(50, 70), Color.White);


                if (_battleSystem.State == BattleState.PlayerTurn)
                    _battleMenu.Draw(_spriteBatch, Color.White, Color.Yellow);

                if (_battleSystem.State == BattleState.Win || _battleSystem.State == BattleState.Lose)
                    _spriteBatch.DrawString(_battleFont, "Press R to Exit battle", new Vector2(50,305), Color.White);

                // Draw HUD with Level / XP so you can verify XP changes while playing.
                if (_hero != null && _currentState != GameState.MainMenu)
                {
                    string xpText = $"Level: {_hero.Level}   XP: {_hero.CurrentXP}/{_hero.XPToNextLevel}";
                    _spriteBatch.DrawString(_battleFont, xpText, new Vector2(10, 10), Color.White);
                }

                _spriteBatch.End();

                base.Draw(gameTime);
                return;
            }

            base.Draw(gameTime);
        }

        private void LoadLevel(GameState level)
        {
            _movement = new MovementManager(_levelManager.GetSpawnPointForLevel(level), _particleSystem);
            _encounters = _encounterManager.GetEncountersForLevel(level);
        }
        
        private void ResetGame()
        {
            // Recreate hero and battle system so next playthrough starts fresh.
            _hero = new Hero("Hero", 30, 4, battleHeroTexture, battleHeroRect);
            _battleSystem = new BattleSystem();
            _encounterManager = new EncounterManager();
            // Movement and encounters will be reinitialized when the player selects "Start Game" from main menu.
        }

        private Vector2 GetClampedCameraTarget(Vector2 worldPosition)
        {
            var viewport = GraphicsDevice.Viewport;
            float vw = viewport.Width;
            float vh = viewport.Height;

            // center target on the player's middle
            var target = worldPosition + new Vector2(_playerFrameWidth / 2f, _playerFrameHeight / 2f) - new Vector2(vw / 2f, vh / 2f);

            float maxX = System.MathF.Max(0, WorldWidth - vw);
            float maxY = System.MathF.Max(0, WorldHeight - vh);

            target.X = MathHelper.Clamp(target.X, 0, maxX);
            target.Y = MathHelper.Clamp(target.Y, 0, maxY);

            return target;
        }

        private void UpdateCameraSmooth(GameTime gameTime)
        {
            if (_movement == null)
                return;

            Vector2 desired = GetClampedCameraTarget(_movement.position);

            // Lerp based smoothing; factor is frame-time scaled.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float t = 1f - System.MathF.Exp(-CameraSmoothSpeed * dt); // better feeling smoothing independent of frame-rate

            _cameraPosition = Vector2.Lerp(_cameraPosition, desired, t);

            // final clamp for safety
            float maxX = System.MathF.Max(0, WorldWidth - GraphicsDevice.Viewport.Width);
            float maxY = System.MathF.Max(0, WorldHeight - GraphicsDevice.Viewport.Height);
            _cameraPosition.X = MathHelper.Clamp(_cameraPosition.X, 0, maxX);
            _cameraPosition.Y = MathHelper.Clamp(_cameraPosition.Y, 0, maxY);
        }

    }
}