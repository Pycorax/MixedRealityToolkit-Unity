﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities
{
    public enum TrackedObjectType
    {
        /// <summary>
        /// Calculates position and orientation from the main camera.
        /// </summary>
        Head,
        /// <summary>
        /// Calculates position and orientation from the left motion-tracked controller.
        /// </summary>
        MotionControllerLeft,
        /// <summary>
        /// Calculates position and orientation from the right motion-tracked controller.
        /// </summary>
        MotionControllerRight
    }
}