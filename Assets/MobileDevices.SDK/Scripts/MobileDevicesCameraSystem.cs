// Copyright(c) 2019 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using System.Collections;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.ARFoundation;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    public class MobileDevicesCameraSystem : BaseCoreSystem, IMixedRealityCameraSystem
    {
        private ARCameraBackground arCameraBackground;
        private ARCameraManager arCameraManager;
        private GameObject arSession;

        private GameObject arSessionOrigin;
        private TrackedPoseDriver trackedPoseDriver;

        private MixedRealityCameraProfile cameraProfile;

        public MobileDevicesCameraSystem(
            IMixedRealityServiceRegistrar registrar,
            BaseMixedRealityProfile profile = null) : base(registrar, profile)
        {
        }

        public bool IsOpaque => true;

        public override void Destroy()
        {
#if !UNITY_EDITOR
            ReleaseForARFoundation();
#endif
            base.Destroy();
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "MRTK For Smart Phone Camera System";

        /// <inheritdoc />
        public MixedRealityCameraProfile CameraProfile
        {
            get
            {
                if (cameraProfile == null)
                {
                    cameraProfile = ConfigurationProfile as MixedRealityCameraProfile;
                }

                return cameraProfile;
            }
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
#if !UNITY_EDITOR
            ApplySettingsForARFoundation();
#endif
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();
#if !UNITY_EDITOR
            ApplySettingsForARFoundation();
#endif
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Camera Systems to compare to.
            return false;
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        private void ReleaseForARFoundation()
        {
            if (MixedRealityPlayspace.Transform.GetComponentInChildren<ARSessionOrigin>() == null) return;
            if (MixedRealityPlayspace.Transform.GetComponentInChildren<ARSession>() == null) return;

            CameraCache.Main.transform.parent = MixedRealityPlayspace.Transform;
            Object.Destroy(trackedPoseDriver);
            Object.Destroy(arCameraBackground);
            Object.Destroy(arCameraManager);
            Object.Destroy(arSessionOrigin);
            Object.Destroy(arSession);

            arSessionOrigin = null;
            arSession = null;
            arCameraBackground = null;
            arCameraManager = null;
            trackedPoseDriver = null;
        }

        private void ApplySettingsForARFoundation()
        {
            if (MixedRealityPlayspace.Transform.GetComponentInChildren<ARSessionOrigin>() != null) return;
            if (MixedRealityPlayspace.Transform.GetComponentInChildren<ARSession>() != null) return;

            // Setting Camera for ARFoundation.
            trackedPoseDriver = CameraCache.Main.gameObject.AddComponent<TrackedPoseDriver>();
            arCameraManager = CameraCache.Main.gameObject.AddComponent<ARCameraManager>();
            arCameraBackground = CameraCache.Main.gameObject.AddComponent<ARCameraBackground>();

            this.arSessionOrigin = new GameObject();
            this.arSessionOrigin.name = "AR Session Origin";
            this.arSessionOrigin.transform.parent = MixedRealityPlayspace.Transform;
            CameraCache.Main.transform.parent = this.arSessionOrigin.transform;
            var arSessionOrigin = this.arSessionOrigin.AddComponent<ARSessionOrigin>();
            arSessionOrigin.camera = CameraCache.Main;

            arSession = new GameObject();
            arSession.name = "AR Session";
            arSession.transform.parent = MixedRealityPlayspace.Transform;
            var arSessionComp = arSession.AddComponent<ARSession>();
            arSessionComp.attemptUpdate = true;
            arSessionComp.matchFrameRate = true;
            arSession.AddComponent<ARInputManager>();

            trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice,
                TrackedPoseDriver.TrackedPose.ColorCamera);
        }
    }
}