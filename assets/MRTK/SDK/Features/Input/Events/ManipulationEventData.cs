// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Information associated with a particular manipulation event.
    /// </summary>
    public class ManipulationEventData
    {
        /// <summary>
        /// 正在操纵的对象
        /// </summary>
        public GameObject ManipulationSource { get; set; }

        /// <summary>
        /// 操纵对象或悬停在对象上的指针。对于OnManipulationEnded，将为空.
        /// </summary>
        public IMixedRealityPointer Pointer { get; set; }

        /// <summary>
        ///是否为近交互作用.
        /// </summary>
        public bool IsNearInteraction { get; set; }

        /// <summary>
        /// Center of the <see cref="ObjectManipulator"/>'s Pointer in world space
        /// 世界空间中<see cref=“ObjectManipulator”/>指针的中心
        /// </summary>
        public Vector3 PointerCentroid { get; set; }

        /// <summary>
        /// Pointer's Velocity.指针速度
        /// </summary>
        public Vector3 PointerVelocity { get; set; }

        /// <summary>
        /// Pointer's Angular Velocity in Eulers.指针的角速度（欧拉）
        /// </summary>
        public Vector3 PointerAngularVelocity { get; set; }
    }
}
