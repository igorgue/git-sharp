// Object.cs
//
// Authors:
//   Hector E. Gomez <hectoregm@gmail.com>
//   Igor Guerrero Fonseca <igfgt1@gmail.com>
//
// Copyright (c) 2008
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Text;

namespace Mono.Git.Core
{
	/// <summary>
	/// Contains the object types of Git
	/// </summary>
	public enum Type
	{
		Blob,
		Tree,
		Commit,
		Tag
	}
	
	/// <summary>
	/// Struct that represent a SHA1 hash
	/// </summary>
	public struct SHA1
	{
		public byte[] bytes;
	}
	
	/// <summary>
	/// Class that holds the basic object information
	/// </summary>
	public abstract class Object
	{
		protected SHA1 id;
		protected Type type;
		
		public Type Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public SHA1 Id {
			get {
				return id;
			}
			set {
				id = value;
			}
		}
		
		public Object(Type t)
		{
			id.bytes = new byte[160];
			type = t;
		}
		
		public string ToHexString ()
		{
			return BytesToHexString (id.bytes);
		}
		
		// Static helper functions
		
		/// <summary>
		/// Create a header for a Git hash
		/// </summary>
		/// <param name="objType">
		/// Type of the object to hash<see cref="Type"/>
		/// </param>
		/// <param name="dataSize">
		/// Size of the object to hash<see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Header<see cref="System.Byte"/>
		/// </returns>
		public static byte[] CreateHashHeader (Type objType, int dataSize)
		{
			return Encoding.Default.GetBytes (String.Format ("{0} {1}\0",
			                                                 objType.ToString ().ToLower (),
			                                                 dataSize.ToString ()));
		}
		
		/// <summary>
		/// Convert a Byte array to Hexadecimal format
		/// </summary>
		/// <param name="bytes">
		/// A byte array<see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// A String of a byte array converted to Hexadecimial format<see cref="System.String"/>
		/// </returns>
		public static string BytesToHexString (byte[] data)
		{
			StringBuilder hexString = new StringBuilder (data.Length);
			
			foreach (byte b in data) {
				hexString.Append (b.ToString ("x2"));
			}
			
			return hexString.ToString ();
		}
		
		/// <summary>
		/// Convert a Byte array to string
		/// </summary>
		/// <param name="bytes">
		/// A byte array<see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// A String of a byte array converted to String format<see cref="System.String"/>
		/// </returns>
		public static string BytesToString (byte[] data)
		{
			StringBuilder str = new StringBuilder (data.Length);
			
			foreach (byte b in data) {
				str.Append (b.ToString ());
			}
			
			return str.ToString ();
		}
		
		/// <summary>
		/// Compute the byte array to a SHA1 hash
		/// </summary>
		/// <param name="bytes">
		/// A byte array to convert<see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// A SHA1 byte array<see cref="System.Byte"/>
		/// </returns>
		public static byte[] ComputeSHA1Hash (byte[] bytes)
		{
			return System.Security.Cryptography.SHA1.Create ().ComputeHash (bytes);
		}
		
		/// <summary>
		/// Hash a single file by a given Filename
		/// </summary>
		/// <param name="filename">
		/// A file path<see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A sha1 hash<see cref="System.Byte"/>
		/// </returns>
		public static byte[] HashFile (Type objType, string filename)
		{
			byte[] bytes;
			byte[] data = new byte [160];
			byte[] header;
			
			FileStream fd = File.OpenRead (filename);
			
			bytes = new BinaryReader (fd).ReadBytes ((int) fd.Length);
			
			// Closing stream
			fd.Close ();
			
			header = CreateHashHeader (objType, bytes.Length);
			
			data = new byte[header.Length + bytes.Length];
			
			header.CopyTo (data, 0);
			bytes.CopyTo (data, header.Length);
			
			return ComputeSHA1Hash (data);
		}
		
		/// <summary>
		/// Compress a byte array
		/// </summary>
		/// <param name="data">
		/// A data byte array<see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// A compressed byte array<see cref="System.Byte"/>
		/// </returns>
		public static byte[] Compress (byte[] data)
		{
			MemoryStream ms = new MemoryStream ();
			DeflateStream ds = new DeflateStream (ms, CompressionMode.Compress);
			
			ds.Write (data, 0, data.Length);
			ds.Flush ();
			ds.Close ();
			
			return ms.ToArray ();
		}
		
		/// <summary>
		/// Decompress a byte array
		/// </summary>
		/// <param name="data">
		/// A data byte array<see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// A decompressed byte array<see cref="System.Byte"/>
		/// </returns>
		public static byte[] Decompress(byte[] data)
		{
			const int BUFFER_SIZE = 256;
			byte[] tempArray = new byte[BUFFER_SIZE];
			List<byte[]> tempList = new List<byte[]>();
			int count = 0, length = 0;
			
			MemoryStream ms = new MemoryStream (data);
			DeflateStream ds = new DeflateStream (ms, CompressionMode.Decompress);
			
			while ((count = ds.Read (tempArray, 0, BUFFER_SIZE)) > 0) {
				if (count == BUFFER_SIZE) {
					tempList.Add (tempArray);
					tempArray = new byte[BUFFER_SIZE];
				} else {
					byte[] temp = new byte[count];
					Array.Copy (tempArray, 0, temp, 0, count);
					tempList.Add (temp);
				}
				length += count;
			}
			
			byte[] retVal = new byte[length];
			
			count = 0;
			foreach (byte[] temp in tempList) {
				Array.Copy (temp, 0, retVal, count, temp.Length);
				count += temp.Length;
			}
			
			return retVal;
		}
	}
}
