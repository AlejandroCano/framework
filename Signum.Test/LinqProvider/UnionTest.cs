using Signum.Engine.Maps;

namespace Signum.Test.LinqProvider;

public class UnionTest
{
    public UnionTest()
    {
        MusicStarter.StartAndLoad();
        Connector.CurrentLogger = new DebugTextWriter();
    }

    static void AssertUnion<T>(IQueryable<T> first, IQueryable<T> second)
    {
        var expected = first.ToList().Union(second.ToList()).ToList();
        var query = first.Union(second);
        Assert.Contains("UNION", query.QueryText());
        var actual = query.ToList();
        Assert.Equal(expected.Count, actual.Count);
        Assert.All(expected, value => Assert.Contains(value, actual));
    }

    [Fact]
    public void IntegersWithOverlap()
    {
        var years = Database.Query<AlbumEntity>().Select(a => a.Year);
        AssertUnion(years.Where(y => y < 2000), years.Where(y => y > 1990));
    }

    [Fact]
    public void StringsFromDifferentColumns()
    {
        AssertUnion(Database.Query<ArtistEntity>().Select(a => a.Name),
            Database.Query<AlbumEntity>().Select(a => a.Label.Name));
    }

    [Fact]
    public void ComputedScalars()
    {
        AssertUnion(Database.Query<ArtistEntity>().Select(a => a.Name.Length + 1),
            Database.Query<AlbumEntity>().Select(a => a.Year % 10));
    }

    [Fact]
    public void ConstantsRemainDistinct()
    {
        var artists = Database.Query<ArtistEntity>();
        var query = artists.Select(a => 1).Union(artists.Select(a => 2));
        Assert.Equal(new[] { 1, 2 }, query.OrderBy(a => a).ToArray());
        Assert.Equal(2, query.Count());
        Assert.Equal(2, query.Select(a => 0).Count());
    }

    [Fact]
    public void EmptyBranches()
    {
        var years = Database.Query<AlbumEntity>().Select(a => a.Year);
        AssertUnion(years.Where(y => false), years);
        AssertUnion(years, years.Where(y => false));
        Assert.Empty(years.Where(y => false).Union(years.Where(y => false)).ToList());
        Assert.False(years.Where(y => false).Union(years.Where(y => false)).Any());
    }

    [Fact]
    public void Enums()
    {
        var sexes = Database.Query<ArtistEntity>().Select(a => a.Sex);
        AssertUnion(sexes, sexes);
    }

    [Fact]
    public void NullableEnums()
    {
        var artists = Database.Query<ArtistEntity>();
        AssertUnion(artists.Select(a => a.Status), artists.Select(a => (Status?)null));
    }

    [Fact]
    public void NullableScalars()
    {
        var albums = Database.Query<AlbumEntity>();
        var query = albums.Select(a => (int?)a.Year).Union(albums.Select(a => (int?)null));
        AssertUnion(albums.Select(a => (int?)a.Year), albums.Select(a => (int?)null));
        Assert.Single(query.Where(y => y == null).ToList());
    }

    [Fact]
    public void Booleans()
    {
        AssertUnion(Database.Query<ArtistEntity>().Select(a => a.Dead),
            Database.Query<AlbumEntity>().Select(a => a.Year > 2000));
    }

    [Fact]
    public void Dates()
    {
        var notes = Database.Query<NoteWithDateEntity>();
        AssertUnion(notes.Select(a => a.CreationTime), notes.Select(a => a.CreationTime));
        AssertUnion(notes.Select(a => a.ReleaseDate), notes.Select(a => (DateOnly?)a.CreationDate));
    }

    [Fact]
    public void PrimaryKeys()
    {
        var artists = Database.Query<ArtistEntity>();
        AssertUnion(artists.Where(a => a.Dead).Select(a => a.Id), artists.Select(a => a.Id));
    }

    [Fact]
    public void GuidPrimaryKeys()
    {
        var ids = Database.Query<NoteWithDateEntity>().Select(a => a.Id);
        AssertUnion(ids, ids);
    }

    [Fact]
    public void NullablePrimaryKeys()
    {
        var artists = Database.Query<ArtistEntity>();
        AssertUnion(artists.Select(a => (PrimaryKey?)a.Id), artists.Select(a => (PrimaryKey?)a.Id));

        var ids = artists.Select(a => (PrimaryKey?)a.Id);
        var nulls = artists.Select(a => (PrimaryKey?)null);
        AssertUnion(ids, nulls);
        AssertUnion(nulls, ids);
        Assert.Single(nulls.Union(nulls).ToList());
        Assert.Null(nulls.Union(nulls).Single());
    }

    [Fact]
    public void NullableGuidPrimaryKeys()
    {
        var notes = Database.Query<NoteWithDateEntity>();
        var ids = notes.Select(a => (PrimaryKey?)a.Id);
        var nulls = notes.Select(a => (PrimaryKey?)null);
        AssertUnion(ids, nulls);
        AssertUnion(nulls, ids);
    }

    [Fact]
    public void DecimalAndGuidScalars()
    {
        var years = Database.Query<AlbumEntity>().Select(a => (decimal)a.Year / 10);
        AssertUnion(years, years);
        var guids = Database.Query<NoteWithDateEntity>().Select(a => (Guid)a.Id.Object);
        AssertUnion(guids, guids);
    }

