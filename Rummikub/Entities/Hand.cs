// Author: Jacob Lisogurski
// File Name: Hand.cs
// Project Name: Rummikub
// Creation Date: June 11, 2025
// Modified Date: July 26, 2025
// Description: The player's hand.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rummikub.Entities;

public class Hand
{
    // Note this information is only relevant for the player's hand, as the computer's hand is not shown to the user
    private const int TileSpacing = 5;
    private const int InitialSize = 14;

    private int _x = 145;
    private readonly int _y = 730;
    private readonly List<Tile> _hand = [];

    // Pre: None
    // Post: A hand
    // Desc: Constructor for the hand
    public Hand()
    {
        
    }

    // Pre: A hand
    // Post: A copy of the given hand
    // Desc: Creates a copy of a hand
    public Hand(Hand hand)
    {
        var tiles = hand.GetTiles();

        foreach (var tile in tiles)
        {
            _hand.Add(new Tile(tile));
        }
    }
    
    public Hand(Hand hand, int x)
    {
        var tiles = hand.GetTiles();
        _x = x;

        foreach (var tile in tiles)
        {
            Add(new Tile(tile));
        }
    }

    // Pre: None
    // Post: A list of the tiles in the hand
    // Desc: Getter for the hand
    public List<Tile> GetTiles()
    {
        return _hand.ToList();
    }

    // Pre: None
    // Post: The number of tiles in the hand
    // Desc: Get the size of the hand
    public int Count()
    {
        return _hand.Count;
    }

    // Pre: None
    // Post: Whether the hand is empty
    // Desc: Return whether the hand is empty
    public bool IsEmpty()
    {
        return Count() == 0;
    }
    
    // Pre: The previous mouse state
    // Post: The clicked tile
    // Desc: If a tile has been clicked, returns said tile
    public (Tile, int) GetClickedTile(MouseState previousState)
    {
        MouseState mouseState = Mouse.GetState();
        
        if (mouseState.LeftButton != ButtonState.Pressed || previousState.LeftButton == ButtonState.Pressed)
        {
            return (null, -1);
        }

        for (int i = 0; i < _hand.Count; i++) if (_hand[i].IsCursorOnTile(mouseState)) return (_hand[i], i);
        return (null, -1);
    }
    
    // Pre: A tile
    // Post: None
    // Desc: Adds a tile to the hand and positions it on the screen
    public void Add(Tile tile)
    {
        Rectangle rect;

        if (_hand.Count == 0)
        {
            rect = new Rectangle(_x, _y, Tile.Width, Tile.Height);
        }
        else
        {
            var temp = _hand[^1].GetDestinationRectangle();
            rect = new Rectangle(temp.X + TileSpacing + Tile.Width, _y, Tile.Width, Tile.Height);
        }

        _hand.Add(tile);
        tile.SetDestinationRectangle(rect);
    }

    // Pre: A tile to add to the hand
    // Post: None
    // Desc: Add a tile to the hand without worrying about the location of the tile on the screen
    public void TempAdd(Tile tile)
    {
        _hand.Add(tile);
    }

    // Pre: The tile to be removed
    // Post: None
    // Desc: Removes a tile and adjusts the position of all other tiles
    public void RemoveTile(Tile tile)
    {
        foreach (var handTile in _hand)
        {
            if (!handTile.IsEqual(tile)) continue;
            _hand.Remove(handTile);
            break;
        }
        
        for (var i = 0; i < _hand.Count; i++)
        {
            var destRect = new Rectangle(_x + i * (Tile.Width + TileSpacing), _y, Tile.Width, Tile.Height);
            _hand[i].SetDestinationRectangle(destRect);
        }
    }

    // Pre: An index to remove from
    // Post: None
    // Desc: Remove a tile from the hand by index, and adjust the positions of all tiles
    public void RemoveAt(int index)
    {
        _hand.RemoveAt(index);
        
        for (var i = 0; i < _hand.Count; i++)
        {
            var destRect = new Rectangle(_x + i * (Tile.Width + TileSpacing), _y, Tile.Width, Tile.Height);
            _hand[i].SetDestinationRectangle(destRect);
        }
    }
    
    // Pre: A deck, which must have at least 14 tiles
    // Post: None
    // Desc: Draws 14 cards to the player's hand to start the game
    public void Populate(Deck deck)
    {
        for (var i = 0; i < InitialSize; i++)
        {
            var temp = deck.Pop();
            Add(temp);
        }
    }
    
