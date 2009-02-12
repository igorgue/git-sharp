// IndexEntry.cs
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

namespace Mono.Git.Core
{
	public class IndexEntry
	{
		private int ctime; // TODO: A conversion method to handle this
		private int mtime; // TODO: A conversion method to handle this
		private int dev;
		private int ino; // Inode
		private int mode; // Create mode(umask)
		private int uid; // we don't really need this(jgit has as trick for this)
		private int gid; // apply jgit trick here too
		private int size;
		private bool modified; // merged or not?
		private SHA1 id; // ID
		private string name; // whole path
		
		public int Ctime { get; set; }
		public int Mtime { get; set; }
		public int Dev { get; set; }
		public int Ino { get; set; }
		public int Mode { get; set; }
		public int Uid { get; set; }
		public int Gid { get; set; }
		public int Size { get; set; }
		public bool Modified { get; set; }
		public SHA1 Id { get; set; }
		public string Name { get; set; }
		
		public IndexEntry ()
		{
		}
	}
}
