using UnityEngine;

namespace Anura.ConfigurationModule.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Config", menuName = "Configurations/Config", order = 1)]
    public class Config : ScriptableObject
    {
        [Header("Match Configuration")]
        [Space]

        [SerializeField]
        private int maxNumberOfRounds = 15;
        [SerializeField]
        private int turnDurationInSeconds = 30;

        [SerializeField]
        private Color team1Color = Color.red;
        [SerializeField]
        private Color team2Color = Color.blue;

        [Header("Player configurations")]
        [Space]

        [SerializeField]
        private int playerTotalHealth = 100;

        [SerializeField, Tooltip("Whether or not a player can steer while jumping")]
        private bool airControl = false;

        [SerializeField] private float playerSpeed;
        
        [SerializeField, Tooltip("Amount of force added when the player jumps.")]
        private float playerJumpForce;
        
        [Range(0, .3f), SerializeField, Tooltip("How much to smooth out the movement")] 
        private float movementSmoothing = .05f;

        [Header("Indicator configurations")]
        [Space]

        [SerializeField] private float indicatorSpeed;
        [SerializeField] private Vector2 bulletSpeed;
        [SerializeField, Min(0.3f)] private Vector2 pressTimer;
        [SerializeField] private float bufferMaxTimer;
        [SerializeField] private float factorRotationRocket;


        public int GetMaxNumberOfRounds()
        {
            return maxNumberOfRounds;
        }

        public Color GetFirstTeamColor()
        {
            return team1Color;
        }

        public Color GetSecondTeamColor()
        {
            return team2Color;
        }

        public int GetTurnDurationInSeconds()
        {
            return turnDurationInSeconds;
        }

        public int GetPlayerTotalHealth()
        {
            return playerTotalHealth;
        }


        public bool GetAirControl()
        {
            return airControl;
        }
     
        public float GetPlayerSpeed()
        {
            return playerSpeed;
        }

        public float GetPlayerJumpForce()
        {
            return playerJumpForce;
        }

        public float GetMovementSmoothing()
        {
            return movementSmoothing;
        }


        //indicator

        public float GetIndicatorSpeed()
        {
            return indicatorSpeed;
        }

        public float GetBulletSpeed(float multiplier)
        {
            return multiplier * GetSpeedPerSecond();
        }

        public Vector2 GetPressTimer()
        {
            return pressTimer;
        }

        public float GetBufferMaxTimer()
        {
            return bufferMaxTimer;
        }

        public float GetValidIndicatorTime()
        {
            return GetPressTimer().y + GetBufferMaxTimer();
        }

        public float GetFactorRotationIndicator()
        {
            return factorRotationRocket;
        }

        private float GetSpeedPerSecond()
        {
            return bulletSpeed.y / GetPressTimer().y;
        }
    }
}
