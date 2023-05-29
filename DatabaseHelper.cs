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
            insertCommand.CommandText = "CREATE TABLE article_authors (article_id INTEGER, author_id INTEGER, FOREIGN KEY (article_id) REFERENCES articles(id), FOREIGN KEY (author_id) REFERENCES authors(id), PRIMARY KEY (article_id, author_id))";

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
            insertCommand.CommandText = "CREATE TABLE providers_tags (provider_id INTEGER, tag_id INTEGER, FOREIGN KEY (provider_id) REFERENCES providers(id), FOREIGN KEY (tag_id) REFERENCES tags(id), PRIMARY KEY (provider_id, tag_id))";

            insertCommand.ExecuteNonQuery();
        }
    }

    public void CreateInstitutionsTagsTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "CREATE TABLE institutions_tags (institution_id INTEGER, tag_id INTEGER, FOREIGN KEY (institution_id) REFERENCES institutions(id), FOREIGN KEY (tag_id) REFERENCES tags(id), PRIMARY KEY (institution_id, tag_id))";

            insertCommand.ExecuteNonQuery();
        }
    }

    public void DropExistingTables()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var dropTablesSql = @"
                DROP TABLE IF EXISTS providers;
                DROP TABLE IF EXISTS authors;
                DROP TABLE IF EXISTS institutions;
                DROP TABLE IF EXISTS article_authors;
                DROP TABLE IF EXISTS articles;
                DROP TABLE IF EXISTS resources;
                DROP TABLE IF EXISTS providers_tags;
                DROP TABLE IF EXISTS institutions_tags;
                DROP TABLE IF EXISTS tags;";
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
            var querySql = "SELECT * FROM institutions;";
            var command = connection.CreateCommand();
            command.CommandText = querySql;

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {

                    var articleAuthor = new Institution
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
                        Link = reader.GetString(9)
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

            var institutionsToInsert = new[]
      {
            new Institution { Id = 0, Name = "MD Anderson Cancer Center", Lab = "Msaouel Lab", Address = "1515 Holcombe Blvd", City = "Houston", State = "TX", Country = "United States", Postal = "77030", Image = "MDAnderson.png", Link = "https://www.mdanderson.org/research/" },
            new Institution { Id = 1, Name = "Brighams and Womens Hospital", Lab = "Henske Lab", Address = "20 Shattuck Street Thorn Building, (Elevator D) Room 826", City = "Boston", State = "MA", Country = "United States", Postal = "02115", Image = "Brigham.png", Link = "https://henskelab.bwh.harvard.edu/" },
            new Institution { Id = 2, Name = "National Cancer Institute", Lab = "Linehan Lab", Address = "10 Center Dr", City = "Bethesda", State = "MD", Country = "United States", Postal = "20892", Image = "NCI.png", Link = "https://ccr.cancer.gov/staff-directory/w-marston-linehan" },
            new Institution { Id = 3, Name = "Memorial Sloan Kettering Cancer Center", Lab = "", Address = "1275 York Ave", City = "New York", State = "NY", Country = "United States", Postal = "10065", Image = "sloan.png", Link = "https://ccr.cancer.gov/staff-directory/w-marston-linehan" },
        
            // Add other institutions here
        };

            foreach (var institution in institutionsToInsert)
            {
                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = @"
            INSERT INTO institutions (id, name, lab, address, city, state, country, postal, image, link)
            VALUES (@Id, @Name, @Lab, @Address, @City, @State, @Country, @Postal, @Image, @Link)";

                insertCommand.Parameters.AddWithValue("@Id", institution.Id);
                insertCommand.Parameters.AddWithValue("@Name", institution.Name);
                insertCommand.Parameters.AddWithValue("@Lab", institution.Lab);
                insertCommand.Parameters.AddWithValue("@Address", institution.Address);
                insertCommand.Parameters.AddWithValue("@City", institution.City);
                insertCommand.Parameters.AddWithValue("@State", institution.State);
                insertCommand.Parameters.AddWithValue("@Country", institution.Country);
                insertCommand.Parameters.AddWithValue("@Postal", institution.Postal);
                insertCommand.Parameters.AddWithValue("@Image", institution.Image);
                insertCommand.Parameters.AddWithValue("@Link", institution.Link);

                Console.WriteLine($"SQL Command Text: {insertCommand.CommandText}");
                Console.WriteLine($"Inserting institution: Id={institution.Id}, Name={institution.Name}, Lab={institution.Lab}, Address={institution.Address}, City={institution.City}, State={institution.State}, Country={institution.Country}, Postal={institution.Postal}, Image={institution.Image}, Link={institution.Link}");



                insertCommand.ExecuteNonQuery();
                //print what was inserted 
                Console.WriteLine($"Inserted {institution.Name}");

            }
        }
    }



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
            insertCommand.Parameters.AddWithValue("@title", article.Title);
            insertCommand.Parameters.AddWithValue("@date_published", article.DatePublished);
            insertCommand.Parameters.AddWithValue("@link", article.Link);
            insertCommand.Parameters.AddWithValue("@type", article.Type);

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
                        Title = title,
                        DatePublished = datePublished,
                        Link = link,
                        Type = type
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
