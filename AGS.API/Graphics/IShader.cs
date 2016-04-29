using System;

namespace AGS.API
{
	public interface IShader
	{
		IShader Compile();
		void Bind();
		void Unbind();
		bool SetVariable(string name, float x);
		bool SetVariable(string name, float x, float y);
		bool SetVariable(string name, float x, float y, float z);
		bool SetVariable(string name, float x, float y, float z, float w);
		bool SetVariable(string name, Color c);
	}
}

