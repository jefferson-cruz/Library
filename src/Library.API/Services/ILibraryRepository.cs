using Library.API.Entities;
using System;
using System.Collections.Generic;

namespace Library.API.Services
{
    public interface ILibraryRepository
    {
        IEnumerable<Author> GetAuthors();
        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        void UpdateAuthor(Author author);
        bool AuthorExists(Guid authorId);
        IEnumerable<Book> GetBooksForAuthor(Guid authorId);
        Book GetBookForAuthor(Guid authorId, Guid bookId);
        void AddBookForAuthor(Guid authorId, Book book);
        void UpdateBookForAuthor(Book book);
        void DeleteBook(Book book);
        IEnumerable<AuthorContact> GetContactsForAuthor(Guid authorId);
        AuthorContact GetContactForAuthor(Guid authorId, Guid authorContactId);
        void AddContactForAuthor(Guid authorId, AuthorContact contact);
        void UpdateContactForAuthor(AuthorContact contact);
        void DeleteContact(AuthorContact book);
        bool Save();
    }
}
