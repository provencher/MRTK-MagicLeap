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

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality;

namespace prvncher.MRTK_MagicLeap.DeviceManagement.Input
{
    public class MagicLeapHand : BaseHand, IMixedRealityHand
    {
        public MagicLeapHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null) : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }
        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            throw new System.NotImplementedException();
        }

#if PLATFORM_LUMIN
        private void Initalize()
        {

        }

        public void CleanupHand()
        {

        }

        public void DoUpdate()
        {

        }
#endif // PLATFORM_LUMIN

    }
}