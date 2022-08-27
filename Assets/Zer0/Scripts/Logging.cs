using System;
using UnityEngine;

namespace Zer0
{
    public static class Logging
    {
        public static void LogMessage(errorLevel errorLevel, Color messageColor, string message)
        {
            var color = messageColor;
            
            switch (errorLevel)
            {
                case errorLevel.Log:
                    Debug.Log(
                        $"<color=#{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}>{message}</color>");
                    break;
                case errorLevel.Warning:
                    color = Color.yellow;
                    Debug.LogWarning(
                        $"<color=#{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}>{message}</color>");
                    break;
                case errorLevel.Error:
                    color = Color.red;
                    Debug.LogError(
                        $"<color=#{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}>{message}</color>");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorLevel), errorLevel, null);
            }
        }
    }
}
