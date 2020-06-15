# Imagegram

Test project that was created for the BandLab challange. It provides a RESTful Web API for *Account-Post-Comment* domain operations.

##API endpoints

Full API documentation can be found at ***http(s)//[applicationUrl]/swagger*** after the application has been launched. There are latest implemented endpoints descriptions below: 

- **POST /accounts** - used to create a new account;
- **POST /posts** - used to create a post;
- **POST /posts/{postId}/comments** - used to create a post comment;
- **GET /posts** - used to retrieve all posts;
- **GET /posts/{postId}/comments** - used to retrieve all post comments;
- **DELETE /accounts/me** - used to delete currently authenticated used with all his/her posts and comments.

##Authentication

Authentication is performed using the value from the `X-Account-Id` HTTP request header. Example: `X-Account-Id: 00000000-0000-0000-0000-000000000000`.

##Structure

Project structure is the following:

- **Imagegram.Api** - contains source code of the application;
- **Imagegram.Database** - contains database-related scripts;
- **Imagegram.sln** - the solution file.

Business logic services are located in the *Imagegram.Api.Services* namespace.
Entity persistence is implemented using the *Repository* pattern and lightweight *Dapper* ORM. Queries were placed directly at the repositories for simplicity (it is only a test task, not an enterprise project).
In order to separate business logic layers *Controllers* use *Services* for the entity operations, not *Repositories* directly.
*File storage* and *Post* options can be configured via application settings.
The following design considerations were taken into account, but not implemented for simplicity reasons:
- type column definition provider for the SQL queries;
- SQL indices for the `[ItemCursor]` columns.

##Performance

The system was designed and tested to satisfy the following requirements:

- maximum repsonse time for any API call except file uploads: **50ms**;
- minimum throughput: **100 RPS**.

##License

This software is licensed under the MIT license. More details can be found in the *LICENSE.txt* file located at the repository root.

##Author

This software was developed by Denys Zosimovych.

- [LinkedIn](https://www.linkedin.com/in/denys-zosimovych-9633b311b/)
- [GitHub](https://github.com/unsinedZ)