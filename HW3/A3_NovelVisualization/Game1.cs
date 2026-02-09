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

    private Random _rand;
    
    private List <(int, int)> _frequencies;
    
    private SpriteFont _font;

    private string words;
    private String[] _uniqueWords;
    
    private Texture2D _barTexture;

    private Color[] _wordColors;
    
    private KeyboardState _keyboardState;
    private KeyboardState _previousKeyboardState;
    private MouseState _mouseState;
    private MouseState _previousMouseState;
    
    private bool _switch;
    private bool _pressed;
    
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
        
        _font =  Content.Load<SpriteFont>("Fonts/Font");
        
        _barTexture.SetData(new[] { Color.White });
        _wordColors = new Color[]{Color.Red, Color.Blue, Color.Green};

        _rand = new Random();

        LoadWordFrequencies();
        LoadUniqueWords();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        _keyboardState = Keyboard.GetState();
        _mouseState = Mouse.GetState();

        if (_keyboardState.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter))
        {
            _switch = !_switch;
        }

        if (_mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            _pressed = true;
        }
        
        _previousKeyboardState = _keyboardState;
        _previousMouseState = _mouseState;

        base.Update(gameTime);
    }

    private void LoadWordFrequencies()
    {
        string path = "Content/Text/wordfrequency.txt";

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

    public void LoadUniqueWords()
    {
        String path = "Content/Text/uniquewords.txt";
        String text = "";
        
        using (var stream = TitleContainer.OpenStream(path))
        using (var reader = new StreamReader(stream))
        {
            text = File.ReadAllText(path);
            _uniqueWords = text.Split("\n");
        }
    }
    
    private void DisplayUniqueWords()
    {
        bool outOfBoundsX = false;
        bool outOfBoundsY = false;
        int y_val = 0;
        int y_inc = 30;

        while (!outOfBoundsY)
        {
            String displayString = "";
            outOfBoundsX = false;
            while (!outOfBoundsX)
            {
                String newWord = _uniqueWords[_rand.Next(0,_uniqueWords.Length)];
                if (_font.MeasureString(displayString + newWord).X > _graphics.PreferredBackBufferWidth)
                {
                    outOfBoundsX = true;
                }
                else
                {
                    _spriteBatch.DrawString(_font, newWord + " ", new Vector2(_font.MeasureString(displayString).X, y_val), _wordColors[_rand.Next(_wordColors.Length)]);
                }
                displayString = displayString + newWord + " ";
            }

            if (y_val + y_inc > _graphics.PreferredBackBufferHeight)
            {
                outOfBoundsY = true;
            }
            else
            {
                y_val += y_inc;
            }
        }

        _pressed = false;
    }

    protected override void Draw(GameTime gameTime)
    {
        // TODO: Add your drawing code here

        base.Draw(gameTime);
        _spriteBatch.Begin();
        if (_switch == true)
        {
            GraphicsDevice.Clear(Color.NavajoWhite);
            DisplayWordFrequency();
        }
        if(_switch == false && _pressed == true)
        {
            GraphicsDevice.Clear(Color.NavajoWhite);
            DisplayUniqueWords();
        }
        _spriteBatch.End();
    }
}