
using System;
using System.Text;



namespace VonNeumannSimulator
{

	/// <summary>
	/// Represents a CPU Register
	/// </summary>
	public struct Register
	{

		#region Fields

		/// <summary>
		/// Stores a decimal representation of the content
		/// </summary>
		private int decimalVal;

		/// <summary>
		/// The number of bits in the register
		/// </summary>
		private byte size;

		#endregion



		#region Constructor

		/// <summary>
		/// Constructs a register with n-bits.
		/// </summary>
		/// <param name="n">Indicates the maximum number of bits in this register.</param>
		public Register( byte n )
		{
			
			size = n;
			decimalVal = 0;

		}

		#endregion



		#region Properties

		/// <summary>
		/// Gets or sets the register value using an integer.
		/// </summary>
		public int Value
		{

			get
			{

				if ( size == 8 )
				{

					decimalVal &= (int)~( 0xFFFFFF00 );
					return (byte)decimalVal;

				}
				else if ( size == 12 )
				{

					decimalVal &= (int)~( 0xFFFFF000 );
					return (Int16)decimalVal;

				}
				else
				{

					decimalVal &= (int)~( 0xFFFF0000 );
					return (Int16)decimalVal;

				}

			}

			set
			{

				if ( size == 8 )
				{

					decimalVal = (byte)value;

				}
				else if (size == 12)
				{

					decimalVal = (Int16)value;

				}
				else if ( size == 16 )
				{

					decimalVal = (Int16)value;

				}

				

			}

		}

		/// <summary>
		/// Gets or sets the register value using a Hexadecimal string.
		/// </summary>
		public string HexStringValue
		{
			get
			{

				string hex = this.Value.ToString( "X" );


				// Truncate high order nybbles if hex larger than register size
					byte NybbleCount = (byte)( size / 4 );
					if ( hex.Length > NybbleCount )
						hex = hex.Substring( hex.Length - NybbleCount, NybbleCount );


				// Pad high order with zeros if hex smaller than register size
					byte NybbleDifference = (byte)( NybbleCount - hex.Length );
					if ( NybbleDifference > 0 )
						return new string( '0', NybbleDifference ) + hex;


				return hex;

			}

			set
			{

				this.Value = System.Convert.ToInt32( value, 16 );

			}
		}

		#endregion



		#region Methods


		/// <summary>
		/// The result of a unary + operation on a numeric type is just the value of the operand.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		[Help( "ms-help://MS.LHSMSSDK.1033/MS.LHSNETFX30SDK.1033/dv_csref/html/93e56486-bb42-43c1-bd43-60af11e64e67.htm" )]
		public static int operator +( Register a )
		{
			return a.Value;
		}



		/// <summary>
		/// The increment operator (++) increments its operand by 1.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		[Help( "ms-help://MS.LHSMSSDK.1033/MS.LHSNETFX30SDK.1033/dv_csref/html/e9dec353-070b-44fb-98ed-eb8fdf753feb.htm" )]
		public static Register operator ++( Register a )
		{

			a.Value++;
			return a;

		}


		/// <summary>
		/// Converts the value of this instance to a System.String
		/// </summary>
		/// <returns>A System.String representation of this instance</returns>
		public override string ToString()
		{

			// Accumulates bits for this register's binary representation
			StringBuilder bits = new StringBuilder();


			// Get mask from register size
			long mask = (long)Math.Pow( 2, size - 1 );


			// Iterate each bit and append it to the string
			while ( mask > 0 )
			{
				bits.Append( ( ( this.Value & mask ) == 0 ) ? '0' : '1' );
				mask >>= 1;
			}


			// Add spacing between nybbles
			for ( int i = bits.Length - 4 ; i > 0 ; i -= 4 )
				bits.Insert( i, ' ' );


			return String.Format( "{0,4} {1,21}  {2}", this.HexStringValue, bits.ToString(), this.Value );

		}

		#endregion

	}
}