    // Pre: The spritebatch and the image of all the tiles
    // Post: None
    // Desc: Draws all cards in the hand to the screen
    public void Draw(SpriteBatch spriteBatch, Texture2D img)
    {
        foreach (var tile in _hand)
        {
            tile.Draw(spriteBatch, img);
        }
    }
    
    // Pre: None
    // Post: None
    // Desc: Moves the hand right
    public void MoveRight()
    {
        foreach (var tile in _hand) tile.MoveRight();
        _x += 2;
    }

    // Pre: None
    // Post: None
    // Desc: Moves the hand left
    public void MoveLeft()
    {
        foreach (var tile in _hand) tile.MoveLeft();
        _x -= 2;
    }

    // Pre: None
    // Post: The x-coordinate of this tile on the screen
    // Desc: Returns this tile's x-coordinate
    public int X()
    {
        return _x;
    }

    // Pre: None
    // Post: None
    // Desc: Sorts the hand by colour then number
    public void SortColourThenNumber()
    {
        // Sorts the hand by moving all tiles to a TileCollection, then moving the tiles back to the hand
        // This works because TileCollections are already sorted as needed
        TileCollection tiles = new();
        foreach (var tile in _hand) tiles.Add(tile);
        
        List<Tile> temp = tiles.GetTiles();
        
        _hand.Clear();
        foreach (var tile in temp) Add(tile);
    }

    // Pre: None
    // Post: None
    // Desc: Sorts the hand by number then color
    public void SortNumberThenColour()
    {
        // Sorts the hand by moving all tiles to a different TileCollection based on the tile's number,
        // then moving all tiles back to the hand in order of number
        // This works because TileCollections are already by colour then number, so adding tiles to 
        // TileCollections based on number sorts the tiles by number, then the TileCollection sorts by colour
        TileCollection[] tilesByNumber = new TileCollection[Deck.NumNumbers];
        List<Tile> sortedTiles = [];

        for (int i = 0; i < tilesByNumber.Length; i++) tilesByNumber[i] = new();
        foreach (Tile tile in _hand) tilesByNumber[tile.Number() - 1].Add(tile);

        foreach (var tiles in tilesByNumber) sortedTiles.AddRange(tiles.GetTiles());
        
        _hand.Clear();
        foreach (var tile in sortedTiles) Add(tile);
    }

    // Pre: The tile to insert and the previous mouse state
    // Post: A boolean representing whether a tile was successfully inserted
    // Desc: Inserts a tile in the hand
    public bool Insert(Tile tile, MouseState previousState)
    {
        // Store whether the mouse was clicked
        MouseState currentState = Mouse.GetState();
        bool a = currentState.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released;
        
        // Store whether the mouse is low enough on the screen to click the hand
        bool b = currentState.Y > _y - 10;

        // If the hand is empty, add a tile iff the mouse clicked low enough on the screen
        if (IsEmpty())
        {
            if (a && b) Add(tile);
            return a && b;
        }
        
        // If the mouse clicked to the left of the hand, add the tile to the start of the hand
        if (a && b && Mouse.GetState().X < _hand[0].GetDestinationRectangle().X + Tile.Width / 2)
        {
            List<Tile> handCopy = _hand.ToList();
            _hand.Clear();
            
            Add(tile);
            foreach (Tile t in handCopy) Add(t);
            return true;
        }
        
        // If the mouse clicked to the right of the hand, add the tile to the end of the hand
        if (a && b && Mouse.GetState().X > _hand[^1].GetDestinationRectangle().X + Tile.Width / 2)
        {
            Add(tile);
            return true;
        }
        
        // For each adjacent pair of tiles, check if the mouse has clicked between them
        // If it has, insert the tile between those two tiles
        for (int i = 0; i < _hand.Count - 1; i++)
        {
            bool c = currentState.X > _hand[i].GetDestinationRectangle().X + Tile.Width / 2;
            bool d = currentState.X < _hand[i + 1].GetDestinationRectangle().X + Tile.Width / 2;

            if (a && b && c && d)
            {
                List<Tile> handCopy = [];

                for (int j = 0; j <= i; j++)
                {
                    handCopy.Add(_hand[j]);
                }
                
                handCopy.Add(tile);

                for (int j = i + 1; j < _hand.Count; j++)
                {
                    handCopy.Add(_hand[j]);
                }

                _hand.Clear();
                foreach (Tile tile2 in handCopy) Add(tile2);
                return true;
            }
        }
        
        return false;
    }
}