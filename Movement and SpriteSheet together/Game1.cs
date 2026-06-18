using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

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
        Texture2D battleBackgroundTexture;
        Texture2D mainMenuTexture;

        Texture2D battleHeroTexture;
        Texture2D beeTexture;
        Texture2D boarTexture;
        Texture2D snowmanTexture;
        Texture2D mushroomTexture;
        Texture2D piggyTexture;
        Texture2D skeletonTexture;
        Texture2D witchDoctorTexture;

        // map enemy name -> texture (case-insensitive)
        private Dictionary<string, Texture2D> _enemyTextures;

        Rectangle battleHeroRect;
        Rectangle backgroundRect;

        SpriteFont _font;
        SpriteFont _battleFont;
        SpriteFont _menuFont;
        SpriteFont _victoryFont;

        Hero _hero;

        Song menuSound;
        Song gameSound;
        Song attackSound;

        // track currently playing song so we don't force-restart the same song repeatedly
        private Song _currentlyPlayingSong;

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
        private const float CameraSmoothSpeed = 2f;

        private Encounter _currentEncounter = null;

        // Respawn delay used when reloading or when an encounter is defeated (seconds).
        private const float EncounterRespawnDelay = 5f;

        // Count how many global respawn cycles have happened; used to increase difficulty on full-respawn.
        private int _respawnCycle = 0;

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _currentState = GameState.MainMenu;

            battleHeroRect = new Rectangle(150,155,65,75);

            backgroundRect = new Rectangle(0, 0, WorldWidth, WorldHeight);

            Window.Title = "Turn-Based RPG Game";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            playerTexture = Content.Load<Texture2D>("blonde_man");
            rectangleTexure = Content.Load<Texture2D>("rectangle");
            particleTexure = Content.Load<Texture2D>("circle");
            backgroundTexture = Content.Load<Texture2D>("Overworld");
            battleHeroTexture = Content.Load<Texture2D>("Enemies/blonde");
            beeTexture = Content.Load<Texture2D>("Enemies/Bee_Idle");
            boarTexture = Content.Load<Texture2D>("Enemies/Boar_Idle");
            snowmanTexture = Content.Load<Texture2D>("Enemies/Christmas_Snowman_G_Idle");
            mushroomTexture = Content.Load<Texture2D>("Enemies/Mushroom_Reg");
            piggyTexture = Content.Load<Texture2D>("Enemies/Piggy_Idle");
            skeletonTexture = Content.Load<Texture2D>("Enemies/Skeleton_Idle");
            witchDoctorTexture = Content.Load<Texture2D>("Enemies/Witch_Doctor_Idle");
            battleBackgroundTexture = Content.Load<Texture2D>("Battle_Arena");
            mainMenuTexture = Content.Load<Texture2D>("Main_Menu");

            // Build a lookup so we can draw the correct texture for the current enemy.
            _enemyTextures = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase)
            {
                ["Snowman"] = snowmanTexture,
                ["Boar"] = boarTexture,
                ["Bee"] = beeTexture,
                ["Piggy"] = piggyTexture,
                ["Mushroom"] = mushroomTexture,
                ["Witch Doctor"] = witchDoctorTexture,
                ["Skeleton"] = skeletonTexture,   
            };

            _font = Content.Load<SpriteFont>("TitleFont");
            _battleFont = Content.Load<SpriteFont>("BattleFont");
            _menuFont = Content.Load<SpriteFont>("BattleMenu");
            _victoryFont = Content.Load<SpriteFont>("VictoryScreen");

            menuSound = Content.Load<Song>("Music/Menu_music");
            gameSound = Content.Load<Song>("Music/Game_music");

            PlaySong(menuSound, repeating: true, volume: 0.6f);

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
                            _currentEncounter = enc;

                            // Set game state and mark that a battle started
                            _currentState = GameState.Battle;
                            _battleStarted = true;
                            break;
                        }
                    }

                    UpdateCameraSmooth(gameTime);

                    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    foreach (var enc in _encounters)
                    {
                        bool timerFinished = enc.UpdateRespawn(dt);

                        // If timer finished (or it was already zero) and the encounter is currently inactive,
                        // only reactivate when the player is not standing on the encounter hitbox.
                        if (timerFinished || (!enc.IsAwaitingRespawn && !enc.Active))
                        {
                            if (playerRect.Intersects(enc.Hitbox))
                            {
                                // Player still on the spot — keep it inactive and add a small additional delay
                                // so they don't instantly trigger when they step off.
                                enc.StartRespawn(0.75f);
                            }
                            else
                            {
                                // Reactivate the encounter and restore the enemy's HP/stats
                                enc.ResetEnemy();
                                enc.Active = true;
                            }
                        }
                    }

                    if (_encounters == null || _encounters.Count == 0 || _encounters.All(e => !e.Active && !e.IsAwaitingRespawn))
                    {
                        // Reload scaled encounters
                        // Start them in an inactive/awaiting-respawn state so they don't immediately fight.
                        // increment respawn cycle to slightly increase difficulty each global respawn
                        _respawnCycle++;
                        var levelFactor = (_hero?.Level ?? 1) + _respawnCycle;
                        var newEncounters = _encounterManager.GetEncountersForLevel(GameState.Level1, levelFactor);
                        foreach (var e in newEncounters)
                            e.StartRespawn(EncounterRespawnDelay);

                        _encounters = newEncounters;
                    }
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

                        // Start respawn for the encounter that was just defeated (so it won't immediately retrigger)
                        _currentEncounter?.StartRespawn(EncounterRespawnDelay);
                        // Clear the current encounter reference so the defeated instance won't be reused immediately
                        _currentEncounter = null;

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
                _spriteBatch.Draw(mainMenuTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
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
                    var color = enc.Active ? Color.Red * 0.5f : Color.Gray * 0.4f;
                    _spriteBatch.Draw(rectangleTexure, new Rectangle(enc.Hitbox.X, enc.Hitbox.Y, enc.Hitbox.Width, enc.Hitbox.Height), color);
                }

                _spriteBatch.End();

                base.Draw(gameTime);
                return;
            }

            if (_currentState == GameState.Controls)
            {
                _spriteBatch.Begin();

                _spriteBatch.DrawString(_menuFont, "Controls: ", new Vector2(25,50), Color.White);
                _spriteBatch.DrawString(_menuFont, "Move: WASD", new Vector2(25, 75), Color.White);
                _spriteBatch.DrawString(_menuFont, "Combat: Up and Down arrows to navigate, Enter to confirm", new Vector2(25,100), Color.White);
                _spriteBatch.DrawString(_menuFont, "Left click to interact with menu while in battle", new Vector2(105,125), Color.White);
                _spriteBatch.DrawString(_menuFont, "Press R if you won or lost the battle and to exit this menu", new Vector2(105,150), Color.White);

                _spriteBatch.DrawString(_menuFont, "How to Play: ", new Vector2(25, 200), Color.White);
                _spriteBatch.DrawString(_menuFont, "Walk around and run into enemies to start battles", new Vector2(25,225), Color.White);
                _spriteBatch.DrawString(_menuFont, "Defeat enemies to gain xp and level up and get stronger", new Vector2(25,250), Color.White);
                _spriteBatch.DrawString(_menuFont, "Try to survive as long as you can", new Vector2(25,275), Color.White);
                _spriteBatch.DrawString(_menuFont, "Defeat as many enemies as you can", new Vector2(25,300), Color.White);

                _spriteBatch.End();
                
                base.Draw(gameTime);
                return;

            }
             
            if (_currentState == GameState.Battle)
            {
                _spriteBatch.Begin();

                var hero = _battleSystem.Hero;
                var enemy = _battleSystem.Enemy;

                _spriteBatch.Draw(battleBackgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                // Draw player hero on left
                _spriteBatch.Draw(battleHeroTexture, battleHeroRect, Color.White);

                // Draw enemy image based on enemy.Name
                if (enemy != null)
                {
                    var tex = GetEnemyTextureByName(enemy.Name);
                    if (tex != null)
                    {
                        // Fit enemy image into a compact rectangle on the right side of the screen.
                        const int maxSize = 130;
                        float scale = Math.Min(maxSize / (float)Math.Max(1, tex.Width), maxSize / (float)Math.Max(1, tex.Height));
                        int drawW = (int)(tex.Width * scale);
                        int drawH = (int)(tex.Height * scale);

                        // Position near the existing text area (adjust as needed)
                        var enemyRect = new Rectangle(400, 140, drawW, drawH);
                        _spriteBatch.Draw(tex, enemyRect, Color.White);
                    }
                    else
                    {
                        // fallback: draw placeholder rectangle texture
                        _spriteBatch.Draw(rectangleTexure, new Rectangle(450, 140, 96, 96), Color.White);
                    }
                }

                _spriteBatch.DrawString(_battleFont, $"HP: {hero.HP}", new Vector2(158, 225), Color.White);

                _spriteBatch.DrawString(_battleFont, $"HP: {enemy.HP}", new Vector2(429, 244), Color.White);
                
                if (_battleSystem.State == BattleState.PlayerTurn || _battleSystem.State == BattleState.EnemyTurn)
                    _spriteBatch.DrawString(_battleFont, $"Turn: {_battleSystem.State}", new Vector2(50, 50), Color.Yellow);
                
                _spriteBatch.DrawString(_battleFont, $"Action: {_battleSystem.LastAction}", new Vector2(50, 70), Color.White);

                if (_battleSystem.State == BattleState.PlayerTurn)
                    _battleMenu.Draw(_spriteBatch, Color.White, Color.Yellow);

                if(_battleSystem.State == BattleState.Win)
                    _spriteBatch.DrawString(_victoryFont, "You Win!", new Vector2(280, 200), Color.Green);

                if (_battleSystem.State == BattleState.Lose)
                    _spriteBatch.DrawString(_victoryFont, "You Lose!", new Vector2(280, 200), Color.Red);

                if (_battleSystem.State == BattleState.Win || _battleSystem.State == BattleState.Lose)
                    _spriteBatch.DrawString(_battleFont, "Press R to Exit battle", new Vector2(280,245), Color.White);

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

        /// <summary>
        /// Return the texture that matches the enemy name (case-insensitive).
        /// Returns null when no mapping exists.
        /// </summary>
        private Texture2D GetEnemyTextureByName(string name)
        {
            if (string.IsNullOrEmpty(name) || _enemyTextures == null)
                return null;

            _enemyTextures.TryGetValue(name, out var tex);
            return tex;
        }

        private void LoadLevel(GameState level)
        {
            _movement = new MovementManager(_levelManager.GetSpawnPointForLevel(level), _particleSystem);
            _encounters = _encounterManager.GetEncountersForLevel(level);

            PlaySong(gameSound, repeating: true, volume: 0.6f);
        }
        
        private void ResetGame()
        {
            // Recreate hero and battle system so next playthrough starts fresh.
            _hero = new Hero("Hero", 30, 4, battleHeroTexture, battleHeroRect);
            _battleSystem = new BattleSystem();
            _encounterManager = new EncounterManager();
        }

        private void ScaleEncountersToPlayerLevel(List<Encounter> encounters, int playerLevel)
        {
            if (encounters == null || encounters.Count == 0)
                return;

            // Example: 20% stronger per level above 1
            float multiplier = 1f + (playerLevel - 1) * 0.2f;

            for (int i = 0; i < encounters.Count; i++)
            {
                var enc = encounters[i];
                var baseEnemy = enc.Enemy;

                // read base stats and scale them
                int scaledHP = Math.Max(1, (int)System.MathF.Ceiling(baseEnemy.HP * multiplier));
                int scaledAttack = Math.Max(1, (int)System.MathF.Ceiling(baseEnemy.AttackPower * multiplier));
                int scaledXP = Math.Max(1, (int)System.MathF.Ceiling(baseEnemy.XPValue * multiplier));

                enc.Enemy = new Enemy(baseEnemy.Name, scaledHP, scaledAttack, scaledXP);
            }
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

        private void PlaySong(Song song, bool repeating = true, float volume = 1f)
        {
            if (song == null)
                return;

            // avoid restarting the same song repeatedly
            if (_currentlyPlayingSong == song && MediaPlayer.State == MediaState.Playing)
                return;

            MediaPlayer.IsRepeating = repeating;
            MediaPlayer.Volume = MathHelper.Clamp(volume, 0f, 1f);
            MediaPlayer.Play(song);
            _currentlyPlayingSong = song;
        }

        private void StopSong()
        {
            MediaPlayer.Stop();
            _currentlyPlayingSong = null;
        }
    }
}