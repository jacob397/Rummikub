using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rummikub.Entities;

namespace Rummikub;

public class Game1 : Game
{
    private const int Home = 0;
    private const int Game = 1;
    private const int Instructions = 2;

    private const int Player = 0;
    private const int Opponent = 1;
    private const int Waiting = 2;

    private const int PlayButtonX = 370;
    private const int PlayButtonY = 396;
    private const int PlayButtonWidth = 460;
    private const int PlayButtonHeight = 160;
    
    private const int InstructionButtonX = 600;
    private const int InstructionButtonY = 655;
    private const int InstructionButtonWidth = 160;
    private const int InstructionButtonHeight = 50;
    
    private const int GoBackButtonX = 520;
    private const int GoBackButtonY = 690;
    private const int GoBackButtonWidth = 160;
    private const int GoBackButtonHeight = 50;
    
    private const int QuitButtonX = 770;
    private const int QuitButtonY = 655;
    private const int QuitButtonWidth = 120;
    private const int QuitButtonHeight = 50;
    
    private const int ExitButtonX = 1145;
    private const int ExitButtonY = 10;
    private const int ExitButtonWidth = 45;
    private const int ExitButtonHeight = 45;
    
    private const int UndoButtonX = 470;
    private const int UndoButtonY = 655;
    private const int UndoButtonWidth = 120;
    private const int UndoButtonHeight = 50;
    
    private const int EndTurnButtonX = 310;
    private const int EndTurnButtonY = 655;
    private const int EndTurnButtonWidth = 150;
    private const int EndTurnButtonHeight = 50;
    
    private const int DisabledEndTurnButtonX = 310;
    private const int DisabledEndTurnButtonY = 655;
    private const int DisabledEndTurnButtonWidth = 150;
    private const int DisabledEndTurnButtonHeight = 50;

    private const int OpponentEndTurnMessageX = 915;
    private const int OpponentEndTurnMessageY = 570;
    private const int OpponentEndTurnMessageWidth = 225;
    private const int OpponentEndTurnMessageHeight = 135;

    private const int HandX = 0;
    private const int HandY = 720;
    private const int HandWidth = 1200;
    private const int HandHeight = 135;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private MouseState _previousMouseState;

    private Texture2D _homeImg;
    private Texture2D _gameImg;
    private Texture2D _instructionsImg;
    private Texture2D _tilesImg;
    private Texture2D _disabledButtonImg;
    private Texture2D _opponentEndTurnImg;
    
    private Rectangle _homeRect;
    private Rectangle _gameRect;
    private Rectangle _instructionsRect;
    private Rectangle _handRect;
    private Rectangle _disabledButtonRect;
    private Rectangle _opponentEndTurnRect;
    
    private int _gamestate;
    private int _playerTurn;

    private Deck _deck;
    private Board _board;
    private Hand _playerHand;
    private Hand _opponentHand;

    private Board _oldBoard;
    private Hand _oldHand;
    
    private Tile _selectedTile;
    private bool _handTileSelected;
    private bool _boardTileSelected;
    private int[] _boardTileCoordinates;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1200;
        _graphics.PreferredBackBufferHeight = 800;
        _graphics.ApplyChanges();
        
        _gamestate = Home;
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _homeImg = Content.Load<Texture2D>("Images/Backgrounds/home");
        _gameImg = Content.Load<Texture2D>("Images/Backgrounds/game");
        _tilesImg = Content.Load<Texture2D>("Images/Foregrounds/tiles");
        _instructionsImg = Content.Load<Texture2D>("Images/Backgrounds/instructions");
        _disabledButtonImg = Content.Load<Texture2D>("Images/Foregrounds/disabled_button");
        _opponentEndTurnImg = Content.Load<Texture2D>("Images/Foregrounds/opponent_turn_msg");
        
        _homeRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _gameRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _instructionsRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _handRect = new Rectangle(HandX, HandY, HandWidth, HandHeight);
        _disabledButtonRect = new Rectangle(DisabledEndTurnButtonX, DisabledEndTurnButtonY, 
            DisabledEndTurnButtonWidth, DisabledEndTurnButtonHeight);
        _opponentEndTurnRect = new Rectangle(OpponentEndTurnMessageX, OpponentEndTurnMessageY, 
            OpponentEndTurnMessageWidth, OpponentEndTurnMessageHeight);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        switch (_gamestate)
        {
            case Home:
                UpdateHome(gameTime);
                break;
            case Game:
                UpdateGame(gameTime);
                break;
            case Instructions:
                UpdateInstructions(gameTime);
                break;
        }

