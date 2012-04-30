
using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;



namespace VonNeumannSimulator
{

	/// <summary>
	/// Class representation of a Central Processing Unit
	/// </summary>
	public sealed partial class CPU
	{

		#region Fields

		/// <summary>
		/// 16-bit Accumulator Register
		/// </summary>
		private Register AC = new Register( 16 );

		/// <summary>
		/// 12-bit Address Register
		/// </summary>
		private Register AR = new Register( 12 );

		/// <summary>
		/// Decoded decimal value of opcode
		/// </summary>
		private int D = 0;

		/// <summary>
		/// 16-bit Data Register
		/// </summary>
		private Register DR = new Register( 16 );

		/// <summary>
		/// Flip-flop carry-out of ALU
		/// </summary>
		private int E = 0;
		
		/// <summary>
		/// Control flip-flop set to 1 when new information is available in the input device
		/// and is cleared to 0 when the information is accepted by the computer, synchronizes
		/// the timing rate difference between the input device and the computer.
		/// </summary>
		private int FGI = 0;
		
		/// <summary>
		/// Control flip-flop set to 1 when the output device is available to accept data,
		/// cleared to 0 while the output device is processing the data then set back to 1.
		/// </summary>
		private int FGO = 0;
	
		/// <summary>
		/// Flip-flop high order bit of instruction register
		/// </summary>
		private int I = 0;
		
		/// <summary>
		/// Flip-flop interrupt enable on (1)
		/// </summary>
		private int IEN = 0;

		/// <summary>
		/// 8-bit Input Register
		/// </summary>
		private Register INPR = new Register( 8 );
		
		/// <summary>
		/// 4 digit hex representation of 16-bit binary instruction
		/// </summary>
		private string instruction = String.Empty;
		
		/// <summary>
		/// 
		/// </summary>
		private string instructionType = String.Empty;

		
		/// <summary>
		/// Used to track interrupt count
		/// </summary>
                private int interruptCursor = 0;
		

		/// <summary>
		/// An int array with preset interrupt clock cycles
		/// </summary>
		private int[] interruptTime = new int[] { 23, 32, 112 };

		/// <summary>
		/// 16-bit Instruction Register
		/// </summary>
		private Register IR = new Register( 16 );

		/// <summary>
		/// Memory object 4 digit hex representing 16-bit words
		/// </summary>
		private RandomAccessMemory Memory = new RandomAccessMemory( 4096 );

		/// <summary>
		/// Prevents an inadvertant continuous loop condition
		/// </summary>
		private const int MAX_CLOCK_CYCLES = 600;

		/// <summary>
		/// 8-bit Output Register
		/// </summary>
		private Register OUTR = new Register( 8 );

		/// <summary>
		/// 12-bit Program Counter Register
		/// </summary>
		private Register PC = new Register( 12 );

		/// <summary>
		/// Flip-flop interrupt request flag set to 1 if IEN = 1 and FGI = 1
		/// </summary>
		private int R = 0;

		/// <summary>
		/// Flip-flop CPU start (1) flag
		/// </summary>
		private int S = 0;

		/// <summary>
		/// Clock cycle counter
		/// </summary>
		private int timeCC = 0;

		/// <summary>
		/// 16-bit Temporary Register
		/// </summary>
		private Register TR = new Register( 16 );

		/// <summary>
		/// A String array with preset INPR values: "A3", "B4", and "C5"
		/// </summary>
		private string[] valueForINPR = new string[] { "A3", "B4", "C5" };

		#endregion



		#region Constructor

