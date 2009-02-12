// ConfigValue.cs
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

namespace Mono.Git.Core.Repository
{
	public class ConfigValue
	{
		private string config_section;
		private string config_name;
		private string config_value;
		
		public string Section
		{
			set {
				config_section = value;
			}
			get {
				return config_section;
			}
		}
		
		public string Value
		{
			set {
				config_value = value;
			}
			get {
				return config_value;
			}
		}
		
		public string Name
		{
			set {
				config_name = value;
			}
			get {
				return config_name;
			}
		}
		
		public ConfigValue ()
		{
		}
		
		public ConfigValue (string confSection, string confName, string confValue)
		{
			config_section = confSection;
			config_name = confName;
			config_value = confValue;
		}
	}
}
