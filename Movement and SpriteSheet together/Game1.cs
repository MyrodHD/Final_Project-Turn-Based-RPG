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
            Level2,
            Level3,
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

        Texture2D battleHeroTexture;
        Rectangle battleHeroRect;

        SpriteFont _font;
        SpriteFont _battleFont;

        Hero _hero;

        BattleSystem _battleSystem;
        private bool _battleStarted = false;

        private List<Encounter> _encounters = new List<Encounter>();
        private int _playerFrameWidth;
        private int _playerFrameHeight;

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _currentState = GameState.MainMenu;

            battleHeroRect = new Rectangle(150,155,65,75);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            playerTexture = Content.Load<Texture2D>("player_hat_spritesheet");
            rectangleTexure = Content.Load<Texture2D>("rectangle");
            particleTexure = Content.Load<Texture2D>("circle");
            battleHeroTexture = Content.Load<Texture2D>("rectangle");

            _font = Content.Load<SpriteFont>("TitleFont");
            _battleFont = Content.Load<SpriteFont>("BattleFont");

            List<string> menuItems = new List<string> { "Start Game", "Controls" };
            
            _menuManager = new MenuManager(_font, menuItems);

            _playerSprite = new SpriteManager(playerTexture, 4, 4);
            
            _particleSystem = new ParticleSystem(particleTexure);

            _movement = new MovementManager(new Vector2(100,100), _particleSystem);

            _battleSystem = new BattleSystem();

            _battleMenu = new BattleMenu(_battleFont, new List<string> { "Attack", "Heal" }, new Vector2(50, 260));

            _playerFrameWidth = playerTexture.Width / 4;
            _playerFrameHeight = playerTexture.Height / 4;

            //Below is where the hero and enemies are created for player to fight.

            _hero = new Hero("Hero", 30, 4, battleHeroTexture, battleHeroRect);

            _encounters.Add(new Encounter(new Rectangle(220, 180, 48, 48), new Enemy("Goblin", 16, 3)));
            _encounters.Add(new Encounter(new Rectangle(420, 260, 56, 56), new Enemy("Wolf", 18, 3)));
            _encounters.Add(new Encounter(new Rectangle(80, 360, 48, 48), new Enemy("Bandit", 20, 4)));
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

                    break;

                case GameState.Level2:
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

                    if ((_battleSystem.State == BattleState.Win || _battleSystem.State == BattleState.Lose))
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.R))
                            _currentState = GameState.Level1;

                        _battleStarted = false;
                    }
                        
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            if (_currentState == GameState.MainMenu)
            {
                _menuManager.Draw(_spriteBatch);
            }

            if (_currentState == GameState.Level1)
            {

                _playerSprite.Draw(_spriteBatch, _movement.position);
                _particleSystem.Draw(_spriteBatch);

                foreach (var enc in _encounters)
                {
                    var color = enc.Active ? Color.Red * 0.6f : Color.Gray * 0.4f;
                    _spriteBatch.Draw(rectangleTexure, new Rectangle(enc.Hitbox.X, enc.Hitbox.Y, enc.Hitbox.Width, enc.Hitbox.Height), color);
                }

            }

            if (_currentState == GameState.Battle)
            {
                var hero = _battleSystem.Hero;
                var enemy = _battleSystem.Enemy;

                _spriteBatch.Draw(battleHeroTexture, battleHeroRect, Color.Blue);
                _spriteBatch.DrawString(_battleFont, $"HP: {hero.HP}", new Vector2(158, 240), Color.White);
                _spriteBatch.DrawString(_battleFont, $"Enemy: {enemy.Name}  HP: {enemy.HP}", new Vector2(450, 190), Color.White);
                _spriteBatch.DrawString(_battleFont, $"State: {_battleSystem.State}", new Vector2(50, 50), Color.Yellow);
                _spriteBatch.DrawString(_battleFont, $"Action: {_battleSystem.LastAction}", new Vector2(50, 70), Color.White);


                if (_battleSystem.State == BattleState.PlayerTurn)
                    _battleMenu.Draw(_spriteBatch, Color.White, Color.Yellow);

                if (_battleSystem.State == BattleState.Win || _battleSystem.State == BattleState.Lose)
                    _spriteBatch.DrawString(_battleFont, "Press R to Exit battle", new Vector2(50,305), Color.White);
            }
        

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private class Encounter
        {
            public Rectangle Hitbox;
            public Enemy Enemy;
            public bool Active = true;

            public Encounter(Rectangle hitbox, Enemy enemy)
            {
                Hitbox = hitbox;
                Enemy = enemy;
            }
        }

    }
}
