using System.Collections.Generic;
using UnityEngine;

namespace MxUnikit.Extensions
{
    public static class MxExtensions
    {
        #region Destroy Children

        public static Transform DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }

            return transform;
        }

        public static Transform DestroyChildren(this Transform transform, params Transform[] ignoredTransforms)
        {
            return ignoredTransforms is not { Length: > 0 }
                ? transform.DestroyChildren()
                : transform.DestroyChildren((IEnumerable<Transform>)ignoredTransforms);
        }

        public static Transform DestroyChildren(this Transform transform, IEnumerable<Transform> ignoredTransforms)
        {
            if (ignoredTransforms is null or ICollection<Transform> { Count: 0 })
            {
                return transform.DestroyChildren();
            }

            HashSet<Transform> ignoredSet = new HashSet<Transform>(ignoredTransforms);

            if (ignoredSet is { Count: 0 })
            {
                return transform.DestroyChildren();
            }

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);

                if (!ignoredSet.Contains(child))
                {
                    Object.Destroy(child.gameObject);
                }
            }

            return transform;
        }

        #endregion
    }
}
