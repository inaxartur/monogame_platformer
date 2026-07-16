using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// dotnet run --project MonoGame_Platformer

namespace MonoGame_Platformer;

public class Game1 : Game
{
    private const int TILESIZE = 64;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Sprite _sprite;

    private Dictionary<Vector2, int> _tileMap;
    private List<Rectangle> _textureStore;
    private Texture2D _texture;
    private List<Sprite> _sprites;
    private List<Rectangle> intersections;

    public static Game1 Instance { get; private set; }

    List<Sprite> Sprites => _sprites;

    public Game1()
    {
        Instance = this;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _tileMap = LoadMap("../../../Data/tilemap.csv");
        _textureStore = new() //list of rectangles - loading the source rectangles for the tileset
        {
            new Rectangle(0, 0, 16, 16), // grass 0
            new Rectangle(16, 0, 16, 16), // stone 1
            new Rectangle(0, 16, 16, 16), // bricks 2
            new Rectangle(16, 16, 16, 16), // stone bricks 3
        };
        intersections = new();
    }

    private Dictionary<Vector2, int> LoadMap(string filepath) // loading the tilemap from a csv file
    {
        Dictionary<Vector2, int> result = new(); // create a new dictionary to hold the tilemap data

        StreamReader reader = new(filepath); // open the csv file for reading

        int y = 0;
        string line;
        while((line = reader.ReadLine()) != null)
        {
            string[] item = line.Split(',');
            for (int x = 0; x < item.Length; x++)
            {
                if(int.TryParse(item[x], out int tileId))
                {
                    if (tileId > 0) // only add tiles which are not air
                    {
                        result[new Vector2(x, y)] = tileId;
                    }
                }
            }
            y++;
        }

        return result;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _texture = Content.Load<Texture2D>("tileset");
        _sprites = new();

        Texture2D playerTexture = Content.Load<Texture2D>("player_frame_0");
        Texture2D enemyTexture = Content.Load<Texture2D>("enemy_frame_0");

        _sprites.Add(new Player(playerTexture, new Rectangle(0, 0, TILESIZE, TILESIZE), 6f));
        _sprites.Add(new Enemy(enemyTexture, 100f, new Rectangle(600, 5*TILESIZE, TILESIZE, TILESIZE), 120));
        _sprites.Add(new Enemy(enemyTexture, 100f, new Rectangle(330, 5*TILESIZE, TILESIZE, TILESIZE), 50));
       
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        foreach (var sprite in _sprites)
        {
            sprite.Update(gameTime);
        }

        _sprites[0].rect.X += (int)_sprites[0].velocity.X;
        intersections = getIntersectingTilesHorizontal(_sprites[0].rect);

        foreach (var tile in intersections)
        {
            if (_tileMap.TryGetValue(new Vector2(tile.X, tile.Y), out int tileId))
            {
                // Handle collision with the tile
                Rectangle collision = new Rectangle(tile.X * TILESIZE, tile.Y * TILESIZE, TILESIZE, TILESIZE);
                if (_sprites[0].velocity.X > 0.0f) // moving right
                {
                    _sprites[0].rect.X = collision.Left - _sprites[0].rect.Width;
                }
                else if (_sprites[0].velocity.X < 0.0f) // moving left
                {
                    _sprites[0].rect.X = collision.Right;
                }
            }
        }

        _sprites[0].rect.Y += (int)_sprites[0].velocity.Y;
        intersections = getIntersectingTilesVertical(_sprites[0].rect);

        foreach (var tile in intersections)
        {
            if (_tileMap.TryGetValue(new Vector2(tile.X, tile.Y), out int tileId))
            {
                // Handle collision with the tile
                Rectangle collision = new Rectangle(tile.X * TILESIZE, tile.Y * TILESIZE, TILESIZE, TILESIZE);
                if (_sprites[0].velocity.Y > 0.0f) // moving down
                {
                    _sprites[0].rect.Y = collision.Top - _sprites[0].rect.Height;
                    _sprites[0].SetGroundedTrue(); // set the player to grounded if they are on the ground
                } else if (_sprites[0].velocity.Y < 0.0f) // moving up
                {
                    _sprites[0].rect.Y = collision.Bottom;
                }
            }
        }
        SpriteCollisionManager.CheckCollisionsForEntities(_sprites); //check collision between sprites

        base.Update(gameTime);
    }

    public List<Rectangle> getIntersectingTilesHorizontal(Rectangle target)
    {
        List<Rectangle> intersectingTiles = new();

        for (int x = 0; x<= 1; x++)
        {
            for (int y = 0; y <= 1; y++) 
            {
                intersectingTiles.Add(new Rectangle(
                    (target.X + x*TILESIZE) / TILESIZE,
                    (target.Y + y*(TILESIZE-1)) / TILESIZE,
                    TILESIZE,
                    TILESIZE
                ));

            }
        }

        return intersectingTiles;
    }
    public List<Rectangle> getIntersectingTilesVertical(Rectangle target)
    {
        List<Rectangle> intersectingTiles = new();

        for (int x = 0; x<= 1; x++)
        {
            for (int y = 0; y <= 1; y++)
            {
                intersectingTiles.Add(new Rectangle(
                    (target.X + x*(TILESIZE-1)) / TILESIZE,
                    (target.Y + y*TILESIZE) / TILESIZE,
                    TILESIZE,
                    TILESIZE
                ));

            }
        }
        return intersectingTiles;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp); // clamping for pixel art

        foreach (var tile in _tileMap) // draw the tiles
        {
            Rectangle dest = new((int)tile.Key.X * TILESIZE, (int)tile.Key.Y * TILESIZE, TILESIZE, TILESIZE);

            Rectangle source = _textureStore[tile.Value - 1]; 

            _spriteBatch.Draw(_texture, dest, source, Color.White);
        }

        foreach (var sprite in _sprites) // draw the sprites
        {
            _spriteBatch.Draw(sprite.texture, sprite.rect, Color.White);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
