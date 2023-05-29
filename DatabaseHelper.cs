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
            insertCommand.CommandText = "CREATE TABLE providers_tags (provider_id INTEGER, tag_id INTEGER, FOREIGN KEY (provider_id) REFERENCES providers(id) ON DELETE CASCADE, FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE, PRIMARY KEY (provider_id, tag_id))";

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
               DROP TABLE IF EXISTS providers_tags;
                DROP TABLE IF EXISTS article_authors;
                DROP TABLE IF EXISTS institution_tags;
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
                    var tagsString = reader.IsDBNull(10) ? "" : reader.GetString(10);
                    var tagsList = new List<string>(tagsString.Split(','));
                    var articleAuthor = new Institution
                    {
                        id = reader.GetInt32(0),
                        name = reader.GetString(1),
                        lab = reader.GetString(2),
                        address = reader.GetString(3),
                        city = reader.GetString(4),
                        state = reader.GetString(5),
                        country = reader.GetString(6),
                        postal = reader.GetString(7),
                        image = reader.GetString(8),
                        link = reader.GetString(9),
                        tags = tagsList,
                    };

                    institutions.Add(articleAuthor);
                }
            }
        }

        return institutions;
    }




    public void InsertAllInstitutions()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var tagsToInsert = new List<string> { "fresh_tissue", "fixed_tissue", "trials" };

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
        new Institution { id = 0, name = "MD Anderson Cancer Center", lab = "Msaouel Lab", address = "1515 Holcombe Blvd", city = "Houston", state = "TX", country = "United States", postal = "77030", image = "MDAnderson.png", link = "https://www.mdanderson.org/research/" },
        new Institution { id = 1, name = "Brighams and Womens Hospital", lab = "Henske Lab", address = "20 Shattuck Street Thorn Building, (Elevator D) Room 826", city = "Boston", state = "MA", country = "United States", postal = "02115", image = "Brigham.png", link = "https://henskelab.bwh.harvard.edu/" },
        new Institution { id = 2, name = "National Cancer Institute", lab = "Linehan Lab", address = "10 Center Dr", city = "Bethesda", state = "MD", country = "United States", postal = "20892", image = "NCI.png", link = "https://ccr.cancer.gov/staff-directory/w-marston-linehan" },
        new Institution { id = 3, name = "Memorial Sloan Kettering Cancer Center", lab = "", address = "1275 York Ave", city = "New York", state = "NY", country = "United States", postal = "10065", image = "sloan.png", link = "https://ccr.cancer.gov/staff-directory/w-marston-linehan" },
    };

        foreach (var institution in institutionsToInsert)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
            INSERT INTO institutions (id, name, lab, address, city, state, country, postal, image, link)
            VALUES (@id, @name, @lab, @address, @city, @state, @country, @postal, @image, @link)";

            insertCommand.Parameters.AddWithValue("@id", institution.id);
            insertCommand.Parameters.AddWithValue("@name", institution.name);
            insertCommand.Parameters.AddWithValue("@lab", institution.lab);
            insertCommand.Parameters.AddWithValue("@address", institution.address);
            insertCommand.Parameters.AddWithValue("@city", institution.city);
            insertCommand.Parameters.AddWithValue("@state", institution.state);
            insertCommand.Parameters.AddWithValue("@country", institution.country);
            insertCommand.Parameters.AddWithValue("@postal", institution.postal);
            insertCommand.Parameters.AddWithValue("@image", institution.image);
            insertCommand.Parameters.AddWithValue("@link", institution.link);

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



    public void InsertCustomer(string firstName, string lastName)
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
