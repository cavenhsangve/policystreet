import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EmployeeService, PagedResult } from '../../core/services/employee.service';
import { Employee } from '../../models/employee.model';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './employee-list.component.html'
})
export class EmployeeListComponent implements OnInit
{
  private employeeService = inject(EmployeeService);
  private router = inject(Router);

  employees: Employee[] = [];
  isLoading = false;
  errorMessage: string | null = null;
  page = 1;
  pageSize = 10;
  total = 0;

  ngOnInit(): void
  {
    this.loadEmployees();
  }

  loadEmployees(): void
  {
    this.isLoading = true;
    this.errorMessage = null;
    this.employeeService.getAll(this.page, this.pageSize).subscribe({
      next: (result: PagedResult<Employee>) =>
      {
        this.employees = result.data;
        this.total = result.total;
        this.isLoading = false;
      },
      error: () =>
      {
        this.errorMessage = 'Failed to load employees. Please try again.';
        this.isLoading = false;
      }
    });
  }

  onAdd(): void
  {
    this.router.navigate(['/employees/new']);
  }

  onEdit(id: number): void
  {
    this.router.navigate(['/employees', id, 'edit']);
  }

  onDelete(id: number): void
  {
    if (!confirm('Are you sure you want to delete this employee?'))
    {
      return;
    }
    this.employeeService.delete(id).subscribe({
      next: () => this.loadEmployees(),
      error: () =>
      {
        this.errorMessage = 'Failed to delete employee. Please try again.';
      }
    });
  }

  get totalPages(): number
  {
    return Math.ceil(this.total / this.pageSize);
  }

  onPageChange(newPage: number): void
  {
    this.page = newPage;
    this.loadEmployees();
  }
}
