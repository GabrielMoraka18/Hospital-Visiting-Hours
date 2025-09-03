using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hospital_Visiting_Hours
{
	public partial class AdminDashboard : Window
	{
		private Dictionary<string, List<MainWindow.Visitor>> _patientVisitors;

		public AdminDashboard(Dictionary<string, List<MainWindow.Visitor>> visitors)
		{
			InitializeComponent();
			_patientVisitors = visitors;

			LoadDashboardData();
		}

		private void LoadDashboardData()
		{
			int total = 0, inside = 0, waiting = 0, completed = 0;

			var patientStats = new List<PatientQueueInfo>();

			foreach (var patient in _patientVisitors)
			{
				var visitors = patient.Value;
				int insideCount = visitors.Count(v => v.IsInside);
				int waitingCount = visitors.Count(v => !v.IsInside);
				int totalCount = visitors.Count;

				patientStats.Add(new PatientQueueInfo
				{
					PatientName = patient.Key.Replace("_", " "),
					InsideCount = insideCount,
					WaitingCount = waitingCount,
					TotalCount = totalCount
				});

				total += totalCount;
				inside += insideCount;
				waiting += waitingCount;
			}

			// Display summary
			txtTotalVisitors.Text = total.ToString();
			txtInsideVisitors.Text = inside.ToString();
			txtWaitingVisitors.Text = waiting.ToString();
			txtCompletedVisitors.Text = completed.ToString(); // Reserved for future status tracking

			// Show in list view
			lvPatientQueues.ItemsSource = patientStats;
		}

		public class PatientQueueInfo
		{
			public string PatientName { get; set; }
			public int InsideCount { get; set; }
			public int WaitingCount { get; set; }
			public int TotalCount { get; set; }
		}
	}
}
