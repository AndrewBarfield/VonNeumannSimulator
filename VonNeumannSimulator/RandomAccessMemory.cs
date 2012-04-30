
using System;
using System.IO;
using System.Text;



namespace VonNeumannSimulator
{

	/// <summary>
	/// CPU Memory
	/// </summary>
	public struct RandomAccessMemory
	{

		#region Fields

		/// <summary>
		/// 
		/// </summary>
		private int instructionCount;

		/// <summary>
		/// String array contains instructions/data.
		/// </summary>
		private string[] memoryCells;

		/// <summary>
		/// Boolean array that maintains status of its relative string array index
		/// </summary>
		private bool[] memoryCellUsed;

		#endregion



		#region Constructor

		/// <summary>
		/// Sets Memory to size specified, creating string array M[] and boolean array used[].
		/// Initial values are "7001" and false.
		/// </summary>
		/// <param name="n"></param>
		public RandomAccessMemory( int n )
		{

			// There are no instructions in memory
				this.instructionCount = 0;

			// Initialize each memory cell to the Halt instruction
				this.memoryCells = new string[n];
				for ( int i = 0 ; i < n ; i++ )
					memoryCells[i] = "7001";

			// Initialize used array
				this.memoryCellUsed = new bool[n];

		}

		#endregion



		#region Indexer

		/// <summary>
		/// Memory cell array
		/// </summary>
		/// <param name="index">Memory cell index</param>
		/// <returns>System.String containing instructions or data.</returns>
		public string this[int index]
		{

			get
			{

				return memoryCells[index];

			}

			set
			{

				memoryCells[index] = value;
				memoryCellUsed[index] = true;

			}

		}

		#endregion



		#region Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public int LoadFromFile( string fileName )
		{

			StreamReader programReader = null;
			int pcValue = -1;

			try
			{

				// Open the file for reading
					programReader = File.OpenText( fileName );

				// Initial hexadecimal value for the PC
					pcValue = System.Convert.ToInt32( programReader.ReadLine().Split( ' ' )[0], 16 );

				// Read and parse "Total number of memory words"
					if ( !Int32.TryParse( programReader.ReadLine().Split( ' ' )[0], out instructionCount ) )
						return -1;

				// Read each line and store value into memory location
					for ( int i = 0 ; i < instructionCount ; i++ )
					{

						string[] mem = programReader.ReadLine().Split( ' ' );
						int n = System.Convert.ToInt32( mem[0], 16 );
						memoryCells[n] = mem[1];
						memoryCellUsed[n] = true;

					}

			}
			catch
			{

				pcValue = -1;

			}
			finally
			{


				if ( programReader != null )
				{

					programReader.Close();
					programReader = null;

				}

			}

			return pcValue;
		}


		/// <summary>
		/// Converts this RandomAccessMemory structure to a human-readable string.
		/// </summary>
		public override string ToString()
		{

			StringBuilder sb = new StringBuilder( instructionCount );

			for ( int i = 0 ; i < memoryCells.Length ; i++ )
			{

				// OPTIMIZATION: "StringBuilder Mistake, Benchmark"
				// http://dotnetperls.com/Content/StringBuilder-Mistake.aspx

				if ( memoryCellUsed[i] )
					sb.Append( "\tMemory[" ).Append( i ).Append( "] = " ).Append( "\"" ).Append( memoryCells[i] ).Append( "\"\n" );

			}

			return sb.ToString();

		}

		#endregion

	}

}
