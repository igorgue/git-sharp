// Configuration.cs
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

namespace Git.Repository
{
	/// <summary>
	/// It holds all the user configuration
	/// </summary>
	public class Configuration : IConfiguration
	{
		string head;
		string description;
		string path;
		string template;
		ConfigEntry[] entries;
		
		public string Head { get; set; }
		
		public string Description { get; set; }
		
		public string Path { get; set; }
		
		public string Template { get; set; }
		
		public ConfigEntry[] Entries {
			get {
				return entries;
			}
		}
		
		public Configuration (string head, string description, string path, string template)
		{
			// so if we don't pass null we asign it, if we do pass null or empty then we grab the default
			this.head = String.IsNullOrEmpty (head) ? GetDefaultHead () : head;
			this.description = String.IsNullOrEmpty (description) ? GetDefaultDescription () : description;
			this.path = String.IsNullOrEmpty (path) ? GetDefaultConfigDir () : path;
			this.template = String.IsNullOrEmpty (template) ? GetDefaultTemplateDir () : template;
			
			// doing some validations
			if (!Directory.Exists (this.path))
				throw new ArgumentException (String.Format ("The provided path ({0}) of the git config doesn't exist", this.path));
			
			if (!Directory.Exists (this.template))
				throw new ArgumentException (String.Format ("The provided path ({0}) of the git repo template doesn't exist", this.template));
		}
		
		// These methods are used to get default configurations from C Git(mostly in UNIX systems)
		public static string GetDefaultHead ()
		{
			return "ref: refs/heads/master";
		}
		
		public static string GetDefaultTemplateDir ()
		{
			// FIXME: So UNIX name, not right for multiplatform
			return "/usr/share/git-core/templates";
		}
		
		public static string GetDefaultConfigDir ()
		{
			return ".git";
		}
		
		public static string GetDefaultDescription ()
		{
			return "Unnamed repository; edit this file to name it for gitweb.";
		}
	}
}
