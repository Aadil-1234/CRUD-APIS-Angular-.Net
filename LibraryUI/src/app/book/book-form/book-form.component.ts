import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BookService } from '../../book.service';

@Component({
  selector: 'app-book-form',
  templateUrl: './book-form.component.html'
})
export class BookFormComponent implements OnInit {
  bookForm: FormGroup;
  isEditMode = false;
  bookId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private bookService: BookService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.bookForm = this.fb.group({
      title: ['', Validators.required],
      author: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.bookId = +id;
      // Note: getBook method should be added to BookService if needed
      // For now, we'll handle edit mode on form submission
    }
  }

  onSubmit(): void {
    if (this.bookForm.valid) {
      const bookData = this.bookForm.value;
      if (this.isEditMode && this.bookId) {
        bookData.id = this.bookId;
        // Note: updateBook method should be added to BookService
        // this.bookService.updateBook(this.bookId, bookData).subscribe(() => {
        //   this.router.navigate(['/books']);
        // });
        alert('Update functionality will be added once the backend API is updated');
      } else {
        this.bookService.addBook(bookData).subscribe(() => {
          this.router.navigate(['/books']);
        });
      }
    }
  }
}
