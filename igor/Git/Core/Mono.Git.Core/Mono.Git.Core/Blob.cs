// Blob.cs
//
// Author:
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
using System.IO;
using System.IO.Compression;

namespace Mono.Git.Core
{
	/// <summary>
	/// Class that store a Blob
	/// </summary>
	public class Blob : Object
	{
		private byte[] content;
		
		public byte[] Content
		{
			get {
				return content;
			}
			set {
				content = value;
			}
		}
		
		/// <summary>
		/// Initialize the object type and the bytes
		/// </summary>
		public Blob () : base (Type.Blob)
		{
		}
		
		/// <summary>
		/// Initialize the Object with a given file path
		/// </summary>
		/// <param name="objType">
		/// A type of object<see cref="Type"/>
		/// </param>
		/// <param name="filePath">
		/// A path represented by a string<see cref="System.String"/>
		/// </param>
		public Blob (string filePath) : base (Type.Blob)
		{
			id.bytes = HashFile (type, filePath);
			AddContent (filePath);
		}
		
		public void AddContent (string filePath)
		{
			FileStream f = File.Open (filePath, FileMode.Open);
			
			content = new BinaryReader (f).ReadBytes ((int) f.Length);
			
			f.Close ();
		}
		
		/// <summary>
		/// TODO: Test, remove this please later
		/// </summary>
		/// <param name="filePath">
		/// A <see cref="System.String"/>
		/// </param>
		public static void Write (string filePath)
		{
			Blob b = new Blob (filePath);
			b.AddContent (filePath);
			
			FileStream f = new FileStream (b.ToHexString (), FileMode.Create);
			//FileStream f = new FileStream ("hello.object", FileMode.Create);
			
			byte[] bytesHeader;
			bytesHeader = Object.CreateHashHeader (Mono.Git.Core.Type.Blob, b.Content.Length);
			
			//f.Write (bytesHeader, 0, bytesHeader.Length);
			
			// Reading the file content
			FileStream st = new FileStream (filePath, FileMode.Open);
			FileInfo fileInfo = new FileInfo (filePath);
			BinaryReader br = new BinaryReader (st);
			
			byte[] data = new byte[bytesHeader.Length + fileInfo.Length];
			bytesHeader.CopyTo (data, 0); 
			br.ReadBytes (Convert.ToInt32 (fileInfo.Length.ToString ())).CopyTo (data, bytesHeader.Length);
			
			//f.Write (data, 0, data.Length);
			
			MemoryStream ms = new MemoryStream ();
			DeflateStream ds = new DeflateStream (ms, CompressionMode.Compress, true);
			ds.Write (data, 0, data.Length);
			ds.Close ();
			
			Console.WriteLine ("Original size {0}, Compressed size {1}", data.Length, ms.Length);
			
			f.Write (ms.GetBuffer (), 0, ms.GetBuffer ().Length);
			
			f.Close ();
		}
		
		/// <summary>
		/// TODO: This is a test only
		/// </summary>
		/// <param name="filePath">
		/// A <see cref="System.String"/>
		/// </param>
		public static void Read (string filePath)
		{
			FileStream fs = new FileStream (filePath, FileMode.Open);
			BinaryReader br = new BinaryReader (fs);
			
			byte[] data = br.ReadBytes (Convert.ToInt32 (fs.Length));
			
			Console.WriteLine (Object.BytesToString (data));
		}
	}
}
