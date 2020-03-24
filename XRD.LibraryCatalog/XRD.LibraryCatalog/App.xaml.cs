using System;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NLog;

namespace XRD.LibCat {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		#region StartUp and Logging
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			SetupExceptionHandling();
		}

		private void SetupExceptionHandling() {
			AppDomain.CurrentDomain.UnhandledException += (s, e) =>
			LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
			DispatcherUnhandledException += (s, e) => {
				LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
				e.Handled = true;
			};

			TaskScheduler.UnobservedTaskException += (s, e) => {
				LogUnhandledException(e.Exception, "TaskScheduler.UnobserverdTaskException");
				e.SetObserved();
			};
		}

		private void LogUnhandledException(Exception exception, string source) {
			string message = $"Unhandled exception ({source})";
			try {
				System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
				message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
			}catch(Exception ex) {
				_logger.Error(ex, "Exception in LogUnhandledException");
			}finally {
				_logger.Error(exception, message);
			}
		}
		#endregion

		public static void ShowValidationMessage(List<EntityValidationError> validationErrors) {
			if (validationErrors.IsNullOrEmpty())
				return;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach(var ve in validationErrors) {
				if (sb.Length > 0)
					sb.Append(Environment.NewLine + Environment.NewLine);
				sb.Append(ve.ErrorDescription);
			}
			MessageBox.Show(sb.ToString(), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public static string AppDirectory {
			get {
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "XRD");
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				path = Path.Combine(path, "LibraryCatalog");
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				return path;
			}
		}
		public static string DbPath =>
			Path.Combine(AppDirectory, "XrdLibary.db");

		public static Models.LibraryContext DbContext {
			get {
				var optionsBuilder = new DbContextOptionsBuilder<Models.LibraryContext>();
				optionsBuilder.UseSqlite($"Data Source={DbPath}");
				var res = new Models.LibraryContext(optionsBuilder.Options);
				res.Database.Migrate();
				return res;
			}
		}

		private static System.Net.Http.HttpClient httpClient = null;
		public static System.Net.Http.HttpClient HttpClient {
			get {
				if (httpClient == null)
					httpClient = new System.Net.Http.HttpClient();
				return httpClient;
			}
		}

		public static TWindow OpenWindow<TWindow>(int? id = null) where TWindow : Controls.EntityBoundWindow {
			if (Current.Windows.Count > 0) {
				foreach (var w in Current.Windows) {
					if (w is TWindow ebw) {
						if (ebw.EntityId == id) {
							ebw.Focus();
							return ebw as TWindow;
						}
					}
				}
			}

			var wnd = Activator.CreateInstance(typeof(TWindow)) as TWindow;
			wnd.EntityId = id;
			wnd.Show();
			return wnd;
		}
	}
}
