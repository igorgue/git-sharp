// Repository.cs
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

namespace Mono.Git.Core.Repository
{
	/// <summary>
	/// It represent all the repository data
	/// </summary>
	public class Repo
	{
		private Configuration config;
		private ObjectStore objects;
		private Index index_file;
		
		public string Path
		{
			set {
				config.ConfigPath = value;
			}
			get {
				return config.ConfigPath;
			}
		}
		
		public Configuration Config
		{
			set {
				config = value;
			}
			get {
				return config;
			}
		}
		
		public ObjectStore Objects
		{
			set {
				objects = value;
			}
			get {
				return objects;
			}
		}
		
		public Index IndexFile
		{
			set {
				index_file = value;
			}
			get {
				return index_file;
			}
		}
		
		public Repo ()
		{
		}
		
		public Repo (Configuration conf)
		{
			config = conf;
			//objects = new ObjectStore ();
		}
		
		/// <summary>
		/// Here we create a empty repository described here(in UNIX)
		/// /usr/share/git-core/templates
		/// </summary>
		public static void Init (string templatePath, string repoPath, string permissions)
		{
			bool reinit = false;
			DirectoryInfo target = new DirectoryInfo (repoPath);
			DirectoryInfo source = new DirectoryInfo (templatePath);
			
			if (target.Exists) {
				reinit = true;
				target.Delete (true); // we also delete subdirectories
			}
			
			target.Create ();
			
			CopyAll (source, target);
			
			if (reinit) {
				Console.WriteLine ("Reinitialized directory in {0}", repoPath);
			} else {
				Console.WriteLine ("Initialized directory in {0}", repoPath);
			}
		}
		
		public static void Init (Configuration config, string permissions)
		{
			Init (config.TemplatePath, config.ConfigPath, permissions);
		}
		
		/// <summary>
		/// Copy all the content of a directory to another
		/// </summary>
		/// <param name="source">
		/// A source directory<see cref="DirectoryInfo"/>
		/// </param>
		/// <param name="target">
		/// A target direcotory<see cref="DirectoryInfo"/>
		/// </param>
		public static void CopyAll (DirectoryInfo source, DirectoryInfo target)
		{
			// If the directory exists we create it
			if (target.Exists == false) {
				target.Create ();
			}
			
			// We copy each file from source to target
			foreach (FileInfo fi in source.GetFiles ()) {
				fi.CopyTo (System.IO.Path.Combine (target.ToString (), fi.Name), true);
			}
			
			// Copy subdirs using.
			foreach (DirectoryInfo sourceSubDir in source.GetDirectories ()) {
				DirectoryInfo nextTargetSubDir = target.CreateSubdirectory (sourceSubDir.Name);
				CopyAll (sourceSubDir, nextTargetSubDir);
			}
		}
	}
}
