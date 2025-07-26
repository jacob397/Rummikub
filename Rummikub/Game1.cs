// Author: Jacob Lisogurski
// File Name: Game1.cs
// Project Name: Rummikub
// Creation Date: June 11, 2025
// Modified Date: July 26, 2025
// Description: An online version of the tile game Rummikub.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rummikub.Entities;

namespace Rummikub;

public class Game1 : Game
{
    // Gamestate constants
    private const int Home = 0;
    private const int Game = 1;
    private const int Instructions = 2;
    private const int Win = 3;
    private const int Lose = 4;
    private const int Tie = 5;

    // Turn constants
    private const int Player = 0;
    private const int Opponent = 1;
    private const int Waiting = 2;
    private const int PlayerWon = 3;
    private const int OpponentWon = 4;
    private const int PlayerTie = 5;

    // Length of the opponent's turn (ms)
    private const int MaxTurnLength = 6000;

    // Coordinates/Dimensions of buttons
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

    private const int DisabledButtonsX = 310;
    private const int DisabledButtonsY = 655;
    private const int DisabledButtonsWidth = 800;
    private const int DisabledButtonsHeight = 50;
    
    private const int SortColourNumberX = 1010;
    private const int SortColourNumberY = 655;
    private const int SortColourNumberWidth = 100;
    private const int SortColourNumberHeight = 50;

    private const int SortNumberColourX = 900;
    private const int SortNumberColourY = 655;
    private const int SortNumberColourWidth = 100;
    private const int SortNumberColourHeight = 50;

    private const int HomeButtonX = 360;
    private const int HomeButtonY = 424;
    private const int HomeButtonWidth = 480;
    private const int HomeButtonHeight = 180;
    
    // Coordinates/Dimensions of the image that communicates whose turn it is
    private const int TurnMessageX = 90;
    private const int TurnMessageY = 655;
    private const int TurnMessageWidth = 210;
    private const int TurnMessageHeight = 50;
    
    // Images
    private Texture2D _homeImg;
    private Texture2D _gameImg;
    private Texture2D _instructionsImg;
    private Texture2D _tilesImg;
    private Texture2D _disabledButtonImg;
    private Texture2D _playerTurnImg;
    private Texture2D _opponentTurnImg;
    private Texture2D _sortColourNumberImg;
    private Texture2D _sortNumberColourImg;
    private Texture2D _disabledButtonsImg;
    private Texture2D _clickToProceedImg;
    private Texture2D _winScreenImg;
    private Texture2D _loseScreenImg;
    private Texture2D _tieScreenImg;
    
    // Rectangles for images
    private Rectangle _homeRect;
    private Rectangle _gameRect;
    private Rectangle _endgameRect;
    private Rectangle _instructionsRect;
    private Rectangle _disabledButtonRect;
    private Rectangle _turnRect;
    private Rectangle _sortColourNumberRect;
    private Rectangle _sortNumberColourRect;
    private Rectangle _disabledButtonsRect;
    private Rectangle _homeButtonRect;
    
    // Sound effects
    private SoundEffect _tileClickSound;
    private SoundEffect _buttonClickSound;
    
    // The current game state, player turn, and length of the opponent's turn (if applicable)
    private int _gamestate;
    private int _playerTurn;
    private int _turnLength;

    // The deck, game board, and both hands
    private Deck _deck;
    private Board _board;
    private Hand _playerHand;
    private Hand _opponentHand;

    // The board and hand at the beginning of a given turn (this is needed for the undo button)
    private Board _oldBoard;
    private Hand _oldHand;
    
    // The current and previous selected tie, booleans tracking whether a hand tile or board tile has been selected,
    // and the coordinates of the tile on the board (if applicable)
    private Tile _selectedTile;
    private Tile _previousSelectedTile;
    private bool _handTileSelected;
    private bool _boardTileSelected;
    private int[] _boardTileCoordinates;

    // Tracks if the computer player's previous turn was skipped
    private bool _previousTurnSkipped;

    // The opponent's move
    private Move _opponentMove;
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private MouseState _previousMouseState;

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
        
        // Set the gamestate so that the game opens on the home screen
        _gamestate = Home;
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load images
        _homeImg = Content.Load<Texture2D>("Images/Backgrounds/home");
        _gameImg = Content.Load<Texture2D>("Images/Backgrounds/game");
        _tilesImg = Content.Load<Texture2D>("Images/Foregrounds/tiles");
        _instructionsImg = Content.Load<Texture2D>("Images/Backgrounds/instructions");
        _disabledButtonImg = Content.Load<Texture2D>("Images/Foregrounds/disabled_button");
        _playerTurnImg = Content.Load<Texture2D>("Images/Foregrounds/player_turn");
        _opponentTurnImg = Content.Load<Texture2D>("Images/Foregrounds/opponent_turn");
        _sortColourNumberImg = Content.Load<Texture2D>("Images/Foregrounds/sort_colour_number");
        _sortNumberColourImg = Content.Load<Texture2D>("Images/Foregrounds/sort_number_colour");
        _disabledButtonsImg = Content.Load<Texture2D>("Images/Foregrounds/disabled_buttons");
        _clickToProceedImg = Content.Load<Texture2D>("Images/Foregrounds/click_to_proceed");
        _winScreenImg = Content.Load<Texture2D>("Images/Backgrounds/win_screen");
        _loseScreenImg = Content.Load<Texture2D>("Images/Backgrounds/lose_screen");
        _tieScreenImg = Content.Load<Texture2D>("Images/Backgrounds/tie_screen");
        
        // Initialize rectangles for images
        _homeRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _gameRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _instructionsRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _disabledButtonRect = new Rectangle(DisabledEndTurnButtonX, DisabledEndTurnButtonY, 
            DisabledEndTurnButtonWidth, DisabledEndTurnButtonHeight);
        _turnRect = new Rectangle(TurnMessageX, TurnMessageY, TurnMessageWidth, TurnMessageHeight);
        _sortColourNumberRect = new Rectangle(SortColourNumberX, SortColourNumberY, SortColourNumberWidth, SortColourNumberHeight);
        _sortNumberColourRect = new Rectangle(SortNumberColourX, SortNumberColourY, SortNumberColourWidth, SortNumberColourHeight);
        _disabledButtonsRect = new Rectangle(DisabledButtonsX, DisabledButtonsY, DisabledButtonsWidth, DisabledButtonsHeight);
        _endgameRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _homeButtonRect = new Rectangle(HomeButtonX, HomeButtonY, HomeButtonWidth, HomeButtonHeight);
        
