using Microsoft.Data.Sqlite;
using Chromophobe.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;


public class DatabaseHelper
{
    string connectionString = "Data Source=ChromophobeDatabase.db";

    public void CreateDatabase(string databaseName)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
        }
    }

    public void CreateArticlesTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE articles (id INTEGER PRIMARY KEY AUTOINCREMENT, title TEXT NOT NULL, headline TEXT, description TEXT, date_published DATE NOT NULL, link TEXT NOT NULL)";

            insertCommand.ExecuteNonQuery();
        }
    }

    public void CreateAuthorsTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE IF NOT EXISTS authors(id INTEGER PRIMARY KEY AUTOINCREMENT,first_name TEXT,last_name TEXT,designation TEXT,institution TEXT)";
            insertCommand.ExecuteNonQuery();
        }
    }

    public void CreateArticleAuthorsTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE article_authors (article_id INTEGER, author_id INTEGER, FOREIGN KEY (article_id) REFERENCES articles(id) ON DELETE CASCADE, FOREIGN KEY (author_id) REFERENCES authors(id) ON DELETE CASCADE, PRIMARY KEY (article_id, author_id))";

            insertCommand.ExecuteNonQuery();
        }
    }

    public void CreateResourcesTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE resources (id INTEGER PRIMARY KEY, title TEXT, description TEXT, type TEXT, link TEXT);";

            insertCommand.ExecuteNonQuery();
        }
    }


    public void CreateInstitutionsTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE institutions (id INTEGER, name TEXT NOT NULL, lab TEXT, address TEXT, city TEXT, state TEXT, country TEXT, postal TEXT, image TEXT, link TEXT);";

            insertCommand.ExecuteNonQuery();
        }
    }

    public void CreateProvidersTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE providers (id INTEGER PRIMARY KEY, first_name TEXT NOT NULL, last_name TEXT NOT NULL, designation TEXT, institution TEXT NOT NULL, address TEXT, city TEXT, state TEXT, country TEXT, postal TEXT, image TEXT, link TEXT);";

            insertCommand.ExecuteNonQuery();
        }
    }
    public void CreateTagsTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE tags (id INTEGER PRIMARY KEY, name TEXT NOT NULL);";

            insertCommand.ExecuteNonQuery();
        }
    }

    public void CreateProviderTagsTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE provider_tags (provider_id INTEGER, tag_id INTEGER, FOREIGN KEY (provider_id) REFERENCES providers(id) ON DELETE CASCADE, FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE, PRIMARY KEY (provider_id, tag_id))";

            insertCommand.ExecuteNonQuery();
        }
    }

    public void CreateInstitutionsTagsTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE institution_tags (institution_id INTEGER, tag_id INTEGER, FOREIGN KEY (institution_id) REFERENCES institutions(id) ON DELETE CASCADE, FOREIGN KEY (tag_id) REFERENCES tags(id)ON DELETE CASCADE, PRIMARY KEY (institution_id, tag_id))";

            insertCommand.ExecuteNonQuery();
        }
    }

    public void DropExistingTables()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var dropTablesSql = @"
            PRAGMA foreign_keys = 0;
                DROP TABLE IF EXISTS article_authors;
                DROP TABLE IF EXISTS institution_tags;
                DROP TABLE IF EXISTS provider_tags;
                DROP TABLE IF EXISTS providers;
                DROP TABLE IF EXISTS authors;
                DROP TABLE IF EXISTS institutions;
                DROP TABLE IF EXISTS articles;
                DROP TABLE IF EXISTS resources;
                DROP TABLE IF EXISTS tags;
                PRAGMA foreign_keys = 1;";
            var dropCommand = connection.CreateCommand();
            dropCommand.CommandText = dropTablesSql;
            dropCommand.ExecuteNonQuery();

        }
        Console.WriteLine("dropped tables");
    }

    public async Task GetArticlesAuthors()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            var querySql = "SELECT * FROM article_authors;";
            var command = connection.CreateCommand();
            command.CommandText = querySql;

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    // Print out the values of each column in the row
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader.GetName(i)}: {reader.GetValue(i)} ");
                    }
                    Console.WriteLine(); // Print a newline at the end of each row
                }
            }
        }
    }


    //     // public async Task<string> GetArticlesAuthors()
    //     {
    //         List<ArticleAuthor> articlesAuthors = new List<ArticleAuthor>();

    //         using (var connection = new SqliteConnection(connectionString))
    // {
    //     await connection.OpenAsync();
    //     var querySql = "SELECT * FROM articles_authors;";
    //     var command = connection.CreateCommand();
    //     command.CommandText = querySql;

    //     using (var reader = await command.ExecuteReaderAsync())
    //     {
    //         while (await reader.ReadAsync())
    //         {
    //             // Assuming ArticleAuthor has properties Id, ArticleId, and AuthorId
    //             var articleAuthor = new ArticleAuthor
    //             {
    //                 Id = reader.GetInt32(0),
    //                 ArticleId = reader.GetInt32(1),
    //                 AuthorId = reader.GetInt32(2),
    //             };

    //             articlesAuthors.Add(articleAuthor);
    //         }
    //     }
    // }

    // return JsonSerializer.Serialize(articlesAuthors);
    //     }


    public async Task<List<Institution>> GetInstitutions()
    {
        List<Institution> institutions = new List<Institution>();

        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            var querySql = @"SELECT institutions.id, institutions.name, institutions.lab, institutions.address, institutions.city, institutions.state, institutions.country, institutions.postal, institutions.image, institutions.link, GROUP_CONCAT(tags.name) AS tags
                FROM institutions
                LEFT JOIN institution_tags ON institutions.id = institution_tags.institution_id
                LEFT JOIN tags ON institution_tags.tag_id = tags.id
                GROUP BY institutions.id;";
            var command = connection.CreateCommand();
            command.CommandText = querySql;

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var tagsString = reader.IsDBNull(10) ? null : reader.GetString(10);
                    var tagsList = string.IsNullOrEmpty(tagsString) ? new List<string>() : new List<string>(tagsString.Split(','));
                    var institution = new Institution
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Lab = reader.GetString(2),
                        Address = reader.GetString(3),
                        City = reader.GetString(4),
                        State = reader.GetString(5),
                        Country = reader.GetString(6),
                        Postal = reader.GetString(7),
                        Image = reader.GetString(8),
                        Link = reader.GetString(9),
                        Tags = tagsList,
                    };

                    institutions.Add(institution);
                }
            }
        }

        return institutions;
    }


    public async Task<List<Provider>> GetProviders()
    {
        List<Provider> providers = new List<Provider>();

        using (var connection = new SqliteConnection(connectionString))
        {
            await connection.OpenAsync();
            var querySql = @"SELECT providers.id, providers.first_name, providers.last_name, providers.designation, providers.institution, providers.address, providers.city, providers.state, providers.country, providers.postal, providers.image, providers.link, GROUP_CONCAT(tags.name) AS tags
                FROM providers
                LEFT JOIN provider_tags ON providers.id = provider_tags.provider_id
                LEFT JOIN tags ON provider_tags.tag_id = tags.id
                GROUP BY providers.id;";
            var command = connection.CreateCommand();
            command.CommandText = querySql;

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {

                    var tagsString = reader.IsDBNull(12) ? null : reader.GetString(12);
                    var tagsList = string.IsNullOrEmpty(tagsString) ? new List<string>() : new List<string>(tagsString.Split(','));
                    var provider = new Provider
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Designation = reader.GetString(3),
                        Institution = reader.GetString(4),
                        Address = reader.GetString(5),
                        City = reader.GetString(6),
                        State = reader.GetString(7),
                        Country = reader.GetString(8),
                        Postal = reader.GetString(9),
                        Image = reader.GetString(10),
                        Link = reader.GetString(11),
                        Tags = tagsList,

                    };

                    providers.Add(provider);
                }
            }
        }

        return providers;
    }

    public void InsertAllProviders()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            try
            {
                // Insert institutions 
                InsertProviders(connection);

                // todo: Insert tags into the tags table
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }


    public void InsertAllInstitutions()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tagsToInsert = new List<string> { "fresh_tissue", "fixed_tissue", "trials", "rcc_specialist", "physician_scientist", "research" };

            try
            {
                // Insert tags into the tags table
                InsertTags(connection, tagsToInsert);

                // Insert institutions 
                InsertInstitutions(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private void InsertTags(SqliteConnection connection, List<string> tags)
    {
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var insertTagsCommand = connection.CreateCommand();
                insertTagsCommand.Transaction = transaction;

                foreach (var tag in tags)
                {
                    insertTagsCommand.CommandText = @"
                    INSERT INTO tags (name)
                    VALUES (@name)";
                    insertTagsCommand.Parameters.Clear();
                    insertTagsCommand.Parameters.AddWithValue("@name", tag);

                    Console.WriteLine(tag);
                    Console.WriteLine($"SQL Command Text: {insertTagsCommand.CommandText}");
                    insertTagsCommand.ExecuteNonQuery();
                }

                transaction.Commit(); // Commit the tags transaction
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                transaction.Rollback();
            }
        }
    }

    private void InsertInstitutions(SqliteConnection connection)
    {
        var institutionsToInsert = new[]
        {
            new Institution { Id = 1, Name = "MD Anderson Cancer Center", Lab = "Msaouel Lab", Address = "1515 Holcombe Blvd", City = "Houston", State = "TX", Country = "United States", Postal = "77030", Image = "MDAnderson.png", Link = "https://www.mdanderson.org/research/" },
            new Institution { Id = 2, Name = "Brighams and Womens Hospital", Lab = "Henske Lab", Address = "20 Shattuck Street Thorn Building, (Elevator D) Room 826", City = "Boston", State = "MA", Country = "United States", Postal = "02115", Image = "Brigham.png", Link = "https://henskelab.bwh.harvard.edu/" },
            new Institution { Id = 3, Name = "National Cancer Institute", Lab = "Linehan Lab", Address = "10 Center Dr", City = "Bethesda", State = "MD", Country = "United States", Postal = "20892", Image = "NCI.png", Link = "https://ccr.cancer.gov/staff-directory/w-marston-linehan" },
            new Institution { Id = 4, Name = "Memorial Sloan Kettering Cancer Center", Lab = "", Address = "1275 York Ave", City = "New York", State = "NY", Country = "United States", Postal = "10065", Image = "sloan.png", Link = "https://ccr.cancer.gov/staff-directory/w-marston-linehan" }
    };

        foreach (var institution in institutionsToInsert)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
            INSERT INTO institutions (id, name, lab, address, city, state, country, postal, image, link)
            VALUES (@id, @name, @lab, @address, @city, @state, @country, @postal, @image, @link)";

            insertCommand.Parameters.AddWithValue("@id", institution.Id);
            insertCommand.Parameters.AddWithValue("@name", institution.Name);
            insertCommand.Parameters.AddWithValue("@lab", institution.Lab);
            insertCommand.Parameters.AddWithValue("@address", institution.Address);
            insertCommand.Parameters.AddWithValue("@city", institution.City);
            insertCommand.Parameters.AddWithValue("@state", institution.State);
            insertCommand.Parameters.AddWithValue("@country", institution.Country);
            insertCommand.Parameters.AddWithValue("@postal", institution.Postal);
            insertCommand.Parameters.AddWithValue("@image", institution.Image);
            insertCommand.Parameters.AddWithValue("@link", institution.Link);
            Console.WriteLine($"SQL Command Text: {insertCommand.CommandText}");
            insertCommand.ExecuteNonQuery();
        }
    }

    private void InsertProviders(SqliteConnection connection)
    {
        var providersToInsert = new List<Provider>
        {
        new Provider { Id = 1, FirstName = "Pavlos", LastName = "Msaouel", Designation = "MD PhD", Institution = "MD Anderson Cancer Center", Address = "1515 Holcombe Blvd", City = "Houston", State = "TX", Country = "United States", Postal = "77030", Image = "msaouel.png", Link = "https://faculty.mdanderson.org/profiles/pavlos_msaouel.html"},
        new Provider { Id = 2, FirstName = "Andrew", LastName = "Hahn", Designation = "MD", Institution = "MD Anderson Cancer Center", Address = "1515 Holcombe Blvd", City = "Houston", State = "TX", Country = "United States", Postal = "77030", Image = "andrewHahn.png", Link = "https://faculty.mdanderson.org/profiles/andrew_hahn.html"},
        new Provider { Id = 4, FirstName = "Martin", LastName = "Voss", Designation = "MD", Institution = "Memorial Sloan Kettering Cancer Center", Address = "1275 York Ave", City = "New York", State = "NY", Country = "United States", Postal = "10065", Image = "voss.png", Link = "https://www.mskcc.org/cancer-care/doctors/martin-voss" },
        new Provider { Id = 5, FirstName = "Lisa", LastName = "Henske", Designation = "MD PhD", Institution = "Dana Farber Cancer Institute", Address = "450 Brookline Ave", City = "Boston", State = "NY", Country = "United States", Postal = "10065", Image = "henske.png", Link = "https://www.dana-farber.org/find-a-doctor/elizabeth-p-henske/" },
        };

        foreach (var provider in providersToInsert)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
            INSERT INTO providers (id, first_name, last_name, designation, institution, address, city, state, country, postal, image, link)
            VALUES (@id, @firstName, @lastName, @designation, @institution, @address, @city, @state, @country, @postal, @image, @link)";

            insertCommand.Parameters.AddWithValue("@id", provider.Id);
            insertCommand.Parameters.AddWithValue("@firstName", provider.FirstName);
            insertCommand.Parameters.AddWithValue("@lastName", provider.LastName);
            insertCommand.Parameters.AddWithValue("@designation", provider.Designation);
            insertCommand.Parameters.AddWithValue("@institution", provider.Institution);
            insertCommand.Parameters.AddWithValue("@address", provider.Address);
            insertCommand.Parameters.AddWithValue("@city", provider.City);
            insertCommand.Parameters.AddWithValue("@state", provider.State);
            insertCommand.Parameters.AddWithValue("@country", provider.Country);
            insertCommand.Parameters.AddWithValue("@postal", provider.Postal);
            insertCommand.Parameters.AddWithValue("@image", provider.Image);
            insertCommand.Parameters.AddWithValue("@link", provider.Link);

            Console.WriteLine($"SQL Command Text: {insertCommand.CommandText}");
            insertCommand.ExecuteNonQuery();
        }

    }



    //this needs to be revised, ending up with foreign key mismatches
    // public void InsertInstitutionTags()
    // {
    //     using (var connection = new SqliteConnection(connectionString))
    //     {
    //         connection.Open();

    //         var institutionTagsToInsert = new[]
    //         {
    //         new { InstitutionId = 0, TagId = 1 },
    //         // new { InstitutionId = 0, TagId = 2 },
    //         // new { InstitutionId = 0, TagId = 3 },
    //         // new { InstitutionId = 1, TagId = 1 }
    //     };

    //         using (var transaction = connection.BeginTransaction())
    //         {
    //             try
    //             {
    //                 foreach (var institutionTag in institutionTagsToInsert)
    //                 {
    //                     var insertCommand = connection.CreateCommand();
    //                     insertCommand.Transaction = transaction;
    //                     insertCommand.CommandText = @"
    //                     INSERT INTO institution_tags (institution_id, tag_id)
    //                     VALUES (@institutionId, @tagId)";
    //                     insertCommand.Parameters.AddWithValue("@institutionId", institutionTag.InstitutionId);
    //                     insertCommand.Parameters.AddWithValue("@tagId", institutionTag.TagId);
    //                     insertCommand.ExecuteNonQuery();
    //                 }

    //                 transaction.Commit();
    //                 Console.WriteLine("Institution tags inserted successfully.");
    //             }
    //             catch (Exception ex)
    //             {
    //                 Console.WriteLine($"Error: {ex.Message}");
    //                 transaction.Rollback();
    //             }
    //         }
    //     }
    // }


    public void InsertArticle(Article article)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO articles (title, date_published, link, type) VALUES (@title, @date_published, @link, @type)";
            insertCommand.Parameters.AddWithValue("@title", article.title);
            insertCommand.Parameters.AddWithValue("@date_published", article.datePublished);
            insertCommand.Parameters.AddWithValue("@link", article.link);
            insertCommand.Parameters.AddWithValue("@type", article.type);

            insertCommand.ExecuteNonQuery();
        }
    }
    public void CreateCustomer(string firstName, string lastName)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Customers (FirstName, LastName) VALUES (@FirstName, @LastName)";
            insertCommand.Parameters.AddWithValue("@FirstName", firstName);
            insertCommand.Parameters.AddWithValue("@LastName", lastName);

            insertCommand.ExecuteNonQuery();
        }
    }

    public string PrintCustomers()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM Articles";
            List<Article> articles = new List<Article>();

            using (var reader = selectCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    string title = reader.GetString(1);
                    DateTime datePublished = reader.GetDateTime(2);
                    string link = reader.GetString(3);
                    string type = reader.GetString(4);

                    Article article = new Article
                    {
                        title = title,
                        datePublished = datePublished,
                        link = link,
                        type = type
                    };

                    articles.Add(article);
                }
            }

            string json = JsonSerializer.Serialize(articles);

            return json;
        }
    }
}

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && reader.GetString() is string dateString)
        {
            if (DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
        }

        throw new JsonException($"Unable to parse date in the format '{DateFormat}'.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
