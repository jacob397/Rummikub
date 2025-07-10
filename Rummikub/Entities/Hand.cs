// Author: Jacob Lisogurski
// File Name: Hand.cs
// Project Name: Rummikub
// Creation Date: June 11, 2025
// Modified Date:
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

    private int _x = 15;
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
    
    // Pre: The previous mouse state
    // Post: The clicked tile
    // Desc: If a tile has been clicked, returns said tile
    public Tile GetClickedTile(MouseState previousState)
    {
        if (Mouse.GetState().LeftButton != ButtonState.Pressed || previousState.LeftButton == ButtonState.Pressed)
        {
            return null;
        }

        foreach (var tile in _hand)
        {
            if (tile.IsCursorOnTile(Mouse.GetState()))
            {
                return tile;
            }
        }

        return null;
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
        foreach (var tile in _hand)
        {
            tile.MoveRight();
        }

        _x += 2;
    }

    // Pre: None
    // Post: None
    // Desc: Moves the hand left
    public void MoveLeft()
    {
        foreach (var tile in _hand)
        {
            tile.MoveLeft();
        }
        
        _x -= 2;
    }
}