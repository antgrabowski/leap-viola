/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2021.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using System;
using UnityEngine;

namespace Leap.Unity.Space
{
    [System.Obsolete("This code will be moved to a legacy package in the next major version of the plugin. If you believe that it needs to be kept in tracking, please open a discussion on the GitHub forum (https://github.com/ultraleap/UnityPlugin/discussions)")]
    [DisallowMultipleComponent]
    public class LeapSpaceAnchor : MonoBehaviour
    {

        [HideInInspector]
        public LeapSpaceAnchor parent;

        [HideInInspector]
        public LeapSpace space;

        public ITransformer transformer;

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        public void RecalculateParentAnchor()
        {
            if (this is LeapSpace)
            {
                parent = this;
            }
            else
            {
                parent = GetAnchor(transform.parent);
            }
        }

        public static LeapSpaceAnchor GetAnchor(Transform root)
        {
            while (true)
            {
                if (root == null)
                {
                    return null;
                }

                var anchor = root.GetComponent<LeapSpaceAnchor>();
                if (anchor != null && anchor.enabled)
                {
                    return anchor;
                }

                root = root.parent;
            }
        }
    }
}