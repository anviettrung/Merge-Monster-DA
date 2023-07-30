using System;
using UnityEngine;

namespace AVT
{
    public static class CopyBones
    {
        private static int CompareName(Transform a, Transform b)
        {
            return a.name.CompareTo(b.name);
        }

        public static void CreateBonesArray(GameObject source, GameObject target, out Transform[] sourceBones, out Transform[] targetBones)
        {
            sourceBones = source.GetComponentsInChildren<Transform>();
            targetBones = target.GetComponentsInChildren<Transform>();

            Array.Sort(sourceBones, CompareName);
            Array.Sort(targetBones, CompareName);
        }

        public static void DoCopy(Transform[] source, ref Transform[] target)
        {
            for (int i = 0; i < source.Length; i++)
            {
                target[i].localPosition = source[i].localPosition;
                target[i].localRotation = source[i].localRotation;
                target[i].localScale = source[i].localScale;
            }
        }
    }
}