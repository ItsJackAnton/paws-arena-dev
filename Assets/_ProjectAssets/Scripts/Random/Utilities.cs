using UnityEngine;

public static class Utilities
{
    public static Sprite ConvertTextureToProfileSprite(Texture2D _texture,Vector2Int _startCoordinates,int _squareSize)
    {
        if (_startCoordinates.x < 0 || _startCoordinates.y < 0 || 
            _startCoordinates.x + _squareSize > _texture.width || 
            _startCoordinates.y + _squareSize > _texture.height)
        {
            Debug.LogError("Requested area is out of the texture bounds.");
            return null;
        }

        Rect _rect = new Rect(_startCoordinates.x, _startCoordinates.y, _squareSize, _squareSize);
        Vector2 _pivot = new Vector2(0.5f, 0.5f); // Pivot at the center of the square

        Sprite _sprite = Sprite.Create(_texture, _rect, _pivot);

        return _sprite;
    }
    
    public static Sprite TextureToSprite(Texture2D _texture)
    {
        return Sprite.Create(
            _texture,
            new Rect(0, 0, _texture.width, _texture.height), 
            new Vector2(0.5f, 0.5f)
        );
    }
}
