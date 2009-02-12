// TreeEntry.cs
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
using System.Text;
using Mono.Git.Core;
using Mono.Git.Core.Repository;

namespace Mono.Git.Core
{
	/// <summary>
	/// Every Tree has entries, which are more trees and blobs, this is a
	/// representation of a tree entry that can be another tree or a blob
	/// </summary>
	public struct TreeEntry
	{
		private string name;
		private GitFileMode mode;
		private SHA1 id;
		
		public string Name { get { return name; } }
		public byte[] Mode { get { return mode.ModeBits; } }
		public SHA1 Id { get { return id; } }
		
		public TreeEntry (byte[] mode, string name, SHA1 id)
		{
			if (id.Bytes.Length != 20)
				throw new ArgumentException (String.Format
				                             ("The size of the SHA1 is incorrect, tree entry is invalid has to be 20 and is: ", 
				                              id.Bytes.Length));
			
			if (String.IsNullOrEmpty (name))
				throw new ArgumentException ("Name is null, tree entry is invalid");
			
			if (mode.Length < 5)
				throw new ArgumentException ("mode size is incorrect, tree entry is invalid, size has to be longer than 5 or 5");
			
			this.mode = new GitFileMode (mode);
			this.name = name;
			this.id = id;
		}
		
		public override string ToString ()
		{
			return String.Format ("{0} {1} {2}\t{3}", mode, GetGitType (), id.ToHexString (), name);
		}
		
		/// <summary>
		/// Blob or Tree?
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		private string GetGitType ()
		{
			return mode.ModeBits[0] == (byte) 52 ? "tree" : "blob";
		}
		
		public bool IsTree ()
		{
			return mode.IsDirectory ();
		}
	}
}
