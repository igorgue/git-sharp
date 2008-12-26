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
using System.Text;
using Mono.Git.Core.Repository;

namespace Mono.Git.Core
{
	/// <summary>
	/// Class that store a Blob
	/// </summary>
	public class Blob : Object
	{
		private byte[] data;
		public byte[] Data { get { return data; } }
		
		public override Type Type { get { return Type.Blob; } }
		
		public Blob (byte[] data) : base (Type.Blob, data)
		{
			this.data = data;
		}
		
		public bool IsText ()
		{
			// FIXME: Very silly implementation :(
			foreach (char c in data) {
				if (c == ' ' || c == '\n')
					continue;
				if (Char.IsLetterOrDigit (c) || Char.IsPunctuation (c) || Char.IsSymbol (c))
					continue;
				
				return false;
			}
			
			return true;
		}
		
		public string GetText ()
		{
			if (IsText ())
				return Encoding.UTF8.GetString (data);
			
			throw new FieldAccessException ("The data in this blob is not text");
		}
		
		protected override byte[] Decode ()
		{
			return data;
		}
		
		protected override void Encode (byte[] data)
		{
			MemoryStream ms = new MemoryStream (data);
			
			byte[] header;
			int pos = 0;
			
			this.data = new byte[data.Length - pos];
			
			Array.Copy (data, pos - 1, this.data, 0, data.Length - pos);
		}
	}
}
