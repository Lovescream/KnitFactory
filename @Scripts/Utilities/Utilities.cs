using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Utilities
{
    #region Generals

    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        if (!obj.TryGetComponent<T>(out T component)) component = obj.AddComponent<T>();
        return component;
    }

    public static T FindChild<T>(GameObject obj, string name = null) where T : Component
    {
        if (obj == null) return null;
        T[] components = obj.GetComponentsInChildren<T>(true);
        if (components.Length == 0) return null;
        if (string.IsNullOrEmpty(name)) return components[0];
        return components.Where(x => x.name == name).FirstOrDefault();
    }

    public static T FindChildDirect<T>(GameObject obj, string name = null) where T : Component
    {
        if (obj == null) return null;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform t = obj.transform.GetChild(i);
            if (string.IsNullOrEmpty(name) || t.name == name)
                if (t.TryGetComponent(out T component))
                    return component;
        }

        return null;
    }

    public static GameObject FindChild(GameObject obj, string name = null)
    {
        Transform transform = FindChild<Transform>(obj, name);
        if (transform == null) return null;
        return transform.gameObject;
    }

    public static GameObject FindChildDirect(GameObject obj, string name = null)
    {
        Transform transform = FindChildDirect<Transform>(obj, name);
        if (transform == null) return null;
        return transform.gameObject;
    }

    public static void DestroyAllChildren(this Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(t.GetChild(i).gameObject);
        }
    }

    #endregion

    #region Colors

    public static Color GetColor(this ColorType type)
    {
        Color color;
        switch (type)
        {
            case ColorType.White: return ColorUtility.TryParseHtmlString("#F9FEFE", out color) ? color : Color.white;
            case ColorType.Red: return ColorUtility.TryParseHtmlString("#F66E75", out color) ? color : Color.white;
            case ColorType.Yellow: return ColorUtility.TryParseHtmlString("#FCF046", out color) ? color : Color.white;
            case ColorType.Blue: return ColorUtility.TryParseHtmlString("#5354BA", out color) ? color : Color.white;
            case ColorType.Orange: return ColorUtility.TryParseHtmlString("#F79D37", out color) ? color : Color.white;
            case ColorType.Purple: return ColorUtility.TryParseHtmlString("#D78BFB", out color) ? color : Color.white;
            case ColorType.Pink: return ColorUtility.TryParseHtmlString("#F77EC7", out color) ? color : Color.white;
            case ColorType.Mint: return ColorUtility.TryParseHtmlString("#596969", out color) ? color : Color.white;
            case ColorType.Sky: return ColorUtility.TryParseHtmlString("#51A7E3", out color) ? color : Color.white;
            case ColorType.Lime: return ColorUtility.TryParseHtmlString("#BAED13", out color) ? color : Color.white;
            default: return ColorUtility.TryParseHtmlString("#FFFFFF", out color) ? color : Color.white;
        }
    }

    public static Color GetColor(this Difficulty difficulty)
    {
        Color color;
        switch (difficulty)
        {
            case Difficulty.Normal: return ColorUtility.TryParseHtmlString("#AEEEFF", out color) ? color : Color.white;
            case Difficulty.Hard: return ColorUtility.TryParseHtmlString("#CFA7E3", out color) ? color : Color.white;
            case Difficulty.SuperHard:
                return ColorUtility.TryParseHtmlString("#F1B0CB", out color) ? color : Color.white;
            default: return ColorUtility.TryParseHtmlString("#AEEEFF", out color) ? color : Color.white;
        }
    }

    public static Color GetTextColor(this Difficulty difficulty)
    {
        Color color;
        switch (difficulty)
        {
            case Difficulty.Normal: return ColorUtility.TryParseHtmlString("#1D5995", out color) ? color : Color.white;
            case Difficulty.Hard: return ColorUtility.TryParseHtmlString("#BA6E79", out color) ? color : Color.white;
            case Difficulty.SuperHard:
                return ColorUtility.TryParseHtmlString("#C6B179", out color) ? color : Color.white;
            default: return ColorUtility.TryParseHtmlString("#1D5995", out color) ? color : Color.white;
        }
    }

    #endregion

    #region Math

    public static byte RotateShift(this byte value, int count)
    {
        return (byte)((value << count) | (value >> (8 - count)));
    }

    public static string GetFormattedCurrency(this int value)
    {
        if (value < 1000) return value.ToString();

        if (value < 1000000)
        {
            double v = (double)value / 1000;
            if (v >= 100) return $"{Math.Floor(v)}K";
            return v >= 10 ? $"{Math.Floor(v * 10) / 10:0.0}K" : $"{Math.Floor(v * 100) / 100:0.00}K";
        }

        if (value < 1000000000)
        {
            double v = (double)value / 1000000;
            if (v >= 100) return $"{Math.Floor(v)}M";
            return v >= 10 ? $"{Math.Floor(v * 10) / 10:0.0}M" : $"{Math.Floor(v * 100) / 100:0.00}M";
        }

        return string.Empty;
    }

    #endregion

    #region Vector

    public static Vector3 SetX(this Vector3 vector, float x)
    {
        return new(x, vector.y, vector.z);
    }

    public static Vector3 SetY(this Vector3 vector, float y)
    {
        return new(vector.x, y, vector.z);
    }

    public static Vector3 SetZ(this Vector3 vector, float z)
    {
        return new(vector.x, vector.y, z);
    }

    public static void SetPositionX(this Transform transform, float x)
    {
        Vector3 position = transform.position;
        position.x = x;
        transform.position = position;
    }

    public static void SetPositionY(this Transform transform, float y)
    {
        Vector3 position = transform.position;
        position.y = y;
        transform.position = position;
    }

    public static void SetPositionZ(this Transform transform, float z)
    {
        Vector3 position = transform.position;
        position.z = z;
        transform.position = position;
    }

    public static Vector2 GetCenter(this List<Vector2> vectors)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        foreach (Vector2 v in vectors)
        {
            if (v.x < minX) minX = v.x;
            if (v.x > maxX) maxX = v.x;
            if (v.y < minY) minY = v.y;
            if (v.y > maxY) maxY = v.y;
        }

        return new((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
    }

    public static Vector3 GetCenter(this List<Vector3> vectors)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        foreach (Vector3 v in vectors)
        {
            if (v.x < minX) minX = v.x;
            if (v.x > maxX) maxX = v.x;
            if (v.y < minY) minY = v.y;
            if (v.y > maxY) maxY = v.y;
            if (v.z < minZ) minZ = v.z;
            if (v.z > maxZ) maxZ = v.z;
        }

        return new((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, (minZ + maxZ) * 0.5f);
    }

    // 좌표 리스트를 원점(최조 좌표)을 기준으로 정규화.
    public static void Normalize(this List<Vector2Int> points)
    {
        int minX = points.Min(p => p.x);
        int minY = points.Min(p => p.y);
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = new(points[i].x - minX, points[i].y - minY);
        }
    }

    public static Vector2 GetRandomPointInEllipse(float xFactor, float yFactor)
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        float RandomRadius = Mathf.Sqrt(Random.value);

        float x = RandomRadius * Mathf.Cos(randomAngle) * xFactor;
        float y = RandomRadius * Mathf.Sin(randomAngle) * yFactor;

        return new Vector2(x, y);
    }
    
    //방향벡터에 따라 회전값을 결정하는 함수
    public static float GetTargetRotationZ(Vector2 directionVector)
    {
        Vector2 dir = directionVector.normalized;
        
        float signedAngle = Vector2.SignedAngle(Vector2.up, dir);
        float angleFromUp = Mathf.Abs(signedAngle);

        if (angleFromUp > 90f) angleFromUp = 180f - angleFromUp;

        const float maxRotation = 45f;
        float t = 1f - Mathf.Abs(angleFromUp - 45f) / 45f; 
        t = Mathf.Clamp01(t);
        float magnitude = maxRotation * t;

        float sign = -Mathf.Sign(dir.x);

        float targetRotationZ = magnitude * sign;

        return targetRotationZ;
    }

    #endregion

    #region Direction

    public static Direction GetDirection(this Vector3 from, Vector3 to)
    {
        const float e = 0.1f;

        float dx = to.x - from.x;
        float dy = to.y - from.y;

        if (Mathf.Abs(dx) >= Mathf.Abs(dy))
        {
            if (dx > e) return Direction.Right;
            if (dx < -e) return Direction.Left;
        }
        else
        {
            if (dy > e) return Direction.Top;
            if (dy < -e) return Direction.Bottom;
        }

        return Direction.None;
    }

    public static Vector2Int GetIndex(this Direction direction)
    {
        return direction switch
        {
            Direction.Top => new(0, 1),
            Direction.Right => new(1, 0),
            Direction.Bottom => new(0, -1),
            Direction.Left => new(-1, 0),
            _ => Vector2Int.zero
        };
    }

    public static Vector2Int GetIndex(this Direction direction, Direction additionalDirection)
    {
        return direction.GetIndex() + additionalDirection.GetIndex();
    }

    public static Vector3 GetRotation(this Direction direction)
    {
        return direction switch
        {
            Direction.Top => new(0, 180, 0),
            Direction.Right => new(0, 270, 0),
            Direction.Bottom => new(0, 0, 0),
            Direction.Left => new(0, 90, 0),
            _ => Vector3.zero
        };
    }

    public static Direction GetOpposite(this Direction direction)
    {
        return direction switch
        {
            Direction.Top => Direction.Bottom,
            Direction.Right => Direction.Left,
            Direction.Bottom => Direction.Top,
            Direction.Left => Direction.Right,
            _ => Direction.None
        };
    }

    public static Direction GetCounterClockwise(this Direction direction)
    {
        return direction switch
        {
            Direction.Top => Direction.Left,
            Direction.Left => Direction.Bottom,
            Direction.Bottom => Direction.Right,
            Direction.Right => Direction.Top,
            _ => Direction.None
        };
    }

    public static Direction GetClockwise(this Direction direction)
    {
        return direction switch
        {
            Direction.Top => Direction.Right,
            Direction.Right => Direction.Bottom,
            Direction.Bottom => Direction.Left,
            Direction.Left => Direction.Top,
            _ => Direction.None
        };
    }

    public static Orientation Flip(this Orientation orientation)
    {
        return orientation switch
        {
            Orientation.Horizontal => Orientation.Vertical,
            Orientation.Vertical => Orientation.Horizontal,
            _ => Orientation.None
        };
    }

    public static Vector2Int DirectionVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Top => new(0, 1),
            Direction.Bottom => new(0, -1),
            Direction.Left => new(-1, 0),
            Direction.Right => new(1, 0),
            _ => Vector2Int.zero
        };
    }

    public static Vector2Int WallDirectionVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Top => new(-1, 0),
            Direction.Bottom => new(1, 0),
            Direction.Left => new(0, -1),
            Direction.Right => new(0, 1),
            _ => Vector2Int.zero
        };
    }

    public static int ToMask(this IEnumerable<Direction> directions)
    {
        int result = 0;
        foreach (Direction direction in directions)
        {
            switch (direction)
            {
                case Direction.Top: result |= 1; break;
                case Direction.Right: result |= 2; break;
                case Direction.Bottom: result |= 4; break;
                case Direction.Left: result |= 8; break;
            }
        }

        return result;
    }

    #endregion
}

