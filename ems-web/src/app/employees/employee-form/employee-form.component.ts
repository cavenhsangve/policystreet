import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { EmployeeService } from '../../core/services/employee.service';
import { EmployeeDto } from '../../models/employee.model';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './employee-form.component.html'
})
export class EmployeeFormComponent implements OnInit
{
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private employeeService = inject(EmployeeService);

  isEditMode = false;
  employeeId: number | null = null;
  isLoading = false;
  isSaving = false;
  errorMessage: string | null = null;

  form = this.fb.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
    phone: ['', [Validators.maxLength(20), Validators.pattern(/^\+?[\d\s\-().]{7,20}$/)]],
    department: ['', [Validators.required, Validators.maxLength(100)]],
    position: ['', [Validators.required, Validators.maxLength(100)]],
    salary: [null as number | null, [Validators.required, Validators.min(0.01)]]
  });

  ngOnInit(): void
  {
    const id = this.route.snapshot.paramMap.get('id');
    if (id)
    {
      this.isEditMode = true;
      this.employeeId = +id;
      this.loadEmployee(this.employeeId);
    }
  }

  loadEmployee(id: number): void
  {
    this.isLoading = true;
    this.employeeService.getById(id).subscribe({
      next: (emp) =>
      {
        this.form.patchValue(emp);
        this.isLoading = false;
      },
      error: () =>
      {
        this.errorMessage = 'Failed to load employee.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void
  {
    if (this.form.invalid)
    {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = null;
    const dto = this.form.getRawValue() as EmployeeDto;

    const request$ = this.isEditMode && this.employeeId
      ? this.employeeService.update(this.employeeId, dto)
      : this.employeeService.create(dto);

    request$.subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (err: HttpErrorResponse) =>
      {
        this.isSaving = false;
        if (err.status === 409)
        {
          this.errorMessage = 'An employee with this email already exists.';
        }
        else if (err.status === 400)
        {
          this.errorMessage = 'Please fix the validation errors and try again.';
        }
        else
        {
          this.errorMessage = 'An unexpected error occurred. Please try again.';
        }
      }
    });
  }

  onCancel(): void
  {
    this.router.navigate(['/employees']);
  }

  getError(field: string): string | null
  {
    const control = this.form.get(field);
    if (!control || !control.invalid || !control.touched)
    {
      return null;
    }
    if (control.hasError('required'))
    {
      return `${field} is required.`;
    }
    if (control.hasError('email'))
    {
      return 'Invalid email format.';
    }
    if (control.hasError('pattern'))
    {
      return 'Invalid phone format.';
    }
    if (control.hasError('maxlength'))
    {
      return 'Value is too long.';
    }
    if (control.hasError('min'))
    {
      return 'Salary must be greater than 0.';
    }
    return null;
  }
}
