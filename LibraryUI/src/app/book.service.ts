import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Book } from './models/book.model';
import { PaginatedResult } from './models/paginated-result.model';

@Injectable({ providedIn: 'root' })
export class BookService {
  // IMPORTANT: Check your backend terminal for the port number (usually 5000, 5001, 7000, or 7100)
  private apiUrl = 'http://localhost:5112/api/books'; 

  constructor(private http: HttpClient) { }

  getBooks(search: string = '', sortBy: string = '', page: number = 1, pageSize: number = 10): Observable<PaginatedResult<Book>> { 
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (sortBy) params = params.set('sortBy', sortBy);
    params = params.set('page', page.toString());
    params = params.set('pageSize', pageSize.toString());

    return this.http.get<PaginatedResult<Book>>(this.apiUrl, { params }); 
  }

  getBook(id: number): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/${id}`);
  }

  addBook(book: Book): Observable<Book> { 
    return this.http.post<Book>(this.apiUrl, book); 
  }

  updateBook(id: number, book: Book): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, book);
  }

  deleteBook(id: number): Observable<void> { 
    return this.http.delete<void>(`${this.apiUrl}/${id}`); 
  }
}
