// Index.cs
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
using System.Collections;
using System.IO;
using System.Text;
using Mono.Git.Core.Repository;

namespace Mono.Git.Core
{
	/// <summary>
	/// Merge stage enumeration
	/// 
	/// Normal: It Doesn't need any merge (0)
	/// Ancestor: Something is not ok(needs merge) (1)
	/// Our: Our version (2)
	/// Their: Their version (3)
	/// 
	/// A fully merged stage is always 0(Normal)
	/// </summary>
	public enum IndexStage
	{
		Normal,
		Ancestor,
		Our,
		Their
	}
	
	public class Index
	{
		private IndexHeader header;
		private IndexEntry[] entries; // Index Entries
		private uint stage; // Merged in the constructor
		private FileStream index_file;
		private bool changed;
		
		public IndexEntry[] Entries
		{
			set {
				entries = value;
			}
			get {
				return entries;
			}
		}
		
		public uint Stage
		{
			set {
				stage = value;
			}
			get {
				return stage;
			}
		}
		
		public FileStream IndexFile
		{
			set {
				index_file = value;
			}
			get {
				return index_file;
			}
		}
		
		public bool Changed
		{
			set {
				changed = value;
			}
			get {
				return changed;
			}
		}
		
		public IndexHeader Header
		{
			set {
				header = value;
			}
			get {
				return header;
			}
		}
		
		public Index ()
		{
			Init ();
		}
		
		public Index (string indexFilePath)
		{
			// Recreate the index file if it exist
			if (File.Exists (indexFilePath))
				File.Create (indexFilePath);
			
			Init ();
		}
		
		public void Init ()
		{
			// FIXME: Here we should add the entries before using them
			//header = new IndexHeader (entries);
			stage = (uint) IndexStage.Normal;
		}
		
		public void AddEntry (IndexEntry indexEntry)
		{
			ArrayList array = new ArrayList (entries);
			
			array.Add (indexEntry);
			
			entries = (IndexEntry[]) array.ToArray ();
		}
		
		public void RemoveEntry (IndexEntry indexEntry)
		{
			ArrayList array = new ArrayList (entries);
			
			array.Remove (indexEntry);
			
			entries = (IndexEntry[]) array.ToArray ();
		}
		
		public void ReadTree (Tree tree, Repo repo)
		{
			ReadTree ("", tree, repo);
		}
		
		public void ReadTree (string prefix, Tree tree, Repo repo)
		{
			TreeEntry[] treeEntries = tree.Entries;
			
			foreach (TreeEntry te in treeEntries) {
				string name;
				
				if (!String.IsNullOrEmpty (prefix))
					name = String.Format ("{0}/{1}", prefix, te.Name);
				else
					name = te.Name;
				
//				if (te.IsTree (repo))
//					ReadTree (name, tree, repo);
			}
		}
		
		public static IndexHeader GetHeader (BinaryReader br)
		{
			IndexHeader indexHeader = new IndexHeader ();
			
			indexHeader.Signature = br.ReadInt32 ();
			indexHeader.Version = br.ReadInt32 ();
			indexHeader.Entries = br.ReadInt32 ();
			
			//TODO:
			return indexHeader;
		}
		
		public static IndexEntry GetNextEntry (BinaryReader br)
		{
			IndexEntry entry = new IndexEntry ();
			
			// Content
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			
			// SHA1 5x32 = 160
			//??? Console.WriteLine (Mono.Git.Core.Object.BytesToHexString (br.ReadBytes (20)));
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			
			//entry = br.ReadInt16 (); // flag
			Console.Write ("Characters: ");
			
			for (;;) {
				char c = br.ReadChar ();
				if (c == '\0')
					break;
				Console.Write (c);
			}
			
			Console.Write ('\n');
			
			return entry;
		}
	}
}
