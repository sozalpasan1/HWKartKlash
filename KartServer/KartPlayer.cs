using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace KartServer
{
    public class KartPlayer
    {
        public string Id { get; private set; }
        public TcpClient TcpClient { get; private set; }
        
        // State
        public float PositionX { get; set; } = 0f;
        public float PositionY { get; set; } = 0f;
        public float PositionZ { get; set; } = 0f;
        public float RotationY { get; set; } = 0f;
        public float Speed { get; set; } = 0f;
        
        // Input state
        private Dictionary<string, int> inputState = new Dictionary<string, int>();
        
        // Constants for physics
        private const float MAX_SPEED = 15f;
        private const float ACCELERATION = 0.2f;
        private const float DECELERATION = 0.1f;
        private const float TURN_SPEED = 2.5f;
        
        public KartPlayer(string id, TcpClient client)
        {
            Id = id;
            TcpClient = client;
            
            // Initialize input states
            inputState["W"] = 0; // Forward
            inputState["S"] = 0; // Backward
            inputState["A"] = 0; // Left
            inputState["D"] = 0; // Right
            inputState["Space"] = 0; // Brake
        }
        
        public void UpdateInput(string key, int value)
        {
            if (inputState.ContainsKey(key))
            {
                inputState[key] = value;
            }
        }
        
        public void UpdatePhysics(float deltaTime)
        {
            // Handle acceleration and deceleration
            if (inputState["W"] > 0)
            {
                // Accelerate forward
                Speed += ACCELERATION * deltaTime;
                if (Speed > MAX_SPEED) Speed = MAX_SPEED;
            }
            else if (inputState["S"] > 0)
            {
                // Accelerate backward
                Speed -= ACCELERATION * deltaTime;
                if (Speed < -MAX_SPEED / 2) Speed = -MAX_SPEED / 2; // Slower in reverse
            }
            else
            {
                // Natural deceleration
                if (Speed > 0)
                {
                    Speed -= DECELERATION * deltaTime;
                    if (Speed < 0) Speed = 0;
                }
                else if (Speed < 0)
                {
                    Speed += DECELERATION * deltaTime;
                    if (Speed > 0) Speed = 0;
                }
            }
            
            // Apply braking
            if (inputState["Space"] > 0)
            {
                Speed *= 0.95f; // Quick deceleration
            }
            
            // Handle turning
            if (inputState["A"] > 0)
            {
                // Turn left
                RotationY += TURN_SPEED * deltaTime * (Speed != 0 ? 1 : 0);
            }
            if (inputState["D"] > 0)
            {
                // Turn right
                RotationY -= TURN_SPEED * deltaTime * (Speed != 0 ? 1 : 0);
            }
            
            // Normalize rotation
            while (RotationY > 360) RotationY -= 360;
            while (RotationY < 0) RotationY += 360;
            
            // Calculate new position based on speed and rotation
            float radians = RotationY * (float)Math.PI / 180f;
            PositionX += (float)Math.Sin(radians) * Speed * deltaTime;
            PositionZ += (float)Math.Cos(radians) * Speed * deltaTime;
        }
        
        public string GetStateMessage()
        {
            return $"STATE:{Id}:{PositionX}:{PositionY}:{PositionZ}:{RotationY}:{Speed}";
        }
    }
}