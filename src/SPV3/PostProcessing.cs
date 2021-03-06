/**
 * Copyright (c) 2019 Emilian Roman
 * Copyright (c) 2020 Noah Sherwin
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

namespace HXE.SPV3
{
  /// <summary>
  ///   Represents SPV3 post-processing settings.
  /// </summary>
  public class PostProcessing
  {
    /// <summary>
    ///   Depth of Field values.
    /// </summary>
    public enum DofOptions
    {
      Off  = 0x0,
      Low  = 0x1,
      High = 0x2
    }

    /// <summary>
    ///   Motion Blur values.
    /// </summary>
    public enum MotionBlurOptions
    {
      Off      = 0x0,
      BuiltIn  = 0x1,
      PombLow  = 0x2,
      PombHigh = 0x3
    }

    /// <summary>
    ///   MXAO values.
    /// </summary>
    public enum MxaoOptions
    {
      Off  = 0x0,
      Low  = 0x1,
      High = 0x2
    }

    public bool                       Internal           { get; set; } = true;
    public bool                       External           { get; set; } = true;
    public bool                       GBuffer            { get; set; } = true;
    public bool                       DepthFade          { get; set; } = true;
    public bool                       Bloom              { get; set; } = false;
    public bool                       LensDirt           { get; set; } = false;
    public bool                       DynamicLensFlares  { get; set; } = false;
    public bool                       VolumetricLighting { get; set; } = false;
    public bool                       AntiAliasing       { get; set; } = false;
    public bool                       HudVisor           { get; set; } = true;
    public bool                       FilmGrain          { get; set; } = false;
    public MotionBlurOptions          MotionBlur         { get; set; } = MotionBlurOptions.Off;
    public MxaoOptions                MXAO               { get; set; } = MxaoOptions.Off;
    public DofOptions                 DOF                { get; set; } = DofOptions.Off;
    public ExperimentalPostProcessing Experimental       { get; set; } = new ExperimentalPostProcessing();
    public bool                       SSR                { get; set; } = false;
    public bool                       Deband             { get; set; } = false;
    public bool                       AdaptiveHDR        { get; set; } = false;

    /// <summary>
    ///   Experimental overrides for HCE.
    /// </summary>
    public class ExperimentalPostProcessing
    {
      public enum ColorBlindModeOptions
      {
        Off          = 0x0,
        Protanopia   = 0x1,
        Deuteranopes = 0x2,
        Tritanopes   = 0x3
      }

      public enum ThreeDimensionalOptions
      {
        Off          = 0x0,
        Anaglyphic   = 0x1,
        Interleaving = 0x2,
        SideBySide   = 0x3
      }

      public ThreeDimensionalOptions ThreeDimensional { get; set; } = ThreeDimensionalOptions.Off;
      public ColorBlindModeOptions   ColorBlindMode   { get; set; } = ColorBlindModeOptions.Off;
    }
  }
}
