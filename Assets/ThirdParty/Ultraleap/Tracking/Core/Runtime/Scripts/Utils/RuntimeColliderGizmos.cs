/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2021.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using UnityEngine;

namespace Leap.Unity.RuntimeGizmos
{
    [System.Obsolete("This code will be moved to a legacy package in the next major version of the plugin. If you believe that it needs to be kept in tracking, please open a discussion on the GitHub forum (https://github.com/ultraleap/UnityPlugin/discussions)")]
    public class RuntimeColliderGizmos : MonoBehaviour, IRuntimeGizmoComponent
    {

        public Color color = Color.white;
        public bool useWireframe = true;
        public bool traverseHierarchy = true;
        public bool drawTriggers = false;

        /// <summary>
        /// An empty Start() method; gives the MonoBehaviour an enable/disable checkbox.
        /// </summary>
        void Start() { }

        public void OnDrawRuntimeGizmos(RuntimeGizmoDrawer drawer)
        {
            if (!this.gameObject.activeInHierarchy
                || !this.enabled) return;

            drawer.color = color;
            drawer.DrawColliders(gameObject, useWireframe, traverseHierarchy, drawTriggers);
        }
    }
}