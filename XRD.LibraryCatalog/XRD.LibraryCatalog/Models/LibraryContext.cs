using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace XRD.LibCat.Models {
	public class LibraryContext : DbContext {
		private bool isCleanup = false;
		public LibraryContext() : base() { }

		public LibraryContext(DbContextOptions options) : base(options) { }

		#region DbSets
		public DbSet<Author> Authors { get; set; }
		public DbSet<BorrowingHistory> BorrowingHistories { get; set; }
		public DbSet<CatalogEntry> CatalogEntries { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public DbSet<Identifier> Identifiers { get; set; }
		public DbSet<OwnedBook> OwnedBooks { get; set; }
		public DbSet<Patron> Patrons { get; set; }
		public DbSet<StaffMember> StaffMembers { get; set; }
		#endregion

		#region Overrides
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			base.OnConfiguring(optionsBuilder);

			if (!optionsBuilder.IsConfigured)
				optionsBuilder.UseSqlite($"Data Source={App.DbPath}");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new Author.Config());
			modelBuilder.ApplyConfiguration(new BorrowingHistory.Config());
			modelBuilder.ApplyConfiguration(new CatalogEntry.Config());
			modelBuilder.ApplyConfiguration(new Genre.Config());
			modelBuilder.ApplyConfiguration(new Identifier.Config());
			modelBuilder.ApplyConfiguration(new OwnedBook.Config());
			modelBuilder.ApplyConfiguration(new Patron.Config());
			modelBuilder.ApplyConfiguration(new StaffMember.Config());
		}

		public override int SaveChanges(bool acceptAllChangesOnSuccess) {
			performPresave();
			return base.SaveChanges(acceptAllChangesOnSuccess);
		}
		public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) {
			performPresave();
			return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}
		#endregion

		private void performPresave() {
			if (!isCleanup) {
				var qryD = from d
						   in ChangeTracker.Entries<ISoftDeleted>()
						   where d.State == EntityState.Deleted
						   select d;
				if (qryD.Any()) {
					foreach (var d in qryD) {
						d.Entity.IsDeleted = true;
						d.State = EntityState.Modified;
					}
				}
			}

			var qryE = from e
					   in ChangeTracker.Entries<Abstract.ModifiableEntity>()
					   where e.State == EntityState.Added
					   || e.State == EntityState.Modified
					   select e;

			if (qryE.Any()) {
				foreach (var e in qryE) {
					e.Entity.Ts = DateTime.UtcNow;
					if (e.State == EntityState.Modified)
						e.Entity.Ec++;
				}
			}
		}

		private static readonly Dictionary<Type, string> EntityDisplayNames = new Dictionary<Type, string>();
		public string GetDisplayName(Type t) {
			if (EntityDisplayNames.ContainsKey(t)) {
				return EntityDisplayNames[t];
			}

			var att = t.GetCustomAttribute<DescriptionAttribute>();
			string name;
			if (att == null)
				name = t.Name.SplitCamelCase(true);
			else
				name = att.Description;
			EntityDisplayNames.Add(t, name);
			return name;
		}
		#region Clean-Up methods
		public async Task<int> CleanUpAuthors(string wrong, string right) {
			isCleanup = true;
			var qry = from va
					  in Authors
					  where va.FullName == wrong
					  select va;

			if (await qry.AnyAsync()) {
				foreach (var va in await qry.ToListAsync()) {
					va.FullName = right;
				}
			}
			int res = await SaveChangesAsync();
			isCleanup = false;
			return res;
		}

		public async Task<int> CleanUpPublishers(string wrong, string right) {
			isCleanup = true;
			var qry = from v
					  in CatalogEntries
					  where v.Publisher == wrong
					  select v;

			if (await qry.AnyAsync()) {
				foreach (var v in await qry.ToListAsync())
					v.Publisher = right;
			}
			int res = await SaveChangesAsync();
			isCleanup = false;
			return res;
		}
		#endregion
		private IQueryable<string> queryDistinctPublishers =>
			(from v
			 in CatalogEntries
			 select v.Publisher
			).Distinct();

		public List<string> GetPublishers() =>
			queryDistinctPublishers.OrderBy(p => p).ToList();

		public async Task<List<string>> GetPublishersAsync() =>
			await queryDistinctPublishers.OrderBy(p => p).ToListAsync();

		public IQueryable<CatalogEntry> QueryBooksByPublisher(string publisher) {
			if (string.IsNullOrWhiteSpace(publisher)) {
				return from v
					   in CatalogEntries
					   where v.Publisher == null
					   || v.Publisher == string.Empty
					   select v;
			} else {
				if (publisher.ContainsWildcard()) {
					return from v
						   in CatalogEntries
						   where EF.Functions.Like(v.Publisher, publisher.ToSqlLikeFilter())
						   select v;
				} else {
					return from v
						   in CatalogEntries
						   where v.Publisher == publisher.TrimTo(1000)
						   select v;
				}
			}
		}

		private IQueryable<string> queryDistinctAuthors =>
			(from a
			 in Authors
			 select a.FullName
			).Distinct();

		public List<string> GetAuthors() =>
			queryDistinctAuthors.OrderBy(a => a).ToList();

		public async Task<List<string>> GetAuthorsAsync() =>
			await queryDistinctAuthors.OrderBy(a => a).ToListAsync();

		public IQueryable<CatalogEntry> QueryBooksByAuthor(string authorName) {
			if (string.IsNullOrWhiteSpace(authorName))
				return null;

			if (authorName.ContainsWildcard()) {
				return from v
					   in CatalogEntries
					   join a in Authors on v.Id equals a.CatId
					   where EF.Functions.Like(a.FullName, authorName.ToSqlLikeFilter())
					   select v;
			} else {
				return from v
					   in CatalogEntries
					   join a in Authors on v.Id equals a.CatId
					   where a.FullName == authorName.Trim()
					   select v;
			}
		}

		private IQueryable<string> queryDistinctGenres =>
			(from g
			 in Genres
			 select g.Value
			 ).Distinct();

		public List<string> GetGenres() =>
			queryDistinctGenres.OrderBy(a => a).ToList();
		public async Task<List<string>> GetGenresAsync() =>
			await queryDistinctGenres.OrderBy(a => a).ToListAsync();
		
		public IQueryable<CatalogEntry> QueryBooksByGenre(string genre) {
			if (string.IsNullOrWhiteSpace(genre))
				return null;
			if(genre.ContainsWildcard()) {
				return from c
					   in CatalogEntries
					   join g in Genres on c.Id equals g.CatId
					   where EF.Functions.Like(g.Value, genre.ToSqlLikeFilter())
					   select c;
			}else {
				return from c
					   in CatalogEntries
					   join g in Genres on c.Id equals g.CatId
					   where g.Value == genre.Trim()
					   select c;
			}
		}

		public IQueryable<CatalogEntry> QueryBooksByIdentifier(string identifier) {
			if (string.IsNullOrWhiteSpace(identifier))
				return null;
			if(identifier.ContainsWildcard()) {
				return from c
					   in CatalogEntries
					   join i in Identifiers on c.Id equals i.CatId
					   where EF.Functions.Like(i.Value, identifier.ToSqlLikeFilter())
					   select c;
			}else {
				return from c
					   in CatalogEntries
					   join i in Identifiers on c.Id equals i.CatId
					   where i.Value == identifier.Trim()
					   select c;
			}
		}

		public IQueryable<CatalogEntry> QueryBooksByTitle(string title) {
			if (string.IsNullOrWhiteSpace(title))
				return null;

			if (title.ContainsWildcard()) {
				title = title.ToSqlLikeFilter();
				return from v
					   in CatalogEntries
					   where EF.Functions.Like(v.Title, title)
					   || EF.Functions.Like(v.Subtitle, title)
					   select v;
			} else {
				title = title.TrimTo(1000);
				return from v
					   in CatalogEntries
					   where v.Title == title
					   || v.Subtitle == title
					   select v;
			}
		}

		public IQueryable<CatalogEntry> AddCatalogIncludes(IQueryable<CatalogEntry> query) =>
			query.Include(c => c.Authors)
				.Include(c => c.Genres)
				.Include(c => c.Identifiers)
				.Include(c => c.OwnedBooks);
	}
}