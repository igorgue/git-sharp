// IndexTest.cs
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
using Mono.Git.Core;

namespace Mono.Git.Tests
{
	
	
	public class IndexTest
	{
		
		public IndexTest ()
		{
		}
		
		public static void IndexTest1 ()
		{
			//Index index = new Index ();
			//IndexEntry iEntry = new IndexEntry ();
			
			FileStream fs = File.OpenRead (".git/index");
			BinaryReader br = new BinaryReader (fs);
			
			// HEADER <- I hate you!!!
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			Console.WriteLine (br.ReadInt32 ());
			
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
			//??? Console.WriteLine (Mono.Git.Core.Object.BytesToHexString (br.ReadBytes (160)));
			//Console.WriteLine (Mono.Git.Core.SHA1.BytesToHexString (br.ReadBytes (20)));
//			Console.WriteLine (br.ReadInt32 ());
//			Console.WriteLine (br.ReadInt32 ());
//			Console.WriteLine (br.ReadInt32 ());
//			Console.WriteLine (br.ReadInt32 ());
//			Console.WriteLine (br.ReadInt32 ());
			
			Console.WriteLine (br.ReadInt16 ()); // flag
			Console.Write ("Characters: ");
			
			for (;;) {
				char c = br.ReadChar ();
				if (c == '\0')
					break;
				Console.Write (c);
			}
			
			Console.Write ('\n');
			
			br.Close ();
			fs.Close ();
		}
	}
}
