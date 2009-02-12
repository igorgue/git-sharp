// Tree.cs
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
using System.Collections;
using System.Collections.Generic;
using Mono.Git.Core;

namespace Mono.Git.Core
{
	/// <summary>
	/// A class that stores a tree of a commit
	/// </summary>
	public class Tree : Object
	{
		private TreeEntry[] entries;
		
		public override Type Type { get { return Type.Tree; } }
		public TreeEntry[] Entries { get{ return entries; } }
		
		/// <summary>
		/// We need to create a empty tree to "test"
		/// </summary>
		public Tree () : base (Type.Tree, null)
		{
			//
		}
		
		public Tree (byte[] content) : base (Type.Tree, content) // TODO: add a real encoding
		{
			if (content == null)
				throw new ArgumentException ("The content provided is null");
			
			// iterate over the whole array
			for (int i = 0; i < content.Length; i++) {
				//Console.WriteLine ("uno: {0}, dos: {1}, tres: {2}, cuatro: {3}, cinco: {4}, seis: {5}", (char)content[i], (char)content[i + 1], (char)content[i + 2], (char)content[i + 3], (char)content[i + 4], (char)content[i + 5]);
				
				byte[] mode = new byte[6];
				byte[] id = new byte[20];
				string name;
				
				ParseTreeEntry (content, ref i, out mode, out name, out id);
				
//				Console.WriteLine ("we parse i = " + i);
//				
//				Console.WriteLine ("mode: " + new GitFileMode (mode));
//				Console.WriteLine ("name: " + name);
//				Console.WriteLine ("id: " + new SHA1 (id, false).ToHexString ());
				
				//string lineRead = Console.ReadLine ();
				
//				if (lineRead.Contains ("q"))
//					break;
				
				AddEntry (mode, name, new SHA1 (id, false));
			}
		}
		
		private void AddEntry (byte[] data)
		{
			if (data.Length <= 27)
				throw new ArgumentException ("The data is not a tree entry, the size is to small");
			
			int pos = 0;
			byte[] mode;
			byte[] id;
			string name;
			
			ParseTreeEntry (data, ref pos, out mode, out name, out id);
			
			Console.WriteLine ("Name: " + name);
			
			AddEntry (mode, name, new SHA1 (id, false));
		}
		
		private void AddEntry (byte[] mode, string name, SHA1 id)
		{
			if (entries == null) {
				entries = new TreeEntry[0];
			}
			
			List<TreeEntry> entriesList = new List<TreeEntry> (entries);
			TreeEntry entry = new TreeEntry (mode, name, id);
			
			entriesList.Add (new TreeEntry (mode, name, id));
			
			entries = entriesList.ToArray ();
		}
		
		private TreeEntry GetTreeEntry (string name)
		{
			foreach (TreeEntry te in entries) {
				if (te.Name.Equals (name)) {
					return te;
				}
			}
			
			throw new ArgumentException ("entry: {0} was not found", name);
		}
		
		private bool RemoveEntry (string name)
		{
			List<TreeEntry> entriesList = new List<TreeEntry> (entries);
			
			TreeEntry entry = GetTreeEntry (name);
			
			if (entriesList.Remove (entry)){
				entries = entriesList.ToArray ();
				return true;
			} else {
				return false;
			}
		}
		
		public void SortEnties ()
		{
			throw new NotImplementedException ("Sort tree entries is not implemented yet");
		}
		
		/// <summary>
		/// This means that is a tree that doesn't cotains children trees
		/// </summary>
		/// <param name="tree">
		/// A <see cref="Tree"/>
		/// </param>
		/// <param name="store">
		/// A <see cref="ObjectStore"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		protected static bool IsLastChild (Tree tree, ObjectStore store)
		{
			foreach (TreeEntry entry in tree.Entries) {
				if (store.Get (entry.Id).Type == Type.Tree)
					return false;
			}
			
			return true;
		}
		
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder (entries.Length);
			for (int i = 0; i < entries.Length; i++)
				sb.Append (String.Format ("{0}\n", entries[i]));
			
			return sb.ToString ();
		}
		
		protected override byte[] Decode ()
		{
			throw new NotImplementedException ();
		}
		
		protected override void Encode (byte[] content)
		{
			throw new NotImplementedException ();
		}
	}
}
