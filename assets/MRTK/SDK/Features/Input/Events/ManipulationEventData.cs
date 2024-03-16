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
        /// ���ڲ��ݵĶ���
        /// </summary>
        public GameObject ManipulationSource { get; set; }

        /// <summary>
        /// ���ݶ������ͣ�ڶ����ϵ�ָ�롣����OnManipulationEnded����Ϊ��.
        /// </summary>
        public IMixedRealityPointer Pointer { get; set; }

        /// <summary>
        ///�Ƿ�Ϊ����������.
        /// </summary>
        public bool IsNearInteraction { get; set; }

        /// <summary>
        /// Center of the <see cref="ObjectManipulator"/>'s Pointer in world space
        /// ����ռ���<see cref=��ObjectManipulator��/>ָ�������
        /// </summary>
        public Vector3 PointerCentroid { get; set; }

        /// <summary>
        /// Pointer's Velocity.ָ���ٶ�
        /// </summary>
        public Vector3 PointerVelocity { get; set; }

        /// <summary>
        /// Pointer's Angular Velocity in Eulers.ָ��Ľ��ٶȣ�ŷ����
        /// </summary>
        public Vector3 PointerAngularVelocity { get; set; }
    }
}
