using System;
using GTA.Math;

namespace Astros2
{
    public static class Utils
    {
        public static double RadToDeg(double rad)
        {
            return rad * 180.0 / Math.PI;
        }

        public static Vector3 InverseTransformDirection(Quaternion rotation, Vector3 vector)
        {

            Vector3 output = Quaternion.Invert(rotation) * vector;
            return output;
        }

        public static Vector3 DirectionToRotation(Vector3 direction)
        {
            direction.Normalize();

            var x = Math.Atan2(direction.Z, Math.Sqrt(direction.Y * direction.Y + direction.X * direction.X));
            var y = 0;
            var z = -Math.Atan2(direction.X, direction.Y);

            return new Vector3
            {
                X = (float)RadToDeg(x),
                Y = (float)RadToDeg(y),
                Z = (float)RadToDeg(z)
            };
        }

        public static Vector3 RightVector(this Vector3 position, Vector3 up)
        {
            position.Normalize();
            up.Normalize();
            return Vector3.Cross(position, up);
        }

        public static Vector3 LeftVector(this Vector3 position, Vector3 up)
        {
            position.Normalize();
            up.Normalize();
            return -(Vector3.Cross(position, up));
        }
    }
}
