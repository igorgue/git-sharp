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

namespace Mono.Git.Core.Repository
{
	/// <summary>
	/// It holds all the user configuration
	/// </summary>
	public class Configuration : IConfiguration
	{
		private string head;
		private string description;
		private string path;
		private string template_path;
		private ConfigValue[] values;
		
		public string Description
		{
			set {
				description = value;
			}
			get {
				return description;
			}
		}
		
		public string Head
		{
			set {
				head = value;
			}
			get {
				return head;
			}
		}
		
		public string ConfigPath
		{
			set {
				path = value;
			}
			get {
				return path;
			}
		}
		
		public string TemplatePath
		{
			set {
				template_path = value;
			}
			get {
				return template_path;
			}
		}
		
		public ConfigValue[] Values
		{
			set {
				values = value;
			}
			get {
				return values;
			}
		}
		
		public Configuration ()
		{
		}
		
		public void AddConfigValues (string configPath)
		{
			path = configPath;
			AddConfigValues ();
		}
		
		public void AddConfigValues ()
		{
			string line = null;
			string section = null;
			
			TextReader tr = new StreamReader (path);
			
			while ((line = tr.ReadLine()) != null) {
				if (line.StartsWith ("[") && line.EndsWith ("]")) {
					section = line;
					line = tr.ReadLine ();
				}
				
				// TODO: for now we are going to read the value and name as a whole
				// the idea is:
				//
				// Remove all the text after "="(and "=" too) that's the name
				// Remove all the text before "="(and "=" too) that's the value 
				ConfigValue configValue = new ConfigValue (section, line, line);
				
				AddConfigValue (configValue);
			}
			
			tr.Close ();
		}
		
		public void AddConfigValue (string configSection, string configName, string configValue)
		{
			ConfigValue confValue = new ConfigValue (configSection, configName, configValue);
			AddConfigValue (confValue);
		}
		
		public void AddConfigValue (ConfigValue val)
		{
			if (values == null) {
				values = new ConfigValue[0];
				values[0] = val;
			} else {
				ConfigValue[] tmp = new ConfigValue[values.Length + 1];
				
				for (int i = 0; i < values.Length; i++)
					tmp[i] = values[i];
					
				tmp[tmp.Length - 1] = new ConfigValue();
				tmp[tmp.Length - 1] = val;
				values = tmp;
			}
			
			//ArrayList valuesArray = new ArrayList (values);
			
			//valuesArray.Add (val);
			
			//values = (ConfigValue[]) valuesArray.ToArray ();
			//values[0] = val;
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
