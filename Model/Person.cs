using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingDemo.Model
{
	public class Person
	{
		public string name { get; set; }
		public List<Completion>? completions { get; set; }
	}
}
