// ObjectStore.cs
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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Mono.Git.Core
{
	/// <summary>
	/// This class has all the information about the object store tipicaly
	/// .git/objects
	/// </summary>
	public class ObjectStore
	{
		private Dictionary<SHA1, Object> cache;
		private List<Object> write_queue;
		private string path;
		
		// Zlib first two bytes, these are the ones we are going to use when writing,
		// those bytes(78 and 1) mean that we are in a 32bit environment and using the
		// fastest deflate algorithm
		private static readonly byte ZlibFirstByte = 78;
		private static readonly byte ZlibSecondByte = 1;
		
		public string Path { get { return path; } }
		
		public Object Get (string hexstring)
		{
			return Get (new SHA1 (SHA1.FromHexString (hexstring), false));
		}
		
		public Object Get (SHA1 key)
		{
			if (cache == null)
				cache = new Dictionary<SHA1,Object>();
			
			if (cache.ContainsKey (key))
				return cache[key];
			
			if (ObjectExist (key)) {
				cache.Add (key, RetrieveObject (key));
			} else {
				throw new ArgumentException (String.Format ("The specified key {0} does not exist", key.ToHexString ()));
			}
			
			return cache[key];
		}
		
		private bool ObjectExist (SHA1 id)
		{
			if (File.Exists (GetObjectFullPath (id)))
				return true;
			
			return false;
		}
		
		public ObjectStore (string path)
		{
			if (Directory.Exists (path)) {
				write_queue = new List<Object> ();
				this.path = System.IO.Path.GetFullPath (path);
			} else
				throw new ArgumentException ("Invalid provided path");
		}
		
		private string GetObjectFullPath (SHA1 id)
		{
			return path.EndsWith ("/") ? (path + id.GetGitFileName ()) : (path + "/" + id.GetGitFileName ());
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
			data = RemoveZlibSignature (data);
			
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
			data = AddZlibSignature (data);
			
			MemoryStream ms = new MemoryStream ();
			DeflateStream ds = new DeflateStream (ms, CompressionMode.Compress);
			
			ds.Write (data, 0, data.Length);
			BinaryWriter bw = new BinaryWriter (ms);
			
			bw.Write ((int) 1);
			bw.Write ((int) 2);
			bw.Write ((int) 3);
			bw.Write ((int) 4);
			
			ds.Flush ();
			ds.Close ();
			
			return AddZlibSignature (ms.ToArray ());
		}
		
		/// <summary>
		/// Problem: in order to compress with Zlib(C Git deflate library)
		/// we must add 2 bytes at the begining of this, those represent the type of
		/// algorithm used, it just simple extra data that just confused me for a while
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte"/>
		/// </returns>
		private static byte[] AddZlibSignature (byte[] data)
		{
			List<byte> list = new List<byte> (data);
			
			list.Insert (0, ZlibFirstByte);
			list.Insert (1, ZlibSecondByte);
			
			return list.ToArray ();
		}
		
		/// <summary>
		/// Problem: in order to decompress with Zlib(C Git deflate library)
		/// we must remove 2 bytes at the begining of this, those represent the type of
		/// algorithm used, it just simple extra data that just confused me for a while	
		/// <param name="data">
		/// A <see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte"/>
		/// </returns>
		private static byte[] RemoveZlibSignature (byte[] data)
		{
			List<byte> list = new List<byte> (data);
			
			list.RemoveRange (0, 2);
			
			return list.ToArray ();
		}
		
		/// <summary>
		/// Create a object first parent... e.g. f4/47d5573b70cf042d036bfa62f55cdefccd9909
		/// f4 is the parent we create in this method
		/// </summary>
		/// <param name="id">
		/// A <see cref="SHA1"/>
		/// </param>
		private void CreateObjectParent (SHA1 id)
		{
			string fullPath = null;
			
			if (!path.EndsWith ("/"))
				fullPath = path + "/" + id.ToHexString (0, 1);
			else
				fullPath = path + id.ToHexString (0, 1);
			
			if (!Directory.Exists (fullPath))
				Directory.CreateDirectory (fullPath);
		}
		
		private void WriteBuffer (SHA1 id, byte[] content)
		{
			try {
				CreateObjectParent (id);
				
				FileStream fs = File.Open (GetObjectFullPath (id), FileMode.CreateNew);
				BinaryWriter bw = new BinaryWriter (fs);
				
				bw.Write (Compress (content));
				
				bw.Close ();
				fs.Close ();
			} catch (Exception e) {
				Console.WriteLine ("The file object you are trying to write already exist");
			}
		}
		
		private void WriteBuffer (SHA1 id, byte[] content, bool overwrite)
		{
			if (overwrite) {
				File.Delete (GetObjectFullPath (id));
				WriteBuffer (id, content);
				return;
			}
			
			WriteBuffer (id, content);
		}
		
		private byte[] ReadBuffer (SHA1 id)
		{
			try {
				FileStream fs = File.Open (GetObjectFullPath (id), FileMode.Open);
				BinaryReader br = new BinaryReader (fs);
				
				// I declared this here to close the stream and reader
				byte[] data = Decompress (br.ReadBytes ((int)fs.Length));
				
				br.Close ();
				fs.Close ();
				
				return data;
			} catch (FieldAccessException e) {
				Console.WriteLine ("The file object you are trying to read does not exist {0}", e);
			}
			
			return null;
		}
		
		private void PersistObject (SHA1 id)
		{
			Object o = cache[id];
			
			FileStream fs = new FileStream(GetObjectFullPath (o.Id), FileMode.CreateNew, FileAccess.Write);
			BinaryWriter bw = new BinaryWriter (fs);
			
			bw.Write (Compress (o.Content));
			
			bw.Close ();
			fs.Close ();
		}
		
		private byte[] RetrieveContent (SHA1 id)
		{
			FileStream fs = new FileStream (GetObjectFullPath (id), FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader (fs);
			
			byte[] bytes = new byte[(int) fs.Length];
			fs.Read (bytes, 0, (int) fs.Length);
			bytes = Decompress (bytes);
			
			br.Close ();
			fs.Close ();
			
			return bytes;
		}
		
		private Object RetrieveObject (SHA1 id)
		{
			byte[] bytes = RetrieveContent (id);
			
			return Object.DecodeObject (bytes);
		}
		
		private void AddToQueue (Object o)
		{
			if (write_queue == null)
				write_queue = new List<Object> ();
			
			write_queue.Add (o);
		}
		
		/// <summary>
		/// This method write the queue content to the filesystem
		/// </summary>
		public void Write ()
		{
			if (write_queue.Count == 0) // means is empty thus we don't need to do anything
				return;
			
			// first we write every single object in the write_queue
			foreach (Object o in write_queue.ToArray ()) {
				WriteBuffer (o.Id, o.Content);
			}
			
			// now we clear the queue
			write_queue.Clear ();
		}
		
		public void LsTree (string hexstring)
		{
			Tree tree = (Tree) Get (hexstring);
			
			LsTree (tree);
		}
		
		private void LsTree (Tree tree)
		{
			Console.Write (tree);
		}
		
		/// <summary>
		/// Checkin a single object by using its path
		/// </summary>
		/// <param name="type">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="path">
		/// A <see cref="System.String"/>
		/// </param>
		public void Checkin (string path)
		{
			if (!File.Exists (path)) {
				Console.WriteLine ("File {0} doesn't exist", path);
				return;
			}
			
			// Its a tree
			if (Directory.Exists (path)) {
				throw new NotImplementedException ("This is not implemented yet");
				return;
			}
			
			// its a blob
			FileStream fs = File.Open (path, FileMode.Open, FileAccess.Read);
			byte[] content = new byte[(int) fs.Length];
			
			fs.Read (content, 0, (int) content.Length);
			fs.Close ();
			
			Blob blob = new Blob (content);
			
			AddToQueue (blob);
		}
		
		/// <summary>
		/// Get a commit and from there start the checkout
		/// </summary>
		/// <param name="gitDir">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="commit">
		/// A <see cref="Commit"/>
		/// </param>
		public void Checkout (string baseDir, Commit commit)
		{
			throw new NotImplementedException ("This is not yet implemented");
		}
		
		/// <summary>
		/// Get a tree and create the structure from there
		/// </summary>
		/// <param name="gitDir">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="tree">
		/// A <see cref="Tree"/>
		/// </param>
		public void Checkout (string baseDir, Tree tree)
		{
			for (int i = 0; i < tree.Entries.Length; i++) {
				string fullPath = baseDir + "/" + tree.Entries[i].Name;
				Console.WriteLine ("Entry: #{0} {1} {2}", i, tree.Entries[i].Name, tree.Entries[i].Id.ToHexString ());
				
				if (tree.Entries[i].IsTree ()) {
					if (!Directory.Exists (fullPath))
						Directory.CreateDirectory (fullPath);
					
					Checkout (fullPath, (Tree) Get (tree.Entries[i].Id));
					continue;
				}
				
				FileStream fs = new FileStream (fullPath, FileMode.Create, FileAccess.Write);
				Blob blobToWrite = (Blob) Get (tree.Entries[i].Id);
				
				fs.Write (blobToWrite.Data, 0, blobToWrite.Data.Length);
				
				// TODO: Set FileAttributes
				//File.SetAttributes ("", FileAttributes.
				
				fs.Close ();
			}
		}
	}
}
