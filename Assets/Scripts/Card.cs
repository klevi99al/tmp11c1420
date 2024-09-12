using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class Card : ScriptableObject
{
    public int id;
    public bool isMatched;            
    public Sprite cardFront;          
    public Sprite cardBack;           
}
