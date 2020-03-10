using System;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace XRD.LibCat {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
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
	}
}
