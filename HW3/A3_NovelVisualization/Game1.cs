using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace A3_NovelVisualization;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private List <(int, int)> _frequencies;

    private string words;
    
    private Texture2D _barTexture;
    
    private KeyboardState _keyboardState;
    private KeyboardState _previousKeyboardState;
    
    private bool _switch;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _barTexture =  new Texture2D(GraphicsDevice, 1,1);
        _frequencies = new List <(int, int)>();
        _previousKeyboardState = Keyboard.GetState();
        _switch = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _graphics.PreferredBackBufferWidth = 700;
        _graphics.PreferredBackBufferHeight = 600;
        _graphics.ApplyChanges();
        
        _barTexture.SetData(new[] { Color.White });
        
        LoadWordFrequencies();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        _keyboardState = Keyboard.GetState();

        if (_keyboardState.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter))
        {
            _switch = !_switch;
        }
        
        _previousKeyboardState = _keyboardState;

        base.Update(gameTime);
    }

    private void LoadWordFrequencies()
    {
        string path = "wordfrequency.txt";

        using (var stream = TitleContainer.OpenStream(path))
        using (var reader = new StreamReader(stream))
        {
            while ((words = reader.ReadLine()) != null)
            {
                var split = words.Split(':');
                _frequencies.Add((int.Parse(split[0]), int.Parse(split[1])));
            }
        }

    }

    private void DisplayWordFrequency()
    {
        int currentX = 5;
        int currentY = 20;
        int maxX = 600;
        int maxY = 700;
        int yMargin = maxY - 20;
        int maxCount = _frequencies[0].Item2; //Already sorted, so we know the max count is in the first row
        float scale = (float) maxX/maxCount;
        int barHeight = 15;
        int barGap = 5;
        int spacing = barHeight + barGap;

        for (int i = 0; i < _frequencies.Count; i++)
        {
            int frequency = _frequencies[i].Item1;
            int count = _frequencies[i].Item2;
            int barWidth = (int)(count * scale);
            
            float intensity = (float) count / maxCount;
            
            Color barColor = new Color(0.0f, 0.0f, intensity);
            
            Rectangle barRect = new Rectangle(currentX, currentY, barWidth, barHeight);
            
            _spriteBatch.Draw(_barTexture, barRect, barColor);
            
            currentY += spacing;

            if (currentY >= yMargin)
            {
                break;
            }
            
        }
        

    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.NavajoWhite);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
        _spriteBatch.Begin();
        if (_switch == true)
        {
            GraphicsDevice.Clear(Color.Red);
        }
        else
        {
            DisplayWordFrequency();
        }
        _spriteBatch.End();
    }
}