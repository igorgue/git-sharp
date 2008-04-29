// Hash.cs
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
//

using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GitSharp.Core
{
	/// <summary>
	/// Class used to hash an object
	/// </summary>
	public class Hash
	{	
		public Hash ()
		{
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
		public static string BytesToHex (byte[] bytes)
		{
			StringBuilder hexString = new StringBuilder (bytes.Length);
			for (int i = 0; i < bytes.Length; i++) {
				hexString.Append (bytes [i].ToString ("x2"));
			}
			
			return hexString.ToString ();
		}
		
		/// <summary>
		/// Converts the object to a SHA1 bytes array
		/// </summary>
		/// <param name="data">
		/// Object to convert<see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A byte array to return<see cref="System.Byte"/>
		/// </returns>
		public static byte[] HashObject (object data)
		{
			throw new NotImplementedException("HashObject (object data) is not implemented");
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
		public static byte[] HashFile (string filename)
		{
			List<byte> bytes = new List<byte> ();
			string header;
			BinaryReader fd = new BinaryReader (File.Open (filename, FileMode.Open));
			
			while (fd.PeekChar () != -1)
				bytes.Add (fd.ReadByte ());
			
			byte[] byteInput = bytes.ToArray ();
			
			header = "blob " + (byteInput.Length + 0).ToString () + "\0";
			
			byte[] byteHeader = Encoding.ASCII.GetBytes (header);
			byte[] data = new byte[byteHeader.Length + byteInput.Length];
			
			byteHeader.CopyTo (data, 0);
			byteInput.CopyTo (data, byteHeader.Length);
			
			SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
			byte[] result = sha1.ComputeHash (data);
			
			return result;
		}
		
		/// <summary>
		/// A method to test the hash generation
		/// </summary>
		/// <param name="args">
		/// Filename to convert<see cref="System.String"/>
		/// </param>
		public static void Test (string[] args)
		{
			string str = BytesToHex (HashFile (args [0]));
			System.Console.WriteLine (str);
		}
	}
}
