using System.Text.Json;
using TrainingDemo.Model;

namespace TrainingDemo
{
	public class Program
	{
		static void Main(string[] args)
		{
			var reportGenerator = new TrainingReportGenerator();
			reportGenerator.GenerateReports();
		}
	}

	public class TrainingReportGenerator
	{
		private readonly JsonSerializerOptions _options;

		public TrainingReportGenerator()
		{
			_options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			};
		}

		public void GenerateReports()
		{
			var people = LoadPeopleData();

			if (people == null || !people.Any())
			{
				Console.WriteLine("No data was found or deserialized.");
				return;
			}

			GenerateTrainingCompletionCountReport(people);
			GenerateTrainingCompletionByFiscalYearReport(people);
			GenerateExpiringTrainingReport(people);

			Console.WriteLine("All reports generated");
		}

		public List<Person> LoadPeopleData()
		{
			if (!Directory.Exists("Output"))
			{
				Directory.CreateDirectory("Output");
			}

			string jsonData = File.ReadAllText(Path.Combine("Data/trainings.txt"));
			return JsonSerializer.Deserialize<List<Person>>(jsonData);
		}

		//Task 1: List each completed training with a count of how many people completed it.
		public void GenerateTrainingCompletionCountReport(List<Person> people)
		{
			var completedTrainingCounts = people
				.Where(person => person.completions != null)
				.SelectMany(person => person.completions)
				.GroupBy(completion => completion.name)
				.Select(group => new
				{
					TrainingName = group.Key,
					Count = group.Count()
				}).ToList();

			File.WriteAllText(Path.Combine("Output/output_1.txt"), JsonSerializer.Serialize(completedTrainingCounts, _options));
		}

		//Task 2: List all people that completed specific trainings in a given fiscal year.
		public void GenerateTrainingCompletionByFiscalYearReport(List<Person> people)
		{
			var targetTrainings = new[] { "Electrical Safety for Labs", "X-Ray Safety", "Laboratory Safety Training" };
			DateTime fiscalYearStart = new DateTime(2023, 7, 1);
			DateTime fiscalYearEnd = new DateTime(2024, 6, 30);

			var trainingInFiscalYear = people
				.Where(person => person.completions != null)
				.SelectMany(person => person.completions
					.Where(completion => targetTrainings.Contains(completion.name)
										 && DateTime.TryParse(completion.timestamp, out DateTime completionDate)
										 && completionDate >= fiscalYearStart
										 && completionDate <= fiscalYearEnd)
					.Select(completion => new
					{
						TrainingName = completion.name,
						PersonName = person.name
					}))
				.GroupBy(x => x.TrainingName)
				.Select(group => new
				{
					TrainingName = group.Key,
					//Year = fiscalYearStart.Year,
					People = group.Select(x => x.PersonName).Distinct().ToList()
				})
				.ToList();

			File.WriteAllText(Path.Combine("Output/output_2.txt"), JsonSerializer.Serialize(trainingInFiscalYear, _options));
		}

		//Task 3: Find people with expired or soon-to-expire trainings
		public void GenerateExpiringTrainingReport(List<Person> people)
		{
			DateTime targetDate = new DateTime(2023, 10, 1);
			DateTime oneMonthFromTarget = targetDate.AddMonths(1);

			var peopleWithExpiringTrainings = people
				.Where(person => person.completions != null)
				.Select(person => new
				{
					PersonName = person.name,
					Trainings = person.completions
						.Select(completion => new
						{
							TrainingName = completion.name,
							ExpirationDateParsed = DateTime.TryParse(completion.expires, out DateTime expiresDate) ? (DateTime?)expiresDate : null
						})
						.Where(x => x.ExpirationDateParsed.HasValue && (x.ExpirationDateParsed.Value < targetDate || x.ExpirationDateParsed.Value <= oneMonthFromTarget))
						.Select(x => new
						{
							x.TrainingName,
							Status = x.ExpirationDateParsed.Value < targetDate ? "Expired" : "Expires Soon"
						})
						.ToList()
				})
				.Where(person => person.Trainings.Any())
				.ToList();

			File.WriteAllText(Path.Combine("Output/output_3.txt"), JsonSerializer.Serialize(peopleWithExpiringTrainings, _options));
		}
	}
}
