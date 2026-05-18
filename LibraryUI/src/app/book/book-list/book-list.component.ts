import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BookService } from '../../book.service';
import { Book } from '../../models/book.model';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit {
  protected readonly Math = Math;
  books: Book[] = [];

  // Pagination & Search state
  searchTerm: string = '';
  sortBy: string = '';
  currentPage: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  isLoading: boolean = false;
  jumpPage: number | null = null;

  private searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;

  constructor(private bookService: BookService, private router: Router) { }

  ngOnInit(): void {
    this.loadBooks();

    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      this.searchTerm = term;
      this.currentPage = 1;
      this.loadBooks();
    });
  }

  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe();
  }

  loadBooks(): void {
    this.isLoading = true;
    this.bookService.getBooks(this.searchTerm, this.sortBy, this.currentPage, this.pageSize)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (data) => {
          this.books = data.items || [];
          this.totalCount = data.totalCount || 0;
          this.totalPages = data.totalPages || 0;
          this.currentPage = data.currentPage || 1;
          this.jumpPage = this.currentPage;
        },
        error: (err) => console.error('Error loading books', err)
      });
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.loadBooks();
  }

  toggleSort(column: 'title' | 'author'): void {
    if (column === 'title') {
      this.sortBy = this.sortBy === 'title-az' ? 'title-za' : 'title-az';
    } else if (column === 'author') {
      this.sortBy = this.sortBy === 'author-az' ? 'author-za' : 'author-az';
    }
    this.onSortChange();
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.loadBooks();
    }
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadBooks();
  }

  onJumpPage(): void {
    if (this.jumpPage && this.jumpPage >= 1 && this.jumpPage <= this.totalPages) {
      this.onPageChange(this.jumpPage);
    } else {
      this.jumpPage = this.currentPage;
    }
  }

  get visiblePages(): number[] {
    const pages: number[] = [];
    const maxVisible = 5;
    let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
    let end = Math.min(this.totalPages, start + maxVisible - 1);

    if (end - start + 1 < maxVisible) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }

  deleteBook(id: number): void {
    if (confirm('Are you sure you want to delete this book?')) {
      this.bookService.deleteBook(id).subscribe(() => {
        this.loadBooks();
      });
    }
  }

  editBook(id: number | undefined): void {
    if (id !== undefined) {
      this.router.navigate(['/books/edit', id]);
    }
  }
}
