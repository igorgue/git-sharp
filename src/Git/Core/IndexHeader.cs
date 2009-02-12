// IndexHeader.cs
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

namespace Mono.Git.Core
{
	public class IndexHeader
	{
		private int signature;
		private int version;
		private int entries;
		
		public int Signature {
			get;
			set;
		}
		
		public int Version {
			get;
			set;
		}
		
		public int Entries
		{
			get;
			set;
		}
		
		public IndexHeader ()
		{
			signature = 0x44495243;
			version = 2;
			//entries = indexEntries.Length;
		}
		
		public IndexHeader (IndexEntry[] indexEntries)
		{
			signature = 0x44495243;
			version = 2;
			entries = indexEntries.Length;
		}
		
		public FileStream Write (FileStream fs)
		{
			BinaryWriter bw = new BinaryWriter (fs);
			
			bw.Write (signature);
			bw.Write (version);
			bw.Write (entries);
			
			return fs;
		}
	}
}
