using UnityEngine;


namespace Assets.Scripts
{
    [System.Serializable]
    public class Piece
    {
        public int OriginalI { get; set; }
        public int OriginalJ { get; set; }

        public int CurrentI { get; set; }
        public int CurrentJ { get; set; }

        public GameObject GameObject { get; set; }
    }
}