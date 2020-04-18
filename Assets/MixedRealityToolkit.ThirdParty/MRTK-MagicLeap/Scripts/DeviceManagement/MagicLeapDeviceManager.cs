﻿//------------------------------------------------------------------------------ -
//MRTK-MagicLeap
//https ://github.com/provencher/MRTK-MagicLeap
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using Microsoft.MixedReality.Toolkit;

#if PLATFORM_LUMIN
using UnityEngine.XR.MagicLeap;
#endif

namespace prvncher.MRTK_MagicLeap.DeviceManagement
{
    /// <summary>
    /// Manages Magic Leap Device
    /// </summary>
    [MixedRealityDataProvider(typeof(IMixedRealityInputSystem), SupportedPlatforms.Lumin, "Magic Leap Device Manager")]
    public class MagicLeapDeviceManager : XRSDKDeviceManager
    {
        private Dictionary<Handedness, Input.MagicLeapHand> trackedHands = new Dictionary<Handedness, Input.MagicLeapHand>();

        private readonly MLHandTracking.HandKeyPose[] supportedGestures = new[]
        {
            MLHandTracking.HandKeyPose.Finger,
            MLHandTracking.HandKeyPose.Pinch,
            MLHandTracking.HandKeyPose.Fist,
            MLHandTracking.HandKeyPose.Thumb,
            MLHandTracking.HandKeyPose.L,
            MLHandTracking.HandKeyPose.OpenHand,
            MLHandTracking.HandKeyPose.Ok,
            MLHandTracking.HandKeyPose.C,
            MLHandTracking.HandKeyPose.NoPose
        };

        public bool IsReady { get; private set; } = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public MagicLeapDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile)
        {
        }

        public override void Update()
        {
            // Ensure input is active
            StartMLInput();
            UpdateHands();
        }

        public override void Disable()
        {
            DiscardHandInput();
            StopMLInput();
        }

        public override IMixedRealityController[] GetActiveControllers()
        {
            return trackedHands.Values.ToArray<IMixedRealityController>();
        }

        private void DiscardHandInput()
        {
            foreach (var hand in trackedHands)
            {
                RecyclePointers(hand.Value.InputSource);
            }
            trackedHands.Clear();
        }

#if PLATFORM_LUMIN
        private void StartMLInput()
        {
            if (IsReady) return;

            // Turn on inputs:
            if (!MLHandTracking.IsStarted)
            {
                if (!MLHandTracking.Start().IsOk)
                {
                    Debug.LogError("Failed to initialize ML Hand Tracking");
                    return;
                }
                else
                {
                    MLHandTracking.KeyPoseManager.SetKeyPointsFilterLevel(MLHandTracking.KeyPointFilterLevel.Smoothed);
                }
            }

            // Setup hand tracking and supported gestures
            MLHandTracking.KeyPoseManager.EnableKeyPoses(supportedGestures, true, false);

            IsReady = true;
        }

        private void StopMLInput()
        {
            //turn off hand tracking:
            if (MLHandTracking.IsStarted)
            {
                MLHandTracking.Stop();
            }
            IsReady = false;
        }

        /// <inheritdoc />
        public override bool CheckCapability(MixedRealityCapability capability)
        {
            return (capability == MixedRealityCapability.ArticulatedHand);
        }

        #region Hand Management
        protected void UpdateHands()
        {
            //avoid MLHands start failures:
            if (!IsReady)
            {
                return;
            }

            UpdateHand(MLHandTracking.Right, Handedness.Right);
            UpdateHand(MLHandTracking.Left, Handedness.Left);
        }

        protected void UpdateHand(MLHandTracking.Hand mlHand, Handedness handedness)
        {
            if (mlHand.IsVisible)
            {
                var hand = GetOrAddHand(mlHand, handedness);
                hand.DoUpdate();
            }
            else
            {
                RemoveHandDevice(handedness);
            }
        }

        private Input.MagicLeapHand GetOrAddHand(MLHandTracking.Hand mlHand, Handedness handedness)
        {
            if (trackedHands.ContainsKey(handedness))
            {
                return trackedHands[handedness];
            }

            // Add new hand
            var pointers = RequestPointers(SupportedControllerType.ArticulatedHand, handedness);
            var inputSourceType = InputSourceType.Hand;

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            var inputSource = inputSystem?.RequestNewGenericInputSource($"Oculus Quest {handedness} Hand", pointers, inputSourceType);

            var controller = new Input.MagicLeapHand(TrackingState.Tracked, handedness, inputSource);

            // Code is obsolete later on, but older MRTK versions require it.
            controller.SetupConfiguration(typeof(Input.MagicLeapHand));

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            inputSystem?.RaiseSourceDetected(controller.InputSource, controller);

            trackedHands.Add(handedness, controller);

            return controller;
        }

        private void RemoveHandDevice(Handedness handedness)
        {
            if (trackedHands.TryGetValue(handedness, out Input.MagicLeapHand hand))
            {
                RemoveHandDevice(hand);
            }
        }

        private void RemoveAllHandDevices()
        {
            if (trackedHands.Count == 0) return;

            // Create a new list to avoid causing an error removing items from a list currently being iterated on.
            foreach (var hand in new List<Input.MagicLeapHand>(trackedHands.Values))
            {
                RemoveHandDevice(hand);
            }
            trackedHands.Clear();
        }

        private void RemoveHandDevice(Input.MagicLeapHand hand)
        {
            if (hand == null) return;

            hand.CleanupHand();
            CoreServices.InputSystem?.RaiseSourceLost(hand.InputSource, hand);
            trackedHands.Remove(hand.ControllerHandedness);

            // Recycle pointers makes this loop obsolete. If you are using an MRTK version older than 2.3, please use the loop and comment out RecyclePointers.
            /*
            foreach (IMixedRealityPointer pointer in hand.InputSource.Pointers)
            {
                if (pointer == null) continue;
                pointer.Controller = null;
            }
            */
            RecyclePointers(hand.InputSource);
        }
        #endregion
#else
        private void StartMLInput()
        {
        }

        private void StopMLInput()
        {
        }

        private void UpdateHands()
        {
        }
#endif // PLATFORM_LUMIN
    }
}