        // Initialize audio
        _tileClickSound = Content.Load<SoundEffect>("Audio/tile_click");
        _buttonClickSound = Content.Load<SoundEffect>("Audio/button_click");
    }

    protected override void Update(GameTime gameTime)
    {
        // Quit if back or escape keys are clicked
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update based on the gamestate
        switch (_gamestate)
        {
            case Home:
                UpdateHome();
                break;
            case Game:
                UpdateGame(gameTime);
                break;
            case Instructions:
                UpdateInstructions();
                break;
            case Win:
                UpdateEndgame();
                break;
            case Lose:
                UpdateEndgame();
                break;
            case Tie:
                UpdateEndgame();
                break;
        }

        // Store the previous mouse state to later distinguish one long click from two consecutive clicks
        _previousMouseState = Mouse.GetState();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        // Draw based on the gamestate
        switch (_gamestate)
        {
            case Home:
                DrawHome();
                break;
            case Game:
                DrawGame();
                break;
            case Instructions:
                DrawInstructions();
                break;
            case Win:
                DrawWin();
                break;
            case Lose:
                DrawLose();
                break;
            case Tie:
                DrawTie();
                break;
        }
        
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    // Pre: None
    // Post: None
    // Desc: Update the home screen
    private void UpdateHome()
    {
        if (PlayButtonClicked())
        {
            // If the play button was clicked, start the game
            _buttonClickSound.Play();
            _gamestate = Game;
            SetupGame();
        }
        else if (ExitButtonClicked())
        {
            // If the exit button was clicked, end the game
            Exit();
        }
    }

    // Pre: gameTime, to use for a timer
    // Post: None
    // Desc: Update the game
    private void UpdateGame(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();
        
        // Update the game based on whose turn it is
        switch (_playerTurn)
        {
            case Player:
                DoPlayerMove();
                break;
            case Opponent:
                DoOpponentMove(gameTime);
                break;
            case Waiting:
                WaitForOpponentMove(gameTime);
                break;
            case PlayerWon:
                // If the user clicks, end the game
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _gamestate = Win;
                }
                break;
            case OpponentWon:
                // If the user clicks, end the game
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _gamestate = Lose;
                }
                break;
            case PlayerTie:
                // If the user clicks, end the game
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _gamestate = Tie;
                }
                break;
        }
        
        // Move the player's hand if prompted
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            _playerHand.MoveLeft();
        }
        else if (keyboardState.IsKeyDown(Keys.Right))
        {
            _playerHand.MoveRight();
        }
        
        // If a tile has been selected or deselected, play an audio
        if ((_previousSelectedTile == null) != (_selectedTile == null))
        {
            if (_selectedTile != null && !_selectedTile.IsEmpty()) _tileClickSound.Play();
            if (_previousSelectedTile != null && !_previousSelectedTile.IsEmpty()) _tileClickSound.Play();
        }
        
        // Store the previous selected tile
        _previousSelectedTile = _selectedTile;
    }
    
    // Pre: None
    // Post: None
    // Desc: Update the instructions
    private void UpdateInstructions()
    {
        if (GoBackButtonClicked())
        {
            _buttonClickSound.Play();
            _gamestate = Game;
        }
    }

    // Pre: None
    // Post: None
    // Desc: Update the end game screen
    private void UpdateEndgame()
    {
        if (HomeButtonClicked())
        {
            _buttonClickSound.Play();
            _gamestate = Home;
        }
    }
    
    // Pre: None
    // Post: None
    // Desc: Draw the game
    private void DrawHome()
    {
        _spriteBatch.Draw(_homeImg, _homeRect, Color.White);
    }
    
    // Pre: None
    // Post: None
    // Desc: Draw the game
    private void DrawGame()
    {
        // Draw the game
        _spriteBatch.Draw(_gameImg, _gameRect, Color.White);
        _spriteBatch.Draw(_sortColourNumberImg, _sortColourNumberRect, Color.White);
        _spriteBatch.Draw(_sortNumberColourImg, _sortNumberColourRect, Color.White);
        _board.Draw(_spriteBatch, _tilesImg);
        _playerHand.Draw(_spriteBatch, _tilesImg);

        // Disable the button that allows for ending a turn if the player should not be able to end a turn
        if (!_board.IsValid() || !IsHandValid() || _playerTurn != Player)
        {
            _spriteBatch.Draw(_disabledButtonImg, _disabledButtonRect, Color.White);
        }

        // Indicate whose turn it is, whether a player has won, and whether any buttons are disabled
        switch (_playerTurn)
        {
            case Player:
                _spriteBatch.Draw(_playerTurnImg, _turnRect, Color.White);
                break;
            case Opponent:
                _spriteBatch.Draw(_opponentTurnImg, _turnRect, Color.White);
                _spriteBatch.Draw(_disabledButtonsImg, _disabledButtonsRect, Color.White);
                break;
            case Waiting:
                _spriteBatch.Draw(_opponentTurnImg, _turnRect, Color.White);
                _spriteBatch.Draw(_disabledButtonsImg, _disabledButtonsRect, Color.White);
                break;
            case PlayerWon:
                _spriteBatch.Draw(_clickToProceedImg, _turnRect, Color.White);
                _spriteBatch.Draw(_disabledButtonsImg, _disabledButtonsRect, Color.White);
                break;
            case OpponentWon:
                _spriteBatch.Draw(_clickToProceedImg, _turnRect, Color.White);
                _spriteBatch.Draw(_disabledButtonsImg, _disabledButtonsRect, Color.White);
                break;
            case PlayerTie:
                _spriteBatch.Draw(_clickToProceedImg, _turnRect, Color.White);
                _spriteBatch.Draw(_disabledButtonsImg, _disabledButtonsRect, Color.White);
                break;
        }
        
        // Draw the selected tile if applicable (the tile the player is currently dragging)
        if (_selectedTile != null) _selectedTile.Draw(_spriteBatch, _tilesImg);
    }
    
    // Pre: None
    // Post: None
    // Desc: Draw the instructions
    private void DrawInstructions()
    {
        _spriteBatch.Draw(_instructionsImg, _instructionsRect, Color.White);
    }
    
    // Pre: None
    // Post: None
    // Desc: Draw the win screen
    private void DrawWin()
    {
        _spriteBatch.Draw(_winScreenImg, _endgameRect, Color.White);
    }

    // Pre: None
    // Post: None
    // Desc: Draw the lose screen
    private void DrawLose()
    {
        _spriteBatch.Draw(_loseScreenImg, _endgameRect, Color.White);
    }

    // Pre: None
    // Post: None
    // Desc: Draw the tie screen
    private void DrawTie()
    {
        _spriteBatch.Draw(_tieScreenImg, _endgameRect, Color.White);
    }
    
    // Pre: None
    // Post: None
    // Desc: Setup the game
    private void SetupGame()
    {
        // Create the deck, board, and both player hands
        _deck = new Deck();
        _playerHand = new Hand();
        _opponentHand = new Hand();
        _board = new Board();
        
        // Draw the initial fourteen tiles in both hands
        _playerHand.Populate(_deck);
        _opponentHand.Populate(_deck);
        
        // Store the old board and old hand so the undo button works
        _oldBoard = new Board(_board);
        _oldHand = new Hand(_playerHand);
        
        // Note that when the game starts, no tile has been selected and no turns have been skipped
        _handTileSelected = false;
        _boardTileSelected = false;
        _boardTileCoordinates = null;
        _previousTurnSkipped = false;
        
        // Have the player go first
        _playerTurn = Player;
    }
    
    // Pre: None
    // Post: None
    // Desc: Do the player's turn
    private void DoPlayerMove()
    {
        // Allow the player to move tiles
        AttemptToMoveTile();
            
        // Respond to button clicks
        if (_selectedTile == null && InstructionButtonClicked())
        {
            _buttonClickSound.Play();
            _gamestate = Instructions;
        }
        else if (_selectedTile == null && QuitButtonClicked())
        {
            _buttonClickSound.Play();
            _gamestate = Home;
        }
        else if (_selectedTile == null && UndoButtonClicked())
        {
            _buttonClickSound.Play();
            Undo();
        }
        else if (_selectedTile == null && EndTurnButtonClicked())
        {
            if (_board.IsValid() && IsHandValid())
            {
                _buttonClickSound.Play();
                EndTurn();
            }
        }
        else if (_selectedTile == null && SortColourNumberButtonClicked())
        {
            _buttonClickSound.Play();
            _playerHand.SortColourThenNumber();
        }
        else if (_selectedTile == null && SortNumberColourButtonClicked())
        {
            _buttonClickSound.Play();
            _playerHand.SortNumberThenColour();
        }
    }
    
    // Pre: gameTime
    // Post: None
    // Desc: Do the opponent player's move
    private void DoOpponentMove(GameTime gameTime)
    {
        // Search for a potential move
        _opponentMove = GetBestMove(_board, _opponentHand);
        if (_opponentMove.Score() <= 0) _opponentMove = GetModerateMove(_board, _opponentHand);
        if (_opponentMove.Score() <= 0) _opponentMove = GetGoodMove(_board, _opponentHand);
        if (_opponentMove.Score() <= 0) _opponentMove = GetSimpleMove(_board, _opponentHand);
        
        // Update the player turn and the turn length timer
        _playerTurn = Waiting;
        _turnLength = (int) gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    // Pre: gameTime
    // Post: None
    // Desc: Wait six seconds after the opponent move started before doing the opponent move and starting the player move
    private void WaitForOpponentMove(GameTime gameTime)
    {
        // Update the turn timer
        _turnLength += (int) gameTime.ElapsedGameTime.TotalMilliseconds;
        
        // If the timer is up, finish the turn
        if (_turnLength > MaxTurnLength)
        {
            // If the opponent's move involves placing no hand tiles or is invalid, have them draw a tile
            // Otherwise, do the turn
            if (_opponentMove.Score() > 0)
            {
                _previousTurnSkipped = false;
                _opponentMove.Do();
            }
            else if (_deck.IsEmpty())
            {
                _previousTurnSkipped = true;
            }
            else
            {
                _previousTurnSkipped = false;
                _opponentHand.Add(_deck.Pop());
            }

            // Update the player turn, and the old board and old hand
            _playerTurn = _opponentHand.IsEmpty() ? OpponentWon : Player;
            _oldBoard = new Board(_board);
            _oldHand = new Hand(_playerHand);
        }
    }
    
    // Pre: None
    // Post: None
    // Desc: Allow the player to move tiles around
    private void AttemptToMoveTile()
    {
        // Track if a tile has been picked up/selected in this call to update
        bool tileRecentlySelected = false;
        
        if (_boardTileSelected || _handTileSelected)
        {
            // If a tile has been selected, attempt to place it
            AttemptPlaceTile();
        }
        else if (AttemptSelectTile())
        {
            // If no tile has been selected, attempt to select one and note whether this has been successful
            tileRecentlySelected = true;
        }

        // Place a selected tile back in the player's hand
        if (_selectedTile != null && !tileRecentlySelected && _playerHand.Insert(_selectedTile, _previousMouseState))
        {
            _handTileSelected = false;
            _boardTileSelected = false;
            _selectedTile = null;
        }
    }
    
    // Pre: None
    // Post: None
    // Desc: Allow the player to select a tile to move
    private bool AttemptSelectTile()
    {
        // Attempt to select a hand tile
        var temp = _playerHand.GetClickedTile(_previousMouseState);
        _selectedTile = temp.Item1;
        int index = temp.Item2;

        // If no tile was selected from the hand, attempt to select one from the board
        // Otherwise, remove the selected tile from the hand
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
            _playerHand.RemoveAt(index);
            _handTileSelected = true;
            return true;
        }
        
        return false;
    }
    
    // Pre: None
    // Post: None
    // Desc: Allow the player to place a tile
    private void AttemptPlaceTile()
    {
        MouseState mouseState = Mouse.GetState();
        
        // If a board tile has been clicked, this stores the coordinates of that tile on the board
        // Note this is different from the coordinates of the cursor
        int[] clickedCoordinates = _board.GetClickedCoordinates(mouseState, _previousMouseState);

        if (clickedCoordinates != null && _board.IsTileEmpty(clickedCoordinates))
        {
            // If an empty space on the board has been clicked, the selected tile is placed there
            _board.PlaceTile(clickedCoordinates[0], clickedCoordinates[1], _selectedTile);
            _selectedTile = null;
            _boardTileSelected = false;
            _handTileSelected = false;
        }
        else
        {
            // If the tile hasn't been placed, this keeps the tile on the cursor
            _selectedTile.SetDestinationRectangle
                (new Rectangle(mouseState.X - Tile.Width / 2, mouseState.Y - Tile.Height / 2, Tile.Width, Tile.Height));
        }
    }
    
    // Pre: None
    // Post: A bool representing the negation of whether the player's hand has any board tiles in it
    // Desc: Return said bool
    private bool IsHandValid()
    {
        // Store the player's current and former tiles
        List<Tile> newHandTiles = _playerHand.GetTiles();
        List<Tile> oldHandTiles = _oldHand.GetTiles().ToList();

        // Return false if the new hand is not a subset of the old hand (accounting for multiplicity)
        foreach (Tile tile in newHandTiles) if (!RemoveTile(oldHandTiles, tile)) return false;
        
        // Return true otherwise
        return true;
    }

    // Pre: None
    // Post: None
    // Desc: End the player's turn
    private void EndTurn()
    {
        // If the player has not placed any tiles, attempt to draw a tile
        if (_playerHand.Count() == _oldHand.Count())
        {
            if (!_deck.IsEmpty()) _playerHand.Add(_deck.Pop());
            else if (_previousTurnSkipped) _playerTurn = PlayerTie;
        }

        // Update the player turn
        if (_playerTurn != PlayerTie) _playerTurn = _playerHand.IsEmpty() ? PlayerWon : Opponent;
    }
    
    // Pre: None
    // Post: A bool representing whether the play button was clicked
    // Desc: Return said bool
    private bool PlayButtonClicked()
    {
        Rectangle rect = new Rectangle(PlayButtonX, PlayButtonY, PlayButtonWidth, PlayButtonHeight);
        return RectangleClicked(rect);
    }

    // Pre: None
    // Post: A bool representing whether the instruction button was clicked
    // Desc: Return said bool
    private bool InstructionButtonClicked()
    {
        Rectangle rect = new Rectangle(InstructionButtonX, InstructionButtonY, InstructionButtonWidth, InstructionButtonHeight);
        return RectangleClicked(rect);
    }

    // Pre: None
    // Post: A bool representing whether the quit button was clicked
    // Desc: Return said bool
    private bool QuitButtonClicked()
    {
        Rectangle rect = new Rectangle(QuitButtonX, QuitButtonY, QuitButtonWidth, QuitButtonHeight);
        return RectangleClicked(rect);
    }

    // Pre: None
    // Post: A bool representing whether the undo button was clicked
    // Desc: Return said bool
    private bool UndoButtonClicked()
    {
        Rectangle rect = new Rectangle(UndoButtonX, UndoButtonY, UndoButtonWidth, UndoButtonHeight);
        return RectangleClicked(rect);
    }

    // Pre: None
    // Post: A bool representing whether the go back button was clicked
    // Desc: Return said bool
    private bool GoBackButtonClicked()
    {
        Rectangle rect = new Rectangle(GoBackButtonX, GoBackButtonY, GoBackButtonWidth, GoBackButtonHeight);
        return RectangleClicked(rect);
    }

    // Pre: None
    // Post: A bool representing whether the end turn button was clicked
    // Desc: Return said bool
    private bool EndTurnButtonClicked()
    {
        Rectangle rect = new Rectangle(EndTurnButtonX, EndTurnButtonY, EndTurnButtonWidth, EndTurnButtonHeight);
        return RectangleClicked(rect);
    }

    // Pre: None
    // Post: A bool representing whether the exit button was clicked
    // Desc: Return said bool
    private bool ExitButtonClicked()
    {
        Rectangle rect = new Rectangle(ExitButtonX, ExitButtonY, ExitButtonWidth, ExitButtonHeight);
        return RectangleClicked(rect);
    }

    // Pre: None
    // Post: A bool representing whether the home button was clicked
    // Desc: Return said bool
    private bool HomeButtonClicked()
    {
        return RectangleClicked(_homeButtonRect);
    }

    // Pre: None
    // Post: A bool representing whether the sort number then colour button was clicked
    // Desc: Return said bool
    private bool SortNumberColourButtonClicked()
    {
        return RectangleClicked(_sortNumberColourRect);
    }

    // Pre: None
    // Post: A bool representing whether the sort colour then number button was clicked
    // Desc: Return said bool
    private bool SortColourNumberButtonClicked()
    {
        return RectangleClicked(_sortColourNumberRect);
    }

    // Pre: None
    // Post: None
    // Desc: Undo a player's turn
    private void Undo()
    {
        // Revert the board and player hand
        _board = new Board(_oldBoard);
        _playerHand = new Hand(_oldHand, _playerHand.X());
    }

    // Pre: The board and the opponent player's hand
    // Post: A move object
    // Desc: Get a move for the opponent player where they take a tile from the board and use it to make a set
    private Move GetGoodMove(Board board, Hand hand)
    {
        // Store all combinations of tiles that the opponent can take from the board and put in their hand
        List<(List<Tile>, Board)> removableTileCombinations = GetRemovableTileCombinations(board);

        // Store potential moves (including one where no new tiles are placed)
        List<Move> moves = [new Move(board.GetSets(), board, hand)];

        foreach ((List<Tile>, Board) i in removableTileCombinations)
        {
            moves.Add(GetPotentialGoodMove(board, i.Item2, hand, i.Item1));
        }

        // Search for best move (the one with the best score)
        Move bestMove = moves[0];
        int bestScore = bestMove.Score();

        for (int i = 1; i < moves.Count; i++)
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
    
    // Pre: The board and the opponent player's hand
    // Post: A move object
    // Desc: Randomly select some groups that can be placed, look at every combination of groups, and return the move
    //       with the optimal combination of groups (and all additional runs that can be placed as well)
    private Move GetBestMove(Board board, Hand hand)
    {
        // Find all groups that can be placed
        List<List<List<Tile>>> potentialGroups = GetPotentialGroups(_board.GetTiles().ToList(), 
            _opponentHand.GetTiles().ToList());
        
        // If no groups can be placed, place runs
        if (potentialGroups.Count == 0)
        {
            List<Tile> tiles = board.GetTiles().ToList();
            tiles.AddRange(hand.GetTiles());
            return new Move(GetAllRuns(SortColourThenRank(tiles)), board, hand);
        }
        
        // Randomly pick six elements from potentialGroups for efficiency
        Random rng = new();
        
        for (var i = 0; i < 1000; i++)
        {
            var rand1 = rng.Next(0, potentialGroups.Count);
            var rand2 = rng.Next(0, potentialGroups.Count);
            (potentialGroups[rand1], potentialGroups[rand2]) = (potentialGroups[rand2], potentialGroups[rand1]);
        }
        
        potentialGroups = potentialGroups.Take(6).ToList();
        
        // From the potential groups, generate all valid combinations of sets that can be placed
        // Store all potential moves and the unused tiles in each move
        List<List<List<Tile>>> setCombinations = GenerateGroupCombinations(potentialGroups);
        Move[] moves = new Move[setCombinations.Count];
        TileCollection[] remainingTiles = new TileCollection[setCombinations.Count];
        
        List<Tile> boardTiles = board.GetTiles();
        List<Tile> handTiles = hand.GetTiles();

        for (int i = 0; i < setCombinations.Count; i++)
        {
            // Initialize the list of unused tiles
            remainingTiles[i] = new();
            remainingTiles[i].AddRange(boardTiles);
            remainingTiles[i].AddRange(handTiles);
            
            // Initialize the collection of used tiles
            TileCollection usedTilesCollection = new();

            foreach (List<Tile> set in setCombinations[i])
            {
                usedTilesCollection.AddRange(set);
            }
            
            // Remove all used tiles from the collection of unused tiles
            List<Tile> usedTiles = usedTilesCollection.GetTiles();
            foreach (Tile tile in usedTiles) remainingTiles[i].Remove(tile);
            
            // Get the remaining sets that can be made and add them to the collection of sets for this move
            setCombinations[i].AddRange(GetAllRuns(remainingTiles[i].GetTiles()));
            moves[i] = new Move(setCombinations[i], board, hand);
        }
        
        // Search for the best move (the one that involves placing the most tiles)
        Move bestMove = moves[0];
        int bestScore = bestMove.Score();

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

    // Pre: The board and the opponent player's hand
    // Post: A move object
    // Desc: Get a move for the opponent player where they place a new set
    private Move GetSimpleMove(Board board, Hand hand)
    {
        // Store the sets on the board and search for a new set to place
        List<List<Tile>> sets = board.GetSets();
        List<Tile> set = GetSet(hand.GetTiles().ToList());
        
        // If a new set was found, include it in the player's move
        if (set == null) return new Move(sets, board, hand);
        sets.Add(set);
        return new Move(sets, board, hand);
    }

    // Pre: The board and the opponent player's hand
    // Post: A move object
    // Desc: Get a move for the opponent player where they add a tile to an existing set
    private Move GetModerateMove(Board board, Hand hand)
    {
        List<List<Tile>> sets = board.GetSets();
        List<Tile> handTiles = hand.GetTiles().ToList();

        foreach (List<Tile> set in sets)
        {
            if (Board.IsRun(set))
            {
                // Store tiles that can be appended to the run
                Tile tileBeforeRun = new Tile(set[0].Number() - 1, set[0].Colour());
                Tile tileAfterRun = new Tile(set[^1].Number() + 1, set[0].Colour());

                // If the player has a tile that can be appended to the run, place it
                if (Contains(handTiles, tileBeforeRun))
                {
                    RemoveTile(handTiles, tileBeforeRun);
                    set.Insert(0, tileBeforeRun);
                }
                if (Contains(handTiles, tileAfterRun))
                {
                    RemoveTile(handTiles, tileAfterRun);
                    set.Add(tileAfterRun);
                }
            }
            else if (Board.IsGroup(set) && set.Count == 3)
            {
                // Store the tile that can be appended to the group
                Tile missingTile = GetMissingTile(set);
                
                // If the player has a tile that can be appended to the group, place it
                if (Contains(handTiles, missingTile))
                {
                    RemoveTile(handTiles, missingTile);
                    set.Add(missingTile);
                }
            }
        }
        
        return new Move(sets, board, hand);
    }
    
    // Pre: The board and the opponent player's hand before the move, a list of tiles to remove from the board and add
    //      to the players hand, and the board after removing those tiles
    // Post: A move object
    // Desc: Get a move for the opponent player where they take a tiles (removableTiles) from the board and use them
    //       to make a set
    private Move GetPotentialGoodMove(Board beforeMove, Board afterMove, Hand hand, List<Tile> removableTiles)
    {
        // Store the board and hand after the opponent puts board tiles in their hand
        Board newBoard = new Board(afterMove);
        Hand newHand = new Hand(hand);

        // Store the opponent's hand after they put board tiles in their hand
        foreach (Tile tile in removableTiles) newHand.TempAdd(tile);
        
        // Store the list of sets for the Move object representing this move
        List<List<Tile>> sets = newBoard.GetSets();
        List<Tile> set = GetSet(newHand.GetTiles());
        if (set != null) sets.Add(set);

        // Return the Move object associated with this move
        return new Move(sets, beforeMove, hand);
    }
    
    // Pre: The tiles from the board and the tiles from a hand
    // Post: All possible groups that can be made from board and hand tiles, formatted for use in the
    //       GenerateGroupCombinations function
    // Desc: Get all groups from the board
    private List<List<List<Tile>>> GetPotentialGroups(List<Tile> boardTiles, List<Tile> handTiles)
    {
        // Store a list of all board and hand tiles
        List<Tile> usableTiles = handTiles;
        usableTiles.AddRange(boardTiles);
        
        // The hand tiles sorted by number
        List<Tile>[] tilesByNumber = new List<Tile>[13];
        for (int i = 0; i < tilesByNumber.Length; i++) tilesByNumber[i] = new List<Tile>();
        foreach (Tile tile in usableTiles) tilesByNumber[tile.Number() - 1].Add(tile);

        // Return the groups (the reason this is a triple-nested list is explained in the documentation of the function
        // GetPotentialGroupsOneNumber)
        List<List<List<Tile>>> groups = new List<List<List<Tile>>>();
        for (int i = 0; i < tilesByNumber.Length; i++) groups.AddRange(GetPotentialGroupsOneNumber(tilesByNumber[i], i + 1));
        return groups;
    }

    // Pre: The tiles from the board and hand of a given rank, and the rank of said tiles
    // Post: All possible groups from the list of tiles, formatted for use in the GenerateGroupCombinations function
    // Desc: Get all groups from the board and hand of a given number
    private List<List<List<Tile>>> GetPotentialGroupsOneNumber(List<Tile> tiles, int number)
    {
        // The code here looks complicated, but the general idea is we have a few cases (treat these cases like an
        // if-else-if):
        // Case 1: tiles contains 0 colour A or B tiles for some distinct A, B in S (S = {red, blue, yellow, black})
        //         No groups can be made, return [].
        // Case 2: tiles contains 0 colour A tiles and 1 colour B tile for some distinct A, B in S
        //         One group can be made. It contains three tiles of each colour not equal to A, all with the number
        //         number. Call this group "a" and return [[a]].
        // Case 3: tiles contains 0 Colour A tiles and 2 colour B, C, and D tiles for some distinct A, B, C, D in S
        //         Let a be a group of three tiles, all with the number number, containing tiles of colours B, C, and D.
        //         Let b = a. a and b can both be placed without affecting each other, so we return [[a], [b]].
        // Case 4: tiles contains 1 colour A and B tile for some distinct A, B in S
        //         The possibility of tiles containing no tiles of any other colours has been covered by previous cases,
        //         so all five groups (a, b, c, d, e) with tiles of the specified number can be placed. We return
        //         [[a, b, c, d, e]]. All sets go in the same list because only one set can be placed.
        // Case 5: tiles contains 1 colour A tile and 2 colour B, C, D tiles for some distinct A, B, C, D in S
        //         All five groups (a, b, c, d, e) with tiles of the specified number can be placed. In addition, a
        //         group f with tiles of colour B, C, and D can be placed, without affecting the ability to place any
        //         of sets a, b, c, d, or e. So, we return [[a, b, c, d, e], [f]].
        // Case 6: tiles contains 2 of every colour tile
        //         All five groups (a, b, c, d, e) with tiles of the specified number can be placed. Any two such sets
        //         can be placed, so we return [[a, b, c, d, e], [a, b, c, d, e]].
        //
        // In general, for each inner list of sets, only one set in the list can be placed.

        // Constants to allow us to index an array of colours
        int red = 0;
        int blue = 1;
        int yellow = 2;
        int black = 3;
        
        // The number of colours that show up 0, 1, and 2 times in tiles
        int num0s = 0;
        int num1s = 0;
        int num2s = 0;

        // The number of red, blue, yellow, and black tiles in tiles
        int[] numColours = [0, 0, 0, 0];
        ConsoleColor[] colours = 
            [ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.Black];
        
        // Calculate the number of red, blue, yellow, and black tiles in tiles
        foreach (Tile tile in tiles)
        {
            switch (tile.Colour())
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
        
        // Calculate the number of colours that show up 0, 1, and 2 times in tiles
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

        // Case 1
        if (num0s >= 2)
        {
            return [];
        }

        // Case 2
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
        
        // Case 3
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

        // Case 4
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

        // Case 5
        if (numColours[0] == 1 || numColours[1] == 1 || numColours[2] == 1 || numColours[3] == 1)
        {
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

            List<List<Tile>> groupList1 = [group1, group2, group3, group4, group5];
            List<List<Tile>> groupList2 = [group6];
            return [groupList1, groupList2];
        }

        // Case 6
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
    
    // Pre: A list of tiles
    // Post: A run that can be made from those tiles
    // Desc: Return a run that can be made from the given list of tiles
    private List<Tile> GetRun(List<Tile> tiles)
    {
        // Sort the tiles to better look for runs
        tiles = SortColourThenRank(tiles);
        
        // If there are no tiles, then there are no runs
        if (tiles.Count == 0) return null;
        
        // Store the run
        List<Tile> run = [tiles[0]];
        
        // Store whether both tiles of the same colour and rank have been found, and count the number of consecutive
        // tiles that have duplicates
        bool foundDuplicate = false;
        int numDuplicates = 0;

        for (int i = 1; i < tiles.Count; i++)
        {
            if (foundDuplicate)
            {
                if (tiles[i].Colour() != run[^1].Colour()) // A tile that cannot be added to the run has been found
                {
                    // If the run is sufficiently large, return it
                    if (run.Count >= Board.MinSetSize) return run;
                                
                    // Restart the search for a run
                    run = [tiles[i]];
                                    
                    foundDuplicate = false;
                    numDuplicates = 0;
                }
                else if (ContainsBoth(tiles, new Tile(run[^1].Number() + 1, run[^1].Colour())))
                {
                    // If the next tile in the run has been found (twice), add it to the run and note the existence of
                    // duplicates
                    run.Add(tiles[i]);
                    numDuplicates++;
                    i++;
                }
                else
                {
                    // In this case, a duplicate has been found and the run is large enough to be placed
                    if (run.Count >= Board.MinSetSize)
                    {
                        if (numDuplicates == 1)
                        {
                            // If 1 duplicate has been found, return the run if this duplicate can be placed in its
                            // own set (the two tiles that would follow it in a run are in tiles)
                            if (Contains(tiles, new Tile(run[^1].Number() + 1, run[^1].Colour()))
                                && Contains(tiles, new Tile(run[^1].Number() + 2, run[^1].Colour())))
                            {
                                return run;
                            }
                        }
                        else if (numDuplicates == 2)
                        {
                            // If 2 duplicates have been found, return the run if this duplicate can be placed in its
                            // own set (the tile that would follow it in a run is in tiles)
                            if (Contains(tiles, new Tile(run[^1].Number() + 1, run[^1].Colour())))
                            {
                                return run;
                            }
                        }
                        else
                        {
                            // If more than 2 duplicates have been found, they can be placed in a set together, so the
                            // currently stored run can be returned
                            return run;
                        }
                    }
                
                    // Reset variables
                    foundDuplicate = false;
                    numDuplicates = 0;
                    i--;
                }
            }
            else
            {
                if (tiles[i].Colour() != run[^1].Colour())
                {
                    // If the run is sufficiently large, return it
                    if (run.Count >= Board.MinSetSize) return run;
                    
                    // Otherwise, restart the run
                    run = [tiles[i]];
                }
                else if (tiles[i].Number() == run[^1].Number())
                {
                    // If a duplicate has been found, note as such
                    foundDuplicate = true;
                    numDuplicates = 1;
                }
                else if (tiles[i].Number() == run[^1].Number() + 1)
                {
                    // If the next tile in the run has been found, add it to the run
                    run.Add(tiles[i]);
                }
                else
                {
                    // If the run is sufficiently large, return it
                    if (run.Count >= Board.MinSetSize) return run;
                    
                    // Otherwise, restart the run
                    run = [tiles[i]];
                }
            }
        }
        
        // If the run is sufficiently large, return the run, and otherwise, return null
        if (run.Count >= Board.MinSetSize) return run;
        return null;
    }

    // Pre: A list of tiles
    // Post: A list of all runs that can be made from the list of tiles
    // Desc: Return said list of runs
    private List<List<Tile>> GetAllRuns(List<Tile> tiles)
    {
        // The list of runs and the current run
        List<List<Tile>> runs = new List<List<Tile>>();

        while (true)
        {
            // If no run was found, break to return the runs
            List<Tile> run = GetRun(tiles);
            if (run == null) break;
            
            // If a run was found, add it to the list of runs and remove it from the list of tiles that can be used to
            // make a run
            runs.Add(run);
            foreach (Tile tile in run) RemoveTile(tiles, tile);
        }
        
        return runs;
    }
    
    // Pre: The potential groups that can be made, formatted as from the GetPotentialGroups function
    // Post: All combinations of said groups that can be placed together
    // Desc: Return all combinations of the input groups that can be placed at the same time
    private List<List<List<Tile>>> GenerateGroupCombinations(List<List<List<Tile>>> potentialGroups)
    {
        // Store the answer
        List<List<List<Tile>>> ans = [];
        if (potentialGroups.Count == 0) return ans;
        
        // If there is one list of potential groups, we consider the possibility where each group is placed
        // on its own
        if (potentialGroups.Count == 1)
        {
            ans.Add([]);
            foreach (List<Tile> i in potentialGroups[0]) ans.Add([i]);
            return ans;
        }
        
        // Recursively generate combinations of groups on a subset of the input that excludes one list of sets
        List<List<List<Tile>>> potentialGroupsCopy = potentialGroups.ToList();
        potentialGroupsCopy.RemoveAt(potentialGroups.Count - 1);
        List<List<List<Tile>>> smallAns = GenerateGroupCombinations(potentialGroupsCopy);

        foreach (List<List<Tile>> i in smallAns)
        {
            // Add each list of sets in the recursive solution to the final solution
            ans.Add(i);
            
            // For each set in the excluded list of sets, add it to a copy of the list of sets in the recursive
            // solution and add that appended list to the final solution
            // This is because exactly one set in the excluded list of sets can be placed, due to the preconditions
            foreach (List<Tile> j in potentialGroups[^1])
            {
                List<List<Tile>> iCopy = i.ToList();
                iCopy.Add(j);
                ans.Add(iCopy);
            }
        }

        return ans;
    }
    
    // Pre: A group of three tiles
    // Post: The one tile that can be added to that group
    // Desc: Get the missing tile from groups of three
    private Tile GetMissingTile(List<Tile> group)
    {
        // Track whether there is a given colour in the set
        bool containsRed = false;
        bool containsBlue = false;
        bool containsYellow = false;
        bool containsBlack = false;
        
        // Store every tile that has the same number as tiles in the set
        Tile redTile = new Tile(group[0].Number(), ConsoleColor.Red);
        Tile blueTile = new Tile(group[0].Number(), ConsoleColor.Blue);
        Tile yellowTile = new Tile(group[0].Number(), ConsoleColor.Yellow);
        Tile blackTile = new Tile(group[0].Number(), ConsoleColor.Black);

        // Determine whether the set contains tiles of any given colour
        foreach (Tile tile in group)
        {
            switch (tile.Colour())
            {
                case ConsoleColor.Red:
                    containsRed = true;
                    break;
                case ConsoleColor.Blue:
                    containsBlue = true;
                    break;
                case ConsoleColor.Yellow:
                    containsYellow = true;
                    break;
                case ConsoleColor.Black:
                    containsBlack = true;
                    break;
            }
        }

        // If the set does not contain a given colour, return a tile of that colour with the number of colours
        // in the group
        if (!containsRed) return redTile;
        if (!containsBlue) return blueTile;
        if (!containsYellow) return yellowTile;
        if (!containsBlack) return blackTile;
        return null;
    }

    // Pre: A list of tiles
    // Post: A set that can be made from those tiles
    // Desc: Return said set
    private List<Tile> GetSet(List<Tile> tiles)
    {
        // If a run is found, return it, and otherwise return a group
        return GetRun(tiles) ?? GetGroup(tiles);
    }

    // Pre: A list of tiles
    // Post: A group that can be made from those tiles
    // Desc: Return said group
    private List<Tile> GetGroup(List<Tile> tiles)
    {
        // Create a copy of tiles that has only distinct tiles
        List<Tile> tilesCopy = new List<Tile>();
        foreach (var tile in tiles) if (!Contains(tilesCopy, tile)) tilesCopy.Add(tile);
        
        // Sort the tiles by number
        List<Tile>[] tilesByNumber = new List<Tile>[Deck.NumNumbers];
        for (int i = 0; i < tilesByNumber.Length; i++) tilesByNumber[i] = new();

        // Sort the tiles into lists by number
        // If doing this produces a list that is a group, return that list
        foreach (var tile in tilesCopy)
        {
            tilesByNumber[tile.Number() - 1].Add(tile);
            if (tilesByNumber[tile.Number() - 1].Count >= Board.MinSetSize) return tilesByNumber[tile.Number() - 1];
        }

        // If no group was found, return null
        return null;
    }

    // Pre: The board
    // Post: A list of tuples where the first element is a tile that can be removed from a board set, and the second is
    //       what the board will look like after removing that tile
    // Desc: Return said list
    private List<(Tile, Board)> GetRemovableTiles(Board board)
    {
        // Store all removable tiles together with what the board would look like if that tile was removed
        List<(Tile, Board)> removableTiles = [];
        
        List<List<Tile>> sets = board.GetSets();

        // Iterate through each set on the board and search for tiles that can be removed
        foreach (List<Tile> set in sets)
        {
            // If the set is a group of four tiles, any tile can be removed
            // If the set is a run, tiles on the ends or in the middle (three positions from the left and right)
            // can be removed
            // Note some tiles may need to be moved around to avoid gaps between tiles in the same set
            if (Board.IsGroup(set) && set.Count == 4)
            {
                Board board1 = new Board(board);
                board1.RemoveTile(set[0].X(), set[0].Y());
                removableTiles.Add((set[0], board1));

                Board board2 = new Board(board);
                board2.RemoveTile(set[0].X(), set[0].Y());
                board2.PlaceTile(set[1].X(), set[1].Y(), new Tile(set[0]));
                removableTiles.Add((set[1], board2));

                Board board3 = new Board(board);
                board3.RemoveTile(set[3].X(), set[3].Y());
                board3.PlaceTile(set[2].X(), set[2].Y(), new Tile(set[3]));
                removableTiles.Add((set[2], board3));
                
                Board board4 = new Board(board);
                board4.RemoveTile(set[3].X(), set[3].Y());
                removableTiles.Add((set[3], board4));
            }
            else
            {
                for (int i = Board.MinSetSize; i + Board.MinSetSize < set.Count; i++)
                {
                    Board board1 = new Board(board);
                    board1.RemoveTile(set[i].X(), set[i].Y());
                    removableTiles.Add((set[i], board1));
                }
                
                if (set.Count > Board.MinSetSize)
                {
                    Board board1 = new Board(board);
                    board1.RemoveTile(set[0].X(), set[0].Y());
                    removableTiles.Add((set[0], board1));
                    
                    Board board2 = new Board(board);
                    board2.RemoveTile(set[^1].X(), set[^1].Y());
                    removableTiles.Add((set[^1], board2));
                }
            }
        }
        
        return removableTiles;
    }

    // Pre: The board
    // Post: A list of tuples where the first element is a list of tiles that can be removed from board sets, and the
    //       second is what the board will look like after removing those tiles
    // Desc: Return said list
    private List<(List<Tile>, Board)> GetRemovableTileCombinations(Board board)
    {
        // Store the solution
        List<(List<Tile>, Board)> removableTileCombinations = [];
        
        // Store all removable tiles together with what the board would look like if that tile was removed
        List<(Tile, Board)> removableTiles = GetRemovableTiles(board);
        foreach (var i in removableTiles) removableTileCombinations.Add(([i.Item1], i.Item2));
        
        for (int i = 0; i < removableTiles.Count; i++)
        {
            // For each tile T that can be removed from the board, find what tiles can be removed after T
            Board newBoard = removableTiles[i].Item2;
            List<(Tile, Board)> removableTiles2 = GetRemovableTiles(newBoard);

            // Add all pairs of tiles that can be removed together to the solution
            foreach (var j in removableTiles2)
            {
                removableTileCombinations.Add(([removableTiles[i].Item1, j.Item1], j.Item2));
            }
        }
        
        return removableTileCombinations;
    }
    
    // Pre: A list of tiles and a tile
    // Post: A bool representing whether tile is in tiles
    // Desc: Remove a tile from a list of tiles
    private bool RemoveTile(List<Tile> tiles, Tile tile)
    {
        // Search for the tile to remove and return true if it is found
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
    
    // Pre: A rectangle
    // Post: A bool representing whether that rectangle was clicked
    // Desc: Return said bool
    private bool RectangleClicked(Rectangle rect)
    {
        MouseState mouseState = Mouse.GetState();
        return mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released
                                                            && rect.Contains(mouseState.Position);
    }
    
    // Pre: A list of tiles
    // Post: A list of tiles, now sorted by colour then rank
    // Desc: Return a list of tiles sorted by colour then number
    private List<Tile> SortColourThenRank(List<Tile> tiles)
    {
        // Store the tiles
        TileCollection tileCollection = new();
        foreach (Tile tile in tiles) tileCollection.Add(tile);
        
        // TileCollection objects will always return tiles sorted by colour then number, which is why this works
        return tileCollection.GetTiles();
    }

    // Pre: A list of tiles and a tile
    // Post: A bool representing whether tile is in tiles
    // Desc: Return said bool
    private bool Contains(List<Tile> tiles, Tile tile)
    {
        // Return true if the tile is found in the collection
        foreach (Tile tile1 in tiles) if (tile1.IsEqual(tile)) return true;
        
        // Return false otherwise
        return false;
    }

    // Pre: A list of tiles and a tile
    // Post: A bool representing whether tile is in tiles twice
    // Desc: Return said bool
    private bool ContainsBoth(List<Tile> tiles, Tile tile)
    {
        // The number of times the tile is found in the collection
        int numTile = 0;

        // Store the number of times the tile is found in the collection
        foreach (Tile tile1 in tiles)
        {
            if (tile1.IsEqual(tile))
            {
                numTile++;
            }
        }
        
        // Return true iff the tile is found in the collection twice
        return numTile == 2;
    }
}