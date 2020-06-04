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

namespace HXE.SPV3
{
  /// <summary>
  ///   Object representation of the SPV3.3 campaign attributes.
  /// </summary>
  public static class Campaign
  {
    /// <summary>
    ///   Available SPV3.3 difficulties.
    /// </summary>
    public enum Difficulty
    {
      Normal,    // normal
      Heroic,    // hard
      Legendary, // impossible
      Noble      // easy
    }

    /// <summary>
    ///   Available SPV3.3 missions.
    /// </summary>
    public enum Mission
    {
      Spv3A10        = 0x2 /* base value */, /* must match data\levels\ui\scripts\script.hsc */
      Spv3A30        = Spv3A10        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3A50        = Spv3A30        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3B30        = Spv3A50        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3B30Evolved = Spv3B30        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3B40        = Spv3B30Evolved + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3C10        = Spv3B40        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3C20        = Spv3C10        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3C40        = Spv3C20        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3D20        = Spv3C40        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3D25        = Spv3D20        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3D30        = Spv3D25        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3D30Evolved = Spv3D30        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3D40        = Spv3D30Evolved + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      LumoriaA       = Spv3D40        + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      LumoriaB       = LumoriaA       + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      LumoriaCd      = LumoriaB       + 0x1, /* must match data\levels\ui\scripts\script.hsc */
      Spv3A05        = LumoriaCd      + 0x1  /* must match data\levels\ui\scripts\script.hsc */
    }
  }
}