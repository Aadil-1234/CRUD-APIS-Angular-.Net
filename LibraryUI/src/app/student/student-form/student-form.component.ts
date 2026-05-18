import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentService } from '../../student.service';
import { Student } from '../../models/student.model';

@Component({
  selector: 'app-student-form',
  templateUrl: './student-form.component.html'
})
export class StudentFormComponent implements OnInit {
  studentForm: FormGroup;
  isEditMode = false;
  studentId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private studentService: StudentService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.studentForm = this.fb.group({
      name: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      course: ['', Validators.required],
      department: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.studentId = +id;
      this.studentService.getStudent(this.studentId).subscribe((student: Student) => {
        this.studentForm.patchValue(student);
      });
    }
  }

  onSubmit(): void {
    if (!this.studentForm.valid) {
      return;
    }

    const studentData = this.studentForm.value as Student;

    if (this.isEditMode && this.studentId) {
      studentData.studentId = this.studentId;
      this.studentService.updateStudent(this.studentId, studentData).subscribe(() => {
        this.router.navigate(['/students']);
      });
    } else {
      this.studentService.addStudent(studentData).subscribe(() => {
        this.router.navigate(['/students']);
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/students']);
  }
}

