// FileMode.cs
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

namespace Mono.Git.Core
{
	/// <summary>
	/// Enumeration with all the file modes that git handles
	/// </summary>
//	public enum GitFileModeTypes
//	{
//		Tree,
//		Symlink,
//		RegularFile,
//		ExecutableFile,
//	}
	[Flags]
	public enum IndividualFileType
	{
		Zero = 0,
		File = 1,
		SymLink = 2,
		Directory = 4
	}
	
	[Flags]
	public enum IndividualFileMode
	{
		None = 0,
		Read = 4,
		Write = 2,
		Execute = 1,
		ReadExecute = 5,
		ReadWrite = 6,
		All = 7
	}
	
	public struct GitFileMode
	{
		private IndividualFileType file_type;
		private IndividualFileType sym_link;
		private IndividualFileType zero;
		private IndividualFileMode user;
		private IndividualFileMode group;
		private IndividualFileMode other;
		
		public GitFileMode (byte[] mode)
		{
			if ((char) mode[0] != '4') {
				file_type = (IndividualFileType) mode[0] - 48;
				sym_link = (IndividualFileType) mode[1] - 48;
				zero = (IndividualFileType) mode[2] - 48;
				user = (IndividualFileMode) mode[3] - 48;
				group = (IndividualFileMode) mode[4] - 48;
				other = (IndividualFileMode) mode[5] - 48;
			} else {
				file_type = (IndividualFileType) mode[0] - 48;
				sym_link = (IndividualFileType) mode[1] - 48;
				zero = IndividualFileType.Zero;
				user = (IndividualFileMode) mode[2] - 48;
				group = (IndividualFileMode) mode[3] - 48;
				other = (IndividualFileMode) mode[4] - 48;
			}
		}
		
		public byte[] ModeBits {
			get {
				if (file_type == IndividualFileType.Directory) {
					return new byte[] {(byte) (file_type + 48), (byte) (sym_link + 48), (byte) (user + 48), (byte) (group + 48), (byte) (other + 48)};
				}
				return new byte[] {(byte) (file_type + 48), (byte) (sym_link + 48), (byte) (zero + 48), (byte) (user + 48), (byte) (group + 48), (byte) (other + 48)};
			}
		}
		
		public override string ToString ()
		{
			if (file_type == IndividualFileType.Directory)
				return String.Format ("0{0}{1}{2}{3}{4}", (uint) file_type, (uint) sym_link, (uint) user, (uint) group, (uint) other);
			
			return String.Format ("{0}{1}{2}{3}{4}{5}", (uint) file_type, (uint) sym_link, (uint) zero, (uint) user, (uint) group, (uint) other);
		}
		
		public bool IsDirectory ()
		{
			return file_type == IndividualFileType.Directory;
		}
	}
}
