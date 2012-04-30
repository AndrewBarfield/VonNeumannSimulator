using System;
using System.Collections;

namespace VonNeumannSimulator
{

	// http://people.uncw.edu/tagliarinig/Courses/242/RegisterTransfer/MicroOperationControlFunctions.htm
	public sealed partial class CPU
	{

		#region Memory-reference Instructions

		private void ANDInstruction()
		{

			// DR <- M[AR]
			DR.HexStringValue = Memory[+AR];
			timeCC++;

			// AC <- AC^DR
			AC.Value = AC.Value & DR.Value;
			timeCC++;

		}

		
		private void ADDInstruction()
		{
			
			// DR <- M[AR]
			DR.HexStringValue = Memory[+AR];
			timeCC++;

			// AC <- AC+DR, E <- Cout, SC <-0
			AC.Value += DR.Value;


			//    v
			// 0001 0000 0000 0000 0000
			E = ( AC.Value & 0x10000 ) >> 16;


			// Clear upper 16 bits (ie, this register is only 16 bits)
			AC.Value &= (int)~( 0xFFFF0000 );
			timeCC++;
		}


		private void LDAInstruction()
		{

			// DR <- M[AR]
			DR.HexStringValue = Memory[+AR];
			timeCC++;

			// AC <- DR
			AC = DR;
			timeCC++;

		}


		private void STAInstruction()
		{

			// M[AR] <- AC, SC <- 0
			Memory[+AR] = AC.HexStringValue;
			timeCC++;
		}


		private void BUNInstruction()
		{

			// PC <- AR, SC <- 0
			PC = AR;
			timeCC++;
		}


		private void BSAInstruction()
		{

			// M[AR] <- PC, AR <- AR+1
			Memory[+AR] = PC.HexStringValue;
			AR++;
			timeCC++;

			// PC <- AR, SC <- 0
			PC = AR;
			timeCC++;
		}


		private void ISZInstruction()
		{

			// DR <- M[AR]
			DR.HexStringValue = Memory[+AR];
			timeCC++;

			// DR <- DR+1
			DR++;
			timeCC++;

			// M[AR] <- DR, if( DR=0 ) then( PC <- PC+1 ), SC <- 0
			Memory[+AR] = DR.HexStringValue;
			if ( DR.Value == 0 )
				PC++;

			timeCC++;

		}

		#endregion





		#region Register-reference Instructions

		private void CLAInstruction()
		{
			AC.Value = 0;
			timeCC++;

		}


		private void CLEInstruction()
		{
			E = 0;
			timeCC++;

		}


		private void CMAInstruction()
		{
			AC.Value = ~AC.Value;
			timeCC++;
		}


		private void CMEInstruction()
		{
			E = ~E;
			timeCC++;
		}


		private void CIRInstruction()
		{

                        //
			AC.Value &= (int)~( 0xFFFF0000 );

                        //
                        int tmp = ( AC.Value & 0x1 );

			// AC <- shr AC
			AC.Value >>= 1;


			// AC(15) <- E
			if ( E == 0 )
			{
				// Clear AC(15)
				AC.Value &= ~( 1 << 15 );
			}
			else
			{
				// Set AC(15)
				AC.Value |= ( 1 << 15 );
			}

                        // E <- AC(0)
                        E = tmp;

                        timeCC++;

		}


		private void CILInstruction()
		{

                        // AC <- shl AC, AC(0) <- E, E <- AC(15)


                        // Store AC(15)
                        int tmp = ( AC.Value & ( 1 << 15 ) ) >> 15;


			// AC <- shl AC
			AC.Value <<= 1;


			// AC(0) <- E
			if ( E == 0 )
			{
				// Clear AC(0)
				AC.Value &= ~( 0x01 );
			}
			else
			{
				// Set AC(0)
				AC.Value |= ( 0x01 );
			}

                        E = tmp;

			// Clear the high 16 bits becuase I'm using an Int32
			// and the bumped bit doesnt actually get bumped
			AC.Value &= (int)~( 0xFFFF0000 );

			timeCC++;

		}


		private void INCInstruction()
		{
			AC++;
			timeCC++;
		}


		private void SPAInstruction()
		{
			// If( AC(15) = 0 ) then( PC¬ PC+1 )
			if ( ( AC.Value & 0x8000 ) == 0 )
				PC++;
			
			timeCC++;

		}


		private void SNAInstruction()
		{
			// If( AC(15) = 1 ) then( PC¬ PC+1 )
			if ( ( AC.Value & 0x8000 ) == 1 )
				PC++;

			timeCC++;

		}


		private void SZAInstruction()
		{
			// If( AC = 0 ) then( PC¬ PC+1 )
			if ( AC.Value == 0 )
				PC++;

		}


		private void SZEInstruction()
		{
			// If( E = 0 ) then( PC¬ PC+1 )
			if ( E == 0 )
				PC++;

			timeCC++;

		}


		private void HLTInstruction()
		{
			S = 0;
			timeCC++;
		}

		#endregion





		#region I/O Instructions

		private void INPInstruction()
		{
			// AC(0..7) <- INPR, FGI <- 0
			//AC.Value |= (byte)INPR.Value;

                        // -32766 + ( -32766 & 0x000000FF ) | 180
                        AC.Value = AC.Value + ( AC.Value & 0x000000FF ) | INPR.Value;
			FGI = 0;

			timeCC++;

		}


		private void OUTInstruction()
		{
			// OUTR <- AC(0..7), FGO <- 0
			OUTR.Value = (byte)( AC.Value & 0x0000FFFF );
			FGO = 0;

			timeCC++;
		}


		private void SKIInstruction()
		{
			// If( FGI = 1 ) then( PC <- PC+1 )
			if ( FGI == 1 )
				PC++;

			timeCC++;

		}


		private void SKOInstruction()
		{
			// If( FGO = 1 ) then( PC <- PC+1 )
			if ( FGO == 1 )
				PC++;

			timeCC++;

		}


		private void IONInstruction()
		{
			// IEN <- 1
			IEN = 1;
			timeCC++;

		}


		private void IOFInstruction()
		{
			// IEN <- 0
			IEN = 0;
			timeCC++;

		}


                private void RTIInstruction()
                {
                        // R'T4: AR <- 0
                        AR.Value = 0;
                        timeCC++;

                        // R'T5: PC <- M[AR], IEN <- 1
                        PC.HexStringValue = Memory[+AR];
                        IEN = 1;
                        timeCC++;
                }

		#endregion




	}
}