		/// <summary>
		/// Constructs a CPU with 4096 16-bit words of memory
		/// </summary>
		/// <param name="fileName"></param>
		public CPU( string fileName )
		{

			// Load program file into memory and get initial PC value
				PC.Value = Memory.LoadFromFile( fileName );


			// Begin main loop
				if ( PC.Value != -1 )
				{

					// Start program file execution
						S = 1;
						showState( 6 );


					// Begin Main Loop
						while ( S != 0 )
						{

							fetch();

							if ( decode() )
							{

								execute();

								cfi();

								showState( 5 );

								if ( timeCC >= MAX_CLOCK_CYCLES )
									break;
							}
							else
							{

								Console.WriteLine( "\nERROR: Invalid instruction could not be decoded" );

							}
						}


					Console.WriteLine( "\nThe CPU Simulation has ended" );
				}
				else
				{
					Console.WriteLine( "ERROR: File \"{0}\" failed to load properly.", fileName );
				}


			// Clean Up
				S = 0;


		}

		#endregion



		#region Methods

		/// <summary>
		/// Places the contents of the PC into AR, then places the contents of memory pointed to by AR into the IR.
		/// </summary>
		private void fetch()
		{

			// R'T0:
			
				// AR <- PC;
					AR = PC;

				// T0 -> T1
					timeCC++;

			// R'T1:
			

				// IR <- M[AR]
					IR.HexStringValue = Memory[ +AR ];

				// PC <- PC + 1
					PC++;

				// T1 -> T2
					timeCC++;

		}