        _previousMouseState = Mouse.GetState();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        switch (_gamestate)
        {
            case Home:
                DrawHome(gameTime);
                break;
            case Game:
                DrawGame(gameTime);
                break;
            case Instructions:
                DrawInstructions();
                break;
        }
        
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void UpdateHome(GameTime gameTime)
    {
        if (PlayButtonClicked(gameTime))
        {
            _gamestate = Game;
            SetupGame();
        }
        else if (ExitButtonClicked())
        {
            Exit();
        }
    }

    private void UpdateGame(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();

        if (_playerTurn == Player)
        {
            AttemptToMoveTile();
            
            if (_selectedTile == null && InstructionButtonClicked())
            {
                _gamestate = Instructions;
            }
            else if (_selectedTile == null && QuitButtonClicked())
            {
                _gamestate = Home;
            }
            else if (_selectedTile == null && UndoButtonClicked())
            {
                Undo();
            }
            else if (_selectedTile == null && EndTurnButtonClicked())
            {
                if (_board.IsValid() && IsHandValid())
                {
                    EndTurn();
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _playerHand.MoveLeft();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _playerHand.MoveRight();
            }
        }
        else if (_playerTurn == Opponent)
        {
            Move bestMove = GetBestMove(_board, _opponentHand, GetPotentialGroups(_board, _opponentHand));

            if (bestMove.Score() <= 0)
            {
                _opponentHand.Add(_deck.Pop());
            }
            else
            {
                bestMove.Do();
            }
            
            _playerTurn = Waiting;
        }
        else if (_playerTurn == Waiting)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && 
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                _playerTurn = Player;
            }
        }
        