public class Vector2IntDictionaryConverter : JsonConverter<Dictionary<Vector2Int, int>>
{
    public override Dictionary<Vector2Int, int> ReadJson(JsonReader reader, Type objectType,
        Dictionary<Vector2Int, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        Dictionary<Vector2Int, int> dictionary = new(jObject.Count);

        foreach (JProperty prop in jObject.Properties())
        {
            string s = prop.Name.Trim('(', ')');
            string[] parts = s.Split(',');
            if (parts.Length != 2) throw new JsonSerializationException($"Invalid Vector2Int key format: {prop.Name}");

            if (!int.TryParse(parts[0], out int x) || !int.TryParse(parts[1], out int y))
                throw new JsonSerializationException($"Cannot parse Vector2Int components from \"{prop.Name}\"");

            int value = prop.Value.ToObject<int>();
            dictionary.Add(new Vector2Int(x, y), value);
        }

        return dictionary;
    }

    public override void WriteJson(JsonWriter writer, Dictionary<Vector2Int, int> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        foreach (KeyValuePair<Vector2Int, int> kv in value)
        {
            writer.WritePropertyName($"({kv.Key.x}, {kv.Key.y})");
            writer.WriteValue(kv.Value);
        }

        writer.WriteEndObject();
    }
}
    
public static class WorldUIScaler
{
    private static readonly Vector3 ReferenceScale = Vector3.one;

