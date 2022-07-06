using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace GameKit
{
    public static class CommonExtension
    {
        private static Random rng = new Random();

        public static T GetRandom<T>(this IList<T> list, bool remove = false)
        {
            if (list == null || list.Count == 0) return default(T);
            var n      = rng.Next(0, list.Count);
            var result = list[n];
            if (remove)
                list.RemoveAt(n);
            return result;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (action == null) throw new NullReferenceException("Action");
            foreach (T element in source)
            {
                action(element);
            }
        }

        public static T GetOrAddComponent<T>(this Component child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                return child.gameObject.AddComponent<T>();
            }

            return result;
        }

        public static T GetOrAddComponent<T>(this GameObject child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                return child.AddComponent<T>();
            }

            return result;
        }

        /// <summary>
        /// Задает нового родителя и обнуляет позици, поворот и скейл
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent">новый родитель</param>
        public static void SetParentZero(this Transform child, Transform parent)
        {
            child.SetParent(parent);
            child.localScale       = Vector3.one;
            child.localPosition    = Vector3.zero;
            child.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// Задает нового родителя и обнуляет позицию
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent">новый родитель</param>
        public static void SetParentPositionZero(this Transform child, Transform parent)
        {
            child.SetParent(parent);
            child.localPosition = Vector3.zero;
        }


        /// <summary>
        /// Перемешивает лист
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k     = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T[] GetShuffleArray<T>(this IList<T> array)
        {
            var copyArray = array.ToArray();
            var n         = copyArray.Length;
            while (n > 1)
            {
                var k    = rng.Next(n--);
                var temp = copyArray[n];
                copyArray[n] = copyArray[k];
                copyArray[k] = temp;
            }

            return copyArray;
        }

        public static void DestroyChildren(this Transform parent)
        {
            if (parent == null) return;
            for (int i = 0; i < parent.childCount; i++)
                Object.Destroy(parent.GetChild(i).gameObject);
        }
    }
}