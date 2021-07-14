﻿/**
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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using static System.Console;
using static System.Environment;
using static System.IO.Compression.CompressionMode;
using static System.IO.FileMode;
using static System.IO.Path;
using static System.IO.SeekOrigin;
using static System.Reflection.Assembly;
using static System.Text.Encoding;
using static HXE.Console;

namespace HXE
{
	/**
	 * Class containing the data structure and logic for creating HXE SFX binaries.
	 */
	public class SFX
	{
		/**
		 * Files to be compressed and extracted on the filesystem using the HXE SFX system.
		 */
		public List<Entry> Entries { get; set; } = new List<Entry>();

		/**
		 * Retrieves a byte array representing the object state which can hydrate an SFX instance using Deserialise().
		 */
		public byte[] Serialise()
		{
			var xml = new XmlSerializer(typeof(SFX));
			using (var sw = new StringWriter())
			{
				xml.Serialize(sw, this);
				return Unicode.GetBytes(sw.ToString());
			}
		}

		/**
		 * Hydrates the SFX instance properties with inbound data previously created by Serialise().
		 */
		public void Deserialise(byte[] data)
		{
			using (var sr = new StringReader(Unicode.GetString(data)))
			{
				var sfx = (SFX) new XmlSerializer(typeof(SFX)).Deserialize(sr);
				Entries = sfx.Entries;
			}
		}

		/**
		 * Creates an SFX of the given source, using the current HXE executable, to the given target.
		 */
		public static void Compile(DirectoryInfo source, DirectoryInfo target)
		{
			target.Create();

			/**
			 * We will create an SFX for all of the files residing in the given source directory. Our goal is to contain all
			 * of the data in a single file, and subsequently reproduce the directory tree & file contents.
			 *
			 * For a reasonable balance between space savings and process time, we will DEFLATE each file before including it
			 * in the output SFX binary.
			 *
			 * The structure of the output SFX is essentially:
			 *
			 * [hxe assembly] + [deflate data] + [sfx object] + [sfx object length]
			 * |------------|   |------------|   |----------|   |-----------------|
			 *              |                |              |                     |
			 *              |                |              |                     +- used for seeking the start of this sfx
			 *              |                |              |                        object, by reading backwards from EOF
			 *              |                |              |
			 *              |                |              +----------------------- specifies the deflate offsets & output
			 *              |                |                                       file name + path on the filesystem
			 *              |                |
			 *              |                +-------------------------------------- deflate data for each discovered file in
			 *              |                                                        the source directory
			 *              |
			 *              +------------------------------------------------------- contents of this executable; essentially,
			 *                                                                       we append the aforementioned data to the
			 *                                                                       end of this executable
			 */

			var sfx   = new SFX();
			var files = source.GetFiles("*", SearchOption.AllDirectories);

			var sourceHxe = new FileInfo(GetEntryAssembly()?.Location ?? throw new InvalidOperationException());
			var targetHxe = new FileInfo(Combine(target.FullName, "hxe.sfx.exe"));

			Info($"Source: {source.FullName}");
			Info($"Target: {target.FullName}");

			Info($"Source HXE EXE: {sourceHxe.FullName}");
			Info($"Target HXE SFX: {targetHxe.FullName}");

			if (targetHxe.Exists)
				targetHxe.Delete();

			/**
			 * We create a copy of this executable for subsequent appending of the DEFLATE & SFX data. We refresh the FileInfo
			 * object to ensure that we have the real length of the executable on the fs.
			 */

			sourceHxe.CopyTo(targetHxe.FullName);
			targetHxe.Refresh();

			Info($"Copied {targetHxe.Length} bytes to: {targetHxe.Length}");

			/**
			 * For each discovered file in the given source directory, we will:
			 *
			 * 1. Append a DEFLATE representation of it to the copied HXE executable;
			 * 2. Create an SFX entry specifying:
			 *    a) the file name & path on the fs
			 *    b) the file length on the fs
			 *    c) offset of the DEFLATE data in HXE SFX
			 */

			foreach (var file in files)
			{
				/**
				 * We append the DEFLATE data to the HXE SFX binary. After the procedure is done, we will refresh the FileInfo
				 * for the SFX to retrieve its new length. This length will be used to determine length of the DEFALTE and thus
				 * its offset in the HXE SFX binary.
				 */

				long deflateLength;

				{
					using (var iStream = file.OpenRead())
					using (var oStream = System.IO.File.Open(targetHxe.FullName, Append))
					using (var dStream = new DeflateStream(oStream, Compress))
					{
						iStream.CopyTo(dStream);
					}

					var oldLength = targetHxe.Length;
					targetHxe.Refresh();
					var newLength = targetHxe.Length;
					
					Info($"HXE SFX increased from {oldLength} to {newLength} bytes.");
					
					deflateLength = newLength - oldLength;

					Info($"DEFLATE length is thus {deflateLength} bytes.");
				}

				/**
				 * We will add an entry for the current file and its DEFLATE representation to teach the HXE SFX how to recreate
				 * the file down the line.
				 *
				 * The entries are designed to recreate the structure of the source directory, in a given arbitrary target
				 * directory. As such, we will avoid absoltue paths for the files and instead infer paths relative to the source
				 * directory.
				 *
				 * Each DEFLATE entry will be appended at the end of the HXE SFX binary. To determine where each file's DEFLATE
				 * representation starts, we determine the offset by extracting the DEFLATE length, from the HXE SFX binary
				 * length.
				 */

				{
					Info($"Acknowledging new entry: {targetHxe.Name} <= {file.Name}");

					var name   = file.Name;
					var length = file.Length;
					var offset = targetHxe.Length - deflateLength;
					var path = file.DirectoryName != null && file.DirectoryName.Equals(source.FullName)
						? string.Empty
						: file.DirectoryName?.Substring(source.FullName.Length + 1);

					Debug($"NAME   - {name}");
					Debug($"LENGTH - {length}");
					Debug($"OFFSET - {offset}");
					Debug($"PATH   - {path}");

					sfx.Entries.Add(new Entry
					{
						Name   = name,
						Path   = path,
						Length = length,
						Offset = offset
					});

					targetHxe.Refresh();
				}

				WriteLine(NewLine + new string('-', 80));
			}

			/**
			 * Once we have created & appended the DEFLATE data for each file, and also populated the SFX object, we will
			 * serialise it to a byte array which in turn gets appended to the HXE SFX binary as well. This makes the SFX
			 * completely self-contained and portable.
			 *
			 * Because an SFX object's array representation is a variable length, we will append the said length to the binary
			 * as well. This will allow HXE to determine both the start and end of the SFX data when deserialising it, by
			 * seeking backwards from the EOF. By seeking as many bytes backwards as specified in the length value, the start
			 * of the SFX data can be determined.
			 */

			{
				var sfxData   = sfx.Serialise();
				var sfxOffset = targetHxe.Length;

				Info($"Appending {sfxData.Length} bytes of SFX information.");
				Debug($"OFFSET: {sfxOffset}");

				using (var hxeStream = new FileStream(targetHxe.FullName, Append))
				using (var binWriter = new BinaryWriter(hxeStream))
				{
					binWriter.Write(sfxData);
					binWriter.Write(sfxOffset);
				}
			}
		}

		/**
		 * Extracts the SFX contents of the current HXE executable to the given target.
		 */
		public static void Extract(DirectoryInfo target)
		{
			target.Create();

			/**
			 * We will assume that the current HXE executable contains SFX data to be extracted.
			 */

			var hxe = new FileInfo(GetEntryAssembly()?.Location ?? throw new InvalidOperationException());
			var sfx = new SFX();

			Info($"Determining SFX structure in {hxe.Name}");

			/**
			 * We first hydrate the SFX object with information from the HXE SFX binary. The start of the SFX data is inferred
			 * from the length specified at the EOF. We also need to keep in mind the length of the variable that specifies
			 * the length of the SFX data. Since it's a long, the size would be 8.
			 *
			 * With the above confusion in mind:
			 * - Starting offset of the SFX data = (hxe length - sizeof(long) - sfx data length)
			 * - Ending offset of the SFX data   = (hxe length - sizeof(long))
			 */

			using (var binReader = new BinaryReader(hxe.OpenRead()))
			{
				binReader.BaseStream.Seek(hxe.Length - sizeof(long), Begin);
				binReader.BaseStream.Seek(binReader.ReadInt64(),     Begin);
				sfx.Deserialise
				(
					binReader.ReadBytes
					(
						(int) hxe.Length - (int) binReader.BaseStream.Position - sizeof(long)
					)
				);
			}

			/**
			 * For each entry specified in the SFX object, we seek its DEFLATE data and extract it to the fs at the given path
			 * and filename. Because .NET doesn't support copying a specified amount of bytes from a stream to another, we are
			 * implementing our own method for copying the data using buffers.
			 */

			foreach (var entry in sfx.Entries)
			{
				var original = new FileInfo(Combine(target.FullName, entry.Path, entry.Name));

				original.Directory?.Create();

				Info($"Extracting DEFLATE to: {original.FullName}");

				using (var iStream = hxe.OpenRead())
				using (var oStream = original.Create())
				using (var dStream = new DeflateStream(iStream, Decompress))
				{
					dStream.BaseStream.Seek(entry.Offset, Begin);
					var bytes  = entry.Length;
					var buffer = new byte[0x8000];
					int read;
					while (bytes > 0 && (read = dStream.Read(buffer, 0, Math.Min(buffer.Length, (int) bytes))) > 0)
					{
						oStream.Write(buffer, 0, read);
						bytes -= read;
					}
				}
			}
		}

		/**
		 * Entry for a file to be compressed and extracted using the HXE SFX system.
		 */
		public class Entry
		{
			public string Name   { get; set; }                 /* original file name on the filesystem    */
			public string Path   { get; set; } = string.Empty; /* path relative to root source/target dir */
			public long   Offset { get; set; }                 /* offset in the HXE SFX executable        */
			public long   Length { get; set; }                 /* file length on the filesystem           */
		}
	}
}