		/// <summary>
		/// Decode gets the binary string representation of IR
		/// </summary>
		private bool decode()
		{

			// Locals
				bool success = true;


			// Single method call for IR decimal value
				int IRVal = IR.Value;
				instructionType = String.Empty;
				instruction = String.Empty;


			// T2:

				// D <-- IR[14...12]
					D = ( IRVal & 0x07000 ) >> 12;

				// I <-- IR[15]
					I = ( IRVal & 0x8000 ) >> 15;
				
				// AR <- IR[11...0]
					AR.Value = IRVal & 0xFFF;

				// Update clock
					timeCC++;


			// 
				if ( D == 7 )
				{


					if ( I == 0 )
					{

						// Register Referencing Instruction
							instructionType = "Register Type";


						// Is the instruction valid?
							if ( SparseBitcount( AR.Value ) == 1 )
							{

								// Valid
								switch ( AR.Value )
								{
									case 2048:
										instruction = "CLA";
										break;

									case 1024:
										instruction = "CLE";
										break;

									case 512:
										instruction = "CMA";
										break;

									case 256:
										instruction = "CME";
										break;

									case 128:
										instruction = "CIR";
										break;

									case 64:
										instruction = "CIL";
										break;

									case 32:
										instruction = "INC";
										break;

									case 16:
										instruction = "SPA";
										break;

									case 8:
										instruction = "SNA";
										break;

									case 4:
										instruction = "SZA";
										break;

									case 2:
										instruction = "SZE";
										break;

									case 1:
										instruction = "HLT";
										break;

									default:
										instruction = "Unknown";
										S = 0;
										success = false;
										break;
								}



							}
							else
							{

								// Invalid 
									S = 0;
									success = false;
							}
					}
					else
					{

						// I/O Instruction
							instructionType = "I/O Type";

							switch ( AR.Value )
							{

								case 2048:
									instruction = "INP";
									break;

								case 1024:
									instruction = "OUT";
									break;

								case 512:
									instruction = "SKI";
									break;

								case 256:
									instruction = "SKO";
									break;

								case 128:
									instruction = "ION";
									break;

								case 64:
									instruction = "IOF";
									break;

                                                                case 32:
                                                                        instruction = "RTI";
                                                                        break;

								default:
									instruction = "Unknown";
									S = 0;
									success = false;
									break;
							}

					}


				}
				else
				{

					// Memory Referencing Instruction

						if ( I == 0 )
						{

							instructionType = "memory referencing--direct addressing mode";

						}
						else
						{

							instructionType = "memory referencing--indirect addressing mode";

							AR.Value = System.Convert.ToInt32( Memory[+AR], 16 ) & 0xFFF;
							timeCC++;

						}

					// 
						switch ( D )
						{
							case 0:
								instruction = "AND";
								break;

							case 1:
								instruction = "ADD";
								break;

							case 2:
								instruction = "LDA";
								break;

							case 3:
								instruction = "STA";
								break;

							case 4:
								instruction = "BUN";
								break;

							case 5:
								instruction = "BSA";
								break;

							case 6:
								instruction = "ISZ";
								break;

							default:
								instruction = "Unknown";
								S = 0;
								success = false;
								break;
						}
						

				}
			
				return success;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		static int SparseBitcount( int n )
		{
			int count = 0;
			while ( n != 0 )
			{
				count++;
				n &= ( n - 1 );
			}
			return count;
		}



		/// <summary>
		/// 
		/// </summary>
		private void execute()
		{

			// Use reflection to invoke the current instruction's associated method.
			// i.e., if instruction = "ADD" then the ADDInstruction() method will be invoked.

			this.GetType().InvokeMember(
				instruction + "Instruction",
				BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
				null,
				this,
				null );
		}


		/// <summary>
		/// 
		/// </summary>
		private void cfi()
		{

                        //Check for interrupts, if (IEN==1)&&(FGI==1), then R = 1
                                if ( IEN == 1 && FGI == 1 )
                                {

                                        R = 1;


                                        // RT0: AR<-0, TR<-PC
                                                AR.Value = 0;
                                                TR = PC;
                                                timeCC++;


                                        // RT1: M[AR]<-TR, PC<-0
                                                Memory[AR.Value] = TR.HexStringValue;
                                                PC.Value = 0;
                                                timeCC++;


                                        // RT2: PC<-PC+1, IEN<-0, R<-0
                                                PC++;
                                                IEN = 0;
                                                R = 0;
                                                timeCC++;


                                }

                                if ( interruptCursor < interruptTime.Length )
                                {

                                        // Simulate Input...
                                        if ( timeCC >= interruptTime[interruptCursor] && FGI == 0 )
                                        {

                                                // Copy the value into the INPR
                                                INPR.HexStringValue = valueForINPR[interruptCursor];

                                                // Set FGI=1
                                                FGI = 1;

                                                // Advance the script cursor (to the next request)
                                                interruptCursor++;

                                        }

                                }

		}

		/// <summary>
		/// Shows state of the machine based on requested detail level
		/// </summary>
		/// <param name="detailLevel"></param>
		private void showState( int detailLevel )
		{

			StringBuilder sb = new StringBuilder();


			if ( detailLevel > 4 )
			{

				// Time
				sb.Append( "\n\nClock Cycle = " ).Append( timeCC );

			}

			if ( detailLevel > 5 )
			{

				sb.Append( "\n\n" ).Append( Memory.ToString() );

			}

			if ( detailLevel > 2 )
			{

				//Flags
				sb.Append( "\n\tFLAGS: S=" ).Append( S ).Append( " I=" ).Append( I ).Append( " D=" ).Append( D ).Append( " E=" ).Append( E ).Append( " R=" ).Append( R )
					.Append( " IEN=" ).Append( IEN ).Append( " FGI=" ).Append( FGI ).Append( " FGO=" ).Append( FGO );

			}

			if ( detailLevel > 1 )
			{

				// Registers
				sb.Append( "\n\n\t  AR => " ).Append( AR ).Append( "\n\t  PC => " ).Append( PC ).Append( "\n\t  IR => " ).Append( IR ).Append( "\n\t  DR => " ).Append( DR )
					.Append( "\n\t  AC => " ).Append( AC ).Append( "\n\t  TR => " ).Append( TR ).Append( "\n\tINPR => " ).Append( INPR ).Append( "\n\tOUTR => " ).AppendLine( OUTR.ToString() );

			}

			if ( detailLevel > 0 )
			{

				// Instruction and Type
				if ( instruction != String.Empty)
					sb.AppendLine( "\n\t" + instruction + " instruction\n\t" + instructionType );
			}

			Console.Write( sb.ToString() );
		}

		#endregion

	}
}
