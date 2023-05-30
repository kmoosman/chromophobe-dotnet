# Summary

This project is a .NET application that serves up data on a rare cancer to create a super resource/collection. The initial implementation focuses on providing data for chromophobe kidney cancer but can be updated to support other types of cancers. This application was developed as a trial project to learn a new language, so it may not be in a complete state, and additional routes need to be added.

## Getting Started

To start the project, follow these steps:

1. Run `dotnet build` to build the project.
2. Run `dotnet run` to start the application.

Note: The application uses SQLite without migrations. The database is intended to be used locally, and for a production application, a more robust option should be implemented

## Seed Data Population

To populate the seed data, follow these steps:

1. After starting the application, run the following command in your terminal or command prompt:
   `curl -X POST -H "Content-Type: application/json" http://localhost:5194/createWorld`
   This will create all the necessary tables and seed them with most of the data. However, please note that the tag inserts were not implemented, so you'll have to add them manually.

2. To manually add the recommended inserts, access the SQLite console by running the following command:
   `sqlite3 ChromophobeDatabase.db`

Once inside the console, execute the following SQL inserts:

````INSERT INTO institution_tags (institution_id, tag_id) VALUES(1, 1);
INSERT INTO institution_tags (institution_id, tag_id) VALUES(1, 2);
INSERT INTO institution_tags (institution_id, tag_id) VALUES(1, 3);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(1, 4);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(1, 5);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(1, 6);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(2, 4);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(3, 4);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(3, 5);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(4, 4);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(4, 5);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(4, 6);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(5, 4);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(5, 5);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(6, 4);
INSERT INTO provider_tags (provider_id, tag_id) VALUES(7, 4);```


## Resetting the Database

If you need to revert the database back to its initial state, you can run the following command:

`curl -X POST -H "Content-Type: application/json" http://localhost:5194/drop`


**CAUTION:** Please exercise caution when using this command, as it will drop all the tables in the database. Remember that these instructions are intended for local usage, and appropriate changes must be made for any production applications.

Please refer to the documentation or consult with the development team for any further assistance or modifications needed for specific deployment scenarios.


````