        _previousMouseState = mouseState;
    }

    private void AttemptToMoveTile()
    {
        bool tileRecentlySelected = false;
        MouseState mouseState = Mouse.GetState();
        
        if (_boardTileSelected)
        {
            AttemptPlaceBoardTile(mouseState);
        }
        else if (_handTileSelected)
        {
            AttemptPlaceHandTile(mouseState);
        }
        else if (AttemptSelectTile())
        {
            tileRecentlySelected = true;
        }

        if (_selectedTile != null && RectangleClicked(mouseState, _handRect) && !tileRecentlySelected)
        {
            _playerHand.Add(_selectedTile);
            _handTileSelected = false;
            _boardTileSelected = false;
            _selectedTile = null;
        }
    }

    private void EndTurn()
    {
        if (_playerHand.Count() == _oldHand.Count())
        {
            _playerHand.Add(_deck.Pop());
        }

        _oldHand = new Hand(_playerHand);
        _playerTurn = Opponent;
    }

    private bool IsHandValid()
    {
        List<Tile> newHandTiles = _playerHand.GetTiles();
        List<Tile> oldHandTiles = _oldHand.GetTiles().ToList();

        for (int i = 0; i < newHandTiles.Count; i++)
        {
            if (!RemoveTile(oldHandTiles, newHandTiles[i]))
            {
                return false;
            }
        }
        
        return true;
    }
    
    private bool RemoveTile(List<Tile> tiles, Tile tile)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].IsEqual(tile))
            {
                tiles.RemoveAt(i);
                return true;
            }
        }
        
        return false;
    }
    
    private void AttemptPlaceBoardTile(MouseState mouseState)
    {
        int[] clickedCoordinates = _board.GetClickedCoordinates(mouseState, _previousMouseState);

        if (clickedCoordinates != null && _board.IsTileEmpty(clickedCoordinates))
        {
            _selectedTile.SetDestinationRectangle
                (_board.GetTileRectangle(clickedCoordinates[0], clickedCoordinates[1]));
            _board.PlaceTile(clickedCoordinates[0], clickedCoordinates[1], _selectedTile);
            _selectedTile = null;
            _boardTileSelected = false;
        }
        else
        {
            _selectedTile.SetDestinationRectangle
                (new Rectangle(mouseState.X - Tile.Width / 2, mouseState.Y - Tile.Height / 2, Tile.Width, Tile.Height));
        }
    }

    private void AttemptPlaceHandTile(MouseState mouseState)
    {
        int[] clickedCoordinates = _board.GetClickedCoordinates(mouseState, _previousMouseState);
            
        if (clickedCoordinates != null && _board.IsTileEmpty(clickedCoordinates))
        {
            _board.PlaceTile(clickedCoordinates[0], clickedCoordinates[1], _selectedTile);
            _selectedTile = null;
            _handTileSelected = false;
        }
        else
        {
            _selectedTile.SetDestinationRectangle
                (new Rectangle(mouseState.X - Tile.Width / 2, mouseState.Y - Tile.Height / 2, Tile.Width, Tile.Height));
        }
    }

    private bool AttemptSelectTile()
    {
        _selectedTile = _playerHand.GetClickedTile(_previousMouseState);

        if (_selectedTile == null)
        {
            _selectedTile = _board.GetClickedTile(_previousMouseState);
            _boardTileCoordinates = _board.GetClickedCoordinates(Mouse.GetState(), _previousMouseState); 
            _boardTileSelected = _selectedTile != null && !_selectedTile.IsEmpty();
                
            if (_boardTileSelected)
            {
                _board.RemoveTile(_boardTileCoordinates[0], _boardTileCoordinates[1]);
                return true;
            }
        }
        else
        {
            _playerHand.RemoveTile(_selectedTile);
            _handTileSelected = true;
            return true;
        }
        
        return false;
    }
    
    private bool RectangleClicked(MouseState mouseState, Rectangle rect)
    {
        return mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released
            && rect.Contains(mouseState.Position);
    }

    private void UpdateInstructions(GameTime gameTime)
    {
        if (GoBackButtonClicked())
        {
            _gamestate = Game;
        }
    }

    private void DrawHome(GameTime gameTime)
    {
        _spriteBatch.Draw(_homeImg, _homeRect, Color.White);
    }

    private void DrawGame(GameTime gameTime)
    {
        _spriteBatch.Draw(_gameImg, _gameRect, Color.White);
        _board.Draw(_spriteBatch, _tilesImg);
        _playerHand.Draw(_spriteBatch, _tilesImg);

        if (!_board.IsValid() || !IsHandValid() || _playerTurn != Player)
        {
            _spriteBatch.Draw(_disabledButtonImg, _disabledButtonRect, Color.White);
        }
        
        if (_selectedTile != null)
        {
            _selectedTile.Draw(_spriteBatch, _tilesImg);
        }

        if (_playerTurn == Waiting)
        {
            _spriteBatch.Draw(_opponentEndTurnImg, _opponentEndTurnRect, Color.White);
        }
    }

    private void DrawInstructions()
    {
        _spriteBatch.Draw(_instructionsImg, _instructionsRect, Color.White);
    }
    
    private bool PlayButtonClicked(GameTime gameTime)
    {
        Rectangle rect = new Rectangle(PlayButtonX, PlayButtonY, PlayButtonWidth, PlayButtonHeight);
        return RectangleClicked(Mouse.GetState(), rect);
    }

    private bool InstructionButtonClicked()
    {
        Rectangle rect = new Rectangle(InstructionButtonX, InstructionButtonY, InstructionButtonWidth, InstructionButtonHeight);
        return RectangleClicked(Mouse.GetState(), rect);
    }

    private bool QuitButtonClicked()
    {
        Rectangle rect = new Rectangle(QuitButtonX, QuitButtonY, QuitButtonWidth, QuitButtonHeight);
        return RectangleClicked(Mouse.GetState(), rect);
    }

    private bool UndoButtonClicked()
    {
        Rectangle rect = new Rectangle(UndoButtonX, UndoButtonY, UndoButtonWidth, UndoButtonHeight);
        return RectangleClicked(Mouse.GetState(), rect);
    }

    private bool GoBackButtonClicked()
    {
        Rectangle rect = new Rectangle(GoBackButtonX, GoBackButtonY, GoBackButtonWidth, GoBackButtonHeight);
        return RectangleClicked(Mouse.GetState(), rect);
    }

    private bool EndTurnButtonClicked()
    {
        Rectangle rect = new Rectangle(EndTurnButtonX, EndTurnButtonY, EndTurnButtonWidth, EndTurnButtonHeight);
        return RectangleClicked(Mouse.GetState(), rect);
    }

    private bool ExitButtonClicked()
    {
        Rectangle rect = new Rectangle(ExitButtonX, ExitButtonY, ExitButtonWidth, ExitButtonHeight);
        return RectangleClicked(Mouse.GetState(), rect);
    }

    private void Undo()
    {
        _board = new Board(_oldBoard);
        _playerHand = new Hand(_oldHand);
    }

    private void SetupGame()
    {
        _deck = new Deck();
        _playerHand = new Hand();
        _opponentHand = new Hand();
        _board = new Board();
        
        _playerHand.Populate(_deck); 
        _opponentHand.Populate(_deck);
        
        _oldBoard = new Board(_board);
        _oldHand = new Hand(_playerHand);
        
        _handTileSelected = false;
        _boardTileSelected = false;
        _boardTileCoordinates = null;
        _playerTurn = Player;
    }

    private List<List<List<Tile>>> GetPotentialGroups(Board board, Hand hand)
    {
        List<List<List<Tile>>> groups = new List<List<List<Tile>>>();
        List<Tile>[] tilesByNumber = new List<Tile>[13];

        List<Tile> boardTiles = board.GetTiles().ToList();
        List<Tile> usableTiles = hand.GetTiles().ToList();
        usableTiles.AddRange(boardTiles);

        for (int i = 0; i < tilesByNumber.Length; i++)
        {
            tilesByNumber[i] = new List<Tile>();
        }
        
        for (int i = 0; i < usableTiles.Count; i++)
        {
            tilesByNumber[usableTiles[i].Number() - 1].Add(usableTiles[i]);
        }

        for (int i = 0; i < tilesByNumber.Length; i++)
        {
            groups.AddRange(GetPotentialGroupsOneNumber(tilesByNumber[i], i + 1));
        }
        
        return groups;
    }

    private List<List<List<Tile>>> GetPotentialGroupsOneNumber(List<Tile> tiles, int number)
    {
        int red = 0;
        int blue = 1;
        int yellow = 2;
        int black = 3;
        
        int num0s = 0;
        int num1s = 0;
        int num2s = 0;

        int[] numColours = [0, 0, 0, 0];
        ConsoleColor[] colours = 
            { ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.Black };
        
        for (int i = 0; i < tiles.Count; i++)
        {
            switch (tiles[i].Colour())
            {
                case ConsoleColor.Red:
                    numColours[red]++;
                    break;
                case ConsoleColor.Blue:
                    numColours[blue]++;
                    break;
                case ConsoleColor.Yellow:
                    numColours[yellow]++;
                    break;
                case ConsoleColor.Black:
                    numColours[black]++;
                    break;
            }
        }
        
        foreach (int i in numColours)
        {
            switch (i)
            {
                case 0:
                    num0s++;
                    break;
                case 1:
                    num1s++;
                    break;
                case 2:
                    num2s++;
                    break;
            }
        }

        if (num0s >= 2)
        {
            return [];
        }

        if (num0s >= 1 && num1s >= 1)
        {
            List<Tile> group = [];

            for (int i = 0; i < colours.Length; i++)
            {
                if (numColours[i] != 0)
                {
                    group.Add(new Tile(number, colours[i]));
                }
            }

            return [[group]];
        }
        
        if (num0s == 1 && num2s == 3)
        {
            List<Tile> group1 = [];
            List<Tile> group2 = [];

            for (int i = 0; i < colours.Length; i++)
            {
                if (numColours[i] != num0s)
                {
                    group1.Add(new Tile(number, colours[i]));
                    group2.Add(new Tile(number, colours[i]));
                }
            }

            return [[group1], [group2]];
        }

        if (num1s >= 2)
        {
            List<Tile> group1 = [];
            List<Tile> group2 = [];
            List<Tile> group3 = [];
            List<Tile> group4 = [];
            List<Tile> group5 = [];
            
            group1.Add(new Tile(number, colours[0]));
            group1.Add(new Tile(number, colours[1]));
            group1.Add(new Tile(number, colours[2]));
            group1.Add(new Tile(number, colours[3]));
            
            group2.Add(new Tile(number, colours[1]));
            group2.Add(new Tile(number, colours[2]));
            group2.Add(new Tile(number, colours[3]));

            group3.Add(new Tile(number, colours[0]));
            group3.Add(new Tile(number, colours[2]));
            group3.Add(new Tile(number, colours[3]));

            group4.Add(new Tile(number, colours[0]));
            group4.Add(new Tile(number, colours[1]));
            group4.Add(new Tile(number, colours[3]));

            group5.Add(new Tile(number, colours[0]));
            group5.Add(new Tile(number, colours[1]));
            group5.Add(new Tile(number, colours[2]));
            
            return [[group1, group2, group3, group4, group5]];
        }

        if (numColours[0] == 1 || numColours[1] == 1 || numColours[2] == 1 || numColours[3] == 1)
        {
            List<List<Tile>> groupList1 = [];
            List<List<Tile>> groupList2 = [];
            List<Tile> group1 = [];
            List<Tile> group2 = [];
            List<Tile> group3 = [];
            List<Tile> group4 = [];
            List<Tile> group5 = [];
            List<Tile> group6 = [];
            
            group1.Add(new Tile(number, colours[0]));
            group1.Add(new Tile(number, colours[1]));
            group1.Add(new Tile(number, colours[2]));
            group1.Add(new Tile(number, colours[3]));
            
            group2.Add(new Tile(number, colours[1]));
            group2.Add(new Tile(number, colours[2]));
            group2.Add(new Tile(number, colours[3]));

            group3.Add(new Tile(number, colours[0]));
            group3.Add(new Tile(number, colours[2]));
            group3.Add(new Tile(number, colours[3]));

            group4.Add(new Tile(number, colours[0]));
            group4.Add(new Tile(number, colours[1]));
            group4.Add(new Tile(number, colours[3]));

            group5.Add(new Tile(number, colours[0]));
            group5.Add(new Tile(number, colours[1]));
            group5.Add(new Tile(number, colours[2]));
            
            for (int i = 0; i < colours.Length; i++)
            {
                if (numColours[i] == 2)
                {
                    group6.Add(new Tile(number, colours[i]));
                }
            }

            groupList1 = [group1, group2, group3, group4, group5];
            groupList2 = [group6];
            return [groupList1, groupList2];
        }

        List<Tile> rybkGroup1 = [];
        List<Tile> ybkGroup1 = [];
        List<Tile> rbkGroup1 = [];
        List<Tile> rykGroup1 = [];
        List<Tile> rybGroup1 = [];
        
        List<Tile> rybkGroup2 = [];
        List<Tile> ybkGroup2 = [];
        List<Tile> rbkGroup2 = [];
        List<Tile> rykGroup2 = [];
        List<Tile> rybGroup2 = [];

        rybkGroup1.Add(new Tile(number, colours[0]));
        rybkGroup1.Add(new Tile(number, colours[1]));
        rybkGroup1.Add(new Tile(number, colours[2]));
        rybkGroup1.Add(new Tile(number, colours[3]));
        
        ybkGroup1.Add(new Tile(number, colours[1]));
        ybkGroup1.Add(new Tile(number, colours[2]));
        ybkGroup1.Add(new Tile(number, colours[3]));
        
        rbkGroup1.Add(new Tile(number, colours[0]));
        rbkGroup1.Add(new Tile(number, colours[2]));
        rbkGroup1.Add(new Tile(number, colours[3]));
        
        rykGroup1.Add(new Tile(number, colours[0]));
        rykGroup1.Add(new Tile(number, colours[1]));
        rykGroup1.Add(new Tile(number, colours[3]));
        
        rybGroup1.Add(new Tile(number, colours[0]));
        rybGroup1.Add(new Tile(number, colours[1]));
        rybGroup1.Add(new Tile(number, colours[2]));
        
        rybkGroup2.Add(new Tile(number, colours[0]));
        rybkGroup2.Add(new Tile(number, colours[1]));
        rybkGroup2.Add(new Tile(number, colours[2]));
        rybkGroup2.Add(new Tile(number, colours[3]));
        
        ybkGroup2.Add(new Tile(number, colours[1]));
        ybkGroup2.Add(new Tile(number, colours[2]));
        ybkGroup2.Add(new Tile(number, colours[3]));
        
        rbkGroup2.Add(new Tile(number, colours[0]));
        rbkGroup2.Add(new Tile(number, colours[2]));
        rbkGroup2.Add(new Tile(number, colours[3]));
        
        rykGroup2.Add(new Tile(number, colours[0]));
        rykGroup2.Add(new Tile(number, colours[1]));
        rykGroup2.Add(new Tile(number, colours[3]));
        
        rybGroup2.Add(new Tile(number, colours[0]));
        rybGroup2.Add(new Tile(number, colours[1]));
        rybGroup2.Add(new Tile(number, colours[2]));

        return [[rybkGroup1, ybkGroup1, rbkGroup1, rykGroup1, rybGroup1], 
            [rybkGroup2, ybkGroup2, rbkGroup2, rykGroup2, rybGroup2]];
    }

    private List<Tile> SortColourThenRank(List<Tile> tiles)
    {
        TileCollection redTiles = new();
        TileCollection blueTiles = new();
        TileCollection yellowTiles = new();
        TileCollection blackTiles = new();
        List<Tile> sortedTiles = new List<Tile>();

        foreach (Tile tile in tiles)
        {
            switch (tile.Colour())
            {
                case ConsoleColor.Red:
                    redTiles.Add(tile);
                    break;
                case ConsoleColor.Blue:
                    blueTiles.Add(tile);
                    break;
                case ConsoleColor.Yellow:
                    yellowTiles.Add(tile);
                    break;
                case ConsoleColor.Black:
                    blackTiles.Add(tile);
                    break;
            }
        }
        
        sortedTiles.AddRange(redTiles.GetTiles());
        sortedTiles.AddRange(blueTiles.GetTiles());
        sortedTiles.AddRange(yellowTiles.GetTiles());
        sortedTiles.AddRange(blackTiles.GetTiles());
        return sortedTiles;
    }

    private void SortRank(List<Tile> tiles)
    {
        Tile temp;

        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = 1; j < tiles.Count - i; j++)
            {
                if (tiles[j - 1].Number() > tiles[j].Number())
                {
                    temp = tiles[j - 1];
                    tiles[j - 1] = tiles[j];
                    tiles[j] = temp;
                }
            }
        }
    }

    private bool Contains(List<Tile> tiles, Tile tile)
    {
        foreach (Tile tile1 in tiles)
        {
            if (tile1.IsEqual(tile))
            {
                return true;
            }
        }
        
        return false;
    }

    private List<Tile> GetRun(List<Tile> tiles)
    {
        if (tiles.Count == 0)
        {
            return null;
        }
        
        List<Tile> run = new List<Tile>() { tiles[0] };

        bool foundDuplicate = false;
        int numDuplicates = 0;

        for (int i = 1; i < tiles.Count; i++)
        {
            if (foundDuplicate)
            {
                if (tiles[i].Colour() != run[run.Count - 1].Colour())
                { 
                    if (run.Count >= Board.MinSetSize)
                    {
                        return run;
                    }
                                    
                    run = new List<Tile>() { tiles[i] };
                                    
                    foundDuplicate = false;
                    numDuplicates = 0;
                }
                else if (ContainsBoth(tiles, new Tile(run[run.Count - 1].Number() + 1, run[run.Count - 1].Colour())))
                {
                    run.Add(tiles[i]);
                    numDuplicates++;
                    i++;
                }
                else
                {
                    if (run.Count >= Board.MinSetSize)
                    {
                        if (numDuplicates == 1)
                        {
                            if (Contains(tiles, new Tile(run[run.Count - 1].Number() + 1, run[run.Count - 1].Colour()))
                                && Contains(tiles, new Tile(run[run.Count - 1].Number() + 2, run[run.Count - 1].Colour())))
                            {
                                return run;
                            }
                        }
                        else if (numDuplicates == 2)
                        {
                            if (Contains(tiles, new Tile(run[run.Count - 1].Number() + 1, run[run.Count - 1].Colour())))
                            {
                                return run;
                            }
                        }
                        else
                        {
                            return run;
                        }
                    }
                
                    foundDuplicate = false;
                    numDuplicates = 0;
                    i--;
                }
            }
            else
            {
                if (tiles[i].Colour() != run[run.Count - 1].Colour())
                {
                    if (run.Count >= Board.MinSetSize)
                    {
                        return run;
                    }
                    
                    run = new List<Tile>() { tiles[i] };
                }
                else if (tiles[i].Number() == run[run.Count - 1].Number())
                {
                    foundDuplicate = true;
                    numDuplicates = 1;
                }
                else if (tiles[i].Number() == run[run.Count - 1].Number() + 1)
                {
                    run.Add(tiles[i]);
                }
                else
                {
                    if (run.Count >= Board.MinSetSize)
                    {
                        return run;
                    }
                    
                    run = new List<Tile>() { tiles[i] };
                }
            }
        }
        
        if (run.Count >= Board.MinSetSize)
        {
            return run;
        }
        
        return null;
    }

    // Precondition: tiles is sorted by colour then rank
    public List<List<Tile>> GetAllRuns(List<Tile> tiles)
    {
        List<List<Tile>> runs = new List<List<Tile>>();
        List<Tile> run;

        while (true)
        {
            run = GetRun(tiles);
            if (run == null) break;
            runs.Add(run);
            foreach (Tile tile in run) Remove(tiles, tile);
        }
        
        return runs;
    }

    public void Remove(List<Tile> tiles, Tile tile)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].IsEqual(tile))
            {
                tiles.RemoveAt(i);
                return;
            }
        }
    }

    public bool ContainsBoth(List<Tile> tiles, Tile tile)
    {
        int numTile = 0;

        foreach (Tile tile1 in tiles)
        {
            if (tile1.IsEqual(tile))
            {
                numTile++;
            }
        }
        
        return numTile == 2;
    }
    
    private List<List<List<Tile>>> GenerateGroupCombinations(List<List<List<Tile>>> potentialGroups)
    {
        List<List<List<Tile>>> ans = [];
        
        if (potentialGroups.Count == 0) return [];
        if (potentialGroups.Count == 1)
        {
            ans.Add([]);
            foreach (List<Tile> i in potentialGroups[0]) ans.Add([i]);
            return ans;
        }
        
        List<List<List<Tile>>> potentialGroupsCopy = potentialGroups.ToList();
        potentialGroupsCopy.RemoveAt(potentialGroups.Count - 1);
        List<List<List<Tile>>> smallAns = GenerateGroupCombinations(potentialGroupsCopy);

        foreach (List<List<Tile>> i in smallAns)
        {
            ans.Add(i);
            foreach (List<Tile> j in potentialGroups[^1])
            {
                List<List<Tile>> iCopy = i.ToList();
                iCopy.Add(j);
                ans.Add(iCopy);
            }
        }

        return ans;
    }

    private Move GetBestMove(Board board, Hand hand, List<List<List<Tile>>> potentialGroups)
    {
        Move bestMove;
        int bestScore;
        
        List<List<List<Tile>>> setCombinations = GenerateGroupCombinations(potentialGroups);
        Move[] moves = new Move[setCombinations.Count];
        TileCollection[] remainingTiles = new TileCollection[setCombinations.Count];
        
        List<Tile> boardTiles = board.GetTiles();
        List<Tile> handTiles = hand.GetTiles();

        if (setCombinations.Count == 0)
        {
            List<Tile> tiles = board.GetTiles().ToList();
            tiles.AddRange(hand.GetTiles());
            return new Move(GetAllRuns(SortColourThenRank(tiles)), board, hand);
        }

        for (int i = 0; i < setCombinations.Count; i++)
        {
            remainingTiles[i] = new();
            remainingTiles[i].AddRange(boardTiles);
            remainingTiles[i].AddRange(handTiles);
            TileCollection usedTilesCollection = new();

            foreach (List<Tile> set in setCombinations[i])
            {
                usedTilesCollection.AddRange(set);
            }
            
            List<Tile> usedTiles = usedTilesCollection.GetTiles();

            foreach (Tile tile in usedTiles)
            {
                remainingTiles[i].Remove(tile);
            }
            
            setCombinations[i].AddRange(GetAllRuns(remainingTiles[i].GetTiles()));
            moves[i] = new Move(setCombinations[i], board, hand);
        }
        
        bestMove = moves[0];
        bestScore = bestMove.Score();

        for (int i = 1; i < moves.Length; i++)
        {
            int score = moves[i].Score();

            if (score > bestScore)
            {
                bestMove = moves[i];
                bestScore = score;
            }
        }
        
        return bestMove;
    }
}