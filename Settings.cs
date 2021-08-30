using System;

namespace CustomKart
{
    [Serializable]
    public class Settings
    {
        public int TileSize;
        
        public string BackElement;
        
        public string MainBody;
        
        public string Pipes;
        
        public string Seat;
        
        public string Sprites;
        
        public string Wheel;

        public Settings()
        {
            TileSize = 256;
            
            BackElement = "BackElement.obj";

            MainBody = "MainBody.obj";
            
            Pipes = "Pipes.obj";
            
            Seat = "Seat.obj";
            
            Sprites = "Sprites.png";
            
            Wheel = "Wheel.obj";
        }
    }
}
