using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EarthSimulator.MapEditor
{
    public class MapData
    {
        private static Color mapColor = Color.red;
        private static Color changeColor = Color.blue;
        private static Texture2D map = null;
        private static Texture2D colorMap = null;
        private static RawImage image;

        public static bool IsForceContinue = false;
        public static float BoarderGenerateSpeed = 10;
        public static float MapGenerateSpeed = 1;

        private static GameObject baseGameObject;

        public static Color MapColor
        {
            get
            {
                return mapColor;
            }
            set
            {
                mapColor = value;
            }
        }

        public static Color ChangeColor
        {
            get
            {
                return changeColor;
            }
            set
            {
                changeColor = value;
            }
        }

        public static Color GetMapColor(int x, int y)
        {
            return map.GetPixel(x - 1, y);
        }

        public static Texture2D Map
        {
            get
            {
                return map;
            }
            set
            {
                if (map == null || value == null)
                    map = value;
            }
        }

        public static Texture2D ColorMap
        {
            get
            {
                return colorMap;
            }
            set
            {
                if (colorMap == null || value == null)
                    colorMap = value;
            }
        }

        public static RawImage Image
        {
            get
            {
                return image;
            }
            set
            {
                if (image == null || value == null)
                    image = value;
            }
        }

        public static GameObject BaseGameObject
        {
            get
            {
                if (baseGameObject == null)
                {
                    baseGameObject = new GameObject();
                    baseGameObject.name = "BaseGameObject";
                }
                return baseGameObject;
            }
        }

        public static bool ColorNear(Color color1, Color color2, float delta = 0)
        {
            if (Mathf.Abs(color1.r - color2.r) + Mathf.Abs(color1.g - color2.g) + Mathf.Abs(color1.b - color2.b) <= delta)
                return true;
            else
                return false;
        }
    }
}