    [Fact]
    public void PrimaryKeyContainsSubquery()
    {
        var artists = Database.Query<ArtistEntity>();
        var ids = artists.Where(a => a.Dead).Select(a => a.Id)
            .Union(artists.Where(a => a.Sex == Sex.Female).Select(a => a.Id));
        var actual = artists.Where(a => ids.Contains(a.Id)).Select(a => a.Id).ToList();
        var expected = artists.Where(a => a.Dead || a.Sex == Sex.Female).Select(a => a.Id).ToList();
        Assert.Equal(expected.OrderBy(a => a), actual.OrderBy(a => a));
    }

    [Fact]
    public void GuidPrimaryKeyContainsSubquery()
    {
        var notes = Database.Query<NoteWithDateEntity>();
        var ids = notes.Select(a => a.Id).Union(notes.Where(a => false).Select(a => a.Id));
        Assert.Equal(notes.Count(), notes.Count(a => ids.Contains(a.Id)));
    }

    [Fact]
    public void WhereSelectAnyCountAndContains()
    {
        var years = Database.Query<AlbumEntity>().Select(a => a.Year);
        var query = years.Union(years.Where(y => y > 1990)).Where(y => y > 1980).Select(y => y + 1);
        var expected = years.ToList().Distinct().Where(y => y > 1980).Select(y => y + 1).ToList();
        Assert.NotEmpty(expected);
        Assert.Equal(expected.OrderBy(y => y), query.OrderBy(y => y).ToList());
        Assert.Equal(expected.Count, query.Count());
        Assert.True(query.Any(y => y == expected[0]));
        Assert.True(query.Contains(expected[0]));
        Assert.False(query.Contains(-1));
    }

    [Fact]
    public void DiscardedColumnStillDeterminesEquality()
    {
        var ids = Database.Query<ArtistEntity>().Select(a => a.Id);
        Assert.True(ids.Count() > 1);
        Assert.Equal(ids.Count(), ids.Union(ids).Select(a => 0).Count());
        Assert.Equal(ids.Count(), ids.Union(ids).Select(a => 0).ToList().Count);
    }

    [Fact]
    public void OrderedBranchesWithTake()
    {
        var years = Database.Query<AlbumEntity>().Select(a => a.Year);
        var first = years.OrderBy(y => y).Take(2);
        var second = years.OrderByDescending(y => y).Take(3);
        var expected = first.ToList().Union(second.ToList()).OrderBy(y => y).Take(3).ToList();
        Assert.Equal(expected, first.Union(second).OrderBy(y => y).Take(3).ToList());
    }

    [Fact]
    public void ChainedUnions()
    {
        var artists = Database.Query<ArtistEntity>();
        var query = artists.Select(a => 1).Union(artists.Select(a => 2)).Union(artists.Select(a => 3));
        Assert.Equal(new[] { 1, 2, 3 }, query.OrderBy(a => a).ToArray());
        Assert.Equal(3, query.Count());
    }

    [Fact]
    public void CorrelatedContains()
    {
        var artists = Database.Query<ArtistEntity>();
        var query = artists.Where(a => artists.Where(b => b.Id == a.Id).Select(b => b.Name)
            .Union(artists.Where(b => b.Dead && b.Id == a.Id).Select(b => b.Name))
            .Contains(a.Name));
        Assert.Equal(artists.Count(), query.Count());
    }

    [Fact]
    public void FiltersApplyToBothBranches()
    {
        var expected = Database.Query<ArtistEntity>().Where(a => a.Sex == Sex.Female).Select(a => a.Id).ToList();
        FilterQueryResult<ArtistEntity>? Filter(FilterQueryArgs args) => new(a => a.Sex == Sex.Female, null);
        try
        {
            Schema.Current.EntityEvents<ArtistEntity>().FilterQuery += Filter;
            var artists = Database.Query<ArtistEntity>();
            var actual = artists.Where(a => a.Dead).Select(a => a.Id)
                .Union(artists.Where(a => !a.Dead).Select(a => a.Id)).ToList();
            Assert.Equal(expected.OrderBy(a => a), actual.OrderBy(a => a));
        }
        finally
        {
            Schema.Current.EntityEvents<ArtistEntity>().FilterQuery -= Filter;
        }
    }

    [Fact]
    public void RejectComparer()
    {
        var names = Database.Query<ArtistEntity>().Select(a => a.Name);
        var ex = Assert.Throws<NotSupportedException>(() => names.Union(names, StringComparer.OrdinalIgnoreCase).ToList());
        Assert.Contains("comparer", ex.Message);
    }

    [Fact]
    public void RejectEntitiesLitesAndAnonymousProjections()
    {
        var artists = Database.Query<ArtistEntity>();
        Assert.Throws<NotSupportedException>(() => artists.Union(artists).ToList());
        var lites = artists.Select(a => a.ToLite());
        Assert.Throws<NotSupportedException>(() => lites.Union(lites).ToList());
        var pairs = artists.Select(a => new { a.Id, a.Name });
        Assert.Throws<NotSupportedException>(() => pairs.Union(pairs).ToList());
    }

    [Fact]
    public void RejectDifferentPrimaryKeyTypes()
    {
        var ints = Database.Query<ArtistEntity>().Select(a => a.Id);
        var guids = Database.Query<NoteWithDateEntity>().Select(a => a.Id);
        var ex = Assert.Throws<NotSupportedException>(() => ints.Union(guids).ToList());
        Assert.Contains("underlying", ex.Message);
    }
}
