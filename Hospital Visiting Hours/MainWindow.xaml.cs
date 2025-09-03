using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Hospital_Visiting_Hours
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// Stores visitors per patient (using patient full name as key)
		private Dictionary<string, List<Visitor>> patientVisitors = new Dictionary<string, List<Visitor>>();

		// Currently selected patient name (used for displaying correct visitors)
		private string currentPatientKey = "";

		public MainWindow()
		{
			InitializeComponent();

			// 🔽 Call QR Code generation on app startup
			string registrationUrl = "https://hospital-registration.example.com"; // Change this to your real URL
			GenerateQRCode(registrationUrl);


			InitializeComponent();

			dpVisitDate.SelectedDate = DateTime.Today;


		}

		// ✅ QR Code Generation Method
		private void GenerateQRCode(string url)
		{
			using (var qrGenerator = new QRCodeGenerator())
			{
				var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
				var qrCode = new QRCode(qrCodeData);

				using (Bitmap bitmap = qrCode.GetGraphic(20))
				using (MemoryStream memory = new MemoryStream())
				{
					bitmap.Save(memory, ImageFormat.Png);
					memory.Position = 0;

					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.StreamSource = memory;
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapImage.EndInit();

					imgQRCode.Source = bitmapImage;
				}
			}
		}

		// 1. Hook into the internal TextBox inside ComboBox when it loads
		private void cbSearchPatient_Loaded(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)cbSearchPatient.Template.FindName("PART_EditableTextBox", cbSearchPatient);
			if (textBox != null)
			{
				textBox.TextChanged += CbSearchPatient_TextChanged;
			}
		}

		// 2. Triggered as user types in the ComboBox
		private void CbSearchPatient_TextChanged(object sender, TextChangedEventArgs e)
		{
			string input = cbSearchPatient.Text.ToLower().Trim();

			// Suggest matching patients
			var suggestions = patientVisitors.Keys
				.Where(k => k.Contains(input))
				.Select(k => k.Replace("_", " "))
				.Distinct()
				.ToList();

			cbSearchPatient.ItemsSource = suggestions;
			cbSearchPatient.IsDropDownOpen = suggestions.Any();
		}

		// 3. When user selects a patient from suggestions
		private void cbSearchPatient_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbSearchPatient.SelectedItem == null)
				return;

			string selected = cbSearchPatient.SelectedItem.ToString().ToLower().Replace(" ", "_");

			if (patientVisitors.ContainsKey(selected))
			{
				currentPatientKey = selected;
				RefreshVisitorLists();
			}
		}

		// Visitor data model
		public class Visitor
		{
			public string Name { get; set; }
			public string Surname { get; set; }
			public string Cellphone { get; set; }
			public string PatientName { get; set; }
			public string PatientSurname { get; set; }
			public DateTime VisitDate { get; set; }
			public string VisitTime { get; set; }
			public bool IsInside { get; set; }

			public string DisplayName => $"{Name} {Surname} - {Cellphone}";
		}

		// Check-in logic
		private void CheckIn_Click(object sender, RoutedEventArgs e)
		{
			string visitorName = txtVisitorName.Text.Trim();
			string visitorSurname = txtVisitorSurname.Text.Trim();
			string cellphone = txtCellphone.Text.Trim();
			string patientName = txtPatientName.Text.Trim();
			string patientSurname = txtPatientSurname.Text.Trim();
			DateTime? visitDate = dpVisitDate.SelectedDate;
			ComboBoxItem selectedTimeItem = cbVisitTime.SelectedItem as ComboBoxItem;
			string visitTime = selectedTimeItem?.Content.ToString() ?? "";

			// === Validations ===

			if (visitorName.Length < 3 || visitorSurname.Length < 3)
			{
				MessageBox.Show("Visitor name and surname must each be at least 3 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (!Regex.IsMatch(cellphone, @"^0\d{9}$"))
			{
				MessageBox.Show("Cellphone number must be a valid South African number (10 digits, starts with 0).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (!visitDate.HasValue || visitDate.Value.Date != DateTime.Today)
			{
				MessageBox.Show("Visit date must be today only.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrEmpty(visitTime))
			{
				MessageBox.Show("Please select a visit time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			string patientKey = $"{patientName.ToLower()}_{patientSurname.ToLower()}";

			if (!patientVisitors.ContainsKey(patientKey))
				patientVisitors[patientKey] = new List<Visitor>();

			List<Visitor> visitors = patientVisitors[patientKey];

			if (visitors.Count >= 5)
			{
				MessageBox.Show("Maximum 5 visitors already registered for this patient.", "Limit Reached", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			if (visitors.Any(v => v.Cellphone == cellphone))
			{
				MessageBox.Show("This visitor is already registered.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var newVisitor = new Visitor
			{
				Name = visitorName,
				Surname = visitorSurname,
				Cellphone = cellphone,
				PatientName = patientName,
				PatientSurname = patientSurname,
				VisitDate = visitDate.Value,
				VisitTime = visitTime,
				IsInside = visitors.Count(v => v.IsInside) < 2
			};

			visitors.Add(newVisitor);
			currentPatientKey = patientKey;
			RefreshVisitorLists();

			// Clear form
			txtVisitorName.Text = "";
			txtVisitorSurname.Text = "";
			txtCellphone.Text = "";
			txtPatientName.Text = "";
			txtPatientSurname.Text = "";
			cbVisitTime.SelectedIndex = -1;
		}

		// Refresh Inside & Waiting Lists
		private void RefreshVisitorLists()
		{
			if (string.IsNullOrEmpty(currentPatientKey) || !patientVisitors.ContainsKey(currentPatientKey))
				return;

			var visitors = patientVisitors[currentPatientKey];
			lstInsideVisitors.ItemsSource = visitors.Where(v => v.IsInside).ToList();
			lstWaitingVisitors.ItemsSource = visitors.Where(v => !v.IsInside).ToList();
		}

		// Move from Waiting to Inside
		private void MoveToInside_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(currentPatientKey)) return;

			var visitors = patientVisitors[currentPatientKey];
			if (visitors.Count(v => v.IsInside) >= 2)
			{
				MessageBox.Show("Maximum 2 visitors allowed inside.", "Limit Reached", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var button = sender as Button;
			var visitor = button?.Tag as Visitor;
			if (visitor != null)
			{
				visitor.IsInside = true;
				RefreshVisitorLists();
			}
		}

		// Move from Inside to Waiting
		private void SendBackToWaiting_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var visitor = button?.Tag as Visitor;
			if (visitor != null)
			{
				visitor.IsInside = false;
				RefreshVisitorLists();
			}
		}

		// Remove Visitor
		private void RemoveVisitor_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var visitor = button?.Tag as Visitor;
			if (visitor != null && patientVisitors.ContainsKey(currentPatientKey))
			{
				patientVisitors[currentPatientKey].Remove(visitor);
				RefreshVisitorLists();
			}
		}



		private void OpenDashboard_Click(object sender, RoutedEventArgs e)
		{
			var dashboard = new AdminDashboard(patientVisitors);
			dashboard.ShowDialog();
		}



	}
}
