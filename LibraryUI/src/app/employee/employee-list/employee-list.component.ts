import { Component, OnInit } from '@angular/core';
import { EmployeeService } from '../../employee.service';
import { Employee } from '../../models/employee.model';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css']
})
export class EmployeeListComponent implements OnInit {
  protected readonly Math = Math;
  employees: Employee[] = [];

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

  constructor(private employeeService: EmployeeService) { }

  ngOnInit(): void {
    this.loadEmployees();

    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      this.searchTerm = term;
      this.currentPage = 1;
      this.loadEmployees();
    });
  }

  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe();
  }

  loadEmployees(): void {
    this.isLoading = true;
    this.employeeService.getEmployees(this.searchTerm, this.sortBy, this.currentPage, this.pageSize)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (data) => {
          this.employees = data.items || [];
          this.totalCount = data.totalCount || 0;
          this.totalPages = data.totalPages || 0;
          this.currentPage = data.currentPage || 1;
          this.jumpPage = this.currentPage;
        },
        error: (err) => console.error('Error loading employees', err)
      });
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.loadEmployees();
  }

  toggleSort(column: 'name' | 'dept'): void {
    if (column === 'name') {
      this.sortBy = this.sortBy === 'name-az' ? 'name-za' : 'name-az';
    } else if (column === 'dept') {
      this.sortBy = this.sortBy === 'dept-az' ? 'dept-za' : 'dept-az';
    }
    this.onSortChange();
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.loadEmployees();
    }
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadEmployees();
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

  deleteEmployee(id: number): void {
    if (confirm('Are you sure you want to delete this employee?')) {
      this.employeeService.deleteEmployee(id).subscribe(() => {
        this.loadEmployees();
      });
    }
  }
}
