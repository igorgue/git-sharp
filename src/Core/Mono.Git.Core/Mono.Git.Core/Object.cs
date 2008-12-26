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
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
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
	/// Class that holds the basic object information
	/// </summary>
	public abstract class Object
	{
		private SHA1 id;
		// it represent the object uncompressed content
		private byte[] content;
		
		public abstract Type Type { get; }
		
		public SHA1 Id { get { return id; } }
		public byte[] Content { get { return content; } }
		
		public Object (Type type, byte[] content)
		{
			// FIXME: ok, if we got a 0 length content(a blob), what we can do?
			if (content.Length == 0 || content == null)
				return;
			
			byte[] header = CreateObjectHeader (type, content.Length.ToString ());
			this.content = new byte[header.Length + content.Length];
			
			// filling the content
			Array.Copy (header, 0, this.content, 0, header.Length); // Copying the header first
			Array.Copy (content, 0, this.content, header.Length, content.Length); // then the data
			
			Console.WriteLine ("Length: " + this.content.Length);
			foreach (char c in this.content) {
				if (c == '\n') {
					Console.WriteLine ("\\n");
					continue;
				}
				if (c == '\0')
					Console.Write ("[NULL]");
				Console.Write (c);
			}
			
			// initializing the id with the content
			id = new SHA1 (this.content, true);
			
			Console.WriteLine ("ID: " + id.ToHexString ());
		}
		
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
		public static byte[] CreateObjectHeader (Type objType, string dataSize)
		{
			return Encoding.UTF8.GetBytes (String.Format ("{0} {1}\0",
			                                                 Object.TypeToString (objType),
			                                                 dataSize));
		}
		
		// These methods are to parse different objects from a input byte array
		
		protected static bool ParseNewLine (byte[] input, ref int pos)
		{
			if (input.Length == pos)
				return false;
			
			if ((char)input[pos++] == '\n')
				return true;
			else {
				pos--;
				return false;
			}
		}
		
		protected static bool ParseZero (byte[] input, ref int pos)
		{
			if (input.Length == pos)
				return false;
			
			if ((char)input[pos++] == '\0')
				return true;
			else {
				pos--;
				return false;
			}
		}
		
		protected static bool ParseSpace (byte[] input, ref int pos)
		{
			if (input.Length == pos)
				return false;
			
			if ((char)input[pos++] == ' ')
				return true;
			else {
				pos--;
				return false;
			}
		}
		
		protected static bool ParseString (byte[] input, ref int pos, out string str)
		{
			int index = pos;
			str = null;
			
			if (input.Length == pos)
				return false;
			
			while ((input [index] != (byte)'\n') && (input [index] != (byte)0))
			{
				if (index < input.Length) {
					index++;
				} else {
					return false;
				}
			}
			
			int length = index - pos;
			if (length == 0)
			{
				return false;
			}
			
			str = Encoding.UTF8.GetString (input, pos, length);
			pos = index;
			
 			return true;
		}
		
		protected static bool ParseNoSpaceString (byte[] input, ref int pos, out string str)
		{
			int index = pos;
			str = null;
			
			if (input.Length == pos)
				return false;
			
			while ((input [index] != (byte)'\n') && (input [index] != (byte)0) && (input [index] != (byte)' '))
			{
				if (index < input.Length) {
					index++;
				} else {
					return false;
				}
			}
			
			int length = index - pos;
			if (length == 0)
			{
				return false;
			}
			
			str = Encoding.UTF8.GetString (input, pos, length);
			pos = index;
			
 			return true;
		}
		
		protected static bool ParseHeader (byte[] input, ref int pos, out Type type, out string length)
		{
			// FIXME: I'm getting an error if I don't asign these parameters, this is because
			// I get out the method before asigned anything to those parameters
			length = null;
			//type = Type.Blob;
			
			// Here I get out of the method
			if (!ParseType (input, ref pos, out type))
				return false;
			
			if (!ParseNoSpaceString (input, ref pos, out length))
				return false;
			
			// The next byte is null, so we skip it
			pos++;
			
			return true;
		}
		
		protected static bool ParseType (byte[] input, ref int pos, out Type type)
		{
			string decodedType;
			type = Type.Blob; // we need a default
			
			if (pos != 0)
				return false;
			
			if (!ParseNoSpaceString (input, ref pos, out decodedType))
				return false;
			
			pos ++;
			
			switch (decodedType) {
			case "blob":
				type = Type.Blob;
				return true;
			case "tree":
				type = Type.Tree;
				return true;
			case "commit":
				type = Type.Commit;
				return true;
				break;
			case "tag":
				type = Type.Tag;
				return true;
				break;
			}
			
			return true;
		}
		
		protected static bool ParseInteger (byte[] input, ref int pos, out int integer)
		{
			integer = 0;
			
			if (pos >= (input.Length - 4))
				return false;
			
			integer += ((int)input[pos++]) | (((int)input[pos++]) << 8) | (((int)input[pos++]) << 16) | (((int)input[pos++]) << 24);
			
			// Here you added pos += 4, why? I don't see the point on doing that since you incremented it in the pos++
			
			return true;
		}
		
		protected static bool ParseTreeEntry (byte[] input, ref int pos, out byte[] mode, out string name, out byte[] id)
		{
			if ((char) input[pos] == '4') {
				//Console.WriteLine ("mode = new byte [5];");
				mode = new byte [5];
			} else {
				//Console.WriteLine ("mode = new byte [6];");
				mode = new byte [6];
			}
			
			id = new byte [20];
			name = null;
			
			if (input.Length <= 27)
				throw new ArgumentException ("The data is not a tree entry, the size is to small");
			
			Array.Copy (input, pos, mode, 0, mode.Length);
			//Console.WriteLine (new GitFileMode (mode).ToString ());
			pos += (mode.Length + 1);
			
			if (!ParseString (input, ref pos, out name))
				return false;
			
			pos++;
			
			Array.Copy (input, pos, id, 0, 20);
			
			pos += 19;
			
			return true;
		}
		
		protected static void EncodeString (ref MemoryStream ms, string str)
		{
			BinaryWriter bw = new BinaryWriter (ms);
			bw.Write (Encoding.UTF8.GetBytes (str));
			
			bw.Close ();
		}
		
		protected static void EncodeHeader (ref MemoryStream ms, Type type, string length)
		{
			EncodeHeader (ref ms, CreateObjectHeader (type, length));
		}
		
		protected static void EncodeTreeEntry (ref MemoryStream ms, byte[] mode, string name, byte[] id)
		{
			BinaryWriter bw = new BinaryWriter (ms);
			
			bw.Write (mode);
			EncodeSpace (ref ms);
			bw.Write (name);
			
			bw.Write (id);
		}
		
		protected static void EncodeHeader (ref MemoryStream ms, byte[] header)
		{
			BinaryWriter bw = new BinaryWriter (ms);
			bw.Write (header);
			bw.Close ();
		}
		
		protected static void EncodeInteger (ref MemoryStream ms, int integer)
		{
			BinaryWriter bw = new BinaryWriter (ms);
			bw.Write (integer);
			
			bw.Close ();
		}
		
		protected static void EncodeZero (ref MemoryStream ms)
		{
			ms.WriteByte ((byte)'\0');
		}
		
		protected static void EncodeNewLine (ref MemoryStream ms)
		{
			ms.WriteByte ((byte)'\n');
		}
		
		protected static void EncodeSpace (ref MemoryStream ms)
		{
			ms.WriteByte ((byte)' ');
		}
		
		/// <summary>
		/// Convert a type to its string representation
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		public static string TypeToString (Type type)
		{
			switch (type) {
			case Type.Blob:
				return "blob";
			case Type.Tree:
				return "tree";
			case Type.Tag:
				return "tag";
			case Type.Commit:
				return "commit";
			}
			
			return "blob";
		}
		
		public static Type StringToType (string type)
		{
			switch (type) {
			case "blob":
				return Mono.Git.Core.Type.Blob;
				break;
			case "tree":
				return Mono.Git.Core.Type.Tree;
				break;
			case "commit":
				return Mono.Git.Core.Type.Commit;
				break;
			case "tag":
				return Mono.Git.Core.Type.Tag;
				break;
			}
			
			return Mono.Git.Core.Type.Blob;
		}
		
		public static Object DecodeObject (byte[] content)
		{
			Type type;
			string length;
			int pos = 0;
			
			ParseHeader (content, ref pos, out type, out length);
			
			byte[] objectContent = new byte[(content.Length) - pos];
			Array.Copy (content, pos, objectContent, 0, objectContent.Length);
			
			switch (type) {
			case Type.Blob:
				return new Blob (objectContent);
			case Type.Tree:
				return new Tree (objectContent);
				break;
			case Type.Tag:
				//TOOD: return new Tag (contents);
				break;
			case Type.Commit:
				//TODO: return new Commit (contents);
				break;
			}
			
			// To ensure that all code paths returns an object
			// anyway this code will never be reached
			return new Blob (objectContent);
		}
		
		protected abstract byte[] Decode ();
		protected abstract void Encode (byte[] data);
	}
}
