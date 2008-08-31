// Main.cs
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

namespace Mono.Git.Tests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length != 1) {
				Console.WriteLine ("No argument provided");
				return;
			}
			
			switch (args[0]) {
			case "blob":
				new BlobTest ("a7efe6dbed6a3a0d73030134521c8b1048e6a7");
				break;
			case "repo":
//				new RepositoryTest ();
//				RepositoryTest.ConfigurationTests ();
				break;
			case "index":
				new IndexTest ();
				IndexTest.IndexTest1 ();
				break;
			case "bytes":
				ObjectTest.ByteWriterTest ();
				break;
			case "read":
				ObjectTest.ReadGitObj ();
				break;
			case "tree":
				ObjectTest.ReadGitTree ();
				break;
			case "mode":
				ObjectTest.TestFileSystemModes ();
				break;
			case "dir":
				ObjectTest.ReadDirectories ("/home/igor/gsoc/Git/Tests/bin/Debug");
				break;
			case "checkout1":
				ObjectTest.CheckoutTest ("ffa5153020bab4598f676c5b2f7df97bc582989d", "/home/igor/gsoc/Git/Tests/bin/Debug/test/.git/objects");
				break;
			case "checkout2":
				// we try with a big git repo(C Git) but since we don't support pack,
				// we can't really run this test :(
				ObjectTest.CheckoutTest ("4a7bd524b0f232513255daf7d1f9b430405749e8", "/home/igor/gsoc/Git/Tests/bin/Debug/test2/.git/objects");
				break;
			case "ls-tree1":
				ObjectTest.LsTreeTest ("ffa5153020bab4598f676c5b2f7df97bc582989d", "/home/igor/gsoc/Git/Tests/bin/Debug/test/.git/objects");
				break;
			case "ls-tree2":
				ObjectTest.LsTreeTest ("4a7bd524b0f232513255daf7d1f9b430405749e8", "/home/igor/gsoc/Git/Tests/bin/Debug/test2/.git/objects");
				break;
			case "checkinobject":
				ObjectTest.CheckinObject ("/home/igor/gsoc/Git/Tests/bin/Debug/test3/test.cs", "/home/igor/gsoc/Git/Tests/bin/Debug/test3/.git/objects");
				break;
			case "viewobj":
				ObjectTest.ViewCompressedFile ("/home/igor/gsoc/Git/Tests/bin/Debug/test3/.git/objects/d9/a0bfec6aa9ecce447f6c7eec21025ce3437449");
				//ObjectTest.ViewCompressedFile ("/home/igor/gsoc/Git/Tests/bin/Debug/test3/.git/objects/3c/ebb761d58bfbbf98823e917a44857ca17ba1bb");
				break;
			}
		}
	}
}