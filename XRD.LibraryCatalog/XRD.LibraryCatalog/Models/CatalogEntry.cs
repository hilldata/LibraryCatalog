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
	public class CatalogEntry : Abstract.ModifiableEntity, IHasRestrictions, ISoftDeleted, INotifyPropertyChanged {
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
		public override List<EntityValidationError> Validate() {
			List<EntityValidationError> res = new List<EntityValidationError>();
			if (string.IsNullOrWhiteSpace(Title))
				res.Add(new EntityValidationError(nameof(Title), "Title is required."));
			if(!string.IsNullOrWhiteSpace(PubDate)) {
				if (PubDate.Length > 100)
					PubDate = PubDate.Truncate(100);
			}
			return res;
		}

		internal class Config : EntityConfig<CatalogEntry> {
			public override void Configure(EntityTypeBuilder<CatalogEntry> builder) {
				base.Configure(builder);

				builder.HasMany(b => b.Authors)
					.WithOne(a => a.Book)
					.HasForeignKey(a => a.CatId)
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
					sb.Append(g.Value);
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
					sb.Append(g.Value);
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
				res = new Author(this, name, Authors.Count + 1, role);
				Authors.Add(res);
				FirePropertyChangedEvent(nameof(Authors));
			}
			return res;
		}

		public bool DelAuthor(string fullName) {
			if (string.IsNullOrWhiteSpace(fullName))
				return false;
			var res = Authors.Where(a => a.FullName.ToLower().Equals(fullName.Trim().ToLower())).FirstOrDefault();
			if(res == null) {
				PersonName pn = new PersonName(fullName);
				res = Authors.Where(a => a.FullName.ToLower().Equals(pn.ToFullName().ToLower())).FirstOrDefault();
				if (res == null)
					return false;
			}
			Authors.Remove(res);
			RenumberAuthors();
			FirePropertyChangedEvent(nameof(Authors));
			return true;
		}
		public bool DelAuthor(Author author) {
			if (!Authors.Contains(author))
				return false;
			Authors.Remove(author);
			RenumberAuthors();
			FirePropertyChangedEvent(nameof(Authors));
			return true;
		}

		public bool MoveAuthorDown(Author author) {
			if (author == null)
				return false;
			if (Authors.IsNullOrEmpty())
				return false;
			if (author.OrdIndex < Authors.Count) {
				var next = Authors.Where(a => a.OrdIndex > author.OrdIndex).OrderBy(a => a.OrdIndex).FirstOrDefault();
				if (next == null)
					return false;
				int temp = next.OrdIndex;
				next.OrdIndex = author.OrdIndex;
				author.OrdIndex = temp;
				FirePropertyChangedEvent(nameof(Authors));
				return true;
			}
			return false;
		}

		public bool MoveAuthorUp(Author author) {
			if (author == null)
				return false;
			if (Authors.IsNullOrEmpty())
				return false;
			if(author.OrdIndex > 1) {
				var prev = Authors.Where(a => a.OrdIndex < author.OrdIndex).OrderByDescending(a => a.OrdIndex).FirstOrDefault();
				if (prev == null)
					return false;
				int temp = prev.OrdIndex;
				prev.OrdIndex = author.OrdIndex;
				author.OrdIndex = temp;
				FirePropertyChangedEvent(nameof(Authors));
				return true;
			}
			return false;
		}

		public bool RenumberAuthors() {
			if (Authors.IsNullOrEmpty())
				return false;
			int c = 0;
			bool changed = false;
			foreach(Author a in Authors.OrderBy(a=>a.OrdIndex)) {
				c++;
				if (a.OrdIndex != c) {
					a.OrdIndex = c;
					changed = true;
				}
			}
			if (changed)
				FirePropertyChangedEvent(nameof(Authors));
			return changed;
		}

		public Genre AddGenre(string genre) {
			if (string.IsNullOrWhiteSpace(genre))
				return null;

			var res = Genres.Where(g => g.Value.ToLower().Equals(genre.Trim().ToLower())).FirstOrDefault();
			if(res == null) { 
				res = new Genre(this, genre.Trim());
				Genres.Add(res);
				FirePropertyChangedEvent(nameof(Genres));
			}
			return res;
		}

		public bool DelGenre(string genre) {
			if (string.IsNullOrWhiteSpace(genre))
				return false;

			var res = Genres.Where(g => g.Value.ToLower().Equals(genre.Trim().ToLower())).FirstOrDefault();
			if (res == null)
				return false;
			Genres.Remove(res);
			FirePropertyChangedEvent(nameof(Genres));
			return true;
		}

		public bool DelGenre(Genre genre) {
			if (!Genres.Contains(genre))
				return false;
			Genres.Remove(genre);
			FirePropertyChangedEvent(nameof(Genres));
			return true;
		}		

		public Identifier AddIdentifier(string identifier) {
			if (string.IsNullOrWhiteSpace(identifier))
				return null;

			identifier = Identifier.FixValue(identifier);
			var res = Identifiers.Where(i => i.Value.Equals(identifier)).FirstOrDefault();
			if (res == null) {
				res = new Identifier(this, identifier);
				Identifiers.Add(res);
				FirePropertyChangedEvent(nameof(Identifiers));
			}
			return res;
		}

		public bool DelIdentifier(string identifier) {
			if (string.IsNullOrWhiteSpace(identifier))
				return false;
			var res = Identifiers.Where(i => i.Value.Equals(Identifier.FixValue(identifier))).FirstOrDefault();
			if (res == null)
				return false;
			Identifiers.Remove(res);
			FirePropertyChangedEvent(nameof(Identifiers));
			return true;
		}

		public bool DelIdentifier(Identifier identifier) {
			if (!Identifiers.Contains(identifier))
				return false;
			Identifiers.Remove(identifier);
			FirePropertyChangedEvent(nameof(Identifiers));
			return true;
		}

		public OwnedBook AddOwnedBook(int bookNum) {
			OwnedBook res = OwnedBooks.Where(b => b.BookNumber == bookNum).FirstOrDefault();
			if(res == null) {
				res = new OwnedBook(bookNum, this);
				OwnedBooks.Add(res);
				FirePropertyChangedEvent(nameof(OwnedBooks));
			}
			return res;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void FirePropertyChangedEvent(string pName) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
	}
}