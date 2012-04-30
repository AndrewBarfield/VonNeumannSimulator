
using System;
using System.Diagnostics;
using System.IO;


namespace VonNeumannSimulator
{

	/// <summary>
	/// Entry point class for simulator
	/// </summary>
	public class Program
	{

		/// <summary>
		/// The main() method, declares, instantiates, and starts the CPU
		/// </summary>
		/// <param name="args"></param>
		static void Main( string[] args )
		{

			Console.WindowLeft = 0;
			Console.WindowTop = 0;

			Console.WindowWidth = 100;
			Console.BufferWidth = 100;

			Console.WindowHeight = 69;
			Console.BufferHeight = 5000;

			Console.Title = "VonNeumann Simulator";

                        
			if ( File.Exists( args[0] ) )
			{
				
				CPU c = new CPU( args[0] );

			}
			else
			{

				Console.WriteLine( "\nERROR: File \"{0}\" could not be found.", args[0] );

			}
                        

			// Wait for user
			Console.Read();

		}


	}
}
