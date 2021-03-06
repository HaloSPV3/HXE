/**
 * Copyright (c) 2019 Emilian Roman
 * Copyright (c) 2021 Noah Sherwin
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

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using static System.IO.File;
using static HXE.Net.DefaultHttpClient;
using static HXE.Paths.MCC;

namespace HXE.MCC
{
    public static class Halo1
    {
        /// <summary>
        ///     Set a new path for Halo1.dll
        /// </summary>
        public static void SetHalo1Path(Platform platform)
        {
            switch (platform)
            {
                case Platform.Steam:
                    var mccH1 = Path.Combine(HTMCC, H1Dir, H1dll);
                    Steam.Libraries.ParseLibraries();
                    Halo1Path = Steam.Libraries.FindInLibraries(mccH1)[0];
                    break;

                case Platform.WinStore:
                    var drives = DriveInfo.GetDrives().ToList();
                    var driveLetter = drives.First(drive =>
                        Exists(Path.Combine(drive.Name, UwpH1DllPath))).Name;
                    Halo1Path = Path.Combine(driveLetter, UwpH1DllPath);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                      "Cannot set Halo1.dll path: Specified platform is invalid.");
            }

            if (!Path.IsPathRooted(Halo1Path))
                throw new FileNotFoundException($"Halo1.dll path is invalid: {Halo1Path}");

            if (Halo1Path == null)
                throw new NullReferenceException("Halo1.dll path is null or empty.");

            // this works, but it might not be worth the maintenance workload.
            //if (!Halo1DLLIsCertified())
            //    throw new CryptographicException("Halo1.dll's digital signature is invalid.");
        }

        /// <summary>
        /// Check if the inferred Halo1.dll is probably legitimate.
        /// </summary>
        /// <returns>
        /// true if the file's Digital Certificate is valid.
        /// </returns>
        public static bool Halo1DLLIsCertified()
        {
            /// Known issue: This doesn't verify the file is a real Halo1.dll
            /// It merely checks if the file's certificate is valid.
            ///
            /// Tip:
            /// If getting the data for 'P7B_Fallback' and 'remoteCert'
            /// took >200ms for each to complete, we would move them to
            /// new methods/functions as Async Tasks so they can be
            /// executed simultaneously and wait for both to complete
            /// (successfully or not) before the last line which compares
            /// one of those certificates to Halo1.dll's certificate.
            ///
            /// However, only the remoteCert takes a significant amount of time
            /// to complete and it has a 200ms timeout to mitigate wait time.

            /** Convert embedded resource to X509Certificate */
            X509Certificate P7B_Fallback;
            try
            {
                var ms = new MemoryStream();
                var assembly = Assembly.GetExecutingAssembly();
                assembly.GetManifestResourceStream(@"HXE.Assets.343I_DER.cer")
                  .CopyTo(ms);
                P7B_Fallback = new X509Certificate(ms.ToArray());
            }
            catch (Exception e)
            {
                throw new Exception("Failed to read our embedded copy of 343 Industries' code certificate",
                  e);
            }

            /** Get 343 Industries' Public Key from internet source. */
            // TODO: Add OFFICIAL URI for web-accessible public key
            var uri = "https://github.com/HaloSPV3/HCE/releases/download/updates/343I_DER.exp2022-04-27.cer";
            X509Certificate remoteCert = null;
            var remoteFailedOrTimedOut = false;
            try
            {
                var response = Client.GetAsync(uri).Result;
                MemoryStream ms = (MemoryStream) response.Content.ReadAsStream();
                byte[] msArray = ms.ToArray();
                remoteCert = new X509Certificate(msArray);
            }
            catch (Exception)
            {
                remoteFailedOrTimedOut = true;
            }

            /** Get Public Key of a reference Certificate.
             * If we failed to get the remote certificate,
             * fallback to the embedded resource.
             * This allows for offline validation.
             */
            var publicKey_ref = remoteFailedOrTimedOut ?
                P7B_Fallback.GetPublicKey() :
                remoteCert.GetPublicKey();

            /** Get certificate from local Halo1.dll. */
            var publicKey_file = new X509Certificate(Halo1Path).GetPublicKey();

            /** Return whether or not Halo1.dll is valid.
             * If the Halo1.dll certificate's public key is equal to
             * the cached PublicKey, return True. */
            return publicKey_file.SequenceEqual(publicKey_ref);
        }

        public enum Platform
        {
            Steam,
            WinStore
        }
    }
}
