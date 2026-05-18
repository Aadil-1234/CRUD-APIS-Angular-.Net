import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { BookService } from '../book.service';
import { Book } from '../models/book.model';
import { Employee } from '../models/employee.model';
import { Client } from '../models/client.model';
import { Student } from '../models/student.model';
import { ClientListComponent } from '../client/client-list/client-list.component';
import { StudentListComponent } from '../student/student-list/student-list.component';
import { Subject, Subscription, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, catchError, finalize } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styles: [`
    .pagination-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 10px;
    }
    .loading-overlay {
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(255,255,255,0.7);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 10;
    }
    .sort-header {
      cursor: pointer;
      user-select: none;
      white-space: nowrap;
      display: flex;
      align-items: center;
      gap: 8px;
      transition: all 0.2s ease;
      padding: 6px 10px;
      border-radius: 4px;
    }
    .sort-header:hover {
      background-color: rgba(255,255,255,0.1);
    }
    .sort-header.active {
      color: #fff;
      background-color: rgba(255,255,255,0.15);
    }
    .sort-icons {
      display: flex;
      flex-direction: column;
      line-height: 0.6;
      font-size: 0.65rem;
      color: rgba(255,255,255,0.3);
      transition: color 0.2s;
    }
    .sort-header:hover .sort-icons {
      color: rgba(255,255,255,0.6);
    }
    .sort-header.active .sort-icons .active-sort {
      color: #0dcaf0; /* Cyan color for active sort */
      text-shadow: 0 0 5px rgba(13, 202, 240, 0.5);
    }
  `]
})
export class DashboardComponent implements OnInit, OnDestroy {
  protected readonly Math = Math;
  // Books Logic
  books: Book[] = [];
  newBook: Book = { title: '', author: '' };
  searchTerm: string = '';
  sortBy: string = '';
  
  // Pagination state
  currentPage: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  isLoading: boolean = false;
  jumpPage: number | null = null;

  private searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;
  private queryParamsSubscription?: Subscription;

  // Clients interaction
  selectedClient: Client | null = null;
  @ViewChild('clientList') clientList!: ClientListComponent;

  // Students interaction
  selectedStudent: Student | null = null;
  @ViewChild('studentList') studentList!: StudentListComponent;

  constructor(
    private bookService: BookService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Listen to query params for persistence
    this.queryParamsSubscription = this.route.queryParams.subscribe(params => {
      this.searchTerm = params['search'] || '';
      this.sortBy = params['sortBy'] || '';
      this.currentPage = +(params['page'] || 1);
      this.pageSize = +(params['pageSize'] || 10);
      this.loadBooks();
    });

    // Debounced search logic
    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      this.searchTerm = term;
      this.currentPage = 1;
      this.updateUrl();
    });
  }

  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe();
    this.queryParamsSubscription?.unsubscribe();
  }

  // Books Methods
  loadBooks(): void {
    this.isLoading = true;
    this.bookService.getBooks(this.searchTerm, this.sortBy, this.currentPage, this.pageSize)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (data) => {
          this.books = data.items || [];
          this.totalCount = data.totalCount || 0;
          this.totalPages = data.totalPages || 0;
          if (data.currentPage !== this.currentPage) {
            this.currentPage = data.currentPage || 1;
            this.updateUrl();
          }
          this.jumpPage = this.currentPage;
        },
        error: (err) => console.error('Error loading books', err)
      });
  }

  updateUrl(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        search: this.searchTerm || null,
        sortBy: this.sortBy || null,
        page: this.currentPage,
        pageSize: this.pageSize
      },
      queryParamsHandling: 'merge'
    });
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.updateUrl();
  }

  toggleSort(column: 'title' | 'author'): void {
    if (column === 'title') {
      this.sortBy = this.sortBy === 'az' ? 'za' : 'az';
    } else if (column === 'author') {
      this.sortBy = this.sortBy === 'author-az' ? 'author-za' : 'author-az';
    }
    this.onSortChange();
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.updateUrl();
    }
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.updateUrl();
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

  addBook(): void {
    if (!this.newBook.title || !this.newBook.author) return;
    this.bookService.addBook(this.newBook).subscribe({
      next: () => {
        this.loadBooks();
        this.newBook = { title: '', author: '' };
      },
      error: (err) => console.error('Error adding book', err)
    });
  }

  deleteBook(id: number | undefined): void {
    if (id !== undefined) {
      this.bookService.deleteBook(id).subscribe({
        next: () => this.loadBooks(),
        error: (err) => console.error('Error deleting book', err)
      });
    }
  }

  // Clients Methods
  onEditClient(client: Client): void {
    this.selectedClient = { ...client };
  }

  onClientSaved(): void {
    this.selectedClient = null;
    if (this.clientList) {
      this.clientList.loadClients();
    }
  }

  // Students Methods
  onEditStudent(student: Student): void {
    this.selectedStudent = { ...student };
  }

  onStudentSaved(): void {
    this.selectedStudent = null;
    if (this.studentList) {
      this.studentList.loadStudents();
    }
  }
}
