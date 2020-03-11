using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XRD.LibCat.Models {
	[Description("Catalog Entry")]
	[Table("tblCatalog")]
	public class CatalogEntry : Abstract.ModifiableEntity, IHasRestrictions, ISoftDeleted {
		#region Constructors
		/// <summary>
		/// Default constructor
		/// </summary>
		public CatalogEntry() : base() { }

		public CatalogEntry(bool isNew) : base(isNew) { }

		public CatalogEntry(string title, string subtitle, List<string> ids, string publisher, string pubDate = null, int? pages = null, string desc = null, string shelf = null, int? minAge = null, int? maxAge = null, GradeLevels minGrd = GradeLevels.NotSet, GradeLevels maxGrd = GradeLevels.NotSet, params string[] genres) : base(true) {
			Title = title;
			Subtitle = subtitle;
			if (!ids.IsNullOrEmpty()) {
				foreach(var i in ids) {
					Identifiers.Add(new Identifier(this, i));
				}
			}

			Publisher = publisher;
			PubDate = pubDate;
			PageCount = pages;
			Description = !string.IsNullOrWhiteSpace(desc) ? desc.Trim() : null;
			MinAge = minAge;
			MaxAge = maxAge;
			MinGrade = minGrd;
			MaxGrade = maxGrd;
			ShelfLocation = !string.IsNullOrWhiteSpace(shelf) ? shelf : null;
			if(!genres.IsNullOrEmpty()) {
				foreach (var g in genres)
					Genres.Add(new Genre(this, g));
			}
		}
		#endregion

		#region DB Columns
		private string _title;
		[Required, StringLength(1000)]
		[Display(Name = "Title", ShortName = "Title", Description = "The book's title.")]
		public string Title {
			get => _title;
			set => _title = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(1000) : throw new ArgumentNullException("Title");
		}

		private string _subtitle;
		[StringLength(1000)]
		[Display(Name = "Subtitle", ShortName = "Subtitle", Description = "The book's subtitle (optional).")]
		public string Subtitle {
			get => _subtitle;
			set => _subtitle = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(1000) : null;
		}

		private string _publisher;
		[StringLength(1000)]
		[Display(Name = "Publisher", ShortName = "Publisher", Description = "The book's publisher (optional).")]
		public string Publisher {
			get => _publisher;
			set => _publisher = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(1000) : null;
		}

		[StringLength(100)]
		[Display(Name = "Publish Date", ShortName = "Pub Date", Description = "The date the book was published.")]
		public string PubDate { get; set; }

		[Display(Name = "Description", ShortName = "Desc.", Description = "A brief description/synopsis of the book.")]
		public string Description { get; set; }

		[Display(Name = "Page Count", ShortName = "Pgs.", Description = "The number of pages in the book.")]
		public int? PageCount { get; set; }

		#region Restrictions
		[Display(Name = "Minimum Checkout Age", ShortName = "Min. Age", Description = "The minimum age allowed to checkout this book (optional).")]
		public int? MinAge { get; set; }

		[Display(Name = "Maximum Checkout Age", ShortName = "Max. Age", Description = "The maximum age allowed to checkout this book (optional).")]
		public int? MaxAge { get; set; }

		[Display(Name = "Minimum Checkout Grade", ShortName = "Min. Grade", Description = "The minimum grade level allowed to checkout this book (optional).")]
		public GradeLevels MinGrade { get; set; }

		[Display(Name = "Maximum Checkout Grade", ShortName = "Max. Grade", Description = "The maximum grade level allowed to checkout this book (optional).")]
		public GradeLevels MaxGrade { get; set; }
		#endregion

		[Column("ShelfLocation")]
		private string _shelfLocation;
		[StringLength(150)]
		[Display(Name = "Shelf Location", ShortName = "Shelf", Description = "The shelf where the book should be stored in the library (optional).")]
		public string ShelfLocation {
			get => _shelfLocation;
			set => _shelfLocation = !string.IsNullOrWhiteSpace(value) ? value.TrimTo(150) : null;
		}

		[Display(Name = "Is Deleted?", ShortName = "Is Del.?", Description = "Has this book been flagged as deleted (no longer own any copies)?")]
		public bool IsDeleted { get; set; } = false;
		#endregion

		#region Navigation
		public virtual ICollection<Author> Authors { get; set; }

		public virtual ICollection<Genre> Genres { get; set; }
		public virtual ICollection<Identifier> Identifiers { get; set; }
		public virtual ICollection<OwnedBook> OwnedBooks { get; set; }
		#endregion

		protected override void InstantiateCollections() {
			base.InstantiateCollections();
			Authors = new HashSet<Author>();
			Identifiers = new HashSet<Identifier>();
			OwnedBooks = new HashSet<OwnedBook>();
			Genres = new HashSet<Genre>();
		}

		internal class Config : EntityConfig<CatalogEntry> {
			public override void Configure(EntityTypeBuilder<CatalogEntry> builder) {
				base.Configure(builder);

				builder.HasMany(b => b.Authors)
					.WithOne(a => a.Book)
					.HasForeignKey(a => a.VolId)
					.HasPrincipalKey(b => b.Id)
					.OnDelete(DeleteBehavior.Restrict);

				builder.HasMany(b => b.Genres)
					.WithOne(g => g.Book)
					.HasForeignKey(g => g.CatId)
					.HasPrincipalKey(b => b.Id)
					.OnDelete(DeleteBehavior.Cascade);

				builder.HasMany(b => b.Identifiers)
					.WithOne(i => i.Book)
					.HasForeignKey(i => i.CatId)
					.HasPrincipalKey(b => b.Id)
					.OnDelete(DeleteBehavior.Cascade);

				builder.HasMany(b => b.OwnedBooks)
					.WithOne(o => o.Book)
					.HasForeignKey(o => o.CatId)
					.HasPrincipalKey(b => b.Id)
					.OnDelete(DeleteBehavior.Restrict);
			}
		}
		/// <summary>
		/// A single string containing all authors.
		/// </summary>
		public string AuthorDisplay {
			get {
				if (Authors.IsNullOrEmpty())
					return null;
				if (Authors.Count == 1)
					return Authors.First().FullName;
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				foreach (var a in Authors) {
					if (sb.Length > 0)
						sb.Append("; ");
					sb.Append(a.FullName);
				}
				return sb.ToString();
			}
		}

		public string GenreDisplay {
			get {
				if (Genres.IsNullOrEmpty())
					return null;
				if (Genres.Count == 1)
					return Genres.First().Value;
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				foreach(var g in Genres) {
					if (sb.Length > 0)
						sb.Append("; ");
					sb.Append(g);
				}
				return sb.ToString();
			}
		}

		public string IdentifierDisplay {
			get {
				if (Identifiers.IsNullOrEmpty())
					return null;
				if (Identifiers.Count == 1)
					return Identifiers.First().Value;
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				foreach (var g in Identifiers) {
					if (sb.Length > 0)
						sb.Append("; ");
					sb.Append(g);
				}
				return sb.ToString();
			}
		}

		public Author AddAuthor(string name, string role = null) {
			if (string.IsNullOrWhiteSpace(name))
				return null;

			PersonName pn = new PersonName(name);
			name = pn.ToFullName(includeMiddle: true);

			var res = Authors.Where(a => a.FullName.ToLower().Equals(name.ToLower())).FirstOrDefault();
			if(res == null) {
				res = new Author(this, name, Authors.Count, role);
				Authors.Add(res);
			}
			return res;
		}

		public Genre AddGenre(string genre) {
			if (string.IsNullOrWhiteSpace(genre))
				return null;

			var res = Genres.Where(g => g.Value.ToLower().Equals(genre.Trim().ToLower())).FirstOrDefault();
			if(res == null) { 
				res = new Genre(this, genre.Trim());
				Genres.Add(res);
			}
			return res;
		}

		public Identifier AddIdentifier(string identifier) {
			if (string.IsNullOrWhiteSpace(identifier))
				return null;

			identifier = identifier.Trim().ToUpper();
			var res = Identifiers.Where(i => i.Value.Equals(identifier)).FirstOrDefault();
			if (res == null) {
				res = new Identifier(this, identifier);
				Identifiers.Add(res);
			}
			return res;
		}

		public OwnedBook AddOwnedBook(int bookNum) {
			OwnedBook res = OwnedBooks.Where(b => b.BookNumber == bookNum).FirstOrDefault();
			if(res == null) {
				res = new OwnedBook(bookNum, this);
				OwnedBooks.Add(res);
			}
			return res;
		}
	}
}