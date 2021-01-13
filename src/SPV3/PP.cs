/**
 * Copyright (c) 2021 Emilian Roman
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
  public static class PP
  {
    public const int INTERNAL              = 1 << 0x00;
    public const int EXTERNAL              = 1 << 0x01;
    public const int GBUFFER               = 1 << 0x02;
    public const int DEPTH_FADE            = 1 << 0x03;
    public const int BLOOM                 = 1 << 0x04;
    public const int LENS_DIRT             = 1 << 0x05;
    public const int DYNAMIC_LENS_FLARES   = 1 << 0x06;
    public const int VOLUMETRIC_LIGHTING   = 1 << 0x07;
    public const int ANTI_ALIASING         = 1 << 0x08;
    public const int HUD_VISOR             = 1 << 0x09;
    public const int FILM_GRAIN            = 1 << 0x0A;
    public const int MOTION_BLUR_BUILT_IN  = 1 << 0x0B;
    public const int MOTION_BLUR_POMB_LOW  = 1 << 0x0C;
    public const int MOTION_BLUR_POMB_HIGH = 1 << 0x0D;
    public const int MXAO_LOW              = 1 << 0x0E;
    public const int MXAO_HIGH             = 1 << 0x0F;
    public const int DOF_LOW               = 1 << 0x10;
    public const int DOF_HIGH              = 1 << 0x11;
    public const int SSR                   = 1 << 0x12;
    public const int DEBAND                = 1 << 0x13;
    public const int ADAPTIVE_HDR          = 1 << 0x14;
  }
}