    public static Vector3 ApplyScale(this Camera cam)
    {
        float ratio = cam.orthographicSize / ScreenManager.ReferenceCameraSize * Main.Screen.FinalScaleRatio;
        return ReferenceScale * ratio;
    }

    public static float ApplyRatio(this Camera cam)
    {
        float ratio = cam.orthographicSize / ScreenManager.ReferenceCameraSize * Main.Screen.FinalScaleRatio;
        return ratio;
    }
}

    public static class Util
    {
        public static void DebugLog(string message)
        {
#if UNITY_EDITOR || ACTIONFIT_DEBUG
            Debug.Log(message);
#endif
        }

        public static void DebugLogWarning(string message)
        {
#if UNITY_EDITOR || ACTIONFIT_DEBUG
            Debug.LogWarning(message);
#endif
        }

        public static void DebugLogError(string message)
        {
#if UNITY_EDITOR || ACTIONFIT_DEBUG
            Debug.LogError(message);
#endif
        }

        public static void DebugCheckError(string message)
        {
#if UNITY_EDITOR || ACTIONFIT_DEBUG
            Debug.LogError("Check: " + message);
#endif
        }

        public static void CheckEmptyComponent<T>(this T component) where T : Component
        {
#if UNITY_EDITOR || ACTIONFIT_DEBUG
            if (component != null) return;
            DebugLogError($"{typeof(T)} - Component is null");
#endif
        }

        public static void CheckEmptyComponent<T>(params T[] types) where T : Component
        {
#if UNITY_EDITOR || ACTIONFIT_DEBUG
            foreach (var type in types)
            {
                if (type != null) continue;
                DebugLogError($"{typeof(T)} - Component is null");
            }
#endif
        }

        /// <summary>
        /// trParent에서 자기 자신과 자식 오브젝트 중 name과 맞는 오브젝트에서 T컴포넌트를 가져와 반환하는 메서드
        /// </summary>
        public static T GetComponentInChildrenName<T>(this Transform trParent, string name) where T : Component
        {
            if (trParent == null)
            {
                DebugLogError("GetComponentInChildrenName: obj is null");
                return null;
            }

            foreach (Transform child in trParent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name.Equals(name, StringComparison.Ordinal))
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }

            DebugLogError($"GetComponentInChildrenName: '{name}' is {typeof(T).Name} not found.");
            return null;
        }

        /// <summary>
        /// 현재 시간을 yyyyMMdd(20250101)형태의 string으로 반환받는 메서드
        /// </summary>
        public static string GetDateStr() => DateTime.Now.ToString("yyyyMMdd");

        public static long GetDataLong() => long.Parse(GetDateStr());

        public static bool EqualDate(ref string saveDate)
        {
            string curDate = GetDateStr();
            bool result = curDate == saveDate;
            if(!result) saveDate = curDate;
            return result;
        }
        
        public static long CurTimeTick() => DateTime.Now.Ticks;

        public static void SetAlpha(this SpriteRenderer renderer, float alpha)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }

        public static string GetCustomTimeString(long timeSeconds, TextTimeType timeType)
        {
            int timeInt = (int)timeType;
        
            string seconds = string.Empty;
            string minutes = string.Empty;
            string hours = string.Empty;
            string days = string.Empty;
        
            if (timeInt == 0)
            {
                DebugLogError("time is null");
                return string.Empty;
            }
        
            if (((int)TextTimeType.day & timeInt) != 0)
            {
                string dayStr = LocalizationManager.GetLocalString(nameof(ELocalizedName.timer_Day));
                days = $"{(timeSeconds / (24 * 60 * 60)):D2}{dayStr} ";
                timeSeconds %= (24 * 60 * 60);
            } 
        
            if (((int)TextTimeType.hour & timeInt) != 0)
            {
                string hourStr = LocalizationManager.GetLocalString(nameof(ELocalizedName.timer_Hour));
                hours = $"{(timeSeconds / (60 * 60)):D2}{hourStr} ";
                timeSeconds %= (60 * 60);
            }
        
            if (((int)TextTimeType.minutes & timeInt) != 0)
            {
                string minuteStr = LocalizationManager.GetLocalString(nameof(ELocalizedName.timer_Minute));
                minutes = $"{(timeSeconds / 60):D2}{minuteStr} ";
                timeSeconds %= 60;
            }
        
            if (((int)TextTimeType.seconds & timeInt) != 0) 
            {
                string secondStr = LocalizationManager.GetLocalString(nameof(ELocalizedName.timer_Second));
                seconds = $"{timeSeconds:D2}{secondStr} ";
            }
            return (days + hours + minutes + seconds).Trim();
        }
        
        

        /// <summary>
        /// 시간을 적절한 형식으로 포맷팅
        /// 1시간 미만: "59:59" (분:초)
        /// 1시간 이상: "01h03m" (시간h분m)
        /// </summary>
        public static string FormatTimeDisplay(int totalMinutes, int seconds)
        {
            if (totalMinutes < 60)
            {
                string minutesStr = LocalizationManager.GetLocalString(ELocalizedName.timer_Minute);
                string secondsStr = LocalizationManager.GetLocalString(ELocalizedName.timer_Second);
                // 1시간 미만: 분:초 형식
                return $"{totalMinutes:D2}{minutesStr} {seconds:D2}{secondsStr}";
            }
            else
            {
                string hourStr = LocalizationManager.GetLocalString(ELocalizedName.timer_Hour);
                string minutesStr = LocalizationManager.GetLocalString(ELocalizedName.timer_Minute);
                // 1시간 이상: 시간h분m 형식
                int hours = totalMinutes / 60;
                int minutes = totalMinutes % 60;
                return $"{hours:D2}{hourStr} {minutes:D2}{minutesStr}";
            }
        }
    }

    [Flags]
    public enum TextTimeType
    {
        day = 1 << 0,
        hour = 1 << 1,
        minutes = 1 << 2,
        seconds = 1 << 3,
    }