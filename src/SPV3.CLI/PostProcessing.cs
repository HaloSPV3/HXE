/**
 * Copyright (c) 2019 Emilian Roman
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

namespace SPV3.CLI
{
  /// <summary>
  ///   Object representing Post-Processing effects.
  /// </summary>
  public class PostProcessing
  {
    /// <summary>
    ///   Depth of Field values.
    /// </summary>
    public enum DofOptions
    {
      Off,
      Low,
      High
    }

    /// <summary>
    ///   Motion Blur values.
    /// </summary>
    public enum MotionBlurOptions
    {
      Off,
      BuiltIn,
      PombLow,
      PombHigh
    }

    /// <summary>
    ///   MXAO values.
    /// </summary>
    public enum MxaoOptions
    {
      Off,
      Low,
      High
    }

    public bool Internal          { get; set; }
    public bool External          { get; set; }
    public bool GBuffer           { get; set; }
    public bool DepthFade         { get; set; }
    public bool Bloom             { get; set; }
    public bool LensDirt          { get; set; }
    public bool DynamicLensFlares { get; set; }
    public bool Volumetrics       { get; set; }
    public bool AntiAliasing      { get; set; }
    public bool HudVisor          { get; set; }

    public MotionBlurOptions MotionBlur { get; set; } = MotionBlurOptions.PombHigh;
    public MxaoOptions       Mxao       { get; set; } = MxaoOptions.High;
    public DofOptions        Dof        { get; set; } = DofOptions.High;

    public ExperimentalPostProcessing Experimental { get; set; } = new ExperimentalPostProcessing();

    /// <summary>
    ///   Experimental overrides for SPV3.
    /// </summary>
    public class ExperimentalPostProcessing
    {
      public enum ThreeDimensionalOptions
      {
        Off,
        Anaglyphic,
        Interleaving,
        SideBySide
      }

      public enum ColorBlindModeOptions
      {
        Off,
        Protanopia,
        Deuteranopes,
        Tritanopes
      }

      public ThreeDimensionalOptions ThreeDimensional { get; set; } = ThreeDimensionalOptions.Off;
      public ColorBlindModeOptions ColorBlindMode { get; set; } = ColorBlindModeOptions.Off;
    }
  }
}