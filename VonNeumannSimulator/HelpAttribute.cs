
using System;
using System.Text;



namespace VonNeumannSimulator
{

	/// <summary>
	/// Help Attribute
	/// </summary>
	public class HelpAttribute : Attribute
	{
		string url;

		string topic;


		/// <summary>
		/// Help Attribute
		/// </summary>
		/// <param name="url"></param>
		public HelpAttribute( string url )
		{
			this.url = url;
		}

		/// <summary>
		/// Help Attribute URL
		/// </summary>
		public string Url
		{
			get
			{
				return url;
			}
		}


		/// <summary>
		/// Help Attribute Topic
		/// </summary>
		public string Topic
		{
			get
			{
				return topic;
			}
			set
			{
				topic = value;
			}
		}
	}


}
